// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Audio
{
    /// <summary>
    /// A <see cref="SoundEffectInstance"/> for which the audio buffer is provided by the game at run time.
    /// </summary>
    public sealed partial class DynamicSoundEffectInstance : SoundEffectInstance
    {
        #region Public Properties

        /// <summary>
        /// This value has no effect on DynamicSoundEffectInstance.
        /// It may not be set.
        /// </summary>
        public override bool IsLooped
        {
            get
            {
                return false;
            }

            set
            {
                AssertNotDisposed();
                if (value == true)
                    throw new InvalidOperationException("IsLooped cannot be set true. Submit looped audio data to implement looping.");
            }
        }

        /// <inheritdoc />
        public override SoundState State
        {
            get
            {
                AssertNotDisposed();
                return _state;
            }
        }

        /// <summary>
        /// Returns the number of audio buffers queued for playback.
        /// </summary>
        public int PendingBufferCount
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetPendingBufferCount();
            }
        }

        /// <summary>
        /// The event that occurs when the number of queued audio buffers is less than or equal to 2.
        /// </summary>
        /// <remarks>
        /// This event may occur when <see cref="Play()"/> is called or during playback when a buffer is completed.
        /// </remarks>
        public event EventHandler<EventArgs> BufferNeeded;

        #endregion

        private const int TargetPendingBufferCount = 3;
        private int _buffersNeeded;
        private int _sampleRate;
        private AudioChannels _channels;
        private SoundState _state;

        #region Public Constructor

        /// <param name="sampleRate">Sample rate, in Hertz (Hz).</param>
        /// <param name="channels">Number of channels (mono or stereo).</param>
        public DynamicSoundEffectInstance(int sampleRate, AudioChannels channels)
        {
            SoundEffect.Initialize();
            if (SoundEffect._systemState != SoundEffect.SoundSystemState.Initialized)
                throw new NoAudioHardwareException("Audio has failed to initialize. Call SoundEffect.Initialize() before sound operation to get more specific errors.");

            if ((sampleRate < 8000) || (sampleRate > 48000))
                throw new ArgumentOutOfRangeException("sampleRate");
            if ((channels != AudioChannels.Mono) && (channels != AudioChannels.Stereo))
                throw new ArgumentOutOfRangeException("channels");

            _sampleRate = sampleRate;
            _channels = channels;
            _state = SoundState.Stopped;
            PlatformCreate();
            
            // This instance is added to the pool so that its volume reflects master volume changes
            // and it contributes to the playing instances limit, but the source/voice is not owned by the pool.
            _isPooled = false;
            _isDynamic = true;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns the duration of an audio buffer of the specified size, based on the settings of this instance.
        /// </summary>
        /// <param name="sizeInBytes">Size of the buffer, in bytes.</param>
        /// <returns>The playback length of the buffer.</returns>
        public TimeSpan GetSampleDuration(int sizeInBytes)
        {
            AssertNotDisposed();
            return SoundEffect.GetSampleDuration(sizeInBytes, _sampleRate, _channels);
        }

        /// <summary>
        /// Returns the size, in bytes, of a buffer of the specified duration, based on the settings of this instance.
        /// </summary>
        /// <param name="duration">The playback length of the buffer.</param>
        /// <returns>The data size of the buffer, in bytes.</returns>
        public int GetSampleSizeInBytes(TimeSpan duration)
        {
            AssertNotDisposed();
            return SoundEffect.GetSampleSizeInBytes(duration, _sampleRate, _channels);
        }

        /// <summary>
        /// Plays or resumes the DynamicSoundEffectInstance.
        /// </summary>
        public override void Play()
        {
            AssertNotDisposed();

            if (_state != SoundState.Playing)
            {
                // Ensure that the volume reflects master volume, which is done by the setter.
                Volume = Volume;

                // Add the instance to the pool
                if (!SoundEffectInstancePool.SoundsAvailable)
                    throw new InstancePlayLimitException();
                SoundEffectInstancePool.Remove(this);

                PlatformPlay();
                _state = SoundState.Playing;

                CheckBufferCount();
                DynamicSoundEffectInstanceManager.AddInstance(this);
            }
        }

        /// <summary>
        /// Pauses playback of the DynamicSoundEffectInstance.
        /// </summary>
        public override void Pause()
        {
            AssertNotDisposed();
            PlatformPause();
            _state = SoundState.Paused;
        }

        /// <summary>
        /// Resumes playback of the DynamicSoundEffectInstance.
        /// </summary>
        public override void Resume()
        {
            AssertNotDisposed();

            if (_state != SoundState.Playing)
            {
                Volume = Volume;

                // Add the instance to the pool
                if (!SoundEffectInstancePool.SoundsAvailable)
                    throw new InstancePlayLimitException();
                SoundEffectInstancePool.Remove(this);
            }

            PlatformResume();
            _state = SoundState.Playing;
        }

        /// <summary>
        /// Immediately stops playing the DynamicSoundEffectInstance.
        /// </summary>
        /// <remarks>
        /// Calling this also releases all queued buffers.
        /// </remarks>
        public override void Stop()
        {
            Stop(true);
        }

        /// <summary>
        /// Stops playing the DynamicSoundEffectInstance.
        /// If the <paramref name="immediate"/> parameter is false, this call has no effect.
        /// </summary>
        /// <remarks>
        /// Calling this also releases all queued buffers.
        /// </remarks>
        /// <param name="immediate">When set to false, this call has no effect.</param>
        public override void Stop(bool immediate)
        {
            AssertNotDisposed();
            
            if (immediate)
            {
                DynamicSoundEffectInstanceManager.RemoveInstance(this);

                PlatformStop();
                _state = SoundState.Stopped;

                SoundEffectInstancePool.Add(this);
            }
        }

        /// <summary>
        /// Queues an audio buffer for playback.
        /// </summary>
        /// <remarks>
        /// The buffer length must conform to alignment requirements for the audio format.
        /// </remarks>
        /// <param name="buffer">The buffer containing PCM audio data.</param>
        public void SubmitBuffer(byte[] buffer)
        {
            AssertNotDisposed();
            
            if (buffer.Length == 0)
                throw new ArgumentException("Buffer may not be empty.");

            // Ensure that the buffer length matches alignment.
            // The data must be 16-bit, so the length is a multiple of 2 (mono) or 4 (stereo).
            var sampleSize = 2 * (int)_channels;
            if (buffer.Length % sampleSize != 0)
                throw new ArgumentException("Buffer length does not match format alignment.");

            SubmitBuffer(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Queues an audio buffer for playback.
        /// </summary>
        /// <remarks>
        /// The buffer length must conform to alignment requirements for the audio format.
        /// </remarks>
        /// <param name="buffer">The buffer containing PCM audio data.</param>
        /// <param name="offset">The starting position of audio data.</param>
        /// <param name="count">The amount of bytes to use.</param>
        public void SubmitBuffer(byte[] buffer, int offset, int count)
        {
            AssertNotDisposed();
            
            if ((buffer == null) || (buffer.Length == 0))
                throw new ArgumentException("Buffer may not be null or empty.");
            if (count <= 0)
                throw new ArgumentException("Number of bytes must be greater than zero.");
            if ((offset + count) > buffer.Length)
                throw new ArgumentException("Buffer is shorter than the specified number of bytes from the offset.");

            // Ensure that the buffer length and start position match alignment.
            var sampleSize = 2 * (int)_channels;
            if (count % sampleSize != 0)
                throw new ArgumentException("Number of bytes does not match format alignment.");
            if (offset % sampleSize != 0)
                throw new ArgumentException("Offset into the buffer does not match format alignment.");

            PlatformSubmitBuffer(buffer, offset, count);
        }

        #endregion

        #region Nonpublic Functions

        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
        }

        /// <summary />
        protected override void Dispose(bool disposing)
        {
            PlatformDispose(disposing);
            base.Dispose(disposing);
        }

        private void CheckBufferCount()
        {
            if ((PendingBufferCount < TargetPendingBufferCount) && (_state == SoundState.Playing))
                _buffersNeeded++;
        }

        internal void UpdateQueue()
        {
            // Update the buffers
            PlatformUpdateQueue();

            // Raise the event
            var bufferNeededHandler = BufferNeeded;
            if (bufferNeededHandler != null)
            {
                if (_buffersNeeded >= 1)
                {
                    // At least one event is needed after the initial play call and per processed buffer
                    int minimumNoOfBufferNeededEvents = (_buffersNeeded < 3) ? _buffersNeeded : 3;

                    // Multiple events may be raised to reach the target buffer count so long as
                    // a SubmitBuffer occurs for each event
                    int noOfBuffersNeededEventsToReachTarget = TargetPendingBufferCount - PendingBufferCount;

                    for (int i = 0; i < noOfBuffersNeededEventsToReachTarget; i++)
                    {
                        // Raise the BufferNeeded event and check if SubmitBuffer occurred
                        int lastPendingBufferCount = PendingBufferCount;
                        bufferNeededHandler(this, EventArgs.Empty);
                        bool submitBufferOccurred = PendingBufferCount > lastPendingBufferCount;

                        // Stop raising events if no SubmitBuffer occurred during BufferNeeded and
                        // the minimum number of events has already been raised
                        if (!submitBufferOccurred && ((i + 1) >= minimumNoOfBufferNeededEvents))
                            break;
                    }
                }
            }

            _buffersNeeded = 0;
        }

        #endregion
    }
}
