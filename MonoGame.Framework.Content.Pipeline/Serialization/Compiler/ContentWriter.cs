// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Utilities.LZ4;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Provides an implementation for many of the ContentCompiler methods including compilation, state tracking for shared resources and creation of the header type manifest.
    /// </summary>
    /// <remarks>A new ContentWriter is constructed for each compilation operation.</remarks>
    public sealed class ContentWriter : BinaryWriter
    {
        const byte XnbFormatVersion = 5;
        const byte HiDefContent = 0x01;
        const byte ContentCompressedLzx = 0x80;
        const byte ContentCompressedLz4 = 0x40;
        const int HeaderSize = 6;

        ContentCompiler compiler;
        TargetPlatform targetPlatform;
        GraphicsProfile targetProfile;
        string rootDirectory;
        string referenceRelocationPath;
        bool compressContent;
        bool disposed;
        List<ContentTypeWriter> typeWriters = new List<ContentTypeWriter>();
        Dictionary<Type, int> typeWriterMap = new Dictionary<Type, int>();
        Dictionary<Type, ContentTypeWriter> typeMap = new Dictionary<Type, ContentTypeWriter>();
        List<object> sharedResources = new List<object>();
        Dictionary<object, int> sharedResourceMap = new Dictionary<object, int>();
        Stream outputStream;
        Stream bodyStream;

        // This array must remain in sync with TargetPlatform
        static char[] targetPlatformIdentifiers = new[]
        {
            'w', // Windows (DirectX)
            'x', // Xbox360
            'i', // iOS
            'a', // Android
            'd', // DesktopGL
            'X', // MacOSX
            'n', // NativeClient
            'r', // RaspberryPi
            'P', // PlayStation4
            '5', // PlayStation5
            'O', // XboxOne
            'S', // Nintendo Switch
            'b', // WebAssembly and Bridge.NET
            'V', // DesktopVK (Vulkan)
            'G', // Windows GDK
            's', // Xbox Series
        };

        /// <summary>
        /// Gets the content build target platform.
        /// </summary>
        public TargetPlatform TargetPlatform { get { return targetPlatform; } }

        /// <summary>
        /// Gets or sets the target graphics profile.
        /// </summary>
        public GraphicsProfile TargetProfile { get { return targetProfile; } }

        /// <summary>
        /// Creates a new instance of ContentWriter.
        /// </summary>
        /// <param name="compiler">The compiler object that created this writer.</param>
        /// <param name="output">The stream to write the XNB file to.</param>
        /// <param name="targetPlatform">The platform the XNB is intended for.</param>
        /// <param name="targetProfile">The graphics profile of the target.</param>
        /// <param name="compressContent">True if the content should be compressed.</param>
        /// <param name="rootDirectory">The root directory of the content.</param>
        /// <param name="referenceRelocationPath">The path of the XNB file, used to calculate relative paths for external references.</param>
        internal ContentWriter(ContentCompiler compiler, Stream output, TargetPlatform targetPlatform, GraphicsProfile targetProfile, bool compressContent, string rootDirectory, string referenceRelocationPath)
            : base(output)
        {
            this.compiler = compiler;
            this.targetPlatform = targetPlatform;
            this.targetProfile = targetProfile;
            this.compressContent = compressContent;
            this.rootDirectory = rootDirectory;

            // Normalize the directory format so PathHelper.GetRelativePath will compute external references correctly.
            this.referenceRelocationPath = PathHelper.NormalizeDirectory(referenceRelocationPath);

            outputStream = this.OutStream;
            bodyStream = new MemoryStream();
            this.OutStream = bodyStream;
        }

        /// <summary>
        /// Releases the resources used by the IDisposable class.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Make sure the binary writer has the original stream back
                    this.OutStream = outputStream;

                    // Dispose managed resources we allocated
                    if (bodyStream != null)
                        bodyStream.Dispose();
                    bodyStream = null;
                }
                disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// All content has been written, so now finalize the header, footer and anything else that needs finalizing.
        /// </summary>
        public void FinalizeContent()
        {
            // Write shared resources to the end of body stream
            WriteSharedResources();

            using (var contentStream = new MemoryStream())
            {
                this.OutStream = contentStream;
                WriteTypeWriters();
                bodyStream.Position = 0;
                bodyStream.CopyTo(contentStream);
                contentStream.Position = 0;

                // Before we write the header, try to compress the body stream. If compression fails, we want to
                // turn off the compressContent flag so the correct flags are written in the header
                Stream compressedStream = null;
                try
                {
                    if (compressContent)
                    {
                        compressedStream = new MemoryStream();
                        this.OutStream = compressedStream;
                        if (!WriteCompressedStream(contentStream))
                        {
                            // The compression failed (sometimes LZ4 does fail, for various reasons), so just write
                            // it out uncompressed.
                            compressContent = false;
                            compressedStream.Dispose();
                            compressedStream = null;
                        }
                    }

                    this.OutStream = outputStream;
                    WriteHeader();
                    if (compressedStream != null)
                    {
                        compressedStream.Position = 0;
                        compressedStream.CopyTo(outputStream);
                    }
                    else
                    {
                        WriteUncompressedStream(contentStream);
                    }
                }
                finally
                {
                    if (compressedStream != null)
                        compressedStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Write the table of content type writers.
        /// </summary>
        void WriteTypeWriters()
        {
            Write7BitEncodedInt(typeWriters.Count);
            foreach (var typeWriter in typeWriters)
            {
                Write(typeWriter.GetRuntimeReader(targetPlatform));
                Write(typeWriter.TypeVersion);
            }
            Write7BitEncodedInt(sharedResources.Count);
        }

        /// <summary>
        /// Write the header to the output stream.
        /// </summary>
        void WriteHeader()
        {
            Write('X');
            Write('N');
            Write('B');
            Write(targetPlatformIdentifiers[(int)targetPlatform]);
            Write(XnbFormatVersion);
            // We cannot use LZX compression, so we use the public domain LZ4 compression. Use one of the spare bits in the flags byte to specify LZ4.
            byte flags = (byte)((targetProfile == GraphicsProfile.HiDef ? HiDefContent : (byte)0) | (compressContent ? ContentCompressedLz4 : (byte)0));
            Write(flags);
        }

        /// <summary>
        /// Write all shared resources at the end of the file.
        /// </summary>
        void WriteSharedResources()
        {
            for (int i = 0; i < sharedResources.Count; i++)
            {
                var resource = sharedResources[i];
                WriteObject<object>(resource);
            }
        }

        /// <summary>
        /// Compress the stream and write it to the output.
        /// </summary>
        /// <param name="stream">The stream to compress and write to the output.</param>
        /// <returns>true if the write succeeds</returns>
        bool WriteCompressedStream(MemoryStream stream)
        {
            // Compress stream
            var maxLength = LZ4Codec.MaximumOutputLength((int)stream.Length);
            var outputArray = new byte[maxLength * 2];
            int resultLength = LZ4Codec.Encode32HC(stream.GetBuffer(), 0, (int)stream.Length, outputArray, 0, maxLength);
            if (resultLength < 0)
                return false;
            UInt32 totalSize = (UInt32)(HeaderSize + resultLength + sizeof(UInt32) + sizeof(UInt32));
            Write(totalSize);
            Write((int)stream.Length);
            OutStream.Write(outputArray, 0, resultLength);
            return true;
        }

        /// <summary>
        /// Write the uncompressed stream to the output.
        /// </summary>
        /// <param name="stream">The stream to write to the output.</param>
        /// <returns>true if the write succeeds</returns>
        bool WriteUncompressedStream(Stream stream)
        {
            UInt32 totalSize = (UInt32)(HeaderSize + stream.Length + sizeof(UInt32));
            Write(totalSize);
            stream.CopyTo(OutStream);
            return true;
        }

        /// <summary>
        /// Gets a ContentTypeWriter for the given type.
        /// </summary>
        /// <param name="type">The type of the object to write.</param>
        /// <returns>The ContentTypeWriter for the type.</returns>
        internal ContentTypeWriter GetTypeWriter(Type type)
        {
            ContentTypeWriter typeWriter = null;
            if (!typeMap.TryGetValue(type, out typeWriter))
            {
                int index = typeWriters.Count;
                typeWriter = compiler.GetTypeWriter(type);

                typeWriters.Add(typeWriter);
                if (!typeWriterMap.ContainsKey(typeWriter.GetType()))
                    typeWriterMap.Add(typeWriter.GetType(), index);

                typeMap.Add(type, typeWriter);

                typeWriter.OnAddedToContentWriter(this);
            }
            return typeWriter;
        }

        /// <summary>
        /// Writes the name of an external file to the output binary.
        /// </summary>
        /// <typeparam name="T">The type of reference.</typeparam>
        /// <param name="reference">External reference to a data file for the content item.</param>
        public void WriteExternalReference<T>(ExternalReference<T> reference)
        {
            if (reference == null)
            {
                Write(string.Empty);
            }
            else
            {
                string fileName = reference.Filename;
                if (string.IsNullOrEmpty(fileName))
                {
                    Write(string.Empty);
                }
                else
                {
                    // Make sure the filename ends with .xnb
                    if (!fileName.EndsWith(".xnb"))
                        throw new ArgumentException(string.Format("ExternalReference '{0}' must reference a .xnb file", fileName));
                    // Make sure it is in the same root directory
                    if (!fileName.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase))
                        throw new ArgumentException(string.Format("ExternalReference '{0}' must be in the root directory '{1}'", fileName, rootDirectory));
                    // Strip the .xnb extension
                    fileName = fileName.Substring(0, fileName.Length - 4);
                    // Get the relative directory
                    fileName = PathHelper.GetRelativePath(referenceRelocationPath, fileName);
                    Write(fileName);
                }
            }
        }

        /// <summary>
        /// Writes a single object preceded by a type identifier to the output binary.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value to write.</param>
        /// <remarks>This method can be called recursively with a null value.</remarks>
        public void WriteObject<T>(T value)
        {
            if (value == null)
                Write7BitEncodedInt(0);
            else
            {
                var typeWriter = GetTypeWriter(value.GetType());

                // Because zero means null object, we add one to 
                // the index before writing it to the file.
                var index = typeWriterMap[typeWriter.GetType()];
                Write7BitEncodedInt(index + 1);

                typeWriter.Write(this, value);                
            }
        }

        /// <summary>
        /// Writes a single object to the output binary, using the specified type hint and writer worker.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value to write.</param>
        /// <param name="typeWriter">The content type writer.</param>
        /// <remarks>The type hint should be retrieved from the Initialize method of the ContentTypeWriter
        /// that is calling WriteObject, by calling GetTypeWriter and passing it the type of the field used
        /// to hold the value being serialized.
        /// </remarks>
        public void WriteObject<T>(T value, ContentTypeWriter typeWriter)
        {
            if (typeWriter == null)
                throw new ArgumentNullException("typeWriter");

            if (typeWriter.TargetType.IsValueType)
                typeWriter.Write(this, value);
            else
                WriteObject(value);
        }

        /// <summary>
        /// Writes a single object to the output binary as an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value to write.</param>
        /// <remarks>If you specify a base class of the actual object value only data from this base type
        /// will be written. This method does not write any type identifier so it cannot support null or
        /// polymorphic values, and the reader must specify an identical type while loading the compiled data.</remarks>
        public void WriteRawObject<T>(T value)
        {
            WriteRawObject<T>(value, GetTypeWriter(typeof(T)));
        }

        /// <summary>
        /// Writes a single object to the output binary using the specified writer worker.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value to write.</param>
        /// <param name="typeWriter">The writer worker. This should be looked up from the Initialize method
        /// of the ContentTypeWriter that is calling WriteRawObject, by calling GetTypeWriter.</param>
        /// <remarks>WriteRawObject does not write any type identifier, so it cannot support null or polymorphic
        /// values, and the reader must specify an identical type while loading the compiled data.</remarks>
        public void WriteRawObject<T>(T value, ContentTypeWriter typeWriter)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (typeWriter == null)
                throw new ArgumentNullException("typeWriter");

            typeWriter.Write(this, value);
        }

        /// <summary>
        /// Adds a shared reference to the output binary and records the object to be serialized later.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The object to record.</param>
        public void WriteSharedResource<T>(T value)
        {
            if (value == null)
            {
                // Zero means a null value
                Write7BitEncodedInt(0);
            }
            else
            {
                int index;
                if (!sharedResourceMap.TryGetValue(value, out index))
                {
                    // Add it to the list of shared resources
                    index = sharedResources.Count;
                    sharedResources.Add(value);
                    sharedResourceMap.Add(value, index);
                }
                // Because zero means null value, we add one before writing the index to the file
                Write7BitEncodedInt(index + 1);
            }
        }

        /// <summary>
        /// Writes a Color value.
        /// </summary>
        /// <param name="value">Value of a color using Red, Green, Blue, and Alpha values to write.</param>
        public void Write(Color value)
        {
            Write(value.R);
            Write(value.G);
            Write(value.B);
            Write(value.A);
        }

        /// <summary>
        /// Writes a Matrix value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void Write(Matrix value)
        {
            Write(value.M11);
            Write(value.M12);
            Write(value.M13);
            Write(value.M14);
            Write(value.M21);
            Write(value.M22);
            Write(value.M23);
            Write(value.M24);
            Write(value.M31);
            Write(value.M32);
            Write(value.M33);
            Write(value.M34);
            Write(value.M41);
            Write(value.M42);
            Write(value.M43);
            Write(value.M44);
        }

        /// <summary>
        /// Writes a Matrix value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void Write(Quaternion value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }

        /// <summary>
        /// Writes a Vector2 value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }

        /// <summary>
        /// Writes a Vector3 value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        /// <summary>
        /// Writes a Vector4 value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void Write(Vector4 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }

        /// <summary>
        /// Writes a BoundingSphere value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        internal void Write(BoundingSphere value)
        {
            Write(value.Center);
            Write(value.Radius);
        }

        /// <summary>
        /// Writes a Rectangle value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        internal void Write(Rectangle value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Width);
            Write(value.Height);
        }

        /// <summary>
        /// Helper for checking if a type can be deserialized into an existing object.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type can be deserialized into an existing object.</returns>
        internal bool CanDeserializeIntoExistingObject(Type type)
        {
            var typeWriter = compiler.GetTypeWriter(type);
            return typeWriter != null && typeWriter.CanDeserializeIntoExistingObject;
        }
    }
}
