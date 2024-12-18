// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Provides properties for compass readings.
    /// </summary>
    public struct CompassReading : ISensorReading
    {
        /// <summary>
        /// Heading accuracy.
        /// </summary>
        public double HeadingAccuracy { get; internal set; }
        /// <summary>
        /// Magnetic heading.
        /// </summary>
        public double MagneticHeading { get; internal set; }
        /// <summary>
        /// Magnetometer reading.
        /// </summary>
        public Vector3 MagnetometerReading { get; internal set; }
        /// <inheritdoc/>
        public DateTimeOffset Timestamp { get; internal set; }
        /// <summary>
        /// True heading.
        /// </summary>
        public double TrueHeading { get; internal set; }
    }
}
