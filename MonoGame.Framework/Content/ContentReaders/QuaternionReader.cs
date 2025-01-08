// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class QuaternionReader : ContentTypeReader<Quaternion>
    {
        public QuaternionReader()
        {
        }

        protected internal override Quaternion Read(ContentReader input, Quaternion existingInstance)
        {
            return input.ReadQuaternion();
        }
    }
}
