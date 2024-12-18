// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate
{
    /// <summary>
    /// Provides a generic implementation of ContentTypeSerializer methods and properties for serializing and deserializing a specific managed type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ContentTypeSerializer<T> : ContentTypeSerializer
    {
        /// <summary>
        /// Initializes a new instance of the ContentTypeSerializer class.
        /// </summary>
        protected ContentTypeSerializer() :
            this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContentTypeSerializer class using the specified XML shortcut name.
        /// </summary>
        /// <param name="xmlTypeName">Xml shortcut name.</param>
        protected ContentTypeSerializer(string xmlTypeName) :
            base(typeof(T), xmlTypeName)
        {
        }

        /// <summary/>
        protected internal abstract T Deserialize(IntermediateReader input, ContentSerializerAttribute format, T existingInstance);

        /// <summary/>
        protected internal override object Deserialize(IntermediateReader input, ContentSerializerAttribute format, object existingInstance)
        {
            var cast = existingInstance == null ? default(T) : (T)existingInstance;
            return Deserialize(input, format, cast);
        }

        /// <inheritdoc cref="ContentTypeSerializer.ObjectIsEmpty"/>
        public virtual bool ObjectIsEmpty(T value)
        {
            return base.ObjectIsEmpty(value);
        }

        /// <inheritdoc cref="ContentTypeSerializer.ObjectIsEmpty"/>
        public override bool ObjectIsEmpty(object value)
        {
            var cast = value == null ? default(T) : (T)value;
            return ObjectIsEmpty(cast);
        }

        /// <summary/>
        protected internal virtual void ScanChildren(IntermediateSerializer serializer, ChildCallback callback, T value)
        {
        }

        /// <summary/>
        protected internal override void ScanChildren(IntermediateSerializer serializer, ChildCallback callback, object value)
        {
            if (value == null)
                return;
            ScanChildren(serializer, callback, (T)value);
        }

        /// <summary/>
        protected internal abstract void Serialize(IntermediateWriter output, T value, ContentSerializerAttribute format);

        /// <summary/>
        protected internal override void Serialize(IntermediateWriter output, object value, ContentSerializerAttribute format)
        {
            var cast = value == null ? default(T) : (T)value;
            Serialize(output, cast, format);
        }
    }
}
