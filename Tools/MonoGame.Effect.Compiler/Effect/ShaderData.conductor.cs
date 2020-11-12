using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using MonoGame.Effect.TPGParser;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Effect
{
	internal partial class ShaderData
	{
        public static ShaderData CreateGLSL_Conductor(string sourceCode, int sharedIndex, 
            ShaderStage shaderStage, string shaderFunction,
            int shaderModelMajor, int shaderModelMinor, string shaderModelExtension,
            List<ConstantBufferData> cbuffers, Dictionary<string, SamplerStateInfo> samplerStates,
            bool debug, bool isESSL)
        {
            var shaderData = new ShaderData(shaderStage, sharedIndex, new byte[0]);

            //==============================================================
            // Setup input data for ShaderConductor
            //==============================================================
            var sourceDesc = new ShaderConductor.SourceDesc()
            {
                entryPoint = shaderFunction,
                source = sourceCode,
                stage = ConvertToConductorShaderStage(shaderStage),
            };

            int shaderModelMajorDX = Math.Max(shaderModelMajor, 4);
            int shaderModelMinorDX = shaderModelMajor < 4 ? 0 : shaderModelMinor;

            var options = ShaderConductor.OptionsDesc.Default;
            options.enableDebugInfo = false;
            options.shaderModel = new ShaderConductor.ShaderModel(shaderModelMajorDX, shaderModelMinorDX);

            //==============================================================
            // Choose best GLSL/ESSL target version based on DX shader model
            //==============================================================
            string glslVersion = "";
            switch (shaderModelMajor)
            {   
                case 0: //                                   GLSL    ESSL                 OpenGL (Shader)       OpenGL ES (Shader)                                                            
                case 1:                                                                          
                case 2:             glslVersion = !isESSL ? "110" : "100";    break; //   2.0                   2.0
                case 3:             glslVersion = !isESSL ? "110" : "100";    break; //   2.0                   2.0
                case 4:                                                                     
                    switch (shaderModelExtension)                                           
                    {                                                                       
                        case "9_1": glslVersion = !isESSL ? "110" : "100";    break; //   2.0                   2.0
                        case "9_2": glslVersion = !isESSL ? "110" : "100";    break; //   2.0                   2.0
                        case "9_3": glslVersion = !isESSL ? "110" : "100";    break; //   2.0                   2.0            
                        default:    glslVersion = !isESSL ? "330" : "300 es"; break; //   3.3 (GS, TESS)        3.0              
                    }
                    break;
                default:            glslVersion = !isESSL ? "430" : "320 es"; break; //   4.3 (GS, TESS, CS)    3.2 (GS, TESS, CS)       
            }

            var target = new ShaderConductor.TargetDesc
            {
                language = isESSL ? ShaderConductor.ShadingLanguage.Essl : ShaderConductor.ShadingLanguage.Glsl,
                version = glslVersion,  
                asModule = false,
            };

            //==============================================================
            // Cross compile HLSL source code to GLSL using ShaderConductor
            // 1.) DirectXShaderCompiler converts HLSL to SPIR-V
            // 2.) SPIRV-CROSS converts SPIR-V to GLSL
            //==============================================================

            // Change working directory, otherwise ShaderConductor can't load dlls
            string previousWorkingDir = System.IO.Directory.GetCurrentDirectory();
            string newWorkingDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            System.IO.Directory.SetCurrentDirectory(newWorkingDir);

            ShaderConductor.Compile(ref sourceDesc, ref options, ref target, out ShaderConductor.ResultDesc result);

            System.IO.Directory.SetCurrentDirectory(previousWorkingDir); 

            //==============================================================
            // Handle compiler errors
            //==============================================================
            if (result.hasError)
            {
                IntPtr errorBlob = ShaderConductor.GetShaderConductorBlobData(result.errorWarningMsg);
                int errorBlobSize = ShaderConductor.GetShaderConductorBlobSize(result.errorWarningMsg);
                string errorMsg = Marshal.PtrToStringAnsi(errorBlob, errorBlobSize);
                throw new Exception(errorMsg);
            }
            else
            {
                if (!result.isText)
                    throw new Exception("ShaderConductor result is not text");

                //==============================================================
                // Get the GLSL code out of the ShaderConductor blob
                //==============================================================
                var targetBlob = ShaderConductor.GetShaderConductorBlobData(result.target);
                int targetBlobSize = ShaderConductor.GetShaderConductorBlobSize(result.target);
                string glsl = Marshal.PtrToStringAnsi(targetBlob, targetBlobSize);

                //==============================================================
                // Apply some fixes to the GLSL code, then add it to shaderData
                //==============================================================

                // version header for old OpenGL versions can make OpenGL ES crash.
                if (isESSL && !glslVersion.Contains("es"))
                    GLSLManipulator.RemoveVersionHeader(ref glsl);

                // remove separate shader objects extension requirement for OpenGL 2
                if (glslVersion.StartsWith("10") ||
                    glslVersion.StartsWith("11") ||
                    glslVersion.StartsWith("12"))
                    GLSLManipulator.RemoveARBSeparateShaderObjects(ref glsl);

                if (shaderStage == ShaderStage.VertexShader)
                {
                    // OpenGL doesn't like the gl_PerVertex declaration.
                    GLSLManipulator.RemoveOutGlPerVertex(ref glsl);
                    // Add posFixup code, so we can compensate for differences btw DirectX and OpenGL
                    GLSLManipulator.AddPosFixupUniformAndCode(ref glsl);
                }

                if (shaderStage == ShaderStage.GeometryShader ||
                    shaderStage == ShaderStage.HullShader)
                {
                    // OpenGL doesn't like the gl_PerVertex declaration.
                    GLSLManipulator.RemoveInGlPerVertex(ref glsl);
                }

                shaderData.ShaderCode = Encoding.ASCII.GetBytes(glsl);

                //==============================================================
                // Add attributes (stage inputs) to shaderData
                //==============================================================
                if (shaderStage == ShaderStage.VertexShader)
                {
                    var stageInputs = ShaderConductor.GetStageInputs(result);
                    shaderData._attributes = new Attribute[stageInputs.Count];

                    for (int i = 0; i < stageInputs.Count; i++)
                    {
                        shaderData._attributes[i].name = stageInputs[i].name;
                        shaderData._attributes[i].location = stageInputs[i].location;
                        shaderData._attributes[i].usage = GetVertexElementUsageFromStageInput(stageInputs[i]);
                        shaderData._attributes[i].index = stageInputs[i].index;
                    }
                }
                else
                    shaderData._attributes = new Attribute[0];

                //==============================================================
                // Add constant buffers (uniform buffers) to shaderData
                //==============================================================
                var uniformBuffers = ShaderConductor.GetUniformBuffers(result);
                shaderData._cbuffers = new int[uniformBuffers.Count];

                for (var i = 0; i < uniformBuffers.Count; i++)
                {
                    var cb = new ConstantBufferData(uniformBuffers[i]);

                    // Look for a duplicate cbuffer in the list
                    for (var c = 0; c < cbuffers.Count; c++)
                    {
                        if (cb.SameAs(cbuffers[c]))
                        {
                            cb = null;
                            shaderData._cbuffers[i] = c;
                            break;
                        }
                    }

                    // Add a new cbuffer
                    if (cb != null)
                    {
                        shaderData._cbuffers[i] = cbuffers.Count;
                        cbuffers.Add(cb);
                    }
                }

                //==============================================================
                // Add samplers to shaderData
                //==============================================================
                var samplers = ShaderConductor.GetSamplers(result);
                shaderData._samplers = new Sampler[samplers.Count];

                for (int i = 0; i < samplers.Count; i++)
                {
                    shaderData._samplers[i].samplerName = samplers[i].name;
                    shaderData._samplers[i].parameterName = samplers[i].textureName;
                    shaderData._samplers[i].samplerSlot = samplers[i].slot;
                    shaderData._samplers[i].textureSlot = samplers[i].textureSlot;
                    shaderData._samplers[i].type = ConvertSamplerTypeToMojo(samplers[i]);
                    shaderData._samplers[i].parameter = -1; //sampler mapping to parameter is unknown atm

                    if (samplerStates.TryGetValue(samplers[i].originalName, out SamplerStateInfo state))
                        shaderData._samplers[i].state = state.State;
                }
            }

            //==============================================================
            // Cleanup
            //==============================================================
            ShaderConductor.DestroyShaderConductorBlob(result.target);
            ShaderConductor.DestroyShaderConductorBlob(result.errorWarningMsg);

            return shaderData;
        }

        //==============================================================
        // Conversions btw. MonoGame and ShaderConductor
        //==============================================================
        public static ShaderConductor.ShaderStage ConvertToConductorShaderStage(Effect.ShaderStage stage)
        {
            switch (stage)
            {
                case Effect.ShaderStage.VertexShader: return ShaderConductor.ShaderStage.VertexShader;
                case Effect.ShaderStage.PixelShader: return ShaderConductor.ShaderStage.PixelShader;
                case Effect.ShaderStage.HullShader: return ShaderConductor.ShaderStage.HullShader;
                case Effect.ShaderStage.DomainShader: return ShaderConductor.ShaderStage.DomainShader;
                case Effect.ShaderStage.GeometryShader: return ShaderConductor.ShaderStage.GeometryShader;
                default:
                    throw new Exception("Shader stage conversion failed.");
            }
        }

        public static MojoShader.MOJOSHADER_samplerType ConvertSamplerTypeToMojo(ShaderConductor.Sampler sampler)
        {
            switch (sampler.type)
            {
                case 0: return MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_1D;
                case 1: return MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_2D;
                case 2: return MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_VOLUME;
                case 3: return MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_CUBE;
                default:
                    return MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_UNKNOWN;
            }
        }

        public static VertexElementUsage GetVertexElementUsageFromStageInput(ShaderConductor.StageInput stageInput)
        {
            switch (stageInput.usage)
            {
                case "BINORMAL":
                    return VertexElementUsage.Binormal;
                case "BLENDINDICES":
                    return VertexElementUsage.BlendIndices;
                case "BLENDWEIGHT":
                    return VertexElementUsage.BlendWeight;
                case "COLOR":
                    return VertexElementUsage.Color;
                case "NORMAL":
                    return VertexElementUsage.Normal;
                case "POSITION":
                    return VertexElementUsage.Position;
                case "PSIZE":
                    return VertexElementUsage.PointSize;
                case "TANGENT":
                    return VertexElementUsage.Tangent;
                case "TEXCOORD":
                    return VertexElementUsage.TextureCoordinate;
                case "FOG":
                    return VertexElementUsage.Fog;
                case "TESSFACTOR":
                    return VertexElementUsage.TessellateFactor;
                case "DEPTH":
                    return VertexElementUsage.Depth;
                case "SAMPLE":
                    return VertexElementUsage.Sample;
                default:
                    throw new Exception("No VertexElementUsage found for attribute " + stageInput.usage);
            }
        }
    }
}
