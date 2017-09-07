// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Used to initialize and control the presentation of the graphics device.
    /// </summary>
    public partial class GraphicsDeviceManager : IGraphicsDeviceService, IDisposable, IGraphicsDeviceManager
    {
        private readonly Game _game;
        private GraphicsDevice _graphicsDevice;
        private bool _initialized = false;

        private int _preferredBackBufferHeight;
        private int _preferredBackBufferWidth;
        private SurfaceFormat _preferredBackBufferFormat;
        private DepthFormat _preferredDepthStencilFormat;
        private bool _preferMultiSampling;
        private DisplayOrientation _supportedOrientations;
        private bool _synchronizedWithVerticalRetrace = true;
        private bool _drawBegun;
        private bool _disposed;
        private bool _hardwareModeSwitch = true;
        private bool _wantFullScreen;
        private GraphicsProfile _graphicsProfile;
        // dirty flag for ApplyChanges
        private bool _shouldApplyChanges;

        /// <summary>
        /// The default back buffer width.
        /// </summary>
        public static readonly int DefaultBackBufferWidth = 800;

        /// <summary>
        /// The default back buffer height.
        /// </summary>
        public static readonly int DefaultBackBufferHeight = 480;

        /// <summary>
        /// Optional override for platform specific defaults.
        /// </summary>
        partial void PlatformConstruct();

        /// <summary>
        /// Associates this graphics device manager to a game instances.
        /// </summary>
        /// <param name="game">The game instance to attach.</param>
        public GraphicsDeviceManager(Game game)
        {
            if (game == null)
                throw new ArgumentNullException("game", "Game cannot be null.");

            _game = game;

            _supportedOrientations = DisplayOrientation.Default;
            _preferredBackBufferFormat = SurfaceFormat.Color;
            _preferredDepthStencilFormat = DepthFormat.Depth24;
            _synchronizedWithVerticalRetrace = true;

            // Assume the window client size as the default back 
            // buffer resolution in the landscape orientation.
            var clientBounds = _game.Window.ClientBounds;
            if (clientBounds.Width >= clientBounds.Height)
            {
                _preferredBackBufferWidth = clientBounds.Width;
                _preferredBackBufferHeight = clientBounds.Height;
            }
            else
            {
                _preferredBackBufferWidth = clientBounds.Height;
                _preferredBackBufferHeight = clientBounds.Width;
            }

            // Default to windowed mode... this is ignored on platforms that don't support it.
            _wantFullScreen = false;

            // XNA would read this from the manifest, but it would always default
            // to reach unless changed.  So lets mimic that without the manifest bit.
            GraphicsProfile = GraphicsProfile.Reach;

            // Let the plaform optionally overload construction defaults.
            PlatformConstruct();

            if (_game.Services.GetService(typeof(IGraphicsDeviceManager)) != null)
                throw new ArgumentException("A graphics device manager is already registered.  The graphics device manager cannot be changed once it is set.");
            _game.graphicsDeviceManager = this;

            _game.Services.AddService(typeof(IGraphicsDeviceManager), this);
            _game.Services.AddService(typeof(IGraphicsDeviceService), this);
        }

        ~GraphicsDeviceManager()
        {
            Dispose(false);
        }

        private void CreateDevice()
        {
            if (_graphicsDevice != null)
                return;

            try
            {
                if (!_initialized)
                    Initialize();

                var gdi = DoPreparingDeviceSettings();
                CreateDevice(gdi);
            }
            catch (NoSuitableGraphicsDeviceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NoSuitableGraphicsDeviceException("Failed to create graphics device!", ex);
            }
        }

        private void CreateDevice(GraphicsDeviceInformation gdi)
        {
            if (_graphicsDevice != null)
                return;

            _graphicsDevice = new GraphicsDevice(gdi);
            _shouldApplyChanges = false;

            // hook up reset events
            GraphicsDevice.DeviceReset     += (sender, args) => OnDeviceReset(args);
            GraphicsDevice.DeviceResetting += (sender, args) => OnDeviceResetting(args);

            // update the touchpanel display size when the graphicsdevice is reset
            _graphicsDevice.DeviceReset += UpdateTouchPanel;
            _graphicsDevice.PresentationChanged += OnPresentationChanged;

            OnDeviceCreated(EventArgs.Empty);
        }

        void IGraphicsDeviceManager.CreateDevice()
        {
            CreateDevice();
        }

        public bool BeginDraw()
        {
            if (_graphicsDevice == null)
                return false;

            _drawBegun = true;
            return true;
        }

        public void EndDraw()
        {
            if (_graphicsDevice != null && _drawBegun)
            {
                _drawBegun = false;
                _graphicsDevice.Present();
            }
        }

        #region IGraphicsDeviceService Members

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
        public event EventHandler<PreparingDeviceSettingsEventArgs> PreparingDeviceSettings;
        public event EventHandler<EventArgs> Disposed;

        protected void OnDeviceDisposing(EventArgs e)
        {
            EventHelpers.Raise(this, DeviceDisposing, e);
        }

        protected void OnDeviceResetting(EventArgs e)
        {
            EventHelpers.Raise(this, DeviceResetting, e);
        }

        internal void OnDeviceReset(EventArgs e)
        {
            EventHelpers.Raise(this, DeviceReset, e);
        }

        internal void OnDeviceCreated(EventArgs e)
        {
            EventHelpers.Raise(this, DeviceCreated, e);
        }

        /// <summary>
        /// This populates a GraphicsDeviceInformation instance and invokes PreparingDeviceSettings to
        /// allow users to change the settings. Then returns that GraphicsDeviceInformation.
        /// Throws NullReferenceException if users set GraphicsDeviceInformation.PresentationParameters to null.
        /// </summary>
        private GraphicsDeviceInformation DoPreparingDeviceSettings()
        {
            var gdi = FindBestDevice(true);
            var preparingDeviceSettingsHandler = PreparingDeviceSettings;

            if (preparingDeviceSettingsHandler != null)
            {
                // this allows users to overwrite settings through the argument
                var args = new PreparingDeviceSettingsEventArgs(gdi);
                preparingDeviceSettingsHandler(this, args);

                if (gdi.PresentationParameters == null || gdi.Adapter == null)
                    throw new NullReferenceException("Members should not be set to null in PreparingDeviceSettingsEventArgs");
            }

            return gdi;
        }

        #endregion

        #region IDisposable Members

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
                    if (_graphicsDevice != null)
                    {
                        _graphicsDevice.Dispose();
                        _graphicsDevice = null;
                    }
                }
                _disposed = true;
                EventHelpers.Raise(this, Disposed, EventArgs.Empty);
            }
        }

        #endregion

        partial void PlatformApplyChanges();

        partial void PlatformPreparePresentationParameters(PresentationParameters presentationParameters);

        private void PreparePresentationParameters(PresentationParameters presentationParameters)
        {
            presentationParameters.BackBufferFormat = _preferredBackBufferFormat;
            presentationParameters.BackBufferWidth = _preferredBackBufferWidth;
            presentationParameters.BackBufferHeight = _preferredBackBufferHeight;
            presentationParameters.DepthStencilFormat = _preferredDepthStencilFormat;
            presentationParameters.IsFullScreen = _wantFullScreen;
            presentationParameters.HardwareModeSwitch = _hardwareModeSwitch;
            presentationParameters.PresentationInterval = _synchronizedWithVerticalRetrace ? PresentInterval.One : PresentInterval.Immediate;
            presentationParameters.DisplayOrientation = _game.Window.CurrentOrientation;
            presentationParameters.DeviceWindowHandle = _game.Window.Handle;

            if (_preferMultiSampling)
            {
                // always initialize MultiSampleCount to the maximum, if users want to overwrite
                // this they have to respond to the PreparingDeviceSettingsEvent and modify
                // args.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount
                presentationParameters.MultiSampleCount = GraphicsDevice != null
                    ? GraphicsDevice.GraphicsCapabilities.MaxMultiSampleCount
                    : 32;
            }
            else
            {
                presentationParameters.MultiSampleCount = 0;
            }

            PlatformPreparePresentationParameters(presentationParameters);
        }

        /// <summary>
        /// Finds the best device configuration that is compatible with the current device preferences.
        /// </summary>
        /// <param name="anySuitableDevice">If true, the best device configuration can be selected from any
        /// available adaptor. If false, only the current adaptor is used.</param>
        /// <returns>The best device configuration that could be found.</returns>
        protected virtual GraphicsDeviceInformation FindBestDevice(bool anySuitableDevice)
        {
            // Create a list of available devices
            var devices = new List<GraphicsDeviceInformation>();
            if (anySuitableDevice)
            {
                foreach (var adapter in GraphicsAdapter.Adapters)
                {
                    if (adapter.IsProfileSupported(GraphicsProfile))
                        AddModes(adapter, devices);
                }
            }
            else
            {
                var adapter = GraphicsAdapter.DefaultAdapter;
                if (adapter.IsProfileSupported(GraphicsProfile))
                    AddModes(adapter, devices);
            }

            // Rank them to get the most preferred device first
            RankDevices(devices);

            // No devices left in the list?
            if (devices.Count == 0)
            {
                throw new NoSuitableGraphicsDeviceException(FrameworkResources.CouldNotFindCompatibleGraphicsDevice);
            }

            // The first device in the list is the most suitable
            return devices[0];
        }

        // Add all modes supported by an adapter to the list
        void AddModes(GraphicsAdapter adapter, List<GraphicsDeviceInformation> devices)
        {
            int multiSampleCount = 0;
            if (_preferMultiSampling)
            {
                // Always initialize MultiSampleCount to the maximum. If users want to overwrite
                // this they have to respond to the PreparingDeviceSettingsEvent and modify
                // args.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount
                multiSampleCount = GraphicsDevice != null ? GraphicsDevice.GraphicsCapabilities.MaxMultiSampleCount : 32;
            }

            if (IsFullScreen)
            {
                // Fullscreen mode adds every supported display mode by each adaptor
                foreach (var mode in adapter.SupportedDisplayModes)
                {
                    var gdi = new GraphicsDeviceInformation()
                    {
                        Adapter = adapter,
                        GraphicsProfile = GraphicsProfile,
                        PresentationParameters = new PresentationParameters()
                        {
                            BackBufferFormat = mode.Format,
                            BackBufferHeight = mode.Height,
                            BackBufferWidth = mode.Width,
                            DepthStencilFormat = PreferredDepthStencilFormat,
                            IsFullScreen = IsFullScreen,
                            HardwareModeSwitch = _hardwareModeSwitch,
                            PresentationInterval = _synchronizedWithVerticalRetrace ? PresentInterval.One : PresentInterval.Immediate,
                            DisplayOrientation = _game.Window.CurrentOrientation,
                            DeviceWindowHandle = _game.Window.Handle,
                            MultiSampleCount = multiSampleCount
                        }
                    };
                    PlatformPreparePresentationParameters(gdi.PresentationParameters);
                    devices.Add(gdi);
                }
            }
            else
            {
                // Windowed mode only adds an entry for the requested window size
                var gdi = new GraphicsDeviceInformation()
                {
                    Adapter = adapter,
                    GraphicsProfile = GraphicsProfile,
                    PresentationParameters = new PresentationParameters()
                    {
                        BackBufferFormat = PreferredBackBufferFormat,
                        BackBufferHeight = PreferredBackBufferHeight,
                        BackBufferWidth = PreferredBackBufferWidth,
                        DepthStencilFormat = PreferredDepthStencilFormat,
                        IsFullScreen = IsFullScreen,
                        HardwareModeSwitch = _hardwareModeSwitch,
                        PresentationInterval = _synchronizedWithVerticalRetrace ? PresentInterval.One : PresentInterval.Immediate,
                        DisplayOrientation = _game.Window.CurrentOrientation,
                        DeviceWindowHandle = _game.Window.Handle,
                        MultiSampleCount = multiSampleCount
                    }
                };
                PlatformPreparePresentationParameters(gdi.PresentationParameters);
                devices.Add(gdi);
            }
        }

        int CompareGraphicsDeviceInformation(GraphicsDeviceInformation left, GraphicsDeviceInformation right)
        {
            var leftPP = left.PresentationParameters;
            var rightPP = right.PresentationParameters;
            var leftAdapter = left.Adapter;
            var rightAdapter = right.Adapter;

            // Prefer a higher graphics profile
            if (left.GraphicsProfile != right.GraphicsProfile)
                return left.GraphicsProfile > right.GraphicsProfile ? -1 : 1;

            // Prefer windowed/fullscreen based on IsFullScreen
            if (leftPP.IsFullScreen != rightPP.IsFullScreen)
                return IsFullScreen == leftPP.IsFullScreen ? -1 : 1;

            // BackBufferFormat
            if (leftPP.BackBufferFormat != rightPP.BackBufferFormat)
            {
                var preferredSize = PreferredBackBufferFormat.GetSize();
                var leftRank = leftPP.BackBufferFormat.GetSize() == preferredSize ? 0 : 1;
                var rightRank = rightPP.BackBufferFormat.GetSize() == preferredSize ? 0 : 1;
                if (leftRank != rightRank)
                    return leftRank < rightRank ? -1 : 1;
            }

            // MultiSampleCount
            if (leftPP.MultiSampleCount != rightPP.MultiSampleCount)
                return leftPP.MultiSampleCount > rightPP.MultiSampleCount ? -1 : 1;

            // Resolution
            int leftWidthDiff = Math.Abs(leftPP.BackBufferWidth - PreferredBackBufferWidth);
            int leftHeightDiff = Math.Abs(leftPP.BackBufferHeight - PreferredBackBufferHeight);
            int rightWidthDiff = Math.Abs(rightPP.BackBufferWidth - PreferredBackBufferWidth);
            int rightHeightDiff = Math.Abs(rightPP.BackBufferHeight - PreferredBackBufferHeight);
            if (leftHeightDiff != rightHeightDiff)
                return leftHeightDiff < rightHeightDiff ? -1 : 1;
            if (leftWidthDiff != rightWidthDiff)
                return leftWidthDiff < rightWidthDiff ? -1 : 1;

            // Aspect ratio
            var targetAspectRatio = (float)PreferredBackBufferWidth / (float)PreferredBackBufferHeight;
            var leftAspectRatio = (float)leftPP.BackBufferWidth / (float)leftPP.BackBufferHeight;
            var rightAspectRatio = (float)rightPP.BackBufferWidth / (float)rightPP.BackBufferHeight;
            if (Math.Abs(leftAspectRatio - rightAspectRatio) > 0.1f)
                return Math.Abs(leftAspectRatio - targetAspectRatio) < Math.Abs(rightAspectRatio - targetAspectRatio) ? -1 : 1;

            // Default adapter first
            if (leftAdapter.IsDefaultAdapter != rightAdapter.IsDefaultAdapter)
                return leftAdapter.IsDefaultAdapter ? -1 : 1;

            return 0;
        }

        /// <summary>
        /// Orders the supplied devices based on the current preferences.
        /// </summary>
        /// <param name="foundDevices">The list of devices to rank.</param>
        /// <remarks>
        /// The list of devices is sorted so that devices earlier in the list are preferred over devices
        /// later in the list. Devices may be removed from the list if they do not satisfy the criteria.
        /// </remarks>
        protected virtual void RankDevices(List<GraphicsDeviceInformation> foundDevices)
        {
            // Filter out any unsuitable graphics profiles. Hopefully there shouldn't be many to remove
            for (int i = foundDevices.Count - 1; i >= 0; --i)
            {
                if (foundDevices[i].GraphicsProfile > GraphicsProfile)
                    foundDevices.RemoveAt(i);
            }

            foundDevices.Sort(CompareGraphicsDeviceInformation);
        }

        /// <summary>
        /// Applies any pending property changes to the graphics device.
        /// </summary>
        public void ApplyChanges()
        {
            // The GraphicsDeviceManager must be registered with the Game.Services container
            if (this != _game.Services.GetService<IGraphicsDeviceManager>())
            {
                throw new InvalidOperationException(FrameworkResources.GraphicsDeviceManagerNotRegistered);
            }

            // If the device hasn't been created then create it now.
            if (_graphicsDevice == null)
                CreateDevice();

            if (!_shouldApplyChanges)
                return;

            _shouldApplyChanges = false;

            _game.Window.SetSupportedOrientations(_supportedOrientations);

            // Allow for optional platform specific behavior.
            PlatformApplyChanges();

            // Populates a GraphicsDeviceInformation with settings in this GraphicsDeviceManager and allows users to
            // override them with PrepareDeviceSettings event. This information should be applied to the GraphicsDevice
            var gdi = DoPreparingDeviceSettings();

            if (gdi.GraphicsProfile != GraphicsDevice.GraphicsProfile)
            {
                // If the GraphicsProfile changed we need to create a new GraphicsDevice
                DisposeGraphicsDevice();
                CreateDevice(gdi);
                return;
            }

            GraphicsDevice.Reset(gdi.PresentationParameters);
        }

        private void DisposeGraphicsDevice()
        {
            _graphicsDevice.Dispose();
            EventHelpers.Raise(this, DeviceDisposing, EventArgs.Empty);
            _graphicsDevice = null;
        }

        partial void PlatformInitialize(PresentationParameters presentationParameters);

        private void Initialize()
        {
            _game.Window.SetSupportedOrientations(_supportedOrientations);

            var presentationParameters = new PresentationParameters();
            PreparePresentationParameters(presentationParameters);

            // Allow for any per-platform changes to the presentation.
            PlatformInitialize(presentationParameters);

            _initialized = true;
        }

        private void UpdateTouchPanel(object sender, EventArgs eventArgs)
        {
            TouchPanel.DisplayWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            TouchPanel.DisplayHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;
            TouchPanel.DisplayOrientation = _graphicsDevice.PresentationParameters.DisplayOrientation;
        }

        /// <summary>
        /// Toggles between windowed and fullscreen modes.
        /// </summary>
        /// <remarks>
        /// Note that on platforms that do not support windowed modes this has no affect.
        /// </remarks>
        public void ToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;
            ApplyChanges();
        }

        private void OnPresentationChanged(object sender, PresentationEventArgs args)
        {
            _game.Platform.OnPresentationChanged(args.PresentationParameters);
        }

        /// <summary>
        /// The profile which determines the graphics feature level.
        /// </summary>
        public GraphicsProfile GraphicsProfile
        {
            get
            {
                return _graphicsProfile;
            }
            set
            {
                _shouldApplyChanges = true;
                _graphicsProfile = value;
            }
        }

        /// <summary>
        /// Returns the graphics device for this manager.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return _graphicsDevice;
            }
        }

        /// <summary>
        /// Indicates the desire to switch into fullscreen mode.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set fullscreen mode during initialization.  If
        /// set after startup you must call ApplyChanges() for the fullscreen mode to be changed.
        /// Note that for some platforms that do not support windowed modes this property has no affect.
        /// </remarks>
        public bool IsFullScreen
        {
            get { return _wantFullScreen; }
            set
            {
                _shouldApplyChanges = true;
                _wantFullScreen = value;
            }
        }

        /// <summary>
        /// Gets or sets the boolean which defines how window switches from windowed to fullscreen state.
        /// "Hard" mode(true) is slow to switch, but more effecient for performance, while "soft" mode(false) is vice versa.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool HardwareModeSwitch
        {
            get { return _hardwareModeSwitch;}
            set
            {
                _shouldApplyChanges = true;
                _hardwareModeSwitch = value;
            }
        }

        /// <summary>
        /// Indicates the desire for a multisampled back buffer.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the MSAA mode during initialization.  If
        /// set after startup you must call ApplyChanges() for the MSAA mode to be changed.
        /// </remarks>
        public bool PreferMultiSampling
        {
            get
            {
                return _preferMultiSampling;
            }
            set
            {
                _shouldApplyChanges = true;
                _preferMultiSampling = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer color format.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the format during initialization.  If
        /// set after startup you must call ApplyChanges() for the format to be changed.
        /// </remarks>
        public SurfaceFormat PreferredBackBufferFormat
        {
            get
            {
                return _preferredBackBufferFormat;
            }
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferFormat = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer height in pixels.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the height during initialization.  If
        /// set after startup you must call ApplyChanges() for the height to be changed.
        /// </remarks>
        public int PreferredBackBufferHeight
        {
            get
            {
                return _preferredBackBufferHeight;
            }
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferHeight = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer width in pixels.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the width during initialization.  If
        /// set after startup you must call ApplyChanges() for the width to be changed.
        /// </remarks>
        public int PreferredBackBufferWidth
        {
            get
            {
                return _preferredBackBufferWidth;
            }
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferWidth = value;
            }
        }

        /// <summary>
        /// Indicates the desired depth-stencil buffer format.
        /// </summary>
        /// <remarks>
        /// The depth-stencil buffer format defines the scene depth precision and stencil bits available for effects during rendering.
        /// When called at startup this will automatically set the format during initialization.  If
        /// set after startup you must call ApplyChanges() for the format to be changed.
        /// </remarks>
        public DepthFormat PreferredDepthStencilFormat
        {
            get
            {
                return _preferredDepthStencilFormat;
            }
            set
            {
                _shouldApplyChanges = true;
                _preferredDepthStencilFormat = value;
            }
        }

        /// <summary>
        /// Indicates the desire for vsync when presenting the back buffer.
        /// </summary>
        /// <remarks>
        /// Vsync limits the frame rate of the game to the monitor referesh rate to prevent screen tearing.
        /// When called at startup this will automatically set the vsync mode during initialization.  If
        /// set after startup you must call ApplyChanges() for the vsync mode to be changed.
        /// </remarks>
        public bool SynchronizeWithVerticalRetrace
        {
            get
            {
                return _synchronizedWithVerticalRetrace;
            }
            set
            {
                _shouldApplyChanges = true;
                _synchronizedWithVerticalRetrace = value;
            }
        }

        /// <summary>
        /// Indicates the desired allowable display orientations when the device is rotated.
        /// </summary>
        /// <remarks>
        /// This property only applies to mobile platforms with automatic display rotation.
        /// When called at startup this will automatically apply the supported orientations during initialization.  If
        /// set after startup you must call ApplyChanges() for the supported orientations to be changed.
        /// </remarks>
        public DisplayOrientation SupportedOrientations
        {
            get
            {
                return _supportedOrientations;
            }
            set
            {
                _shouldApplyChanges = true;
                _supportedOrientations = value;
            }
        }
    }
}
