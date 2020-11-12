// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using MonoGame.Effect.TPGParser;

namespace MonoGame.Effect
{
    class OpenGLShaderProfile : ShaderProfile
    {
        private bool _useMojo;

        protected virtual bool IsESSL => false;

        private static readonly Regex GlslPixelShaderRegex = DirectX11ShaderProfile.HlslPixelShaderRegex;
        private static readonly Regex GlslVertexShaderRegex = DirectX11ShaderProfile.HlslVertexShaderRegex;
        private static readonly Regex GlslHullShaderRegex = DirectX11ShaderProfile.HlslHullShaderRegex;
        private static readonly Regex GlslDomainShaderRegex = DirectX11ShaderProfile.HlslDomainShaderRegex;
        private static readonly Regex GlslGeometryShaderRegex = DirectX11ShaderProfile.HlslGeometryShaderRegex;

        public OpenGLShaderProfile()
            : base("OpenGL", 0)
        {
        }

        protected OpenGLShaderProfile(string name)
            : base(name, 0)
        {
        }

        internal override void AddMacros(Dictionary<string, string> macros, Options options)
        {
            macros.Add("GLSL", "1");
            macros.Add("OPENGL", "1");

            _useMojo = options.IsDefined("MOJO");
            if (!_useMojo)
                macros.Add("SM4", "1");
        }

        internal override Regex GetShaderModelRegex(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.VertexShader:
                    return GlslVertexShaderRegex;
                case ShaderStage.PixelShader:
                    return GlslPixelShaderRegex;
                case ShaderStage.HullShader:
                    return GlslHullShaderRegex;
                case ShaderStage.DomainShader:
                    return GlslDomainShaderRegex;
                case ShaderStage.GeometryShader:
                    return GlslGeometryShaderRegex;
                default:
                    throw new Exception("GetShaderModelRegex: Unknown shader stage");
            }
        }

        internal override void ValidateShaderModels(PassInfo pass)
        {
            int maxSM = _useMojo ? 3 : 5;
            if (_useMojo)
            {
                int major, minor;
                string extension;

                if (!string.IsNullOrEmpty(pass.vsFunction))
                {
                    ParseShaderModel(pass.vsModel, GlslVertexShaderRegex, out major, out minor, out extension);
                    if (major > maxSM)
                        throw new Exception(String.Format("Invalid profile '{0}'. Vertex shader '{1}' must be SM {2}.0 or lower!", pass.vsModel, pass.vsFunction, maxSM));
                }

                if (!string.IsNullOrEmpty(pass.psFunction))
                {
                    ParseShaderModel(pass.psModel, GlslPixelShaderRegex, out major, out minor, out extension);
                    if (major > maxSM)
                        throw new Exception(String.Format("Invalid profile '{0}'. Pixel shader '{1}' must be SM {2}.0 or lower!", pass.vsModel, pass.psFunction, maxSM));
                }
            }
        }

        internal override ShaderData CreateShader(ShaderResult shaderResult, string shaderFunction, string shaderProfile, ShaderStage shaderStage, EffectObject effect, ref string errorsAndWarnings)
        {
            if (_useMojo)
            {
                // For now GLSL is only supported via translation
                // using MojoShader which works from HLSL bytecode.
                var bytecode = EffectObject.CompileHLSL(shaderResult, shaderFunction, shaderProfile, ref errorsAndWarnings);

                // First look to see if we already created this same shader.
                foreach (var shader in effect.Shaders)
                {
                    if (bytecode.SequenceEqual(shader.Bytecode))
                        return shader;
                }

                var shaderInfo = shaderResult.ShaderInfo;
                var shaderData = ShaderData.CreateGLSL_Mojo(bytecode, shaderStage, effect.ConstantBuffers, effect.Shaders.Count, shaderInfo.SamplerStates, shaderResult.Debug);
                effect.Shaders.Add(shaderData);

                return shaderData;
            }
            else
            {
                ParseShaderModel(shaderProfile, GetShaderModelRegex(shaderStage), out int smMajor, out int smMinor, out string smExtension);

                var shaderInfo = shaderResult.ShaderInfo;
                var sourceCode = shaderResult.FileContent;

                var shaderData = ShaderData.CreateGLSL_Conductor(sourceCode, effect.Shaders.Count,
                    shaderStage, shaderFunction,
                    smMajor, smMinor, smExtension,
                    effect.ConstantBuffers, shaderInfo.SamplerStates,
                    shaderResult.Debug, IsESSL);

                // See if we already created this same shader.
                foreach (var shader in effect.Shaders)
                {
                    if (shaderData.ShaderCode.SequenceEqual(shader.ShaderCode))
                        return shader;
                }

                effect.Shaders.Add(shaderData);
                return shaderData;
            }
        }

        internal void MakeSeparateSamplersForDifferentTextures(List<ShaderData> shaders)
        {
            // MojoShader handles this differently
            if (_useMojo)
                return;

            // When a sampler samples from multiple textures, we have to create separate samplers for every texture.
            // Each of these samplers needs a unique sampler slot assigned.
            // The samplerSlotMapping dictionary maps from (originalDXSamplerSlot, textureSlot) to uniqueSamplerSlot.
            var samplerSlotMapping = new Dictionary<(int, int), int>();

            // Assign sampler slots in two passes.
            // In the first pass we fill up the samplerSlotMapping dictionary with original DX sampler slots only.
            // Original DX sampler slots have priority over newly created sampler slots, because we want to keep DX register bindings intact.
            // In the second pass we will then also add new sampler slots for multi-texture samplers.
            for (int pass = 0; pass < 2; pass++)
            {
                foreach (var shader in shaders)
                {
                    for (int i = 0; i < shader._samplers.Length; i++)
                    {
                        var sampler = shader._samplers[i];

                        // Check if we already assigned a unique slot index to the current sampler-texture combination, if not, assign one.
                        if (!samplerSlotMapping.TryGetValue((sampler.samplerSlot, sampler.textureSlot), out int uniqueSamplerSlot))
                        {
                            // If the original DX sampler slot is still available, keep it, that way we keep register bindings intact.
                            if (!samplerSlotMapping.ContainsValue(sampler.samplerSlot))
                            {
                                uniqueSamplerSlot = sampler.samplerSlot;
                                samplerSlotMapping.Add((sampler.samplerSlot, sampler.textureSlot), uniqueSamplerSlot);
                            }
                            else
                            {
                                // The original DX sampler slot is already used by another sampler-texture combination
                                // We have to find a new free sampler slot, but only in the 2nd pass. 
                                if (pass == 1)
                                {
                                    uniqueSamplerSlot = 1;
                                    while (samplerSlotMapping.ContainsValue(uniqueSamplerSlot))
                                        uniqueSamplerSlot++;

                                    samplerSlotMapping.Add((sampler.samplerSlot, sampler.textureSlot), uniqueSamplerSlot);
                                }
                            }
                        }

                        // assign new slots in 2nd pass
                        if (pass == 1)  
                            sampler.samplerSlot = uniqueSamplerSlot;
                    }
                }
            }
        }
    }

    class OpenGLESShaderProfile : OpenGLShaderProfile
    {
        protected override bool IsESSL => true;

        public OpenGLESShaderProfile()
            : base("OpenGLES")
        {
        }

        internal override void AddMacros(Dictionary<string, string> macros, Options options)
        {
            base.AddMacros(macros, options);
            macros.Add("ESSL", "1");
        }
    }
}
