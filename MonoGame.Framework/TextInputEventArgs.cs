// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// This class is used for the game window's TextInput event as EventArgs.
    /// </summary>
    public struct TextInputEventArgs
    {
        public TextInputEventArgs(char character = '\u0000', Keys key = Keys.None)
        {
            Character = character;
            Key = key;
        }
        public char Character
        {
            get; private set;
        }
        public Keys Key
        {
            get; private set;
        }
    }
}
