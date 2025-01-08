// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    internal class StringReader : ContentTypeReader<String>
    {
        public StringReader()
        {
        }

        protected internal override string Read(ContentReader input, string existingInstance)
        {
            return input.ReadString();
        }
    }
}
