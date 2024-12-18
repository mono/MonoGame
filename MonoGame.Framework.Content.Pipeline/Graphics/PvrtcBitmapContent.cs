// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Content.Pipeline.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties and methods for creating and maintaining a compressed bitmap resource.
    /// </summary>
    public abstract class PvrtcBitmapContent : BitmapContent
    {
        internal byte[] _bitmapData;

        /// <inheritdoc/>
        public PvrtcBitmapContent(int width, int height)
            : base(width, height)
        {
        }

        int GetDataSize()
        {
            SurfaceFormat format;
            TryGetFormat(out format);
            switch (format)
            {
                case SurfaceFormat.RgbPvrtc2Bpp:
                case SurfaceFormat.RgbaPvrtc2Bpp:
                    return (Math.Max(Width, 16) * Math.Max(Height, 8) * 2 + 7) / 8;

                case SurfaceFormat.RgbPvrtc4Bpp:
                case SurfaceFormat.RgbaPvrtc4Bpp:
                    return (Math.Max(Width, 8) * Math.Max(Height, 8) * 4 + 7) / 8;
            }

            return 0;
        }

        /// <inheritdoc/>
        public override byte[] GetPixelData()
        {
            if (_bitmapData == null)
                throw new InvalidOperationException("No data set on bitmap");
            var result = new byte[_bitmapData.Length];
            Buffer.BlockCopy(_bitmapData, 0, result, 0, _bitmapData.Length);
            return result;
        }

        /// <inheritdoc/>
        public override void SetPixelData(byte[] sourceData)
        {
            var size = GetDataSize();
            if (sourceData.Length != size)
                throw new ArgumentException("Incorrect data size. Expected " + size + " bytes");
            if (_bitmapData == null || _bitmapData.Length != size)
                _bitmapData = new byte[size];
            Buffer.BlockCopy(sourceData, 0, _bitmapData, 0, size);
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

            BasisU.EncodeBytes(
                sourceBitmap: sourceBitmap,
                destinationFormat: format,
                out var compressedBytes);

            // Need to pad out the data that may come back from basisU & KTX.
            //  Pvrtc has a minimum size as referenced here, https://github.com/BinomialLLC/basis_universal/issues/30
            //  however, when the texture is loaded back from the KTX data, it can lose some padding.
            //  To counter that, fill in zero's for the remaining data.
            var expectedSize = GetDataSize();
            if (expectedSize > compressedBytes.Length)
            {
                var nextBytes = new byte[expectedSize];
                Array.Copy(compressedBytes, nextBytes, compressedBytes.Length);
                compressedBytes = nextBytes;
            }


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

            // No other support for copying from a PVR texture yet
            return false;
        }
    }
}
