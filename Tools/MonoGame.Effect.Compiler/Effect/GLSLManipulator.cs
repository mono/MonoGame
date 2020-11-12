using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Effect.TPGParser;

namespace MonoGame.Effect
{
    internal static class GLSLManipulator
    {
        static char[] lineEnder = { '\n', '\0' };

        public static void RemoveVersionHeader(ref string glsl)
        {
            int version = glsl.IndexOf("#version");
            if (version >= 0)
            {
                int lineEnd = glsl.IndexOfAny(lineEnder, version);
                glsl = glsl.Remove(version, lineEnd - version);
            }
        }

        public static void RemoveARBSeparateShaderObjects(ref string glsl)
        {
            glsl = glsl.Replace("#extension GL_ARB_separate_shader_objects : require\n", "");
        }

        public static void RemoveOutGlPerVertex(ref string glsl)
        {
            string gl_PerVertex = "\nout gl_PerVertex\n{\n    vec4 gl_Position;\n};\n";
            glsl = glsl.Replace(gl_PerVertex, "");
        }

        public static void RemoveInGlPerVertex(ref string glsl)
        {
            string gl_PerVertex = "\nin gl_PerVertex\n{\n    vec4 gl_Position;\n};\n";
            glsl = glsl.Replace(gl_PerVertex, "");
        }

        public static void AddPosFixupUniformAndCode(ref string glsl)
        {
                // Add posFixup parameter to the vertex shader, so we can compensate for differences btw DirectX and OpenGL
                string posFixup = "uniform vec4 posFixup;";

                int cursor = glsl.LastIndexOf('#');
                if (cursor < 0)
                    cursor = 0;
                else
                    cursor = glsl.IndexOfAny(lineEnder, cursor);

                glsl = glsl.Insert(cursor, "\n" + posFixup);

                // Add posFixup code to the end of the vertex shader.
                // OpenGL uses flipped y-coordinates when rendering to a render target, in this case posFixup.y will be -1.
                // posFixup.zw is for emulating the DX9 half-pixel-offset.
                // The final change to gl_Position.z is needed because OpenGL uses a -1..1 clipspace, while DX uses 0..1
                string posFixupCode =
                "    gl_Position.y = gl_Position.y * posFixup.y;\n" +
                "    gl_Position.xy += posFixup.zw * gl_Position.ww;\n" +
                "    gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;\n";

                // the assumption here is that the final closing brace belongs to the main vertex shader function, let's hope for the best.
                cursor = glsl.LastIndexOf('}');
                glsl = glsl.Insert(cursor, posFixupCode);
        }
    }
}
