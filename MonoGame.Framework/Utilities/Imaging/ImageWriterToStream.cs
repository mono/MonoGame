﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.Utilities
{
    internal unsafe class ImageWriterToStream
    {
        private Stream _stream;
        private byte[] _buffer = new byte[1024];

        private int WriteCallback(void* context, void* data, int size)
        {
            if (data == null || size <= 0)
            {
                return 0;
            }

            if (_buffer.Length < size)
            {
                _buffer = new byte[size*2];
            }

            var bptr = (byte*) data;

            Marshal.Copy(new IntPtr(bptr), _buffer, 0, size);

            _stream.Write(_buffer, 0, size);

            return size;
        }

        private void WriteCallback2(void* context, void* data, int size)
        {
            WriteCallback(context, data, size);
        }

        public void Write(byte[] bytes, int x, int y, int comp, ImageWriterType type, Stream dest)
        {
            try
            {
                _stream = dest;
                fixed (byte* b = &bytes[0])
                {
                    switch (type)
                    {
                        case ImageWriterType.Bmp:
                            Imaging.stbi_write_bmp_to_func(WriteCallback, null, x, y, comp, b);
                            break;
                        case ImageWriterType.Tga:
                            Imaging.stbi_write_tga_to_func(WriteCallback, null, x, y, comp, b);
                            break;
                        case ImageWriterType.Jpg:
                            Imaging.tje_encode_with_func(WriteCallback2, null, 2, x, y, comp, b);
                            break;

                        case ImageWriterType.Png:
                            Imaging.stbi_write_png_to_func(WriteCallback, null, x, y, comp, b, x*comp);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("type", type, null);
                    }
                }
            }
            finally
            {
                _stream = null;
            }
        }
    }
}