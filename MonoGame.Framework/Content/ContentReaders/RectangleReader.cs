// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class RectangleReader : ContentTypeReader<Rectangle>
    {
        public RectangleReader()
        {
        }

        protected internal override Rectangle Read(ContentReader input, Rectangle existingInstance)
        {
            int left = input.ReadInt32();
            int top = input.ReadInt32();
            int width = input.ReadInt32();
            int height = input.ReadInt32();
            return new Rectangle(left, top, width, height);
        }
    }
}
