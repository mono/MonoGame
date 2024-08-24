// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpFont;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    // Uses FreeType to rasterize TrueType fonts into a series of glyph bitmaps.
    internal class SharpFontImporter : IFontImporter
    {
        // Properties hold the imported font data.
        public IEnumerable<Glyph> Glyphs
        {
            get; private set;
        }

        public float LineSpacing
        {
            get; private set;
        }

        /// <summary>
        /// Max height from the top of the line to the baseline of tbe text, also known as the ascender of the font.
        /// </summary>
        public int YOffsetMin
        {
            get; private set;
        }

        /// <summary>
        /// Indicates if the italic style had to be simulated.
        /// </summary>
        public bool Italiced
        {
            get; private set;
        }

        /// <summary>
        /// Indicates if the bold style had to be simulated.
        /// </summary>
        public bool Emboldened
        {
            get; private set;
        }
        // Size of the temp surface used for GDI+ rasterization.
        const int MaxGlyphSize = 1024;

        Library lib = null;

        public void Import(FontDescription options, string fontName)
        {
            lib = new Library();
            // Create a bunch of GDI+ objects.
            var face = CreateFontFace(options, fontName);
            Italiced = options.Style.HasFlag(FontDescriptionStyle.Italic) && !face.StyleFlags.HasFlag(StyleFlags.Italic);
            Emboldened = options.Style.HasFlag(FontDescriptionStyle.Bold) && !face.StyleFlags.HasFlag(StyleFlags.Bold);
            bool draw3Times = options.Size > 15;
            try
            {
                // Which characters do we want to include?
                var characters = options.Characters;

                var glyphList = new List<Glyph>();
                var glyphMaps = new Dictionary<uint, GlyphData>();

                // Rasterize each character in turn.
                foreach (char character in characters)
                {
                    uint glyphIndex = face.GetCharIndex(character);
                    if (!glyphMaps.TryGetValue(glyphIndex, out GlyphData glyphData))
                    {
                        glyphData = ImportGlyph(character, face, Italiced, Emboldened, draw3Times);
                        glyphMaps.Add(glyphIndex, glyphData);
                    }

                    var glyph = new Glyph(character, glyphData);
                    glyphList.Add(glyph);
                }
                Glyphs = glyphList;

                // Store the font height.
                LineSpacing = face.Size.Metrics.Height >> 6;

                // The height used to calculate the Y offset for each character.
                YOffsetMin = -face.Size.Metrics.Ascender >> 6;
            }
            finally
            {
                if (face != null)
                    face.Dispose();
                if (lib != null)
                {
                    lib.Dispose();
                    lib = null;
                }
            }
        }


        // Attempts to instantiate the requested GDI+ font object.
        private Face CreateFontFace(FontDescription options, string fontName)
        {
            const uint dpi = 96;
            var face = lib.NewFace(fontName, 0);
            var fixedSize = ((int) options.Size) << 6;
            face.SetCharSize(0, fixedSize, dpi, dpi);

            if (face.FamilyName == "Microsoft Sans Serif" && options.FontName != "Microsoft Sans Serif")
                throw new PipelineException(string.Format("Font {0} is not installed on this computer.", options.FontName));

            return face;

        }

        // Rasterizes a single character glyph.
        private GlyphData ImportGlyph(uint glyphIndex, Face face, bool italice, bool embolden, bool draw3Times)
        {
            face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
            face.Glyph.RenderGlyph(RenderMode.Normal);

            // Render the character.
            BitmapContent glyphBitmap = null;
            var abc = new ABCFloat();
            int finalWidth = face.Glyph.Bitmap.Width;
            int finalHeight = face.Glyph.Bitmap.Rows;
            if (face.Glyph.Bitmap.Width > 0 && face.Glyph.Bitmap.Rows > 0)
            {
                byte[] gpixelAlphas = new byte[face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows];
                //if the character bitmap has 1bpp we have to expand the buffer data to get the 8bpp pixel data
                //each byte in bitmap.bufferdata contains the value of to 8 pixels in the row
                //if bitmap is of width 10, each row has 2 bytes with 10 valid bits, and the last 6 bits of 2nd byte must be discarded
                if (face.Glyph.Bitmap.PixelMode == PixelMode.Mono)
                {
                    //variables needed for the expansion, amount of written data, length of the data to write
                    int written = 0, length = face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows;
                    for (int i = 0; written < length; i++)
                    {
                        //width in pixels of each row
                        int width = face.Glyph.Bitmap.Width;
                        while (width > 0)
                        {
                            //valid data in the current byte
                            int stride = MathHelper.Min(8, width);
                            //copy the valid bytes to pixeldata
                            ExpandByteAndCopy(face.Glyph.Bitmap.BufferData[i], stride, gpixelAlphas, written);
                            written += stride;
                            width -= stride;
                            if (width > 0)
                                i++;
                        }
                    }
                }
                else
                    Marshal.Copy(face.Glyph.Bitmap.Buffer, gpixelAlphas, 0, gpixelAlphas.Length);
                if (embolden)
                    Embolden(ref gpixelAlphas, ref abc, finalWidth, finalHeight, ref finalWidth, ref finalHeight, draw3Times);
                if (italice)
                    Italice(ref gpixelAlphas, ref abc, finalWidth, finalHeight, ref finalWidth);
                glyphBitmap = new PixelBitmapContent<byte>(finalWidth, finalHeight);
                glyphBitmap.SetPixelData(gpixelAlphas);
            }

            if (glyphBitmap == null)
            {
                var gHA = face.Glyph.Metrics.HorizontalAdvance >> 6;
                var gVA = face.Size.Metrics.Height >> 6;

                gHA = gHA > 0 ? gHA : gVA;
                gVA = gVA > 0 ? gVA : gHA;

                glyphBitmap = new PixelBitmapContent<byte>(gHA, gVA);
            }

            abc.A += face.Glyph.Metrics.HorizontalBearingX >> 6;
            abc.B += face.Glyph.Metrics.Width >> 6;
            abc.C += (face.Glyph.Metrics.HorizontalAdvance >> 6) - (abc.A + abc.B) + finalWidth - face.Glyph.Bitmap.Width;

            // Construct the output Glyph object.
            return new GlyphData(glyphIndex, glyphBitmap)
            {
                XOffset = -(face.Glyph.Advance.X >> 6),
                XAdvance = abc.A + abc.B + abc.C,//face.Glyph.Metrics.HorizontalAdvance >> 6,
                YOffset = -(face.Glyph.Metrics.HorizontalBearingY >> 6),
                CharacterWidths = abc
            };
        }


        /// <summary>
        /// Reads each individual bit of a byte from left to right and expands it to a full byte,
        /// ones get byte.maxvalue, and zeros get byte.minvalue.
        /// </summary>
        /// <param name="origin">Byte to expand and copy</param>
        /// <param name="length">Number of Bits of the Byte to copy, from 1 to 8</param>
        /// <param name="destination">Byte array where to copy the results</param>
        /// <param name="startIndex">Position where to begin copying the results in destination</param>
        private static void ExpandByteAndCopy(byte origin, int length, byte[] destination, int startIndex)
        {
            byte tmp;
            for (int i = 7; i > 7 - length; i--)
            {
                tmp = (byte) (1 << i);
                if (origin / tmp == 1)
                {
                    destination[startIndex + 7 - i] = byte.MaxValue;
                    origin -= tmp;
                }
                else
                    destination[startIndex + 7 - i] = byte.MinValue;
            }
        }

        private void Embolden(ref byte[] bitmap, ref ABCFloat charSize, int initialWidth, int height, ref int finalWidth, ref int finalHeight, bool draw3times)
        {
            finalWidth = initialWidth + (draw3times ? 2 : 1);
            charSize.A -= draw3times ? 2 : 1;
            charSize.C -= draw3times ? 2 : 1;
            int kLimit = draw3times ? 1 : 0;
            finalHeight += draw3times ? 1 : 0;
            byte[] destination = new byte[finalWidth * finalHeight];
            byte orig, dest;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < initialWidth; j++)
                {
                    orig = bitmap[initialWidth * i + j];
                    for (int k = -1; k <= kLimit; k++)
                    {
                        dest = destination[i * finalWidth + j + 1 + k];
                        destination[i * finalWidth + j + 1 + k] = orig + dest > byte.MaxValue ? byte.MaxValue : (byte) (orig + dest);
                        if (draw3times)
                        {
                            dest = destination[i * finalWidth + j + 1 + k + finalWidth];
                            destination[i * finalWidth + j + 1 + k + finalWidth] = orig + dest > byte.MaxValue ? byte.MaxValue : (byte) (orig + dest);
                        }
                    }
                }
            }
            bitmap = destination;
        }

        private void Italice(ref byte[] bitmap, ref ABCFloat charSize, int initialWidth, int height, ref int finalWidth)
        {
            double displacement = Math.Tan(0.349066);//20 degrees converted to radians
            finalWidth += (int) Math.Ceiling(displacement * height);
            int extraWidth = finalWidth - initialWidth;
            charSize.A -= extraWidth;
            charSize.C -= extraWidth;
            byte[] destination = new byte[finalWidth * height];
            double currentDisplacement = 0;
            for (int row = height - 1; row >= 0; row--, currentDisplacement += displacement)
            {
                double leftVal = Math.Ceiling(currentDisplacement) - currentDisplacement;
                double rightVal = currentDisplacement - Math.Floor(currentDisplacement);
                if (leftVal == 0 && rightVal == 0)
                    leftVal = 1;
                for (int j = 0; j < initialWidth; j++)
                {
                    int OrigPos = row * initialWidth + j;
                    int DestPos = row * finalWidth + j + (int) Math.Floor(currentDisplacement);
                    if (destination[DestPos] + bitmap[OrigPos] * leftVal > byte.MaxValue)
                        destination[DestPos] = byte.MaxValue;
                    else
                        destination[DestPos] += (byte) (bitmap[OrigPos] * leftVal);
                    if (destination[DestPos + 1] + bitmap[OrigPos] * rightVal > byte.MaxValue)
                        destination[DestPos + 1] = byte.MaxValue;
                    else
                        destination[DestPos + 1] += (byte) (bitmap[OrigPos] * rightVal);
                }
            }
            bitmap = destination;
        }
    }
}
