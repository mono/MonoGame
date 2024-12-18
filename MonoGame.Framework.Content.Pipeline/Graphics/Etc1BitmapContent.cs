// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Content.Pipeline.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Supports the processing of a texture compressed using ETC1.
    /// </summary>
    public class Etc1BitmapContent : BitmapContent
    {
        byte[] _data;

        /// <summary>
        /// Initializes a new instance of Etc1BitmapContent.
        /// </summary>
        protected Etc1BitmapContent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of Etc1BitmapContent with the specified width or height.
        /// </summary>
        /// <param name="width">Width in pixels of the bitmap resource.</param>
        /// <param name="height">Height in pixels of the bitmap resource.</param>
        public Etc1BitmapContent(int width, int height)
            : base(width, height)
        {
        }

        /// <inheritdoc/>
        public override byte[] GetPixelData()
        {
            return _data;
        }

        /// <inheritdoc/>
        public override void SetPixelData(byte[] sourceData)
        {
            int bytesRequired = ((Width + 3) >> 2) * ((Height + 3) >> 2) * SurfaceFormat.RgbEtc1.GetSize();
            if (bytesRequired != sourceData.Length)
                throw new ArgumentException("ETC1 bitmap with width " + Width + " and height " + Height + " needs "
                    + bytesRequired + " bytes. Received " + sourceData.Length + " bytes");

            if (_data == null || _data.Length != bytesRequired)
                _data = new byte[bytesRequired];
            Buffer.BlockCopy(sourceData, 0, _data, 0, bytesRequired);
        }

        /// <inheritdoc/>
        protected override bool TryCopyFrom(BitmapContent sourceBitmap, Rectangle sourceRegion, Rectangle destinationRegion)
        {
            SurfaceFormat sourceFormat;
            if (!sourceBitmap.TryGetFormat(out sourceFormat))
                return false;

            SurfaceFormat format;
            TryGetFormat(out format);

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (SurfaceFormat.RgbEtc1 == sourceFormat && (sourceRegion == new Rectangle(0, 0, Width, Height)) && sourceRegion == destinationRegion)
            {
                SetPixelData(sourceBitmap.GetPixelData());
                return true;
            }

            // Destination region copy is not yet supported
            if (destinationRegion != new Rectangle(0, 0, Width, Height))
                return false;

            // If the source is not Vector4 or requires resizing, send it through BitmapContent.Copy
            if (!(sourceBitmap is PixelBitmapContent<Vector4>) || sourceRegion.Width != destinationRegion.Width || sourceRegion.Height != destinationRegion.Height)
            {
                try
                {
                    BitmapContent.Copy(sourceBitmap, sourceRegion, this, destinationRegion);
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }

            Crunch.EncodeBytes(
                sourceBitmap: sourceBitmap,
                crunchFormat: CrunchFormat.Etc1,
                out var compressedBytes);
            SetPixelData(compressedBytes);

            return true;
        }

        /// <inheritdoc/>
        protected override bool TryCopyTo(BitmapContent destinationBitmap, Rectangle sourceRegion, Rectangle destinationRegion)
        {
            SurfaceFormat destinationFormat;
            if (!destinationBitmap.TryGetFormat(out destinationFormat))
                return false;

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (SurfaceFormat.RgbEtc1 == destinationFormat && (sourceRegion == new Rectangle(0, 0, Width, Height)) && sourceRegion == destinationRegion)
            {
                destinationBitmap.SetPixelData(GetPixelData());
                return true;
            }

            // No other support for copying from a ETC1 texture yet
            return false;
        }

        /// <summary>
        /// Gets the corresponding GPU texture format for the specified bitmap type.
        /// </summary>
        /// <param name="format">Format being retrieved.</param>
        /// <returns>The GPU texture format of the bitmap type.</returns>
        public override bool TryGetFormat(out SurfaceFormat format)
        {
            format = SurfaceFormat.RgbEtc1;
            return true;
        }

        /// <summary>
        /// Returns a string description of the bitmap.
        /// </summary>
        /// <returns>Description of the bitmap.</returns>
        public override string ToString()
        {
            return "ETC1 " + Width + "x" + Height;
        }
    }
}
