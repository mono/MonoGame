// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class BasicEffectReader : ContentTypeReader<BasicEffect>
    {
        protected internal override BasicEffect Read(ContentReader input, BasicEffect existingInstance)
        {
            var effect = new BasicEffect(input.GetGraphicsDevice());
            var texture = input.ReadExternalReference<Texture>() as Texture2D;
            if (texture != null)
            {
                effect.Texture = texture;
                effect.TextureEnabled = true;
            }

            effect.DiffuseColor = input.ReadVector3();
            effect.EmissiveColor = input.ReadVector3();
            effect.SpecularColor = input.ReadVector3();
            effect.SpecularPower = input.ReadSingle();
            effect.Alpha = input.ReadSingle();
            effect.VertexColorEnabled = input.ReadBoolean();
            return effect;
        }
    }
}
