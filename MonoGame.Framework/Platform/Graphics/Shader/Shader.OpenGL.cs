// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Diagnostics;
using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial class Shader
    {
        // The shader handle.
        private int _shaderHandle = -1;

        // We keep this around for recompiling on context lost and debugging.
        private string _glslCode;

        private static int PlatformProfile()
        {
            return 0;
        }

        private void PlatformConstruct(ShaderStage stage, byte[] shaderBytecode)
        {
            _glslCode = System.Text.Encoding.ASCII.GetString(shaderBytecode);

            HashKey = MonoGame.Framework.Utilities.Hash.ComputeHash(shaderBytecode);
        }

        internal int GetShaderHandle()
        {
            // If the shader has already been created then return it.
            if (_shaderHandle != -1)
                return _shaderHandle;

            //
            _shaderHandle = GL.CreateShader(ShaderStageToOGLShaderType(Stage));
            GraphicsExtensions.CheckGLError();
            GL.ShaderSource(_shaderHandle, _glslCode);
            GraphicsExtensions.CheckGLError();
            GL.CompileShader(_shaderHandle);
            GraphicsExtensions.CheckGLError();
            int compiled = 0;
            GL.GetShader(_shaderHandle, ShaderParameter.CompileStatus, out compiled);
            GraphicsExtensions.CheckGLError();
            if (compiled != (int)Bool.True)
            {
                var log = GL.GetShaderInfoLog(_shaderHandle);
                Debug.WriteLine(log);

                GraphicsDevice.DisposeShader(_shaderHandle);
                _shaderHandle = -1;

                throw new InvalidOperationException("Shader Compilation Failed");
            }

            return _shaderHandle;
        }

        internal void GetVertexAttributeLocations(int program)
        {
            for (int i = 0; i < Attributes.Length; ++i)
            {
                Attributes[i].location = GL.GetAttribLocation(program, Attributes[i].name);
                GraphicsExtensions.CheckGLError();
            }
        }

        internal int GetAttribLocation(VertexElementUsage usage, int index)
        {
            for (int i = 0; i < Attributes.Length; ++i)
            {
                if ((Attributes[i].usage == usage) && (Attributes[i].index == index))
                    return Attributes[i].location;
            }
            return -1;
        }

        internal void ApplySamplerTextureUnits(int program)
        {
            // Assign the texture unit index to the sampler uniforms.
            foreach (var sampler in Samplers)
            {
                var loc = GL.GetUniformLocation(program, sampler.name);
                GraphicsExtensions.CheckGLError();
                if (loc != -1)
                {
                    GL.Uniform1(loc, sampler.samplerSlot);
                    GraphicsExtensions.CheckGLError();
                }
            }
        }

        private void PlatformGraphicsDeviceResetting()
        {
            if (_shaderHandle != -1)
            {
                GraphicsDevice.DisposeShader(_shaderHandle);
                _shaderHandle = -1;
            }
        }

        private ShaderType ShaderStageToOGLShaderType(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return ShaderType.VertexShader;
                case ShaderStage.Pixel:
                    return ShaderType.FragmentShader;
                case ShaderStage.Hull:
                    return ShaderType.TesselationControlShader;
                case ShaderStage.Domain:
                    return ShaderType.TesselationEvaluationShader;
                case ShaderStage.Geometry:
                    return ShaderType.GeometryShader;
                default:
                    throw new ArgumentException("ShaderStage not supported: " + stage);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && _shaderHandle != -1)
            {
                GraphicsDevice.DisposeShader(_shaderHandle);
                _shaderHandle = -1;
            }

            base.Dispose(disposing);
        }
    }
}
