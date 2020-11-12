// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using MonoGame.Framework.Utilities;
using System.Runtime.InteropServices;


namespace Microsoft.Xna.Framework.Graphics
{
    public partial class GraphicsDevice : IDisposable
    {
        /// <summary>
        /// Indicates if DX9 style pixel addressing or current standard
        /// pixel addressing should be used. This flag is set to
        /// <c>false</c> by default. If `UseHalfPixelOffset` is
        /// `true` you have to add half-pixel offset to a Projection matrix.
        /// See also <see cref="GraphicsDeviceManager.PreferHalfPixelOffset"/>.
        /// </summary>
        /// <remarks>
        /// XNA uses DirectX9 for its graphics. DirectX9 interprets UV
        /// coordinates differently from other graphics API's. This is
        /// typically referred to as the half-pixel offset. MonoGame
        /// replicates XNA behavior if this flag is set to <c>true</c>.
        /// </remarks>
        public bool UseHalfPixelOffset { get; private set; }

        private Viewport _viewport;

        private bool _isDisposed;

        // On Intel Integrated graphics, there is a fast hw unit for doing
        // clears to colors where all components are either 0 or 255.
        // Despite XNA4 using Purple here, we use black (in Release) to avoid
        // performance warnings on Intel/Mesa
#if DEBUG
        private static Color _discardColor = new Color(68, 34, 136, 255);
#else
        private static Color _discardColor = new Color(0, 0, 0, 255);
#endif

        private Color _blendFactor = Color.White;
        private bool _blendFactorDirty;

        private BlendState _blendState;
        private BlendState _actualBlendState;
        private bool _blendStateDirty;

        private BlendState _blendStateAdditive;
        private BlendState _blendStateAlphaBlend;
        private BlendState _blendStateNonPremultiplied;
        private BlendState _blendStateOpaque;

        private DepthStencilState _depthStencilState;
        private DepthStencilState _actualDepthStencilState;
        private bool _depthStencilStateDirty;

        private DepthStencilState _depthStencilStateDefault;
        private DepthStencilState _depthStencilStateDepthRead;
        private DepthStencilState _depthStencilStateNone;

        private RasterizerState _rasterizerState;
        private RasterizerState _actualRasterizerState;
        private bool _rasterizerStateDirty;

        private RasterizerState _rasterizerStateCullClockwise;
        private RasterizerState _rasterizerStateCullCounterClockwise;
        private RasterizerState _rasterizerStateCullNone;

        private Rectangle _scissorRectangle;
        private bool _scissorRectangleDirty;

        private VertexBufferBindings _vertexBuffers;
        private bool _vertexBuffersDirty;

        private IndexBuffer _indexBuffer;
        private bool _indexBufferDirty;

        private readonly RenderTargetBinding[] _currentRenderTargetBindings = new RenderTargetBinding[4];
        private int _currentRenderTargetCount;
        private readonly RenderTargetBinding[] _tempRenderTargetBinding = new RenderTargetBinding[1];

        internal GraphicsCapabilities GraphicsCapabilities { get; private set; }

        public TextureCollection Textures { get; private set; }

        public TextureCollection VertexTextures { get; private set; }

        public TextureCollection HullTextures { get; private set; }

        public TextureCollection DomainTextures { get; private set; }

        public TextureCollection GeometryTextures { get; private set; }

        public SamplerStateCollection SamplerStates { get; private set; }

        public SamplerStateCollection VertexSamplerStates { get; private set; }

        public SamplerStateCollection HullSamplerStates { get; private set; }

        public SamplerStateCollection DomainSamplerStates { get; private set; }

        public SamplerStateCollection GeometrySamplerStates { get; private set; }

      

        /// <summary>
        /// Get or set the color a <see cref="RenderTarget2D"/> is cleared to when it is set.
        /// </summary>
        public static Color DiscardColor {
			get { return _discardColor; }
			set { _discardColor = value; }
		}

        /// <summary>
        /// The active vertex shader.
        /// </summary>
        private Shader _vertexShader;
        private bool _vertexShaderDirty;
        private bool VertexShaderDirty
        {
            get { return _vertexShaderDirty; }
        }

        /// <summary>
        /// The active pixel shader.
        /// </summary>
        private Shader _pixelShader;
        private bool _pixelShaderDirty;
        private bool PixelShaderDirty
        {
            get { return _pixelShaderDirty; }
        }

        /// <summary>
        /// The active domain shader.
        /// </summary>
        private Shader _domainShader;
        private bool _domainShaderDirty;
        private bool DomainShaderDirty
        {
            get { return _domainShaderDirty; }
        }

        /// <summary>
        /// The active hull shader.
        /// </summary>
        private Shader _hullShader;
        private bool _hullShaderDirty;
        private bool HullShaderDirty
        {
            get { return _hullShaderDirty; }
        }

        /// <summary>
        /// The active geometry shader.
        /// </summary>
        private Shader _geometryShader;
        private bool _geometryShaderDirty;
        private bool GeometryShaderDirty
        {
            get { return _geometryShaderDirty; }
        }

        private readonly ConstantBufferCollection _vertexConstantBuffers = new ConstantBufferCollection(ShaderStage.Vertex, 16);
        private readonly ConstantBufferCollection _pixelConstantBuffers = new ConstantBufferCollection(ShaderStage.Pixel, 16);
        private readonly ConstantBufferCollection _hullConstantBuffers = new ConstantBufferCollection(ShaderStage.Hull, 16);
        private readonly ConstantBufferCollection _domainConstantBuffers = new ConstantBufferCollection(ShaderStage.Domain, 16);
        private readonly ConstantBufferCollection _geometryConstantBuffers = new ConstantBufferCollection(ShaderStage.Geometry, 16);

        /// <summary>
        /// The cache of effects from unique byte streams.
        /// </summary>
        internal Dictionary<int, Effect> EffectCache;

        // Resources may be added to and removed from the list from many threads.
        private readonly object _resourcesLock = new object();

        // Use WeakReference for the global resources list as we do not know when a resource
        // may be disposed and collected. We do not want to prevent a resource from being
        // collected by holding a strong reference to it in this list.
        private readonly List<WeakReference> _resources = new List<WeakReference>();

		// TODO Graphics Device events need implementing
		public event EventHandler<EventArgs> DeviceLost;
		public event EventHandler<EventArgs> DeviceReset;
		public event EventHandler<EventArgs> DeviceResetting;
		public event EventHandler<ResourceCreatedEventArgs> ResourceCreated;
		public event EventHandler<ResourceDestroyedEventArgs> ResourceDestroyed;
        public event EventHandler<EventArgs> Disposing;

        internal event EventHandler<PresentationEventArgs> PresentationChanged;

        private int _maxVertexBufferSlots;
        internal int MaxTextureSlots;
        internal int MaxVertexTextureSlots;

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

		public bool IsContentLost {
			get {
				// We will just return IsDisposed for now
				// as that is the only case I can see for now
				return IsDisposed;
			}
		}

        internal bool IsRenderTargetBound
        {
            get
            {
                return _currentRenderTargetCount > 0;
            }
        }

        internal DepthFormat ActiveDepthFormat
        {
            get
            {
                return IsRenderTargetBound
                    ? _currentRenderTargetBindings[0].DepthFormat
                    : PresentationParameters.DepthStencilFormat;
            }
        }

        public GraphicsAdapter Adapter
        {
            get;
            private set;
        }

        internal GraphicsMetrics _graphicsMetrics;

        /// <summary>
        /// The rendering information for debugging and profiling.
        /// The metrics are reset every frame after draw within <see cref="GraphicsDevice.Present"/>. 
        /// </summary>
        public GraphicsMetrics Metrics { get { return _graphicsMetrics; } set { _graphicsMetrics = value; } }

        private GraphicsDebug _graphicsDebug;

        /// <summary>
        /// Access debugging APIs for the graphics subsystem.
        /// </summary>
        public GraphicsDebug GraphicsDebug { get { return _graphicsDebug; } set { _graphicsDebug = value; } }

        internal GraphicsDevice()
		{
            PresentationParameters = new PresentationParameters();
            PresentationParameters.DepthStencilFormat = DepthFormat.Depth24;
            Setup();
            GraphicsCapabilities = new GraphicsCapabilities();
            GraphicsCapabilities.Initialize(this);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsDevice" /> class.
        /// </summary>
        /// <param name="adapter">The graphics adapter.</param>
        /// <param name="graphicsProfile">The graphics profile.</param>
        /// <param name="presentationParameters">The presentation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="presentationParameters"/> is <see langword="null"/>.
        /// </exception>
        public GraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, PresentationParameters presentationParameters)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");
            if (!adapter.IsProfileSupported(graphicsProfile))
                throw new NoSuitableGraphicsDeviceException(String.Format("Adapter '{0}' does not support the {1} profile.", adapter.Description, graphicsProfile));
            if (presentationParameters == null)
                throw new ArgumentNullException("presentationParameters");
            Adapter = adapter;
            PresentationParameters = presentationParameters;
            _graphicsProfile = graphicsProfile;
            Setup();
            GraphicsCapabilities = new GraphicsCapabilities();
            GraphicsCapabilities.Initialize(this);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsDevice" /> class.
        /// </summary>
        /// <param name="adapter">The graphics adapter.</param>
        /// <param name="graphicsProfile">The graphics profile.</param>
        /// <param name="preferHalfPixelOffset"> Indicates if DX9 style pixel addressing or current standard pixel addressing should be used. This value is passed to <see cref="GraphicsDevice.UseHalfPixelOffset"/></param>
        /// <param name="presentationParameters">The presentation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="presentationParameters"/> is <see langword="null"/>.
        /// </exception>
        public GraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, bool preferHalfPixelOffset, PresentationParameters presentationParameters)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");
            if (!adapter.IsProfileSupported(graphicsProfile))
                throw new NoSuitableGraphicsDeviceException(String.Format("Adapter '{0}' does not support the {1} profile.", adapter.Description, graphicsProfile));
            if (presentationParameters == null)
                throw new ArgumentNullException("presentationParameters");
#if DIRECTX
            // TODO we need to figure out how to inject the half pixel offset into DX shaders
            preferHalfPixelOffset = false;
#endif
            Adapter = adapter;
            _graphicsProfile = graphicsProfile;
            UseHalfPixelOffset = preferHalfPixelOffset;
            PresentationParameters = presentationParameters;
            Setup();
            GraphicsCapabilities = new GraphicsCapabilities();
            GraphicsCapabilities.Initialize(this);

            Initialize();
        }

        private void Setup()
        {
#if DEBUG
            if (DisplayMode == null)
            {
                throw new Exception(
                    "Unable to determine the current display mode.  This can indicate that the " +
                    "game is not configured to be HiDPI aware under Windows 10 or later.  See " +
                    "https://github.com/MonoGame/MonoGame/issues/5040 for more information.");
            }
#endif

            // Initialize the main viewport
            _viewport = new Viewport (0, 0,
			                         DisplayMode.Width, DisplayMode.Height);
			_viewport.MaxDepth = 1.0f;

            PlatformSetup();

            Textures = new TextureCollection(this, MaxTextureSlots);
            SamplerStates = new SamplerStateCollection(this, MaxTextureSlots);

            // In DirectX we use separate texture/sampler collections for every shader stage.
            // In OpenGL we use one big texture/sampler collection for all stages.
#if OPENGL
            VertexTextures = Textures;
            HullTextures = Textures;
            DomainTextures = Textures;
            GeometryTextures = Textures;
            Textures = Textures;

            VertexSamplerStates = SamplerStates;
            HullSamplerStates = SamplerStates;
            DomainSamplerStates = SamplerStates;
            GeometrySamplerStates = SamplerStates;
            SamplerStates = SamplerStates;
#else
            VertexTextures = new TextureCollection(this, MaxVertexTextureSlots);
            HullTextures = new TextureCollection(this, MaxVertexTextureSlots);
            DomainTextures = new TextureCollection(this, MaxVertexTextureSlots);
            GeometryTextures = new TextureCollection(this, MaxVertexTextureSlots);
            Textures = new TextureCollection(this, MaxTextureSlots);

            VertexSamplerStates = new SamplerStateCollection(this, MaxVertexTextureSlots);
            HullSamplerStates = new SamplerStateCollection(this, MaxVertexTextureSlots);
            DomainSamplerStates = new SamplerStateCollection(this, MaxVertexTextureSlots);
            GeometrySamplerStates = new SamplerStateCollection(this, MaxVertexTextureSlots);
            SamplerStates = new SamplerStateCollection(this, MaxTextureSlots);
#endif

            _blendStateAdditive = BlendState.Additive.Clone();
            _blendStateAlphaBlend = BlendState.AlphaBlend.Clone();
            _blendStateNonPremultiplied = BlendState.NonPremultiplied.Clone();
            _blendStateOpaque = BlendState.Opaque.Clone();

            BlendState = BlendState.Opaque;

            _depthStencilStateDefault = DepthStencilState.Default.Clone();
            _depthStencilStateDepthRead = DepthStencilState.DepthRead.Clone();
            _depthStencilStateNone = DepthStencilState.None.Clone();

            DepthStencilState = DepthStencilState.Default;

            _rasterizerStateCullClockwise = RasterizerState.CullClockwise.Clone();
            _rasterizerStateCullCounterClockwise = RasterizerState.CullCounterClockwise.Clone();
            _rasterizerStateCullNone = RasterizerState.CullNone.Clone();

            RasterizerState = RasterizerState.CullCounterClockwise;

            EffectCache = new Dictionary<int, Effect>();
        }

        ~GraphicsDevice()
        {
            Dispose(false);
        }

        internal int GetClampedMultisampleCount(int multiSampleCount)
        {
            if (multiSampleCount > 1)
            {
                // Round down MultiSampleCount to the nearest power of two
                // hack from http://stackoverflow.com/a/2681094
                // Note: this will return an incorrect, but large value
                // for very large numbers. That doesn't matter because
                // the number will get clamped below anyway in this case.
                var msc = multiSampleCount;
                msc = msc | (msc >> 1);
                msc = msc | (msc >> 2);
                msc = msc | (msc >> 4);
                msc -= (msc >> 1);
                // and clamp it to what the device can handle
                if (msc > GraphicsCapabilities.MaxMultiSampleCount)
                    msc = GraphicsCapabilities.MaxMultiSampleCount;

                return msc;
            }
            else return 0;
        }

        internal void Initialize()
        {
            PlatformInitialize();

            // Force set the default render states.
            _blendStateDirty = _depthStencilStateDirty = _rasterizerStateDirty = true;
            BlendState = BlendState.Opaque;
            DepthStencilState = DepthStencilState.Default;
            RasterizerState = RasterizerState.CullCounterClockwise;

            // Clear the texture and sampler collections forcing
            // the state to be reapplied.
            Textures.Clear();         
            VertexTextures.Clear();
            HullTextures.Clear();
            DomainTextures.Clear();
            GeometryTextures.Clear();

            SamplerStates.Clear();
            VertexSamplerStates.Clear();
            HullSamplerStates.Clear();
            DomainSamplerStates.Clear();
            GeometrySamplerStates.Clear();

            // Clear constant buffers
            _vertexConstantBuffers.Clear();
            _pixelConstantBuffers.Clear();
            _hullConstantBuffers.Clear();
            _domainConstantBuffers.Clear();
            _geometryConstantBuffers.Clear();

            // Force set the buffers and shaders on next ApplyState() call
            _vertexBuffers = new VertexBufferBindings(_maxVertexBufferSlots);
            _vertexBuffersDirty = true;
            _indexBufferDirty = true;
            _vertexShaderDirty = true;
            _pixelShaderDirty = true;
            _hullShaderDirty = true;
            _domainShaderDirty = true;
            _geometryShaderDirty = true;

            // Set the default scissor rect.
            _scissorRectangleDirty = true;
            ScissorRectangle = _viewport.Bounds;

            // Set the default render target.
            ApplyRenderTargets(null);
        }

        public RasterizerState RasterizerState
        {
            get
            {
                return _rasterizerState;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                // Don't set the same state twice!
                if (_rasterizerState == value)
                    return;

                if (!value.DepthClipEnable && !GraphicsCapabilities.SupportsDepthClamp)
                    throw new InvalidOperationException("Cannot set RasterizerState.DepthClipEnable to false on this graphics device");

                _rasterizerState = value;

                // Static state properties never actually get bound;
                // instead we use our GraphicsDevice-specific version of them.
                var newRasterizerState = _rasterizerState;
                if (ReferenceEquals(_rasterizerState, RasterizerState.CullClockwise))
                    newRasterizerState = _rasterizerStateCullClockwise;
                else if (ReferenceEquals(_rasterizerState, RasterizerState.CullCounterClockwise))
                    newRasterizerState = _rasterizerStateCullCounterClockwise;
                else if (ReferenceEquals(_rasterizerState, RasterizerState.CullNone))
                    newRasterizerState = _rasterizerStateCullNone;

                newRasterizerState.BindToGraphicsDevice(this);

                _actualRasterizerState = newRasterizerState;

                _rasterizerStateDirty = true;
            }
        }

        /// <summary>
        /// The color used as blend factor when alpha blending.
        /// </summary>
        /// <remarks>
        /// When only changing BlendFactor, use this rather than <see cref="Graphics.BlendState.BlendFactor"/> to
        /// only update BlendFactor so the whole BlendState does not have to be updated.
        /// </remarks>
        public Color BlendFactor
        {
            get { return _blendFactor; }
            set
            {
                if (_blendFactor == value)
                    return;
                _blendFactor = value;
                _blendFactorDirty = true;
            }
        }

        public BlendState BlendState
        {
			get { return _blendState; }
			set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                // Don't set the same state twice!
                if (_blendState == value)
                    return;

				_blendState = value;

                // Static state properties never actually get bound;
                // instead we use our GraphicsDevice-specific version of them.
                var newBlendState = _blendState;
                if (ReferenceEquals(_blendState, BlendState.Additive))
                    newBlendState = _blendStateAdditive;
                else if (ReferenceEquals(_blendState, BlendState.AlphaBlend))
                    newBlendState = _blendStateAlphaBlend;
                else if (ReferenceEquals(_blendState, BlendState.NonPremultiplied))
                    newBlendState = _blendStateNonPremultiplied;
                else if (ReferenceEquals(_blendState, BlendState.Opaque))
                    newBlendState = _blendStateOpaque;

                if (newBlendState.IndependentBlendEnable && !GraphicsCapabilities.SupportsSeparateBlendStates)
                    throw new PlatformNotSupportedException("Independent blend states requires at least OpenGL 4.0 or GL_ARB_draw_buffers_blend. Try upgrading your graphics drivers.");

                // Blend state is now bound to a device... no one should
                // be changing the state of the blend state object now!
                newBlendState.BindToGraphicsDevice(this);

                _actualBlendState = newBlendState;

                BlendFactor = _actualBlendState.BlendFactor;

                _blendStateDirty = true;
            }
		}

        public DepthStencilState DepthStencilState
        {
            get { return _depthStencilState; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                // Don't set the same state twice!
                if (_depthStencilState == value)
                    return;

                _depthStencilState = value;

                // Static state properties never actually get bound;
                // instead we use our GraphicsDevice-specific version of them.
                var newDepthStencilState = _depthStencilState;
                if (ReferenceEquals(_depthStencilState, DepthStencilState.Default))
                    newDepthStencilState = _depthStencilStateDefault;
                else if (ReferenceEquals(_depthStencilState, DepthStencilState.DepthRead))
                    newDepthStencilState = _depthStencilStateDepthRead;
                else if (ReferenceEquals(_depthStencilState, DepthStencilState.None))
                    newDepthStencilState = _depthStencilStateNone;

                newDepthStencilState.BindToGraphicsDevice(this);

                _actualDepthStencilState = newDepthStencilState;

                _depthStencilStateDirty = true;
            }
        }

        internal void ApplyState(bool applyShaders)
        {
            PlatformBeginApplyState();

            PlatformApplyBlend();

            if (_depthStencilStateDirty)
            {
                _actualDepthStencilState.PlatformApplyState(this);
                _depthStencilStateDirty = false;
            }

            if (_rasterizerStateDirty)
            {
                _actualRasterizerState.PlatformApplyState(this);
                _rasterizerStateDirty = false;
            }

            PlatformApplyState(applyShaders);
        }

        public void Clear(Color color)
        {
            var options = ClearOptions.Target;
            options |= ClearOptions.DepthBuffer;
            options |= ClearOptions.Stencil;
            PlatformClear(options, color.ToVector4(), _viewport.MaxDepth, 0);

            unchecked
            {
                _graphicsMetrics._clearCount++;
            }
        }

        public void Clear(ClearOptions options, Color color, float depth, int stencil)
        {
            PlatformClear(options, color.ToVector4(), depth, stencil);

            unchecked
            {
                _graphicsMetrics._clearCount++;
            }
        }

		public void Clear(ClearOptions options, Vector4 color, float depth, int stencil)
		{
            PlatformClear(options, color, depth, stencil);

            unchecked
            {
                _graphicsMetrics._clearCount++;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Dispose of all remaining graphics resources before disposing of the graphics device
                    lock (_resourcesLock)
                    {
                        foreach (var resource in _resources.ToArray())
                        {
                            var target = resource.Target as IDisposable;
                            if (target != null)
                                target.Dispose();
                        }
                        _resources.Clear();
                    }

                    // Clear the effect cache.
                    EffectCache.Clear();

                    _blendState = null;
                    _actualBlendState = null;
                    _blendStateAdditive.Dispose();
                    _blendStateAlphaBlend.Dispose();
                    _blendStateNonPremultiplied.Dispose();
                    _blendStateOpaque.Dispose();

                    _depthStencilState = null;
                    _actualDepthStencilState = null;
                    _depthStencilStateDefault.Dispose();
                    _depthStencilStateDepthRead.Dispose();
                    _depthStencilStateNone.Dispose();

                    _rasterizerState = null;
                    _actualRasterizerState = null;
                    _rasterizerStateCullClockwise.Dispose();
                    _rasterizerStateCullCounterClockwise.Dispose();
                    _rasterizerStateCullNone.Dispose();

                    PlatformDispose();
                }

                _isDisposed = true;
                EventHelpers.Raise(this, Disposing, EventArgs.Empty);
            }
        }

        internal void AddResourceReference(WeakReference resourceReference)
        {
            lock (_resourcesLock)
            {
                _resources.Add(resourceReference);
            }
        }

        internal void RemoveResourceReference(WeakReference resourceReference)
        {
            lock (_resourcesLock)
            {
                _resources.Remove(resourceReference);
            }
        }

        public void Present()
        {
            // We cannot present with a RT set on the device.
            if (_currentRenderTargetCount != 0)
                throw new InvalidOperationException("Cannot call Present when a render target is active.");

            _graphicsMetrics = new GraphicsMetrics();
            PlatformPresent();
        }

        /*
        public void Present(Rectangle? sourceRectangle, Rectangle? destinationRectangle, IntPtr overrideWindowHandle)
        {
            throw new NotImplementedException();
        }
        */

        partial void PlatformReset();

        public void Reset()
        {
            PlatformReset();

            EventHelpers.Raise(this, DeviceResetting, EventArgs.Empty);

            // Update the back buffer.
            OnPresentationChanged();
            
            EventHelpers.Raise(this, PresentationChanged, new PresentationEventArgs(PresentationParameters));
            EventHelpers.Raise(this, DeviceReset, EventArgs.Empty);
       }

        public void Reset(PresentationParameters presentationParameters)
        {
            if (presentationParameters == null)
                throw new ArgumentNullException("presentationParameters");

            PresentationParameters = presentationParameters;
            Reset();
        }

        /// <summary>
        /// Trigger the DeviceResetting event
        /// Currently internal to allow the various platforms to send the event at the appropriate time.
        /// </summary>
        internal void OnDeviceResetting()
        {
            EventHelpers.Raise(this, DeviceResetting, EventArgs.Empty);

            lock (_resourcesLock)
            {
                foreach (var resource in _resources)
                {
                    var target = resource.Target as GraphicsResource;
                    if (target != null)
                        target.GraphicsDeviceResetting();
                }

                // Remove references to resources that have been garbage collected.
                _resources.RemoveAll(wr => !wr.IsAlive);
            }
        }

        /// <summary>
        /// Trigger the DeviceReset event to allow games to be notified of a device reset.
        /// Currently internal to allow the various platforms to send the event at the appropriate time.
        /// </summary>
        internal void OnDeviceReset()
        {
            EventHelpers.Raise(this, DeviceReset, EventArgs.Empty);
        }

        public DisplayMode DisplayMode
        {
            get
            {
                return Adapter.CurrentDisplayMode;
            }
        }

        public GraphicsDeviceStatus GraphicsDeviceStatus
        {
            get
            {
                return GraphicsDeviceStatus.Normal;
            }
        }

        public PresentationParameters PresentationParameters
        {
            get;
            private set;
        }

        public Viewport Viewport
        {
            get
            {
                return _viewport;
            }

            set
            {
                _viewport = value;
                PlatformSetViewport(ref value);
            }
        }

        private readonly GraphicsProfile _graphicsProfile;
        public GraphicsProfile GraphicsProfile
        {
            get { return _graphicsProfile; }
        }

        public Rectangle ScissorRectangle
        {
            get
            {
                return _scissorRectangle;
            }

            set
            {
                if (_scissorRectangle == value)
                    return;

                _scissorRectangle = value;
                _scissorRectangleDirty = true;
            }
        }

        public int RenderTargetCount
        {
            get
            {
                return _currentRenderTargetCount;
            }
        }

		public void SetRenderTarget(RenderTarget2D renderTarget)
		{
			if (renderTarget == null)
		    {
                SetRenderTargets(null);
		    }
			else
			{
				_tempRenderTargetBinding[0] = new RenderTargetBinding(renderTarget);
				SetRenderTargets(_tempRenderTargetBinding);
			}
		}

        public void SetRenderTarget(RenderTargetCube renderTarget, CubeMapFace cubeMapFace)
        {
            if (renderTarget == null)
            {
                SetRenderTargets(null);
            }
            else
            {
                _tempRenderTargetBinding[0] = new RenderTargetBinding(renderTarget, cubeMapFace);
                SetRenderTargets(_tempRenderTargetBinding);
            }
        }

		public void SetRenderTargets(params RenderTargetBinding[] renderTargets)
		{
            // Avoid having to check for null and zero length.
            var renderTargetCount = 0;
            if (renderTargets != null)
            {
                renderTargetCount = renderTargets.Length;
                if (renderTargetCount == 0)
                {
                    renderTargets = null;
                }
            }

            // Try to early out if the current and new bindings are equal.
            if (_currentRenderTargetCount == renderTargetCount)
            {
                var isEqual = true;
                for (var i = 0; i < _currentRenderTargetCount; i++)
                {
                    if (_currentRenderTargetBindings[i].RenderTarget != renderTargets[i].RenderTarget ||
                        _currentRenderTargetBindings[i].ArraySlice != renderTargets[i].ArraySlice)
                    {
                        isEqual = false;
                        break;
                    }
                }

                if (isEqual)
                    return;
            }

            ApplyRenderTargets(renderTargets);

            if (renderTargetCount == 0)
            {
                unchecked
                {
                    _graphicsMetrics._targetCount++;
                }
            }
            else
            {
                unchecked
                {
                    _graphicsMetrics._targetCount += renderTargetCount;
                }
            }
        }

        internal void ApplyRenderTargets(RenderTargetBinding[] renderTargets)
        {
            var clearTarget = false;

            PlatformResolveRenderTargets();

            // Clear the current bindings.
            Array.Clear(_currentRenderTargetBindings, 0, _currentRenderTargetBindings.Length);

            int renderTargetWidth;
            int renderTargetHeight;
            if (renderTargets == null)
            {
                _currentRenderTargetCount = 0;

                PlatformApplyDefaultRenderTarget();
                clearTarget = PresentationParameters.RenderTargetUsage == RenderTargetUsage.DiscardContents;

                renderTargetWidth = PresentationParameters.BackBufferWidth;
                renderTargetHeight = PresentationParameters.BackBufferHeight;
            }
			else
			{
                // Copy the new bindings.
                Array.Copy(renderTargets, _currentRenderTargetBindings, renderTargets.Length);
                _currentRenderTargetCount = renderTargets.Length;

                var renderTarget = PlatformApplyRenderTargets();

                // We clear the render target if asked.
                clearTarget = renderTarget.RenderTargetUsage == RenderTargetUsage.DiscardContents;

                renderTargetWidth = renderTarget.Width;
                renderTargetHeight = renderTarget.Height;
            }

            // Set the viewport to the size of the first render target.
            Viewport = new Viewport(0, 0, renderTargetWidth, renderTargetHeight);

            // Set the scissor rectangle to the size of the first render target.
            ScissorRectangle = new Rectangle(0, 0, renderTargetWidth, renderTargetHeight);

            // In XNA 4, because of hardware limitations on Xbox, when
            // a render target doesn't have PreserveContents as its usage
            // it is cleared before being rendered to.
            if (clearTarget)
                Clear(DiscardColor);
        }

		public RenderTargetBinding[] GetRenderTargets()
		{
            // Return a correctly sized copy our internal array.
            var bindings = new RenderTargetBinding[_currentRenderTargetCount];
            Array.Copy(_currentRenderTargetBindings, bindings, _currentRenderTargetCount);
            return bindings;
		}

        public void GetRenderTargets(RenderTargetBinding[] outTargets)
        {
            Debug.Assert(outTargets.Length == _currentRenderTargetCount, "Invalid outTargets array length!");
            Array.Copy(_currentRenderTargetBindings, outTargets, _currentRenderTargetCount);
        }

        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
            _vertexBuffersDirty |= (vertexBuffer == null)
                                   ? _vertexBuffers.Clear()
                                   : _vertexBuffers.Set(vertexBuffer, 0);
        }

        public void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset)
        {
            // Validate vertexOffset.
            if (vertexOffset < 0
                || vertexBuffer == null && vertexOffset != 0
                || vertexBuffer != null && vertexOffset >= vertexBuffer.VertexCount)
            {
                throw new ArgumentOutOfRangeException("vertexOffset");
            }

            _vertexBuffersDirty |= (vertexBuffer == null)
                                   ? _vertexBuffers.Clear()
                                   : _vertexBuffers.Set(vertexBuffer, vertexOffset);
        }

        public void SetVertexBuffers(params VertexBufferBinding[] vertexBuffers)
        {
            if (vertexBuffers == null || vertexBuffers.Length == 0)
            {
                _vertexBuffersDirty |= _vertexBuffers.Clear();
            }
            else
            {
                if (vertexBuffers.Length > _maxVertexBufferSlots)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, "Max number of vertex buffers is {0}.", _maxVertexBufferSlots);
                    throw new ArgumentOutOfRangeException("vertexBuffers", message);
                }

                _vertexBuffersDirty |= _vertexBuffers.Set(vertexBuffers);
            }
        }

        private void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            if (_indexBuffer == indexBuffer)
                return;

            _indexBuffer = indexBuffer;
            _indexBufferDirty = true;
        }

        public IndexBuffer Indices { set { SetIndexBuffer(value); } get { return _indexBuffer; } }

        internal Shader VertexShader
        {
            get { return _vertexShader; }
            set
            {
                if (_vertexShader == value)
                    return;

                _vertexShader = value;
                _vertexConstantBuffers.Clear();
                _vertexShaderDirty = true;
            }
        }

        internal Shader PixelShader
        {
            get { return _pixelShader; }
            set
            {
                if (_pixelShader == value)
                    return;

                _pixelShader = value;
                _pixelConstantBuffers.Clear();
                _pixelShaderDirty = true;
            }
        }

        internal Shader HullShader
        {
            get { return _hullShader; }
            set
            {
                if (_hullShader == value)
                    return;

                _hullShader = value;
                _hullConstantBuffers.Clear();
                _hullShaderDirty = true;
            }
        }

        internal Shader DomainShader
        {
            get { return _domainShader; }
            set
            {
                if (_domainShader == value)
                    return;

                _domainShader = value;
                _domainConstantBuffers.Clear();
                _domainShaderDirty = true;
            }
        }

        internal Shader GeometryShader
        {
            get { return _geometryShader; }
            set
            {
                if (_geometryShader == value)
                    return;

                _geometryShader = value;
                _geometryConstantBuffers.Clear();
                _geometryShaderDirty = true;
            }
        }

        internal void SetConstantBuffer(ShaderStage stage, int slot, ConstantBuffer buffer)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    _vertexConstantBuffers[slot] = buffer;
                    break;
                case ShaderStage.Pixel:
                    _pixelConstantBuffers[slot] = buffer;
                    break;
                case ShaderStage.Hull:
                    _hullConstantBuffers[slot] = buffer;
                    break;
                case ShaderStage.Domain:
                    _domainConstantBuffers[slot] = buffer;
                    break;
                case ShaderStage.Geometry:
                    _geometryConstantBuffers[slot] = buffer;
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public bool ResourcesLost { get; set; }

        /// <summary>
        /// Draw geometry by indexing into the vertex buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="minVertexIndex">This is unused and remains here only for XNA API compatibility.</param>
        /// <param name="numVertices">This is unused and remains here only for XNA API compatibility.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives to render from the index buffer.</param>
        /// <remarks>Note that minVertexIndex and numVertices are unused in MonoGame and will be ignored.</remarks>
        [Obsolete("Use DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount) instead. In future versions this method can be removed.")]
        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
        {
            DrawIndexedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount);
        }

        /// <summary>
        /// Draw geometry by indexing into the vertex buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives to render from the index buffer.</param>
        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
        {
            if (_vertexShader == null)
                throw new InvalidOperationException("Vertex shader must be set before calling DrawIndexedPrimitives.");

            if (_vertexBuffers.Count == 0)
                throw new InvalidOperationException("Vertex buffer must be set before calling DrawIndexedPrimitives.");

            if (_indexBuffer == null)
                throw new InvalidOperationException("Index buffer must be set before calling DrawIndexedPrimitives.");

            bool isPatchPrimitives = primitiveType >= PrimitiveType.PatchListWith1ControlPoints && primitiveType <= PrimitiveType.PatchListWith32ControlPoints;

            if (_hullShader == null && isPatchPrimitives)
                throw new ArgumentException("Patch primitives can only be drawn when a hull shader is active");

            if (_hullShader != null && !isPatchPrimitives)
                throw new ArgumentException("If a hull shader is active the primitive type must be one of the patch types");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException("primitiveCount");

            PlatformDrawIndexedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += primitiveCount;
            }
        }

        /// <summary>
        /// Draw primitives of the specified type from the data in an array of vertices without indexing.
        /// </summary>
        /// <typeparam name="T">The type of the vertices.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">An array of vertices to draw.</param>
        /// <param name="vertexOffset">The index in the array of the first vertex that should be rendered.</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <remarks>The <see cref="VertexDeclaration"/> will be found by getting <see cref="IVertexType.VertexDeclaration"/>
        /// from an instance of <typeparamref name="T"/> and cached for subsequent calls.</remarks>
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) where T : struct, IVertexType
        {
            DrawUserPrimitives(primitiveType, vertexData, vertexOffset, primitiveCount, VertexDeclarationCache<T>.VertexDeclaration);
        }

        /// <summary>
        /// Draw primitives of the specified type from the data in the given array of vertices without indexing.
        /// </summary>
        /// <typeparam name="T">The type of the vertices.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">An array of vertices to draw.</param>
        /// <param name="vertexOffset">The index in the array of the first vertex that should be rendered.</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <param name="vertexDeclaration">The layout of the vertices.</param>
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
            if (vertexData == null)
                throw new ArgumentNullException("vertexData");

            if (vertexData.Length == 0)
                throw new ArgumentOutOfRangeException("vertexData");

            if (vertexOffset < 0 || vertexOffset >= vertexData.Length)
                throw new ArgumentOutOfRangeException("vertexOffset");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException("primitiveCount");

            bool isPatchPrimitives = primitiveType >= PrimitiveType.PatchListWith1ControlPoints && primitiveType <= PrimitiveType.PatchListWith32ControlPoints;

            if (_hullShader == null && isPatchPrimitives)
                throw new ArgumentException("Patch primitives can only be drawn when a hull shader is active");

            if (_hullShader != null && !isPatchPrimitives)
                throw new ArgumentException("If a hull shader is active the primitive type must be one of the patch types");

            var vertexCount = GetElementCountArray(primitiveType, primitiveCount);

            if (vertexOffset + vertexCount > vertexData.Length)
                throw new ArgumentOutOfRangeException("primitiveCount");

            if (vertexDeclaration == null)
                throw new ArgumentNullException("vertexDeclaration");

            PlatformDrawUserPrimitives<T>(primitiveType, vertexData, vertexOffset, vertexDeclaration, vertexCount);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += primitiveCount;
            }
        }

        /// <summary>
        /// Draw primitives of the specified type from the currently bound vertexbuffers without indexing.
        /// </summary>
        /// <param name="primitiveType">The type of primitives to draw.</param>
        /// <param name="vertexStart">Index of the vertex to start at.</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        public void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int primitiveCount)
        {
            if (_vertexShader == null)
                throw new InvalidOperationException("Vertex shader must be set before calling DrawPrimitives.");

            if (_vertexBuffers.Count == 0)
                throw new InvalidOperationException("Vertex buffer must be set before calling DrawPrimitives.");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException("primitiveCount");

            bool isPatchPrimitives = primitiveType >= PrimitiveType.PatchListWith1ControlPoints && primitiveType <= PrimitiveType.PatchListWith32ControlPoints;

            if (_hullShader == null && isPatchPrimitives)
                throw new ArgumentException("Patch primitives can only be drawn when a hull shader is active");

            if (_hullShader != null && !isPatchPrimitives)
                throw new ArgumentException("If a hull shader is active the primitive type must be one of the patch types");

            var vertexCount = GetElementCountArray(primitiveType, primitiveCount);

            PlatformDrawPrimitives(primitiveType, vertexStart, vertexCount);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount +=  primitiveCount;
            }
        }

        /// <summary>
        /// Draw primitives of the specified type by indexing into the given array of vertices with 16-bit indices.
        /// </summary>
        /// <typeparam name="T">The type of the vertices.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">An array of vertices to draw.</param>
        /// <param name="vertexOffset">The index in the array of the first vertex to draw.</param>
        /// <param name="indexOffset">The index in the array of indices of the first index to use</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <param name="numVertices">The number of vertices to draw.</param>
        /// <param name="indexData">The index data.</param>
        /// <remarks>The <see cref="VertexDeclaration"/> will be found by getting <see cref="IVertexType.VertexDeclaration"/>
        /// from an instance of <typeparamref name="T"/> and cached for subsequent calls.</remarks>
        /// <remarks>All indices in the vertex buffer are interpreted relative to the specified <paramref name="vertexOffset"/>.
        /// For example a value of zero in the array of indices points to the vertex at index <paramref name="vertexOffset"/>
        /// in the array of vertices.</remarks>
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
        {
            DrawUserIndexedPrimitives<T>(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, VertexDeclarationCache<T>.VertexDeclaration);
        }

        /// <summary>
        /// Draw primitives of the specified type by indexing into the given array of vertices with 16-bit indices.
        /// </summary>
        /// <typeparam name="T">The type of the vertices.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">An array of vertices to draw.</param>
        /// <param name="vertexOffset">The index in the array of the first vertex to draw.</param>
        /// <param name="indexOffset">The index in the array of indices of the first index to use</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <param name="numVertices">The number of vertices to draw.</param>
        /// <param name="indexData">The index data.</param>
        /// <param name="vertexDeclaration">The layout of the vertices.</param>
        /// <remarks>All indices in the vertex buffer are interpreted relative to the specified <paramref name="vertexOffset"/>.
        /// For example a value of zero in the array of indices points to the vertex at index <paramref name="vertexOffset"/>
        /// in the array of vertices.</remarks>
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
            // These parameter checks are a duplicate of the checks in the int[] overload of DrawUserIndexedPrimitives.
            // Inlined here for efficiency.

            if (vertexData == null || vertexData.Length == 0)
                throw new ArgumentNullException("vertexData");

            if (vertexOffset < 0 || vertexOffset >= vertexData.Length)
                throw new ArgumentOutOfRangeException("vertexOffset");

            if (numVertices <= 0 || numVertices > vertexData.Length)
                throw new ArgumentOutOfRangeException("numVertices");

            if (vertexOffset + numVertices > vertexData.Length)
                throw new ArgumentOutOfRangeException("numVertices");

            if (indexData == null || indexData.Length == 0)
                throw new ArgumentNullException("indexData");

            if (indexOffset < 0 || indexOffset >= indexData.Length)
                throw new ArgumentOutOfRangeException("indexOffset");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException("primitiveCount");

            bool isPatchPrimitives = primitiveType >= PrimitiveType.PatchListWith1ControlPoints && primitiveType <= PrimitiveType.PatchListWith32ControlPoints;

            if (_hullShader == null && isPatchPrimitives)
                throw new ArgumentException("Patch primitives can only be drawn when a hull shader is active");

            if (_hullShader != null && !isPatchPrimitives)
                throw new ArgumentException("If a hull shader is active the primitive type must be one of the patch types");

            if (indexOffset + GetElementCountArray(primitiveType, primitiveCount) > indexData.Length)
                throw new ArgumentOutOfRangeException("primitiveCount");

            if (vertexDeclaration == null)
                throw new ArgumentNullException("vertexDeclaration");

            if (vertexDeclaration.VertexStride < ReflectionHelpers.SizeOf<T>.Get())
                throw new ArgumentOutOfRangeException("vertexDeclaration", "Vertex stride of vertexDeclaration should be at least as big as the stride of the actual vertices.");

            PlatformDrawUserIndexedPrimitives<T>(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount +=  primitiveCount;
            }
        }

        /// <summary>
        /// Draw primitives of the specified type by indexing into the given array of vertices with 32-bit indices.
        /// </summary>
        /// <typeparam name="T">The type of the vertices.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">An array of vertices to draw.</param>
        /// <param name="vertexOffset">The index in the array of the first vertex to draw.</param>
        /// <param name="indexOffset">The index in the array of indices of the first index to use</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <param name="numVertices">The number of vertices to draw.</param>
        /// <param name="indexData">The index data.</param>
        /// <remarks>The <see cref="VertexDeclaration"/> will be found by getting <see cref="IVertexType.VertexDeclaration"/>
        /// from an instance of <typeparamref name="T"/> and cached for subsequent calls.</remarks>
        /// <remarks>All indices in the vertex buffer are interpreted relative to the specified <paramref name="vertexOffset"/>.
        /// For example a value of zero in the array of indices points to the vertex at index <paramref name="vertexOffset"/>
        /// in the array of vertices.</remarks>
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
        {
            DrawUserIndexedPrimitives<T>(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, VertexDeclarationCache<T>.VertexDeclaration);
        }

        /// <summary>
        /// Draw primitives of the specified type by indexing into the given array of vertices with 32-bit indices.
        /// </summary>
        /// <typeparam name="T">The type of the vertices.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">An array of vertices to draw.</param>
        /// <param name="vertexOffset">The index in the array of the first vertex to draw.</param>
        /// <param name="indexOffset">The index in the array of indices of the first index to use</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <param name="numVertices">The number of vertices to draw.</param>
        /// <param name="indexData">The index data.</param>
        /// <param name="vertexDeclaration">The layout of the vertices.</param>
        /// <remarks>All indices in the vertex buffer are interpreted relative to the specified <paramref name="vertexOffset"/>.
        /// For example value of zero in the array of indices points to the vertex at index <paramref name="vertexOffset"/>
        /// in the array of vertices.</remarks>
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
            // These parameter checks are a duplicate of the checks in the short[] overload of DrawUserIndexedPrimitives.
            // Inlined here for efficiency.

            if (vertexData == null || vertexData.Length == 0)
                throw new ArgumentNullException("vertexData");

            if (vertexOffset < 0 || vertexOffset >= vertexData.Length)
                throw new ArgumentOutOfRangeException("vertexOffset");

            if (numVertices <= 0 || numVertices > vertexData.Length)
                throw new ArgumentOutOfRangeException("numVertices");

            if (vertexOffset + numVertices > vertexData.Length)
                throw new ArgumentOutOfRangeException("numVertices");

            if (indexData == null || indexData.Length == 0)
                throw new ArgumentNullException("indexData");

            if (indexOffset < 0 || indexOffset >= indexData.Length)
                throw new ArgumentOutOfRangeException("indexOffset");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException("primitiveCount");

            bool isPatchPrimitives = primitiveType >= PrimitiveType.PatchListWith1ControlPoints && primitiveType <= PrimitiveType.PatchListWith32ControlPoints;

            if (_hullShader == null && isPatchPrimitives)
                throw new ArgumentException("Patch primitives can only be drawn when a hull shader is active");

            if (_hullShader != null && !isPatchPrimitives)
                throw new ArgumentException("If a hull shader is active the primitive type must be one of the patch types");

            if (indexOffset + GetElementCountArray(primitiveType, primitiveCount) > indexData.Length)
                throw new ArgumentOutOfRangeException("primitiveCount");

            if (vertexDeclaration == null)
                throw new ArgumentNullException("vertexDeclaration");

            if (vertexDeclaration.VertexStride < ReflectionHelpers.SizeOf<T>.Get())
                throw new ArgumentOutOfRangeException("vertexDeclaration", "Vertex stride of vertexDeclaration should be at least as big as the stride of the actual vertices.");

            PlatformDrawUserIndexedPrimitives<T>(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration);
            
            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount +=  primitiveCount;
            }
        }

        /// <summary>
        /// Draw instanced geometry from the bound vertex buffers and index buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="minVertexIndex">This is unused and remains here only for XNA API compatibility.</param>
        /// <param name="numVertices">This is unused and remains here only for XNA API compatibility.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives in a single instance.</param>
        /// <param name="instanceCount">The number of instances to render.</param>
        /// <remarks>Note that minVertexIndex and numVertices are unused in MonoGame and will be ignored.</remarks>
        [Obsolete("Use DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int instanceCount) instead. In future versions this method can be removed.")]
        public void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex,
                                            int numVertices, int startIndex, int primitiveCount, int instanceCount)
        {
            DrawInstancedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount, 0, instanceCount);
        }

        /// <summary>
        /// Draw instanced geometry from the bound vertex buffers and index buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives in a single instance.</param>
        /// <param name="instanceCount">The number of instances to render.</param>
        /// <remarks>Draw geometry with data from multiple bound vertex streams at different frequencies.</remarks>
        public void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int instanceCount)
        {
            DrawInstancedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount, 0, instanceCount);
        }

        /// <summary>
        /// Draw instanced geometry from the bound vertex buffers and index buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives in a single instance.</param>
        /// <param name="baseInstance">Used to offset the instance range indexed from the instance buffer.</param>
        /// <param name="instanceCount">The number of instances to render.</param>
        /// <remarks>Draw geometry with data from multiple bound vertex streams at different frequencies.</remarks>
        public void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int baseInstance, int instanceCount)
        {
            if (_vertexShader == null)
                throw new InvalidOperationException("Vertex shader must be set before calling DrawInstancedPrimitives.");

            if (_vertexBuffers.Count == 0)
                throw new InvalidOperationException("Vertex buffer must be set before calling DrawInstancedPrimitives.");

            if (_indexBuffer == null)
                throw new InvalidOperationException("Index buffer must be set before calling DrawInstancedPrimitives.");

            bool isPatchPrimitives = primitiveType >= PrimitiveType.PatchListWith1ControlPoints && primitiveType <= PrimitiveType.PatchListWith32ControlPoints;

            if (_hullShader == null && isPatchPrimitives)
                throw new ArgumentException("Patch primitives can only be drawn when a hull shader is active");

            if (_hullShader != null && !isPatchPrimitives)
                throw new ArgumentException("If a hull shader is active the primitive type must be one of the patch types");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException("primitiveCount");

            PlatformDrawInstancedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount, baseInstance, instanceCount);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += (primitiveCount * instanceCount);
            }
        }

        /// <summary>
        /// Gets the Pixel data of what is currently drawn on screen.
        /// The format is whatever the current format of the backbuffer is.
        /// </summary>
        /// <typeparam name="T">A byte[] of size (ViewPort.Width * ViewPort.Height * 4)</typeparam>
        public void GetBackBufferData<T>(T[] data) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException("data");
            GetBackBufferData(null, data, 0, data.Length);
        }

        public void GetBackBufferData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            GetBackBufferData(null, data, startIndex, elementCount);
        }

        public void GetBackBufferData<T>(Rectangle? rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int width, height;
            if (rect.HasValue)
            {
                var rectangle = rect.Value;
                width = rectangle.Width;
                height = rectangle.Height;

                if (rectangle.X < 0 || rectangle.Y < 0 || rectangle.Width <= 0 || rectangle.Height <= 0 ||
                    rectangle.Right > PresentationParameters.BackBufferWidth || rectangle.Top > PresentationParameters.BackBufferHeight)
                    throw new ArgumentException("Rectangle must fit in BackBuffer dimensions");
            }
            else
            {
                width = PresentationParameters.BackBufferWidth;
                height = PresentationParameters.BackBufferHeight;
            }

            var tSize = ReflectionHelpers.SizeOf<T>.Get();
            var fSize = PresentationParameters.BackBufferFormat.GetSize();
            if (tSize > fSize || fSize % tSize != 0)
                throw new ArgumentException("Type T is of an invalid size for the format of this texture.", "T");
            if (startIndex < 0 || startIndex >= data.Length)
                throw new ArgumentException("startIndex must be at least zero and smaller than data.Length.", "startIndex");
            if (data.Length < startIndex + elementCount)
                throw new ArgumentException("The data array is too small.");
            var dataByteSize = width * height * fSize;

            if (elementCount * tSize != dataByteSize)
                throw new ArgumentException(string.Format("elementCount is not the right size, " +
                                            "elementCount * sizeof(T) is {0}, but data size is {1} bytes.",
                                            elementCount * tSize, dataByteSize), "elementCount");

            PlatformGetBackBufferData(rect, data, startIndex, elementCount);
        }

        private static int GetElementCountArray(PrimitiveType primitiveType, int primitiveCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                case PrimitiveType.LineListWithAdjacency:
                    return primitiveCount * 2;
                case PrimitiveType.LineStrip:
                case PrimitiveType.LineStripWithAdjacency:
                    return primitiveCount + 1;
                case PrimitiveType.TriangleList:
                case PrimitiveType.TriangleListWithAdjacency:
                    return primitiveCount * 3;
                case PrimitiveType.TriangleStrip:
                case PrimitiveType.TriangleStripWithAdjacency:
                    return primitiveCount + 2;
                case PrimitiveType.PointList:
                    return primitiveCount;
                case PrimitiveType.PatchListWith1ControlPoints:
                    return primitiveCount * 1;
                case PrimitiveType.PatchListWith2ControlPoints:
                    return primitiveCount * 2;
                case PrimitiveType.PatchListWith3ControlPoints:
                    return primitiveCount * 3;
                case PrimitiveType.PatchListWith4ControlPoints:
                    return primitiveCount * 4;
                case PrimitiveType.PatchListWith5ControlPoints:
                    return primitiveCount * 5;
                case PrimitiveType.PatchListWith6ControlPoints:
                    return primitiveCount * 6;
                case PrimitiveType.PatchListWith7ControlPoints:
                    return primitiveCount * 7;
                case PrimitiveType.PatchListWith8ControlPoints:
                    return primitiveCount * 8;
                case PrimitiveType.PatchListWith9ControlPoints:
                    return primitiveCount * 9;
                case PrimitiveType.PatchListWith10ControlPoints:
                    return primitiveCount * 10;
                case PrimitiveType.PatchListWith11ControlPoints:
                    return primitiveCount * 11;
                case PrimitiveType.PatchListWith12ControlPoints:
                    return primitiveCount * 12;
                case PrimitiveType.PatchListWith13ControlPoints:
                    return primitiveCount * 13;
                case PrimitiveType.PatchListWith14ControlPoints:
                    return primitiveCount * 14;
                case PrimitiveType.PatchListWith15ControlPoints:
                    return primitiveCount * 15;
                case PrimitiveType.PatchListWith16ControlPoints:
                    return primitiveCount * 16;
                case PrimitiveType.PatchListWith17ControlPoints:
                    return primitiveCount * 17;
                case PrimitiveType.PatchListWith18ControlPoints:
                    return primitiveCount * 18;
                case PrimitiveType.PatchListWith19ControlPoints:
                    return primitiveCount * 19;
                case PrimitiveType.PatchListWith20ControlPoints:
                    return primitiveCount * 20;
                case PrimitiveType.PatchListWith21ControlPoints:
                    return primitiveCount * 21;
                case PrimitiveType.PatchListWith22ControlPoints:
                    return primitiveCount * 22;
                case PrimitiveType.PatchListWith23ControlPoints:
                    return primitiveCount * 23;
                case PrimitiveType.PatchListWith24ControlPoints:
                    return primitiveCount * 24;
                case PrimitiveType.PatchListWith25ControlPoints:
                    return primitiveCount * 25;
                case PrimitiveType.PatchListWith26ControlPoints:
                    return primitiveCount * 26;
                case PrimitiveType.PatchListWith27ControlPoints:
                    return primitiveCount * 27;
                case PrimitiveType.PatchListWith28ControlPoints:
                    return primitiveCount * 28;
                case PrimitiveType.PatchListWith29ControlPoints:
                    return primitiveCount * 29;
                case PrimitiveType.PatchListWith30ControlPoints:
                    return primitiveCount * 30;
                case PrimitiveType.PatchListWith31ControlPoints:
                    return primitiveCount * 31;
                case PrimitiveType.PatchListWith32ControlPoints:
                    return primitiveCount * 32;
                default:
                    throw new ArgumentException();
            }
        }

        // uniformly scales down the given rectangle by 10%
        internal static Rectangle GetDefaultTitleSafeArea(int x, int y, int width, int height)
        {
            var marginX = (width + 19) / 20;
            var marginY = (height + 19) / 20;
            x += marginX;
            y += marginY;

            width -= marginX * 2;
            height -= marginY * 2;
            return new Rectangle(x, y, width, height);
        }

        internal static Rectangle GetTitleSafeArea(int x, int y, int width, int height)
        {
            return PlatformGetTitleSafeArea(x, y, width, height);
        }
    }
}
