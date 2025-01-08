// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class EffectReader : ContentTypeReader<Effect>
    {
        public EffectReader()
        {
        }

        protected internal override Effect Read(ContentReader input, Effect existingInstance)
        {
            int dataSize = input.ReadInt32();
            byte[] data = ContentManager.ScratchBufferPool.Get(dataSize);
            input.Read(data, 0, dataSize);
            var effect = new Effect(input.GetGraphicsDevice(), data, 0, dataSize);
            ContentManager.ScratchBufferPool.Return(data);
            effect.Name = input.AssetName;
            return effect;
        }
    }
}
