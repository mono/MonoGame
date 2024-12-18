// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.Xna.Framework.Content.Pipeline.Audio
{
    /// <summary>
    /// Encapsulates and provides operations, such as format conversions, on the
    /// source audio. This type is produced by the audio importers and used by audio
    /// processors to produce compiled audio assets.
    /// </summary>
    /// <remarks>Note that AudioContent can load and process audio files that are not supported by the importers.</remarks>
    public class AudioContent : ContentItem, IDisposable
    {
        private bool _disposed;
        private readonly string _fileName;
        private readonly AudioFileType _fileType;
        private ReadOnlyCollection<byte> _data;
        private TimeSpan _duration;
        private AudioFormat _format;
        private int _loopStart;
        private int _loopLength;

        /// <summary>
        /// The name of the original source audio file.
        /// </summary>
        [ContentSerializer(AllowNull = false)]
        public string FileName { get { return _fileName; } }

        /// <summary>
        /// The type of the original source audio file.
        /// </summary>
        public AudioFileType FileType { get { return _fileType; } }

        /// <summary>
        /// The current raw audio data without header information.
        /// </summary>
        /// <remarks>
        /// This changes from the source data to the output data after conversion.
        /// For MP3 and WMA files this throws an exception to match XNA behavior.
        /// </remarks>
        public ReadOnlyCollection<byte> Data
        {
            get
            {
                if (_disposed || _data == null)
                    throw new InvalidContentException("Could not read the audio data from file \"" + Path.GetFileName(_fileName) + "\".");
                return _data;
            }
        }

        /// <summary>
        /// The duration of the audio data.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
        }

        /// <summary>
        /// The current format of the audio data.
        /// </summary>
        /// <remarks>This changes from the source format to the output format after conversion.</remarks>
        public AudioFormat Format
        {
            get
            {
                return _format;
            }
        }

        /// <summary>
        /// The current loop length in samples.
        /// </summary>
        /// <remarks>This changes from the source loop length to the output loop length after conversion.</remarks>
        public int LoopLength
        {
            get
            {
                return _loopLength;
            }
        }

        /// <summary>
        /// The current loop start location in samples.
        /// </summary>
        /// <remarks>This changes from the source loop start to the output loop start after conversion.</remarks>
        public int LoopStart
        {
            get
            {
                return _loopStart;
            }
        }

        /// <summary>
        /// Initializes a new instance of AudioContent.
        /// </summary>
        /// <param name="audioFileName">Name of the audio source file to be processed.</param>
        /// <param name="audioFileType">Type of the processed audio: WAV, MP3 or WMA.</param>
        /// <remarks>Constructs the object from the specified source file, in the format specified.</remarks>
        public AudioContent(string audioFileName, AudioFileType audioFileType)
        {
            _fileName = audioFileName;

            try
            {
                // Get the full path to the file.
                audioFileName = Path.GetFullPath(audioFileName);

                // Use probe to get the details of the file.
                DefaultAudioProfile.ProbeFormat(audioFileName, out _fileType, out _format, out _duration, out _loopStart, out _loopLength);

                // Looks like XNA only cares about type mismatch when
                // the type is WAV... else it is ok.
                if (    (audioFileType == AudioFileType.Wav || _fileType == AudioFileType.Wav) &&
                        audioFileType != _fileType)
                    throw new ArgumentException("Incorrect file type!", "audioFileType");

                // Only provide the data for WAV files.
                if (audioFileType == AudioFileType.Wav)
                {
                    byte[] rawData;

                    // Must be opened in read mode otherwise it fails to open
                    // read-only files (found in some source control systems)
                    using (var fs = new FileStream(audioFileName, FileMode.Open, FileAccess.Read))
                    {
                        rawData = new byte[fs.Length];
                        fs.Read(rawData, 0, rawData.Length);
                    }

                    AudioFormat riffAudioFormat;
                    var stripped = DefaultAudioProfile.StripRiffWaveHeader(rawData, out riffAudioFormat);

                    if (riffAudioFormat != null)
                    {
                        if ((_format.Format != 2 && _format.Format != 17) && _format.BlockAlign != riffAudioFormat.BlockAlign)
                            throw new InvalidOperationException("Calculated block align does not match RIFF " + _format.BlockAlign + " : " + riffAudioFormat.BlockAlign);
                        if (_format.ChannelCount != riffAudioFormat.ChannelCount)
                            throw new InvalidOperationException("Probed channel count does not match RIFF: " + _format.ChannelCount + ", " + riffAudioFormat.ChannelCount);
                        if (_format.Format != riffAudioFormat.Format)
                            throw new InvalidOperationException("Probed audio format does not match RIFF: " + _format.Format + ", " + riffAudioFormat.Format);
                        if (_format.SampleRate != riffAudioFormat.SampleRate)
                            throw new InvalidOperationException("Probed sample rate does not match RIFF: " + _format.SampleRate + ", " + riffAudioFormat.SampleRate);
                    }

                    _data = Array.AsReadOnly(stripped);
                }
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed to open file {0}. Ensure the file is a valid audio file and is not DRM protected.", Path.GetFileNameWithoutExtension(audioFileName));
                throw new InvalidContentException(message, ex);
            }
        }

        /// <summary>
        /// Transcodes the source audio to the target format and quality.
        /// </summary>
        /// <param name="formatType">Format to convert this audio to.</param>
        /// <param name="quality">Quality of the processed output audio. For streaming formats, it can be one of the following: Low (96 kbps), Medium (128 kbps), Best (192 kbps).  For WAV formats, it can be one of the following: Low (11kHz ADPCM), Medium (22kHz ADPCM), Best (44kHz PCM)</param>
        /// <param name="saveToFile">
        /// The name of the file that the converted audio should be saved into.  This is used for SongContent, where
        /// the audio is stored external to the XNB file.  If this is null, then the converted audio is stored in
        /// the Data property.
        /// </param>
        [Obsolete("You should prefer to use AudioProfile.")]
        public void ConvertFormat(ConversionFormat formatType, ConversionQuality quality, string saveToFile)
        {
            // Call the legacy conversion code.
            DefaultAudioProfile.ConvertToFormat(this, formatType, quality, saveToFile);
        }

        /// <summary>
        /// Sets the data and properties for the audio content.
        /// </summary>
        /// <param name="data">The audio data to be set.</param>
        /// <param name="format">The audio format.</param>
        /// <param name="duration">The duration of the audio.</param>
        /// <param name="loopStart">The start position of the loop.</param>
        /// <param name="loopLength">The length of the loop.</param>
        /// <exception cref="ArgumentNullException">Thrown when the data or format is null.</exception>
        public void SetData(byte[] data, AudioFormat format, TimeSpan duration, int loopStart, int loopLength)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (format == null)
                throw new ArgumentNullException("format");

            _data = Array.AsReadOnly(data);
            _format = format;
            _duration = duration;
            _loopStart = loopStart;
            _loopLength = loopLength;
        }

        /// <inheritdoc cref="IDisposable.Dispose()"/>
        public void Dispose()
        {
            _disposed = true;
            _data = null;
        }
    }
}
