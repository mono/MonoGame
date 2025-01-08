// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Input
{
    internal static class KeysHelper
    {
        static HashSet<int> _map;

        static KeysHelper()
        {
            _map = new HashSet<int>();
#if NET45
            var allKeys = (Keys[])Enum.GetValues(typeof(Keys));
#else
            var allKeys = Enum.GetValues<Keys>();
#endif
            foreach (var key in allKeys)
            {
                _map.Add((int)key);
            }
        }

        /// <summary>
        /// Checks if specified value is valid Key.
        /// </summary>
        /// <param name="value">Keys base value</param>
        /// <returns>Returns true if value is valid Key, false otherwise</returns>
        public static bool IsKey(int value)
        {
            return _map.Contains(value);
        }
    }
}
