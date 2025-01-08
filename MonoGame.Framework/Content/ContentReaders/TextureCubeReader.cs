// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class TextureCubeReader : ContentTypeReader<TextureCube>
    {

        protected internal override TextureCube Read(ContentReader reader, TextureCube existingInstance)
        {
            TextureCube textureCube = null;

			SurfaceFormat surfaceFormat = (SurfaceFormat)reader.ReadInt32();
			int size = reader.ReadInt32();
			int levels = reader.ReadInt32();

            if (existingInstance == null)
                textureCube = new TextureCube(reader.GetGraphicsDevice(), size, levels > 1, surfaceFormat);
            else
                textureCube = existingInstance;

#if OPENGL
            Threading.BlockOnUIThread(() =>
            {
#endif
                for (int face = 0; face < 6; face++)
                {
                    for (int i = 0; i < levels; i++)
                    {
                        int faceSize = reader.ReadInt32();
                        byte[] faceData = ContentManager.ScratchBufferPool.Get(faceSize);
                        reader.Read(faceData, 0, faceSize);
                        textureCube.SetData<byte>((CubeMapFace)face, i, null, faceData, 0, faceSize);
                        ContentManager.ScratchBufferPool.Return(faceData);
                    }
                }
#if OPENGL
            });
#endif

             return textureCube;
        }
    }
}
