using System.Diagnostics;

namespace Microsoft.Xna.Framework.Graphics
{
    public class EffectPass
    {
        private readonly Effect _effect;

        private readonly Shader _vertexShader;
        private readonly Shader _pixelShader;
        private readonly Shader _hullShader;
        private readonly Shader _domainShader;
        private readonly Shader _geometryShader;

        private readonly BlendState _blendState;
        private readonly DepthStencilState _depthStencilState;
        private readonly RasterizerState _rasterizerState;

		public string Name { get; private set; }

        public EffectAnnotationCollection Annotations { get; private set; }

        internal EffectPass(    Effect effect, 
                                string name,
                                Shader vertexShader, 
                                Shader pixelShader,  
                                Shader hullShader,
                                Shader domainShader,
                                Shader geometryShader,
                                BlendState blendState, 
                                DepthStencilState depthStencilState, 
                                RasterizerState rasterizerState,
                                EffectAnnotationCollection annotations )
        {
            Debug.Assert(effect != null, "Got a null effect!");
            Debug.Assert(annotations != null, "Got a null annotation collection!");

            _effect = effect;

            Name = name;

            _vertexShader = vertexShader;
            _pixelShader = pixelShader;
            _hullShader = hullShader;
            _domainShader = domainShader;
            _geometryShader = geometryShader;

            _blendState = blendState;
            _depthStencilState = depthStencilState;
            _rasterizerState = rasterizerState;

            Annotations = annotations;
        }
        
        internal EffectPass(Effect effect, EffectPass cloneSource)
        {
            Debug.Assert(effect != null, "Got a null effect!");
            Debug.Assert(cloneSource != null, "Got a null cloneSource!");

            _effect = effect;

            // Share all the immutable types.
            Name = cloneSource.Name;
            _blendState = cloneSource._blendState;
            _depthStencilState = cloneSource._depthStencilState;
            _rasterizerState = cloneSource._rasterizerState;
            Annotations = cloneSource.Annotations;
            _vertexShader = cloneSource._vertexShader;
            _pixelShader = cloneSource._pixelShader;
            _hullShader = cloneSource._hullShader;
            _domainShader = cloneSource._domainShader;
            _geometryShader = cloneSource._geometryShader;
        }

        public void Apply()
        {
            // Set/get the correct shader handle/cleanups.

            var current = _effect.CurrentTechnique;
            _effect.OnApply();
            if (_effect.CurrentTechnique != current)
            {
                _effect.CurrentTechnique.Passes[0].Apply();
                return;
            }

            var device = _effect.GraphicsDevice;

            if (_vertexShader != null)
            {
                device.VertexShader = _vertexShader;

				// Update the texture parameters.
                SetShaderSamplers(_vertexShader, device.VertexTextures, device.VertexSamplerStates);

                // Update the constant buffers.
                for (var c = 0; c < _vertexShader.CBuffers.Length; c++)
                {
                    var cb = _effect.ConstantBuffers[_vertexShader.CBuffers[c]];
                    cb.Update(_effect.Parameters);
                    device.SetConstantBuffer(ShaderStage.Vertex, c, cb);
                }
            }

            if (_pixelShader != null)
            {
                device.PixelShader = _pixelShader;

                // Update the texture parameters.
                SetShaderSamplers(_pixelShader, device.Textures, device.SamplerStates);
                
                // Update the constant buffers.
                for (var c = 0; c < _pixelShader.CBuffers.Length; c++)
                {
                    var cb = _effect.ConstantBuffers[_pixelShader.CBuffers[c]];
                    cb.Update(_effect.Parameters);
                    device.SetConstantBuffer(ShaderStage.Pixel, c, cb);
                }
            }

            device.HullShader = _hullShader;

            if (_hullShader != null)
            {
                // Update the texture parameters, shared with Vertex shader state
                SetShaderSamplers(_hullShader, device.HullTextures, device.HullSamplerStates);

                for (var c = 0; c < _hullShader.CBuffers.Length; c++)
                {
                    var cb = _effect.ConstantBuffers[_hullShader.CBuffers[c]];
                    cb.Update(_effect.Parameters);
                    device.SetConstantBuffer(ShaderStage.Hull, c, cb);
                }
            }

            device.DomainShader = _domainShader;

            if (_domainShader != null)
            {
                // Update the texture parameters, shared with vertex shader state.
                SetShaderSamplers(_domainShader, device.DomainTextures, device.DomainSamplerStates);

                for (var c = 0; c < _domainShader.CBuffers.Length; c++)
                {
                    var cb = _effect.ConstantBuffers[_domainShader.CBuffers[c]];
                    cb.Update(_effect.Parameters);
                    device.SetConstantBuffer(ShaderStage.Domain, c, cb);
                }
            }

            device.GeometryShader = _geometryShader;

            if (_geometryShader != null)
            {
                // Update the constant buffers.
                SetShaderSamplers(_geometryShader, device.GeometryTextures, device.GeometrySamplerStates);

                for (var c = 0; c < _geometryShader.CBuffers.Length; c++)
                {
                    var cb = _effect.ConstantBuffers[_geometryShader.CBuffers[c]];
                    cb.Update(_effect.Parameters);
                    device.SetConstantBuffer(ShaderStage.Geometry, c, cb);
                }
            }

            // Set the render states if we have some.
            if (_rasterizerState != null)
                device.RasterizerState = _rasterizerState;
            if (_blendState != null)
                device.BlendState = _blendState;
            if (_depthStencilState != null)
                device.DepthStencilState = _depthStencilState;
        }

        private void SetShaderSamplers(Shader shader, TextureCollection textures, SamplerStateCollection samplerStates)
        {
            foreach (var sampler in shader.Samplers)
            {
                var param = _effect.Parameters[sampler.parameter];
                var texture = param.Data as Texture;

                textures[sampler.textureSlot] = texture;

                // If there is a sampler state set it.
                if (sampler.state != null)
                    samplerStates[sampler.samplerSlot] = sampler.state;
            }
        }
    }
}
