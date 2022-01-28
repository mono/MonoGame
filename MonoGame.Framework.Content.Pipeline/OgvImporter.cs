// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline
{
    [ContentImporter(".ogv", DisplayName = "Theora Video in Ogg container - MonoGame", DefaultProcessor = "VideoProcessor")]
    public class OgvImporter : ContentImporter<VideoContent>
    {
        public OgvImporter()
        {
        }

        public override VideoContent Import(string filename, ContentImporterContext context)
        {
            return new VideoContent(filename);
        }
    }
}
