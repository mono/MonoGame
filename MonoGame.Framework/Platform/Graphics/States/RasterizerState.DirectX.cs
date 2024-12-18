// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class RasterizerState
    {
        private SharpDX.Direct3D11.RasterizerState _state;

        /// <inheritdoc />
        protected internal override void GraphicsDeviceResetting()
        {
            SharpDX.Utilities.Dispose(ref _state);
            base.GraphicsDeviceResetting();
        }

        internal void PlatformApplyState(GraphicsDevice device)
        {
            if (_state == null)
            {
                // Build the description.
                var desc = new SharpDX.Direct3D11.RasterizerStateDescription();

                switch ( CullMode )
                {
                    case Graphics.CullMode.CullClockwiseFace:
                        desc.CullMode = SharpDX.Direct3D11.CullMode.Front;
                        break;

                    case Graphics.CullMode.CullCounterClockwiseFace:
                        desc.CullMode = SharpDX.Direct3D11.CullMode.Back;
                        break;

                    case Graphics.CullMode.None:
                        desc.CullMode = SharpDX.Direct3D11.CullMode.None;
                        break;
                }

                desc.IsScissorEnabled = ScissorTestEnable;
                desc.IsMultisampleEnabled = MultiSampleAntiAlias;

                // discussion and explanation in https://github.com/MonoGame/MonoGame/issues/4826
                int depthMul;
                switch (device.ActiveDepthFormat)
                {
                    case DepthFormat.None:
                        depthMul = 0;
                        break;
                    case DepthFormat.Depth16:
                        depthMul = 1 << 16 - 1;
                        break;
                    case DepthFormat.Depth24:
                    case DepthFormat.Depth24Stencil8:
                        depthMul = 1 << 24 - 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                desc.DepthBias = (int) (DepthBias * depthMul);
                desc.SlopeScaledDepthBias = SlopeScaleDepthBias;

                if (FillMode == Graphics.FillMode.WireFrame)
                    desc.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                else
                    desc.FillMode = SharpDX.Direct3D11.FillMode.Solid;

                desc.IsDepthClipEnabled = DepthClipEnable;

                // These are new DX11 features we should consider exposing
                // as part of the extended MonoGame API.
                desc.IsFrontCounterClockwise = false;
                desc.IsAntialiasedLineEnabled = false;

                // To support feature level 9.1 these must
                // be set to these exact values.
                desc.DepthBiasClamp = 0.0f;

                // Create the state.
                _state = new SharpDX.Direct3D11.RasterizerState(GraphicsDevice._d3dDevice, desc);
            }

            Debug.Assert(GraphicsDevice == device, "The state was created for a different device!");

            // NOTE: We make the assumption here that the caller has
            // locked the d3dContext for us to use.

            // Apply the state!
            device._d3dContext.Rasterizer.State = _state;
        }

        partial void PlatformDispose()
        {
            SharpDX.Utilities.Dispose(ref _state);
        }
    }
}
