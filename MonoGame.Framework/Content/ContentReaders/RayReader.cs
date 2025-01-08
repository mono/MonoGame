// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class RayReader : ContentTypeReader<Ray>
    {
        public RayReader()
        {
        }

        protected internal override Ray Read(ContentReader input, Ray existingInstance)
        {
            Vector3 position = input.ReadVector3();
            Vector3 direction = input.ReadVector3();
            return new Ray(position, direction);
        }
    }
}
