// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class Texture3DReader : ContentTypeReader<Texture3D>
    {
        protected internal override Texture3D Read(ContentReader reader, Texture3D existingInstance)
        {
            Texture3D texture = null;

            SurfaceFormat format = (SurfaceFormat)reader.ReadInt32();
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int depth = reader.ReadInt32();
            int levelCount = reader.ReadInt32();

            if (existingInstance == null)
                texture = new Texture3D(reader.GetGraphicsDevice(), width, height, depth, levelCount > 1, format);
            else
                texture = existingInstance;

#if OPENGL
            Threading.BlockOnUIThread(() =>
            {
#endif
                for (int i = 0; i < levelCount; i++)
                {
                    int dataSize = reader.ReadInt32();
                    byte[] data = ContentManager.ScratchBufferPool.Get(dataSize);
                    reader.Read(data, 0, dataSize);
                    texture.SetData(i, 0, 0, width, height, 0, depth, data, 0, dataSize);

                    // Calculate dimensions of next mip level.
                    width = Math.Max(width >> 1, 1);
                    height = Math.Max(height >> 1, 1);
                    depth = Math.Max(depth >> 1, 1);

                    ContentManager.ScratchBufferPool.Return(data);
                }
#if OPENGL
            });
#endif

            return texture;
        }
    }
}
