// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using BCnEncoder.Shared;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Utilities;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties and methods for creating and maintaining an ATC compressed bitmap resource.
    /// </summary>
    public abstract class AtcBitmapContent : BitmapContent
    {
        internal byte[] _bitmapData;

        /// <summary>
        /// Initializes a new instance of AtcBitmapContent.
        /// </summary>
        public AtcBitmapContent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of AtcBitmapContent with the specified width or height.
        /// </summary>
        /// <param name="width">Width, in pixels, of the bitmap resource.</param>
        /// <param name="height">Height, in pixels, of the bitmap resource.</param>
        public AtcBitmapContent(int width, int height)
            : base(width, height)
        {
        }

        /// <inheritdoc/>
        public override byte[] GetPixelData()
        {
            return _bitmapData;
        }

        /// <inheritdoc/>
        public override void SetPixelData(byte[] sourceData)
        {
            _bitmapData = sourceData;
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
            if (format == sourceFormat && (sourceRegion == new Rectangle(0, 0, Width, Height)) && sourceRegion == destinationRegion)
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

            CompressionFormat compressionFormat;
            switch (format)
            {
                case SurfaceFormat.RgbaAtcExplicitAlpha:
                    compressionFormat = CompressionFormat.AtcExplicitAlpha;
                    break;
                case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                    compressionFormat = CompressionFormat.AtcInterpolatedAlpha;
                    break;
                default:
                    throw new PipelineException();
            }
            BcnUtil.Encode(
                sourceBitmap: sourceBitmap,
                destinationFormat: compressionFormat,
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

            SurfaceFormat format;
            TryGetFormat(out format);

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (format == destinationFormat && (sourceRegion == new Rectangle(0, 0, Width, Height)) && sourceRegion == destinationRegion)
            {
                destinationBitmap.SetPixelData(GetPixelData());
                return true;
            }

            // No other support for copying from an ATC texture yet
            return false;
        }
    }
}
