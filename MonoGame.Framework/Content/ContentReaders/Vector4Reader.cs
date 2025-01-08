// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class Vector4Reader : ContentTypeReader<Vector4>
    {
        public Vector4Reader()
        {
        }

        protected internal override Vector4 Read(ContentReader input, Vector4 existingInstance)
        {
            return input.ReadVector4();
        }
    }
}
