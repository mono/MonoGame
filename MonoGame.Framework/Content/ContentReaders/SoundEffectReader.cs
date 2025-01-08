// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Audio;


namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class SoundEffectReader : ContentTypeReader<SoundEffect>
	{
		protected internal override SoundEffect Read(ContentReader input, SoundEffect existingInstance)
		{         
            // XNB format for SoundEffect...
            //            
            // Byte [format size]	Format	WAVEFORMATEX structure
            // UInt32	Data size	
            // Byte [data size]	Data	Audio waveform data
            // Int32	Loop start	In bytes (start must be format block aligned)
            // Int32	Loop length	In bytes (length must be format block aligned)
            // Int32	Duration	In milliseconds

            // The header containss the WAVEFORMATEX header structure
            // defined as the following...
            //
            //  WORD  wFormatTag;       // byte[0]  +2
            //  WORD  nChannels;        // byte[2]  +2
            //  DWORD nSamplesPerSec;   // byte[4]  +4
            //  DWORD nAvgBytesPerSec;  // byte[8]  +4
            //  WORD  nBlockAlign;      // byte[12] +2
            //  WORD  wBitsPerSample;   // byte[14] +2
            //  WORD  cbSize;           // byte[16] +2
            //
            // We let the sound effect deal with parsing this based
            // on what format the audio data actually is.

		    var headerSize = input.ReadInt32();
            var header = input.ReadBytes(headerSize);

            // Read the audio data buffer.
            var dataSize = input.ReadInt32();
            var data = ContentManager.ScratchBufferPool.Get(dataSize);
            input.Read(data, 0, dataSize);

            var loopStart = input.ReadInt32();
            var loopLength = input.ReadInt32();
            var durationMs = input.ReadInt32();

            // Create the effect.
            var effect = new SoundEffect(header, data, dataSize, durationMs, loopStart, loopLength);

            // Store the original asset name for debugging later.
            effect.Name = input.AssetName;

            ContentManager.ScratchBufferPool.Return(data);

            return effect;
        }
	}
}
