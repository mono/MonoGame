// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Flags that describe style information to be applied to text.
    /// You can combine these flags by using a bitwise OR operator (|).
    /// </summary>
    [Flags]
    public enum FontDescriptionStyle
    {
        /// <summary>
        /// Normal text.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Bold text.
        /// </summary>
        Bold,

        /// <summary>
        /// Italic text.
        /// </summary>
        Italic,
    }
}