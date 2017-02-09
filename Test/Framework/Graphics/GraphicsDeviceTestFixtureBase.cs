﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Tests.Components;
using MonoGame.Tests.Utilities;
using NUnit.Framework;

namespace MonoGame.Tests.Graphics
{
    [SetUpFixture]
    internal class GraphicsDeviceSetUpFixture
    {
        public static bool Initialized { get; private set; }
        public static TestGameBase Game { get; private set; }
        public static GraphicsDevice GraphicsDevice { get; private set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        public static bool NeedsHardReset { get; private set; }

        public GraphicsDeviceSetUpFixture()
        {
            NeedsHardReset = true;
        }

        [TearDown]
        public void RunAfterAllTests()
        {
            Game.Dispose();
        }

        public static void ResetAsNeeded()
        {
            if (!Initialized || GraphicsDevice.IsDisposed)
                NeedsHardReset = true;

            if (NeedsHardReset)
            {
                HardReset();
                NeedsHardReset = false;
            }
            else
            {
                SoftReset();
            }
        }

        public static void HardReset()
        {
            if (Game != null)
            {
                Game.Content.Unload();
                Game.Dispose();
            }

            Game = new TestGameBase();
            GraphicsDeviceManager = new GraphicsDeviceManager(Game);
            // some visual tests require a HiDef profile so we default to that
            GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
            ((IGraphicsDeviceManager) Game.Services.GetService(typeof(IGraphicsDeviceManager))).CreateDevice();
            GraphicsDevice = Game.GraphicsDevice;
            Initialized = true;

            GraphicsDeviceManager.DeviceReset += SetNeedsHardReset;
            GraphicsDevice.DeviceReset += SetNeedsHardReset;
        }

        private static void SetNeedsHardReset(object sender, EventArgs e)
        {
            NeedsHardReset = true;
        }

        public static void SoftReset()
        {
            Game.Content.Unload();
            ResetGame();
            ResetGd();
            ResetGdm();
        }

        private static void ResetGame()
        {
            Game.ExitCondition = null;
            Game.SuppressExtraUpdatesAndDraws = false;
            Game.ClearActions();
            Game.Components.Clear();
            // gameTime is only initialized when a game is run
            try { Game.ResetElapsedTime(); } catch { }
        }

        private static void ResetGd()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            try
            {
                for (var i = 0; i < 16; i++)
                GraphicsDevice.SamplerStates[i] = SamplerState.LinearWrap;
            }
            catch { }
            try
                {
                    for (var i = 0; i < 16; i++)
                GraphicsDevice.Textures[i] = null;
            }
            catch { }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Indices = null;
            GraphicsDevice.SetVertexBuffer(null);
            //GraphicsDevice.Reset();
        }

        private static void ResetGdm()
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsDeviceManager.DefaultBackBufferWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsDeviceManager.DefaultBackBufferHeight;
            GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.PreferMultiSampling = false;
            GraphicsDeviceManager.PreferredBackBufferFormat = SurfaceFormat.Color;
            GraphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24;
#if !XNA
            GraphicsDeviceManager.HardwareModeSwitch = true;
#endif
        }
    }

    internal class GraphicsDeviceTestFixtureBase
    {
        protected TestGameBase game;
        protected GraphicsDeviceManager gdm;
        protected GraphicsDevice gd;
        protected ContentManager content;

        #region Frame capturing management

        private bool _framePrepared;
        private bool _frameSubmitted;
        private bool _framesChecked;
        private RenderTarget2D _captureRenderTarget;
        private List<FramePixelData> _submittedFrames;
        private int _totalFramesExpected;
        private readonly IFrameComparer _frameComparer = new PixelDeltaFrameComparer();
        private readonly ActionDaemon _writerThread = new ActionDaemon();

        #endregion

        #region Frame capturing settings

        protected float Similarity;
        protected WriteSettings WriteCapture;
        protected WriteSettings WriteDiffs;
        protected bool ExactNumberSubmits;
        protected Color ClearColor;

        protected Rectangle? CaptureRegion;

        #endregion

        #region SetUp and TearDown

        [SetUp]
        public virtual void SetUp()
        {
            GraphicsDeviceSetUpFixture.ResetAsNeeded();

            game = GraphicsDeviceSetUpFixture.Game;
            gdm = GraphicsDeviceSetUpFixture.GraphicsDeviceManager;

            gd = game.GraphicsDevice;
            content = game.Content;

            _framePrepared = false;
            _frameSubmitted = false;
            _framesChecked = false;

            Similarity = Constants.StandardRequiredSimilarity;
            WriteCapture = WriteSettings.Always;
            WriteDiffs = WriteSettings.WhenFailed;
            ExactNumberSubmits = false;
            ClearColor = Color.CornflowerBlue;
            CaptureRegion = null;

            Paths.SetStandardWorkingDirectory();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (!object.ReferenceEquals(game, GraphicsDeviceSetUpFixture.Game))
                game.Dispose();

            if (_framePrepared && !_framesChecked)
                Assert.Fail("Initialized fixture for rendering but did not check frames.");
        }

        #endregion

        #region Utility Methods

        protected void HardReset()
        {
            GraphicsDeviceSetUpFixture.HardReset();
            game = GraphicsDeviceSetUpFixture.Game;
            gdm = GraphicsDeviceSetUpFixture.GraphicsDeviceManager;
            gd = GraphicsDeviceSetUpFixture.GraphicsDevice;
            content = game.Content;
        }

        protected void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        /// <summary>
        /// Simulate a game loop.
        /// </summary>
        /// <param name="action">The method to execute in the loop, gets the frame number passed to it.</param>
        /// <param name="stopCondition">If this is true the loop will end, gets the frame number passed to it.</param>
        /// <param name="frameTime">Time in ms to sleep after a frame.</param>
        protected void DoGameLoop(Action<int> action, Predicate<int> stopCondition, int frameTime = 16)
        {
            var frame = 0;
            while (!stopCondition(frame))
            {
                action(frame);

                Sleep(frameTime);
                frame++;
            }
        }

        #endregion

        #region Frame capture API

        protected void PrepareFrameCapture(int expected = 1)
        {
            if (_framePrepared)
                throw new Exception("PrepareFrameCapture should only be called once.");
            _framePrepared = true;
            _totalFramesExpected = expected;

            var rect = CaptureRegion ?? new Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height);

			_captureRenderTarget = new RenderTarget2D(
				gd, rect.Width, rect.Height,
				false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _submittedFrames = new List<FramePixelData>();

			gd.SetRenderTarget(_captureRenderTarget);
            gd.Clear(ClearColor);
        }

        protected void SubmitFrame()
        {
            if (!_framePrepared)
                throw new Exception("PrepareFrameCapture must be called before rendering when submitting a frame.");
            _frameSubmitted = true;

            // release the rendertarget so GetData does not fail
            gd.SetRenderTarget(null);

            var capturedFrame = _captureRenderTarget.ToPixelData();
            _submittedFrames.Add(capturedFrame);

            gd.SetRenderTarget(_captureRenderTarget);
        }

        protected void CheckFrames() {
            if (!_framePrepared)
                throw new Exception("PrepareFrameCapture must be called before rendering to be able to check frames.");

            // submit the current frame if one is prepared, but none are submitted yet
            if (!_frameSubmitted)
                SubmitFrame();

			var folderName = TestContext.CurrentContext.GetTestFolderName();
            var referenceImageDirectory = Paths.ReferenceImage(folderName);
            var outputDirectory = Paths.CapturedFrame(folderName);
            var fileName = TestContext.CurrentContext.GetTestFrameFileNameFormat(_totalFramesExpected);
            var capturedImagePath = Path.Combine(outputDirectory, fileName);
            var referenceImagePath = Path.Combine(referenceImageDirectory, fileName);
            
            var allResults = new List<FrameComparisonResult>();
            var failedResults = new List<FrameComparisonResult>();
            var noReference = new List<string>();

            for (var i = 0; i < _submittedFrames.Count; i++)
            {
                var frame = _submittedFrames[i];

                var capturedPath = string.Format(capturedImagePath, i + 1);
                var referencePath = string.Format(referenceImagePath, i + 1);

                if (!File.Exists(referencePath))
                {
                    // no reference frame is available, so just write the image and track the failure
                    if (WriteCapture == WriteSettings.Always || WriteCapture == WriteSettings.WhenFailed)
                    {
                        Directory.CreateDirectory(outputDirectory);
                        _writerThread.AddAction(() =>
                            frame.Save(capturedPath));
                    }
                    noReference.Add(referencePath);
                    continue;
                }

                var refFrame = FramePixelData.FromFile(referencePath);
                var frameSimilarity = _frameComparer.Compare(frame, refFrame);

                var failed = frameSimilarity < Similarity;

                var writeCapture = WriteCapture == WriteSettings.Always ||
                                  (WriteCapture == WriteSettings.WhenFailed && failed);
                var writeDiff = WriteDiffs == WriteSettings.Always ||
                               (WriteDiffs == WriteSettings.WhenFailed && failed);

                var result = new FrameComparisonResult(frameSimilarity, frame, refFrame, 
                    capturedPath, referencePath, failed, writeCapture, writeDiff);

                allResults.Add(result);
                if (failed) failedResults.Add(result);

                if (result.SaveImage)
                {
                    Directory.CreateDirectory(outputDirectory);
                    _writerThread.AddAction(() =>
                        result.CapturedData.Save(result.CapturedImagePath));
                }

                if (result.SaveDiff)
                {
                    var name = string.Format(fileName, i + 1);
                    var path = GetDiffPath(name);
                    result.DiffPath = path;
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    _writerThread.AddAction(() =>
                        WriteDiff(result.CapturedData, result.ReferenceData, path));
                }
            }

            _framesChecked = true;

            // write results to console
            WriteComparisonResultReport(allResults, noReference);

            // now do the actual assertions
            if (ExactNumberSubmits && _totalFramesExpected != allResults.Count)
            {
				Assert.Fail (
					"Expected {0} frame comparison result(s), but found {1}",
					_totalFramesExpected, allResults.Count);
            }

            if (failedResults.Count > 0)
            {
                Assert.Fail(
					"{0} of {1} frames failed the similarity test.",
					failedResults.Count, allResults.Count);
            }

            if (noReference.Count > 0)
            {
                Assert.Fail(
                    "Did not find reference image(s): " + noReference.Aggregate((s1, s2) => s1 + ", " + s2));
            }
        }

        #endregion

        #region ComparisonResults

        private void WriteComparisonResultReport(IEnumerable<FrameComparisonResult> results, List<string> noReference)
		{
			Console.WriteLine ("Required similarity: {0:0.####}", Similarity);
		    foreach (var result in results)
		    {
		        var captureString = result.SaveImage ? ", Capture: " + result.CapturedImagePath : "";
		        var referenceString = ", Reference: " + result.ReferenceImagePath;
                var diffString = result.SaveDiff ? ", Diff: " + result.DiffPath : "";
		        Console.WriteLine(
		            "Similarity: {0:0.####}{1}{2}{3}",
		            result.Similarity, captureString, referenceString, diffString);
		    }
		}

        protected class FrameComparisonResult
        {
            public float Similarity;
            public FramePixelData CapturedData;
            public FramePixelData ReferenceData;
            public string CapturedImagePath;
            public string ReferenceImagePath;
            public string DiffPath;
            public bool Failed;
            public bool SaveImage;
            public bool SaveDiff;

            public FrameComparisonResult(float similarity, FramePixelData captured, FramePixelData reference,
                string capturedImagePath, string referenceImagePath, bool failed, bool saveImage, bool saveDiff)
            {
                Similarity = similarity;
                CapturedData = captured;
                ReferenceData = reference;
                CapturedImagePath = capturedImagePath;
                ReferenceImagePath = referenceImagePath;
                Failed = failed;
                SaveImage = saveImage;
                SaveDiff = saveDiff;
            }
        }

        #endregion

        #region Diff

        private string GetDiffPath(string name)
        {
			var folderName = TestContext.CurrentContext.GetTestFolderName();
            var directory = Paths.CapturedFrameDiff(folderName);
            var diffFileName = string.Format("diff-{0}", name);
            return Path.Combine (directory, diffFileName);
        }

        private void WriteDiff(FramePixelData capture, FramePixelData reference, string outputPath)
        {
            var diff = CreateDiff(capture, reference);
            Normalize(diff);
            diff.Save(outputPath);
        }
        
        private static FramePixelData CreateDiff (FramePixelData a, FramePixelData b)
		{
			int minWidth, maxWidth, minHeight, maxHeight;

			MathUtility.MinMax (a.Width, b.Width, out minWidth, out maxWidth);
			MathUtility.MinMax (a.Height, b.Height, out minHeight, out maxHeight);

			var diff = new FramePixelData (maxWidth, maxHeight);

			for (var y = 0; y < minHeight; ++y) {

				var indexA = y * a.Width;
				var indexB = y * b.Width;
				var indexDiff = y * diff.Width;

				for (var x = 0; x < minWidth; ++x) {
					// Ignore alpha.  If alpha diffs are
					// needed, a special strategy will have
					// to be devised, since XOR'ing two
					// opaque pixels will cause a totally
					// transparent pixel and hide any other
					// difference.
					diff.Data [indexDiff] = new Color (
						(byte) (a.Data [indexA].R ^ b.Data [indexB].R),
						(byte) (a.Data [indexA].G ^ b.Data [indexB].G),
						(byte) (a.Data [indexA].B ^ b.Data [indexB].B));

					indexA++;
					indexB++;
					indexDiff++;
				}
			}

			return diff;
		}

		private static void Normalize (FramePixelData frame)
		{
			var max = new Color(0, 0, 0, 0);
			foreach (var pixel in frame.Data) {
				max.B = Math.Max (pixel.B, max.B);
				max.G = Math.Max (pixel.G, max.G);
				max.R = Math.Max (pixel.R, max.R);
				max.A = Math.Max (pixel.A, max.A);
			}

			if (max.B == 0) max.B = 255;
			if (max.G == 0) max.G = 255;
			if (max.R == 0) max.R = 255;
			if (max.A == 0) max.A = 255;

			for (var i = 0; i < frame.Data.Length; ++i) {
				var pixel = frame.Data[i];


				pixel.B = (byte)(pixel.B * 255 / max.B);
				pixel.G = (byte)(pixel.G * 255 / max.G);
				pixel.R = (byte)(pixel.R * 255 / max.R);
				pixel.A = (byte)(pixel.A * 255 / max.A);

				frame.Data[i] = pixel;
			}
		}

        #endregion

        protected enum WriteSettings
        {
            Never,
            Always,
            WhenFailed,
        }
    }
}