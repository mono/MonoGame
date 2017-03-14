// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics
{
    internal class PresentationChangingEventArgs : EventArgs
    {
        public PresentationParameters PresentationParameters { get; }

        public PresentationChangingEventArgs(PresentationParameters presentationParameters)
        {
            PresentationParameters = presentationParameters;
        }
    }
}
