// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial class ConstantBuffer
    {
        private ShaderProgram _shaderProgram = null;
        private int _glBuffer = -1;
        private int _globalsArrayLocationMojo;  // MojoShader uses one big float4 array for all parameters
        private string[] _parameterShaderNames = null; // If uniform buffers are not available parameters need to be updated individually by name 

        static ConstantBuffer _lastConstantBufferApplied = null;

        private void PlatformInitialize()
        {
            if (!_effect._isMojoShader)
            {
                GL.GenBuffers(1, out _glBuffer);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformClear()
        {
            if (_glBuffer >= 0)
            {
                GL.DeleteBuffers(1, ref _glBuffer);
                GraphicsExtensions.CheckGLError();
            }

            _glBuffer = -1;
            _shaderProgram = null;
        }

        public unsafe void PlatformApply(GraphicsDevice device, ShaderProgram program)
        {
            // NOTE: We assume here the program has 
            // already been set on the device.

            // If the program changed then apply the state.
            if (_shaderProgram != program)
            {
                if (_effect._isMojoShader)
                {
                    var location = program.GetUniformLocation(_name);
                    if (location == -1)
                        return;

                    _globalsArrayLocationMojo = location;
                }

                _shaderProgram = program;
                _dirty = true;
            }

            // If the shader program is the same, the effect may still be different and have different values in the buffer
            if (!Object.ReferenceEquals(this, _lastConstantBufferApplied))
                _dirty = true;

            // If the buffer content hasn't changed then we're
            // done... use the previously set uniform state.
            if (!_dirty)
                return;

            if (_effect._isMojoShader)
            {
                fixed (byte* bytePtr = _buffer)
                {
                    // TODO: We need to know the type of buffer float/int/bool
                    // and cast this correctly... else it doesn't work as i guess
                    // GL is checking the type of the uniform.
                    GL.Uniform4(_globalsArrayLocationMojo, _buffer.Length / 16, (float*)bytePtr);
                    GraphicsExtensions.CheckGLError();
                }
            }
            else 
            {
                int uniformBlockIndex = GL.GetUniformBlockIndex == null ? -1 : GL.GetUniformBlockIndex(program.Program, _name);
                GraphicsExtensions.CheckGLError();

                if (uniformBlockIndex >= 0)
                {
                    GL.BindBuffer(BufferTarget.UniformBuffer, _glBuffer);
                    GraphicsExtensions.CheckGLError();

                    fixed (byte* bytePtr = _buffer)
                    {
                        GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)_buffer.Length, (IntPtr)bytePtr, BufferUsageHint.StreamDraw);
                        GraphicsExtensions.CheckGLError();
                    }

                    GL.BindBufferBase(BufferTarget.UniformBuffer, uniformBlockIndex, _glBuffer);
                    GraphicsExtensions.CheckGLError();
                }
                else // uniform buffers are not available, we have to update the parameters one by one
                    UpdateParametersIndividually(program);
            }
            
            // Clear the dirty flag.
            _dirty = false;

            _lastConstantBufferApplied = this;
        }

        private unsafe void UpdateParametersIndividually(ShaderProgram program)
        {
            if (_parameterShaderNames == null)
            {
                _parameterShaderNames = new string[_parameters.Length];

                for (int i = 0; i < _parameters.Length; i++)
                {
                    int paramIndex = _parameters[i];
                    var param = _effect.Parameters[paramIndex];

                    _parameterShaderNames[i] = _instanceName + "." + param.Name;

                    //if (param.Elements.Count > 1) // array
                    //    _parameterShaderNames[i] += "[0]";
                }
            }

            for (int i = 0; i < _parameters.Length; i++)
            {
                int paramIndex = _parameters[i];
                var param = _effect.Parameters[paramIndex];

                if (param.ParameterType == EffectParameterType.Texture ||
                    param.ParameterType == EffectParameterType.Texture1D ||
                    param.ParameterType == EffectParameterType.Texture2D ||
                    param.ParameterType == EffectParameterType.Texture3D ||
                    param.ParameterType == EffectParameterType.TextureCube ||
                    param.Elements.Count > 0)   // no arrays for now
                    continue;

                int location = program.GetUniformLocation(_parameterShaderNames[i]);
                if (location < 0)
                    continue;

                if (param.ParameterType == EffectParameterType.Int32 || param.ParameterType == EffectParameterType.Bool)
                {
                    GL.Uniform1i(location, ((int[])param.Data)[0]);
                }
                else
                {
                    fixed (float* floatPtr = &((float[])param.Data)[0])
                    {
                        switch (param.ParameterClass)
                        {
                            case EffectParameterClass.Scalar:
                                GL.Uniform1fv(location, 1, floatPtr);
                                break;

                            case EffectParameterClass.Vector:
                                switch (param.ColumnCount)
                                {
                                    case 2: GL.Uniform2fv(location, 1, floatPtr); break;
                                    case 3: GL.Uniform3fv(location, 1, floatPtr); break;
                                    case 4: GL.Uniform4fv(location, 1, floatPtr); break;
                                }
                                break;

                            case EffectParameterClass.Matrix:
                                switch (param.RowCount)
                                {
                                    case 2:
                                        switch (param.ColumnCount)
                                        {
                                            case 2: GL.UniformMatrix2fv(location, 1, false, floatPtr); break;
                                            case 3: GL.UniformMatrix2x3fv(location, 1, false, floatPtr); break;
                                            case 4: GL.UniformMatrix2x4fv(location, 1, false, floatPtr); break;
                                        }
                                        break;
                                    case 3:
                                        switch (param.ColumnCount)
                                        {
                                            case 2: GL.UniformMatrix3x2fv(location, 1, false, floatPtr); break;
                                            case 3: GL.UniformMatrix3fv(location, 1, false, floatPtr); break;
                                            case 4: GL.UniformMatrix3x4fv(location, 1, false, floatPtr); break;
                                        }
                                        break;
                                    case 4:
                                        switch (param.ColumnCount)
                                        {
                                            case 2: GL.UniformMatrix4x2fv(location, 1, false, floatPtr); break;
                                            case 3: GL.UniformMatrix4x3fv(location, 1, false, floatPtr); break;
                                            case 4: GL.UniformMatrix4fv(location, 1, false, floatPtr); break;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                }

                GraphicsExtensions.CheckGLError();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                PlatformClear();

            base.Dispose(disposing);
        }
    }
}
