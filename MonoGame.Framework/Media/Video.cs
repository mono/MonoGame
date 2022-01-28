// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace Microsoft.Xna.Framework.Media
{
    /// <summary>
    /// Represents a video.
    /// </summary>
    public sealed partial class Video : IDisposable
    {
        private bool _disposed;

        #region Public API

        /// <summary>
        /// I actually think this is a file PATH...
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the duration of the Video.
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Gets the frame rate of this video.
        /// </summary>
        public float FramesPerSecond { get; internal set; }

        /// <summary>
        /// Gets the height of this video, in pixels.
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// Gets the VideoSoundtrackType for this video.
        /// </summary>
        public VideoSoundtrackType VideoSoundtrackType { get; internal set; }

        /// <summary>
        /// Gets the width of this video, in pixels.
        /// </summary>
        public int Width { get; internal set; }

        #endregion

        #region Internal API

        internal Video(string fileName, float durationMS) :
            this(fileName)
        {
            Duration = TimeSpan.FromMilliseconds(durationMS);
        }

        internal Video(string fileName)
        {
            FileName = fileName;

#if !WINDOWS_UAP && !DESKTOPGL
            PlatformInitialize();
#endif
        }

        internal Video(string fileName, float durationMS, int width, int height, float framesPerSecond, VideoSoundtrackType videoSoundtrackType ) :
            this(fileName, durationMS)
        {
            Width = width;
            Height = height;
            FramesPerSecond = framesPerSecond;
            VideoSoundtrackType = videoSoundtrackType;

#if DESKTOPGL && !WINDOWS_UAP
            PlatformInitialize();
#endif
        }



        ~Video()
        {
            Dispose(false);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                //PlatformDispose(disposing);
                _disposed = true;
            }
        }

        #endregion
    }
}
