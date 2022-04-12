// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Utilities;
using Glyph = Microsoft.Xna.Framework.Content.Pipeline.Graphics.Glyph;

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Sprite Font Description - MonoGame")]
    public class FontDescriptionProcessor : ContentProcessor<FontDescription, SpriteFontContent>
    {
        [DefaultValue(true)]
        public virtual bool PremultiplyAlpha { get; set; }

        [DefaultValue(typeof(TextureProcessorOutputFormat), "Compressed")]
        public virtual TextureProcessorOutputFormat TextureFormat { get; set; }

        public FontDescriptionProcessor()
        {
            PremultiplyAlpha = true;
            TextureFormat = TextureProcessorOutputFormat.Compressed;
        }

        public override SpriteFontContent Process(FontDescription input, ContentProcessorContext context)
        {
            var output = new SpriteFontContent(input);
            var fontFile = FindFont(input.FontName, input.Style.ToString());

            if (string.IsNullOrWhiteSpace(fontFile))
            {
                var directories = new List<string> { Path.GetDirectoryName(input.Identity.SourceFilename) };
                var extensions = new string[] { "", ".ttf", ".ttc", ".otf" };

                // Add special per platform directories
                if (CurrentPlatform.OS == OS.Windows)
                    directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts"));
                else if (CurrentPlatform.OS == OS.MacOSX)
                {
                    directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Fonts"));
                    directories.Add("/Library/Fonts");
                    directories.Add("/System/Library/Fonts/Supplemental");
                }

                foreach (var dir in directories)
                {
                    foreach(var ext in extensions)
                    {
                        fontFile = Path.Combine(dir, input.FontName + ext);
                        if (File.Exists(fontFile))
                            break;
                    }
                    if (File.Exists(fontFile))
                        break;
                }
            }

            if (!File.Exists(fontFile))
                throw new FileNotFoundException("Could not find \"" + input.FontName + "\" font file at \"" + fontFile + "\".");

			context.Logger.LogMessage ("Building Font {0}", fontFile);
            
            // Get the platform specific texture profile.
            var texProfile = TextureProfile.ForPlatform(context.TargetPlatform);

            {
				if (!File.Exists(fontFile)) {
					throw new Exception(string.Format("Could not load {0}", fontFile));
				}
				var lineSpacing = 0f;
				int yOffsetMin = 0;
				var glyphs = ImportFont(input, out lineSpacing, out yOffsetMin, context, fontFile);

				// Optimize.
				foreach (Glyph glyph in glyphs)
				{
					GlyphCropper.Crop(glyph);
				}

                // We need to know how to pack the glyphs.
                bool requiresPot, requiresSquare;
                texProfile.Requirements(context, TextureFormat, out requiresPot, out requiresSquare);

                var face = GlyphPacker.ArrangeGlyphs(glyphs, requiresPot, requiresSquare);

				// Adjust line and character spacing.
				lineSpacing += input.Spacing;
				output.VerticalLineSpacing = (int)lineSpacing;

				foreach (var glyph in glyphs)
				{
                    output.CharacterMap.Add(glyph.Character);

					var texRect = new Rectangle(glyph.Subrect.X, glyph.Subrect.Y, glyph.Subrect.Width, glyph.Subrect.Height);
					output.Glyphs.Add(texRect);

					var cropping = new Rectangle(0, (int)(glyph.YOffset - yOffsetMin), (int)glyph.XAdvance, output.VerticalLineSpacing);
					output.Cropping.Add(cropping);

					// Set the optional character kerning.
					if (input.UseKerning)
						output.Kerning.Add(new Vector3(glyph.CharacterWidths.A, glyph.CharacterWidths.B, glyph.CharacterWidths.C));
					else
						output.Kerning.Add(new Vector3(0, texRect.Width, 0));
				}

                output.Texture.Faces[0].Add(face);            
			}

            if (PremultiplyAlpha)
            {
                var bmp = output.Texture.Faces[0][0];
                var data = bmp.GetPixelData();
                var idx = 0;
                for (; idx < data.Length; )
                {
                    var r = data[idx];

                    // Special case of simply copying the R component into the A, since R is the value of white alpha we want
                    data[idx + 0] = r;
                    data[idx + 1] = r;
                    data[idx + 2] = r;
                    data[idx + 3] = r;

                    idx += 4;
                }

                bmp.SetPixelData(data);
            }
            else
            {
                var bmp = output.Texture.Faces[0][0];
                var data = bmp.GetPixelData();
                var idx = 0;
                for (; idx < data.Length; )
                {
                    var r = data[idx];

                    // Special case of simply moving the R component into the A and setting RGB to solid white, since R is the value of white alpha we want
                    data[idx + 0] = 255;
                    data[idx + 1] = 255;
                    data[idx + 2] = 255;
                    data[idx + 3] = r;

                    idx += 4;
                }

                bmp.SetPixelData(data);
            }

            // Perform the final texture conversion.
            texProfile.ConvertTexture(context, output.Texture, TextureFormat, true);    

            return output;
        }

		private static Glyph[] ImportFont(FontDescription options, out float lineSpacing, out int yOffsetMin, ContentProcessorContext context, string fontName)
		{
			// Which importer knows how to read this source font?
			IFontImporter importer;

			var TrueTypeFileExtensions = new List<string> { ".ttf", ".ttc", ".otf" };
			//var BitmapFileExtensions = new List<string> { ".bmp", ".png", ".gif" };

			string fileExtension = Path.GetExtension(fontName).ToLowerInvariant();

			//			if (BitmapFileExtensions.Contains(fileExtension))
			//			{
			//				importer = new BitmapImporter();
			//			}
			//			else
			//			{
			if (!TrueTypeFileExtensions.Contains(fileExtension)) 
                throw new PipelineException("Unknown file extension " + fileExtension);

			importer = new SharpFontImporter();

			// Import the source font data.
			importer.Import(options, fontName);

			lineSpacing = importer.LineSpacing;
			yOffsetMin = importer.YOffsetMin;

			// Get all glyphs
			var glyphs = new List<Glyph>(importer.Glyphs);

			// Validate.
			if (glyphs.Count == 0)
			{
				throw new Exception("Font does not contain any glyphs.");
			}

			// Sort the glyphs
			glyphs.Sort((left, right) => left.Character.CompareTo(right.Character));


			// Check that the default character is part of the glyphs
			if (options.DefaultCharacter != null)
			{
				bool defaultCharacterFound = false;
				foreach (var glyph in glyphs)
				{
					if (glyph.Character == options.DefaultCharacter)
					{
						defaultCharacterFound = true;
						break;
					}
				}
				if (!defaultCharacterFound)
				{
					throw new InvalidOperationException("The specified DefaultCharacter is not part of this font.");
				}
			}

			return glyphs.ToArray();
		}

        private string FindFont(string name, string style)
        {
            if (CurrentPlatform.OS == OS.Windows)
            {
                var fontDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
                foreach (var key in new RegistryKey[] { Registry.LocalMachine, Registry.CurrentUser })
                {
                    var subkey = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", false);
                    foreach (var font in subkey.GetValueNames().OrderBy(x => x))
                    {
                        if (font.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                        {
                            var fontPath = subkey.GetValue(font).ToString();

                            // The registry value might have trailing NUL characters
                            // See https://github.com/MonoGame/MonoGame/issues/4061
                            var nulIndex = fontPath.IndexOf('\0');
                            if (nulIndex != -1)
                                fontPath = fontPath.Substring(0, nulIndex);

                            return Path.IsPathRooted(fontPath) ? fontPath : Path.Combine(fontDirectory, fontPath);
                        }
                    }
                }
            }
            else if (CurrentPlatform.OS == OS.Linux)
            {
                string s, e;
                ExternalTool.Run("/bin/bash", string.Format("-c \"fc-match -f '%{{file}}:%{{family}}\\n' '{0}:style={1}'\"", name, style), out s, out e);
                s = s.Trim();

                var split = s.Split(':');
                if (split.Length < 2)
                    return string.Empty;

                // check font family, fontconfig might return a fallback
                if (split[1].Contains(","))
                {
                    // this file defines multiple family names
                    var families = split[1].Split(',');
                    foreach (var f in families)
                    {
                        if (f.ToLowerInvariant() == name.ToLowerInvariant())
                            return split[0];
                    }
                    // didn't find it
                    return string.Empty;
                }
                else
                {
                    if (split[1].ToLowerInvariant() != name.ToLowerInvariant())
                        return string.Empty;
                }

                return split[0];
            }

            return String.Empty;
        }
    }
}
