// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Class to provide properties for processing basic materials.
    /// </summary>
    public class BasicMaterialContent : MaterialContent
    {
        /// <summary>
        /// String key of the alpha value.
        /// </summary>
        public const string AlphaKey = "Alpha";
        /// <summary>
        /// String key of the diffuse color.
        /// </summary>
        public const string DiffuseColorKey = "DiffuseColor";
        /// <summary>
        /// String key of the emissive color.
        /// </summary>
        public const string EmissiveColorKey = "EmissiveColor";
        /// <summary>
        /// String key of the specular color.
        /// </summary>
        public const string SpecularColorKey = "SpecularColor";
        /// <summary>
        /// String key of the specular power.
        /// </summary>
        public const string SpecularPowerKey = "SpecularPower";
        /// <summary>
        /// String key of the texture.
        /// </summary>
        public const string TextureKey = "Texture";
        /// <summary>
        /// String key of the vertex color enable flag.
        /// </summary>
        public const string VertexColorEnabledKey = "VertexColorEnabled";

        /// <summary>
        /// Gets or sets the alpha property.
        /// </summary>
        public float? Alpha
        {
            get { return GetValueTypeProperty<float>(AlphaKey); }
            set { SetProperty(AlphaKey, value); }
        }

        /// <summary>
        /// Gets or sets the diffuse color property.
        /// </summary>
        public Vector3? DiffuseColor
        {
            get { return GetValueTypeProperty<Vector3>(DiffuseColorKey); }
            set { SetProperty(DiffuseColorKey, value); }
        }

        /// <summary>
        /// Gets or sets the emissive color property.
        /// </summary>
        public Vector3? EmissiveColor
        {
            get { return GetValueTypeProperty<Vector3>(EmissiveColorKey); }
            set { SetProperty(EmissiveColorKey, value); }
        }

        /// <summary>
        /// Gets or sets the specular color property.
        /// </summary>
        public Vector3? SpecularColor
        {
            get { return GetValueTypeProperty<Vector3>(SpecularColorKey); }
            set { SetProperty(SpecularColorKey, value); }
        }

        /// <summary>
        /// Gets or sets the specular power property.
        /// </summary>
        public float? SpecularPower
        {
            get { return GetValueTypeProperty<float>(SpecularPowerKey); }
            set { SetProperty(SpecularPowerKey, value); }
        }

        /// <summary>
        /// Gets or sets the texture property.
        /// </summary>
        public ExternalReference<TextureContent> Texture
        {
            get { return GetTexture(TextureKey); }
            set { SetTexture(TextureKey, value); }
        }

        /// <summary>
        /// Gets or sets the vertex color enabled property.
        /// </summary>
        public bool? VertexColorEnabled
        {
            get { return GetValueTypeProperty<bool>(VertexColorEnabledKey); }
            set { SetProperty(VertexColorEnabledKey, value); }
        }
    }
}
