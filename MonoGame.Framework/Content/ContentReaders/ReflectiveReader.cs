// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    internal class ReflectiveReader<T> : ContentTypeReader
    {
        delegate void ReadElement(ContentReader input, object parent);

        private List<ReadElement> _readers;

        private ConstructorInfo _constructor;

        private ContentTypeReader _baseTypeReader;


        public ReflectiveReader() 
            : base(typeof(T))
        {
        }

        public override bool CanDeserializeIntoExistingObject
        {
            get { return TargetType.IsClass(); }
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {
            base.Initialize(manager);

            var baseType = ReflectionHelpers.GetBaseType(TargetType);
            if (baseType != null && baseType != typeof(object))
				_baseTypeReader = manager.GetTypeReader(baseType);

            _constructor = TargetType.GetDefaultConstructor();

            var properties = TargetType.GetAllProperties();
            var fields = TargetType.GetAllFields();
            _readers = new List<ReadElement>(fields.Length + properties.Length);

            // Gather the properties.
            foreach (var property in properties)
            {
                var read = GetElementReader(manager, property);
                if (read != null)
                    _readers.Add(read);
            }
            
            // Gather the fields.
            foreach (var field in fields)
            {
                var read = GetElementReader(manager, field);
                if (read != null)
                    _readers.Add(read);
            }
        }

        private static ReadElement GetElementReader(ContentTypeReaderManager manager, MemberInfo member)
        {
            var property = member as PropertyInfo;
            var field = member as FieldInfo;
            Debug.Assert(field != null || property != null);

            if (property != null)
            {
                // Properties must have at least a getter.
                if (property.CanRead == false)
                    return null;

                // Skip over indexer properties.
                if (property.GetIndexParameters().Any())
                    return null;
            }

            // Are we explicitly asked to ignore this item?
            if (ReflectionHelpers.GetCustomAttribute<ContentSerializerIgnoreAttribute>(member) != null) 
                return null;

            var contentSerializerAttribute = ReflectionHelpers.GetCustomAttribute<ContentSerializerAttribute>(member);
            if (contentSerializerAttribute == null)
            {
                if (property != null)
                {
                    // There is no ContentSerializerAttribute, so non-public
                    // properties cannot be deserialized.
                    if (!ReflectionHelpers.PropertyIsPublic(property))
                        return null;

                    // If the read-only property has a type reader,
                    // and CanDeserializeIntoExistingObject is true,
                    // then it is safe to deserialize into the existing object.
                    if (!property.CanWrite)
                    {
                        var typeReader = manager.GetTypeReader(property.PropertyType);
                        if (typeReader == null || !typeReader.CanDeserializeIntoExistingObject)
                            return null;
                    }
                }
                else
                {
                    // There is no ContentSerializerAttribute, so non-public
                    // fields cannot be deserialized.
                    if (!field.IsPublic)
                        return null;

                    // evolutional: Added check to skip initialise only fields
                    if (field.IsInitOnly)
                        return null;
                }
            }

            Action<object, object> setter;
            Type elementType;
            if (property != null)
            {
                elementType = property.PropertyType;
                if (property.CanWrite)
                    setter = (o, v) => property.SetValue(o, v, null);
                else
                    setter = (o, v) => { };
            }
            else
            {
                elementType = field.FieldType;
                setter = field.SetValue;
            }

            // Shared resources get special treatment.
            if (contentSerializerAttribute != null && contentSerializerAttribute.SharedResource)
            {
                return (input, parent) =>
                {
                    Action<object> action = value => setter(parent, value);
                    input.ReadSharedResource(action);
                };
            }

            // We need to have a reader at this point.
            var reader = manager.GetTypeReader(elementType);
            if (reader == null)
                if (elementType == typeof(System.Array))
                    reader = new ArrayReader<Array>();
                else
                    throw new ContentLoadException(string.Format("Content reader could not be found for {0} type.", elementType.FullName));

            // We use the construct delegate to pick the correct existing 
            // object to be the target of deserialization.
            Func<object, object> construct = parent => null;
            if (property != null && !property.CanWrite)
                construct = parent => property.GetValue(parent, null);

            return (input, parent) =>
            {
                var existing = construct(parent);
                var obj2 = input.ReadObject(reader, existing);
                setter(parent, obj2);
            };
        }
      
        protected internal override object Read(ContentReader input, object existingInstance)
        {
            T obj;
            if (existingInstance != null)
                obj = (T)existingInstance;
            else
                obj = (_constructor == null ? (T)Activator.CreateInstance(typeof(T)) : (T)_constructor.Invoke(null));
		
			if(_baseTypeReader != null)
				_baseTypeReader.Read(input, obj);

            // Box the type.
            var boxed = (object)obj;

            foreach (var reader in _readers)
                reader(input, boxed);

            // Unbox it... required for value types.
            obj = (T)boxed;

            return obj;
        }
    }
}
