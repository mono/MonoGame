// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate
{
    /// <summary>
    /// Provides methods for serializing and deserializing a specific managed type.
    /// </summary>
    public abstract class ContentTypeSerializer
    {
        /// <summary>
        /// Initializes a new instance of the ContentTypeSerializer class.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="xmlTypeName">The XML shortcut name.</param>
        protected ContentTypeSerializer(Type targetType, string xmlTypeName)
        {
            TargetType = targetType;
            XmlTypeName = xmlTypeName;
        }

        /// <summary>
        /// Gets a value indicating whether this component may load data into an existing object or if
        /// it must it construct a new instance of the object before loading the data.
        /// </summary>
        public virtual bool CanDeserializeIntoExistingObject
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets the XML shortcut name (XmlTypeName).
        /// </summary>
        public string XmlTypeName { get; private set; }

        /// <summary/>
        protected internal abstract object Deserialize(IntermediateReader input, ContentSerializerAttribute format, object existingInstance);

        /// <summary/>
        protected internal virtual void Initialize(IntermediateSerializer serializer)
        {
        }

        /// <summary>
        /// Queries whether an object contains data to be serialized.
        /// </summary>
        /// <param name="value">The object to test.</param>
        /// <returns><c>true</c> if the object is empty; otherwise, <c>false</c>.</returns>
        public virtual bool ObjectIsEmpty(object value)
        {
            return false;
        }

        /// <summary/>
        protected internal virtual void ScanChildren(IntermediateSerializer serializer, ChildCallback callback, object value)
        {
        }

        /// <summary/>
        protected internal abstract void Serialize(IntermediateWriter output, object value, ContentSerializerAttribute format);

        /// <summary/>
        internal protected delegate void ChildCallback(ContentTypeSerializer typeSerializer, object value);
    }
}
