// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Diagnostics;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class BlendState
    {
        private SharpDX.Direct3D11.BlendState _state;

        /// <inheritdoc />
        protected internal override void GraphicsDeviceResetting()
        {
            SharpDX.Utilities.Dispose(ref _state);
            base.GraphicsDeviceResetting();
        }

        internal SharpDX.Direct3D11.BlendState GetDxState(GraphicsDevice device)
        {
            if (_state == null)
            {
                // Build the description.
                var desc = new SharpDX.Direct3D11.BlendStateDescription();
                _targetBlendState[0].GetState(ref desc.RenderTarget[0]);
                _targetBlendState[1].GetState(ref desc.RenderTarget[1]);
                _targetBlendState[2].GetState(ref desc.RenderTarget[2]);
                _targetBlendState[3].GetState(ref desc.RenderTarget[3]);
                desc.IndependentBlendEnable = _independentBlendEnable;

                // This is a new DX11 feature we should consider
                // exposing as part of the extended MonoGame API.
                desc.AlphaToCoverageEnable = false;

                // Create the state.
                _state = new SharpDX.Direct3D11.BlendState(GraphicsDevice._d3dDevice, desc);
            }

            Debug.Assert(GraphicsDevice == device, "The state was created for a different device!");

			// Apply the state!
			return _state;
        }

        partial void PlatformDispose()
        {
            SharpDX.Utilities.Dispose(ref _state);
        }
    }
}

