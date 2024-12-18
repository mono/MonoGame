// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Class to provide properties for processing environmental map materials.
    /// </summary>
    public class EnvironmentMapMaterialContent : MaterialContent
    {
        /// <inheritdoc cref="BasicMaterialContent.AlphaKey"/>
        public const string AlphaKey = "Alpha";
        /// <inheritdoc cref="BasicMaterialContent.DiffuseColorKey"/>
        public const string DiffuseColorKey = "DiffuseColor";
        /// <inheritdoc cref="BasicMaterialContent.EmissiveColorKey"/>
        public const string EmissiveColorKey = "EmissiveColor";
        /// <summary>
        /// String key of the environment map.
        /// </summary>
        public const string EnvironmentMapKey = "EnvironmentMap";
        /// <summary>
        /// string key of environment map amount.
        /// </summary>
        public const string EnvironmentMapAmountKey = "EnvironmentMapAmount";
        /// <summary>
        /// String key of the specular color.
        /// </summary>
        public const string EnvironmentMapSpecularKey = " EnvironmentMapSpecular";
        /// <summary>
        /// String key of the fresnel factor.
        /// </summary>
        public const string FresnelFactorKey = "FresnelFactor";
        /// <inheritdoc cref="BasicMaterialContent.TextureKey"/>
        public const string TextureKey = "Texture";

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

        /// <inheritdoc cref="BasicMaterialContent.EmissiveColor"/>
        public Vector3? EmissiveColor
        {
            get { return GetValueTypeProperty<Vector3>(EmissiveColorKey); }
            set { SetProperty(EmissiveColorKey, value); }
        }

        /// <summary>
        /// Gets or sets the environment map property.
        /// </summary>
        public ExternalReference<TextureContent> EnvironmentMap
        {
            get { return GetTexture(EnvironmentMapKey); }
            set { SetTexture(EnvironmentMapKey, value); }
        }

        /// <summary>
        /// Gets or sets the environment map amount property.
        /// </summary>
        public float? EnvironmentMapAmount
        {
            get { return GetValueTypeProperty<float>(EnvironmentMapAmountKey); }
            set { SetProperty(EnvironmentMapAmountKey, value); }
        }

        /// <summary>
        /// Gets or sets the specular color property.
        /// </summary>
        public Vector3? EnvironmentMapSpecular
        {
            get { return GetValueTypeProperty<Vector3>(EnvironmentMapSpecularKey); }
            set { SetProperty(EnvironmentMapSpecularKey, value); }
        }

        /// <summary>
        /// Gets or sets the fresnel factor property.
        /// </summary>
        public float? FresnelFactor
        {
            get { return GetValueTypeProperty<float>(FresnelFactorKey); }
            set { SetProperty(FresnelFactorKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.Texture"/>
        public ExternalReference<TextureContent> Texture
        {
            get { return GetTexture(TextureKey); }
            set { SetTexture(TextureKey, value); }
        }
    }
}
