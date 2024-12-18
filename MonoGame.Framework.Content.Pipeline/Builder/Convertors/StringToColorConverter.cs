using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Microsoft.Xna.Framework.Content.Pipeline.Builder.Convertors
{
    /// <summary>
    /// Class that provides methods to convert a <see cref="Color"/> to and from a string.
    /// </summary>
	public class StringToColorConverter : TypeConverter
	{
        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof (string))
                return base.ConvertTo(context, culture, value, destinationType);

            var color = (Color)value;
            return string.Format("{0},{1},{2},{3}", color.R, color.G, color.B, color.A);
        }

        /// <inheritdoc/>
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof (string))
				return true;

			return base.CanConvertFrom (context, sourceType);
		}

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string strValue = (string)value;

                // Check if the string is in the older XNA "{R:0 G:0 B:0 A:0}" format
                if (strValue.StartsWith('{') && strValue.EndsWith('}'))
                {
                    strValue = strValue.Trim(new char[] { '{', '}' });
                    var parts = strValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        var r = int.Parse(parts[0].Split(':')[1]);
                        var g = int.Parse(parts[1].Split(':')[1]);
                        var b = int.Parse(parts[2].Split(':')[1]);
                        var a = int.Parse(parts[3].Split(':')[1]);
                        return new Microsoft.Xna.Framework.Color(r, g, b, a);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Could not convert from string({0}) to Color, expected format is 'r,g,b,a' or '{{R:0 G:0 B:0 A:0}}'", value));
                    }
                }
                else // Assume the string is in the MonoGame "r,g,b,a" format
                {
                    string[] values = (strValue).Split(new char[] { ',' }, StringSplitOptions.None);
                    if (values.Length == 4)
                    {
                        var r = int.Parse(values[0].Trim());
                        var g = int.Parse(values[1].Trim());
                        var b = int.Parse(values[2].Trim());
                        var a = int.Parse(values[3].Trim());
                        return new Microsoft.Xna.Framework.Color(r, g, b, a);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Could not convert from string({0}) to Color, expected format is 'r,g,b,a' or '{{R:0 G:0 B:0 A:0}}'", value));
                    }
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

    }
}
