using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace MonoGame.Effect.TPGParser
{
    public class PassInfo
    {
        public string name;

        public string vsModel;
        public string vsFunction;

        public string psModel;
        public string psFunction;

        public string hsModel;
        public string hsFunction;

        public string dsModel;
        public string dsFunction;

        public string gsModel;
        public string gsFunction;

        public BlendState blendState;
        public RasterizerState rasterizerState;
        public DepthStencilState depthStencilState;
		
        private static Blend ToAlphaBlend(Blend blend)
        {
            switch (blend)
            {
                case Blend.SourceColor:
                    return Blend.SourceAlpha;
                case Blend.InverseSourceColor:
                    return Blend.InverseSourceAlpha;
                case Blend.DestinationColor:
                    return Blend.DestinationAlpha;
                case Blend.InverseDestinationColor:
                    return Blend.InverseDestinationAlpha;
            }
            return blend;
        }

        public bool AlphaBlendEnable
        {
            set
            {
                if (value)
                {
                    if (blendState == null)
                    {
                        blendState = new BlendState();
                        blendState.ColorSourceBlend = Blend.One;
                        blendState.AlphaSourceBlend = Blend.One;
                        blendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
                        blendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                    }
                }
                else if (!value)
                {
                    if (blendState == null)
                        blendState = new BlendState();
                    blendState.ColorSourceBlend = Blend.One;
                    blendState.AlphaSourceBlend = Blend.One;
                    blendState.ColorDestinationBlend = Blend.Zero;
                    blendState.AlphaDestinationBlend = Blend.Zero;
                }
            }
        }

        public FillMode FillMode
        {
            set
            {
                if (rasterizerState == null)
                    rasterizerState = new RasterizerState();
                rasterizerState.FillMode = value;             
            }
        }

        public CullMode CullMode
        {
            set
            {
                if (rasterizerState == null)
                    rasterizerState = new RasterizerState();
                rasterizerState.CullMode = value;
            }
        }

        public bool ZEnable
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.DepthBufferEnable = value;
            }
        }

        public bool ZWriteEnable
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.DepthBufferWriteEnable = value;
            }
        }

        public CompareFunction DepthBufferFunction
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.DepthBufferFunction = value;
            }
        }

        public bool MultiSampleAntiAlias
        {
            set
            {
                if (rasterizerState == null)
                    rasterizerState = new RasterizerState();
                rasterizerState.MultiSampleAntiAlias = value;
            }
        }

        public bool ScissorTestEnable
        {
            set
            {
                if (rasterizerState == null)
                    rasterizerState = new RasterizerState();
                rasterizerState.ScissorTestEnable = value;
            }
        }

        public bool StencilEnable
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.StencilEnable = value;
            }
        }

        public StencilOperation StencilFail
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.StencilFail = value;
            }
        }

        public CompareFunction StencilFunc
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.StencilFunction = value;
            }
        }

        public int StencilMask
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.StencilMask = value;
            }
        }

        public StencilOperation StencilPass
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.StencilPass = value;
            }
        }

        public int StencilRef
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.ReferenceStencil = value;
            }
        }

        public int StencilWriteMask
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.StencilWriteMask = value;
            }
        }

        public StencilOperation StencilZFail
        {
            set
            {
                if (depthStencilState == null)
                    depthStencilState = new DepthStencilState();
                depthStencilState.StencilDepthBufferFail = value;
            }
        }

        public Blend SrcBlend
        {
            set
            {
                if (blendState == null)
                    blendState = new BlendState();
                blendState.ColorSourceBlend = value;
                blendState.AlphaSourceBlend = ToAlphaBlend(value);
            }
        }

        public Blend DestBlend
        {
            set
            {
                if (blendState == null)
                    blendState = new BlendState();
                blendState.ColorDestinationBlend = value;
                blendState.AlphaDestinationBlend = ToAlphaBlend(value);
            }
        }

        public BlendFunction BlendOp
        {
            set
            {
                if (blendState == null)
                    blendState = new BlendState();
                blendState.AlphaBlendFunction = value;
            }
        }

        public ColorWriteChannels ColorWriteEnable
        {
            set
            {
                if (blendState == null)
                    blendState = new BlendState();
                blendState.ColorWriteChannels = value;
            }    
        }

        public float DepthBias
        {
            set
            {
                if (rasterizerState == null)
                    rasterizerState = new RasterizerState();
                rasterizerState.DepthBias = value;
            }
        }

        public float SlopeScaleDepthBias
        {
            set
            {
                if (rasterizerState == null)
                    rasterizerState = new RasterizerState();
                rasterizerState.SlopeScaleDepthBias = value;
            }
        }
    }
}
