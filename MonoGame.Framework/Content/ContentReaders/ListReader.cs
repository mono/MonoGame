// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class ListReader<T> : ContentTypeReader<List<T>>
    {
        ContentTypeReader elementReader;

        public ListReader()
        {
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {
			Type readerType = typeof(T);
			elementReader = manager.GetTypeReader(readerType);
        }

        public override bool CanDeserializeIntoExistingObject
        {
            get { return true; }
        }

        protected internal override List<T> Read(ContentReader input, List<T> existingInstance)
        {
            int count = input.ReadInt32();
            List<T> list = existingInstance;
            if (list == null) list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                if (ReflectionHelpers.IsValueType(typeof(T)))
				{
                	list.Add(input.ReadObject<T>(elementReader));
				}
				else
				{
                    var readerType = input.Read7BitEncodedInt();
                	list.Add(readerType > 0 ? input.ReadObject<T>(input.TypeReaders[readerType - 1]) : default(T));
				}
            }
            return list;
        }
    }
}
