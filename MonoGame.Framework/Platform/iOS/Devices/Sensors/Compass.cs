using System;

using Microsoft.Xna.Framework;

using CoreMotion;
using Foundation;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Class to provide methods and properties to read device compass data.
    /// </summary>
    public sealed class Compass : SensorBase<CompassReading>
    {
        static readonly int MaxSensorCount = 10;
        static int instanceCount;
        private static bool started = false;
        private static SensorState state = IsSupported ? SensorState.Initializing : SensorState.NotSupported;
        private bool calibrate = false;

        /// <summary>
        /// Event that is triggered when the user calibrates the compass
        /// </summary>
        public event EventHandler<CalibrationEventArgs> Calibrate;

        /// <summary>
        /// Gets a value indicating whether the compass is supported on the current device.
        /// </summary>
        public static bool IsSupported
        {
            get { return motionManager.DeviceMotionAvailable; }
        }

        /// <summary>
        /// Gets the current state of the compass.
        /// </summary>
        public SensorState State
        {
            get { return state; }
        }

        private static event CMDeviceMotionHandler readingChanged;

        /// <summary>
        /// Creates a new instance of the Compass class.
        /// </summary>
        /// <exception cref="SensorFailedException">Thrown if the compass is not supported or limit of instances has been exceeded (10).</exception>
        public Compass()
        {
            if (!IsSupported)
                throw new SensorFailedException("Failed to start compass data acquisition. No default sensor found.");
            else if (instanceCount >= MaxSensorCount)
                throw new SensorFailedException("The limit of 10 simultaneous instances of the Compass class per application has been exceeded.");

            ++instanceCount;

            this.TimeBetweenUpdatesChanged += this.UpdateInterval;
            readingChanged += ReadingChangedHandler;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        protected override void Dispose (bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (started)
                        Stop();
                    --instanceCount;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Begins data acquisition from the compass sensor.
        /// </summary>
        /// <exception cref="SensorFailedException">Thrown if the sensor is already started.</exception>
        public override void Start()
        {
            if (started == false)
            {
                // For true north use CMAttitudeReferenceFrame.XTrueNorthZVertical, but be aware that it requires location service
                motionManager.StartDeviceMotionUpdates(CMAttitudeReferenceFrame.XMagneticNorthZVertical, NSOperationQueue.CurrentQueue, MagnetometerHandler);
                started = true;
                state = SensorState.Ready;
            }
            else
                throw new SensorFailedException("Failed to start compass data acquisition. Data acquisition already started.");
        }

        /// <summary>
        /// Ends the data acquisition from the compass sensor.
        /// </summary>
        public override void Stop()
        {
            motionManager.StopDeviceMotionUpdates();
            started = false;
            state = SensorState.Disabled;
        }

        private void MagnetometerHandler(CMDeviceMotion magnetometerData, NSError error)
        {
            readingChanged(magnetometerData, error);
        }

        private void ReadingChangedHandler(CMDeviceMotion data, NSError error)
        {
            CompassReading reading = new CompassReading();
            this.IsDataValid = error == null;
            if (this.IsDataValid)
            {
                reading.MagnetometerReading = new Vector3((float)data.MagneticField.Field.Y, (float)-data.MagneticField.Field.X, (float)data.MagneticField.Field.Z);
                reading.TrueHeading = Math.Atan2(reading.MagnetometerReading.Y, reading.MagnetometerReading.X) / Math.PI * 180;
                reading.MagneticHeading = reading.TrueHeading;
                switch (data.MagneticField.Accuracy)
                {
                    case CMMagneticFieldCalibrationAccuracy.High:
                        reading.HeadingAccuracy = 5d;
                        break;
                    case CMMagneticFieldCalibrationAccuracy.Medium:
                        reading.HeadingAccuracy = 30d;
                        break;
                    case CMMagneticFieldCalibrationAccuracy.Low:
                        reading.HeadingAccuracy = 45d;
                        break;
                }

                // Send calibrate event if needed
                if (data.MagneticField.Accuracy == CMMagneticFieldCalibrationAccuracy.Uncalibrated)
                {
                    if (this.calibrate == false)
                        EventHelpers.Raise(this, Calibrate, new CalibrationEventArgs());
                    this.calibrate = true;
                }
                else if (this.calibrate == true)
                    this.calibrate = false;

                reading.Timestamp = DateTime.UtcNow;
                this.CurrentValue = reading;
            }
        }

        private void UpdateInterval(object sender, EventArgs args)
        {
            motionManager.MagnetometerUpdateInterval = this.TimeBetweenUpdates.TotalSeconds;
        }
    }
}

