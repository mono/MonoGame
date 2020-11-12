#if OPENGL

using System;
using System.Collections.Generic;
using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{

    internal class ShaderProgram
    {
        public readonly int Program;

        private readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();

        public ShaderProgram(int program)
        {
            Program = program;
        }

        public int GetUniformLocation(string name)
        {
            if (_uniformLocations.ContainsKey(name))
                return _uniformLocations[name];

            var location = GL.GetUniformLocation(Program, name);
            GraphicsExtensions.CheckGLError();
            _uniformLocations[name] = location;
            return location;
        }
    }

    /// <summary>
    /// This class is used to Cache the links between Vertex/Pixel Shaders and Constant Buffers.
    /// It will be responsible for linking the programs under OpenGL if they have not been linked
    /// before. If an existing link exists it will be resused.
    /// </summary>
    internal class ShaderProgramCache : IDisposable
    {
        private readonly Dictionary<int, ShaderProgram> _programCache = new Dictionary<int, ShaderProgram>();
        GraphicsDevice _graphicsDevice;
        bool disposed;

        public ShaderProgramCache(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        ~ShaderProgramCache()
        {
            Dispose(false);
        }

        /// <summary>
        /// Clear the program cache releasing all shader programs.
        /// </summary>
        public void Clear()
        {
            foreach (var pair in _programCache)
            {
                _graphicsDevice.DisposeProgram(pair.Value.Program);
            }
            _programCache.Clear();
        }

        public ShaderProgram GetProgram(Shader vertexShader, Shader pixelShader, Shader hullShader, Shader domainShader, Shader geometryShader)
        {
            // TODO: We should be hashing in the mix of constant 
            // buffers here as well.  This would allow us to optimize
            // setting uniforms to only when a constant buffer changes.

            var key = vertexShader.HashKey ^ pixelShader.HashKey;
            if (hullShader != null && domainShader != null)
                key ^= hullShader.HashKey ^ domainShader.HashKey;
            if (geometryShader != null)
                key ^= geometryShader.HashKey;

            if (!_programCache.ContainsKey(key))
            {
                // the key does not exist so we need to link the programs
                _programCache.Add(key, Link(vertexShader, pixelShader, hullShader, domainShader, geometryShader));
            }

            return _programCache[key];
        }

        private ShaderProgram Link(Shader vertexShader, Shader pixelShader, Shader hullShader, Shader domainShader, Shader geometryShader)
        {
            // NOTE: No need to worry about background threads here
            // as this is only called at draw time when we're in the
            // main drawing thread.
            var program = GL.CreateProgram();
            GraphicsExtensions.CheckGLError();

            GL.AttachShader(program, vertexShader.GetShaderHandle());
            GraphicsExtensions.CheckGLError();

            GL.AttachShader(program, pixelShader.GetShaderHandle());
            GraphicsExtensions.CheckGLError();

            if (hullShader != null && domainShader != null)
            {
                GL.AttachShader(program, hullShader.GetShaderHandle());
                GraphicsExtensions.CheckGLError();

                GL.AttachShader(program, domainShader.GetShaderHandle());
                GraphicsExtensions.CheckGLError();
            }

            if (geometryShader != null)
            {
                GL.AttachShader(program, geometryShader.GetShaderHandle());
                GraphicsExtensions.CheckGLError();
            }

            //vertexShader.BindVertexAttributes(program);

            GL.LinkProgram(program);
            GraphicsExtensions.CheckGLError();

            GL.UseProgram(program);
            GraphicsExtensions.CheckGLError();

            vertexShader.GetVertexAttributeLocations(program);

            vertexShader.ApplySamplerTextureUnits(program);
            pixelShader.ApplySamplerTextureUnits(program);
            
            if (hullShader != null && domainShader != null)
            {
                hullShader.ApplySamplerTextureUnits(program);
                domainShader.ApplySamplerTextureUnits(program);
            }
            if (geometryShader != null)
                geometryShader.ApplySamplerTextureUnits(program);

            var linked = 0;

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out linked);
            GraphicsExtensions.LogGLError("ShaderProgramCache.Link(), GL.GetProgram");
            if (linked == (int)Bool.False)
            {
                var log = GL.GetProgramInfoLog(program);
                Console.WriteLine(log);

                GL.DetachShader(program, vertexShader.GetShaderHandle());
                GL.DetachShader(program, pixelShader.GetShaderHandle());

                if (hullShader != null && domainShader != null)
                {
                    GL.DetachShader(program, hullShader.GetShaderHandle());
                    GL.DetachShader(program, domainShader.GetShaderHandle());
                }
                if (geometryShader != null)
                    GL.DetachShader(program, geometryShader.GetShaderHandle());

                _graphicsDevice.DisposeProgram(program);
                throw new InvalidOperationException("Unable to link effect program");
            }

            return new ShaderProgram(program);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    Clear();
                disposed = true;
            }
        }
    }
}

#endif // OPENGL
