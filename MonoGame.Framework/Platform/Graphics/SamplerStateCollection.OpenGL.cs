// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
//
// Author: Kenneth James Pouncey

using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed partial class SamplerStateCollection
    {
        private void PlatformSetSamplerState(int index)
        {
        }

        private void PlatformClear()
        {
        }

        private void PlatformDirty()
        {
        }

        internal void PlatformSetSamplers(GraphicsDevice device, Shader shader)
        {
            for (int i = 0; i < shader.Samplers.Length; i++)
            {
                var samplerInfo = shader.Samplers[i];
                var samplerState = _actualSamplers[samplerInfo.samplerSlot];
                var texture = device.Textures[samplerInfo.textureSlot];
                
                if (samplerState != null && texture != null)
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + samplerInfo.samplerSlot);
                    GraphicsExtensions.CheckGLError();

                    GL.BindTexture(texture.glTarget, texture.glTexture);
                    GraphicsExtensions.CheckGLError();

                    samplerState.Activate(device, texture.glTarget, texture.LevelCount > 1);

                    unchecked
                    {
                        _graphicsDevice._graphicsMetrics._textureCount++;
                    }
                }
            }
        }
    }
}
