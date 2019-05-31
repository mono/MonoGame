// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Builder.Convertors;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace MonoGame.Tools.Pipeline
{
    public class ImporterTypeDescription
    {
        public string TypeName;
        public string DisplayName;
        public string DefaultProcessor;
        public IEnumerable<string> FileExtensions;
        public Type OutputType;

        public ImporterTypeDescription()
        {
            TypeName = "Invalid / Missing Importer";
        }

        public override string ToString()
        {
            return TypeName;
        }

        public override int GetHashCode()
        {
            return TypeName == null ? 0 : TypeName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ImporterTypeDescription;
            if (other == null)
                return false;

            if (string.IsNullOrEmpty(other.TypeName) != string.IsNullOrEmpty(TypeName))
                return false;

            return TypeName.Equals(other.TypeName);
        }
    };

    public class ProcessorTypeDescription
    {
        #region Supporting Types 

        public struct Property
        {
            public string Name;
            public string DisplayName;
            public Type Type;
            public object DefaultValue;
            public bool Browsable;

            public override string ToString()
            {
                return Name;
            }
        }

        public class ProcessorPropertyCollection : IEnumerable<Property>
        {
            private readonly Property[] _properties;

            public ProcessorPropertyCollection(IEnumerable<Property> properties)
            {
                _properties = properties.ToArray();
            }

            public Property this[int index]
            {
                get
                {
                    return _properties[index];
                }
                set
                {
                    _properties[index] = value;
                }
            }

            public Property this[string name]
            {
                get
                {
                    foreach (var p in _properties)
                    {
                        if (p.Name.Equals(name))
                            return p;
                    }

                    throw new IndexOutOfRangeException();
                }

                set
                {
                    for (var i = 0; i < _properties.Length; i++)
                    {
                        var p = _properties[i];
                        if (p.Name.Equals(name))
                        {
                            _properties[i] = value;
                            return;
                        }

                    }

                    throw new IndexOutOfRangeException();
                }
            }

            public bool Contains(string name)
            {
                return _properties.Any(e => e.Name == name);
            }

            public IEnumerator<Property> GetEnumerator()
            {
                return _properties.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _properties.GetEnumerator();
            }
        }

        #endregion

        public string TypeName;
        public string DisplayName;
        public ProcessorPropertyCollection Properties;
        public Type InputType;

        public override string ToString()
        {
            return TypeName;
        }
    };

    internal class PipelineTypes
    {
        [DebuggerDisplay("ImporterInfo: {Type.Name}")]
        private struct ImporterInfo
        {
            public ContentImporterAttribute Attribute;
            public Type Type;
        }

        [DebuggerDisplay("ProcessorInfo: {Type.Name}")]
        private struct ProcessorInfo
        {
            public ContentProcessorAttribute Attribute;
            public Type Type;
        }

        private static List<ImporterInfo> _importers;
        private static List<ProcessorInfo> _processors;
        private static List<FileSystemWatcher> _watchers;

        // Dictionary of encountered plugin assemblies and their paths
        private static Dictionary<string, string> _pluginAssemblies = new Dictionary<string, string>();
        private static string _currentAssemblyDirectory;

        public static ImporterTypeDescription[] Importers { get; private set; }
        public static ProcessorTypeDescription[] Processors { get; private set; }

        public static ImporterTypeDescription NullImporter { get; private set; }
        public static ProcessorTypeDescription NullProcessor { get; private set; }

        public static ImporterTypeDescription MissingImporter { get; private set; }
        public static ProcessorTypeDescription MissingProcessor { get; private set; }

        public static TypeConverter.StandardValuesCollection ImportersStandardValuesCollection { get; private set; }
        public static TypeConverter.StandardValuesCollection ProcessorsStandardValuesCollection { get; private set; }

        private static readonly Dictionary<string, string> _oldNameRemap = new Dictionary<string, string>()
            {
                { "MGMaterialProcessor", "MaterialProcessor" },
                { "MGSongProcessor", "SongProcessor" },
                { "MGSoundEffectProcessor", "SoundEffectProcessor" },
                { "MGSpriteFontDescriptionProcessor", "FontDescriptionProcessor" },
                { "MGSpriteFontTextureProcessor", "FontTextureProcessor" },
                { "MGTextureProcessor", "TextureProcessor" },
                { "MGEffectProcessor", "EffectProcessor" },
            };

        private static string RemapOldNames(string name)
        {
            if (_oldNameRemap.ContainsKey(name))
                return _oldNameRemap[name];

            return name;
        }

        static PipelineTypes()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            MissingImporter = new ImporterTypeDescription()
            {
                DisplayName = "Invalid / Missing Importer",
            };

            MissingProcessor = new ProcessorTypeDescription()
            {
                DisplayName = "Invalid / Missing Processor",
                Properties = new ProcessorTypeDescription.ProcessorPropertyCollection(new ProcessorTypeDescription.Property[0]),
            };

            NullImporter = new ImporterTypeDescription()
            {
                DisplayName = "",
            };

            NullProcessor = new ProcessorTypeDescription()
            {
                DisplayName = "",
                Properties = new ProcessorTypeDescription.ProcessorPropertyCollection(new ProcessorTypeDescription.Property[0]),
            };

            _watchers = new List<FileSystemWatcher>();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if(args.RequestingAssembly != null && _pluginAssemblies.ContainsKey(args.RequestingAssembly.FullName))
            {
                var path = Path.Combine(_pluginAssemblies[args.RequestingAssembly.FullName], new AssemblyName(args.Name).Name + ".dll");

                if(!_pluginAssemblies.ContainsKey(args.Name))
                    _pluginAssemblies.Add(args.Name, _pluginAssemblies[args.RequestingAssembly.FullName]);

                return Assembly.LoadFrom(path);
            }

            return null;
        }

        public static void Load(PipelineProject project)
        {
            Unload();

            var assemblyPaths = new List<string>();

            var projectRoot = project.Location;

            foreach (var i in project.References)
            {
                var path = Path.Combine(projectRoot, i);

                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException("assemblyFilePath cannot be null!");
                if (!Path.IsPathRooted(path))
                    throw new ArgumentException("assemblyFilePath must be absolute!");

                // Make sure we're not adding the same assembly twice.
                path = PathHelper.Normalize(path);
                if (!assemblyPaths.Contains(path))
                    assemblyPaths.Add(path);
            }

            ResolveReferenceAssemblies(assemblyPaths);
            ResolveBuiltinAssemblies();

            var importerDescriptions = new ImporterTypeDescription[_importers.Count];
            var cur = 0;
            foreach (var item in _importers)
            {
                // Find the abstract base class ContentImporter<T>.
                var baseType = item.Type.BaseType;
                while (!baseType.IsAbstract)
                    baseType = baseType.BaseType;

                var outputType = baseType.GetGenericArguments()[0];
                var name = item.Attribute.DisplayName;
                if (string.IsNullOrEmpty(name))
                    name = item.GetType().Name;
                var desc = new ImporterTypeDescription()
                {
                    TypeName = item.Type.Name,
                    DisplayName = name,
                    DefaultProcessor = item.Attribute.DefaultProcessor,
                    FileExtensions = item.Attribute.FileExtensions,
                    OutputType = outputType,
                };
                importerDescriptions[cur] = desc;
                cur++;
            }

            Importers = importerDescriptions;
            ImportersStandardValuesCollection = new TypeConverter.StandardValuesCollection(Importers);

            var processorDescriptions = new ProcessorTypeDescription[_processors.Count];

            const BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            cur = 0;
            foreach (var item in _processors)
            {
                var obj = Activator.CreateInstance(item.Type);
                var typeProperties = item.Type.GetProperties(bindings);
                var properties = new List<ProcessorTypeDescription.Property>();
                foreach (var i in typeProperties)
                {
                    var attrs = i.GetCustomAttributes(true);
                    var name = i.Name;
                    var browsable = true;
                    var defvalue = i.GetValue(obj, null);

                    foreach (var a in attrs)
                    {
                        if (a is BrowsableAttribute)
                            browsable = (a as BrowsableAttribute).Browsable;
                        else if (a is DisplayNameAttribute)
                            name = (a as DisplayNameAttribute).DisplayName;
                    }

                    var p = new ProcessorTypeDescription.Property()
                    {
                        Name = i.Name,
                        DisplayName = name,
                        Type = i.PropertyType,
                        DefaultValue = defvalue,
                        Browsable = browsable
                    };
                    properties.Add(p);
                }

                var inputType = (obj as IContentProcessor).InputType;
                var desc = new ProcessorTypeDescription()
                {
                    TypeName = item.Type.Name,
                    DisplayName = item.Attribute.DisplayName,
                    Properties = new ProcessorTypeDescription.ProcessorPropertyCollection(properties),
                    InputType = inputType,
                };
                if (string.IsNullOrEmpty(desc.DisplayName))
                    desc.DisplayName = desc.TypeName;

                processorDescriptions[cur] = desc;
                cur++;
            }

            Processors = processorDescriptions;
            ProcessorsStandardValuesCollection = new TypeConverter.StandardValuesCollection(Processors);
        }

        public static void Unload()
        {
            foreach (var watch in _watchers)
                watch.Dispose();
            _watchers.Clear();

            _importers = null;
            Importers = null;

            _processors = null;
            Processors = null;

            _pluginAssemblies.Clear();

            ImportersStandardValuesCollection = null;
            ProcessorsStandardValuesCollection = null;
        }

        public static TypeConverter FindConverter(Type type)
        {
            if (type == typeof(Color))
                return new StringToColorConverter();

            return TypeDescriptor.GetConverter(type);
        }

        public static ImporterTypeDescription FindImporter(string name, string fileExtension)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = RemapOldNames(name);

                foreach (var i in Importers)
                {
                    if (i.TypeName.Equals(name))
                        return i;
                }

                foreach (var i in Importers)
                {
                    if (i.DisplayName.Equals(name))
                        return i;
                }

                //Debug.Fail(string.Format("Importer not found! name={0}, ext={1}", name, fileExtension));
                return null;
            }

            var lowerFileExt = fileExtension.ToLowerInvariant();
            foreach (var i in Importers)
            {
                if (i.FileExtensions.Any(e => e.ToLowerInvariant() == lowerFileExt))
                    return i;
            }

            //Debug.Fail(string.Format("Importer not found! name={0}, ext={1}", name, fileExtension));
            return null;
        }

        public static ProcessorTypeDescription FindProcessor(string name, ImporterTypeDescription importer)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = RemapOldNames(name);

                foreach (var i in Processors)
                {
                    if (i.TypeName.Equals(name))
                        return i;
                }

                //Debug.Fail(string.Format("Processor not found! name={0}, importer={1}", name, importer));
                return null;
            }

            if (importer != null)
            {
                foreach (var i in Processors)
                {
                    if (i.TypeName.Equals(importer.DefaultProcessor))
                        return i;
                }
            }

            //Debug.Fail(string.Format("Processor not found! name={0}, importer={1}", name, importer));
            return null;
        }

        /// <summary>
        /// Loads assemblies from their respective paths and processes their importers and processors
        /// </summary>
        /// <param name="assemblyPaths">The assembly paths.</param>
        private static void ResolveReferenceAssemblies(IEnumerable<string> assemblyPaths)
        {
            _importers = new List<ImporterInfo>();
            _processors = new List<ProcessorInfo>();

            foreach (var watch in _watchers)
                watch.Dispose();
            _watchers.Clear();

            foreach (var path in assemblyPaths)
            {
                try
                {
                    _currentAssemblyDirectory = Path.GetDirectoryName(path);

                    var a = Assembly.Load(File.ReadAllBytes(path));
                    var referencedAssemblies = a.GetReferencedAssemblies();

                    _pluginAssemblies.Add(a.FullName, _currentAssemblyDirectory);
                    foreach (var child in referencedAssemblies)
                    {
                        if(!_pluginAssemblies.ContainsKey(child.FullName))
                            _pluginAssemblies.Add(child.FullName, _currentAssemblyDirectory);
                    }
                    
                    var types = a.GetTypes();
                    ProcessTypes(types);

                    var watch = new FileSystemWatcher();
                    watch.Path = Path.GetDirectoryName(path);
                    watch.EnableRaisingEvents = true;
                    watch.Filter = Path.GetFileName(path);
                    watch.Changed += (sender, e) =>
                    {
                        if (Path.GetFileName(path) == e.Name)
                            PipelineController.Instance.OnReferencesModified();
                    };
                    watch.Created += (sender, e) =>
                    {
                        if (Path.GetFileName(path) == e.Name)
                            PipelineController.Instance.OnReferencesModified();
                    };

                    _watchers.Add(watch);
                }
                catch
                {
                    //Logger.LogWarning(null, null, "Failed to load assembly '{0}': {1}", assemblyPath, e.Message);
                    // The assembly failed to load... nothing
                    // we can do but ignore it.
                    continue;
                }
            }

            _currentAssemblyDirectory = null;
        }

        /// <summary>
        /// Loads assemblies in the current app domain and processes their importers and processors
        /// </summary>
        private static void ResolveBuiltinAssemblies()
        {
            var assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());

            foreach (var asm in assemblies)
            {
#if SHIPPING
                try
#endif
                {
                    if (!asm.ToString().Contains("MonoGame") || _pluginAssemblies.ContainsKey(asm.FullName))
                        continue;

                    var types = asm.GetTypes();
                    ProcessTypes(types);
                }
#if SHIPPING
                catch (Exception e)
                {
                    // ??
                }
#endif
            }
        }

        private static void ProcessTypes(IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                if (t.IsAbstract)
                    continue;

                if (t.GetInterface(@"IContentImporter") == typeof(IContentImporter))
                {
                    var attributes = t.GetCustomAttributes(typeof(ContentImporterAttribute), false);
                    if (attributes.Length != 0)
                    {
                        var importerAttribute = attributes[0] as ContentImporterAttribute;
                        _importers.Add(new ImporterInfo { Attribute = importerAttribute, Type = t });
                    }
                    else
                    {
                        // If no attribute specify default one
                        var importerAttribute = new ContentImporterAttribute(".*");
                        importerAttribute.DefaultProcessor = "";
                        importerAttribute.DisplayName = t.Name;
                        _importers.Add(new ImporterInfo { Attribute = importerAttribute, Type = t });
                    }
                }
                else if (t.GetInterface(@"IContentProcessor") == typeof(IContentProcessor))
                {
                    var attributes = t.GetCustomAttributes(typeof(ContentProcessorAttribute), false);
                    if (attributes.Length != 0)
                    {
                        var processorAttribute = attributes[0] as ContentProcessorAttribute;
                        _processors.Add(new ProcessorInfo { Attribute = processorAttribute, Type = t });
                    }
                }
            }
        }
    }
}
