using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Utilities
{
    internal static partial class ReflectionHelpers
    {
        // This helper caches the Marshal.SizeOf result
        // as it generates an allocation on each call.
        [Obsolete("This should be made private and use FastSizeOf<T>() below instead!")]
        internal static class SizeOf<T>
        {
            static int _sizeOf;

            static SizeOf()
            {
                _sizeOf = Marshal.SizeOf<T>();
            }

            static public int Get()
            {
                return _sizeOf;
            }
        }
    }
}
