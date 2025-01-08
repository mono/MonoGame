// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    class DualTextureEffectReader : ContentTypeReader<DualTextureEffect>
    {
        protected internal override DualTextureEffect Read(ContentReader input, DualTextureEffect existingInstance)
        {
			DualTextureEffect effect = new DualTextureEffect(input.GetGraphicsDevice());
			effect.Texture = input.ReadExternalReference<Texture>() as Texture2D;
			effect.Texture2 = input.ReadExternalReference<Texture>() as Texture2D;
			effect.DiffuseColor = input.ReadVector3 ();
			effect.Alpha = input.ReadSingle ();
			effect.VertexColorEnabled = input.ReadBoolean ();
			return effect;
		}
	}
}

