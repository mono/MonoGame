// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;
namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class VertexDeclarationReader : ContentTypeReader<VertexDeclaration>
	{
		protected internal override VertexDeclaration Read(ContentReader reader, VertexDeclaration existingInstance)
        {
			var vertexStride = reader.ReadInt32();
			var elementCount = reader.ReadInt32();
			VertexElement[] elements = new VertexElement[elementCount];
			for (int i = 0; i < elementCount; ++i)
			{
				var offset = reader.ReadInt32();
				var elementFormat = (VertexElementFormat)reader.ReadInt32();
				var elementUsage = (VertexElementUsage)reader.ReadInt32();
				var usageIndex = reader.ReadInt32();
				elements[i] = new VertexElement(offset, elementFormat, elementUsage, usageIndex);
			}

            return VertexDeclaration.GetOrCreate(vertexStride, elements);
		}
	}
}
