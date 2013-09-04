﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#if OPENGL

#if MONOMAC
using MonoMac.OpenGL;
#elif WINDOWS || LINUX
using OpenTK.Graphics.OpenGL;
#elif GLES
using OpenTK.Graphics.ES20;
using FramebufferAttachment = OpenTK.Graphics.ES20.All;
using FramebufferErrorCode = OpenTK.Graphics.ES20.All;
using FramebufferParameterName = OpenTK.Graphics.ES20.All;
using FramebufferTarget = OpenTK.Graphics.ES20.All;
using GenerateMipmapTarget = OpenTK.Graphics.ES20.All;
using RenderbufferParameterName = OpenTK.Graphics.ES20.All;
using RenderbufferStorage = OpenTK.Graphics.ES20.All;
using RenderbufferTarget = OpenTK.Graphics.ES20.All;
using TextureTarget = OpenTK.Graphics.ES20.All;
#endif

namespace Microsoft.Xna.Framework.Graphics.GLWrappers
{
    /// <summary>
    /// Wrapper implementation for ARB versions of OpenGL framebuffer object (FBO) methods
    /// </summary>
    internal class ArbFboWrapper : IFboWrapper
    {
        public bool IsRenderbuffer(int renderbuffer)
        {
            return GL.IsRenderbuffer(renderbuffer);
        }

        public bool IsRenderbuffer(uint renderbuffer)
        {
            return GL.IsRenderbuffer(renderbuffer);
        }

        public void BindRenderbuffer(RenderbufferTarget target, int renderbuffer)
        {
            GL.BindRenderbuffer(target, renderbuffer);
        }

        public void BindRenderbuffer(RenderbufferTarget target, uint renderbuffer)
        {
            GL.BindRenderbuffer(target, renderbuffer);
        }

        public unsafe void DeleteRenderbuffers(int n, int* renderbuffers)
        {
            GL.DeleteRenderbuffers(n, renderbuffers);
        }

        public void DeleteRenderbuffers(int n, int[] renderbuffers)
        {
            GL.DeleteRenderbuffers(n, renderbuffers);
        }

        public void DeleteRenderbuffers(int n, ref int renderbuffers)
        {
            GL.DeleteRenderbuffers(n, ref renderbuffers);
        }

        public void DeleteRenderbuffers(int n, ref uint renderbuffers)
        {
            GL.DeleteRenderbuffers(n, ref renderbuffers);
        }

        public unsafe void DeleteRenderbuffers(int n, uint* renderbuffers)
        {
            GL.DeleteRenderbuffers(n, renderbuffers);
        }

        public void DeleteRenderbuffers(int n, uint[] renderbuffers)
        {
            GL.DeleteRenderbuffers(n, renderbuffers);
        }

        public unsafe void GenRenderbuffers(int n, int* renderbuffers)
        {
            GL.GenRenderbuffers(n, renderbuffers);
        }

        public void GenRenderbuffers(int n, int[] renderbuffers)
        {
            GL.GenRenderbuffers(n, renderbuffers);
        }

        public void GenRenderbuffers(int n, out int renderbuffers)
        {
#if GLES
            renderbuffers = 0;
            GL.GenRenderbuffers(n, ref renderbuffers);
#else
            GL.GenRenderbuffers(n, out renderbuffers);
#endif
        }

        public void GenRenderbuffers(int n, out uint renderbuffers)
        {
#if GLES
            renderbuffers = 0;
            GL.GenRenderbuffers(n, ref renderbuffers);
#else
            GL.GenRenderbuffers(n, out renderbuffers);
#endif
        }

        public unsafe void GenRenderbuffers(int n, uint* renderbuffers)
        {
            GL.GenRenderbuffers(n, renderbuffers);
        }

        public void GenRenderbuffers(int n, uint[] renderbuffers)
        {
            GL.GenRenderbuffers(n, renderbuffers);
        }

        public void RenderbufferStorage(RenderbufferTarget target, RenderbufferStorage internalformat, int width, int height)
        {
            GL.RenderbufferStorage(target, internalformat, width, height);
        }

#if !MONOMAC
        public unsafe void GetRenderbufferParameter(RenderbufferTarget target, RenderbufferParameterName pname, int* @params)
        {
            GL.GetRenderbufferParameter(target, pname, @params);
        }

        public void GetRenderbufferParameter(RenderbufferTarget target, RenderbufferParameterName pname, int[] @params)
        {
            GL.GetRenderbufferParameter(target, pname, @params);
        }

        public void GetRenderbufferParameter(RenderbufferTarget target, RenderbufferParameterName pname, out int @params)
        {
#if GLES
            @params = 0;
            GL.GetRenderbufferParameter(target, pname, ref @params);
#else
            GL.GetRenderbufferParameter(target, pname, out @params);
#endif
        }
#endif

        public bool IsFramebuffer(int framebuffer)
        {
            return GL.IsFramebuffer(framebuffer);
        }

        public bool IsFramebuffer(uint framebuffer)
        {
            return GL.IsFramebuffer(framebuffer);
        }

        public void BindFramebuffer(FramebufferTarget target, int framebuffer)
        {
            GL.BindFramebuffer(target, framebuffer);
        }

        public void BindFramebuffer(FramebufferTarget target, uint framebuffer)
        {
            GL.BindFramebuffer(target, framebuffer);
        }

        public unsafe void DeleteFramebuffers(int n, int* framebuffers)
        {
            GL.DeleteFramebuffers(n, framebuffers);
        }

        public void DeleteFramebuffers(int n, int[] framebuffers)
        {
            GL.DeleteFramebuffers(n, framebuffers);
        }

        public void DeleteFramebuffers(int n, ref int framebuffers)
        {
            GL.DeleteFramebuffers(n, ref framebuffers);
        }

        public void DeleteFramebuffers(int n, ref uint framebuffers)
        {
            GL.DeleteFramebuffers(n, ref framebuffers);
        }

        public unsafe void DeleteFramebuffers(int n, uint* framebuffers)
        {
            GL.DeleteFramebuffers(n, framebuffers);
        }

        public void DeleteFramebuffers(int n, uint[] framebuffers)
        {
            GL.DeleteFramebuffers(n, framebuffers);
        }

        public unsafe void GenFramebuffers(int n, int* framebuffers)
        {
            GL.GenFramebuffers(n, framebuffers);
        }

        public void GenFramebuffers(int n, int[] framebuffers)
        {
            GL.GenFramebuffers(n, framebuffers);
        }

        public void GenFramebuffers(int n, out int framebuffers)
        {
#if GLES
            framebuffers = 0;
            GL.GenFramebuffers(n, ref framebuffers);
#else
            GL.GenFramebuffers(n, out framebuffers);
#endif
        }

        public void GenFramebuffers(int n, out uint framebuffers)
        {
#if GLES
            framebuffers = 0;
            GL.GenFramebuffers(n, ref framebuffers);
#else
            GL.GenFramebuffers(n, out framebuffers);
#endif
        }

        public unsafe void GenFramebuffers(int n, uint* framebuffers)
        {
            GL.GenFramebuffers(n, framebuffers);
        }

        public void GenFramebuffers(int n, uint[] framebuffers)
        {
            GL.GenFramebuffers(n, framebuffers);
        }

        public FramebufferErrorCode CheckFramebufferStatus(FramebufferTarget target)
        {
            return GL.CheckFramebufferStatus(target);
        }

#if !GLES
        public void FramebufferTexture1D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, int texture, int level)
        {
            GL.FramebufferTexture1D(target, attachment, textarget, texture, level);
        }

        public void FramebufferTexture1D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level)
        {
            GL.FramebufferTexture1D(target, attachment, textarget, texture, level);
        }
#endif

        public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, int texture, int level)
        {
            GL.FramebufferTexture2D(target, attachment, textarget, texture, level);
        }

        public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level)
        {
            GL.FramebufferTexture2D(target, attachment, textarget, texture, level);
        }

#if !GLES
        public void FramebufferTexture3D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, int texture, int level, int zoffset)
        {
            GL.FramebufferTexture3D(target, attachment, textarget, texture, level, zoffset);
        }

        public void FramebufferTexture3D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level, int zoffset)
        {
            GL.FramebufferTexture3D(target, attachment, textarget, texture, level, zoffset);
        }
#endif

        public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, int renderbuffer)
        {
            GL.FramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
        }

        public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, uint renderbuffer)
        {
            GL.FramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
        }

#if !MONOMAC
        public unsafe void GetFramebufferAttachmentParameter(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName pname, int* @params)
        {
            GL.GetFramebufferAttachmentParameter(target, attachment, pname, @params);
        }

        public void GetFramebufferAttachmentParameter(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName pname, int[] @params)
        {
            GL.GetFramebufferAttachmentParameter(target, attachment, pname, @params);
        }

        public void GetFramebufferAttachmentParameter(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName pname, out int @params)
        {
#if GLES
            @params = 0;
            GL.GetFramebufferAttachmentParameter(target, attachment, pname, ref @params);
#else
            GL.GetFramebufferAttachmentParameter(target, attachment, pname, out @params);
#endif
        }

        public void GenerateMipmap(GenerateMipmapTarget target)
        {
            GL.GenerateMipmap(target);
        }
#endif
    }
}

#endif
