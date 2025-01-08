// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class DoubleReader : ContentTypeReader<double>
    {
        public DoubleReader()
        {
        }

        protected internal override double Read(ContentReader input, double existingInstance)
        {
            return input.ReadDouble();
        }
    }
}

