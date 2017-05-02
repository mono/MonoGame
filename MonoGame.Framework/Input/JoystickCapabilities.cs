﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Describes joystick capabilities.
    /// </summary>
    public struct JoystickCapabilities
    {
        /// <summary>
        /// Gets a value indicating whether the joystick is connected.
        /// </summary>
        /// <value><c>true</c> if the joystick is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected { get; internal set; }

        /// <summary>
        /// Gets the unique identifier of the joystick.
        /// </summary>
        /// <value>String representing the unique identifier of the joystick.</value>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the axis count.
        /// </summary>
        /// <value>The number of axes that the joystick possesses.</value>
        public int AxisCount { get; internal set; }

        /// <summary>
        /// Gets the button count.
        /// </summary>
        /// <value>The number of buttons that the joystick possesses.</value>
        public int ButtonCount { get; internal set; }

        /// <summary>
        /// Gets the hat count.
        /// </summary>
        /// <value>The number of hats/dpads that the joystick possesses.</value>
        public int HatCount { get; internal set; }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Microsoft.Xna.Framework.Input.JoystickCapabilities"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Microsoft.Xna.Framework.Input.JoystickCapabilities"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Microsoft.Xna.Framework.Input.JoystickCapabilities"/>.</returns>
        public override string ToString()
        {
            return "[JoystickCapabilities: IsConnected=" + IsConnected + ", Id=" + Id + ", AxisCount=" + AxisCount + ", ButtonCount=" + ButtonCount + ", HatCount=" + HatCount + "]";
        }
    }
}

