// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate
{
    // The intermediate serializer implementation is based on testing XNA behavior and the following sources:
    //
    // http://msdn.microsoft.com/en-us/library/Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate.aspx
    // http://blogs.msdn.com/b/shawnhar/archive/2008/08/12/everything-you-ever-wanted-to-know-about-intermediateserializer.aspx
    // http://blogs.msdn.com/b/shawnhar/archive/2008/08/26/customizing-intermediateserializer-part-1.aspx
    // http://blogs.msdn.com/b/shawnhar/archive/2008/08/26/customizing-intermediateserializer-part-2.aspx
    // http://blogs.msdn.com/b/shawnhar/archive/2008/08/27/why-intermediateserializer-control-attributes-are-not-part-of-the-content-pipeline.aspx
    //

    /// <summary>
    /// Provides methods for reading and writing XNA intermediate XML format.
    /// </summary>
    public class IntermediateSerializer
    {
        /// <summary>
        /// According to the examples on Sean Hargreaves' blog, explicit types
        /// can also specify the type aliases from C#. This maps those names
        /// to the actual .NET framework types for parsing.
        /// </summary>
        private static readonly Dictionary<string, Type> _typeAliases = new Dictionary<string, Type>
        {
            { "bool",   typeof(bool) },
            { "byte",   typeof(byte) },
            { "sbyte",  typeof(sbyte) },
            { "char",   typeof(char) },
            { "decimal",typeof(decimal) },
            { "double", typeof(double) },
            { "float",  typeof(float) },
            { "int",    typeof(int) },
            { "uint",   typeof(uint) },
            { "long",   typeof(long) },
            { "ulong",  typeof(ulong) },
            { "object", typeof(object) },
            { "short",  typeof(short) },
            { "ushort", typeof(ushort) },
            { "string", typeof(string) }
        };

        private static readonly Dictionary<Type, string> _typeAliasesReverse;

        static IntermediateSerializer()
        {
            _typeAliasesReverse = _typeAliases.ToDictionary(x => x.Value, x => x.Key);
        }

        private IntermediateSerializer()
        {
            _scannedObjects = new List<object>();
            _namespaceAliasHelper = new NamespaceAliasHelper(this);
        }

        /// <summary>
        /// Maps "ShortName:" -> "My.Namespace.LongName." for type lookups.
        /// </summary>
        private Dictionary<string, string> _namespaceLookup;

        private Dictionary<Type, ContentTypeSerializer> _serializers;
        private Dictionary<Type, GenericCollectionHelper> _collectionHelpers;

        private Dictionary<Type, Type> _genericSerializerTypes;

        private readonly NamespaceAliasHelper _namespaceAliasHelper;

        private readonly List<object> _scannedObjects;

        /// <summary>
        /// Deserializes an object using the IntermediateSerializer and IntermediateReader.
        /// </summary>
        /// <param name="input">The XmlReader to read from.</param>
        /// <param name="referenceRelocationPath">The path to relocate any relative references to.</param>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <returns>The deserialized object of type T.</returns>
        /// <exception cref="InvalidContentException">Thrown if no content is found.</exception>
        /// <exception cref="XmlException">Thrown if an error occurs parsing the XML.</exception>
        public static T Deserialize<T>(XmlReader input, string referenceRelocationPath)
        {
            var serializer = new IntermediateSerializer();
            var reader = new IntermediateReader(serializer, input, referenceRelocationPath);
            var asset = default(T);

            try
            {
                if (!reader.MoveToElement("XnaContent"))
                    throw new InvalidContentException(string.Format("Could not find XnaContent element in '{0}'.",
                                                                    referenceRelocationPath));

                // Initialize the namespace lookups from
                // the attributes on the XnaContent element.
                serializer.CreateNamespaceLookup(input);

                // Move past the XnaContent.
                input.ReadStartElement();

                // Read the asset.
                var format = new ContentSerializerAttribute {ElementName = "Asset"};
                asset = reader.ReadObject<T>(format);

                // Process the shared resources and external references.
                reader.ReadSharedResources();
                reader.ReadExternalReferences();

                // Move past the closing XnaContent element.
                input.ReadEndElement();
            }
            catch (XmlException xmlException)
            {
                throw reader.NewInvalidContentException(xmlException, "An error occured parsing.");
            }

            return asset;
        }

        /// <summary>
        /// Retrieves the serializer for the specified type.
        /// </summary>
        /// <param name="type">The type for which to retrieve the serializer.</param>
        /// <returns>The serializer for the specified type.</returns>
        /// <exception cref="RankException">Thrown if the type is not a single dimensional array.</exception>
        /// <exception cref="NotImplementedException">Thrown if there is no implementation for the type.</exception>
        public ContentTypeSerializer GetTypeSerializer(Type type)
        {
            // Create the known serializers if we haven't already.
            if (_serializers == null)
            {
                _serializers = new Dictionary<Type, ContentTypeSerializer>();
                _genericSerializerTypes = new Dictionary<Type, Type>();

                var types = ContentTypeSerializerAttribute.GetTypes();
                foreach (var t in types)
                {
                    if (t.IsGenericType)
                    {
                        var genericType = t.BaseType.GetGenericArguments()[0];
                        _genericSerializerTypes.Add(genericType.GetGenericTypeDefinition(), t);
                    }
                    else
                    {
                        var cts = Activator.CreateInstance(t) as ContentTypeSerializer;
                        cts.Initialize(this);
                        _serializers.Add(cts.TargetType, cts);
                    }
                }
            }

            // Look it up.
            ContentTypeSerializer serializer;
            if (_serializers.TryGetValue(type, out serializer))
                return serializer;

            Type serializerType;

            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1)
                    throw new RankException("We only support single dimension arrays.");

                var arrayType = typeof(ArraySerializer<>).MakeGenericType(new[] { type.GetElementType() });
                serializer = (ContentTypeSerializer)Activator.CreateInstance(arrayType);
            }
            else if (type.IsGenericType && _genericSerializerTypes.TryGetValue(type.GetGenericTypeDefinition(), out serializerType))
            {
                serializerType = serializerType.MakeGenericType(type.GetGenericArguments());
                serializer = (ContentTypeSerializer)Activator.CreateInstance(serializerType);
            }
            else if (type.IsEnum)
            {
                serializer = new EnumSerializer(type);
            }
            else if (typeof(IList).IsAssignableFrom(type) && !GenericCollectionHelper.IsGenericCollectionType(type, true))
            {
                // Special handling for non-generic IList types. By the time we get here,
                // generic collection types will already have been handled by one of the known serializers.
                serializer = new NonGenericIListSerializer(type);
            }
            else
            {
                // The reflective serializer is not for primitive types!
                if (type.IsPrimitive)
                    throw new NotImplementedException(string.Format("Unhandled primitive type `{0}`!", type.FullName));

                // We still don't have a serializer then we
                // fallback to the reflection based serializer.
                serializer = new ReflectiveSerializer(type);
            }

            Debug.Assert(serializer.TargetType == type, "Target type mismatch!");

            // We cache the serializer before we initialize it to
            // avoid a stack overflow on recursive types.
            _serializers.Add(type, serializer);
            serializer.Initialize(this);

            return serializer;
        }

        internal GenericCollectionHelper GetCollectionHelper(Type type)
        {
            if (_collectionHelpers == null)
                _collectionHelpers = new Dictionary<Type, GenericCollectionHelper>();

            GenericCollectionHelper result;
            if (!_collectionHelpers.TryGetValue(type, out result))
            {
                result = new GenericCollectionHelper(this, type);
                _collectionHelpers.Add(type, result);
            }
            return result;
        }

        /// <summary>
        /// Serializes the given value of type T to an XML writer.
        /// </summary>
        /// <typeparam name="T">The type of the value to serialize.</typeparam>
        /// <param name="output">The XML writer to write the serialized value to.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="referenceRelocationPath">The path to relocate any relative references to.</param>
        public static void Serialize<T>(XmlWriter output, T value, string referenceRelocationPath)
        {
            var serializer = new IntermediateSerializer();
            var writer = new IntermediateWriter(serializer, output, referenceRelocationPath);
            output.WriteStartElement("XnaContent");

            serializer._namespaceAliasHelper.WriteNamespaces(output, value);

            // Write the asset.
            var format = new ContentSerializerAttribute { ElementName = "Asset" };
            writer.WriteObject<object>(value, format);

            // Process the shared resources and external references.
            writer.WriteSharedResources();
            writer.WriteExternalReferences();

            // Close the XnaContent element.
            output.WriteEndElement();
        }

        /// <summary>
        /// Builds a lookup table from a short name to the full namespace.
        /// </summary>
        private void CreateNamespaceLookup(XmlReader reader)
        {
            _namespaceLookup = new Dictionary<string, string>();

            for (var i=0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                if (reader.Prefix != "xmlns")
                    continue;

                _namespaceLookup.Add(reader.LocalName + ":", reader.Value + ".");
            }
        }

        /// <summary>
        /// Finds the type in any assembly loaded into the AppDomain.
        /// </summary>
        internal Type FindType(string typeName)
        {
            Type foundType;

            typeName = typeName.Trim();

            // Shortcut for friendly C# names
            if (_typeAliases.TryGetValue(typeName, out foundType))
                return foundType;

            // If this is an array then handle it separately.
            if (typeName.EndsWith("[]"))
            {
                var arrayType = typeName.Substring(0, typeName.Length - 2);
                foundType = FindType(arrayType);
                return foundType == null ? null : foundType.MakeArrayType();
            }

            // Expand any namespaces in the asset type
            foreach (var pair in _namespaceLookup)
                typeName = typeName.Replace(pair.Key, pair.Value);
            var expandedName = typeName;

            // If this a generic type, handle it separately.
            if (typeName.EndsWith("]"))
            {
                var openBracketIndex = typeName.IndexOf("[");

                var typeNameWithoutArguments = typeName.Substring(0, openBracketIndex);

                var genericArgumentsString = typeName.Substring(openBracketIndex + 1, typeName.Length - openBracketIndex - 2);
                var genericArgumentsArray = genericArgumentsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var genericArguments = genericArgumentsArray.Select(FindType).ToArray();

                foundType = FindType(typeNameWithoutArguments + "`" + genericArguments.Length);
                return (foundType == null) ? null : foundType.MakeGenericType(genericArguments);
            }

            foundType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from type in assembly.GetTypes()
                         where type.FullName == typeName || type.Name == typeName
                         select type).FirstOrDefault();

            if (foundType == null)
                foundType = Type.GetType(expandedName, false, true);

            return foundType;
        }

        /// <summary>
        /// Gets the (potentially) aliased name for any type.
        /// </summary>
        internal string GetFullTypeName(Type type)
        {
            string typeName;

            // Shortcut for friendly C# names
            if (_typeAliasesReverse.TryGetValue(type, out typeName))
                return typeName;

            // Look for aliased namespace.
            if (_namespaceAliasHelper.TryGetAliasedTypeName(type, out typeName))
                return typeName;

            // Fallback to full type name.
            var typeNamespace = type.Namespace;
            if (!string.IsNullOrEmpty(typeNamespace))
                typeName = typeNamespace + ".";
            typeName += GetTypeName(type);

            return typeName;
        }

        /// <summary>
        /// Returns the name of the type, without the namespace.
        /// For generic types, we add the type parameters in square brackets.
        /// i.e. List&lt;int&gt; becomes List[int]
        /// </summary>
        internal string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var typeName = type.Name;
                int genericBacktickIndex = typeName.IndexOf("`");
                if (genericBacktickIndex >= 0)
                    typeName = typeName.Substring(0, genericBacktickIndex);

                var result = typeName + "[";
                result += string.Join(",", type.GetGenericArguments().Select(GetFullTypeName));
                result += "]";
                return result;
            }

            if (type.IsArray)
                return GetTypeName(type.GetElementType()) + "[]";

            if (type.IsNested)
                return type.DeclaringType.Name + "+" + type.Name;

            return type.Name;
        }

        internal bool AlreadyScanned(object value)
        {
            if (_scannedObjects.Contains(value))
                return true;
            _scannedObjects.Add(value);
            return false;
        }

        internal bool HasTypeAlias(Type type)
        {
            return _typeAliasesReverse.ContainsKey(type);
        }
    }
}
