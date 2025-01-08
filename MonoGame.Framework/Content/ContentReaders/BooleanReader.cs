// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class BooleanReader : ContentTypeReader<bool>
    {
        public BooleanReader()
        {
        }

        protected internal override bool Read(ContentReader input, bool existingInstance)
        {
            return input.ReadBoolean();
        }
    }
}
