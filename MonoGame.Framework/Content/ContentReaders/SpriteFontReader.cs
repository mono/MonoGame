// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    internal class SpriteFontReader : ContentTypeReader<SpriteFont>
    {
        public SpriteFontReader()
        {
        }

        protected internal override SpriteFont Read(ContentReader input, SpriteFont existingInstance)
        {
            if (existingInstance != null)
            {
                // Read the texture into the existing texture instance
                input.ReadObject<Texture2D>(existingInstance.Texture);
                
                // discard the rest of the SpriteFont data as we are only reloading GPU resources for now
                input.ReadObject<List<Rectangle>>();
                input.ReadObject<List<Rectangle>>();
                input.ReadObject<List<char>>();
                input.ReadInt32();
                input.ReadSingle();
                input.ReadObject<List<Vector3>>();
                if (input.ReadBoolean())
                {
                    input.ReadChar();
                }

                return existingInstance;
            }
            else
            {
                // Create a fresh SpriteFont instance
                Texture2D texture = input.ReadObject<Texture2D>();
                List<Rectangle> glyphs = input.ReadObject<List<Rectangle>>();
                List<Rectangle> cropping = input.ReadObject<List<Rectangle>>();
                List<char> charMap = input.ReadObject<List<char>>();
                int lineSpacing = input.ReadInt32();
                float spacing = input.ReadSingle();
                List<Vector3> kerning = input.ReadObject<List<Vector3>>();
                char? defaultCharacter = null;
                if (input.ReadBoolean())
                {
                    defaultCharacter = new char?(input.ReadChar());
                }
                return new SpriteFont(texture, glyphs, cropping, charMap, lineSpacing, spacing, kerning, defaultCharacter);
            }
        }
    }
}
