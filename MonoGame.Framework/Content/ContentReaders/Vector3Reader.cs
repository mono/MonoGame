// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    internal class Vector3Reader : ContentTypeReader<Vector3>
    {
        public Vector3Reader()
        {
        }

        protected internal override Vector3 Read(ContentReader input, Vector3 existingInstance)
        {
            return input.ReadVector3();
        }
    }
}
