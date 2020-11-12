// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#if OPENGL
using MonoGame.OpenGL;
using GetParamName = MonoGame.OpenGL.GetPName;
#endif

namespace Microsoft.Xna.Framework.Graphics
{

    internal partial class GraphicsCapabilities
    {
        /// <summary>
        /// True, if GL_ARB_framebuffer_object is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectARB { get; private set; }

        /// <summary>
        /// True, if GL_EXT_framebuffer_object is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectEXT { get; private set; }

        /// <summary>
        /// True, if GL_IMG_multisampled_render_to_texture is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectIMG { get; private set; }


        private void PlatformInitialize(GraphicsDevice device)
        {
#if GLES
            SupportsNonPowerOfTwo = GL.Extensions.Contains("GL_OES_texture_npot") ||
                GL.Extensions.Contains("GL_ARB_texture_non_power_of_two") ||
                GL.Extensions.Contains("GL_IMG_texture_npot") ||
                GL.Extensions.Contains("GL_NV_texture_npot_2D_mipmap");
#else
            // Unfortunately non PoT texture support is patchy even on desktop systems and we can't
            // rely on the fact that GL2.0+ supposedly supports npot in the core.
            // Reference: http://aras-p.info/blog/2012/10/17/non-power-of-two-textures/
            SupportsNonPowerOfTwo = device._maxTextureSize >= 8192;
#endif

            SupportsTextureFilterAnisotropic = GL.Extensions.Contains("GL_EXT_texture_filter_anisotropic");

#if GLES
			SupportsDepth24 = GL.Extensions.Contains("GL_OES_depth24");
			SupportsPackedDepthStencil = GL.Extensions.Contains("GL_OES_packed_depth_stencil");
			SupportsDepthNonLinear = GL.Extensions.Contains("GL_NV_depth_nonlinear");
            SupportsTextureMaxLevel = GL.Extensions.Contains("GL_APPLE_texture_max_level");
#else
            SupportsDepth24 = true;
            SupportsPackedDepthStencil = true;
            SupportsDepthNonLinear = false;
            SupportsTextureMaxLevel = true;
#endif
            // Texture compression
            SupportsS3tc = GL.Extensions.Contains("GL_EXT_texture_compression_s3tc") ||
                           GL.Extensions.Contains("GL_OES_texture_compression_S3TC") ||
                           GL.Extensions.Contains("GL_EXT_texture_compression_dxt3") ||
                           GL.Extensions.Contains("GL_EXT_texture_compression_dxt5");
            SupportsDxt1 = SupportsS3tc || GL.Extensions.Contains("GL_EXT_texture_compression_dxt1");
            SupportsPvrtc = GL.Extensions.Contains("GL_IMG_texture_compression_pvrtc");
            SupportsEtc1 = GL.Extensions.Contains("GL_OES_compressed_ETC1_RGB8_texture");
            SupportsAtitc = GL.Extensions.Contains("GL_ATI_texture_compression_atitc") ||
                            GL.Extensions.Contains("GL_AMD_compressed_ATC_texture");

            if (GL.BoundApi == GL.RenderApi.ES)
            {
                SupportsEtc2 = device.glMajorVersion >= 3;
            }


            // Framebuffer objects
#if GLES
            SupportsFramebufferObjectARB = GL.BoundApi == GL.RenderApi.ES && (device.glMajorVersion >= 2 || GL.Extensions.Contains("GL_ARB_framebuffer_object")); // always supported on GLES 2.0+
            SupportsFramebufferObjectEXT = GL.Extensions.Contains("GL_EXT_framebuffer_object");;
            SupportsFramebufferObjectIMG = GL.Extensions.Contains("GL_IMG_multisampled_render_to_texture") |
                                                 GL.Extensions.Contains("GL_APPLE_framebuffer_multisample") |
                                                 GL.Extensions.Contains("GL_EXT_multisampled_render_to_texture") |
                                                 GL.Extensions.Contains("GL_NV_framebuffer_multisample");
#else
            // if we're on GL 3.0+, frame buffer extensions are guaranteed to be present, but extensions may be missing
            // it is then safe to assume that GL_ARB_framebuffer_object is present so that the standard function are loaded
            SupportsFramebufferObjectARB = device.glMajorVersion >= 3 || GL.Extensions.Contains("GL_ARB_framebuffer_object");
            SupportsFramebufferObjectEXT = GL.Extensions.Contains("GL_EXT_framebuffer_object");
#endif
            // Anisotropic filtering
            int anisotropy = 0;
            if (SupportsTextureFilterAnisotropic)
            {
                GL.GetInteger((GetPName)GetParamName.MaxTextureMaxAnisotropyExt, out anisotropy);
                GraphicsExtensions.CheckGLError();
            }
            MaxTextureAnisotropy = anisotropy;

            // sRGB
#if GLES
            SupportsSRgb = GL.Extensions.Contains("GL_EXT_sRGB");
            SupportsFloatTextures = GL.BoundApi == GL.RenderApi.ES && (device.glMajorVersion >= 3 || GL.Extensions.Contains("GL_EXT_color_buffer_float"));
            SupportsHalfFloatTextures = GL.BoundApi == GL.RenderApi.ES && (device.glMajorVersion >= 3 || GL.Extensions.Contains("GL_EXT_color_buffer_half_float"));
            SupportsNormalized = GL.BoundApi == GL.RenderApi.ES && (device.glMajorVersion >= 3 && GL.Extensions.Contains("GL_EXT_texture_norm16"));
#else
            SupportsSRgb = GL.Extensions.Contains("GL_EXT_texture_sRGB") && GL.Extensions.Contains("GL_EXT_framebuffer_sRGB");
            SupportsFloatTextures = GL.BoundApi == GL.RenderApi.GL && (device.glMajorVersion >= 3 || GL.Extensions.Contains("GL_ARB_texture_float"));
            SupportsHalfFloatTextures = GL.BoundApi == GL.RenderApi.GL && (device.glMajorVersion >= 3 || GL.Extensions.Contains("GL_ARB_half_float_pixel"));
            SupportsNormalized = GL.BoundApi == GL.RenderApi.GL && (device.glMajorVersion >= 3 || GL.Extensions.Contains("GL_EXT_texture_norm16"));
#endif

            SupportsTextureArrays = device.glMajorVersion >= 3 || GL.Extensions.Contains("EXT_texture_array");

            SupportsDepthClamp = GL.Extensions.Contains("GL_ARB_depth_clamp");

            SupportsVertexTextures = device.MaxVertexTextureSlots > 0; 

            GL.GetInteger((GetPName)GetParamName.MaxSamples, out _maxMultiSampleCount);

            SupportsInstancing = GL.VertexAttribDivisor != null;

            SupportsBaseIndexInstancing = GL.DrawElementsInstancedBaseInstance != null;

#if GLES
            SupportsSeparateBlendStates = false;
#else
            SupportsSeparateBlendStates = device.glMajorVersion >= 4 || GL.Extensions.Contains("GL_ARB_draw_buffers_blend");
#endif
        }

    }
}
