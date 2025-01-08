// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    class IndexBufferReader : ContentTypeReader<IndexBuffer>
    {
        protected internal override IndexBuffer Read(ContentReader input, IndexBuffer existingInstance)
        {
            IndexBuffer indexBuffer = existingInstance;

            bool sixteenBits = input.ReadBoolean();
            int dataSize = input.ReadInt32();
            if (indexBuffer == null)
            {
                indexBuffer = new IndexBuffer(input.GetGraphicsDevice(),
                    sixteenBits ? IndexElementSize.SixteenBits : IndexElementSize.ThirtyTwoBits, 
                    dataSize / (sixteenBits ? 2 : 4), BufferUsage.None);
            }

            byte[] data = ContentManager.ScratchBufferPool.Get(dataSize);
            input.Read(data, 0, dataSize);
            indexBuffer.SetData(data, 0, dataSize);
            ContentManager.ScratchBufferPool.Return(data);

            return indexBuffer;
        }
    }
}
