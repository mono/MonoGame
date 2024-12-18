// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate
{
    /// <summary>
    /// IntermetiateReader is used to read content from the intermediate format.
    /// </summary>
    public sealed class IntermediateReader
    {
        private readonly string _filePath;

        private readonly Dictionary<string, Action<object>> _resourceFixups;

        private readonly Dictionary<string, List<Action<Type, string>>> _externalReferences;

        /// <summary>
        /// Gets the instances XML reader.
        /// </summary>
        public XmlReader Xml { get; private set; }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        public IntermediateSerializer Serializer { get; private set; }

        internal IntermediateReader(IntermediateSerializer serializer, XmlReader xmlReader, string filePath)
        {
            Serializer = serializer;
            Xml = xmlReader;
            _filePath = filePath;
            _resourceFixups = new Dictionary<string, Action<object>>();
            _externalReferences = new Dictionary<string, List<Action<Type, string>>>();
        }

        /// <summary>
        /// Moves the XML reader to the specified element.
        /// </summary>
        /// <param name="elementName">The name of the element to move to.</param>
        /// <returns><c>true</c> if the element is found, <c>false</c> otherwise.</returns>
        public bool MoveToElement(string elementName)
        {
            var nodeType = Xml.MoveToContent();
            return  nodeType == XmlNodeType.Element &&
                    Xml.Name == elementName;
        }

        /// <summary>
        /// Reads an object from the XML.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute to use.</param>
        /// <returns>The deserialized object of type T.</returns>
        public T ReadObject<T>(ContentSerializerAttribute format)
        {
            return ReadObject(format, Serializer.GetTypeSerializer(typeof(T)), default(T));
        }

        /// <summary>
        /// Reads an object from the XML.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute to use.</param>
        /// <param name="typeSerializer">The type serializer to use.</param>
        /// <returns>The deserialized object of type T.</returns>
        public T ReadObject<T>(ContentSerializerAttribute format, ContentTypeSerializer typeSerializer)
        {
            return ReadObject(format, typeSerializer, default(T));
        }

        /// <summary>
        /// Reads an object from the XML.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute of the object.</param>
        /// <param name="typeSerializer">The type serializer for the object.</param>
        /// <param name="existingInstance">An existing instance of the object.</param>
        /// <returns>The deserialized object of type T.</returns>
        /// <exception cref="InvalidContentException">
        /// Thrown when the element can not be found, is null or cannot be assigned.
        /// </exception>
        public T ReadObject<T>(ContentSerializerAttribute format, ContentTypeSerializer typeSerializer, T existingInstance)
        {
            if (!format.FlattenContent)
            {
                if (!MoveToElement(format.ElementName))
                    throw NewInvalidContentException(null, "Element '{0}' was not found.", format.ElementName);

                // Is the object null?
                var isNull = Xml.GetAttribute("Null");
                if (isNull != null && XmlConvert.ToBoolean(isNull))
                {
                    if (!format.AllowNull)
                        throw NewInvalidContentException(null, "Element '{0}' cannot be null.", format.ElementName);

                    Xml.Skip();
                    return default(T);
                }

                // Is the object overloading the serialized type?
                if (Xml.MoveToAttribute("Type"))
                {
                    var type = ReadTypeName();
                    if (type == null)
                        throw NewInvalidContentException(null, "Could not resolve type '{0}'.", Xml.ReadContentAsString());
                    if (!typeSerializer.TargetType.IsAssignableFrom(type))
                        throw NewInvalidContentException(null, "Type '{0}' is not assignable to '{1}'.", type.FullName, typeSerializer.TargetType.FullName);

                    typeSerializer = Serializer.GetTypeSerializer(type);
                    Xml.MoveToElement();
                }
            }

            return ReadRawObject(format, typeSerializer, existingInstance);
        }


        /// <summary>
        /// Reads an object from the XML.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute of the object.</param>
        /// <param name="existingInstance">An existing instance of the object.</param>
        /// <returns>The deserialized object of type T.</returns>
        public T ReadObject<T>(ContentSerializerAttribute format, T existingInstance)
        {
            return ReadObject(format, Serializer.GetTypeSerializer(typeof(T)), existingInstance);
        }

        /// <summary>
        /// Reads a raw object from the XML using the specified format and type serializer.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute of the object.</param>
        /// <returns>The deserialized object of type T.</returns>
        /// <exception cref="InvalidContentException">
        /// Thrown when the element can not be found or is null.
        /// </exception>
        public T ReadRawObject<T>(ContentSerializerAttribute format)
        {
            return ReadRawObject(format, Serializer.GetTypeSerializer(typeof(T)), default(T));
        }

        /// <summary>
        /// Reads an object from the XML.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute of the object.</param>
        /// <param name="typeSerializer">The type serializer for the object.</param>
        /// <returns>The deserialized object of type T.</returns>
        public T ReadRawObject<T>(ContentSerializerAttribute format, ContentTypeSerializer typeSerializer)
        {
            return ReadRawObject(format, typeSerializer, default(T));
        }

        /// <summary>
        /// Reads a raw object from the XML using the specified format and type serializer.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute of the object.</param>
        /// <param name="typeSerializer">The type serializer for the object.</param>
        /// <param name="existingInstance">An existing instance of the object.</param>
        /// <returns>The deserialized object of type T.</returns>
        /// <exception cref="InvalidContentException">
        /// Thrown when the element can not be found or is null.
        /// </exception>
        public T ReadRawObject<T>(ContentSerializerAttribute format, ContentTypeSerializer typeSerializer, T existingInstance)
        {
            if (format.FlattenContent)
            {
                Xml.MoveToContent();
                return (T)typeSerializer.Deserialize(this, format, existingInstance);
            }

            if (!MoveToElement(format.ElementName))
                throw NewInvalidContentException(null, "Element '{0}' was not found.", format.ElementName);

            var isEmpty = Xml.IsEmptyElement;
            if (!isEmpty)
                Xml.ReadStartElement();

            var result = typeSerializer.Deserialize(this, format, existingInstance);

            if (isEmpty)
                Xml.Skip();

            if (!isEmpty)
                Xml.ReadEndElement();

            return (T)result;
        }

        /// <summary>
        /// Reads a raw object from the XML using the specified format and type serializer.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="format">The format attribute of the object.</param>
        /// <param name="existingInstance">An existing instance of the object.</param>
        /// <returns>The deserialized object of type T.</returns>
        /// <exception cref="InvalidContentException">
        /// Thrown when the element can not be found or is null.
        /// </exception>
        public T ReadRawObject<T>(ContentSerializerAttribute format, T existingInstance)
        {
            return ReadRawObject(format, Serializer.GetTypeSerializer(typeof(T)), existingInstance);
        }

        /// <summary>
        /// Reads a shared resource from the XML using the specified format and fixup action.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <param name="format">The format attribute of the resource.</param>
        /// <param name="fixup">The fixup action to apply to the resource.</param>
        /// <exception cref="InvalidContentException">
        /// Thrown if the element specified by the format attribute is not found.
        /// </exception>
        public void ReadSharedResource<T>(ContentSerializerAttribute format, Action<T> fixup)
        {
            string str;

            if (format.FlattenContent)
                str = Xml.ReadContentAsString();
            else
            {
                if (!MoveToElement(format.ElementName))
                    throw NewInvalidContentException(null, "Element '{0}' was not found.", format.ElementName);

                str = Xml.ReadElementContentAsString();
            }

            if (string.IsNullOrEmpty(str))
                return;

            // Do we already have one for this?
            Action<object> prevFixup;
            if (!_resourceFixups.TryGetValue(str, out prevFixup))
                _resourceFixups.Add(str, (o) => fixup((T)o));
            else
            {
                _resourceFixups[str] = (o) =>
                {
                    prevFixup(o);
                    fixup((T)o);
                };
            }
        }

        internal void ReadSharedResources()
        {
            if (!MoveToElement("Resources"))
                return;

            var resources = new Dictionary<string, object>();
            var resourceFormat = new ContentSerializerAttribute { ElementName = "Resource" };

            // Read all the resources.
            Xml.ReadStartElement();
            while (MoveToElement("Resource"))
            {
                var id = Xml.GetAttribute("ID");
                var resource = ReadObject<object>(resourceFormat);
                resources.Add(id, resource);
            }
            Xml.ReadEndElement();

            // Execute the fixups.
            foreach (var fixup in _resourceFixups)
            {
                object resource;
                if (!resources.TryGetValue(fixup.Key, out resource))
                    throw new InvalidContentException("Missing shared resource \"" + fixup.Key + "\".");
                fixup.Value(resource);
            }
        }

        /// <summary>
        /// Reads an external reference of a given type.
        /// </summary>
        /// <param name="existingInstance">The existing instance of the ExternalReference.</param>
        /// <typeparam name="T">The type of the external reference.</typeparam>
        /// <exception cref="InvalidContentException">
        /// Thrown if the external reference type is invalid.
        /// </exception>
        public void ReadExternalReference<T>(ExternalReference<T> existingInstance)
        {
            if (!MoveToElement("Reference"))
                return;

            var str = Xml.ReadElementContentAsString();

            Action<Type, string> fixup = (type, filename) =>
            {
                if (type != typeof(T))
                    throw NewInvalidContentException(null, "Invalid external reference type");

                existingInstance.Filename = filename;
            };

            List<Action<Type, string>> fixups;
            if (!_externalReferences.TryGetValue(str, out fixups))
                _externalReferences.Add(str, fixups = new List<Action<Type, string>>());
            fixups.Add(fixup);
        }

        internal void ReadExternalReferences()
        {
            if (!MoveToElement("ExternalReferences"))
                return;

            var currentDir = Path.GetDirectoryName(_filePath);

            // Read all the external references.
            Xml.ReadStartElement();
            while (MoveToElement("ExternalReference"))
            {
                List<Action<Type, string>> fixups;
                var id = Xml.GetAttribute("ID");
                if (!_externalReferences.TryGetValue(id, out fixups))
                    throw NewInvalidContentException(null, "Unknown external reference id '{0}'!", id);

                Xml.MoveToAttribute("TargetType");
                var targetType = ReadTypeName();
                if (targetType == null)
                    throw NewInvalidContentException(null, "Could not resolve type '{0}'.", Xml.ReadContentAsString());

                Xml.MoveToElement();
                var filename = Xml.ReadElementString();
                filename = Path.Combine(currentDir, filename);

                // Apply the fixups.
                foreach (var fixup in fixups)
                    fixup(targetType, filename);
            }
            Xml.ReadEndElement();
        }

        internal InvalidContentException NewInvalidContentException(Exception innerException, string message, params object[] args)
        {
            var xmlInfo = (IXmlLineInfo)Xml;
            var lineAndColumn = string.Format("{0},{1}", xmlInfo.LineNumber, xmlInfo.LinePosition);
            var identity = new ContentIdentity(_filePath, string.Empty, lineAndColumn);
            return new InvalidContentException(string.Format(message, args), identity, innerException);
        }

        /// <summary>
        /// Reads the next type in the
        /// </summary>
        /// <returns></returns>
        public Type ReadTypeName()
        {
            var typeName = Xml.ReadContentAsString();
            return Serializer.FindType(typeName);
        }
    }
}
