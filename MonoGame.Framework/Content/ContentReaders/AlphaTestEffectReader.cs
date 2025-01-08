// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    class AlphaTestEffectReader : ContentTypeReader<AlphaTestEffect>
    {
        protected internal override AlphaTestEffect Read(ContentReader input, AlphaTestEffect existingInstance)
        {
            var effect = new AlphaTestEffect(input.GetGraphicsDevice());

            effect.Texture = input.ReadExternalReference<Texture>() as Texture2D;
            effect.AlphaFunction = (CompareFunction)input.ReadInt32();
            effect.ReferenceAlpha = (int)input.ReadUInt32();
            effect.DiffuseColor = input.ReadVector3();
            effect.Alpha = input.ReadSingle();
            effect.VertexColorEnabled = input.ReadBoolean();
            return effect;
        }
    }
}
