// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Class to provide properties for processing skinned materials.
    /// </summary>
    public class SkinnedMaterialContent : MaterialContent
    {
        /// <inheritdoc cref="BasicMaterialContent.AlphaKey"/>
        public const string AlphaKey = "Alpha";
        /// <inheritdoc cref="BasicMaterialContent.DiffuseColorKey"/>
        public const string DiffuseColorKey = "DiffuseColor";
        /// <inheritdoc cref="BasicMaterialContent.EmissiveColorKey"/>
        public const string EmissiveColorKey = "EmissiveColor";
        /// <inheritdoc cref="BasicMaterialContent.SpecularColorKey"/>
        public const string SpecularColorKey = "SpecularColor";
        /// <inheritdoc cref="BasicMaterialContent.SpecularPowerKey"/>
        public const string SpecularPowerKey = "SpecularPower";
        /// <inheritdoc cref="BasicMaterialContent.TextureKey"/>
        public const string TextureKey = "Texture";
        /// <summary>
        /// String key of the vertex weights.
        /// </summary>
        public const string WeightsPerVertexKey = "WeightsPerVertex";

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

        /// <inheritdoc cref="BasicMaterialContent.SpecularColor"/>
        public Vector3? SpecularColor
        {
            get { return GetValueTypeProperty<Vector3>(SpecularColorKey); }
            set { SetProperty(SpecularColorKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.SpecularPower"/>
        public float? SpecularPower
        {
            get { return GetValueTypeProperty<float>(SpecularPowerKey); }
            set { SetProperty(SpecularPowerKey, value); }
        }

        /// <inheritdoc cref="BasicMaterialContent.Texture"/>
        public ExternalReference<TextureContent> Texture
        {
            get { return GetTexture(TextureKey); }
            set { SetTexture(TextureKey, value); }
        }

        /// <summary>
        /// Gets or sets the number of weights per vertex.
        /// </summary>
        public int? WeightsPerVertex
        {
            get { return GetValueTypeProperty<int>(WeightsPerVertexKey); }
            set { SetProperty(WeightsPerVertexKey, value); }
        }
    }
}
