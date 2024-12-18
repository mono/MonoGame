// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties for maintaining a 2D texture
    /// </summary>
    public class Texture2DContent : TextureContent
    {
        /// <summary>
        /// Get or set the mipmap chain.
        /// </summary>
        public MipmapChain Mipmaps
        {
            get { return Faces[0]; }
            set { Faces[0] = value; }
        }

        /// <summary>
        /// Initializes a new instance of the Texture2DContent class.
        /// </summary>
        public Texture2DContent() :
            base(new MipmapChainCollection(1, true))
        {
        }

        /// <inheritdoc/>
        public override void Validate(GraphicsProfile? targetProf)
        {
            throw new NotImplementedException();
        }
    }
}
