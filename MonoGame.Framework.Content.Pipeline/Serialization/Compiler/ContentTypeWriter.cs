// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Provides methods and properties for compiling a specific managed type into a binary format.
    /// </summary>
    public abstract class ContentTypeWriter
    {
        private readonly Type _targetType;
        /// <summary/>
        protected int _typeVersion;

        /// <summary>
        /// Determines if deserialization into an existing object is possible.
        /// </summary>
        /// <value>true if the object can be deserialized into; false otherwise.</value>
        public virtual bool CanDeserializeIntoExistingObject
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the type handled by this compiler component.
        /// </summary>
        /// <value>The type handled by this compiler component.</value>
        public Type TargetType { get { return _targetType; } }

        /// <summary>
        /// Gets a format version number for this type.
        /// </summary>
        /// <value>A format version number for this type.</value>
        public virtual int TypeVersion { get { return _typeVersion; } }

        /// <summary>
        /// Initializes a new instance of the ContentTypeWriter class.
        /// </summary>
        /// <param name="targetType"></param>
        protected ContentTypeWriter(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException();

            _targetType = targetType;
        }

        /// <summary>
        /// Gets the assembly qualified name of the runtime loader for this type.
        /// </summary>
        /// <param name="targetPlatform">Name of the platform.</param>
        /// <returns>Name of the runtime loader.</returns>
        public abstract string GetRuntimeReader(TargetPlatform targetPlatform);

        /// <summary>
        /// Gets the assembly qualified name of the runtime target type. The runtime target type often matches the design time type, but may differ.
        /// </summary>
        /// <param name="targetPlatform">The target platform.</param>
        /// <returns>The qualified name.</returns>
        public virtual string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return _targetType.FullName + ", " + _targetType.Assembly.FullName;
        }

        /// <summary>
        /// Retrieves and caches nested type writers and allows for reflection over the target data type. Called by the framework at creation time.
        /// </summary>
        /// <param name="compiler">The content compiler.</param>
        protected virtual void Initialize(ContentCompiler compiler)
        {

        }

        /// <summary>
        /// Allows type writers to add their element type writers to the content writer.
        /// </summary>
        /// <param name="writer">The content writer.</param>
        internal virtual void OnAddedToContentWriter(ContentWriter writer)
        {

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
        protected internal virtual bool ShouldCompressContent(TargetPlatform targetPlatform, object value)
        {
            // For now, only support uncompressed
            return false;

            //switch (targetPlatform)
            //{
            //    case TargetPlatform.Windows:
            //    case TargetPlatform.Linux:
            //    case TargetPlatform.MacOSX:
            //        return true;
            //    default:
            //        return false;
            //}
        }

        /// <summary>
        /// Compiles an object into binary format.
        /// </summary>
        /// <param name="output">The content writer serializing the value.</param>
        /// <param name="value">The resultant object.</param>
        protected internal abstract void Write(ContentWriter output, object value);
    }
}
