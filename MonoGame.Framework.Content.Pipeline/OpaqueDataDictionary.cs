// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides an opaque data dictionary for use in the Content Pipeline.
    /// </summary>
    /// <remarks>
    /// This is equivilant to a <see cref="NamedValueDictionary{T}">NamedValueDictionary</see> where the type of the values is Object.
    /// </remarks>
    [ContentSerializerCollectionItemName("Data")]
    public sealed class OpaqueDataDictionary : NamedValueDictionary<Object>
    {
        /// <summary>
        /// Get the value for the specified key
        /// </summary>
        /// <key>The key of the item to retrieve.</key>
        /// <defaultValue>The default value to return if the key does not exist.</defaultValue>
        /// <returns>The item cast as T, or the default value if the item is not present in the dictonary.</returns>
        public T GetValue<T> (string key, T defaultValue)
        {
            object o;
            if (TryGetValue (key, out o))
                return (T)o ;
            return defaultValue;
        }
    }
}
