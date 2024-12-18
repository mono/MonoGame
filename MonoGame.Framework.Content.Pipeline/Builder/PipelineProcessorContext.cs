// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    /// <inheritdoc/>
    public class PipelineProcessorContext : ContentProcessorContext
    {
        private readonly PipelineManager _manager;

        private readonly PipelineBuildEvent _pipelineEvent;

        /// <summary>
        /// Creates a new pipeline processor context.
        /// </summary>
        /// <param name="manager">Pipeline manager.</param>
        /// <param name="pipelineEvent">Pipeline event.</param>
        public PipelineProcessorContext(PipelineManager manager, PipelineBuildEvent pipelineEvent)
        {
            _manager = manager;
            _pipelineEvent = pipelineEvent;
        }

        /// <inheritdoc/>
        public override TargetPlatform TargetPlatform { get { return _manager.Platform; } }

        /// <inheritdoc/>
        public override GraphicsProfile TargetProfile { get { return _manager.Profile; } }

        /// <inheritdoc/>
        public override string BuildConfiguration { get { return _manager.Config; } }

        /// <inheritdoc/>
        public override string IntermediateDirectory { get { return _manager.IntermediateDirectory; } }

        /// <inheritdoc/>
        public override string OutputDirectory { get { return _manager.OutputDirectory; } }

        /// <inheritdoc/>
        public override string OutputFilename { get { return _pipelineEvent.DestFile; } }

        /// <inheritdoc/>
        public override OpaqueDataDictionary Parameters { get { return _pipelineEvent.Parameters; } }

        /// <inheritdoc/>
        public override ContentBuildLogger Logger { get { return _manager.Logger; } }

        /// <inheritdoc/>
        public override ContentIdentity SourceIdentity { get { return new ContentIdentity(_pipelineEvent.SourceFile); } }

        /// <inheritdoc/>
        public override void AddDependency(string filename)
        {
            _pipelineEvent.Dependencies.AddUnique(filename);
        }

        /// <inheritdoc/>
        public override void AddOutputFile(string filename)
        {
            _pipelineEvent.BuildOutput.AddUnique(filename);
        }

        /// <inheritdoc/>
        public override TOutput Convert<TInput, TOutput>(   TInput input,
                                                            string processorName,
                                                            OpaqueDataDictionary processorParameters)
        {
            var processor = _manager.CreateProcessor(processorName, processorParameters);
            var processContext = new PipelineProcessorContext(_manager, new PipelineBuildEvent { Parameters = processorParameters } );
            using var _ = ContextScopeFactory.BeginContext(processContext);
            var processedObject = processor.Process(input, processContext);

            // Add its dependencies and built assets to ours.
            _pipelineEvent.Dependencies.AddRangeUnique(processContext._pipelineEvent.Dependencies);
            _pipelineEvent.BuildAsset.AddRangeUnique(processContext._pipelineEvent.BuildAsset);

            return (TOutput)processedObject;
        }

        /// <inheritdoc/>
        public override TOutput BuildAndLoadAsset<TInput, TOutput>( ExternalReference<TInput> sourceAsset,
                                                                    string processorName,
                                                                    OpaqueDataDictionary processorParameters,
                                                                    string importerName)
        {
            var sourceFilepath = PathHelper.Normalize(sourceAsset.Filename);

            // The processorName can be null or empty. In this case the asset should
            // be imported but not processed. This is, for example, necessary to merge
            // animation files as described here:
            // http://blogs.msdn.com/b/shawnhar/archive/2010/06/18/merging-animation-files.aspx.
            bool processAsset = !string.IsNullOrEmpty(processorName);
            _manager.ResolveImporterAndProcessor(sourceFilepath, ref importerName, ref processorName);

            var buildEvent = new PipelineBuildEvent
            {
                SourceFile = sourceFilepath,
                Importer = importerName,
                Processor = processAsset ? processorName : null,
                Parameters = _manager.ValidateProcessorParameters(processorName, processorParameters),
            };

            var processedObject = _manager.ProcessContent(buildEvent);

            // Record that we processed this dependent asset.
            _pipelineEvent.Dependencies.AddUnique(sourceFilepath);

            return (TOutput)processedObject;
        }

        /// <inheritdoc/>
        public override ExternalReference<TOutput> BuildAsset<TInput, TOutput>( ExternalReference<TInput> sourceAsset,
                                                                                string processorName,
                                                                                OpaqueDataDictionary processorParameters,
                                                                                string importerName,
                                                                                string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                assetName = _manager.GetAssetName(sourceAsset.Filename, importerName, processorName, processorParameters);

            // Build the content.
            var buildEvent = _manager.BuildContent(sourceAsset.Filename, assetName, importerName, processorName, processorParameters);

            // Record that we built this dependent asset.
            _pipelineEvent.BuildAsset.AddUnique(buildEvent.DestFile);

            return new ExternalReference<TOutput>(buildEvent.DestFile);
        }
    }
}
