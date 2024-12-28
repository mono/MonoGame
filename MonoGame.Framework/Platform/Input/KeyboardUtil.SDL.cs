// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Input
{
    internal static class KeyboardUtil
    {
        static Dictionary<int, Keys> _map;

        static KeyboardUtil()
        {
            _map = new Dictionary<int, Keys>();
            _map.Add(41, Keys.Escape);
            _map.Add(58, Keys.F1);
            _map.Add(59, Keys.F2);
            _map.Add(60, Keys.F3);
            _map.Add(61, Keys.F4);
            _map.Add(62, Keys.F5);
            _map.Add(63, Keys.F6);
            _map.Add(64, Keys.F7);
            _map.Add(65, Keys.F8);
            _map.Add(66, Keys.F9);
            _map.Add(67, Keys.F10);
            _map.Add(68, Keys.F11);
            _map.Add(69, Keys.F12);

            _map.Add(70, Keys.PrintScreen);
            _map.Add(71, Keys.Scroll);
            _map.Add(72, Keys.Pause);

            _map.Add(73, Keys.Insert);
            _map.Add(74, Keys.Home);
            _map.Add(75, Keys.PageUp);
            _map.Add(76, Keys.Delete);
            _map.Add(77, Keys.End);
            _map.Add(78, Keys.PageDown);

            _map.Add(53, Keys.OemTilde);
            _map.Add(30, Keys.D1);
            _map.Add(31, Keys.D2);
            _map.Add(32, Keys.D3);
            _map.Add(33, Keys.D4);
            _map.Add(34, Keys.D5);
            _map.Add(35, Keys.D6);
            _map.Add(36, Keys.D7);
            _map.Add(37, Keys.D8);
            _map.Add(38, Keys.D9);
            _map.Add(39, Keys.D0);
            _map.Add(45, Keys.OemMinus);
            _map.Add(46, Keys.OemPlus);
            _map.Add(42, Keys.Back);

            _map.Add(83, Keys.NumLock);
            _map.Add(84, Keys.Divide);
            _map.Add(85, Keys.Multiply);
            _map.Add(86, Keys.Subtract);
            _map.Add(89, Keys.NumPad1);
            _map.Add(90, Keys.NumPad2);
            _map.Add(91, Keys.NumPad3);
            _map.Add(92, Keys.NumPad4);
            _map.Add(93, Keys.NumPad5);
            _map.Add(94, Keys.NumPad6);
            _map.Add(95, Keys.NumPad7);
            _map.Add(96, Keys.NumPad8);
            _map.Add(97, Keys.NumPad9);
            _map.Add(98, Keys.NumPad0);
            _map.Add(99, Keys.Decimal);
            _map.Add(88, Keys.Enter);

            _map.Add(43, Keys.Tab);
            _map.Add(20, Keys.Q);
            _map.Add(26, Keys.W);
            _map.Add(8, Keys.E);
            _map.Add(21, Keys.R);
            _map.Add(23, Keys.T);
            _map.Add(28, Keys.Y);
            _map.Add(24, Keys.U);
            _map.Add(12, Keys.I);
            _map.Add(18, Keys.O);
            _map.Add(19, Keys.P);
            _map.Add(47, Keys.OemOpenBrackets);
            _map.Add(48, Keys.OemCloseBrackets);
            _map.Add(40, Keys.Enter);
            
            _map.Add(87, Keys.Add);
            _map.Add(57, Keys.CapsLock);
            _map.Add(4, Keys.A);
            _map.Add(22, Keys.S);
            _map.Add(7, Keys.D);
            _map.Add(9, Keys.F);
            _map.Add(10, Keys.G);
            _map.Add(11, Keys.H);
            _map.Add(13, Keys.J);
            _map.Add(14, Keys.K);
            _map.Add(15, Keys.L);
            _map.Add(51, Keys.OemSemicolon);
            _map.Add(52, Keys.OemQuotes);
            _map.Add(49, Keys.OemPipe);
            
            _map.Add(225, Keys.LeftShift);
            _map.Add(100, Keys.OemPipe);
            _map.Add(29, Keys.Z);
            _map.Add(27, Keys.X);
            _map.Add(6, Keys.C);
            _map.Add(25, Keys.V);
            _map.Add(5, Keys.B);
            _map.Add(17, Keys.N);
            _map.Add(16, Keys.M);
            _map.Add(54, Keys.OemComma);
            _map.Add(55, Keys.OemPeriod);
            _map.Add(56, Keys.OemQuestion);
            _map.Add(229, Keys.RightShift);
            
            _map.Add(224, Keys.LeftControl);
            _map.Add(227, Keys.LeftWindows);
            _map.Add(226, Keys.LeftAlt);
            _map.Add(44, Keys.Space);
            _map.Add(230, Keys.RightAlt);
            _map.Add(231, Keys.RightWindows);
            _map.Add(101, Keys.Apps);
            _map.Add(228, Keys.RightControl);

            _map.Add(82, Keys.Up);
            _map.Add(80, Keys.Left);
            _map.Add(81, Keys.Down);
            _map.Add(79, Keys.Right);

            _map.Add(261, Keys.MediaPlayPause);
            _map.Add(260, Keys.MediaStop);
            _map.Add(259, Keys.MediaPreviousTrack);
            _map.Add(258, Keys.MediaNextTrack);
            _map.Add(262, Keys.VolumeMute);
            _map.Add(128, Keys.VolumeUp);
            _map.Add(129, Keys.VolumeDown);
        }

        public static Keys ToXna(int key)
        {
            Keys xnaKey;
            if (_map.TryGetValue(key, out xnaKey))
                return xnaKey;

            return Keys.None;
        }
    }
}

