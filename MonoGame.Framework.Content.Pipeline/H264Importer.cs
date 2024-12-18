// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides methods for reading H.264 video files for use in the Content Pipeline
    /// </summary>
    [ContentImporter(".mp4", DisplayName = "H.264 Video - MonoGame", DefaultProcessor = "VideoProcessor")]
    public class H264Importer : ContentImporter<VideoContent>
    {
        /// <summary>
        /// Creates a new instance of H264Importer
        /// </summary>
        public H264Importer()
        {
        }

        /// <inheritdoc/>
        public override VideoContent Import(string filename, ContentImporterContext context)
        {
            var content = new VideoContent(filename);
            return content;
        }
    }
}
