// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class DateTimeReader : ContentTypeReader<DateTime>
    {
        public DateTimeReader()
        {
        }

        protected internal override DateTime Read(ContentReader input, DateTime existingInstance)
        {
            UInt64 value = input.ReadUInt64();
            UInt64 mask = (UInt64)3 << 62;
            long ticks = (long)(value & ~mask);
            DateTimeKind kind = (DateTimeKind)((value >> 62) & 3);
            return new DateTime(ticks, kind);
        }
    }
}
