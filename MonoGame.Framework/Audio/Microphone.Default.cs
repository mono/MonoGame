﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using OpenAL;
using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Audio
{ 
    public sealed partial class Microphone
    {
        internal void PlatformStart()
        {
			throw new NotImplementedException();
        }

        internal void PlatformStop()
        {
			throw new NotImplementedException();
        }
		
		internal void Update()
		{
			throw new NotImplementedException();
		}
		
		internal int PlatformGetData(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
