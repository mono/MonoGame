#region License
/*
Microsoft Public License (Ms-PL)
MonoGame - Copyright © 2009 The MonoGame Team

All rights reserved.

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
purpose and non-infringement.
*/
#endregion License

using System;
using System.IO;

using Microsoft.Xna.Framework.Audio;

#if IOS
using MonoTouch.Foundation;
using MonoTouch.AVFoundation;
#endif

namespace Microsoft.Xna.Framework.Media
{
    public sealed class Song : IEquatable<Song>, IDisposable
    {
#if IOS
		private AVAudioPlayer _sound;
#elif PSM
        private PSSuiteSong _sound;
#elif !DIRECTX
		private SoundEffectInstance _sound;
#endif
		
		private string _name;
		private int _playCount = 0;
        bool disposed;

        internal Song(string fileName, int durationMS)
            : this(fileName)
        {
            _Duration = TimeSpan.FromMilliseconds(durationMS);
        }
		internal Song(string fileName)
		{			
			_name = fileName;
			
#if IOS
			_sound = AVAudioPlayer.FromUrl(NSUrl.FromFilename(fileName));
			_sound.NumberOfLoops = 0;
            _sound.FinishedPlaying += OnFinishedPlaying;
#elif PSM
            _sound = new PSSuiteSong(_name);
#elif !DIRECTX
            _sound = new SoundEffect(_name).CreateInstance();
#endif
		}

        ~Song()
        {
            Dispose(false);
        }

        public string FilePath
		{
			get { return _name; }
		}
		
		public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        void Dispose(bool disposing)
        {
            if (!disposed)
            {
#if !DIRECTX
                if (disposing)
                {
                    if (_sound != null)
                    {
#if IOS
                       _sound.FinishedPlaying -= OnFinishedPlaying;
#endif
                        _sound.Dispose();
                        _sound = null;
                    }
                }
#endif
                disposed = true;
            }
        }
        
		public bool Equals(Song song)
        {
#if DIRECTX
            return song != null && song.FilePath == FilePath;
#else
			return ((object)song != null) && (Name == song.Name);
#endif
		}
		
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		
		public override bool Equals(Object obj)
		{
			if(obj == null)
			{
				return false;
			}
			
			return Equals(obj as Song);  
		}
		
		public static bool operator ==(Song song1, Song song2)
		{
			if((object)song1 == null)
			{
				return (object)song2 == null;
			}

			return song1.Equals(song2);
		}
		
		public static bool operator !=(Song song1, Song song2)
		{
		  return ! (song1 == song2);
		}

#if !DIRECTX
        internal delegate void FinishedPlayingHandler(object sender, EventArgs args);
		event FinishedPlayingHandler DonePlaying;

		internal void OnFinishedPlaying (object sender, EventArgs args)
		{
			if (DonePlaying == null)
				return;
			
			DonePlaying(sender, args);
		}

		/// <summary>
		/// Set the event handler for "Finished Playing". Done this way to prevent multiple bindings.
		/// </summary>
		internal void SetEventHandler(FinishedPlayingHandler handler)
		{
			if (DonePlaying != null)
				return;
			
			DonePlaying += handler;
		}

		internal void Play()
		{	
			if ( _sound == null )
				return;
			
			_sound.Play();

            _playCount++;
        }

		internal void Resume()
		{
			if (_sound == null)
				return;			
#if IOS

			_sound.Play();
#else
			_sound.Resume();
#endif
		}
		
		internal void Pause()
		{			            
			if ( _sound == null )
				return;
			
			_sound.Pause();
        }
		
		internal void Stop()
		{
			if ( _sound == null )
				return;
			
			_sound.Stop();
			_playCount = 0;
		}

		internal float Volume
		{
			get
			{
				if (_sound != null)
					return _sound.Volume;
				else
					return 0.0f;
			}
			
			set
			{
				if ( _sound != null && _sound.Volume != value )
					_sound.Volume = value;
			}			
		}
#endif // !DIRECTX

        // Returns the duration of a song
        private TimeSpan? _Duration = null;
        public TimeSpan Duration
        {
            get
            {
                if (_Duration != null)
                    return _Duration.Value;
                else
                {
                    TimeSpan r = TimeSpan.Zero;
#if WINDOWS_MEDIA_ENGINE
                    if (MediaPlayer.State != MediaState.Stopped && MediaPlayer.Queue.ActiveSong.Name == Name && MediaPlayer._mediaEngineEx != null)
                    {
                        r = TimeSpan.FromSeconds(MediaPlayer._mediaEngineEx.Duration);
                    }
                    else
                    {
                        r = System.Threading.Tasks.Task.Run(() => _Duration_Get_Async().Result).Result;
                    }
#endif
                    //
                    if (r != TimeSpan.Zero)
                        _Duration = r;
                    //
                    return r;
                }
            }
        }

#if WINDOWS_MEDIA_ENGINE
        private static SharpDX.MediaFoundation.MediaEngine _mediaEngineEx;
        private async System.Threading.Tasks.Task<TimeSpan> _Duration_Get_Async()
        {
            return await System.Threading.Tasks.Task<TimeSpan>.Run(() =>
                {
                    TimeSpan r = TimeSpan.Zero;
                    if (_mediaEngineEx == null)
                    {
                        using (var factory = new SharpDX.MediaFoundation.MediaEngineClassFactory())
                        using (var attributes = new SharpDX.MediaFoundation.MediaEngineAttributes { AudioCategory = SharpDX.Multimedia.AudioStreamCategory.GameMedia })
                        {
                            var mediaEngine = new SharpDX.MediaFoundation.MediaEngine(factory, attributes, SharpDX.MediaFoundation.MediaEngineCreateFlags.AudioOnly);
                            _mediaEngineEx = mediaEngine.QueryInterface<SharpDX.MediaFoundation.MediaEngineEx>();
                        }
                    }
                    //
                    var folder = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                    var path = folder + "\\" + FilePath;
                    var uri = new Uri(path);
                    var converted = uri.AbsoluteUri;

                    _mediaEngineEx.Source = converted;
                    _mediaEngineEx.Load();
                    while (_mediaEngineEx.Error != null || _mediaEngineEx.ReadyState != (short)SharpDX.MediaFoundation.MediaEngineReady.HaveEnoughData) ;
                    if (!double.IsNaN(_mediaEngineEx.Duration))
                        r = TimeSpan.FromSeconds(_mediaEngineEx.Duration);
                    //
                    return r;
                });
        }
#endif
		
		// TODO: Implement
		public TimeSpan Position
        {
            get
            {
                return new TimeSpan(0);				
            }
        }

        public bool IsProtected
        {
            get
            {
				return false;
            }
        }

        public bool IsRated
        {
            get
            {
				return false;
            }
        }

        public string Name
        {
            get
            {
				return Path.GetFileNameWithoutExtension(_name);
            }
        }

        public int PlayCount
        {
            get
            {
				return _playCount;
            }
        }

        public int Rating
        {
            get
            {
				return 0;
            }
        }

        public int TrackNumber
        {
            get
            {
				return 0;
            }
        }
    }
}

