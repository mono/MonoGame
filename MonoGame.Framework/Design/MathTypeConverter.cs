// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design
{
    public class MathTypeConverter : TypeConverter
    {
        protected void StringToList(string str, CultureInfo culture, ref float[] list)
        {
            var words = str.Split(culture.TextInfo.ListSeparator.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);

            if(words.Length != list.Length)
            {
                throw new ArgumentOutOfRangeException("Expected " + list.Length + " but got " + words.Length);
            }
            else
            {
                for(int i=0;i<list.Length;i++)
                {
                    list[i] = float.Parse(words[i]);
                }
            }
        }


        protected void StringToList(string str, CultureInfo culture, ref int[] list)
        {
            var words = str.Split(culture.TextInfo.ListSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (words.Length != list.Length)
            {
                throw new ArgumentOutOfRangeException("Expected " + list.Length + " but got " + words.Length);
            }
            else
            {
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = int.Parse(words[i]);
                }
            }
        }

        protected void StringToList(string str, CultureInfo culture, ref byte[] list)
        {
            var words = str.Split(culture.TextInfo.ListSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (words.Length != list.Length)
            {
                throw new ArgumentOutOfRangeException("Expected " + list.Length + " but got " + words.Length);
            }
            else
            {
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = byte.Parse(words[i]);
                }
            }
        }
    }
}
