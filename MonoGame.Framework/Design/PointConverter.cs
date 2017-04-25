// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design
{
    public class PointConverter : MathTypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(int))
                return true;
            if (destinationType == typeof(Point))
                return true;
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var point = (Point)value;


            if (destinationType == typeof(int))
            {
                return point.X;
            }

            if (destinationType == typeof(Point))
            {
                return point;
            }



            if (destinationType == typeof(string))
            {
                var terms = new string[2];
                terms[0] = point.X.ToString(culture);
                terms[1] = point.Y.ToString(culture);

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
            var point = Point.Zero;

            if (sourceType == typeof(string))
            {
                int[] parts = new int[2];

                StringToList((string)value, culture, ref parts);

                point.X = parts[0];
                point.Y = parts[1];

                return point;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
