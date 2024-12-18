// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties and methods for creating and maintaining an ATC interpolated compressed bitmap resource.
    /// </summary>
    public class AtcInterpolatedBitmapContent : AtcBitmapContent
    {
        /// <summary>
        /// Creates an instance of AtcInterpolatedBitmapContent.
        /// </summary>
        public AtcInterpolatedBitmapContent()
        {
        }

        /// <summary>
        /// Creates an instance of AtcInterpolatedBitmapContent with the specified width and height.
        /// </summary>
        /// <param name="width">The width in pixels of the bitmap.</param>
        /// <param name="height">The height in pixels of the bitmap.</param>
        public AtcInterpolatedBitmapContent(int width, int height)
            : base(width, height)
        {
        }

        /// <summary>
        /// Gets the corresponding GPU texture format for the specified bitmap type.
        /// </summary>
        /// <param name="format">Format being retrieved.</param>
        /// <returns>The GPU texture format of the bitmap type.</returns>
        public override bool TryGetFormat(out SurfaceFormat format)
        {
            format = SurfaceFormat.RgbaAtcInterpolatedAlpha;
            return true;
        }

        /// <summary>
        /// Returns a string description of the bitmap.
        /// </summary>
        /// <returns>Description of the bitmap.</returns>
        public override string ToString()
        {
            return "ATITC Interpolated Alpha " + Width + "x" + Height;
        }
    }
}
