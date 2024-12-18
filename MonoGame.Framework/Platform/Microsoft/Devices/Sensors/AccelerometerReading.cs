// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Stores values from an accelerometer event
    /// </summary>
	public struct AccelerometerReading : ISensorReading
	{
        /// <summary>
        /// Acceleration vector
        /// </summary>
		public Vector3 Acceleration { get; internal set; }
        /// <summary>
        /// Timestamp of the accelerometer reading
        /// </summary>
		public DateTimeOffset Timestamp { get; internal set; }
	}
}

