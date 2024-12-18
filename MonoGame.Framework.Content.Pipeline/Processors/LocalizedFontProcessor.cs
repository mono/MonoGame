#region File Description
//-----------------------------------------------------------------------------
// LocalizedFontProcessor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.ComponentModel;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
#endregion

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Custom processor extends the SpriteFont build process to scan over the resource
    /// strings used by the game, automatically adding whatever characters it finds in
    /// them to the font. This makes sure the game will always have all the characters
    /// it needs, no matter what languages it is localized into, while still producing
    /// an efficient font that does not waste space on unnecessary characters. This is
    /// especially useful for languages such as Japanese and Korean, which have
    /// potentially thousands of different characters, although games typically only
    /// use a small fraction of these. Building only the characters we need is far more
    /// efficient than if we tried to include the entire CJK character region.
    /// </summary>
    [ContentProcessor]
    public class LocalizedFontProcessor : ContentProcessor<LocalizedFontDescription,
                                                    SpriteFontContent>
    {
        /// <summary>
        /// Gets or Sets the premultiply alpha flag.
        /// </summary>
        [DefaultValue(true)]
        public virtual bool PremultiplyAlpha { get; set; }

        /// <summary>
        /// Gets or Sets the target texture output format.
        /// </summary>
        [DefaultValue(typeof(TextureProcessorOutputFormat), "Compressed")]
        public virtual TextureProcessorOutputFormat TextureFormat { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedFontProcessor"/> class.
        /// Sets the <see cref="PremultiplyAlpha"/> property to true and the
        /// <see cref="TextureFormat"/> property to <see cref="TextureProcessorOutputFormat.Compressed"/>.
        /// </summary>
        public LocalizedFontProcessor ()
        {
              PremultiplyAlpha = true;
              TextureFormat = TextureProcessorOutputFormat.Compressed;
        }

        /// <summary>
        /// Converts a font description into SpriteFont format.
        /// </summary>
        public override SpriteFontContent Process(LocalizedFontDescription input,
                                                  ContentProcessorContext context)
        {
            // Scan each .resx file in turn.
            foreach (string resourceFile in input.ResourceFiles)
            {
                string absolutePath = Path.GetFullPath(resourceFile.Replace ('\\', Path.DirectorySeparatorChar).Replace ('/', Path.DirectorySeparatorChar));

                // Make sure the .resx file really does exist.
                if (!File.Exists(absolutePath))
                {
                    throw new InvalidContentException("Can't find " + absolutePath);
                }

                // Load the .resx data.
                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.Load(absolutePath);

                // Scan each string from the .resx file.
                foreach (XmlNode xmlNode in xmlDocument.SelectNodes("root/data/value"))
                {
                    string resourceString = xmlNode.InnerText;

                    // Scan each character of the string.
                    foreach (char usedCharacter in resourceString)
                    {
                        if (!input.Characters.Contains (usedCharacter))
                            input.Characters.Add(usedCharacter);
                    }
                }

                // Mark that this font should be rebuilt if the resource file changes.
                context.AddDependency(absolutePath);
            }

            var parameters = new OpaqueDataDictionary ();
            parameters.Add ("PremultiplyAlpha", PremultiplyAlpha);
            parameters.Add ("TextureFormat", TextureFormat);
            // After adding the necessary characters, we can use the built in
            // FontDescriptionProcessor to do the hard work of building the font for us.
            return context.Convert<FontDescription,
                SpriteFontContent>(input, "FontDescriptionProcessor", parameters);
        }
    }
}
