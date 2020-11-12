// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class Texture
    {
        internal int glTexture = -1;
        internal TextureTarget glTarget;
        internal TextureUnit glTextureUnit = TextureUnit.Texture0;
        internal PixelInternalFormat glInternalFormat;
        internal PixelFormat glFormat;
        internal PixelType glType;

        private void PlatformGraphicsDeviceResetting()
        {
            DeleteGLTexture();
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                DeleteGLTexture();
            }

            base.Dispose(disposing);
        }

        private void DeleteGLTexture()
        {
            if (glTexture > 0)
            {
                GraphicsDevice.DisposeTexture(glTexture);
            }
            glTexture = -1;
        }
    }
}

