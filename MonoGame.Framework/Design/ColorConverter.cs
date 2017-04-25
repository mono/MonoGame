// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design
{
    public class ColorConverter : MathTypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Color))
                return true;
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var colour = (Color)value;

            if (destinationType == typeof(Color))
            {                
                return colour;
            }

            if (destinationType == typeof(string))
            {
                var terms = new string[4];
                terms[0] = colour.R.ToString(culture);
                terms[1] = colour.G.ToString(culture);
                terms[2] = colour.B.ToString(culture);
                terms[3] = colour.A.ToString(culture);

                return string.Join(culture.TextInfo.ListSeparator + " ", terms);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value.GetType();
            var color = Color.Transparent;

            if (sourceType == typeof(string))
            {
                byte[] parts = new byte[4];

                StringToList((string)value, culture, ref parts);

                color.R = parts[0];
                color.G = parts[1];
                color.B = parts[2];
                color.A = parts[3];
                
                return color;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
