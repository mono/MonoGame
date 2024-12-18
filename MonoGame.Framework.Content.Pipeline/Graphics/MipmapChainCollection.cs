// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides methods for maintaining a mipmap chain.
    /// </summary>
    public sealed class MipmapChainCollection : Collection<MipmapChain>
    {
        private readonly bool _fixedSize;

        private const string CannotResizeError = "Cannot resize MipmapChainCollection. This type of texture has a fixed number of faces.";

        internal MipmapChainCollection(int count, bool fixedSize)
        {
            for (var i = 0; i < count; i++)
                Add(new MipmapChain());

            _fixedSize = fixedSize;
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            if (_fixedSize)
                throw new NotSupportedException(CannotResizeError);

            base.ClearItems();
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            if (_fixedSize)
                throw new NotSupportedException(CannotResizeError);

            base.RemoveItem(index);
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, MipmapChain item)
        {
            if (_fixedSize)
                throw new NotSupportedException(CannotResizeError);

            base.InsertItem(index, item);
        }
    }
}
