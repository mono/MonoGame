// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    class BoundingBoxReader : ContentTypeReader<BoundingBox>
    {
        protected internal override BoundingBox Read(ContentReader input, BoundingBox existingInstance)
        {
            var min = input.ReadVector3();
            var max = input.ReadVector3();
            var result = new BoundingBox(min, max);
            return result;
        }
    }
}
