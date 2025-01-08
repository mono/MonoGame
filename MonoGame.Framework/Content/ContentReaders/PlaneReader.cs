// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class PlaneReader : ContentTypeReader<Plane>
    {
        public PlaneReader()
        {
        }

        protected internal override Plane Read(ContentReader input, Plane existingInstance)
        {
            existingInstance.Normal = input.ReadVector3();
            existingInstance.D = input.ReadSingle();
            return existingInstance;
        }
    }
}
