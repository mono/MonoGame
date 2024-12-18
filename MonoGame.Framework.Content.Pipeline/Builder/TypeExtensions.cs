// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    /// <summary>
    /// Provides extension methods for common types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Takes a <see cref="System.Drawing.Color">System.Drawing.Color</see> and converts it to a <see cref="Color">Xna.Framework.Color</see>.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color.</returns>
        public static Color ToColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Takes a <see cref="System.Drawing.Color">System.Drawing.Color</see> and converts it to a <see cref="Vector3">Vector3</see>.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The resulting <see cref="Vector3"/>.</returns>
        public static Vector3 ToVector3(this System.Drawing.Color color)
        {
            return new Vector3(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
        }

        /// <summary>
        /// Adds an item to the list if it is not already present.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="list">The list to add the item to.</param>
        /// <param name="item">The item to add to the list.</param>
        public static void AddUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        /// <summary>
        /// Adds a range of items to the list if they are not already present.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="dstList">The list to add the items to.</param>
        /// <param name="list">The list of items to add.</param>
        public static void AddRangeUnique<T>(this List<T> dstList, List<T> list)
        {
            foreach (var i in list)
            {
                if (!dstList.Contains(i))
                    dstList.Add(i);
            }
        }
    }
}
