using System;

using Microsoft.Xna.Framework;

using MonoTouch.CoreMotion;
using MonoTouch.Foundation;

namespace Microsoft.Devices.Sensors
{
    public sealed class Compass : SensorBase<CompassReading>
    {
        private static CMMotionManager motionManager = new CMMotionManager();
        private static bool started = false;
        private static SensorState state = IsSupported ? SensorState.Initializing : SensorState.NotSupported;
        private bool calibrate = false;

        public event EventHandler<CalibrationEventArgs> Calibrate;

        public static bool IsSupported
        {
            get { return motionManager.DeviceMotionAvailable; }
        }
        public SensorState State
        {
            get { return state; }
        }

        private static event CMDeviceMotionHandler readingChanged;

        public Compass()
        {
            if (!IsSupported)
                throw new SensorFailedException("Compass not supported");

            this.TimeBetweenUpdatesChanged += this.UpdateInterval;
            readingChanged += ReadingChangedHandler;
        }

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
                throw new SensorFailedException("Compass already started");
        }

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
                        this.Calibrate(this, new CalibrationEventArgs());
                    this.calibrate = true;
                }
                else if (this.calibrate == true)
                    this.calibrate = false;

                reading.Timestamp = DateTime.Now;
                this.CurrentValue = reading;
            }
            FireOnCurrentValueChanged(this, new SensorReadingEventArgs<CompassReading>(reading));
        }

        private void UpdateInterval(object sender, EventArgs args)
        {
            motionManager.MagnetometerUpdateInterval = this.TimeBetweenUpdates.TotalSeconds;
        }
    }
}

