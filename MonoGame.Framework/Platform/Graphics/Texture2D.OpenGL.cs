// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Framework.Utilities;

#if IOS
using UIKit;
using CoreGraphics;
using Foundation;
using System.Drawing;
#endif

#if OPENGL
using MonoGame.OpenGL;
using GLPixelFormat = MonoGame.OpenGL.PixelFormat;
using PixelFormat = MonoGame.OpenGL.PixelFormat;

#if ANDROID
using Android.Graphics;
#endif
#endif // OPENGL

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        private void PlatformConstruct(int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared)
        {
            this.glTarget = TextureTarget.Texture2D;
            format.GetGLFormat(GraphicsDevice, out glInternalFormat, out glFormat, out glType);

            Threading.BlockOnUIThread(() =>
            {
                GenerateGLTextureIfRequired();
                int w = width;
                int h = height;
                int level = 0;
                while (true)
                {
                    if (glFormat == GLPixelFormat.CompressedTextureFormats)
                    {
                        int imageSize = 0;
                        // PVRTC has explicit calculations for imageSize
                        // https://www.khronos.org/registry/OpenGL/extensions/IMG/IMG_texture_compression_pvrtc.txt
                        if (format == SurfaceFormat.RgbPvrtc2Bpp || format == SurfaceFormat.RgbaPvrtc2Bpp)
                        {
                            imageSize = (Math.Max(w, 16) * Math.Max(h, 8) * 2 + 7) / 8;
                        }
                        else if (format == SurfaceFormat.RgbPvrtc4Bpp || format == SurfaceFormat.RgbaPvrtc4Bpp)
                        {
                            imageSize = (Math.Max(w, 8) * Math.Max(h, 8) * 4 + 7) / 8;
                        }
                        else
                        {
                            int blockSize = format.GetSize();
                            int blockWidth, blockHeight;
                            format.GetBlockSize(out blockWidth, out blockHeight);
                            int wBlocks = (w + (blockWidth - 1)) / blockWidth;
                            int hBlocks = (h + (blockHeight - 1)) / blockHeight;
                            imageSize = wBlocks * hBlocks * blockSize;
                        }
                        GL.CompressedTexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, imageSize, IntPtr.Zero);
                        GraphicsExtensions.CheckGLError();
                    }
                    else
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, glFormat, glType, IntPtr.Zero);
                        GraphicsExtensions.CheckGLError();
                    }

                    if ((w == 1 && h == 1) || !mipmap)
                        break;
                    if (w > 1)
                        w = w / 2;
                    if (h > 1)
                        h = h / 2;
                    ++level;
                }
            });
        }

        private void PlatformSetDataBody<T>(int level, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            int w, h;
            GetSizeForLevel(Width, Height, level, out w, out h);

            var elementSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            // Use try..finally to make sure dataHandle is freed in case of an error
            try
            {
                var startBytes = startIndex * elementSizeInByte;
                var dataPtr = new IntPtr(dataHandle.AddrOfPinnedObject().ToInt64() + startBytes);
                // Store the current bound texture.
                var prevTexture = GraphicsExtensions.GetBoundTexture2D();

                if (prevTexture != glTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, glTexture);
                    GraphicsExtensions.CheckGLError();
                }

                GenerateGLTextureIfRequired();
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(_format.GetSize(), 8));

                if (glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    GL.CompressedTexImage2D(
                        TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, elementCount * elementSizeInByte, dataPtr);
                }
                else
                {
                    GL.TexImage2D(
                        TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, glFormat, glType, dataPtr);
                }
                GraphicsExtensions.CheckGLError();

#if !ANDROID
                // Required to make sure that any texture uploads on a thread are completed
                // before the main thread tries to use the texture.
                GL.Finish();
                GraphicsExtensions.CheckGLError();
#endif
                // Restore the bound texture.
                if (prevTexture != glTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                    GraphicsExtensions.CheckGLError();
                }
            }
            finally
            {
                dataHandle.Free();
            }
        }

        private void PlatformSetDataBody<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            var elementSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            // Use try..finally to make sure dataHandle is freed in case of an error
            try
            {
                var startBytes = startIndex * elementSizeInByte;
                var dataPtr = new IntPtr(dataHandle.AddrOfPinnedObject().ToInt64() + startBytes);

                // Store the current bound texture.
                var prevTexture = GraphicsExtensions.GetBoundTexture2D();
                if (prevTexture != glTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, glTexture);
                    GraphicsExtensions.CheckGLError();
                }

                GenerateGLTextureIfRequired();
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(_format.GetSize(), 8));

                if (glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    GL.CompressedTexSubImage2D(
                        TextureTarget.Texture2D, level, rect.X, rect.Y, rect.Width, rect.Height,
                        glInternalFormat, elementCount * elementSizeInByte, dataPtr);
                }
                else
                {
                    GL.TexSubImage2D(
                        TextureTarget.Texture2D, level, rect.X, rect.Y, rect.Width, rect.Height,
                        glFormat, glType, dataPtr);
                }
                GraphicsExtensions.CheckGLError();

#if !ANDROID
                // Required to make sure that any texture uploads on a thread are completed
                // before the main thread tries to use the texture.
                GL.Finish();
                GraphicsExtensions.CheckGLError();
#endif
                // Restore the bound texture.
                if (prevTexture != glTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                    GraphicsExtensions.CheckGLError();
                }
            }
            finally
            {
                dataHandle.Free();
            }
        }

        private void PlatformSetData<T>(int level, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            Threading.BlockOnUIThread(SetDataState<T>.Action, new SetDataState<T>
            {
                texture = this,
                level = level,
                data = data,
                startIndex = startIndex,
                elementCount = elementCount
            });
        }

        private void PlatformSetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            Threading.BlockOnUIThread(SetDataRectState<T>.Action, new SetDataRectState<T>
            {
                texture = this,
                level = level,
                arraySlice = arraySlice,
                rect = rect,
                data = data,
                startIndex = startIndex,
                elementCount = elementCount
            });
        }

        private void PlatformGetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            Threading.BlockOnUIThread(() =>
            {
#if GLES
                // TODO: check for non renderable formats (formats that can't be attached to FBO)

                var framebufferId = 0;
                GL.GenFramebuffers(1, out framebufferId);
                GraphicsExtensions.CheckGLError();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
                GraphicsExtensions.CheckGLError();
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, this.glTexture, 0);
                GraphicsExtensions.CheckGLError();

                GL.ReadPixels(rect.X, rect.Y, rect.Width, rect.Height, this.glFormat, this.glType, data);
                GraphicsExtensions.CheckGLError();
                GL.DeleteFramebuffers(1, ref framebufferId);
#else
                var tSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
                var pixelToT = Format.GetSize() / tSizeInByte;

                GL.BindTexture(TextureTarget.Texture2D, this.glTexture);
                GL.PixelStore(PixelStoreParameter.PackAlignment, Math.Min(tSizeInByte, 8));


                if (glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    // Note: for compressed format Format.GetSize() returns the size of a 4x4 block
                    var tFullWidth = Math.Max(this.width >> level, 1) / 4 * pixelToT;
                    var temp = GetDataPool<T>.Pool.Rent(Math.Max(this.height >> level, 1) / 4 * tFullWidth);
                    GL.GetCompressedTexImage(TextureTarget.Texture2D, level, temp);
                    GraphicsExtensions.CheckGLError();

                    var rowCount = rect.Height / 4;
                    var tRectWidth = rect.Width / 4 * Format.GetSize() / tSizeInByte;
                    for (var r = 0; r < rowCount; r++)
                    {
                        var tempStart = rect.X / 4 * pixelToT + (rect.Top / 4 + r) * tFullWidth;
                        var dataStart = startIndex + r * tRectWidth;
                        Array.Copy(temp, tempStart, data, dataStart, tRectWidth);
                    }
                    GetDataPool<T>.Pool.Return(temp);
                }
                //If we want the full texture rect == (0,0,width,height), we dont have to allocate a temp buffer to copy from.
                else if (rect.X == 0 && rect.Y == 0 && this.width == rect.Width && this.height == rect.Height && pixelToT == 1)
                {
                    GL.GetTexImage(TextureTarget.Texture2D, level, glFormat, glType, data);
                    GraphicsExtensions.CheckGLError();
                }
                else
                {
                    // we need to convert from our format size to the size of T here
                    var tFullWidth = Math.Max(this.width >> level, 1) * Format.GetSize() / tSizeInByte;
                    var temp = GetDataPool<T>.Pool.Rent(Math.Max(this.height >> level, 1) * tFullWidth);
                    GL.GetTexImage(TextureTarget.Texture2D, level, glFormat, glType, temp);
                    GraphicsExtensions.CheckGLError();

                    var rowCount = rect.Height;
                    var tRectWidth = rect.Width * pixelToT;
                    for (var r = 0; r < rowCount; r++)
                    {
                        var tempStart = rect.X * pixelToT + (r + rect.Top) * tFullWidth;
                        var dataStart = startIndex + r * tRectWidth;
                        Array.Copy(temp, tempStart, data, dataStart, tRectWidth);
                    }
                    GetDataPool<T>.Pool.Return(temp);
                }
#endif
            });
        }

#if IOS
        public static Texture2D FromStream(GraphicsDevice graphicsDevice, UIImage uiImage)
        {
            return PlatformFromStream(graphicsDevice, uiImage.CGImage);
        }
#elif ANDROID
        public static Texture2D FromStream(GraphicsDevice graphicsDevice, Bitmap bitmap)
        {
            return PlatformFromStream(graphicsDevice, bitmap);
        }

        public void Reload(Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;

            int[] pixels = new int[width * height];
            if ((width != image.Width) || (height != image.Height))
            {
                using (Bitmap imagePadded = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888))
                {
                    Canvas canvas = new Canvas(imagePadded);
                    canvas.DrawARGB(0, 0, 0, 0);
                    canvas.DrawBitmap(image, 0, 0, null);
                    imagePadded.GetPixels(pixels, 0, width, 0, 0, width, height);
                    imagePadded.Recycle();
                }
            }
            else
            {
                image.GetPixels(pixels, 0, width, 0, 0, width, height);
            }

            image.Recycle();

            this.SetData<int>(pixels);
        }
#endif

#if IOS
        private static Texture2D PlatformFromStream(GraphicsDevice graphicsDevice, CGImage cgImage)
        {
            var width = cgImage.Width;
            var height = cgImage.Height;

            var data = new byte[width * height * 4];

            var colorSpace = CGColorSpace.CreateDeviceRGB();
            var bitmapContext = new CGBitmapContext(data, width, height, 8, width * 4, colorSpace, CGBitmapFlags.PremultipliedLast);
            bitmapContext.DrawImage(new RectangleF(0, 0, width, height), cgImage);
            bitmapContext.Dispose();
            colorSpace.Dispose();

            Texture2D texture = null;
            Threading.BlockOnUIThread(() =>
            {
                texture = new Texture2D(graphicsDevice, (int)width, (int)height, false, SurfaceFormat.Color);
                texture.SetData(data);
            });

            return texture;
        }
#elif ANDROID
        private static Texture2D PlatformFromStream(GraphicsDevice graphicsDevice, Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;

            int[] pixels = new int[width * height];
            if ((width != image.Width) || (height != image.Height))
            {
                using (Bitmap imagePadded = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888))
                {
                    Canvas canvas = new Canvas(imagePadded);
                    canvas.DrawARGB(0, 0, 0, 0);
                    canvas.DrawBitmap(image, 0, 0, null);
                    imagePadded.GetPixels(pixels, 0, width, 0, 0, width, height);
                    imagePadded.Recycle();
                }
            }
            else
            {
                image.GetPixels(pixels, 0, width, 0, 0, width, height);
            }
            image.Recycle();

            // Convert from ARGB to ABGR
            ConvertToABGR(height, width, pixels);

            Texture2D texture = null;
            Threading.BlockOnUIThread(() =>
            {
                texture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);
                texture.SetData<int>(pixels);
            });

            return texture;
        }
#endif

        private void FillTextureFromStream(Stream stream)
        {
#if ANDROID
            using (Bitmap image = BitmapFactory.DecodeStream(stream, null, new BitmapFactory.Options
            {
                InScaled = false,
#pragma warning disable CA1422
                InDither = false,
#pragma warning restore CA1422
                InJustDecodeBounds = false,
#pragma warning disable CS0618
                InPurgeable = true,
                InInputShareable = true,
#pragma warning restore CS0618
            }))
            {
                var width = image.Width;
                var height = image.Height;

                int[] pixels = new int[width * height];
                image.GetPixels(pixels, 0, width, 0, 0, width, height);

                // Convert from ARGB to ABGR
                ConvertToABGR(height, width, pixels);

                this.SetData<int>(pixels);
                image.Recycle();
            }
#endif
        }

        // This method allows games that use Texture2D.FromStream
        // to reload their textures after the GL context is lost.
        private void PlatformReload(Stream textureStream)
        {
            var prev = GraphicsExtensions.GetBoundTexture2D();

            GenerateGLTextureIfRequired();
            FillTextureFromStream(textureStream);

            GL.BindTexture(TextureTarget.Texture2D, prev);
        }

        private void GenerateGLTextureIfRequired()
        {
            if (this.glTexture < 0)
            {
                GL.GenTextures(1, out this.glTexture);
                GraphicsExtensions.CheckGLError();

                // For best compatibility and to keep the default wrap mode of XNA, only set ClampToEdge if either
                // dimension is not a power of two.
                var wrap = TextureWrapMode.Repeat;
                if (((width & (width - 1)) != 0) || ((height & (height - 1)) != 0))
                    wrap = TextureWrapMode.ClampToEdge;

                GL.BindTexture(TextureTarget.Texture2D, this.glTexture);
                GraphicsExtensions.CheckGLError();

                GL.TexParameter(
                    TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (_levelCount > 1) ? (int)TextureMinFilter.LinearMipmapLinear : (int)TextureMinFilter.Linear);
                GraphicsExtensions.CheckGLError();

                GL.TexParameter(
                    TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);
                GraphicsExtensions.CheckGLError();

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrap);
                GraphicsExtensions.CheckGLError();

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrap);
                GraphicsExtensions.CheckGLError();

                // Set mipmap levels
#if !GLES
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
#endif
                GraphicsExtensions.CheckGLError();
                if (GraphicsDevice.GraphicsCapabilities.SupportsTextureMaxLevel)
                {
                    if (_levelCount > 0)
                    {
                        GL.TexParameter(TextureTarget.Texture2D, SamplerState.TextureParameterNameTextureMaxLevel, _levelCount - 1);
                    }
                    else
                    {
                        GL.TexParameter(TextureTarget.Texture2D, SamplerState.TextureParameterNameTextureMaxLevel, 1000);
                    }
                    GraphicsExtensions.CheckGLError();
                }
            }
        }

        struct SetDataState<T>
            where T : struct
        {
            public Texture2D texture;
            public int level;
            public T[] data;
            public int startIndex;
            public int elementCount;

            internal static Action<SetDataState<T>> Action =
                (s) => s.texture.PlatformSetDataBody(s.level, s.data, s.startIndex, s.elementCount);
        }

        struct SetDataRectState<T>
            where T : struct
        {
            public Texture2D texture;
            public int level;
            public int arraySlice;
            public Rectangle rect;
            public T[] data;
            public int startIndex;
            public int elementCount;

            public static Action<SetDataRectState<T>> Action =
                (s) => s.texture.PlatformSetDataBody(s.level, s.arraySlice, s.rect, s.data, s.startIndex, s.elementCount);
        }

        private static class GetDataPool<T> where T : struct
        {
            // use int.MaxValue so arrays of any length can be saved in pool
            public static readonly ArrayPool<T> Pool = ArrayPool<T>.Create(int.MaxValue, 1);
        }
    }
}
