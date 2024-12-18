// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Class to provide methods to handle processing of font textures.
    /// </summary>
    [ContentProcessorAttribute(DisplayName = "Font Texture - MonoGame")]
    public class FontTextureProcessor : ContentProcessor<Texture2DContent, SpriteFontContent>
    {
        private Color transparentPixel = Color.Magenta;

        /// <summary>
        /// Gets or sets the first character of the font.
        /// </summary>
        [DefaultValue(' ')]
        public virtual char FirstCharacter { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if the alpha channel should be premultiplied.
        /// </summary>
        [DefaultValue(true)]
        public virtual bool PremultiplyAlpha { get; set; }

        /// <inheritdoc cref="TextureProcessorOutputFormat"/>
        public virtual TextureProcessorOutputFormat TextureFormat { get; set; }

        /// <summary>
        /// Creates a new FontTextureProcessor.
        /// </summary>
        public FontTextureProcessor()
        {
            FirstCharacter = ' ';
            PremultiplyAlpha = true;
        }

        /// <summary>
        /// Gets the character for the specified index relative to the first character.
        /// </summary>
        /// <param name="index">Character index.</param>
        /// <returns>Character at index.</returns>
        protected virtual char GetCharacterForIndex(int index)
        {
            return (char)(((int)FirstCharacter) + index);
        }

        private List<GlyphData> ExtractGlyphs(PixelBitmapContent<Color> bitmap)
        {
            var glyphs = new List<GlyphData>();
            var regions = new List<Rectangle>();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y) != transparentPixel)
                    {
                        // if we don't have a region that has this pixel already
                        var re = regions.Find(r =>
                        {
                            return r.Contains(x, y);
                        });
                        if (re == Rectangle.Empty)
                        {
                            // we have found the top, left of a image.
                            // we now need to scan for the 'bounds'
                            int top = y;
                            int bottom = y;
                            int left = x;
                            int right = x;
                            while (bitmap.GetPixel(right, bottom) != transparentPixel)
                                right++;
                            while (bitmap.GetPixel(left, bottom) != transparentPixel)
                                bottom++;
                            // we got a glyph :)
                            regions.Add(new Rectangle(left, top, right - left, bottom - top));
                            x = right;
                        }
                        else
                        {
                            x += re.Width;
                        }
                    }
                }
            }

            for (int i = 0; i < regions.Count; i++)
            {
                var rect = regions[i];
                var newBitmap = new PixelBitmapContent<Color>(rect.Width, rect.Height);
                BitmapContent.Copy(bitmap, rect, newBitmap, new Rectangle(0, 0, rect.Width, rect.Height));
                var glyphData = new GlyphData((uint)i, newBitmap);
                glyphData.CharacterWidths.B = glyphData.Bitmap.Width;
                glyphs.Add(glyphData);
                //newbitmap.Save (GetCharacterForIndex(i)+".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            return glyphs;
        }

        /// <inheritdoc/>
        public override SpriteFontContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            var output = new SpriteFontContent();

            // extract the glyphs from the texture and map them to a list of characters.
            // we need to call GtCharacterForIndex for each glyph in the Texture to
            // get the char for that glyph, by default we start at ' ' then '!' and then ASCII
            // after that.
            BitmapContent face = input.Faces[0][0];
            SurfaceFormat faceFormat;
            face.TryGetFormat(out faceFormat);
            if (faceFormat != SurfaceFormat.Color)
            {
                var colorFace = new PixelBitmapContent<Color>(face.Width, face.Height);
                BitmapContent.Copy(face, colorFace);
                face = colorFace;
            }

            var glyphs = ExtractGlyphs((PixelBitmapContent<Color>)face);
            // Optimize.
            foreach (var glyph in glyphs)
            {
                GlyphCropper.Crop(glyph);
                output.VerticalLineSpacing = Math.Max(output.VerticalLineSpacing, glyph.Subrect.Height);
            }

            // Get the platform specific texture profile.
            var texProfile = TextureProfile.ForPlatform(context.TargetPlatform);

            // We need to know how to pack the glyphs.
            bool requiresPot, requiresSquare;
            texProfile.Requirements(context, TextureFormat, out requiresPot, out requiresSquare);

            face = GlyphPacker.ArrangeGlyphs(glyphs.ToArray(), requiresPot, requiresSquare);

            foreach (var glyph in glyphs)
            {
                output.CharacterMap.Add(GetCharacterForIndex((int)glyph.GlyphIndex));
                output.Glyphs.Add(new Rectangle(glyph.Subrect.X, glyph.Subrect.Y, glyph.Subrect.Width, glyph.Subrect.Height));
                output.Cropping.Add(new Rectangle((int)glyph.XOffset, (int)glyph.YOffset, glyph.Width, glyph.Height));
                var abc = glyph.CharacterWidths;
                output.Kerning.Add(new Vector3(abc.A, abc.B, abc.C));
            }

            output.Texture.Faces[0].Add(face);

            var bmp = output.Texture.Faces[0][0];
            if (PremultiplyAlpha)
            {
                var data = bmp.GetPixelData();
                var idx = 0;
                for (; idx < data.Length;)
                {
                    var r = data[idx + 0];
                    var g = data[idx + 1];
                    var b = data[idx + 2];
                    var a = data[idx + 3];
                    var col = Color.FromNonPremultiplied(r, g, b, a);

                    data[idx + 0] = col.R;
                    data[idx + 1] = col.G;
                    data[idx + 2] = col.B;
                    data[idx + 3] = col.A;

                    idx += 4;
                }

                bmp.SetPixelData(data);
            }

            // Perform the final texture conversion.
            texProfile.ConvertTexture(context, output.Texture, TextureFormat, true);

            return output;
        }
    }
}
