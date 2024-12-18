// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Provides methods and properties to write a <see cref="SpriteFontContent"/> to binary format.
    /// </summary>
    [ContentTypeWriter]
    public class SpriteFontContentWriter : ContentTypeWriter<SpriteFontContent>
    {
        /// <inheritdoc/>
        protected internal override void Write(ContentWriter output, SpriteFontContent value)
        {
            output.WriteObject(value.Texture);
            output.WriteObject(value.Glyphs);
            output.WriteObject(value.Cropping);
            output.WriteObject(value.CharacterMap);
            output.Write(value.VerticalLineSpacing);
            output.Write(value.HorizontalSpacing);
            output.WriteObject(value.Kerning);
            var hasDefChar = value.DefaultCharacter.HasValue;
            output.Write(hasDefChar);
            if (hasDefChar)
                output.Write(value.DefaultCharacter.Value);
        }

        /// <summary>
        /// Gets the assembly qualified name of the runtime loader for this type.
        /// </summary>
        /// <param name="targetPlatform">Name of the platform.</param>
        /// <returns>Name of the runtime loader.</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // Base the reader type string from a known public class in the same namespace in the same assembly
            Type type = typeof(ContentReader);
			string readerType = type.Namespace + ".SpriteFontReader, " + type.Assembly.FullName;
            return readerType;
        }

        /// <summary>
        /// Gets the assembly qualified name of the runtime target type. The runtime target type often matches the design time type, but may differ.
        /// </summary>
        /// <param name="targetPlatform">The target platform.</param>
        /// <returns>The qualified name.</returns>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            // Base the reader type string from a known public class in the same namespace in the same assembly
            Type type = typeof(ContentReader);
			string readerType = type.Namespace + ".SpriteFontReader, " + type.AssemblyQualifiedName;
            return readerType;
        }

        /// <summary>
        /// Indicates whether a given type of content should be compressed.
        /// </summary>
        /// <param name="targetPlatform">The target platform of the content build.</param>
        /// <param name="value">The object about to be serialized, or null if a collection of objects is to be serialized.</param>
        /// <returns>true if the content of the requested type should be compressed; false otherwise.</returns>
        /// <remarks>This base class implementation of this method always returns true. It should be overridden
        /// to return false if there would be little or no useful reduction in size of the content type's data
        /// from a general-purpose lossless compression algorithm.
        /// The implementations for Song Class and SoundEffect Class data return false because data for these
        /// content types is already in compressed form.</remarks>
        protected internal override bool ShouldCompressContent(TargetPlatform targetPlatform, object value)
        {
            return false;
        }
    }
}
