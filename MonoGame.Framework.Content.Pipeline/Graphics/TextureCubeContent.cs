// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides a base class for texture cube content.
    /// </summary>
    public class TextureCubeContent : TextureContent
    {
        /// <summary>
        /// Creates a new TextureCubeContent object.
        /// </summary>
        public TextureCubeContent() :
            base(new MipmapChainCollection(6, true))
        {
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Validate is not implemented for this texture content type.
        /// </remarks>
        public override void Validate(GraphicsProfile? targetProf)
        {
            throw new NotImplementedException();
        }
    }
}
