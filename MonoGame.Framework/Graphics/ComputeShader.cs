﻿#region License
/*
Microsoft Public License (Ms-PL)
MonoGame - Copyright © 2009-2013 The MonoGame Team

All rights reserved.

This license governs use of the accompanying software. If you use the software,
you accept this license. If you do not accept the license, do not use the
software.

1. Definitions

The terms "reproduce," "reproduction," "derivative works," and "distribution"
have the same meaning here as under U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the
software.

A "contributor" is any person that distributes its contribution under this
license.

"Licensed patents" are a contributor's patent claims that read directly on its
contribution.

2. Grant of Rights

(A) Copyright Grant- Subject to the terms of this license, including the
license conditions and limitations in section 3, each contributor grants you a
non-exclusive, worldwide, royalty-free copyright license to reproduce its
contribution, prepare derivative works of its contribution, and distribute its
contribution or any derivative works that you create.

(B) Patent Grant- Subject to the terms of this license, including the license
conditions and limitations in section 3, each contributor grants you a
non-exclusive, worldwide, royalty-free license under its licensed patents to
make, have made, use, sell, offer for sale, import, and/or otherwise dispose of
its contribution in the software or derivative works of the contribution in the
software.

3. Conditions and Limitations

(A) No Trademark License- This license does not grant you rights to use any
contributors' name, logo, or trademarks.

(B) If you bring a patent claim against any contributor over patents that you
claim are infringed by the software, your patent license from such contributor
to the software ends automatically.

(C) If you distribute any portion of the software, you must retain all
copyright, patent, trademark, and attribution notices that are present in the
software.

(D) If you distribute any portion of the software in source code form, you may
do so only under this license by including a complete copy of this license with
your distribution. If you distribute any portion of the software in compiled or
object code form, you may only do so under a license that complies with this
license.

(E) The software is licensed "as-is." You bear the risk of using it. The
contributors give no express warranties, guarantees or conditions. You may have
additional consumer rights under your local laws which this license cannot
change. To the extent permitted under your local laws, the contributors exclude
the implied warranties of merchantability, fitness for a particular purpose and
non-infringement.
*/
#endregion License

using System;

#if DIRECTX
    using SharpDX.Direct3D11;
#else
    //using OpenGL4Net;
#endif

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed class ComputeShader : IDisposable
    {
#if DIRECTX
        private SharpDX.Direct3D11.ComputeShader computeShader;
        private DeviceContext context;
#elif OPENGL 
        private uint program = 0U; 
#endif

        public ComputeShader(GraphicsDevice graphics, byte[] bytecode)
        {
#if DIRECTX
            Device device = (Device) graphics.Handle;
            computeShader = new SharpDX.Direct3D11.ComputeShader(device, bytecode);
            context = device.ImmediateContext;
#elif OPENGL  
            //uint shader = gl.CreateShader(GL.COMPUTE_SHADER); 
           
            //gl.ShaderSource(shader, Encoding.Unicode.GetString(bytecode));
            //gl.CompileShader(shader);
             
            //StringBuilder log = new StringBuilder(4096);
            //gl.GetShaderInfoLog(shader, sizeof(char)*4096, null, log);
  
            //program = gl.CreateProgram();
            //gl.AttachShader(program, shader);
            //gl.DeleteShader(shader);
#endif
        }

        public void Compute(int threadGroupCountX, int threadGroupCountY, int threadGroupCountZ)
        {
#if DIRECTX
            context.ComputeShader.Set(computeShader);
            context.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
#elif OPENGL
            //if (program != 0)
            //{
            //    gl.UseProgram(program);
            //    gl.DispatchCompute((uint) threadGroupCountX, (uint) threadGroupCountY, (uint) threadGroupCountZ);
            //}
#endif
        }

        public void SetOutputSurface(ComputeSurface2D surface)
        {
#if DIRECTX
            context.ComputeShader.SetUnorderedAccessView(0, surface._view);
#elif OPENGL
            //gl.UseProgram(program);
            //gl.BindImageTexture(0, surface.texture, 0, false, 0, GL.WRITE_ONLY, GL.RGBA32F);
#endif
        }

        ~ComputeShader()
        {
#if DIRECTX
            if (computeShader != null)
            {
                computeShader.Dispose();
            }
#elif OPENGL
            //if (program != 0)
            //{
            //    gl.DeleteProgram(program); 
            //}
#endif
        }

        public void Dispose()
        {
#if DIRECTX
            if (computeShader != null)
            {
                computeShader.Dispose();
            }
#else
            //if (program != 0)
            //{
            //    gl.DeleteProgram(program);
            //}
#endif
            GC.SuppressFinalize(this);
        }
    }
}