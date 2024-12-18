// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Content.Pipeline.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties and methods for creating and maintaining a bitmap resource.
    /// </summary>
    public class PixelBitmapContent<T> : BitmapContent where T : struct, IEquatable<T>
    {
        internal T[][] _pixelData;

        internal SurfaceFormat _format;

        /// <summary>
        /// Initializes a new instance of PixelBitmapContent with the specified width or height.
        /// </summary>
        /// <param name="width">Width, in pixels, of the bitmap resource.</param>
        /// <param name="height">Height, in pixels, of the bitmap resource.</param>
        public PixelBitmapContent(int width, int height)
        {
            if (!TryGetFormat(out _format))
                throw new InvalidOperationException(string.Format("Color format \"{0}\" is not supported",typeof(T).ToString()));
            Height = height;
            Width = width;

            _pixelData = new T[height][];

            for (int y = 0; y < height; y++)
                _pixelData[y] = new T[width];

        }

        /// <inheritdoc cref="BitmapContent.GetPixelData"/>
        public override byte[] GetPixelData()
        {
            var formatSize = _format.GetSize();
            var dataSize = Width * Height * formatSize;
            var outputData = new byte[dataSize];

            for (var x = 0; x < Height; x++)
            {
                var dataHandle = GCHandle.Alloc(_pixelData[x], GCHandleType.Pinned);
                var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64());

                Marshal.Copy(dataPtr, outputData, (formatSize * x * Width), (Width * formatSize));

                dataHandle.Free();
            }

            return outputData;
        }

        /// <inheritdoc cref="BitmapContent.SetPixelData"/>
        public override void SetPixelData(byte[] sourceData)
        {
            var size = _format.GetSize();

            for (var x = 0; x < Height; x++)
            {
                var dataHandle = GCHandle.Alloc(_pixelData[x], GCHandleType.Pinned);
                var dataPtr = (IntPtr)dataHandle.AddrOfPinnedObject().ToInt64();

                Marshal.Copy(sourceData, (x * Width * size), dataPtr, Width * size);

                dataHandle.Free();
            }
        }

        /// <summary>
        /// Returns pixel data for the given row.
        /// </summary>
        /// <param name="y">The row index.</param>
        /// <returns>Array containing the pixel data for the given row.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Row is outside the bounds of the bitmap.</exception>
        public T[] GetRow(int y)
        {
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            return _pixelData[y];
        }

        /// <summary>
        /// Gets the corresponding GPU texture format for the specified bitmap type.
        /// </summary>
        /// <param name="format">Format being retrieved.</param>
        /// <returns>The GPU texture format of the bitmap type.</returns>
        public override bool TryGetFormat(out SurfaceFormat format)
        {
            if (typeof(T) == typeof(Color))
                format = SurfaceFormat.Color;
            else if (typeof(T) == typeof(Bgra4444))
                format = SurfaceFormat.Bgra4444;
            else if (typeof(T) == typeof(Bgra5551))
                format = SurfaceFormat.Bgra5551;
            else if (typeof(T) == typeof(Bgr565))
                format = SurfaceFormat.Bgr565;
            else if (typeof(T) == typeof(Vector4))
                format = SurfaceFormat.Vector4;
            else if (typeof(T) == typeof(Vector2))
                format = SurfaceFormat.Vector2;
            else if (typeof(T) == typeof(Single))
                format = SurfaceFormat.Single;
            else if (typeof(T) == typeof(byte))
                format = SurfaceFormat.Alpha8;
            else if (typeof(T) == typeof(Rgba64))
                format = SurfaceFormat.Rgba64;
            else if (typeof(T) == typeof(Rgba1010102))
                format = SurfaceFormat.Rgba1010102;
            else if (typeof(T) == typeof(Rg32))
                format = SurfaceFormat.Rg32;
            else if (typeof(T) == typeof(Byte4))
                format = SurfaceFormat.Color;
            else if (typeof(T) == typeof(NormalizedByte2))
                format = SurfaceFormat.NormalizedByte2;
            else if (typeof(T) == typeof(NormalizedByte4))
                format = SurfaceFormat.NormalizedByte4;
            else if (typeof(T) == typeof(HalfSingle))
                format = SurfaceFormat.HalfSingle;
            else if (typeof(T) == typeof(HalfVector2))
                format = SurfaceFormat.HalfVector2;
            else if (typeof(T) == typeof(HalfVector4))
                format = SurfaceFormat.HalfVector4;
            else
            {
                format = SurfaceFormat.Color;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the pixel at the specified location.
        /// </summary>
        /// <param name="x">Pixel x coordinate.</param>
        /// <param name="y">Pixel y coordinate.</param>
        /// <returns>The pixel for the specified location.</returns>
        public T GetPixel(int x, int y)
        {
            return _pixelData[y][x];
        }

        /// <summary>
        /// Sets the pixel at the specified location.
        /// </summary>
        /// <param name="x">Pixel x coordinate.</param>
        /// <param name="y">Pixel y coordinate.</param>
        /// <param name="value">The new pixel value.</param>
        public void SetPixel(int x, int y, T value)
        {
            _pixelData[y][x] = value;
        }

        /// <summary>
        /// Replaces all pixels of the specified color with the new color.
        /// </summary>
        /// <param name="originalColor">The color to replace.</param>
        /// <param name="newColor">The color to replace it with.</param>
        public void ReplaceColor(T originalColor, T newColor)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (_pixelData[y][x].Equals(originalColor))
                        _pixelData[y][x] = newColor;
                }
            }
        }

        /// <inheritdoc cref="BitmapContent.TryCopyFrom"/>
        protected override bool TryCopyFrom(BitmapContent sourceBitmap, Rectangle sourceRegion, Rectangle destinationRegion)
        {
            SurfaceFormat sourceFormat;
            if (!sourceBitmap.TryGetFormat(out sourceFormat))
                return false;

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (_format == sourceFormat && (sourceRegion == new Rectangle(0, 0, Width, Height)) && sourceRegion == destinationRegion)
            {
                SetPixelData(sourceBitmap.GetPixelData());
                return true;
            }

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

            // Convert from a Vector4 format
            var src = sourceBitmap as PixelBitmapContent<Vector4>;
            if (default(T) is IPackedVector)
            {
                Parallel.For(0, sourceRegion.Height, (y) =>
                {
                    var pixel = default(T);
                    var p = (IPackedVector)pixel;
                    var row = src.GetRow(sourceRegion.Top + y);
                    for (int x = 0; x < sourceRegion.Width; ++x)
                    {
                        p.PackFromVector4(row[sourceRegion.Left + x]);
                        pixel = (T)p;
                        SetPixel(destinationRegion.Left + x, destinationRegion.Top + y, pixel);
                    }
                });
            }
            else
            {
                var converter = new Vector4Converter() as IVector4Converter<T>;
                // If no converter could be created, converting from this format is not supported
                if (converter == null)
                    return false;

                Parallel.For(0, sourceRegion.Height, (y) =>
                {
                    var row = src.GetRow(sourceRegion.Top + y);
                    for (int x = 0; x < sourceRegion.Width; ++x)
                    {
                        SetPixel(destinationRegion.Left + x, destinationRegion.Top + y, converter.FromVector4(row[sourceRegion.Left + x]));
                    }
                });
            }

            return true;
        }

        /// <inheritdoc cref="BitmapContent.TryCopyTo"/>
        protected override bool TryCopyTo(BitmapContent destinationBitmap, Rectangle sourceRegion, Rectangle destinationRegion)
        {
            SurfaceFormat destinationFormat;
            if (!destinationBitmap.TryGetFormat(out destinationFormat))
                return false;

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (_format == destinationFormat && (sourceRegion == new Rectangle(0, 0, Width, Height)) && sourceRegion == destinationRegion)
            {
                destinationBitmap.SetPixelData(GetPixelData());
                return true;
            }

            // If the destination is not Vector4 or requires resizing, send it through BitmapContent.Copy
            if (!(destinationBitmap is PixelBitmapContent<Vector4>) || sourceRegion.Width != destinationRegion.Width || sourceRegion.Height != destinationRegion.Height)
            {
                try
                {
                    BitmapContent.Copy(this, sourceRegion, destinationBitmap, destinationRegion);
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }

            // Convert to a Vector4 format
            var dest = destinationBitmap as PixelBitmapContent<Vector4>;
            if (default(T) is IPackedVector)
            {
                Parallel.For(0, sourceRegion.Height, (y) =>
                {
                    var row = GetRow(sourceRegion.Top + y);
                    for (int x = 0; x < sourceRegion.Width; ++x)
                        dest.SetPixel(destinationRegion.Left + x, destinationRegion.Top + y, ((IPackedVector)row[sourceRegion.Left + x]).ToVector4());
                });
            }
            else
            {
                var converter = new Vector4Converter() as IVector4Converter<T>;
                // If no converter could be created, converting from this format is not supported
                if (converter == null)
                    return false;

                Parallel.For(0, sourceRegion.Height, (y) =>
                {
                    var row = GetRow(sourceRegion.Top + y);
                    for (int x = 0; x < sourceRegion.Width; ++x)
                        dest.SetPixel(destinationRegion.Left + x, destinationRegion.Top + y, converter.ToVector4(row[sourceRegion.Left + x]));
                });
            }

            return true;
        }
    }
}
