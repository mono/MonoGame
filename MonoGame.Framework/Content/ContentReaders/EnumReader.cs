// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    internal class EnumReader<T> : ContentTypeReader<T>
    {
        ContentTypeReader elementReader;

        public EnumReader()
        {
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {			
			Type readerType = Enum.GetUnderlyingType(typeof(T));
			elementReader = manager.GetTypeReader(readerType);
        }
		
        protected internal override T Read(ContentReader input, T existingInstance)
        {
			return input.ReadRawObject<T>(elementReader);
		}
    }
}

