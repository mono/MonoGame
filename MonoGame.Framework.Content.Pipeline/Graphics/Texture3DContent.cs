// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties for maintaining a 3D texture
    /// </summary>
    public class Texture3DContent : TextureContent
    {
        /// <summary>
        /// Initializes a new instance of the Texture3DContent class.
        /// </summary>
        public Texture3DContent() :
            base(new MipmapChainCollection(0, false))
        {
        }

        /// <inheritdoc/>
        public override void Validate(GraphicsProfile? targetProf)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void GenerateMipmaps(bool overwriteExistingMipmaps)
        {
            throw new NotImplementedException();
        }
    }
}
