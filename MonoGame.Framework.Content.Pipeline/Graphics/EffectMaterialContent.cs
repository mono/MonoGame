// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides methods and properties for maintaining a collection of named effect texture references.
    /// </summary>
    public class EffectMaterialContent : MaterialContent
    {
        /// <summary>
        /// Key to use in the external reference dictionary for the effect content.
        /// </summary>
        public const string EffectKey = "Effect";

        /// <summary>
        /// Key to use in the external reference dictionary for the compiled effect content.
        /// </summary>
        public const string CompiledEffectKey = "CompiledEffect";

        /// <summary>
        /// Returns or sets the external reference to the effects content.
        /// </summary>
        public ExternalReference<EffectContent> Effect
        {
            get { return GetReferenceTypeProperty<ExternalReference<EffectContent>>(EffectKey); }
            set { SetProperty(EffectKey, value); }
        }

        /// <summary>
        /// Returns or sets the external reference to the effects compiled content.
        /// </summary>
        public ExternalReference<CompiledEffectContent> CompiledEffect
        {
            get { return GetReferenceTypeProperty<ExternalReference<CompiledEffectContent>>(CompiledEffectKey); }
            set { SetProperty(CompiledEffectKey, value); }
        }
    }
}
