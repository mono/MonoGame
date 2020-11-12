// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MonoGame.Effect.TPGParser;

namespace MonoGame.Effect
{
    class DirectX11ShaderProfile : ShaderProfile
    {
        internal static readonly Regex HlslPixelShaderRegex = new Regex(@"^ps_(?<major>1|2|3|4|5)_(?<minor>0|1|)(_level_(?<exctension>9_1|9_2|9_3))?$", RegexOptions.Compiled);
        internal static readonly Regex HlslVertexShaderRegex = new Regex(@"^vs_(?<major>1|2|3|4|5)_(?<minor>0|1|)(_level_(?<exctension>9_1|9_2|9_3))?$", RegexOptions.Compiled);
        internal static readonly Regex HlslHullShaderRegex = new Regex(@"^hs_(?<major>1|2|3|4|5)_(?<minor>0|1|)(_level_(?<exctension>9_1|9_2|9_3))?$", RegexOptions.Compiled);
        internal static readonly Regex HlslDomainShaderRegex = new Regex(@"^ds_(?<major>1|2|3|4|5)_(?<minor>0|1|)(_level_(?<exctension>9_1|9_2|9_3))?$", RegexOptions.Compiled);
        internal static readonly Regex HlslGeometryShaderRegex = new Regex(@"^gs_(?<major>1|2|3|4|5)_(?<minor>0|1|)(_level_(?<exctension>9_1|9_2|9_3))?$", RegexOptions.Compiled);

        public DirectX11ShaderProfile()
            : base("DirectX_11", 1)
        {
        }

        internal override void AddMacros(Dictionary<string, string> macros, Options options)
        {
            macros.Add("HLSL", "1");
            macros.Add("SM4", "1");
        }

        internal override Regex GetShaderModelRegex(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.VertexShader:
                    return HlslVertexShaderRegex;
                case ShaderStage.PixelShader:
                    return HlslPixelShaderRegex;
                case ShaderStage.HullShader:
                    return HlslHullShaderRegex;
                case ShaderStage.DomainShader:
                    return HlslDomainShaderRegex;
                case ShaderStage.GeometryShader:
                    return HlslGeometryShaderRegex;
                default:
                    throw new Exception("GetShaderModelRegex: Unknown shader stage");
            }
        }

        internal override void ValidateShaderModels(PassInfo pass)
        {
            int major, minor;
            string extension;

            if (!string.IsNullOrEmpty(pass.vsFunction))
            {
                ParseShaderModel(pass.vsModel, HlslVertexShaderRegex, out major, out minor, out extension);
                if (major <= 3)
                    throw new Exception(String.Format("Invalid profile '{0}'. Vertex shader '{1}' must be SM 4.0 level 9.1 or higher!", pass.vsModel, pass.vsFunction));
            }

            if (!string.IsNullOrEmpty(pass.psFunction))
            {
                ParseShaderModel(pass.psModel, HlslPixelShaderRegex, out major, out minor, out extension);
                if (major <= 3)
                    throw new Exception(String.Format("Invalid profile '{0}'. Pixel shader '{1}' must be SM 4.0 level 9.1 or higher!", pass.vsModel, pass.psFunction));
            }

            if (!string.IsNullOrEmpty(pass.hsFunction))
            {
                ParseShaderModel(pass.hsModel, HlslHullShaderRegex, out major, out minor, out extension);
                if (major <= 4)
                    throw new Exception(String.Format("Invalid profile '{0}'. Hull shader '{1}' must be SM 5.0!", pass.hsModel, pass.hsFunction));
            }

            if (!string.IsNullOrEmpty(pass.dsFunction))
            {
                ParseShaderModel(pass.dsModel, HlslDomainShaderRegex, out major, out minor, out extension);
                if (major <= 4)
                    throw new Exception(String.Format("Invalid profile '{0}'. Domain shader '{1}' must be SM 5.0!", pass.vsModel, pass.dsFunction));
            }

            if (!string.IsNullOrEmpty(pass.gsFunction))
            {
                ParseShaderModel(pass.gsModel, HlslGeometryShaderRegex, out major, out minor, out extension);
                if (major <= 3)
                    throw new Exception(String.Format("Invalid profile '{0}'. Geometry shader '{1}' must be SM 4.0 or higher!", pass.gsModel, pass.gsFunction));
            }
        }

        internal override ShaderData CreateShader(ShaderResult shaderResult, string shaderFunction, string shaderProfile, ShaderStage shaderStage, EffectObject effect, ref string errorsAndWarnings)
        {
            var bytecode = EffectObject.CompileHLSL(shaderResult, shaderFunction, shaderProfile, ref errorsAndWarnings);

            // First look to see if we already created this same shader.
            foreach (var shader in effect.Shaders)
            {
                if (bytecode.SequenceEqual(shader.Bytecode))
                    return shader;
            }

            var shaderInfo = shaderResult.ShaderInfo;
            var shaderData = ShaderData.CreateHLSL(bytecode, shaderStage, effect.ConstantBuffers, effect.Shaders.Count, shaderInfo.SamplerStates, shaderResult.Debug);
            effect.Shaders.Add(shaderData);
            return shaderData;
        }
    }
}
