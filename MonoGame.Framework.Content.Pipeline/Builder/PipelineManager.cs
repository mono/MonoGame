// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using Microsoft.Xna.Framework.Content.Pipeline.Builder.Convertors;
using System.Diagnostics;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    /// <summary>
    /// Class to provide methods and properties for handling content pipelines.
    /// </summary>
    public class PipelineManager
    {
        [DebuggerDisplay("ImporterInfo: {type.Name}")]
        private struct ImporterInfo
        {
            public ContentImporterAttribute attribute;
            public Type type;
            public DateTime assemblyTimestamp;
        };

        private List<ImporterInfo> _importers;

        [DebuggerDisplay("ProcessorInfo: {type.Name}")]
        private struct ProcessorInfo
        {
            public ContentProcessorAttribute attribute;
            public Type type;
            public DateTime assemblyTimestamp;
        };

        private List<ProcessorInfo> _processors;

        private List<Type> _writers;

        // Keep track of all built assets. (Required to resolve automatic names "AssetName_n".)
        //   Key = absolute, normalized path of source file
        //   Value = list of build events
        // (Note: When using external references, an asset may be built multiple times
        // with different parameters.)
        private readonly Dictionary<string, List<PipelineBuildEvent>> _pipelineBuildEvents;

        // Store default values for content processor parameters. (Necessary to compare processor
        // parameters. See PipelineBuildEvent.AreParametersEqual.)
        //   Key = name of content processor
        //   Value = processor parameters
        private readonly Dictionary<string, OpaqueDataDictionary> _processorDefaultValues;

        /// <summary>
        /// Gets or sets the project directory.
        /// </summary>
        public string ProjectDirectory { get; private set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory { get; private set; }

        /// <summary>
        /// Gets or sets the intermediate directory.
        /// </summary>
        public string IntermediateDirectory { get; private set; }

        /// <summary>
        /// Gets the content stats of the previous build.
        /// </summary>
        public ContentStatsCollection ContentStats { get; private set; }

        private ContentCompiler _compiler;

        /// <summary>
        /// Gets or sets the content build logger.
        /// </summary>
        public ContentBuildLogger Logger { get; set; }

        /// <summary>
        /// Gets the list of assemblies referenced by the content project file.
        /// </summary>
        public List<string> Assemblies { get; private set; }

        /// <summary>
        /// The current target graphics profile for which all content is built.
        /// </summary>
        public GraphicsProfile Profile { get; set; }

        /// <summary>
        /// The current target platform for which all content is built.
        /// </summary>
        public TargetPlatform Platform { get; set; }

        /// <summary>
        /// The build configuration passed thru to content processors.
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Gets or sets if the content is compressed.
        /// </summary>
        public bool CompressContent { get; set; }

        /// <summary>
        /// If true exceptions thrown from within an importer or processor are caught and then
        /// thrown from the context. Default value is true.
        /// </summary>
        public bool RethrowExceptions { get; set; }

        /// <summary>
        /// Creates a new instance of PipelineManager.
        /// </summary>
        /// <param name="projectDir">Project directory.</param>
        /// <param name="outputDir">Output directory.</param>
        /// <param name="intermediateDir">Intermediate directory.</param>
        public PipelineManager(string projectDir, string outputDir, string intermediateDir)
        {
            _pipelineBuildEvents = new Dictionary<string, List<PipelineBuildEvent>>();
            _processorDefaultValues = new Dictionary<string, OpaqueDataDictionary>();
            RethrowExceptions = true;

            Assemblies = new List<string>();
            Assemblies.Add(null);
            Logger = new PipelineBuildLogger();

            ProjectDirectory = PathHelper.NormalizeDirectory(projectDir);
            OutputDirectory = PathHelper.NormalizeDirectory(outputDir);
            IntermediateDirectory = PathHelper.NormalizeDirectory(intermediateDir);

	        RegisterCustomConverters();

            // Load the previous content stats.
            ContentStats = new ContentStatsCollection();
            ContentStats.PreviousStats = ContentStatsCollection.Read(intermediateDir);
        }

        /// <summary>
        /// Assigns a type converter for a type
        /// </summary>
        /// <typeparam name="TType">Type to be converted.</typeparam>
        /// <typeparam name="TTypeConverter">Type converter to be applied to the type.</typeparam>
	    public void AssignTypeConverter<TType, TTypeConverter> ()
	    {
		    TypeDescriptor.AddAttributes (typeof (TType), new TypeConverterAttribute (typeof (TTypeConverter)));
	    }

	    private void RegisterCustomConverters ()
	    {
		    AssignTypeConverter<Microsoft.Xna.Framework.Color, StringToColorConverter> ();
	    }

        /// <summary>
        /// Adds an assembly to be used by the pipeline.
        /// </summary>
        /// <param name="assemblyFilePath">Assembly file path.</param>
        /// <exception cref="ArgumentException">Thrown if the assembly file path is not absolute or null.</exception>
        public void AddAssembly(string assemblyFilePath)
        {
            if (assemblyFilePath == null)
                throw new ArgumentException("assemblyFilePath cannot be null!");
            if (!Path.IsPathRooted(assemblyFilePath))
                throw new ArgumentException("assemblyFilePath must be absolute!");

            // Make sure we're not adding the same assembly twice.
            assemblyFilePath = PathHelper.Normalize(assemblyFilePath);
            if (!Assemblies.Contains(assemblyFilePath))
            {
                Assemblies.Add(assemblyFilePath);

                //TODO need better way to update caches
                _processors = null;
                _importers = null;
                _writers = null;
            }
        }

        private void ResolveAssemblies()
        {
            _importers = new List<ImporterInfo>();
            _processors = new List<ProcessorInfo>();
            _writers = new List<Type>();

            // Finally load the pipeline assemblies.
            foreach (var assemblyPath in Assemblies)
            {
                Type[] exportedTypes;
                DateTime assemblyTimestamp;
                try
                {
                    Assembly a;
                    if (string.IsNullOrEmpty(assemblyPath))
                        a = Assembly.GetExecutingAssembly();
                    else
                        a = Assembly.LoadFrom(assemblyPath);

                    exportedTypes = a.GetTypes();
                    assemblyTimestamp = File.GetLastWriteTime(a.Location);
                }
                catch (BadImageFormatException e)
                {
                    Logger.LogWarning(null, null, "Assembly is either corrupt or built using a different " +
                        "target platform than this process. Reference another target architecture (x86, x64, " +
                        "AnyCPU, etc.) of this assembly. '{0}': {1}", assemblyPath, e.Message);
                    // The assembly failed to load... nothing
                    // we can do but ignore it.
                    continue;
                }
                catch (Exception e)
                {
                    Logger.LogWarning(null, null, "Failed to load assembly '{0}': {1}", assemblyPath, e.Message);
                    continue;
                }

                foreach (var t in exportedTypes)
                {
                    if (t.IsAbstract)
                        continue;

                    if (t.GetInterface(@"IContentImporter") != null)
                    {
                        var attributes = t.GetCustomAttributes(typeof (ContentImporterAttribute), false);
                        if (attributes.Length != 0)
                        {
                            var importerAttribute = attributes[0] as ContentImporterAttribute;
                            _importers.Add(new ImporterInfo
                            {
                                attribute = importerAttribute,
                                type = t,
                                assemblyTimestamp = assemblyTimestamp
                            });
                        }
                        else
                        {
                            // If no attribute specify default one
                            var importerAttribute = new ContentImporterAttribute(".*");
                            importerAttribute.DefaultProcessor = "";
                            importerAttribute.DisplayName = t.Name;
                            _importers.Add(new ImporterInfo
                            {
                                attribute = importerAttribute,
                                type = t,
                                assemblyTimestamp = assemblyTimestamp
                            });
                        }
                    }
                    else if (t.GetInterface(@"IContentProcessor") != null)
                    {
                        var attributes = t.GetCustomAttributes(typeof (ContentProcessorAttribute), false);
                        if (attributes.Length != 0)
                        {
                            var processorAttribute = attributes[0] as ContentProcessorAttribute;
                            _processors.Add(new ProcessorInfo
                            {
                                attribute = processorAttribute,
                                type = t,
                                assemblyTimestamp = assemblyTimestamp
                            });
                        }
                    }
                    else if (t.GetInterface(@"ContentTypeWriter") != null)
                    {
						// TODO: This doesn't work... how do i find these?
                        _writers.Add(t);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the importer types.
        /// </summary>
        /// <returns>Array of types.</returns>
        public Type[] GetImporterTypes()
        {
            if (_importers == null)
                ResolveAssemblies();

            List<Type> types = new List<Type>();

            foreach (var item in _importers)
            {
                types.Add(item.type);
            }

            return types.ToArray();
        }

        /// <summary>
        /// Gets the processor types.
        /// </summary>
        /// <returns>Array of types.</returns>
        public Type[] GetProcessorTypes()
        {
            if (_processors == null)
                ResolveAssemblies();

            List<Type> types = new List<Type>();

            foreach (var item in _processors)
            {
                types.Add(item.type);
            }

            return types.ToArray();
        }

        /// <summary>
        /// Creates an instance of an importer.
        /// </summary>
        /// <param name="name">Name of the importer.</param>
        /// <returns>Content importer.</returns>
        public IContentImporter CreateImporter(string name)
        {
            if (_importers == null)
                ResolveAssemblies();

            // Search for the importer.
            foreach (var info in _importers)
            {
                if (info.type.Name.Equals(name))
                    return Activator.CreateInstance(info.type) as IContentImporter;
            }

            return null;
        }

        /// <summary>
        /// Returns the importer type name based on the file extension.
        /// </summary>
        /// <param name="ext">File extension to search for.</param>
        /// <returns>Importer type name or <c>null</c> if not found.</returns>
        public string FindImporterByExtension(string ext)
        {
            if (_importers == null)
                ResolveAssemblies();

            // Search for the importer.
            foreach (var info in _importers)
            {
                if (info.attribute.FileExtensions.Any(e => e.Equals(ext, StringComparison.InvariantCultureIgnoreCase)))
                    return info.type.Name;
            }

            return null;
        }

        /// <summary>
        /// Gets the importer assembly timestamp.
        /// </summary>
        /// <param name="name">Assembly name.</param>
        /// <returns>Timestamp.</returns>
        public DateTime GetImporterAssemblyTimestamp(string name)
        {
            if (_importers == null)
                ResolveAssemblies();

            // Search for the importer.
            foreach (var info in _importers)
            {
                if (info.type.Name.Equals(name))
                    return info.assemblyTimestamp;
            }

            return DateTime.MaxValue;
        }

        /// <summary>
        /// Gets the default processor for an importer.
        /// </summary>
        /// <param name="importer">Name of the importer.</param>
        /// <returns>Name of the processor or <c>null</c> if not found.</returns>
        public string FindDefaultProcessor(string importer)
        {
            if (_importers == null)
                ResolveAssemblies();

            // Search for the importer.
            foreach (var info in _importers)
            {
                if (info.type.Name == importer)
                    return info.attribute.DefaultProcessor;
            }

            return null;
        }

        /// <summary>
        /// Gets the processor type.
        /// </summary>
        /// <param name="name">Name of the processor.</param>
        /// <returns>Type of the processor or <c>null</c> if not found.</returns>
        public Type GetProcessorType(string name)
        {
            if (_processors == null)
                ResolveAssemblies();

            // Search for the processor type.
            foreach (var info in _processors)
            {
                if (info.type.Name.Equals(name))
                    return info.type;
            }

            return null;
        }

        /// <summary>
        /// Resolves the importer and processor names.
        /// </summary>
        /// <param name="sourceFilepath">Path of the source file.</param>
        /// <param name="importerName">Name of the importer.</param>
        /// <param name="processorName">Name of the processor.</param>
        /// <exception cref="Exception">Thrown if a default importer or processor could not be found.</exception>
        public void ResolveImporterAndProcessor(string sourceFilepath, ref string importerName, ref string processorName)
        {
            // Resolve the importer name.
            if (string.IsNullOrEmpty(importerName))
                importerName = FindImporterByExtension(Path.GetExtension(sourceFilepath));
            if (string.IsNullOrEmpty(importerName))
                throw new Exception(string.Format("Couldn't find a default importer for '{0}'!", sourceFilepath));

            // Resolve the processor name.
            if (string.IsNullOrEmpty(processorName))
                processorName = FindDefaultProcessor(importerName);
            if (string.IsNullOrEmpty(processorName))
                throw new Exception(string.Format("Couldn't find a default processor for importer '{0}'!", importerName));
        }

        /// <summary>
        /// Creates an instance of a processor.
        /// </summary>
        /// <param name="name">Name of the processor.</param>
        /// <param name="processorParameters">Opaque data dictionary containing the processor parameters.</param>
        /// <returns>Content processor.</returns>
        public IContentProcessor CreateProcessor(string name, OpaqueDataDictionary processorParameters)
        {
            var processorType = GetProcessorType(name);
            if (processorType == null)
                return null;

            // Create the processor.
            var processor = (IContentProcessor)Activator.CreateInstance(processorType);

            // Convert and set the parameters on the processor.
            foreach (var param in processorParameters)
            {
                var propInfo = processorType.GetProperty(param.Key, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null || propInfo.GetSetMethod(false) == null)
                    continue;

                // If the property value is already of the correct type then set it.
                if (propInfo.PropertyType.IsInstanceOfType(param.Value))
                    propInfo.SetValue(processor, param.Value, null);
                else
                {
                    // Find a type converter for this property.
                    var typeConverter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                    if (typeConverter.CanConvertFrom(param.Value.GetType()))
                    {
                        var propValue = typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, param.Value);
                        propInfo.SetValue(processor, propValue, null);
                    }
                }
            }

            return processor;
        }

        /// <summary>
        /// Gets the default values for the content processor parameters.
        /// </summary>
        /// <param name="processorName">The name of the content processor.</param>
        /// <returns>
        /// A dictionary containing the default value for each parameter. Returns
        /// <see langword="null"/> if the content processor has not been created yet.
        /// </returns>
        public OpaqueDataDictionary GetProcessorDefaultValues(string processorName)
        {
            // null is not allowed as key in dictionary.
            if (processorName == null)
                processorName = string.Empty;

            OpaqueDataDictionary defaultValues;
            if (!_processorDefaultValues.TryGetValue(processorName, out defaultValues))
            {
                // Create the content processor instance and read the default values.
                defaultValues = new OpaqueDataDictionary();
                var processorType = GetProcessorType(processorName);
                if (processorType != null)
                {
                    try
                    {
                        var processor = (IContentProcessor)Activator.CreateInstance(processorType);
                        var properties = processorType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
                        foreach (var property in properties)
                            defaultValues.Add(property.Name, property.GetValue(processor, null));
                    }
                    catch
                    {
                        // Ignore exception. Will be handled in ProcessContent.
                    }
                }

                _processorDefaultValues.Add(processorName, defaultValues);
            }

            return defaultValues;
        }

        /// <summary>
        /// Gets the timestamp of the processor assembly.
        /// </summary>
        /// <param name="name">Name of the processor.</param>
        /// <returns>Timestamp.</returns>
        public DateTime GetProcessorAssemblyTimestamp(string name)
        {
            if (_processors == null)
                ResolveAssemblies();

            // Search for the processor.
            foreach (var info in _processors)
            {
                if (info.type.Name.Equals(name))
                    return info.assemblyTimestamp;
            }

            return DateTime.MaxValue;
        }

        /// <summary>
        /// Validate a list of processor parameters for a given processor type.
        /// </summary>
        /// <param name="name">Name of the processor type.</param>
        /// <param name="processorParameters">Opaque data dictionary containing the processor parameters.</param>
        /// <returns>Opaque data dictionary containing the validated parameters.</returns>
        public OpaqueDataDictionary ValidateProcessorParameters(string name, OpaqueDataDictionary processorParameters)
        {
            var result = new OpaqueDataDictionary();

            var processorType = GetProcessorType(name);
            if (processorType == null || processorParameters == null)
            {
                return result;
            }

            foreach (var param in processorParameters)
            {
                var propInfo = processorType.GetProperty(param.Key, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null || propInfo.GetSetMethod(false) == null)
                    continue;

                // Make sure we can assign the value.
                if (!propInfo.PropertyType.IsInstanceOfType(param.Value))
                {
                    // Make sure we can convert the value.
                    var typeConverter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                    if (!typeConverter.CanConvertFrom(param.Value.GetType()))
                        continue;
                }

                result.Add(param.Key, param.Value);
            }

            return result;
        }

        private void ResolveOutputFilepath(string sourceFilepath, ref string outputFilepath)
        {
            // If the output path is null... build it from the source file path.
            if (string.IsNullOrEmpty(outputFilepath))
            {
                var filename = Path.GetFileNameWithoutExtension(sourceFilepath) + ".xnb";
                var directory = PathHelper.GetRelativePath(ProjectDirectory,
                                                           Path.GetDirectoryName(sourceFilepath) +
                                                           Path.DirectorySeparatorChar);
                outputFilepath = Path.Combine(OutputDirectory, directory, filename);
            }
            else
            {
                // If the extension is not XNB or the source file extension then add XNB.
                var sourceExt = Path.GetExtension(sourceFilepath);
                if (outputFilepath.EndsWith(sourceExt, StringComparison.InvariantCultureIgnoreCase))
                    outputFilepath = outputFilepath.Substring(0, outputFilepath.Length - sourceExt.Length);
                if (!outputFilepath.EndsWith(".xnb", StringComparison.InvariantCultureIgnoreCase))
                    outputFilepath += ".xnb";

                // If the path isn't rooted then put it into the output directory.
                if (!Path.IsPathRooted(outputFilepath))
                    outputFilepath = Path.Combine(OutputDirectory, outputFilepath);
            }

            outputFilepath = PathHelper.Normalize(outputFilepath);
        }

        private PipelineBuildEvent LoadBuildEvent(string destFile, out string eventFilepath)
        {
            var contentPath = Path.ChangeExtension(PathHelper.GetRelativePath(OutputDirectory, destFile), PipelineBuildEvent.Extension);
            eventFilepath = Path.Combine(IntermediateDirectory, contentPath);
            return PipelineBuildEvent.Load(eventFilepath);
        }

        /// <summary>
        /// Registers content to be processed by the pipeline manager.
        /// </summary>
        /// <param name="sourceFilepath">Source file path.</param>
        /// <param name="outputFilepath">Output file path.</param>
        /// <param name="importerName">Name of the importer to use.</param>
        /// <param name="processorName">Name of the processor to use.</param>
        /// <param name="processorParameters">Processor parameters.</param>
        public void RegisterContent(string sourceFilepath, string outputFilepath = null, string importerName = null, string processorName = null, OpaqueDataDictionary processorParameters = null)
        {
            sourceFilepath = PathHelper.Normalize(sourceFilepath);
            ResolveOutputFilepath(sourceFilepath, ref outputFilepath);

            ResolveImporterAndProcessor(sourceFilepath, ref importerName, ref processorName);

            var contentEvent = new PipelineBuildEvent
            {
                SourceFile = sourceFilepath,
                DestFile = outputFilepath,
                Importer = importerName,
                Processor = processorName,
                Parameters = ValidateProcessorParameters(processorName, processorParameters),
            };

            // Register pipeline build event. (Required to correctly resolve external dependencies.)
            TrackPipelineBuildEvent(contentEvent);
        }

        /// <summary>
        /// Content builder.
        /// </summary>
        /// <param name="sourceFilepath">Source file path.</param>
        /// <param name="outputFilepath">Output file path.</param>
        /// <param name="importerName">Name of the importer to use.</param>
        /// <param name="processorName">Name of the processor to use.</param>
        /// <param name="processorParameters">Processor parameters.</param>
        /// <returns>PipelineBuildEvent instance.</returns>
        public PipelineBuildEvent BuildContent(string sourceFilepath, string outputFilepath = null, string importerName = null, string processorName = null, OpaqueDataDictionary processorParameters = null)
        {
            sourceFilepath = PathHelper.Normalize(sourceFilepath);
            ResolveOutputFilepath(sourceFilepath, ref outputFilepath);

            ResolveImporterAndProcessor(sourceFilepath, ref importerName, ref processorName);

            // Record what we're building and how.
            var contentEvent = new PipelineBuildEvent
            {
                SourceFile = sourceFilepath,
                DestFile = outputFilepath,
                Importer = importerName,
                Processor = processorName,
                Parameters = ValidateProcessorParameters(processorName, processorParameters),
            };

            // Load the previous content event if it exists.
            string eventFilepath;
            var cachedEvent = LoadBuildEvent(contentEvent.DestFile, out eventFilepath);

            BuildContent(contentEvent, cachedEvent, eventFilepath);

            return contentEvent;
        }

        private void BuildContent(PipelineBuildEvent pipelineEvent, PipelineBuildEvent cachedEvent, string eventFilepath)
        {
            if (!File.Exists(pipelineEvent.SourceFile))
            {
                Logger.LogMessage("{0}", pipelineEvent.SourceFile);
                throw new PipelineException("The source file '{0}' does not exist!", pipelineEvent.SourceFile);
            }

            Logger.PushFile(pipelineEvent.SourceFile);

            // Keep track of all build events. (Required to resolve automatic names "AssetName_n".)
            TrackPipelineBuildEvent(pipelineEvent);

            var rebuild = pipelineEvent.NeedsRebuild(this, cachedEvent);
            if (rebuild)
                Logger.LogMessage("{0}", pipelineEvent.SourceFile);
            else
                Logger.LogMessage("Skipping {0}", pipelineEvent.SourceFile);

            Logger.Indent();
            try
            {
                if (!rebuild)
                {
                    // While this asset doesn't need to be rebuilt the dependent assets might.
                    foreach (var asset in cachedEvent.BuildAsset)
                    {
                        string assetEventFilepath;
                        var assetCachedEvent = LoadBuildEvent(asset, out assetEventFilepath);

                        // If we cannot find the cached event for the dependancy
                        // then we have to trigger a rebuild of the parent content.
                        if (assetCachedEvent == null)
                        {
                            rebuild = true;
                            break;
                        }

                        var depEvent = new PipelineBuildEvent
                        {
                            SourceFile = assetCachedEvent.SourceFile,
                            DestFile = assetCachedEvent.DestFile,
                            Importer = assetCachedEvent.Importer,
                            Processor = assetCachedEvent.Processor,
                            Parameters = assetCachedEvent.Parameters,
                        };

                        // Give the asset a chance to rebuild.
                        BuildContent(depEvent, assetCachedEvent, assetEventFilepath);
                    }
                }

                // Do we need to rebuild?
                if (rebuild)
                {
                    var startTime = DateTime.UtcNow;

                    // Import and process the content.
                    var processedObject = ProcessContent(pipelineEvent);

                    // Write the content to disk.
                    WriteXnb(processedObject, pipelineEvent);

                    // Store the timestamp of the DLLs containing the importer and processor.
                    pipelineEvent.ImporterTime = GetImporterAssemblyTimestamp(pipelineEvent.Importer);
                    pipelineEvent.ProcessorTime = GetProcessorAssemblyTimestamp(pipelineEvent.Processor);

                    // Store the new event into the intermediate folder.
                    pipelineEvent.Save(eventFilepath);

                    var buildTime = DateTime.UtcNow - startTime;

                    // Record stat for this file.
                    ContentStats.RecordStats(pipelineEvent.SourceFile, pipelineEvent.DestFile, pipelineEvent.Processor, processedObject.GetType(), (float)buildTime.TotalSeconds);
                }
                else
                {
                    // Copy the stats from the previous build.
                    ContentStats.CopyPreviousStats(pipelineEvent.SourceFile);
                }
            }
            finally
            {
                Logger.Unindent();
                Logger.PopFile();
            }
        }

        /// <summary>
        /// Processes the content from the pipeline event.
        /// </summary>
        /// <param name="pipelineEvent">Pipeline event.</param>
        /// <returns>Object processed by the pipeline.</returns>
        /// <exception cref="PipelineException">Thrown if there was an error processing the content.</exception>
        public object ProcessContent(PipelineBuildEvent pipelineEvent)
        {
            if (!File.Exists(pipelineEvent.SourceFile))
                throw new PipelineException("The source file '{0}' does not exist!", pipelineEvent.SourceFile);

            // Store the last write time of the source file
            // so we can detect if it has been changed.
            pipelineEvent.SourceTime = File.GetLastWriteTime(pipelineEvent.SourceFile);

            // Make sure we can find the importer and processor.
            var importer = CreateImporter(pipelineEvent.Importer);
            if (importer == null)
                throw new PipelineException("Failed to create importer '{0}'", pipelineEvent.Importer);

            // Try importing the content.
            object importedObject;
            if (RethrowExceptions)
            {
                try
                {

                    var importContext = new PipelineImporterContext(this, pipelineEvent);
                    using var _ = ContextScopeFactory.BeginContext(importContext, pipelineEvent);

                    importedObject = importer.Import(pipelineEvent.SourceFile, importContext);
                }
                catch (PipelineException)
                {
                    throw;
                }
                catch (InvalidContentException)
                {
                    throw;
                }
                catch (Exception inner)
                {
                    throw new PipelineException(string.Format("Importer '{0}' had unexpected failure!", pipelineEvent.Importer), inner);
                }
            }
            else
            {
                var importContext = new PipelineImporterContext(this, pipelineEvent);
                using var _ = ContextScopeFactory.BeginContext(importContext, pipelineEvent);
                importedObject = importer.Import(pipelineEvent.SourceFile, importContext);
            }

            // The pipelineEvent.Processor can be null or empty. In this case the
            // asset should be imported but not processed.
            if (string.IsNullOrEmpty(pipelineEvent.Processor))
                return importedObject;

            var processor = CreateProcessor(pipelineEvent.Processor, pipelineEvent.Parameters);
            if (processor == null)
                throw new PipelineException("Failed to create processor '{0}'", pipelineEvent.Processor);

            // Make sure the input type is valid.
            if (!processor.InputType.IsAssignableFrom(importedObject.GetType()))
            {
                throw new PipelineException(
                    string.Format("The type '{0}' cannot be processed by {1} as a {2}!",
                    importedObject.GetType().FullName,
                    pipelineEvent.Processor,
                    processor.InputType.FullName));
            }

            // Process the imported object.

            object processedObject;
            if (RethrowExceptions)
            {
                try
                {
                    var processContext = new PipelineProcessorContext(this, pipelineEvent);
                    using var _ = ContextScopeFactory.BeginContext(processContext);
                    processedObject = processor.Process(importedObject, processContext);
                }
                catch (PipelineException)
                {
                    throw;
                }
                catch (InvalidContentException)
                {
                    throw;
                }
                catch (Exception inner)
                {
                    throw new PipelineException(string.Format("Processor '{0}' had unexpected failure!", pipelineEvent.Processor), inner);
                }
            }
            else
            {
                var processContext = new PipelineProcessorContext(this, pipelineEvent);
                using var _ = ContextScopeFactory.BeginContext(processContext);
                processedObject = processor.Process(importedObject, processContext);
            }

            return processedObject;
        }

        /// <summary>
        /// Cleans content from the intermediate folder.
        /// </summary>
        /// <param name="sourceFilepath">Path of the source file.</param>
        /// <param name="outputFilepath">Path of the output file.</param>
        public void CleanContent(string sourceFilepath, string outputFilepath = null)
        {
            // First try to load the event file.
            ResolveOutputFilepath(sourceFilepath, ref outputFilepath);
            string eventFilepath;
            var cachedEvent = LoadBuildEvent(outputFilepath, out eventFilepath);

            if (cachedEvent != null)
            {
                // Recursively clean additional (nested) assets.
                foreach (var asset in cachedEvent.BuildAsset)
                {
                    string assetEventFilepath;
                    var assetCachedEvent = LoadBuildEvent(asset, out assetEventFilepath);

                    if (assetCachedEvent == null)
                    {
                        Logger.LogMessage("Cleaning {0}", asset);

                        // Remove asset (.xnb file) from output folder.
                        FileHelper.DeleteIfExists(asset);

                        // Remove event file (.mgcontent file) from intermediate folder.
                        FileHelper.DeleteIfExists(assetEventFilepath);
                        continue;
                    }

                    CleanContent(string.Empty, asset);
                }

                // Remove related output files (non-XNB files) that were copied to the output folder.
                foreach (var asset in cachedEvent.BuildOutput)
                {
                    Logger.LogMessage("Cleaning {0}", asset);
                    FileHelper.DeleteIfExists(asset);
                }
            }

            Logger.LogMessage("Cleaning {0}", outputFilepath);

            // Remove asset (.xnb file) from output folder.
            FileHelper.DeleteIfExists(outputFilepath);

            // Remove event file (.mgcontent file) from intermediate folder.
            FileHelper.DeleteIfExists(eventFilepath);

            _pipelineBuildEvents.Remove(sourceFilepath);
        }

        private void WriteXnb(object content, PipelineBuildEvent pipelineEvent)
        {
            // Make sure the output directory exists.
            var outputFileDir = Path.GetDirectoryName(pipelineEvent.DestFile);

            Directory.CreateDirectory(outputFileDir);

            if (_compiler == null)
                _compiler = new ContentCompiler();

            // Write the XNB.
            using (var stream = new FileStream(pipelineEvent.DestFile, FileMode.Create, FileAccess.Write, FileShare.None))
                _compiler.Compile(stream, content, Platform, Profile, CompressContent, OutputDirectory, outputFileDir);

            // Store the last write time of the output XNB here
            // so we can verify it hasn't been tampered with.
            pipelineEvent.DestTime = File.GetLastWriteTime(pipelineEvent.DestFile);
        }

        /// <summary>
        /// Stores the pipeline build event (in memory) if no matching event is found.
        /// </summary>
        /// <param name="pipelineEvent">The pipeline build event.</param>
        private void TrackPipelineBuildEvent(PipelineBuildEvent pipelineEvent)
        {
            List<PipelineBuildEvent> pipelineBuildEvents;
            bool eventsFound = _pipelineBuildEvents.TryGetValue(pipelineEvent.SourceFile, out pipelineBuildEvents);
            if (!eventsFound)
            {
                pipelineBuildEvents = new List<PipelineBuildEvent>();
                _pipelineBuildEvents.Add(pipelineEvent.SourceFile, pipelineBuildEvents);
            }

            if (FindMatchingEvent(pipelineBuildEvents, pipelineEvent.DestFile, pipelineEvent.Importer, pipelineEvent.Processor, pipelineEvent.Parameters) == null)
                pipelineBuildEvents.Add(pipelineEvent);
        }

        /// <summary>
        /// Gets an automatic asset name, such as "AssetName_0".
        /// </summary>
        /// <param name="sourceFileName">The source file name.</param>
        /// <param name="importerName">The name of the content importer. Can be <see langword="null"/>.</param>
        /// <param name="processorName">The name of the content processor. Can be <see langword="null"/>.</param>
        /// <param name="processorParameters">The processor parameters. Can be <see langword="null"/>.</param>
        /// <returns>The asset name.</returns>
        public string GetAssetName(string sourceFileName, string importerName, string processorName, OpaqueDataDictionary processorParameters)
        {
            Debug.Assert(Path.IsPathRooted(sourceFileName), "Absolute path expected.");

            // Get source file name, which is used for lookup in _pipelineBuildEvents.
            sourceFileName = PathHelper.Normalize(sourceFileName);
            string relativeSourceFileName = PathHelper.GetRelativePath(ProjectDirectory, sourceFileName);

            List<PipelineBuildEvent> pipelineBuildEvents;
            if (_pipelineBuildEvents.TryGetValue(sourceFileName, out pipelineBuildEvents))
            {
                // This source file has already been build.
                // --> Compare pipeline build events.
                ResolveImporterAndProcessor(sourceFileName, ref importerName, ref processorName);

                var matchingEvent = FindMatchingEvent(pipelineBuildEvents, null, importerName, processorName, processorParameters);
                if (matchingEvent != null)
                {
                    // Matching pipeline build event found.
                    string existingName = matchingEvent.DestFile;
                    existingName = PathHelper.GetRelativePath(OutputDirectory, existingName);
                    existingName = existingName.Substring(0, existingName.Length - 4);   // Remove ".xnb".
                    return existingName;
                }

                Logger.LogMessage(string.Format("Warning: Asset {0} built multiple times with different settings.", relativeSourceFileName));
            }

            // No pipeline build event with matching settings found.
            // Get default asset name (= output file name relative to output folder without ".xnb").
            string directoryName = Path.GetDirectoryName(relativeSourceFileName);
            string fileName = Path.GetFileNameWithoutExtension(relativeSourceFileName);
            string assetName = Path.Combine(directoryName, fileName);
            assetName = PathHelper.Normalize(assetName);
            return AppendAssetNameSuffix(assetName);
        }

        /// <summary>
        /// Determines whether the specified list contains a matching pipeline build event.
        /// </summary>
        /// <param name="pipelineBuildEvents">The list of pipeline build events.</param>
        /// <param name="destFile">Absolute path to the output file. Can be <see langword="null"/>.</param>
        /// <param name="importerName">The name of the content importer. Can be <see langword="null"/>.</param>
        /// <param name="processorName">The name of the content processor. Can be <see langword="null"/>.</param>
        /// <param name="processorParameters">The processor parameters. Can be <see langword="null"/>.</param>
        /// <returns>
        /// The matching pipeline build event, or <see langword="null"/>.
        /// </returns>
        private PipelineBuildEvent FindMatchingEvent(List<PipelineBuildEvent> pipelineBuildEvents, string destFile, string importerName, string processorName, OpaqueDataDictionary processorParameters)
        {
            foreach (var existingBuildEvent in pipelineBuildEvents)
            {
                if ((destFile == null || existingBuildEvent.DestFile.Equals(destFile))
                    && existingBuildEvent.Importer == importerName
                    && existingBuildEvent.Processor == processorName)
                {
                    var defaultValues = GetProcessorDefaultValues(processorName);
                    if (PipelineBuildEvent.AreParametersEqual(existingBuildEvent.Parameters, processorParameters, defaultValues))
                    {
                        return existingBuildEvent;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the asset name including a suffix, such as "_0". (The number is incremented
        /// automatically.
        /// </summary>
        /// <param name="baseAssetName">
        /// The asset name without suffix (relative to output folder).
        /// </param>
        /// <returns>The asset name with suffix.</returns>
        private string AppendAssetNameSuffix(string baseAssetName)
        {
            int index = 0;
            string assetName = baseAssetName + "_0";
            while (IsAssetNameUsed(assetName))
            {
                index++;
                assetName = baseAssetName + '_' + index;
            }

            return assetName;
        }

        /// <summary>
        /// Determines whether the specified asset name is already used.
        /// </summary>
        /// <param name="assetName">The asset name (relative to output folder).</param>
        /// <returns>
        /// <see langword="true"/> if the asset name is already used; otherwise,
        /// <see langword="false"/> if the name is available.
        /// </returns>
        private bool IsAssetNameUsed(string assetName)
        {
            string destFile = Path.Combine(OutputDirectory, assetName + ".xnb");

            return _pipelineBuildEvents.SelectMany(pair => pair.Value)
                                       .Select(pipelineEvent => pipelineEvent.DestFile)
                                       .Any(existingDestFile => destFile.Equals(existingDestFile, StringComparison.OrdinalIgnoreCase));
        }
    }
}
