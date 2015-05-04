﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Writes the enum value to the output. Usually 32 bit, but can be other sizes if T is not integer.
    /// </summary>
    /// <typeparam name="T">The enum type to write.</typeparam>
    [ContentTypeWriter]
    class EnumWriter<T> : BuiltInContentWriter<T>
    {
        Type _underlyingType;

        protected override void Initialize(ContentCompiler compiler)
        {
            base.Initialize(compiler);
            _underlyingType = Enum.GetUnderlyingType(typeof(T));
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Microsoft.Xna.Framework.Content.EnumReader`1[[" + GetRuntimeType(targetPlatform) + "]]";
        }

        protected internal override void Write(ContentWriter output, T value)
        {
            var typeWriter = output.GetTypeWriter(_underlyingType);
            output.WriteRawObject(Convert.ChangeType(value, _underlyingType), typeWriter);
        }
    }
}
