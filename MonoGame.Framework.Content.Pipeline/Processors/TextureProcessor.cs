// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Class to provide methods and properties for processing textures.
    /// </summary>
    [ContentProcessor(DisplayName="Texture - MonoGame")]
    public class TextureProcessor : ContentProcessor<TextureContent, TextureContent>
    {
        /// <summary>
        /// Creates a new instance of the TextureProcessor class.
        /// </summary>
        public TextureProcessor()
        {
            ColorKeyColor = new Color(255, 0, 255, 255);
            ColorKeyEnabled = true;
            PremultiplyAlpha = true;
        }

        /// <summary>
        /// Gets or sets the color key color.
        /// <remarks>
        /// Typically used to make the background color of a texture transparent.
        /// </remarks>
        /// </summary>
        [DefaultValueAttribute(typeof(Color), "255,0,255,255")]
        public virtual Color ColorKeyColor { get; set; }

        /// <summary>
        /// Gets or sets the color key flag.
        /// <remarks>
        /// Must be set to <c>true</c> to use the color key.
        /// </remarks>
        /// </summary>
        [DefaultValueAttribute(true)]
        public virtual bool ColorKeyEnabled { get; set; }

        /// <summary>
        /// Gets or sets the generate mipmaps flag.
        /// </summary>
        public virtual bool GenerateMipmaps { get; set; }

        /// <summary>
        /// Gets or sets the premultiply alpha flag.
        /// </summary>
        [DefaultValueAttribute(true)]
        public virtual bool PremultiplyAlpha { get; set; }

        /// <summary>
        /// Gets or sets the resize to power of two flag.
        /// </summary>
        public virtual bool ResizeToPowerOfTwo { get; set; }

        /// <summary>
        /// Gets or sets the make square flag.
        /// </summary>
        public virtual bool MakeSquare { get; set; }

        /// <summary>
        /// Gets or sets the texture processor for the output format.
        /// </summary>
        public virtual TextureProcessorOutputFormat TextureFormat { get; set; }

        /// <inheritdoc/>
        public override TextureContent Process(TextureContent input, ContentProcessorContext context)
        {
            SurfaceFormat format;
            if (input.Faces[0][0].TryGetFormat(out format))
            {
                // If it is already a compressed format, we cannot do anything else so just return it
                if (format.IsCompressedFormat())
                    return input;
            }

            if (ColorKeyEnabled || ResizeToPowerOfTwo || MakeSquare || PremultiplyAlpha || GenerateMipmaps)
            {
                // Convert to floating point format for modifications. Keep the original format for conversion back later on if required.
                var originalType = input.Faces[0][0].GetType();
                try
                {
                    input.ConvertBitmapType(typeof(PixelBitmapContent<Vector4>));
                }
                catch (Exception ex)
                {
                    context.Logger.LogImportantMessage("Could not convert input texture for processing. " + ex.ToString());
                    throw;
                }

                if (GenerateMipmaps)
                    input.GenerateMipmaps(true);

                for (int f = 0; f < input.Faces.Count; ++f)
                {
                    var face = input.Faces[f];
                    for (int m = 0; m < face.Count; ++m)
                    {
                        var bmp = (PixelBitmapContent<Vector4>)face[m];

                        if (ColorKeyEnabled)
                        {
                            bmp.ReplaceColor(ColorKeyColor.ToVector4(), Vector4.Zero);
                        }

                        if (ResizeToPowerOfTwo)
                        {
                            if (!GraphicsUtil.IsPowerOfTwo(bmp.Width) || !GraphicsUtil.IsPowerOfTwo(bmp.Height) || (MakeSquare && bmp.Height != bmp.Width))
                            {
                                var newWidth = GraphicsUtil.GetNextPowerOfTwo(bmp.Width);
                                var newHeight = GraphicsUtil.GetNextPowerOfTwo(bmp.Height);
                                if (MakeSquare)
                                    newWidth = newHeight = Math.Max(newWidth, newHeight);
                                var resized = new PixelBitmapContent<Vector4>(newWidth, newHeight);
                                BitmapContent.Copy(bmp, resized);
                                bmp = resized;
                            }
                        }
                        else if (MakeSquare && bmp.Height != bmp.Width)
                        {
                            var newSize = Math.Max(bmp.Width, bmp.Height);
                            var resized = new PixelBitmapContent<Vector4>(newSize, newSize);
                            BitmapContent.Copy(bmp, resized);
                        }

                        if (PremultiplyAlpha)
                        {
                            for (int y = 0; y < bmp.Height; ++y)
                            {
                                var row = bmp.GetRow(y);
                                for (int x = 0; x < bmp.Width; ++x)
                                    row[x] = Color.FromNonPremultiplied(row[x]).ToVector4();
                            }
                        }

                        face[m] = bmp;
                    }
                }

                // If no change to the surface format was desired, change it back now before it early outs
                if (TextureFormat == TextureProcessorOutputFormat.NoChange)
                    input.ConvertBitmapType(originalType);
            }

            // Get the texture profile for the platform and let it convert the texture.
            var texProfile = TextureProfile.ForPlatform(context.TargetPlatform);
            texProfile.ConvertTexture(context, input, TextureFormat, false);

            return input;
        }
    }
}
