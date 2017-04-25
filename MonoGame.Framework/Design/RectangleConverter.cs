// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design
{
    public class RectangleConverter : MathTypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Rectangle))
                return true;
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var rectangle = (Rectangle)value;


            if (destinationType == typeof(Point))
            {
                return rectangle;
            }


            if (destinationType == typeof(string))
            {
                var terms = new string[4];
                terms[0] = rectangle.X.ToString(culture);
                terms[1] = rectangle.Y.ToString(culture);
                terms[2] = rectangle.Width.ToString(culture);
                terms[3] = rectangle.Height.ToString(culture);


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
            var rectangle = Rectangle.Empty;

            if (sourceType == typeof(string))
            {
                int[] parts = new int[4];

                StringToList((string)value, culture, ref parts);

                rectangle.X = parts[0];
                rectangle.Y = parts[1];
                rectangle.Width = parts[2];
                rectangle.Height = parts[3];

                return rectangle;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
