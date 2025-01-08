// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    class MatrixReader : ContentTypeReader<Matrix>
    {
        protected internal override Matrix Read(ContentReader input, Matrix existingInstance)
        {
            var m11 = input.ReadSingle();
            var m12 = input.ReadSingle();
            var m13 = input.ReadSingle();
            var m14 = input.ReadSingle();
            var m21 = input.ReadSingle();
            var m22 = input.ReadSingle();
            var m23 = input.ReadSingle();
            var m24 = input.ReadSingle();
            var m31 = input.ReadSingle();
            var m32 = input.ReadSingle();
            var m33 = input.ReadSingle();
            var m34 = input.ReadSingle();
            var m41 = input.ReadSingle();
            var m42 = input.ReadSingle();
            var m43 = input.ReadSingle();
            var m44 = input.ReadSingle();
            return new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }
    }
}
