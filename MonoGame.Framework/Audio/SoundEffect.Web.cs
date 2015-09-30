// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
﻿
using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Audio
{
    public sealed partial class SoundEffect : IDisposable
    {
        private void PlatformLoadAudioStream(Stream s)
        {
        }

        private void PlatformInitialize(byte[] buffer, int sampleRate, AudioChannels channels)
        {
        }
        
        private void PlatformInitialize(byte[] buffer, int offset, int count, int sampleRate, AudioChannels channels, int loopStart, int loopLength)
        {
        }
        
        private void PlatformSetupInstance(SoundEffectInstance instance)
        {
        }
		
		/// <summary>
        /// Test if a SoundEffectInstance is compatible (i.e. same sampling rate, number of channels, etc.) with the SoundEffect.
        /// This method is used by the SoundEffectInstancePool to re-use instances efficiently.
        /// </summary>
        /// <param name="inst">The SoundEffectInstance to test</param>
        /// <returns>True if compatible, false otherwise</returns>
        internal bool PlatformIsInstanceCompatible(SoundEffectInstance inst)
        {
            return true;
        }

        private void PlatformDispose(bool disposing)
        {
        }

        internal static void PlatformShutdown()
        {
        }
    }
}

