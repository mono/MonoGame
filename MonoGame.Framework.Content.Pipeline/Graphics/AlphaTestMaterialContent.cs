// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Class to provide properties for processing basic materials with alpha.
    /// </summary>
    public class AlphaTestMaterialContent : MaterialContent
    {
        /// <inheritdoc cref="BasicMaterialContent.Alpha"/>
        public const string AlphaKey = "Alpha";
        /// <summary>
        /// String key of the alpha function.
        /// </summary>
        public const string AlphaFunctionKey = "AlphaFunction";
        /// <inheritdoc cref="BasicMaterialContent.DiffuseColorKey"/>
        public const string DiffuseColorKey = "DiffuseColor";
        /// <summary>
        /// String key of the alpha reference.
        /// </summary>
        public const string ReferenceAlphaKey = "ReferenceAlpha";
        /// <inheritdoc cref="BasicMaterialContent.TextureKey"/>
        public const string TextureKey = "Texture";
        /// <inheritdoc cref="BasicMaterialContent.VertexColorEnabledKey"/>
        public const string VertexColorEnabledKey = "VertexColorEnabled";

        /// <inheritdoc cref="BasicMaterialContent.Alpha"/>
        public float? Alpha
        {
            get { return GetValueTypeProperty<float>(AlphaKey); }
            set { SetProperty(AlphaKey, value); }
        }

        /// <summary>
        /// Gets or sets the alpha function property.
        /// </summary>
        public CompareFunction? AlphaFunction
        {
            get { return GetValueTypeProperty<CompareFunction>(AlphaFunctionKey); }
            set { SetProperty(AlphaFunctionKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.DiffuseColorKey"/>
        public Vector3? DiffuseColor
        {
            get { return GetValueTypeProperty<Vector3>(DiffuseColorKey); }
            set { SetProperty(DiffuseColorKey, value); }
        }

        /// <summary>
        /// Gets or sets the alpha reference property.
        /// </summary>
        public int? ReferenceAlpha
        {
            get { return GetValueTypeProperty<int>(ReferenceAlphaKey); }
            set { SetProperty(ReferenceAlphaKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.Texture"/>
        public ExternalReference<TextureContent> Texture
        {
            get { return GetTexture(TextureKey); }
            set { SetTexture(TextureKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.VertexColorEnabledKey"/>
        public bool? VertexColorEnabled
        {
            get { return GetValueTypeProperty<bool>(VertexColorEnabledKey); }
            set { SetProperty(VertexColorEnabledKey, value); }
        }
    }
}
