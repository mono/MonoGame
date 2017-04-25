// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Describes a 32-bit packed color.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
#if XNADESIGNPROVIDED
    [System.ComponentModel.TypeConverter(typeof(Microsoft.Xna.Framework.Design.ColorConverter))]
#endif
    public struct Color : IEquatable<Color>
    {
        static Color()
        {
            TransparentBlack = new Color(0);
            Transparent = new Color(0);
            AliceBlue = new Color(0xfffff8f0);
            AntiqueWhite = new Color(0xffd7ebfa);
            Aqua = new Color(0xffffff00);
            Aquamarine = new Color(0xffd4ff7f);
            Azure = new Color(0xfffffff0);
            Beige = new Color(0xffdcf5f5);
            Bisque = new Color(0xffc4e4ff);
            Black = new Color(0xff000000);
            BlanchedAlmond = new Color(0xffcdebff);
            Blue = new Color(0xffff0000);
            BlueViolet = new Color(0xffe22b8a);
            Brown = new Color(0xff2a2aa5);
            BurlyWood = new Color(0xff87b8de);
            CadetBlue = new Color(0xffa09e5f);
            Chartreuse = new Color(0xff00ff7f);
            Chocolate = new Color(0xff1e69d2);
            Coral = new Color(0xff507fff);
            CornflowerBlue = new Color(0xffed9564);
            Cornsilk = new Color(0xffdcf8ff);
            Crimson = new Color(0xff3c14dc);
            Cyan = new Color(0xffffff00);
            DarkBlue = new Color(0xff8b0000);
            DarkCyan = new Color(0xff8b8b00);
            DarkGoldenrod = new Color(0xff0b86b8);
            DarkGray = new Color(0xffa9a9a9);
            DarkGreen = new Color(0xff006400);
            DarkKhaki = new Color(0xff6bb7bd);
            DarkMagenta = new Color(0xff8b008b);
            DarkOliveGreen = new Color(0xff2f6b55);
            DarkOrange = new Color(0xff008cff);
            DarkOrchid = new Color(0xffcc3299);
            DarkRed = new Color(0xff00008b);
            DarkSalmon = new Color(0xff7a96e9);
            DarkSeaGreen = new Color(0xff8bbc8f);
            DarkSlateBlue = new Color(0xff8b3d48);
            DarkSlateGray = new Color(0xff4f4f2f);
            DarkTurquoise = new Color(0xffd1ce00);
            DarkViolet = new Color(0xffd30094);
            DeepPink = new Color(0xff9314ff);
            DeepSkyBlue = new Color(0xffffbf00);
            DimGray = new Color(0xff696969);
            DodgerBlue = new Color(0xffff901e);
            Firebrick = new Color(0xff2222b2);
            FloralWhite = new Color(0xfff0faff);
            ForestGreen = new Color(0xff228b22);
            Fuchsia = new Color(0xffff00ff);
            Gainsboro = new Color(0xffdcdcdc);
            GhostWhite = new Color(0xfffff8f8);
            Gold = new Color(0xff00d7ff);
            Goldenrod = new Color(0xff20a5da);
            Gray = new Color(0xff808080);
            Green = new Color(0xff008000);
            GreenYellow = new Color(0xff2fffad);
            Honeydew = new Color(0xfff0fff0);
            HotPink = new Color(0xffb469ff);
            IndianRed = new Color(0xff5c5ccd);
            Indigo = new Color(0xff82004b);
            Ivory = new Color(0xfff0ffff);
            Khaki = new Color(0xff8ce6f0);
            Lavender = new Color(0xfffae6e6);
            LavenderBlush = new Color(0xfff5f0ff);
            LawnGreen = new Color(0xff00fc7c);
            LemonChiffon = new Color(0xffcdfaff);
            LightBlue = new Color(0xffe6d8ad);
            LightCoral = new Color(0xff8080f0);
            LightCyan = new Color(0xffffffe0);
            LightGoldenrodYellow = new Color(0xffd2fafa);
            LightGray = new Color(0xffd3d3d3);
            LightGreen = new Color(0xff90ee90);
            LightPink = new Color(0xffc1b6ff);
            LightSalmon = new Color(0xff7aa0ff);
            LightSeaGreen = new Color(0xffaab220);
            LightSkyBlue = new Color(0xffface87);
            LightSlateGray = new Color(0xff998877);
            LightSteelBlue = new Color(0xffdec4b0);
            LightYellow = new Color(0xffe0ffff);
            Lime = new Color(0xff00ff00);
            LimeGreen = new Color(0xff32cd32);
            Linen = new Color(0xffe6f0fa);
            Magenta = new Color(0xffff00ff);
            Maroon = new Color(0xff000080);
            MediumAquamarine = new Color(0xffaacd66);
            MediumBlue = new Color(0xffcd0000);
            MediumOrchid = new Color(0xffd355ba);
            MediumPurple = new Color(0xffdb7093);
            MediumSeaGreen = new Color(0xff71b33c);
            MediumSlateBlue = new Color(0xffee687b);
            MediumSpringGreen = new Color(0xff9afa00);
            MediumTurquoise = new Color(0xffccd148);
            MediumVioletRed = new Color(0xff8515c7);
            MidnightBlue = new Color(0xff701919);
            MintCream = new Color(0xfffafff5);
            MistyRose = new Color(0xffe1e4ff);
            Moccasin = new Color(0xffb5e4ff);
            MonoGameOrange = new Color(0xff003ce7);
            NavajoWhite = new Color(0xffaddeff);
            Navy = new Color(0xff800000);
            OldLace = new Color(0xffe6f5fd);
            Olive = new Color(0xff008080);
            OliveDrab = new Color(0xff238e6b);
            Orange = new Color(0xff00a5ff);
            OrangeRed = new Color(0xff0045ff);
            Orchid = new Color(0xffd670da);
            PaleGoldenrod = new Color(0xffaae8ee);
            PaleGreen = new Color(0xff98fb98);
            PaleTurquoise = new Color(0xffeeeeaf);
            PaleVioletRed = new Color(0xff9370db);
            PapayaWhip = new Color(0xffd5efff);
            PeachPuff = new Color(0xffb9daff);
            Peru = new Color(0xff3f85cd);
            Pink = new Color(0xffcbc0ff);
            Plum = new Color(0xffdda0dd);
            PowderBlue = new Color(0xffe6e0b0);
            Purple = new Color(0xff800080);
            Red = new Color(0xff0000ff);
            RosyBrown = new Color(0xff8f8fbc);
            RoyalBlue = new Color(0xffe16941);
            SaddleBrown = new Color(0xff13458b);
            Salmon= new Color(0xff7280fa);
            SandyBrown = new Color(0xff60a4f4);
            SeaGreen = new Color(0xff578b2e);
            SeaShell = new Color(0xffeef5ff);
            Sienna = new Color(0xff2d52a0);
            Silver  = new Color(0xffc0c0c0);
            SkyBlue  = new Color(0xffebce87);
            SlateBlue= new Color(0xffcd5a6a);
            SlateGray= new Color(0xff908070);
            Snow= new Color(0xfffafaff);
            SpringGreen= new Color(0xff7fff00);
            SteelBlue= new Color(0xffb48246);
            Tan= new Color(0xff8cb4d2);
            Teal= new Color(0xff808000);
            Thistle= new Color(0xffd8bfd8);
            Tomato= new Color(0xff4763ff);
            Turquoise= new Color(0xffd0e040);
            Violet= new Color(0xffee82ee);
            Wheat= new Color(0xffb3def5);
            White= new Color(uint.MaxValue);
            WhiteSmoke= new Color(0xfff5f5f5);
            Yellow = new Color(0xff00ffff);
            YellowGreen = new Color(0xff32cd9a);
        }

        // Stored as RGBA with R in the least significant octet:
        // |-------|-------|-------|-------
        // A       B       G       R
        private uint _packedValue;
	  
        /// <summary>
        /// Constructs an RGBA color from a packed value.
        /// The value is a 32-bit unsigned integer, with R in the least significant octet.
        /// </summary>
        /// <param name="packedValue">The packed value.</param>
        [CLSCompliant(false)]
        public Color(uint packedValue)
        {
            _packedValue = packedValue;
        }

        /// <summary>
        /// Constructs an RGBA color from the XYZW unit length components of a vector.
        /// </summary>
        /// <param name="color">A <see cref="Vector4"/> representing color.</param>
        public Color(Vector4 color)
            : this((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255), (int)(color.W * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from the XYZ unit length components of a vector. Alpha value will be opaque.
        /// </summary>
        /// <param name="color">A <see cref="Vector3"/> representing color.</param>
        public Color(Vector3 color)
            : this((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from a <see cref="Color"/> and an alpha value.
        /// </summary>
        /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public Color(Color color, int alpha)
        {
            if ((alpha & 0xFFFFFF00) != 0)
            {
                var clampedA = (uint)MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                _packedValue = (color._packedValue & 0x00FFFFFF) | (clampedA << 24);
            }
            else
            {
                _packedValue = (color._packedValue & 0x00FFFFFF) | ((uint)alpha << 24);
            }
        }

        /// <summary>
        /// Constructs an RGBA color from color and alpha value.
        /// </summary>
        /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public Color(Color color, float alpha):
            this(color, (int)(alpha * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        public Color(float r, float g, float b)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public Color(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public Color(int r, int g, int b)
        {
            _packedValue = 0xFF000000; // A = 255

            if (((r | g | b) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)MathHelper.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)MathHelper.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)MathHelper.Clamp(b, Byte.MinValue, Byte.MaxValue);

                _packedValue |= (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                _packedValue |= ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="alpha">Alpha component value from 0 to 255.</param>
        public Color(int r, int g, int b, int alpha)
        {
            if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)MathHelper.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)MathHelper.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)MathHelper.Clamp(b, Byte.MinValue, Byte.MaxValue);
                var clampedA = (uint)MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                _packedValue = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping, and may therefore be faster than the other overloads.
        /// </remarks>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="alpha"></param>
        public Color(byte r, byte g, byte b, byte alpha)
        {
            _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (r);
        }

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        [DataMember]
        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte) (this._packedValue >> 16);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xff00ffff) | ((uint)value << 16);
            }
        }

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        [DataMember]
        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(this._packedValue >> 8);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffff00ff) | ((uint)value << 8);
            }
        }

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        [DataMember]
        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte) this._packedValue;
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffffff00) | value;
            }
        }

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        [DataMember]
        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(this._packedValue >> 24);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0x00ffffff) | ((uint)value << 24);
            }
        }
		
	/// <summary>
        /// Compares whether two <see cref="Color"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Color"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Color"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Color a, Color b)
        {
            return (a._packedValue == b._packedValue);
        }
	
	/// <summary>
        /// Compares whether two <see cref="Color"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Color"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Color"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
        public static bool operator !=(Color a, Color b)
        {
            return (a._packedValue != b._packedValue);
        }

        /// <summary>
        /// Gets the hash code of this <see cref="Color"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Color"/>.</returns>
        public override int GetHashCode()
        {
            return this._packedValue.GetHashCode();
        }
	
        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is Color) && this.Equals((Color)obj));
        }

        #region Color Bank
        /// <summary>
        /// TransparentBlack color (R:0,G:0,B:0,A:0).
        /// </summary>
        public static Color TransparentBlack
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Transparent color (R:0,G:0,B:0,A:0).
        /// </summary>
        public static Color Transparent
        {
            get;
            private set;
        }
	
	/// <summary>
        /// AliceBlue color (R:240,G:248,B:255,A:255).
        /// </summary>
        public static Color AliceBlue
        {
            get;
            private set;
        }
        
        /// <summary>
        /// AntiqueWhite color (R:250,G:235,B:215,A:255).
        /// </summary>
        public static Color AntiqueWhite
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Aqua color (R:0,G:255,B:255,A:255).
        /// </summary>
	public static Color Aqua
        {
            get;
            private set;
        }
	
	/// <summary>
        /// Aquamarine color (R:127,G:255,B:212,A:255).
        /// </summary>
        public static Color Aquamarine
    {
        get;
        private set;
    }
        
        /// <summary>
        /// Azure color (R:240,G:255,B:255,A:255).
        /// </summary>
	public static Color Azure
        {
            get;
            private set;
        }
	
	/// <summary>
        /// Beige color (R:245,G:245,B:220,A:255).
        /// </summary>
        public static Color Beige
    {
        get;
        private set;
    }
        
        /// <summary>
        /// Bisque color (R:255,G:228,B:196,A:255).
        /// </summary>
        public static Color Bisque
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Black color (R:0,G:0,B:0,A:255).
        /// </summary>
        public static Color Black
        {
            get;
            private set;
        }
        
        /// <summary>
        /// BlanchedAlmond color (R:255,G:235,B:205,A:255).
        /// </summary>
        public static Color BlanchedAlmond
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Blue color (R:0,G:0,B:255,A:255).
        /// </summary>
        public static Color Blue
        {
            get;
            private set;
        }
        
        /// <summary>
        /// BlueViolet color (R:138,G:43,B:226,A:255).
        /// </summary>
        public static Color BlueViolet
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Brown color (R:165,G:42,B:42,A:255).
        /// </summary>
        public static Color Brown
        {
            get;
            private set;
        }
        
        /// <summary>
        /// BurlyWood color (R:222,G:184,B:135,A:255).
        /// </summary>
        public static Color BurlyWood
        {
            get;
            private set;
        }
        
        /// <summary>
        /// CadetBlue color (R:95,G:158,B:160,A:255).
        /// </summary>
        public static Color CadetBlue
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Chartreuse color (R:127,G:255,B:0,A:255).
        /// </summary>
        public static Color Chartreuse
        {
            get;
            private set;
        }
         
        /// <summary>
        /// Chocolate color (R:210,G:105,B:30,A:255).
        /// </summary>
        public static Color Chocolate
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Coral color (R:255,G:127,B:80,A:255).
        /// </summary>
        public static Color Coral
        {
            get;
            private set;
        }
        
        /// <summary>
        /// CornflowerBlue color (R:100,G:149,B:237,A:255).
        /// </summary>
        public static Color CornflowerBlue
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Cornsilk color (R:255,G:248,B:220,A:255).
        /// </summary>
	public static Color Cornsilk
        {
            get;
            private set;
        }
	
	/// <summary>
        /// Crimson color (R:220,G:20,B:60,A:255).
        /// </summary>
        public static Color Crimson
    {
        get;
        private set;
    }
        
        /// <summary>
        /// Cyan color (R:0,G:255,B:255,A:255).
        /// </summary>
        public static Color Cyan
        {
            get;
            private set;
        }
        
        /// <summary>
        /// DarkBlue color (R:0,G:0,B:139,A:255).
        /// </summary>
	public static Color DarkBlue
        {
            get;
            private set;
        }
	
	/// <summary>
        /// DarkCyan color (R:0,G:139,B:139,A:255).
        /// </summary>
        public static Color DarkCyan
    {
        get;
        private set;
    }
        
        /// <summary>
        /// DarkGoldenrod color (R:184,G:134,B:11,A:255).
        /// </summary>
        public static Color DarkGoldenrod
        {
            get;
            private set;
        }
        
        /// <summary>
        /// DarkGray color (R:169,G:169,B:169,A:255).
        /// </summary>
	public static Color DarkGray
        {
            get;
            private set;
        }
	
	/// <summary>
        /// DarkGreen color (R:0,G:100,B:0,A:255).
        /// </summary>
        public static Color DarkGreen
    {
        get;
        private set;
    }
        
        /// <summary>
        /// DarkKhaki color (R:189,G:183,B:107,A:255).
        /// </summary>
        public static Color DarkKhaki
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkMagenta color (R:139,G:0,B:139,A:255).
        /// </summary>
        public static Color DarkMagenta
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkOliveGreen color (R:85,G:107,B:47,A:255).
        /// </summary>
        public static Color DarkOliveGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkOrange color (R:255,G:140,B:0,A:255).
        /// </summary>
        public static Color DarkOrange
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkOrchid color (R:153,G:50,B:204,A:255).
        /// </summary>
        public static Color DarkOrchid
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkRed color (R:139,G:0,B:0,A:255).
        /// </summary>
        public static Color DarkRed
        {
            get;
            private set;
        }
        
	/// <summary>
        /// DarkSalmon color (R:233,G:150,B:122,A:255).
        /// </summary>
        public static Color DarkSalmon
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkSeaGreen color (R:143,G:188,B:139,A:255).
        /// </summary>
        public static Color DarkSeaGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkSlateBlue color (R:72,G:61,B:139,A:255).
        /// </summary>
        public static Color DarkSlateBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkSlateGray color (R:47,G:79,B:79,A:255).
        /// </summary>
        public static Color DarkSlateGray
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkTurquoise color (R:0,G:206,B:209,A:255).
        /// </summary>
        public static Color DarkTurquoise
        {
            get;
            private set;
        }

        /// <summary>
        /// DarkViolet color (R:148,G:0,B:211,A:255).
        /// </summary>
        public static Color DarkViolet
        {
            get;
            private set;
        }
         
        /// <summary>
        /// DeepPink color (R:255,G:20,B:147,A:255).
        /// </summary>
        public static Color DeepPink
        {
            get;
            private set;
        }

        /// <summary>
        /// DeepSkyBlue color (R:0,G:191,B:255,A:255).
        /// </summary>
        public static Color DeepSkyBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// DimGray color (R:105,G:105,B:105,A:255).
        /// </summary>
        public static Color DimGray
        {
            get;
            private set;
        }

        /// <summary>
        /// DodgerBlue color (R:30,G:144,B:255,A:255).
        /// </summary>
        public static Color DodgerBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// Firebrick color (R:178,G:34,B:34,A:255).
        /// </summary>
        public static Color Firebrick
        {
            get;
            private set;
        }

        /// <summary>
        /// FloralWhite color (R:255,G:250,B:240,A:255).
        /// </summary>
        public static Color FloralWhite
        {
            get;
            private set;
        }

        /// <summary>
        /// ForestGreen color (R:34,G:139,B:34,A:255).
        /// </summary>
        public static Color ForestGreen
        {
            get;
            private set;
        }
        
	/// <summary>
        /// Fuchsia color (R:255,G:0,B:255,A:255).
        /// </summary>
        public static Color Fuchsia
        {
            get;
            private set;
        }

        /// <summary>
        /// Gainsboro color (R:220,G:220,B:220,A:255).
        /// </summary>
        public static Color Gainsboro
        {
            get;
            private set;
        }

        /// <summary>
        /// GhostWhite color (R:248,G:248,B:255,A:255).
        /// </summary>
        public static Color GhostWhite
        {
            get;
            private set;
        }
        /// <summary>
        /// Gold color (R:255,G:215,B:0,A:255).
        /// </summary>
        public static Color Gold
        {
            get;
            private set;
        }

        /// <summary>
        /// Goldenrod color (R:218,G:165,B:32,A:255).
        /// </summary>
        public static Color Goldenrod
        {
            get;
            private set;
        }

        /// <summary>
        /// Gray color (R:128,G:128,B:128,A:255).
        /// </summary>
        public static Color Gray
        {
            get;
            private set;
        }

        /// <summary>
        /// Green color (R:0,G:128,B:0,A:255).
        /// </summary>
        public static Color Green
        {
            get;
            private set;
        }

        /// <summary>
        /// GreenYellow color (R:173,G:255,B:47,A:255).
        /// </summary>
        public static Color GreenYellow
        {
            get;
            private set;
        }

        /// <summary>
        /// Honeydew color (R:240,G:255,B:240,A:255).
        /// </summary>
        public static Color Honeydew
        {
            get;
            private set;
        }

        /// <summary>
        /// HotPink color (R:255,G:105,B:180,A:255).
        /// </summary>
        public static Color HotPink
        {
            get;
            private set;
        }
        
        /// <summary>
        /// IndianRed color (R:205,G:92,B:92,A:255).
        /// </summary>
        public static Color IndianRed
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Indigo color (R:75,G:0,B:130,A:255).
        /// </summary>
        public static Color Indigo
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Ivory color (R:255,G:255,B:240,A:255).
        /// </summary>
        public static Color Ivory
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Khaki color (R:240,G:230,B:140,A:255).
        /// </summary>
        public static Color Khaki
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Lavender color (R:230,G:230,B:250,A:255).
        /// </summary>
        public static Color Lavender
        {
            get;
            private set;
        }
        
        /// <summary>
        /// LavenderBlush color (R:255,G:240,B:245,A:255).
        /// </summary>
        public static Color LavenderBlush
        {
            get;
            private set;
        }
        
        /// <summary>
        /// LawnGreen color (R:124,G:252,B:0,A:255).
        /// </summary>
        public static Color LawnGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// LemonChiffon color (R:255,G:250,B:205,A:255).
        /// </summary>
        public static Color LemonChiffon
        {
            get;
            private set;
        }

        /// <summary>
        /// LightBlue color (R:173,G:216,B:230,A:255).
        /// </summary>
        public static Color LightBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// LightCoral color (R:240,G:128,B:128,A:255).
        /// </summary>
        public static Color LightCoral
        {
            get;
            private set;
        }
        
        /// <summary>
        /// LightCyan color (R:224,G:255,B:255,A:255).
        /// </summary>
        public static Color LightCyan
        {
            get;
            private set;
        }

        /// <summary>
        /// LightGoldenrodYellow color (R:250,G:250,B:210,A:255).
        /// </summary>
        public static Color LightGoldenrodYellow
        {
            get;
            private set;
        }
        
        /// <summary>
        /// LightGray color (R:211,G:211,B:211,A:255).
        /// </summary>
        public static Color LightGray
        {
            get;
            private set;
        }

        /// <summary>
        /// LightGreen color (R:144,G:238,B:144,A:255).
        /// </summary>
        public static Color LightGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// LightPink color (R:255,G:182,B:193,A:255).
        /// </summary>
        public static Color LightPink
        {
            get;
            private set;
        }

        /// <summary>
        /// LightSalmon color (R:255,G:160,B:122,A:255).
        /// </summary>
        public static Color LightSalmon
        {
            get;
            private set;
        }

        /// <summary>
        /// LightSeaGreen color (R:32,G:178,B:170,A:255).
        /// </summary>
        public static Color LightSeaGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// LightSkyBlue color (R:135,G:206,B:250,A:255).
        /// </summary>
        public static Color LightSkyBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// LightSlateGray color (R:119,G:136,B:153,A:255).
        /// </summary>
        public static Color LightSlateGray
        {
            get;
            private set;
        }

        /// <summary>
        /// LightSteelBlue color (R:176,G:196,B:222,A:255).
        /// </summary>
        public static Color LightSteelBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// LightYellow color (R:255,G:255,B:224,A:255).
        /// </summary>
        public static Color LightYellow
        {
            get;
            private set;
        }

        /// <summary>
        /// Lime color (R:0,G:255,B:0,A:255).
        /// </summary>
        public static Color Lime
        {
            get;
            private set;
        }

        /// <summary>
        /// LimeGreen color (R:50,G:205,B:50,A:255).
        /// </summary>
        public static Color LimeGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// Linen color (R:250,G:240,B:230,A:255).
        /// </summary>
        public static Color Linen
        {
            get;
            private set;
        }

        /// <summary>
        /// Magenta color (R:255,G:0,B:255,A:255).
        /// </summary>
        public static Color Magenta
        {
            get;
            private set;
        }

        /// <summary>
        /// Maroon color (R:128,G:0,B:0,A:255).
        /// </summary>
        public static Color Maroon
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumAquamarine color (R:102,G:205,B:170,A:255).
        /// </summary>
        public static Color MediumAquamarine
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumBlue color (R:0,G:0,B:205,A:255).
        /// </summary>
        public static Color MediumBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumOrchid color (R:186,G:85,B:211,A:255).
        /// </summary>
        public static Color MediumOrchid
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumPurple color (R:147,G:112,B:219,A:255).
        /// </summary>
        public static Color MediumPurple
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumSeaGreen color (R:60,G:179,B:113,A:255).
        /// </summary>
        public static Color MediumSeaGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumSlateBlue color (R:123,G:104,B:238,A:255).
        /// </summary>
        public static Color MediumSlateBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumSpringGreen color (R:0,G:250,B:154,A:255).
        /// </summary>
        public static Color MediumSpringGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumTurquoise color (R:72,G:209,B:204,A:255).
        /// </summary>
        public static Color MediumTurquoise
        {
            get;
            private set;
        }

        /// <summary>
        /// MediumVioletRed color (R:199,G:21,B:133,A:255).
        /// </summary>
        public static Color MediumVioletRed
        {
            get;
            private set;
        }

        /// <summary>
        /// MidnightBlue color (R:25,G:25,B:112,A:255).
        /// </summary>
        public static Color MidnightBlue
        {
            get;
            private set;
        }

        /// <summary>
        /// MintCream color (R:245,G:255,B:250,A:255).
        /// </summary>
        public static Color MintCream
        {
            get;
            private set;
        }

        /// <summary>
        /// MistyRose color (R:255,G:228,B:225,A:255).
        /// </summary>
        public static Color MistyRose
        {
            get;
            private set;
        }

        /// <summary>
        /// Moccasin color (R:255,G:228,B:181,A:255).
        /// </summary>
        public static Color Moccasin
        {
            get;
            private set;
        }

        /// <summary>
        /// MonoGame orange theme color (R:231,G:60,B:0,A:255).
        /// </summary>
        public static Color MonoGameOrange
        {
            get;
            private set;
        }

        /// <summary>
        /// NavajoWhite color (R:255,G:222,B:173,A:255).
        /// </summary>
        public static Color NavajoWhite
        {
            get;
            private set;
        }

        /// <summary>
        /// Navy color (R:0,G:0,B:128,A:255).
        /// </summary>
        public static Color Navy
        {
            get;
            private set;
        }

        /// <summary>
        /// OldLace color (R:253,G:245,B:230,A:255).
        /// </summary>
        public static Color OldLace
        {
            get;
            private set;
        }

        /// <summary>
        /// Olive color (R:128,G:128,B:0,A:255).
        /// </summary>
        public static Color Olive
        {
            get;
            private set;
        }

        /// <summary>
        /// OliveDrab color (R:107,G:142,B:35,A:255).
        /// </summary>
        public static Color OliveDrab
        {
            get;
            private set;
        }

        /// <summary>
        /// Orange color (R:255,G:165,B:0,A:255).
        /// </summary>
        public static Color Orange
        {
            get;
            private set;
        }

        /// <summary>
        /// OrangeRed color (R:255,G:69,B:0,A:255).
        /// </summary>
        public static Color OrangeRed
        {
            get;
            private set;
        }

        /// <summary>
        /// Orchid color (R:218,G:112,B:214,A:255).
        /// </summary>
        public static Color Orchid
        {
            get;
            private set;
        }

        /// <summary>
        /// PaleGoldenrod color (R:238,G:232,B:170,A:255).
        /// </summary>
        public static Color PaleGoldenrod
        {
            get;
            private set;
        }

        /// <summary>
        /// PaleGreen color (R:152,G:251,B:152,A:255).
        /// </summary>
        public static Color PaleGreen
        {
            get;
            private set;
        }

        /// <summary>
        /// PaleTurquoise color (R:175,G:238,B:238,A:255).
        /// </summary>
        public static Color PaleTurquoise
        {
            get;
            private set;
        }
        /// <summary>
        /// PaleVioletRed color (R:219,G:112,B:147,A:255).
        /// </summary>
        public static Color PaleVioletRed
        {
            get;
            private set;
        }

        /// <summary>
        /// PapayaWhip color (R:255,G:239,B:213,A:255).
        /// </summary>
        public static Color PapayaWhip
        {
            get;
            private set;
        }

        /// <summary>
        /// PeachPuff color (R:255,G:218,B:185,A:255).
        /// </summary>
        public static Color PeachPuff
        {
            get;
            private set;
        }

        /// <summary>
        /// Peru color (R:205,G:133,B:63,A:255).
        /// </summary>
        public static Color Peru
        {
            get;
            private set;
        }

        /// <summary>
        /// Pink color (R:255,G:192,B:203,A:255).
        /// </summary>
        public static Color Pink
        {
            get;
            private set;
        }

        /// <summary>
        /// Plum color (R:221,G:160,B:221,A:255).
        /// </summary>
        public static Color Plum
        {
            get;
            private set;
        }

        /// <summary>
        /// PowderBlue color (R:176,G:224,B:230,A:255).
        /// </summary>
        public static Color PowderBlue
        {
            get;
            private set;
        }

        /// <summary>
        ///  Purple color (R:128,G:0,B:128,A:255).
        /// </summary>
        public static Color Purple
        {
            get;
            private set;
        }

        /// <summary>
        /// Red color (R:255,G:0,B:0,A:255).
        /// </summary>
        public static Color Red
        {
            get;
            private set;
        }

        /// <summary>
        /// RosyBrown color (R:188,G:143,B:143,A:255).
        /// </summary>
        public static Color RosyBrown
        {
            get;
            private set;
        }

        /// <summary>
        /// RoyalBlue color (R:65,G:105,B:225,A:255).
        /// </summary>
        public static Color RoyalBlue
        {
            get;
            private set;
        }

    	/// <summary>
        /// SaddleBrown color (R:139,G:69,B:19,A:255).
        /// </summary>
        public static Color SaddleBrown
        {
            get;
            private set;
        }
    	 
        /// <summary>
        /// Salmon color (R:250,G:128,B:114,A:255).
        /// </summary>
        public static Color Salmon
        {
            get;
            private set;
        }
        
        /// <summary>
        /// SandyBrown color (R:244,G:164,B:96,A:255).
        /// </summary>
        public static Color SandyBrown
        {
            get;
            private set;
        }
        
        /// <summary>
        /// SeaGreen color (R:46,G:139,B:87,A:255).
        /// </summary>
        public static Color SeaGreen
        {
            get;
            private set;
        }
        
    	/// <summary>
        /// SeaShell color (R:255,G:245,B:238,A:255).
        /// </summary>
        public static Color SeaShell
        {
            get;
            private set;
        }
        
    	/// <summary>
        /// Sienna color (R:160,G:82,B:45,A:255).
        /// </summary>
        public static Color Sienna
        {
            get;
            private set;
        }
        
    	/// <summary>
        /// Silver color (R:192,G:192,B:192,A:255).
        /// </summary>
        public static Color Silver
        {
            get;
            private set;
        }
        
       /// <summary>
       /// SkyBlue color (R:135,G:206,B:235,A:255).
       /// </summary>
       public static Color SkyBlue
        {
            get;
            private set;
        }
       
        /// <summary>
        /// SlateBlue color (R:106,G:90,B:205,A:255).
        /// </summary>
        public static Color SlateBlue
       {
           get;
           private set;
       }
      
        /// <summary>
        /// SlateGray color (R:112,G:128,B:144,A:255).
        /// </summary>
        public static Color SlateGray
        {
            get;
            private set;
        }
      
        /// <summary>
        /// Snow color (R:255,G:250,B:250,A:255).
        /// </summary>
        public static Color Snow
        {
            get;
            private set;
        }
      
        /// <summary>
        /// SpringGreen color (R:0,G:255,B:127,A:255).
        /// </summary>
        public static Color SpringGreen
        {
            get;
            private set;
        }
      
        /// <summary>
        /// SteelBlue color (R:70,G:130,B:180,A:255).
        /// </summary>
        public static Color SteelBlue
        {
            get;
            private set;
        }
      
        /// <summary>
        /// Tan color (R:210,G:180,B:140,A:255).
        /// </summary>
        public static Color Tan
        {
            get;
            private set;
        }
       
        /// <summary>
        /// Teal color (R:0,G:128,B:128,A:255).
        /// </summary>
        public static Color Teal
        {
            get;
            private set;
        }
       
        /// <summary>
        /// Thistle color (R:216,G:191,B:216,A:255).
        /// </summary>
        public static Color Thistle
        {
            get;
            private set;
        }
       
        /// <summary>
        /// Tomato color (R:255,G:99,B:71,A:255).
        /// </summary>
        public static Color Tomato
        {
            get;
            private set;
        }
        
    	/// <summary>
        /// Turquoise color (R:64,G:224,B:208,A:255).
        /// </summary>
        public static Color Turquoise
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Violet color (R:238,G:130,B:238,A:255).
        /// </summary>
        public static Color Violet
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Wheat color (R:245,G:222,B:179,A:255).
        /// </summary>
	public static Color Wheat
        {
            get;
            private set;
        }
	
        /// <summary>
        /// White color (R:255,G:255,B:255,A:255).
        /// </summary>
        public static Color White
    {
        get;
        private set;
    }
       
        /// <summary>
        /// WhiteSmoke color (R:245,G:245,B:245,A:255).
        /// </summary>
        public static Color WhiteSmoke
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Yellow color (R:255,G:255,B:0,A:255).
        /// </summary>
        public static Color Yellow
        {
            get;
            private set;
        }
        
        /// <summary>
        /// YellowGreen color (R:154,G:205,B:50,A:255).
        /// </summary>
        public static Color YellowGreen
        {
            get;
            private set;
        }
        #endregion

        /// <summary>
        /// Performs linear interpolation of <see cref="Color"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="Color"/>.</param>
        /// <param name="value2">Destination <see cref="Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        public static Color Lerp(Color value1, Color value2, Single amount)
        {
			amount = MathHelper.Clamp(amount, 0, 1);
            return new Color(   
                (int)MathHelper.Lerp(value1.R, value2.R, amount),
                (int)MathHelper.Lerp(value1.G, value2.G, amount),
                (int)MathHelper.Lerp(value1.B, value2.B, amount),
                (int)MathHelper.Lerp(value1.A, value2.A, amount) );
        }

        /// <summary>
        /// <see cref="Color.Lerp"/> should be used instead of this function.
        /// </summary>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        [Obsolete("Color.Lerp should be used instead of this function.")]
        public static Color LerpPrecise(Color value1, Color value2, Single amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            return new Color(
                (int)MathHelper.LerpPrecise(value1.R, value2.R, amount),
                (int)MathHelper.LerpPrecise(value1.G, value2.G, amount),
                (int)MathHelper.LerpPrecise(value1.B, value2.B, amount),
                (int)MathHelper.LerpPrecise(value1.A, value2.A, amount));
        }
		
	/// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
	public static Color Multiply(Color value, float scale)
	{
	    return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
	}
	
	/// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
	public static Color operator *(Color value, float scale)
        {
            return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Vector3"/> representation for this object.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(R / 255.0f, G / 255.0f, B / 255.0f);
        }

        /// <summary>
        /// Gets a <see cref="Vector4"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Vector4"/> representation for this object.</returns>
        public Vector4 ToVector4()
        {
            return new Vector4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }
	
        /// <summary>
        /// Gets or sets packed value of this <see cref="Color"/>.
        /// </summary>
        [CLSCompliant(false)]
        public UInt32 PackedValue
        {
            get { return _packedValue; }
            set { _packedValue = value; }
        }


        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    this.R.ToString(), "  ",
                    this.G.ToString(), "  ",
                    this.B.ToString(), "  ",
                    this.A.ToString()
                );
            }
        }


        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="Color"/> in the format:
        /// {R:[red] G:[green] B:[blue] A:[alpha]}
        /// </summary>
        /// <returns><see cref="String"/> representation of this <see cref="Color"/>.</returns>
	public override string ToString ()
	{
        StringBuilder sb = new StringBuilder(25);
        sb.Append("{R:");
        sb.Append(R);
        sb.Append(" G:");
        sb.Append(G);
        sb.Append(" B:");
        sb.Append(B);
        sb.Append(" A:");
        sb.Append(A);
        sb.Append("}");
        return sb.ToString();
	}
	
	/// <summary>
        /// Translate a non-premultipled alpha <see cref="Color"/> to a <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="vector">A <see cref="Vector4"/> representing color.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(Vector4 vector)
        {
            return new Color(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);
        }
	
	/// <summary>
        /// Translate a non-premultipled alpha <see cref="Color"/> to a <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color(r * a / 255, g * a / 255, b * a / 255, a);
        }

        #region IEquatable<Color> Members
	
	/// <summary>
        /// Compares whether current instance is equal to specified <see cref="Color"/>.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Color other)
        {
	    return this.PackedValue == other.PackedValue;
        }

        #endregion
    }
}
