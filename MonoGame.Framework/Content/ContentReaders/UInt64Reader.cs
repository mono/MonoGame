// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class UInt64Reader : ContentTypeReader<ulong>
    {
        public UInt64Reader()
        {
        }

        protected internal override ulong Read(ContentReader input, ulong existingInstance)
        {
            return input.ReadUInt64();
        }
    }
}
