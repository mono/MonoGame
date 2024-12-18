// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Provides data for sensor events.
    /// </summary>
    /// <typeparam name="T">The type of the sensor.</typeparam>
	public class SensorReadingEventArgs<T> : EventArgs
		where T : ISensorReading
	{
        /// <summary>
        /// Sensor reader type.
        /// </summary>
		public T SensorReading { get; set; }

        /// <summary>
        /// Creates a new instance of the SensorReadingEventArgs class.
        /// </summary>
        /// <param name="sensorReading">Sensor reader.</param>
		public SensorReadingEventArgs(T sensorReading)
		{
			this.SensorReading = sensorReading;
		}
	}
}

