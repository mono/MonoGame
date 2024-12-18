// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using SharpDX.Direct3D11;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class RenderTarget3D
    {
        private int _currentSlice;
        private RenderTargetView _renderTargetView;
        private DepthStencilView _depthStencilView;

        private void PlatformConstruct(GraphicsDevice graphicsDevice, int width, int height, bool mipMap,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            // Setup the multisampling description.
            var multisampleDesc = new SharpDX.DXGI.SampleDescription(1, 0);
            if (preferredMultiSampleCount > 1)
            {
                multisampleDesc.Count = preferredMultiSampleCount;
                multisampleDesc.Quality = (int)StandardMultisampleQualityLevels.StandardMultisamplePattern;
            }

            // Create a descriptor for the depth/stencil buffer.
            // Allocate a 2-D surface as the depth/stencil buffer.
            // Create a DepthStencil view on this surface to use on bind.
            using (var depthBuffer = new SharpDX.Direct3D11.Texture2D(graphicsDevice._d3dDevice, new Texture2DDescription
            {
                Format = SharpDXHelper.ToFormat(preferredDepthFormat),
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = multisampleDesc,
                BindFlags = BindFlags.DepthStencil,
            }))
            {
                // Create the view for binding to the device.
                _depthStencilView = new DepthStencilView(graphicsDevice._d3dDevice, depthBuffer, new DepthStencilViewDescription()
                {
                    Format = SharpDXHelper.ToFormat(preferredDepthFormat),
                    Dimension = DepthStencilViewDimension.Texture2D
                });
            }
        }

        /// <summary />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SharpDX.Utilities.Dispose(ref _renderTargetView);
                SharpDX.Utilities.Dispose(ref _depthStencilView);
            }

            base.Dispose(disposing);
        }

	    RenderTargetView IRenderTarget.GetRenderTargetView(int arraySlice)
	    {
            if (arraySlice >= Depth)
                throw new ArgumentOutOfRangeException("The arraySlice is out of range for this Texture3D.");

            // Dispose the previous target.
	        if (_currentSlice != arraySlice && _renderTargetView != null)
	        {
	            _renderTargetView.Dispose();
	            _renderTargetView = null;
	        }

            // Create the new target view interface.
	        if (_renderTargetView == null)
	        {
	            _currentSlice = arraySlice;

	            var desc = new RenderTargetViewDescription
	            {
	                Format = SharpDXHelper.ToFormat(_format),
	                Dimension = RenderTargetViewDimension.Texture3D,
	                Texture3D =
	                    {
	                        DepthSliceCount = -1,
	                        FirstDepthSlice = arraySlice,
	                        MipSlice = 0,
	                    }
	            };

	            _renderTargetView = new RenderTargetView(GraphicsDevice._d3dDevice, GetTexture(), desc);
	        }

	        return _renderTargetView;
	    }

	    DepthStencilView IRenderTarget.GetDepthStencilView()
	    {
	        return _depthStencilView;
	    }
    }
}
