// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Microsoft.Xna.Framework.Media;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    internal class VideoReader : ContentTypeReader<Video>
    {
        protected internal override Video Read(ContentReader input, Video existingInstance)
        {
            string path = input.ReadObject<string>();

            if (!string.IsNullOrEmpty(path))
            {
                // Add the ContentManager's RootDirectory
                var dirPath = Path.Combine(input.ContentManager.RootDirectoryFullPath, input.AssetName);

                // Resolve the relative path
                path = FileHelpers.ResolveRelativePath(dirPath, path);
            }

            var durationMS = input.ReadObject<int>();
            var width = input.ReadObject<int>();
            var height = input.ReadObject<int>();
            var framesPerSecond = input.ReadObject<float>();
            var soundTrackType = input.ReadObject<int>();  // 0 = Music, 1 = Dialog, 2 = Music and dialog

            return new Video(path, durationMS, width, height, framesPerSecond, (VideoSoundtrackType)soundTrackType);       
        }
    }
}
