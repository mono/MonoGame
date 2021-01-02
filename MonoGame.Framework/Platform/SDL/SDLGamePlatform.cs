// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework
{
    internal class SdlGamePlatform : GamePlatform
    {
        private const int MouseButtonMaxTime = 1;

        private readonly Game _game;
        private readonly List<Keys> _keys;

        private int _isExiting;
        private uint _resetButtonFlags;
        private SdlGameWindow _view;
        private Dictionary<Sdl.Mouse.Button, uint> _mouseButtonTimeTracker;

        public SdlGamePlatform(Game game)
            : base(game)
        {
            _game = game;
            _keys = new List<Keys>();
            _mouseButtonTimeTracker = new Dictionary<Sdl.Mouse.Button, uint>();
            Keyboard.SetKeys(_keys);

            Sdl.Version sversion;
            Sdl.GetVersion(out sversion);

            Sdl.Major = sversion.Major;
            Sdl.Minor = sversion.Minor;
            Sdl.Patch = sversion.Patch;

            var version = 100 * Sdl.Major + 10 * Sdl.Minor + Sdl.Patch;

            if (version <= 204)
                Debug.WriteLine("Please use SDL 2.0.5 or higher.");

            // Needed so VS can debug the project on Windows
            if (version >= 205 && CurrentPlatform.OS == OS.Windows && Debugger.IsAttached)
                Sdl.SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");

            Sdl.Init((int)(
                Sdl.InitFlags.Video |
                Sdl.InitFlags.Joystick |
                Sdl.InitFlags.GameController |
                Sdl.InitFlags.Haptic
            ));

            Sdl.DisableScreenSaver();

            GamePad.InitDatabase();
            Window = _view = new SdlGameWindow(_game);
        }

        public override GameRunBehavior DefaultRunBehavior
        {
            get { return GameRunBehavior.Synchronous; }
        }

        public override void BeforeInitialize()
        {
            SdlRunLoop();

            base.BeforeInitialize();
        }

        protected override void OnIsMouseVisibleChanged()
        {
            _view.SetCursorVisible(_game.IsMouseVisible);
        }

        internal override void OnPresentationChanged(PresentationParameters pp)
        {
            var displayIndex = Sdl.Window.GetDisplayIndex(Window.Handle);
            var displayName = Sdl.Display.GetDisplayName(displayIndex);
            BeginScreenDeviceChange(pp.IsFullScreen);
            EndScreenDeviceChange(displayName, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public override void RunLoop()
        {
            Sdl.Window.Show(Window.Handle);

            while (true)
            {
                SdlRunLoop();
                Game.Tick();
                Threading.Run();
                GraphicsDevice.DisposeContexts();

                if (_isExiting > 0)
                    break;
            }
        }

        private void SdlRunLoop()
        {
            uint mouseButtonFlags = 0;

            if (_resetButtonFlags > 0)
            {
                if ((_resetButtonFlags & (uint)Sdl.Mouse.Button.Left) > 0)
                    _view.MouseState.LeftButton = ButtonState.Released;

                if ((_resetButtonFlags & (uint)Sdl.Mouse.Button.Right) > 0)
                    _view.MouseState.RightButton = ButtonState.Released;

                if ((_resetButtonFlags & (uint)Sdl.Mouse.Button.Middle) > 0)
                    _view.MouseState.MiddleButton = ButtonState.Released;

                if ((_resetButtonFlags & (uint)Sdl.Mouse.Button.X1Mask) > 0)
                    _view.MouseState.XButton1 = ButtonState.Released;

                if ((_resetButtonFlags & (uint)Sdl.Mouse.Button.X2Mask) > 0)
                    _view.MouseState.XButton2 = ButtonState.Released;

                _resetButtonFlags = 0;
            }

            Sdl.Event ev;

            while (Sdl.PollEvent(out ev) == 1)
            {
                switch (ev.Type)
                {
                    case Sdl.EventType.Quit:
                        _isExiting++;
                        break;
                    case Sdl.EventType.JoyDeviceAdded:
                        Joystick.AddDevice(ev.JoystickDevice.Which);
                        break;
                    case Sdl.EventType.JoyDeviceRemoved:
                        Joystick.RemoveDevice(ev.JoystickDevice.Which);
                        break;
                    case Sdl.EventType.ControllerDeviceRemoved:
                        GamePad.RemoveDevice(ev.ControllerDevice.Which);
                        break;
                    case Sdl.EventType.ControllerButtonUp:
                    case Sdl.EventType.ControllerButtonDown:
                    case Sdl.EventType.ControllerAxisMotion:
                        GamePad.UpdatePacketInfo(ev.ControllerDevice.Which, ev.ControllerDevice.TimeStamp);
                        break;
                    case Sdl.EventType.MouseWheel:
                        const int wheelDelta = 120;
                        _view.MouseState.ScrollWheelValue += ev.Wheel.Y * wheelDelta;
                        _view.MouseState.HorizontalScrollWheelValue += ev.Wheel.X * wheelDelta;
                        break;
                    case Sdl.EventType.MouseMotion:
                        Window.MouseState.X = ev.Motion.X;
                        Window.MouseState.Y = ev.Motion.Y;
                        break;
                    case Sdl.EventType.MouseButtonDown:
                        mouseButtonFlags |= (uint)ev.Button.Button;
                        _mouseButtonTimeTracker[ev.Button.Button] = ev.Button.TimeStamp;

                        switch (ev.Button.Button)
                        {
                            case Sdl.Mouse.Button.Left:
                                _view.MouseState.LeftButton = ButtonState.Pressed;
                                break;
                            case Sdl.Mouse.Button.Middle:
                                _view.MouseState.MiddleButton = ButtonState.Pressed;
                                break;
                            case Sdl.Mouse.Button.Right:
                                _view.MouseState.RightButton = ButtonState.Pressed;
                                break;
                            case Sdl.Mouse.Button.X1Mask:
                                _view.MouseState.XButton1 = ButtonState.Pressed;
                                break;
                            case Sdl.Mouse.Button.X2Mask:
                                _view.MouseState.XButton2 = ButtonState.Pressed;
                                break;
                        }

                        break;
                    case Sdl.EventType.MouseButtonUp:
                        if ((mouseButtonFlags & (uint)ev.Button.Button) > 0)
                        {
                            uint timeStamp = 0;

                            if (_mouseButtonTimeTracker.TryGetValue(ev.Button.Button, out timeStamp) &&
                                ev.Button.TimeStamp >= timeStamp &&
                                ev.Button.TimeStamp <= timeStamp + MouseButtonMaxTime)
                            {
                                _resetButtonFlags |= (uint)ev.Button.Button;
                                break;
                            }
                        }

                        switch (ev.Button.Button)
                        {
                            case Sdl.Mouse.Button.Left:
                                _view.MouseState.LeftButton = ButtonState.Released;
                                break;
                            case Sdl.Mouse.Button.Middle:
                                _view.MouseState.MiddleButton = ButtonState.Released;
                                break;
                            case Sdl.Mouse.Button.Right:
                                _view.MouseState.RightButton = ButtonState.Released;
                                break;
                            case Sdl.Mouse.Button.X1Mask:
                                _view.MouseState.XButton1 = ButtonState.Released;
                                break;
                            case Sdl.Mouse.Button.X2Mask:
                                _view.MouseState.XButton2 = ButtonState.Released;
                                break;
                        }

                        break;
                    case Sdl.EventType.KeyDown:
                        {
                            var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                            if (!_keys.Contains(key))
                                _keys.Add(key);
                            char character = (char)ev.Key.Keysym.Sym;
                            _view.OnKeyDown(new InputKeyEventArgs(key));
                            if (char.IsControl(character))
                                _view.OnTextInput(new TextInputEventArgs(character, key));
                            break;
                        }
                    case Sdl.EventType.KeyUp:
                        {
                            var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                            _keys.Remove(key);
                            _view.OnKeyUp(new InputKeyEventArgs(key));
                            break;
                        }
                    case Sdl.EventType.TextInput:
                        if (_view.IsTextInputHandled)
                        {
                            int len = 0;
                            int utf8character = 0; // using an int to encode multibyte characters longer than 2 bytes
                            byte currentByte = 0;
                            int charByteSize = 0; // UTF8 char lenght to decode
                            int remainingShift = 0;
                            unsafe
                            {
                                while ((currentByte = Marshal.ReadByte((IntPtr)ev.Text.Text, len)) != 0)
                                {
                                    // we're reading the first UTF8 byte, we need to check if it's multibyte
                                    if (charByteSize == 0)
                                    {
                                        if (currentByte < 192)
                                            charByteSize = 1;
                                        else if (currentByte < 224)
                                            charByteSize = 2;
                                        else if (currentByte < 240)
                                            charByteSize = 3;
                                        else
                                            charByteSize = 4;

                                        utf8character = 0;
                                        remainingShift = 4;
                                    }

                                    // assembling the character
                                    utf8character <<= 8;
                                    utf8character |= currentByte;

                                    charByteSize--;
                                    remainingShift--;

                                    if (charByteSize == 0) // finished decoding the current character
                                    {
                                        utf8character <<= remainingShift * 8; // shifting it to full UTF8 scope

                                        // SDL returns UTF8-encoded characters while C# char type is UTF16-encoded (and limited to the 0-FFFF range / does not support surrogate pairs)
                                        // so we need to convert it to Unicode codepoint and check if it's within the supported range
                                        int codepoint = UTF8ToUnicode(utf8character);

                                        if (codepoint >= 0 && codepoint < 0xFFFF)
                                        {
                                            _view.OnTextInput(new TextInputEventArgs((char)codepoint, KeyboardUtil.ToXna(codepoint)));
                                            // UTF16 characters beyond 0xFFFF are not supported (and would require a surrogate encoding that is not supported by the char type)
                                        }
                                    }

                                    len++;
                                }
                            }
                        }
                        break;
                    case Sdl.EventType.WindowEvent:

                        // If the ID is not the same as our main window ID
                        // that means that we received an event from the
                        // dummy window, so don't process the event.
                        if (ev.Window.WindowID != _view.Id)
                            break;

                        switch (ev.Window.EventID)
                        {
                            case Sdl.Window.EventId.Resized:
                            case Sdl.Window.EventId.SizeChanged:
                                _view.ClientResize(ev.Window.Data1, ev.Window.Data2);
                                break;
                            case Sdl.Window.EventId.FocusGained:
                                IsActive = true;
                                break;
                            case Sdl.Window.EventId.FocusLost:
                                IsActive = false;
                                break;
                            case Sdl.Window.EventId.Moved:
                                _view.Moved();
                                break;
                            case Sdl.Window.EventId.Close:
                                _isExiting++;
                                break;
                        }
                        break;
                }
            }
        }

        private int UTF8ToUnicode(int utf8)
        {
            int
                byte4 = utf8 & 0xFF,
                byte3 = (utf8 >> 8) & 0xFF,
                byte2 = (utf8 >> 16) & 0xFF,
                byte1 = (utf8 >> 24) & 0xFF;

            if (byte1 < 0x80)
                return byte1;
            else if (byte1 < 0xC0)
                return -1;
            else if (byte1 < 0xE0 && byte2 >= 0x80 && byte2 < 0xC0)
                return (byte1 % 0x20) * 0x40 + (byte2 % 0x40);
            else if (byte1 < 0xF0 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0)
                return (byte1 % 0x10) * 0x40 * 0x40 + (byte2 % 0x40) * 0x40 + (byte3 % 0x40);
            else if (byte1 < 0xF8 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0 && byte4 >= 0x80 && byte4 < 0xC0)
                return (byte1 % 0x8) * 0x40 * 0x40 * 0x40 + (byte2 % 0x40) * 0x40 * 0x40 + (byte3 % 0x40) * 0x40 + (byte4 % 0x40);
            else
                return -1;
        }

        public override void StartRunLoop()
        {
            throw new NotSupportedException("The desktop platform does not support asynchronous run loops");
        }

        public override void Exit()
        {
            Interlocked.Increment(ref _isExiting);
        }

        public override bool BeforeUpdate(GameTime gameTime)
        {
            return true;
        }

        public override bool BeforeDraw(GameTime gameTime)
        {
            return true;
        }

        public override void EnterFullScreen()
        {
        }

        public override void ExitFullScreen()
        {
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            _view.BeginScreenDeviceChange(willBeFullScreen);
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _view.EndScreenDeviceChange(screenDeviceName, clientWidth, clientHeight);
        }

        public override void Log(string message)
        {
            Console.WriteLine(message);
        }

        public override void Present()
        {
            if (Game.GraphicsDevice != null)
                Game.GraphicsDevice.Present();
        }

        protected override void Dispose(bool disposing)
        {
            if (_view != null)
            {
                _view.Dispose();
                _view = null;

                Joystick.CloseDevices();

                Sdl.Quit();
            }

            base.Dispose(disposing);
        }
    }
}
