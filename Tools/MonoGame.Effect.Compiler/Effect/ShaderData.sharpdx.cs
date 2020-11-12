using System.Collections.Generic;
using SharpDX.Direct3D;
using MonoGame.Effect.TPGParser;

namespace MonoGame.Effect
{
    internal partial class ShaderData
    {
        public static ShaderData CreateHLSL(byte[] byteCode, ShaderStage shaderStage, List<ConstantBufferData> cbuffers, int sharedIndex, Dictionary<string, SamplerStateInfo> samplerStates, bool debug)
        {
            var dxshader = new ShaderData(shaderStage, sharedIndex, byteCode);
            dxshader._attributes = new Attribute[0];

            // Strip the bytecode we're gonna save!
            var stripFlags = SharpDX.D3DCompiler.StripFlags.CompilerStripReflectionData |
                             SharpDX.D3DCompiler.StripFlags.CompilerStripTestBlobs;

            if (!debug)
                stripFlags |= SharpDX.D3DCompiler.StripFlags.CompilerStripDebugInformation;

            using (var original = new SharpDX.D3DCompiler.ShaderBytecode(byteCode))
            {
                // Strip the bytecode for saving to disk.
                var stripped = original.Strip(stripFlags);
                {
                    // Only SM4 and above works with strip... so this can return null!
                    if (stripped != null)
                    {
                        dxshader.ShaderCode = stripped;
                    }
                    else
                    {
                        // TODO: There is a way to strip SM3 and below
                        // but we have to write the method ourselves.
                        // 
                        // If we need to support it then consider porting
                        // this code over...
                        //
                        // http://entland.homelinux.com/blog/2009/01/15/stripping-comments-from-shader-bytecodes/
                        //
                        dxshader.ShaderCode = (byte[])dxshader.Bytecode.Clone();
                    }
                }

                // Use reflection to get details of the shader.
                using (var refelect = new SharpDX.D3DCompiler.ShaderReflection(byteCode))
                {
                    // Get the samplers.
                    var samplers = new List<Sampler>();
                    for (var i = 0; i < refelect.Description.BoundResources; i++)
                    {
                        var rdesc = refelect.GetResourceBindingDescription(i);
                        if (rdesc.Type == SharpDX.D3DCompiler.ShaderInputType.Texture)
                        {
                            var samplerName = rdesc.Name;

                            var sampler = new Sampler
                            {
                                samplerName = string.Empty,
                                textureSlot = rdesc.BindPoint,
                                samplerSlot = rdesc.BindPoint,
                                parameterName = samplerName
                            };
                            
                            SamplerStateInfo state;
                            if (samplerStates.TryGetValue(samplerName, out state))
                            {
                                sampler.parameterName = state.TextureName ?? samplerName;
                                sampler.state = state.State;
                            }
                            else
                            {
                                foreach (var s in samplerStates.Values)
                                {
                                    if (samplerName == s.TextureName)
                                    {
                                        sampler.state = s.State;
                                        samplerName = s.Name;
                                        break;
                                    }
                                }
                            }

                            // Find sampler slot, which can be different from the texture slot.
                            for (int j = 0; j < refelect.Description.BoundResources; j++)
                            {
                                var samplerrdesc = refelect.GetResourceBindingDescription(j);

                                if (samplerrdesc.Type == SharpDX.D3DCompiler.ShaderInputType.Sampler && 
                                    samplerrdesc.Name == samplerName)
                                {
                                    sampler.samplerSlot = samplerrdesc.BindPoint;
                                    break;
                                }
                            }

                            switch (rdesc.Dimension)
                            {
                                case ShaderResourceViewDimension.Texture1D:
                                case ShaderResourceViewDimension.Texture1DArray:
                                    sampler.type = MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_1D;
                                    break;

                                case ShaderResourceViewDimension.Texture2D:
                                case ShaderResourceViewDimension.Texture2DArray:
                                case ShaderResourceViewDimension.Texture2DMultisampled:
                                case ShaderResourceViewDimension.Texture2DMultisampledArray:
                                    sampler.type = MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_2D;
                                    break;

                                case ShaderResourceViewDimension.Texture3D:
                                    sampler.type = MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_VOLUME;
                                    break;

                                case ShaderResourceViewDimension.TextureCube:
                                case ShaderResourceViewDimension.TextureCubeArray:
                                    sampler.type = MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_CUBE;
                                    break;
                            }

                            samplers.Add(sampler);
                        }
                    }
                    dxshader._samplers = samplers.ToArray();

                    // Gather all the constant buffers used by this shader.
                    dxshader._cbuffers = new int[refelect.Description.ConstantBuffers];
                    for (var i = 0; i < refelect.Description.ConstantBuffers; i++)
                    {
                        var cb = new ConstantBufferData(refelect.GetConstantBuffer(i));

                        // Look for a duplicate cbuffer in the list.
                        for (var c = 0; c < cbuffers.Count; c++)
                        {
                            if (cb.SameAs(cbuffers[c]))
                            {
                                cb = null;
                                dxshader._cbuffers[i] = c;
                                break;
                            }
                        }

                        // Add a new cbuffer.
                        if (cb != null)
                        {
                            dxshader._cbuffers[i] = cbuffers.Count;
                            cbuffers.Add(cb);
                        }
                    }
                }
            }

            return dxshader;
        }
    }
}
