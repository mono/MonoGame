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
            _map.Add(8, Keys.Back);
            _map.Add(9, Keys.Tab);
            _map.Add(13, Keys.Enter);
            _map.Add(27, Keys.Escape);
            _map.Add(32, Keys.Space);
            _map.Add(35, Keys.Oem8);
            _map.Add(39, Keys.OemQuotes);
            _map.Add(43, Keys.Add);
            _map.Add(44, Keys.OemComma);
            _map.Add(45, Keys.OemMinus);
            _map.Add(46, Keys.OemPeriod);
            _map.Add(47, Keys.OemQuestion);
            _map.Add(48, Keys.D0);
            _map.Add(49, Keys.D1);
            _map.Add(50, Keys.D2);
            _map.Add(51, Keys.D3);
            _map.Add(52, Keys.D4);
            _map.Add(53, Keys.D5);
            _map.Add(54, Keys.D6);
            _map.Add(55, Keys.D7);
            _map.Add(56, Keys.D8);
            _map.Add(57, Keys.D9);
            _map.Add(59, Keys.OemSemicolon);
            _map.Add(60, Keys.OemBackslash);
            _map.Add(61, Keys.OemPlus);
            _map.Add(91, Keys.OemOpenBrackets);
            _map.Add(92, Keys.OemPipe);
            _map.Add(93, Keys.OemCloseBrackets);
            _map.Add(96, Keys.OemTilde);
            _map.Add(97, Keys.A);
            _map.Add(98, Keys.B);
            _map.Add(99, Keys.C);
            _map.Add(100, Keys.D);
            _map.Add(101, Keys.E);
            _map.Add(102, Keys.F);
            _map.Add(103, Keys.G);
            _map.Add(104, Keys.H);
            _map.Add(105, Keys.I);
            _map.Add(106, Keys.J);
            _map.Add(107, Keys.K);
            _map.Add(108, Keys.L);
            _map.Add(109, Keys.M);
            _map.Add(110, Keys.N);
            _map.Add(111, Keys.O);
            _map.Add(112, Keys.P);
            _map.Add(113, Keys.Q);
            _map.Add(114, Keys.R);
            _map.Add(115, Keys.S);
            _map.Add(116, Keys.T);
            _map.Add(117, Keys.U);
            _map.Add(118, Keys.V);
            _map.Add(119, Keys.W);
            _map.Add(120, Keys.X);
            _map.Add(121, Keys.Y);
            _map.Add(122, Keys.Z);
            _map.Add(127, Keys.Delete);
            _map.Add(1073741881, Keys.CapsLock);
            _map.Add(1073741882, Keys.F1);
            _map.Add(1073741883, Keys.F2);
            _map.Add(1073741884, Keys.F3);
            _map.Add(1073741885, Keys.F4);
            _map.Add(1073741886, Keys.F5);
            _map.Add(1073741887, Keys.F6);
            _map.Add(1073741888, Keys.F7);
            _map.Add(1073741889, Keys.F8);
            _map.Add(1073741890, Keys.F9);
            _map.Add(1073741891, Keys.F10);
            _map.Add(1073741892, Keys.F11);
            _map.Add(1073741893, Keys.F12);
            _map.Add(1073741894, Keys.PrintScreen);
            _map.Add(1073741895, Keys.Scroll);
            _map.Add(1073741896, Keys.Pause);
            _map.Add(1073741897, Keys.Insert);
            _map.Add(1073741898, Keys.Home);
            _map.Add(1073741899, Keys.PageUp);
            _map.Add(1073741901, Keys.End);
            _map.Add(1073741902, Keys.PageDown);
            _map.Add(1073741903, Keys.Right);
            _map.Add(1073741904, Keys.Left);
            _map.Add(1073741905, Keys.Down);
            _map.Add(1073741906, Keys.Up);
            _map.Add(1073741907, Keys.NumLock);
            _map.Add(1073741908, Keys.Divide);
            _map.Add(1073741909, Keys.Multiply);
            _map.Add(1073741910, Keys.Subtract);
            _map.Add(1073741911, Keys.Add);
            _map.Add(1073741912, Keys.Enter);
            _map.Add(1073741913, Keys.NumPad1);
            _map.Add(1073741914, Keys.NumPad2);
            _map.Add(1073741915, Keys.NumPad3);
            _map.Add(1073741916, Keys.NumPad4);
            _map.Add(1073741917, Keys.NumPad5);
            _map.Add(1073741918, Keys.NumPad6);
            _map.Add(1073741919, Keys.NumPad7);
            _map.Add(1073741920, Keys.NumPad8);
            _map.Add(1073741921, Keys.NumPad9);
            _map.Add(1073741922, Keys.NumPad0);
            _map.Add(1073741923, Keys.Decimal);
            _map.Add(1073741925, Keys.Apps);
            _map.Add(1073741928, Keys.F13);
            _map.Add(1073741929, Keys.F14);
            _map.Add(1073741930, Keys.F15);
            _map.Add(1073741931, Keys.F16);
            _map.Add(1073741932, Keys.F17);
            _map.Add(1073741933, Keys.F18);
            _map.Add(1073741934, Keys.F19);
            _map.Add(1073741935, Keys.F20);
            _map.Add(1073741936, Keys.F21);
            _map.Add(1073741937, Keys.F22);
            _map.Add(1073741938, Keys.F23);
            _map.Add(1073741939, Keys.F24);
            _map.Add(1073741951, Keys.VolumeMute);
            _map.Add(1073741952, Keys.VolumeUp);
            _map.Add(1073741953, Keys.VolumeDown);
            _map.Add(1073742040, Keys.OemClear);
            _map.Add(1073742044, Keys.Decimal);
            _map.Add(1073742048, Keys.LeftControl);
            _map.Add(1073742049, Keys.LeftShift);
            _map.Add(1073742050, Keys.LeftAlt);
            _map.Add(1073742051, Keys.LeftWindows);
            _map.Add(1073742052, Keys.RightControl);
            _map.Add(1073742053, Keys.RightShift);
            _map.Add(1073742054, Keys.RightAlt);
            _map.Add(1073742055, Keys.RightWindows);
            _map.Add(1073742082, Keys.MediaNextTrack);
            _map.Add(1073742083, Keys.MediaPreviousTrack);
            _map.Add(1073742084, Keys.MediaStop);
            _map.Add(1073742085, Keys.MediaPlayPause);
            _map.Add(1073742086, Keys.VolumeMute);
            _map.Add(1073742087, Keys.SelectMedia);
            _map.Add(1073742089, Keys.LaunchMail);
            _map.Add(1073742092, Keys.BrowserSearch);
            _map.Add(1073742093, Keys.BrowserHome);
            _map.Add(1073742094, Keys.BrowserBack);
            _map.Add(1073742095, Keys.BrowserForward);
            _map.Add(1073742096, Keys.BrowserStop);
            _map.Add(1073742097, Keys.BrowserRefresh);
            _map.Add(1073742098, Keys.BrowserFavorites);
            _map.Add(1073742106, Keys.Sleep);
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

