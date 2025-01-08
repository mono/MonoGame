// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    class EnvironmentMapEffectReader : ContentTypeReader<EnvironmentMapEffect>
    {
        protected internal override EnvironmentMapEffect Read(ContentReader input, EnvironmentMapEffect existingInstance)
        {
            var effect = new EnvironmentMapEffect(input.GetGraphicsDevice());
            effect.Texture = input.ReadExternalReference<Texture>() as Texture2D;
			effect.EnvironmentMap = input.ReadExternalReference<TextureCube>() as TextureCube;
			effect.EnvironmentMapAmount = input.ReadSingle ();
			effect.EnvironmentMapSpecular = input.ReadVector3 ();
			effect.FresnelFactor = input.ReadSingle ();
			effect.DiffuseColor = input.ReadVector3 ();
			effect.EmissiveColor = input.ReadVector3 ();
			effect.Alpha = input.ReadSingle ();
            return effect;
        }
    }
}
