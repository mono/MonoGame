// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    public class DualTextureMaterialContent : MaterialContent
    {
        /// <inheritdoc cref="BasicMaterialContent.AlphaKey"/>
        public const string AlphaKey = "Alpha";
        /// <inheritdoc cref="BasicMaterialContent.DiffuseColorKey"/>
        public const string DiffuseColorKey = "DiffuseColor";
        /// <inheritdoc cref="BasicMaterialContent.TextureKey"/>
        public const string TextureKey = "Texture";
        /// <summary>
        /// String key of the second texture.
        /// </summary>
        public const string Texture2Key = "Texture2";
        /// <inheritdoc cref="BasicMaterialContent.VertexColorEnabledKey"/>
        public const string VertexColorEnabledKey = "VertexColorEnabled";

        /// <inheritdoc cref="BasicMaterialContent.Alpha"/>
        public float? Alpha
        {
            get { return GetValueTypeProperty<float>(AlphaKey); }
            set { SetProperty(AlphaKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.DiffuseColor"/>
        public Vector3? DiffuseColor
        {
            get { return GetValueTypeProperty<Vector3>(DiffuseColorKey); }
            set { SetProperty(DiffuseColorKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.Texture"/>
        public ExternalReference<TextureContent> Texture
        {
            get { return GetTexture(TextureKey); }
            set { SetTexture(TextureKey, value); }
        }

        /// <summary>
        /// Gets or sets the second texture property.
        /// </summary>
        public ExternalReference<TextureContent> Texture2
        {
            get { return GetTexture(Texture2Key); }
            set { SetTexture(Texture2Key, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.VertexColorEnabled"/>
        public bool? VertexColorEnabled
        {
            get { return GetValueTypeProperty<bool>(VertexColorEnabledKey); }
            set { SetProperty(VertexColorEnabledKey, value); }
        }
    }
}
