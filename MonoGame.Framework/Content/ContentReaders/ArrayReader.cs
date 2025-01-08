// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class ArrayReader<T> : ContentTypeReader<T[]>
    {
        ContentTypeReader elementReader;

        public ArrayReader()
        {
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
		{
			Type readerType = typeof(T);
			elementReader = manager.GetTypeReader(readerType);
        }

        protected internal override T[] Read(ContentReader input, T[] existingInstance)
        {
            uint count = input.ReadUInt32();
            T[] array = existingInstance;
            if (array == null)
                array = new T[count];

            if (ReflectionHelpers.IsValueType(typeof(T)))
			{
                for (uint i = 0; i < count; i++)
                {
                	array[i] = input.ReadObject<T>(elementReader);
                }
			}
			else
			{
                for (uint i = 0; i < count; i++)
                {
                    var readerType = input.Read7BitEncodedInt();
                	array[i] = readerType > 0 ? input.ReadObject<T>(input.TypeReaders[readerType - 1]) : default(T);
                }
			}
            return array;
        }
    }
}
