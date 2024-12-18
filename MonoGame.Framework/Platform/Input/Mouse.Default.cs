// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Input
{
    public static partial class Mouse
    {
        private static IntPtr PlatformGetWindowHandle()
        {
            return IntPtr.Zero;
        }

        private static void PlatformSetWindowHandle(IntPtr windowHandle)
        {
        }

        private static MouseState PlatformGetState(GameWindow window)
        {
            return window.MouseState;
        }

        private static void PlatformSetPosition(int x, int y)
        {
            PrimaryWindow.MouseState.X = x;
            PrimaryWindow.MouseState.Y = y;
        }

        /// <summary>
        /// Sets the <see cref="MouseCursor">MouseCursor</see>.
        /// </summary>
        /// <remarks>
        /// This method does not set a custom cursor as it is not currently supported on the IOS platform.
        /// </remarks>
        /// <param name="cursor">The <see cref="MouseCursor">MouseCursor</see> for the system to use</param>
        public static void PlatformSetCursor(MouseCursor cursor)
        {

        }
    }
}
