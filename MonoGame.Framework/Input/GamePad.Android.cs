// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Android.Views;

namespace Microsoft.Xna.Framework.Input
{
    internal class AndroidGamePad
    {
        public InputDevice _device;
        public int _deviceId;
        public string _descriptor;
        public bool _isConnected;
        public bool DPadButtons;

        public Buttons _buttons;
        public float _leftTrigger, _rightTrigger;
        public Vector2 _leftStick, _rightStick;

        public readonly GamePadCapabilities _capabilities;

        public bool _hasCustomMapping = false;

        Axis _leftStickXCustomAxis = Axis.X;
        Axis _leftStickYCustomAxis = Axis.Y;
        Axis _rightStickXCustomAxis = Axis.Z;
        Axis _rightStickYCustomAxis = Axis.Rz;

        Axis _leftTriggerCustomAxis = Axis.Ltrigger;
        Axis _rightTriggerCustomAxis = Axis.Rtrigger;

        Axis _dPadXCustomAxis = Axis.HatX;
        Axis _dPadYCustomAxis = Axis.HatY;

        public AndroidGamePad(InputDevice device)
        {
            _device = device;
            _deviceId = device.Id;
            _descriptor = device.Descriptor;
            _isConnected = true;

            if (device.Name == "Xbox Wireless Controller")
            {
                _hasCustomMapping = true;
                _leftTriggerCustomAxis = Axis.Brake;
                _rightTriggerCustomAxis = Axis.Gas;
            }
            _capabilities = CapabilitiesOfDevice(device);
        }

        private static GamePadCapabilities CapabilitiesOfDevice(InputDevice device)
        {
            var capabilities = new GamePadCapabilities();
            capabilities.IsConnected = true;
            capabilities.GamePadType = GamePadType.GamePad;
            capabilities.HasLeftVibrationMotor = capabilities.HasRightVibrationMotor = device.Vibrator.HasVibrator;

            // build out supported inputs from what the gamepad exposes
            int[] keyMap = new int[16];
            keyMap[0] = (int)Keycode.ButtonA;
            keyMap[1] = (int)Keycode.ButtonB;
            keyMap[2] = (int)Keycode.ButtonX;
            keyMap[3] = (int)Keycode.ButtonY;

            keyMap[4] = (int)Keycode.ButtonThumbl;
            keyMap[5] = (int)Keycode.ButtonThumbr;

            keyMap[6] = (int)Keycode.ButtonL1;
            keyMap[7] = (int)Keycode.ButtonR1;
            keyMap[8] = (int)Keycode.ButtonL2;
            keyMap[9] = (int)Keycode.ButtonR2;

            keyMap[10] = (int)Keycode.DpadDown;
            keyMap[11] = (int)Keycode.DpadLeft;
            keyMap[12] = (int)Keycode.DpadRight;
            keyMap[13] = (int)Keycode.DpadUp;

            keyMap[14] = (int)Keycode.ButtonStart;
            keyMap[15] = (int)Keycode.Back;

            bool[] hasMap = new bool[16];
            // get a bool[] with indices matching the keyMap
            hasMap = device.HasKeys(keyMap);

            capabilities.HasAButton = hasMap[0];
            capabilities.HasBButton = hasMap[1];
            capabilities.HasXButton = hasMap[2];
            capabilities.HasYButton = hasMap[3];

            // we only check for the thumb button to see if we have 2 thumbsticks
            // if ever a controller doesn't support buttons on the thumbsticks,
            // this will need fixing
            capabilities.HasLeftXThumbStick = hasMap[4];
            capabilities.HasLeftYThumbStick = hasMap[4];
            capabilities.HasRightXThumbStick = hasMap[5];
            capabilities.HasRightYThumbStick = hasMap[5];

            capabilities.HasLeftShoulderButton = hasMap[6];
            capabilities.HasRightShoulderButton = hasMap[7];
            capabilities.HasLeftTrigger = hasMap[8];
            capabilities.HasRightTrigger = hasMap[9];

            capabilities.HasDPadDownButton = hasMap[10];
            capabilities.HasDPadLeftButton = hasMap[11];
            capabilities.HasDPadRightButton = hasMap[12];
            capabilities.HasDPadUpButton = hasMap[13];

            capabilities.HasStartButton = hasMap[14];
            capabilities.HasBackButton = hasMap[15];

            return capabilities;
        }

        internal void ProcessMotionEvent(MotionEvent e)
        {
            _leftStick = new Vector2(e.GetAxisValue(_leftStickXCustomAxis), -e.GetAxisValue(_leftStickYCustomAxis));
            _rightStick = new Vector2(e.GetAxisValue(_rightStickXCustomAxis), -e.GetAxisValue(_rightStickYCustomAxis));

            _leftTrigger = e.GetAxisValue(_leftTriggerCustomAxis);
            _rightTrigger = e.GetAxisValue(_rightTriggerCustomAxis);

            if (!DPadButtons)
            {
                if (e.GetAxisValue(_dPadXCustomAxis) < 0)
                {
                    _buttons |= Buttons.DPadLeft;
                    _buttons &= ~Buttons.DPadRight;
                }
                else if (e.GetAxisValue(_dPadXCustomAxis) > 0)
                {
                    _buttons &= ~Buttons.DPadLeft;
                    _buttons |= Buttons.DPadRight;
                }
                else
                {
                    _buttons &= ~Buttons.DPadLeft;
                    _buttons &= ~Buttons.DPadRight;
                }

                if (e.GetAxisValue(_dPadYCustomAxis) < 0)
                {
                    _buttons |= Buttons.DPadUp;
                    _buttons &= ~Buttons.DPadDown;
                }
                else if (e.GetAxisValue(_dPadYCustomAxis) > 0)
                {
                    _buttons &= ~Buttons.DPadUp;
                    _buttons |= Buttons.DPadDown;
                }
                else
                {
                    _buttons &= ~Buttons.DPadUp;
                    _buttons &= ~Buttons.DPadDown;
                }
            }
        }
    }


    static partial class GamePad
    {
        // we will support up to 4 local controllers
        private static readonly AndroidGamePad[] GamePads = new AndroidGamePad[4];
        // support the back button when we don't have a gamepad connected
        internal static bool Back;

        private static int PlatformGetMaxNumberOfGamePads()
        {
            return 4;
        }

        private static GamePadCapabilities PlatformGetCapabilities(int index)
        {
            var gamePad = GamePads[index];
            if (gamePad != null)
                return gamePad._capabilities;

            // we need to add the default "no gamepad connected but the user hit back"
            // behaviour here
            GamePadCapabilities capabilities = new GamePadCapabilities();
            capabilities.IsConnected = false;
            capabilities.HasBackButton = true;

            return capabilities;
        }

        private static GamePadState PlatformGetState(int index, GamePadDeadZone leftDeadZoneMode, GamePadDeadZone rightDeadZoneMode)
        {
            var gamePad = GamePads[index];
            GamePadState state = GamePadState.Default;
            if (gamePad != null && gamePad._isConnected)
            {
                // Check if the device was disconnected
                var dvc = InputDevice.GetDevice(gamePad._deviceId);
                if (dvc == null)
                {
                    Android.Util.Log.Debug("MonoGame", "Detected controller disconnect [" + index + "] ");
                    gamePad._isConnected = false;
                    return state;
                }

                GamePadThumbSticks thumbSticks = new GamePadThumbSticks(gamePad._leftStick, gamePad._rightStick, leftDeadZoneMode, rightDeadZoneMode);

                state = new GamePadState(
                    thumbSticks,
                    new GamePadTriggers(gamePad._leftTrigger, gamePad._rightTrigger),
                    new GamePadButtons(gamePad._buttons),
                    new GamePadDPad(gamePad._buttons));
            }
            // we need to add the default "no gamepad connected but the user hit back"
            // behaviour here
            else {
                if (index == 0 && Back)
                {
                    // Consume state
                    Back = false;
                    state = new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.Back), new GamePadDPad());
                    state.IsConnected = false;
                }
                else
                    state = new GamePadState();
            }

            return state;
        }

        private static bool PlatformSetVibration(int index, float leftMotor, float rightMotor)
        {
            var gamePad = GamePads[index];
            if (gamePad == null)
                return false;

            var vibrator = gamePad._device.Vibrator;
            if (!vibrator.HasVibrator)
                return false;
            vibrator.Vibrate(500);
            return true;
        }

        internal static AndroidGamePad GetGamePad(InputDevice device)
        {
            if (device == null || (device.Sources & InputSourceType.Gamepad) != InputSourceType.Gamepad)
                return null;

            int firstDisconnectedPadId = -1;
            for (int i = 0; i < GamePads.Length; i++)
            {
                var pad = GamePads[i];
                if (pad != null && pad._isConnected && pad._deviceId == device.Id)
                {
                    return pad;
                }
                else if (pad != null && !pad._isConnected && pad._descriptor == device.Descriptor)
                {
                    Android.Util.Log.Debug("MonoGame", "Found previous controller [" + i + "] " + device.Name);
                    pad._deviceId = device.Id;
                    pad._isConnected = true;
                    return pad;
                }
                else if (pad == null)
                {
                    Android.Util.Log.Debug("MonoGame", "Found new controller [" + i + "] " + device.Name);
                    pad = new AndroidGamePad(device);
                    GamePads[i] = pad;
                    return pad;
                }
                else if (!pad._isConnected && firstDisconnectedPadId < 0)
                {
                    firstDisconnectedPadId = i;
                }
            }

            // If we get here, we failed to find a game pad or an empty slot to create one.
            // If we're holding onto a disconnected pad, overwrite it with this one
            if (firstDisconnectedPadId >= 0)
            {
                Android.Util.Log.Debug("MonoGame", "Found new controller in place of disconnected controller [" + firstDisconnectedPadId + "] " + device.Name);
                var pad = new AndroidGamePad(device);
                GamePads[firstDisconnectedPadId] = pad;
                return pad;
            }

            // All pad slots are taken so ignore further devices.
            return null;
        }

        internal static bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            var gamePad = GetGamePad(e.Device);
            if (gamePad == null)
                return false;

            gamePad.DPadButtons |= e.KeyCode == Keycode.DpadLeft ||
                                   e.KeyCode == Keycode.DpadUp || 
                                   e.KeyCode == Keycode.DpadRight || 
                                   e.KeyCode == Keycode.DpadDown;
            gamePad._buttons |= ButtonForKeyCode(keyCode);

            return true;
        }

        internal static bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            var gamePad = GetGamePad(e.Device);
            if (gamePad == null)
                return false;

            gamePad._buttons &= ~ButtonForKeyCode(keyCode);
            return true;
        }

        internal static bool OnGenericMotionEvent(MotionEvent e)
        {
            var gamePad = GetGamePad(e.Device);
            if (gamePad == null)
                return false;

            if (e.Action != MotionEventActions.Move)
                return false;

            if (gamePad._hasCustomMapping)
            {
                gamePad.ProcessMotionEvent(e);
                return true;
            }

            gamePad._leftStick = new Vector2(e.GetAxisValue(Axis.X), -e.GetAxisValue(Axis.Y));
            gamePad._rightStick = new Vector2(e.GetAxisValue(Axis.Z), -e.GetAxisValue(Axis.Rz));

            gamePad._leftTrigger = e.GetAxisValue(Axis.Ltrigger);
            gamePad._rightTrigger = e.GetAxisValue(Axis.Rtrigger);

            if (!gamePad.DPadButtons)
            {
                if (e.GetAxisValue(Axis.HatX) < 0)
                {
                    gamePad._buttons |= Buttons.DPadLeft;
                    gamePad._buttons &= ~Buttons.DPadRight;
                }
                else if (e.GetAxisValue(Axis.HatX) > 0)
                {
                    gamePad._buttons &= ~Buttons.DPadLeft;
                    gamePad._buttons |= Buttons.DPadRight;
                }
                else
                {
                    gamePad._buttons &= ~Buttons.DPadLeft;
                    gamePad._buttons &= ~Buttons.DPadRight;
                }

                if (e.GetAxisValue(Axis.HatY) < 0)
                {
                    gamePad._buttons |= Buttons.DPadUp;
                    gamePad._buttons &= ~Buttons.DPadDown;
                }
                else if (e.GetAxisValue(Axis.HatY) > 0)
                {
                    gamePad._buttons &= ~Buttons.DPadUp;
                    gamePad._buttons |= Buttons.DPadDown;
                }
                else
                {
                    gamePad._buttons &= ~Buttons.DPadUp;
                    gamePad._buttons &= ~Buttons.DPadDown;
                }
            }

            return true;
        }

        private static Buttons ButtonForKeyCode(Keycode keyCode)
        {
            switch (keyCode)
            {
                case Keycode.ButtonA:
                    return Buttons.A;
                case Keycode.ButtonX:
                    return Buttons.X;
                case Keycode.ButtonY:
                    return Buttons.Y;
                case Keycode.ButtonB:
                    return Buttons.B;

                case Keycode.ButtonL1:
                    return Buttons.LeftShoulder;
                case Keycode.ButtonL2:
                    return Buttons.LeftTrigger;
                case Keycode.ButtonR1:
                    return Buttons.RightShoulder;
                case Keycode.ButtonR2:
                    return Buttons.RightTrigger;

                case Keycode.ButtonThumbl:
                    return Buttons.LeftStick;
                case Keycode.ButtonThumbr:
                    return Buttons.RightStick;

                case Keycode.DpadUp:
                    return Buttons.DPadUp;
                case Keycode.DpadDown:
                    return Buttons.DPadDown;
                case Keycode.DpadLeft:
                    return Buttons.DPadLeft;
                case Keycode.DpadRight:
                    return Buttons.DPadRight;

                case Keycode.ButtonStart:
                    return Buttons.Start;
                case Keycode.Back:
                    return Buttons.Back;
            }

            return 0;
        }

        internal static void Initialize()
        {
            //Iterate and 'connect' any detected gamepads
            foreach (var deviceId in InputDevice.GetDeviceIds())
            {
                GetGamePad(InputDevice.GetDevice(deviceId));
            }
        }
    }
}
