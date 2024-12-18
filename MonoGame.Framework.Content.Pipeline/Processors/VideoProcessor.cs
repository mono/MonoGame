// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Content.Pipeline.Builder;
using System.IO;

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Provides methods and properties for maintaining video content.
    /// </summary>
    [ContentProcessor(DisplayName = "Video - MonoGame")]
    public class VideoProcessor : ContentProcessor<VideoContent, VideoContent>
    {
        /// <inheritdoc/>
        public override VideoContent Process(VideoContent input, ContentProcessorContext context)
        {
            var relative = Path.GetDirectoryName(PathHelper.GetRelativePath(context.OutputDirectory, context.OutputFilename));
            var relVideoPath = PathHelper.Normalize(Path.Combine(relative, Path.GetFileName(input.Filename)));
            var absVideoPath = PathHelper.Normalize(Path.Combine(context.OutputDirectory, relVideoPath));

            // Make sure the output folder for the video exists.
            Directory.CreateDirectory(Path.GetDirectoryName(absVideoPath));

            // Copy the already encoded video file over
            File.Copy(input.Filename, absVideoPath, true);
            context.AddOutputFile(absVideoPath);

            // Fixup relative path
            input.Filename = PathHelper.GetRelativePath(context.OutputFilename, absVideoPath);

            return input;
        }
    }
}
