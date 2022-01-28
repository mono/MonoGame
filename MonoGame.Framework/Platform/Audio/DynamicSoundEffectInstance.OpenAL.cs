// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using MonoGame.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    public sealed partial class DynamicSoundEffectInstance : SoundEffectInstance
    {
        private Queue<OALSoundBuffer> _queuedBuffers;
        private ALFormat _format;

        private void PlatformCreate()
        {
            _format = _channels == AudioChannels.Mono ? ALFormat.Mono16 : ALFormat.Stereo16;
            InitializeSound();

            SourceId = controller.ReserveSource();
            HasSourceId = true;

            _queuedBuffers = new Queue<OALSoundBuffer>();
        }

        private int PlatformGetPendingBufferCount()
        {
            return _queuedBuffers.Count;
        }

        private void PlatformPlay()
        {
            AL.GetError();

            // Ensure that the source is not looped (due to source recycling)
            AL.Source(SourceId, ALSourceb.Looping, false);
            ALHelper.CheckError("Failed to set source loop state.");

            AL.SourcePlay(SourceId);
            ALHelper.CheckError("Failed to play the source.");
        }

        private void PlatformPause()
        {
            AL.GetError();
            AL.SourcePause(SourceId);
            ALHelper.CheckError("Failed to pause the source.");
        }

        private void PlatformResume()
        {
            AL.GetError();
            AL.SourcePlay(SourceId);
            ALHelper.CheckError("Failed to play the source.");
        }

        private void PlatformStop()
        {
            AL.GetError();
            AL.SourceStop(SourceId);
            ALHelper.CheckError("Failed to stop the source.");

            // Remove all queued buffers
            AL.Source(SourceId, ALSourcei.Buffer, 0);
            while (_queuedBuffers.Count > 0)
            {
                var buffer = _queuedBuffers.Dequeue();
                buffer.Dispose();
            }
        }

        private void PlatformSubmitBuffer(byte[] buffer, int offset, int count)
        {
            // Get a buffer
            OALSoundBuffer oalBuffer = new OALSoundBuffer();

            // Bind the data
            if (offset == 0)
            {
                oalBuffer.BindDataBuffer(buffer, _format, count, _sampleRate);
            }
            else
            {
                // BindDataBuffer does not support offset
                var offsetBuffer = new byte[count];
                Array.Copy(buffer, offset, offsetBuffer, 0, count);
                oalBuffer.BindDataBuffer(offsetBuffer, _format, count, _sampleRate);
            }

            // Queue the buffer
            _queuedBuffers.Enqueue(oalBuffer);
            AL.SourceQueueBuffer(SourceId, oalBuffer.OpenALDataBuffer);
            ALHelper.CheckError();

            // If the source has run out of buffers, restart it
            var sourceState = AL.GetSourceState(SourceId);
            if (_state == SoundState.Playing && sourceState == ALSourceState.Stopped)
            {
                AL.SourcePlay(SourceId);
                ALHelper.CheckError("Failed to resume source playback.");
            }
        }

        private unsafe void AlSubmitBuffer(IntPtr dataPtr, int count, ALFormat format)
        {
            // Get a buffer
            OALSoundBuffer oalBuffer = new OALSoundBuffer();

            // Bind the data
            oalBuffer.BindDataBuffer(dataPtr, format, count, _sampleRate);

            // Queue the buffer
            AL.SourceQueueBuffer(SourceId, oalBuffer.OpenALDataBuffer);
            ALHelper.CheckError();
            _queuedBuffers.Enqueue(oalBuffer);

            // If the source has run out of buffers, restart it
            var sourceState = AL.GetSourceState(SourceId);
            if (_state == SoundState.Playing && sourceState == ALSourceState.Stopped)
            {
                AL.SourcePlay(SourceId);
                ALHelper.CheckError("Failed to resume source playback.");
            }
        }

        internal unsafe void SubmitFloatBuffer(IntPtr audioDataPtr, float[] buffer, int sampleOffset)
        {
            var format = _channels == AudioChannels.Mono ? ALFormat.MonoFloat32 : ALFormat.StereoFloat32;

            AlSubmitBuffer(audioDataPtr, buffer.Length, format);
        }

        private void PlatformDispose(bool disposing)
        {
            // SFXI disposal handles buffer detachment and source recycling
            base.Dispose(disposing);

            if (disposing)
            {
                while (_queuedBuffers.Count > 0)
                {
                    var buffer = _queuedBuffers.Dequeue();
                    buffer.Dispose();
                }

                DynamicSoundEffectInstanceManager.RemoveInstance(this);
            }
        }

        private void PlatformUpdateQueue()
        {
            // Get the completed buffers
            AL.GetError();
            int numBuffers;
            AL.GetSource(SourceId, ALGetSourcei.BuffersProcessed, out numBuffers);
            ALHelper.CheckError("Failed to get processed buffer count.");

            // Unqueue them
            if (numBuffers > 0)
            {
                AL.SourceUnqueueBuffers(SourceId, numBuffers);
                ALHelper.CheckError("Failed to unqueue buffers.");
                for (int i = 0; i < numBuffers; i++)
                {
                    var buffer = _queuedBuffers.Dequeue();
                    buffer.Dispose();
                }
            }

            // Raise the event for each removed buffer, if needed
            for (int i = 0; i < numBuffers; i++)
                CheckBufferCount();
        }
    }
}
