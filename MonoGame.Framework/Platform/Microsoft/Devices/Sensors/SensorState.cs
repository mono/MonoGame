// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Defines the state of the sensor.
    /// </summary>
	public enum SensorState
	{
        /// <summary>
        /// Sensor is not supported.
        /// </summary>
		NotSupported,
        /// <summary>
        /// Sensor is ready to be used.
        /// </summary>
		Ready,
        /// <summary>
        /// Sensor is initializing.
        /// </summary>
		Initializing,
        /// <summary>
        /// Sensor has no data.
        /// </summary>
		NoData,
        /// <summary>
        /// No permissions to use the sensor.
        /// </summary>
		NoPermissions,
        /// <summary>
        /// Sensor is disabled.
        /// </summary>
		Disabled
	}
}

