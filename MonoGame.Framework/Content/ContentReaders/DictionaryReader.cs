// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    internal class DictionaryReader<TKey, TValue> : ContentTypeReader<Dictionary<TKey, TValue>>
    {
        ContentTypeReader keyReader;
		ContentTypeReader valueReader;
		
		Type keyType;
		Type valueType;
		
        public DictionaryReader()
        {
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {
			keyType = typeof(TKey);
			valueType = typeof(TValue);
			
			keyReader = manager.GetTypeReader(keyType);
			valueReader = manager.GetTypeReader(valueType);
        }

        public override bool CanDeserializeIntoExistingObject
        {
            get { return true; }
        }

        protected internal override Dictionary<TKey, TValue> Read(ContentReader input, Dictionary<TKey, TValue> existingInstance)
        {
            int count = input.ReadInt32();
            Dictionary<TKey, TValue> dictionary = existingInstance;
            if (dictionary == null)
                dictionary = new Dictionary<TKey, TValue>(count);
            else
                dictionary.Clear();

            for (int i = 0; i < count; i++)
            {
				TKey key;
				TValue value;

                if (ReflectionHelpers.IsValueType(keyType))
                {
                	key = input.ReadObject<TKey>(keyReader);
				}
				else
                {
                    var readerType = input.Read7BitEncodedInt();
                    key = readerType > 0 ? input.ReadObject<TKey>(input.TypeReaders[readerType - 1]) : default(TKey);
                }

                if (ReflectionHelpers.IsValueType(valueType))
				{
                	value = input.ReadObject<TValue>(valueReader);
				}
				else
                {
                    var readerType = input.Read7BitEncodedInt();
                    value = readerType > 0 ? input.ReadObject<TValue>(input.TypeReaders[readerType - 1]) : default(TValue);
                }

                dictionary.Add(key, value);
            }
            return dictionary;
        }
    }
}

