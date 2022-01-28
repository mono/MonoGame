// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Linq;
using Microsoft.Xna.Framework.Media;
using System.Globalization;

namespace Microsoft.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides a base class for all video objects.
    /// </summary>
    public class VideoContent : ContentItem, IDisposable
    {
        private bool _disposed;
        private int _bitsPerSecond;
        private TimeSpan _duration;
        private float _framesPerSecond;
        private int _height;
        private int _width;

        /// <summary>
        /// Gets the bit rate for this video.
        /// </summary>
        public int BitsPerSecond { get { return _bitsPerSecond; } }

        /// <summary>
        /// Gets the duration of this video.
        /// </summary>
        public TimeSpan Duration { get { return _duration; } }

        /// <summary>
        /// Gets or sets the file name for this video.
        /// </summary>
        [ContentSerializerAttribute]
        public string Filename { get; set; }

        /// <summary>
        /// Gets the frame rate for this video.
        /// </summary>
        public float FramesPerSecond { get { return _framesPerSecond; } }

        /// <summary>
        /// Gets the height of this video.
        /// </summary>
        public int Height { get { return _height; } }

        /// <summary>
        /// Gets or sets the type of soundtrack accompanying the video.
        /// </summary>
        [ContentSerializerAttribute]
        public VideoSoundtrackType VideoSoundtrackType { get; set; }

        /// <summary>
        /// Gets the width of this video.
        /// </summary>
        public int Width { get { return _width; } }

        /// <summary>
        /// Initializes a new copy of the VideoContent class for the specified video file.
        /// </summary>
        /// <param name="filename">The file name of the video to import.</param>
        public VideoContent(string filename)
        {
            Filename = filename;

            string stdout, stderr;
            var result = ExternalTool.Run("ffprobe",
                string.Format("-i \"{0}\" -show_format -select_streams v -show_streams -print_format ini", Filename), out stdout, out stderr);

            var lines = stdout.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (!line.Contains('='))
                    continue;

                var key = line.Substring(0, line.IndexOf('='));
                var value = line.Substring(line.IndexOf('=') + 1);
                switch (key)
                {
                    case "duration":
                        double durationSeconds;
                        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out durationSeconds))
                            _duration = TimeSpan.FromSeconds(durationSeconds);
                        break;

                    case "bit_rate":
                        int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _bitsPerSecond);
                        break;

                    case "width":
                        int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _width);
                        break;

                    case "height":
                        int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _height);
                        break;

                    case "r_frame_rate":
                        var frac = value.Split('/');
                        _framesPerSecond = float.Parse(frac[0], CultureInfo.InvariantCulture) / float.Parse(frac[1], CultureInfo.InvariantCulture);
                        break;
                }
            }
        }

        ~VideoContent()
        {
            Dispose(false);
        }

        /// <summary>
        /// Immediately releases the unmanaged resources used by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: Free managed resources here
                    // ...
                }
                // TODO: Free unmanaged resources here
                // ...
                _disposed = true;
            }
        }
    }
}
