// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Represents errors related to sensors.
    /// </summary>
	public class SensorFailedException : Exception
	{
        /// <summary>
        /// Error Id.
        /// </summary>
		public int ErrorId { get; protected set; }

        internal SensorFailedException(string message)
            : base(message)
        {
        }
    }
}

