// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Common interface used for sensor readings.
    /// </summary>
	public interface ISensorReading
	{
        /// <summary>
        /// Timestamp of the sensor reading.
        /// </summary>
		DateTimeOffset Timestamp { get; }
	}
}

