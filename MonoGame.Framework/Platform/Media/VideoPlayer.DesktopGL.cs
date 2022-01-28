#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2019 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class VideoPlayer : IDisposable
    {
        #region Hardware-accelerated YUV -> RGBA

        private Effect shaderProgram;
        private Texture2D[] yuvTextures = new Texture2D[3];

        VertexPositionTexture[] _vertexBuffer;
        short[] _indexBuffer;

        // Used to restore our previous GL state.

        private void GL_initialize()
        {
            // Load the YUV->RGBA Effect
            shaderProgram = new Effect(
                   currentDevice,
                   EffectResource.YUVToRGBAEffect.Bytecode
               );
        }

        private void GL_dispose()
        {
            if (currentDevice == null)
            {
                // We never initialized to begin with...
                return;
            }
            currentDevice = null;

            // Delete the Effect
            if (shaderProgram != null)
            {
                shaderProgram.Dispose();
            }

            // Delete the textures if they exist
            for (int i = 0; i < 3; i += 1)
            {
                if (yuvTextures[i] != null)
                {
                    yuvTextures[i].Dispose();
                }
            }
        }

        private void GL_setupTextures(int width, int height, int uvWidth, int uvHeight)
        {
            // Allocate YUV GL textures
            for (int i = 0; i < 3; i += 1)
            {
                if (yuvTextures[i] != null)
                {
                    yuvTextures[i].Dispose();
                }
            }
            yuvTextures[0] = new Texture2D(
                currentDevice,
                width,
                height,
                false,
                SurfaceFormat.Alpha8
            );
            yuvTextures[1] = new Texture2D(
                currentDevice,
                uvWidth,
                uvHeight,
                false,
                SurfaceFormat.Alpha8
            );
            yuvTextures[2] = new Texture2D(
                currentDevice,
                uvWidth,
                uvHeight,
                false,
                SurfaceFormat.Alpha8
            );

            _vertexBuffer = new VertexPositionTexture[4];
            _vertexBuffer[0] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(0, 0));
            _vertexBuffer[1] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
            _vertexBuffer[2] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
            _vertexBuffer[3] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1));

            _indexBuffer = new short[] { 0, 3, 2, 0, 1, 3 };  
        }

        #endregion

        #region Platform specific methods

        private void PlatformInitialize()
        {
            timer = new Stopwatch();
        }

        private void PlatformSetIsLooped()
        {
        }

        private void PlatformSetIsMuted()
        {
            UpdateVolume();
        }

        private TimeSpan PlatformGetPlayPosition()
        {
            return timer.Elapsed;
        }

        private void PlatformSetVolume()
        {
            UpdateVolume();
        }

        private void PlatformGetState(ref MediaState state)
        {
        }

        #endregion

        #region Private Member Data: XNA VideoPlayer Implementation

        // We use this to update our PlayPosition.
        private Stopwatch timer;

        // Store this to optimize things on our end.
        private RenderTarget2D videoTexture;

        // We need to access the GraphicsDevice frequently.
        private GraphicsDevice currentDevice;

        #endregion

        #region Private Member Data: Theorafile
        private IntPtr yuvData;
        private int currentFrame;

        // Todo: 1000 is a hack to handle sample rate conversion to ms (this var is 4096 * 2 in XNA FNA).
        // probably better to change the dynamicsoundeffectinstance to poll less frequently
        private const int AUDIO_BUFFER_SIZE = 4096 * 2 * 1000; 
        private static readonly float[] audioData = new float[AUDIO_BUFFER_SIZE];
        private static GCHandle audioHandle = GCHandle.Alloc(audioData, GCHandleType.Pinned);
        private IntPtr audioDataPtr = audioHandle.AddrOfPinnedObject();

        #endregion

        #region Private Member Data: Audio Stream

        private DynamicSoundEffectInstance audioStream;

        #endregion

        #region Private Methods: XNA VideoPlayer Implementation

        private void checkDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("VideoPlayer");
            }
        }

        #endregion

        #region Private Methods: Audio Stream

        private void UpdateVolume()
        {
            if (audioStream == null)
            {
                return;
            }
            if (IsMuted)
            {
                audioStream.Volume = 0.0f;
            }
            else
            {
                /* FIXME: Works around MasterVolume only temporarily!
				 * We need to detach this source from the AL listener properties.
				 * -flibit
				 */
                audioStream.Volume = Volume * (1.0f / SoundEffect.MasterVolume);
            }
        }

        #endregion

        #region Public Methods: XNA VideoPlayer Implementation

        private void PlatformDispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            // Stop the VideoPlayer. This gets almost everything...
            Stop();

            // Destroy the other GL bits.
            GL_dispose();

            // Dispose the DynamicSoundEffectInstance
            if (audioStream != null)
            {
                audioStream.Dispose();
                audioStream = null;
            }

            // Dispose the Texture.
            if (videoTexture != null)
            {
                videoTexture.Dispose();
            }

            // Free the YUV buffer
            if (yuvData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(yuvData);
                yuvData = IntPtr.Zero;
            }

            // Okay, we out.
            IsDisposed = true;
        }

        private Texture2D PlatformGetTexture()
        {
            checkDisposed();

            if (Video == null)
            {
                throw new InvalidOperationException();
            }

            // Be sure we can even get something from Theorafile...
            if (State == MediaState.Stopped ||
                Video.theora == IntPtr.Zero ||
                Theorafile.HasVideo(Video.theora) == 0)
            {
                // Screw it, give them the old one.
                return videoTexture;
            }

            int thisFrame = (int)(timer.Elapsed.TotalMilliseconds / (1000.0 / Video.FramesPerSecond));
            if (thisFrame > currentFrame)
            {
                // Only update the textures if we need to!
                if (Theorafile.ReadVideo(
                    Video.theora,
                    yuvData,
                    thisFrame - currentFrame
                ) == 1 || currentFrame == -1)
                {
                    UpdateTexture();
                }
                currentFrame = thisFrame;
            }

            // Check for the end...
            bool ended = Theorafile.EndOfStream(Video.theora) == 1;

            if (audioStream != null)
            {
                ended &= audioStream.PendingBufferCount == 0;
            }

            if (Video.UseElapsedTimeForStop && State == MediaState.Playing)
                ended |= timer.Elapsed.TotalMilliseconds > Video.Duration.TotalMilliseconds;

            if (ended)
            {
                // Stop and reset the timer. If we're looping, the loop will start it again.
                timer.Stop();
                timer.Reset();

                // Kill whatever audio/video we've got
                if (audioStream != null)
                {
                    audioStream.Stop();
                    audioStream.Dispose();
                    audioStream = null;
                }

                // Reset the stream no matter what happens next
                Theorafile.Reset(Video.theora);

                // If looping, go back to the start. Otherwise, we'll be exiting.
                if (IsLooped)
                {
                    // Starting over!
                    InitializeTheoraStream();

                    // Start! Again!
                    timer.Start();
                    if (audioStream != null)
                    {
                        audioStream.Play();
                    }
                }
                else
                {
                    // We out
                    Stop();
                }
            }

            // Finally.
            return videoTexture;
        }


        private void PlatformPlay()
        {
            checkDisposed();

            // Check the player state before attempting anything.
            if (State != MediaState.Stopped)
            {
                return;
            }

            // Carve out YUV buffer before doing any decoder work
            if (yuvData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(yuvData);
            }

            int yuvDataLen = (
                (Video.Width * Video.Height) +
                (Video.UvWidth * Video.UvHeight * 2)
            );
            yuvData = Marshal.AllocHGlobal(yuvDataLen);

            // Hook up the decoder to this player
            InitializeTheoraStream();

            // Set up the texture data
            if (Theorafile.HasVideo(Video.theora) == 1)
            {
                // The VideoPlayer will use the GraphicsDevice that is set now.
                GraphicsDevice _currentDevice = Game.Instance.GraphicsDevice;

                if (currentDevice != _currentDevice)
                {
                    GL_dispose();
                    currentDevice = _currentDevice;
                    GL_initialize();
                }

                RenderTarget2D overlap = videoTexture;
                videoTexture = new RenderTarget2D(
                        currentDevice,
                        Video.Width,
                        Video.Height,
                        false,
                        SurfaceFormat.Color,
                        DepthFormat.None,
                        0,
                        RenderTargetUsage.PreserveContents
                );

                if (overlap != null)
                {
                    overlap.Dispose();
                }
                GL_setupTextures(
                    Video.Width,
                    Video.Height,
                    Video.UvWidth,
                    Video.UvHeight
                );
            }

            // The player can finally start now!
            timer.Start();
            if (audioStream != null)
            {
                audioStream.Play();
            }
        }

        private void PlatformStop()
        {
            checkDisposed();

            // Check the player state before attempting anything.
            if (State == MediaState.Stopped)
            {
                return;
            }

            // Wait for the player to end if it's still going.
            timer.Stop();
            timer.Reset();
            if (audioStream != null)
            {
                audioStream.Stop();
                audioStream.Dispose();
                audioStream = null;
            }
            Theorafile.Reset(Video.theora);
        }

        private void PlatformPause()
        {
            checkDisposed();

            // Check the player state before attempting anything.
            if (State != MediaState.Playing)
            {
                return;
            }

            // Pause timer, audio.
            timer.Stop();
            if (audioStream != null)
            {
                audioStream.Pause();
            }
        }

        private void PlatformResume()
        {
            checkDisposed();

            // Check the player state before attempting anything.
            if (State != MediaState.Paused)
            {
                return;
            }

            // Unpause timer, audio.
            timer.Start();
            if (audioStream != null)
            {
                audioStream.Resume();
            }
        }

        #endregion

        #region Private Theora Audio Stream Methods

        private void OnBufferRequest(object sender, EventArgs args)
        {
            int samples = Theorafile.ReadAudio(
                Video.theora,
                audioDataPtr,
                AUDIO_BUFFER_SIZE
            );
            if (samples > 0)
            {
                audioStream.SubmitFloatBuffer(audioDataPtr, audioData, 0);
            }
            else if (Theorafile.EndOfStream(Video.theora) == 1)
            {
                // Okay, we ran out. No need for this!
                audioStream.BufferNeeded -= OnBufferRequest;
            }
        }

        #endregion

        #region Private Theora Video Stream Methods

        private void UpdateTexture()
        {
            // Prepare YUV GL textures with our current frame data
            yuvTextures[0].SetDataFromPointer(yuvData);

            var yuvData1 = new IntPtr(yuvData.ToInt64() + (Video.Width * Video.Height));
            yuvTextures[1].SetDataFromPointer(yuvData1);

            var yuvData2 = new IntPtr(yuvData1.ToInt64() + (Video.UvWidth * Video.UvHeight));
            yuvTextures[2].SetDataFromPointer(yuvData2);

            //write yuv textures to the shader
            shaderProgram.Parameters["yuvx"].SetValue(yuvTextures[0]);
            shaderProgram.Parameters["yuvy"].SetValue(yuvTextures[1]);
            shaderProgram.Parameters["yuvz"].SetValue(yuvTextures[2]);

            //run the shader
            currentDevice.SetRenderTarget(videoTexture);
            currentDevice.Clear(Color.Transparent);

            shaderProgram.CurrentTechnique.Passes[0].Apply();

            currentDevice.DrawUserIndexedPrimitives
                (PrimitiveType.TriangleList, _vertexBuffer, 0, 4, _indexBuffer, 0, 2);

            //restore rt
            currentDevice.SetRenderTarget(null);
        }

        #endregion

        #region Theora Decoder Hookup Method

        private void InitializeTheoraStream()
        {
            // Grab the first video frame ASAP.
            while (Theorafile.ReadVideo(Video.theora, yuvData, 1) == 0) ;

            // Grab the first bit of audio. We're trying to start the decoding ASAP.
            if (Theorafile.HasAudio(Video.theora) == 1)
            {
                int channels, samplerate;
                Theorafile.AudioInfo(Video.theora, out channels, out samplerate);
                audioStream = new DynamicSoundEffectInstance(
                    samplerate,
                    (AudioChannels)channels
                );
                audioStream.BufferNeeded += OnBufferRequest;
                UpdateVolume();

                // Fill and queue the buffers.
                for (int i = 0; i < 4; i += 1)
                {
                    OnBufferRequest(audioStream, EventArgs.Empty);
                    if (audioStream.PendingBufferCount == i)
                    {
                        break;
                    }
                }
            }

            currentFrame = -1;
        }

        #endregion
    }
}
