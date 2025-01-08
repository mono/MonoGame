// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class Int32Reader : ContentTypeReader<int>
    {
        public Int32Reader()
        {
        }

        protected internal override int Read(ContentReader input, int existingInstance)
        {
            return input.ReadInt32();
        }
    }
}
