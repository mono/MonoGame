using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Effect
{
	internal partial class EffectObject
	{
        private EffectObject()
        {
        }

		public enum D3DRENDERSTATETYPE
        {
		    ZENABLE                   =   7,
		    FILLMODE                  =   8,
		    SHADEMODE                 =   9,
		    ZWRITEENABLE              =  14,
		    ALPHATESTENABLE           =  15,
		    LASTPIXEL                 =  16,
		    SRCBLEND                  =  19,
		    DESTBLEND                 =  20,
		    CULLMODE                  =  22,
		    ZFUNC                     =  23,
		    ALPHAREF                  =  24,
		    ALPHAFUNC                 =  25,
		    DITHERENABLE              =  26,
		    ALPHABLENDENABLE          =  27,
		    FOGENABLE                 =  28,
		    SPECULARENABLE            =  29,
		    FOGCOLOR                  =  34,
		    FOGTABLEMODE              =  35,
		    FOGSTART                  =  36,
		    FOGEND                    =  37,
		    FOGDENSITY                =  38,
		    RANGEFOGENABLE            =  48,
		    STENCILENABLE             =  52,
		    STENCILFAIL               =  53,
		    STENCILZFAIL              =  54,
		    STENCILPASS               =  55,
		    STENCILFUNC               =  56,
		    STENCILREF                =  57,
		    STENCILMASK               =  58,
		    STENCILWRITEMASK          =  59,
		    TEXTUREFACTOR             =  60,
		    WRAP0                     = 128,
		    WRAP1                     = 129,
		    WRAP2                     = 130,
		    WRAP3                     = 131,
		    WRAP4                     = 132,
		    WRAP5                     = 133,
		    WRAP6                     = 134,
		    WRAP7                     = 135,
		    CLIPPING                  = 136,
		    LIGHTING                  = 137,
		    AMBIENT                   = 139,
		    FOGVERTEXMODE             = 140,
		    COLORVERTEX               = 141,
		    LOCALVIEWER               = 142,
		    NORMALIZENORMALS          = 143,
		    DIFFUSEMATERIALSOURCE     = 145,
		    SPECULARMATERIALSOURCE    = 146,
		    AMBIENTMATERIALSOURCE     = 147,
		    EMISSIVEMATERIALSOURCE    = 148,
		    VERTEXBLEND               = 151,
		    CLIPPLANEENABLE           = 152,
		    POINTSIZE                 = 154,
		    POINTSIZE_MIN             = 155,
		    POINTSPRITEENABLE         = 156,
		    POINTSCALEENABLE          = 157,
		    POINTSCALE_A              = 158,
		    POINTSCALE_B              = 159,
		    POINTSCALE_C              = 160,
		    MULTISAMPLEANTIALIAS      = 161,
		    MULTISAMPLEMASK           = 162,
		    PATCHEDGESTYLE            = 163,
		    DEBUGMONITORTOKEN         = 165,
		    POINTSIZE_MAX             = 166,
		    INDEXEDVERTEXBLENDENABLE  = 167,
		    COLORWRITEENABLE          = 168,
		    TWEENFACTOR               = 170,
		    BLENDOP                   = 171,
		    POSITIONDEGREE            = 172,
		    NORMALDEGREE              = 173,
		    SCISSORTESTENABLE         = 174,
		    SLOPESCALEDEPTHBIAS       = 175,
		    ANTIALIASEDLINEENABLE     = 176,
		    MINTESSELLATIONLEVEL      = 178,
		    MAXTESSELLATIONLEVEL      = 179,
		    ADAPTIVETESS_X            = 180,
		    ADAPTIVETESS_Y            = 181,
		    ADAPTIVETESS_Z            = 182,
		    ADAPTIVETESS_W            = 183,
		    ENABLEADAPTIVETESSELLATION= 184,
		    TWOSIDEDSTENCILMODE       = 185,
		    CCW_STENCILFAIL           = 186,
		    CCW_STENCILZFAIL          = 187,
		    CCW_STENCILPASS           = 188,
		    CCW_STENCILFUNC           = 189,
		    COLORWRITEENABLE1         = 190,
		    COLORWRITEENABLE2         = 191,
		    COLORWRITEENABLE3         = 192,
		    BLENDFACTOR               = 193,
		    SRGBWRITEENABLE           = 194,
		    DEPTHBIAS                 = 195,
		    WRAP8                     = 198,
		    WRAP9                     = 199,
		    WRAP10                    = 200,
		    WRAP11                    = 201,
		    WRAP12                    = 202,
		    WRAP13                    = 203,
		    WRAP14                    = 204,
		    WRAP15                    = 205,
		    SEPARATEALPHABLENDENABLE  = 206,
		    SRCBLENDALPHA             = 207,
		    DESTBLENDALPHA            = 208,
		    BLENDOPALPHA              = 209,
		
		    FORCE_DWORD               = 0x7fffffff
		}

		public enum D3DTEXTURESTAGESTATETYPE
        {
		    COLOROP               =  1,
		    COLORARG1             =  2,
		    COLORARG2             =  3,
		    ALPHAOP               =  4,
		    ALPHAARG1             =  5,
		    ALPHAARG2             =  6,
		    BUMPENVMAT00          =  7,
		    BUMPENVMAT01          =  8,
		    BUMPENVMAT10          =  9,
		    BUMPENVMAT11          = 10,
		    TEXCOORDINDEX         = 11,
		    BUMPENVLSCALE         = 22,
		    BUMPENVLOFFSET        = 23,
		    TEXTURETRANSFORMFLAGS = 24,
		    COLORARG0             = 26,
		    ALPHAARG0             = 27,
		    RESULTARG             = 28,
		    CONSTANT              = 32,
		
		    FORCE_DWORD           = 0x7fffffff
		}

		public enum D3DTRANSFORMSTATETYPE
        {
		    VIEW            =  2,
		    PROJECTION      =  3,
		    TEXTURE0        = 16,
		    TEXTURE1        = 17,
		    TEXTURE2        = 18,
		    TEXTURE3        = 19,
		    TEXTURE4        = 20,
		    TEXTURE5        = 21,
		    TEXTURE6        = 22,
		    TEXTURE7        = 23,
			WORLD           = 256,
		    FORCE_DWORD     = 0x7fffffff
		}

		public const int D3DX_PARAMETER_SHARED = 1;
		public const int D3DX_PARAMETER_LITERAL = 2;
		public const int D3DX_PARAMETER_ANNOTATION = 4;

		public enum D3DXPARAMETER_CLASS
		{
			SCALAR,
			VECTOR,
			MATRIX_ROWS,
			MATRIX_COLUMNS,
			OBJECT,
			STRUCT,
			FORCE_DWORD = 0x7fffffff,
		}

		public enum D3DXPARAMETER_TYPE
		{
			VOID,
			BOOL,
			INT,
			FLOAT,
			STRING,
			TEXTURE,
			TEXTURE1D,
			TEXTURE2D,
			TEXTURE3D,
			TEXTURECUBE,
			SAMPLER,
			SAMPLER1D,
			SAMPLER2D,
			SAMPLER3D,
			SAMPLERCUBE,
			PIXELSHADER,
			VERTEXSHADER,
			PIXELFRAGMENT,
			VERTEXFRAGMENT,
            GEOMETRYSHADER,
            HULLSHADER,
            DOMAINSHADER,
            UNSUPPORTED,
			FORCE_DWORD = 0x7fffffff,
		}

		enum D3DSAMPLERSTATETYPE 
        {
		    ADDRESSU       = 1,
		    ADDRESSV       = 2,
		    ADDRESSW       = 3,
		    BORDERCOLOR    = 4,
		    MAGFILTER      = 5,
		    MINFILTER      = 6,
		    MIPFILTER      = 7,
		    MIPMAPLODBIAS  = 8,
		    MAXMIPLEVEL    = 9,
		    MAXANISOTROPY  = 10,
		    SRGBTEXTURE    = 11,
		    ELEMENTINDEX   = 12,
		    DMAPOFFSET     = 13,
		                                
		    FORCE_DWORD   = 0x7fffffff,
		};

		public enum STATE_CLASS
		{
		    LIGHTENABLE,
		    FVF,
		    LIGHT,
		    MATERIAL,
		    NPATCHMODE,
		    PIXELSHADER,
		    RENDERSTATE,
		    SETSAMPLER,
		    SAMPLERSTATE,
		    TEXTURE,
		    TEXTURESTAGE,
		    TRANSFORM,
		    VERTEXSHADER,
		    SHADERCONST,
            GEOMETRYSHADER,
            HULLSHADER,
            DOMAINSHADER,
            UNKNOWN,
		};

		public enum MATERIAL_TYPE
		{
		    DIFFUSE,
		    AMBIENT,
		    SPECULAR,
		    EMISSIVE,
		    POWER,
		};

		public enum LIGHT_TYPE
		{
		    TYPE,
		    DIFFUSE,
		    SPECULAR,
		    AMBIENT,
		    POSITION,
		    DIRECTION,
		    RANGE,
		    FALLOFF,
		    ATTENUATION0,
		    ATTENUATION1,
		    ATTENUATION2,
		    THETA,
		    PHI,
		};

		public enum SHADER_CONSTANT_TYPE
		{
		    VSFLOAT,
		    VSBOOL,
		    VSINT,
		    PSFLOAT,
		    PSBOOL,
		    PSINT,
            GSFLOAT,
            GSBOOL,
            GSINT,
            HSFLOAT,
            HSBOOL,
            HSINT,
            DSFLOAT,
            DSBOOL,
            DSINT,
        }

		public enum STATE_TYPE
		{
			CONSTANT,
			PARAMETER,
			EXPRESSION,
			EXPRESSIONINDEX,
		}

		public class d3dx_parameter
		{
			public string name;
			public string semantic;
			public object data;
			public D3DXPARAMETER_CLASS class_;
			public D3DXPARAMETER_TYPE  type;
			public uint rows;
			public uint columns;
			public uint element_count;
			public uint annotation_count = 0;
			public uint member_count;
			public uint flags = 0;
			public uint bytes = 0;

            public int bufferIndex = -1;
            public int bufferOffset = -1;

		    public d3dx_parameter[] annotation_handles = null;
			public d3dx_parameter[] member_handles;

            public override string ToString()
            {
                if (rows > 0 || columns > 0)
                    return string.Format("{0} {1}{2}x{3} {4} : cb{5},{6}", class_, type, rows, columns, name, bufferIndex, bufferOffset);
                else
                    return string.Format("{0} {1} {2}", class_, type, name);
            }
		}
		
		public class d3dx_state
		{
			public uint operation;
			public uint index;
			public STATE_TYPE type;
			public d3dx_parameter parameter;
		}

		public class d3dx_sampler
		{
		    public uint state_count = 0;
		    public d3dx_state[] states = null;
		}
		
		public class d3dx_pass
		{
			public string name;
			public uint state_count;
		    public uint annotation_count = 0;

			public BlendState blendState;
			public DepthStencilState depthStencilState;
			public RasterizerState rasterizerState;

			public d3dx_state[] states;
		    public d3dx_parameter[] annotation_handles = null;
		}

		public class d3dx_technique
		{
			public string name;
			public uint pass_count;
		    public uint annotation_count = 0;

		    public d3dx_parameter[] annotation_handles = null;
			public d3dx_pass[] pass_handles;
		}

        public class state_info
		{
            public STATE_CLASS class_ { get; private set; }
            public uint op { get; private set; }
            public string name { get; private set; }

			public state_info(STATE_CLASS class_, uint op, string name) 
            {
				this.class_ = class_;
				this.op = op;
				this.name = name;
			}
		}

        /// <summary>
        /// The shared state definition table.
        /// </summary>
		public static readonly state_info[] state_table =
		{
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ZENABLE, "ZENABLE"), /* 0x0 */
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FILLMODE, "FILLMODE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SHADEMODE, "SHADEMODE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ZWRITEENABLE, "ZWRITEENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHATESTENABLE, "ALPHATESTENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.LASTPIXEL, "LASTPIXEL"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SRCBLEND, "SRCBLEND"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DESTBLEND, "DESTBLEND"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CULLMODE, "CULLMODE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ZFUNC, "ZFUNC"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHAREF, "ALPHAREF"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHAFUNC, "ALPHAFUNC"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DITHERENABLE, "DITHERENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHABLENDENABLE, "ALPHABLENDENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGENABLE, "FOGENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SPECULARENABLE, "SPECULARENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGCOLOR, "FOGCOLOR"), /* 0x10 */
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGTABLEMODE, "FOGTABLEMODE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGSTART, "FOGSTART"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGEND, "FOGEND"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGDENSITY, "FOGDENSITY"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.RANGEFOGENABLE, "RANGEFOGENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILENABLE, "STENCILENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILFAIL, "STENCILFAIL"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILZFAIL, "STENCILZFAIL"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILPASS, "STENCILPASS"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILFUNC, "STENCILFUNC"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILREF, "STENCILREF"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILMASK, "STENCILMASK"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILWRITEMASK, "STENCILWRITEMASK"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.TEXTUREFACTOR, "TEXTUREFACTOR"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP0, "WRAP0"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP1, "WRAP1"), /* 0x20 */
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP2, "WRAP2"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP3, "WRAP3"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP4, "WRAP4"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP5, "WRAP5"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP6, "WRAP6"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP7, "WRAP7"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP8, "WRAP8"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP9, "WRAP9"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP10, "WRAP10"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP11, "WRAP11"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP12, "WRAP12"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP13, "WRAP13"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP14, "WRAP14"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP15, "WRAP15"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CLIPPING, "CLIPPING"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.LIGHTING, "LIGHTING"), /* 0x30 */
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.AMBIENT, "AMBIENT"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGVERTEXMODE, "FOGVERTEXMODE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORVERTEX, "COLORVERTEX"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.LOCALVIEWER, "LOCALVIEWER"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.NORMALIZENORMALS, "NORMALIZENORMALS"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DIFFUSEMATERIALSOURCE, "DIFFUSEMATERIALSOURCE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SPECULARMATERIALSOURCE, "SPECULARMATERIALSOURCE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.AMBIENTMATERIALSOURCE, "AMBIENTMATERIALSOURCE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.EMISSIVEMATERIALSOURCE, "EMISSIVEMATERIALSOURCE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.VERTEXBLEND, "VERTEXBLEND"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CLIPPLANEENABLE, "CLIPPLANEENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSIZE, "POINTSIZE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSIZE_MIN, "POINTSIZE_MIN"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSIZE_MAX, "POINTSIZE_MAX"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSPRITEENABLE, "POINTSPRITEENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALEENABLE, "POINTSCALEENABLE"), /* 0x40 */
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALE_A, "POINTSCALE_A"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALE_B, "POINTSCALE_B"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALE_C, "POINTSCALE_C"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MULTISAMPLEANTIALIAS, "MULTISAMPLEANTIALIAS"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MULTISAMPLEMASK, "MULTISAMPLEMASK"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.PATCHEDGESTYLE, "PATCHEDGESTYLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DEBUGMONITORTOKEN, "DEBUGMONITORTOKEN"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.INDEXEDVERTEXBLENDENABLE, "INDEXEDVERTEXBLENDENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE, "COLORWRITEENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.TWEENFACTOR, "TWEENFACTOR"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.BLENDOP, "BLENDOP"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POSITIONDEGREE, "POSITIONDEGREE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.NORMALDEGREE, "NORMALDEGREE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SCISSORTESTENABLE, "SCISSORTESTENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SLOPESCALEDEPTHBIAS, "SLOPESCALEDEPTHBIAS"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ANTIALIASEDLINEENABLE, "ANTIALIASEDLINEENABLE"), /* 0x50 */
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MINTESSELLATIONLEVEL, "MINTESSELLATIONLEVEL"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MAXTESSELLATIONLEVEL, "MAXTESSELLATIONLEVEL"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_X, "ADAPTIVETESS_X"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_Y, "ADAPTIVETESS_Y"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_Z, "ADAPTIVETESS_Z"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_W, "ADAPTIVETESS_W"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ENABLEADAPTIVETESSELLATION, "ENABLEADAPTIVETESSELLATION"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.TWOSIDEDSTENCILMODE, "TWOSIDEDSTENCILMODE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILFAIL, "CCW_STENCILFAIL"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILZFAIL, "CCW_STENCILZFAIL"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILPASS, "CCW_STENCILPASS"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILFUNC, "CCW_STENCILFUNC"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE1, "COLORWRITEENABLE1"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE2, "COLORWRITEENABLE2"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE3, "COLORWRITEENABLE3"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.BLENDFACTOR, "BLENDFACTOR"), /* 0x60 */
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SRGBWRITEENABLE, "SRGBWRITEENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DEPTHBIAS, "DEPTHBIAS"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SEPARATEALPHABLENDENABLE, "SEPARATEALPHABLENDENABLE"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SRCBLENDALPHA, "SRCBLENDALPHA"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DESTBLENDALPHA, "DESTBLENDALPHA"),
			new state_info(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.BLENDOPALPHA, "BLENDOPALPHA"),
			/* Texture stages */
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLOROP, "COLOROP"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLORARG0, "COLORARG0"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLORARG1, "COLORARG1"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLORARG2, "COLORARG2"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAOP, "ALPHAOP"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAARG0, "ALPHAARG0"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAARG1, "ALPHAARG1"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAARG2, "ALPHAARG2"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.RESULTARG, "RESULTARG"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT00, "BUMPENVMAT00"), /* 0x70 */
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT01, "BUMPENVMAT01"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT10, "BUMPENVMAT10"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT11, "BUMPENVMAT11"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.TEXCOORDINDEX, "TEXCOORDINDEX"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVLSCALE, "BUMPENVLSCALE"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVLOFFSET, "BUMPENVLOFFSET"),
			new state_info(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.TEXTURETRANSFORMFLAGS, "TEXTURETRANSFORMFLAGS"),
			/* */
			new state_info(STATE_CLASS.UNKNOWN, 0, "UNKNOWN"),
			/* NPatchMode */
			new state_info(STATE_CLASS.NPATCHMODE, 0, "NPatchMode"),
			/* */
			new state_info(STATE_CLASS.UNKNOWN, 0, "UNKNOWN"),
			/* Transform */
			new state_info(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.PROJECTION, "PROJECTION"),
			new state_info(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.VIEW, "VIEW"),
			new state_info(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.WORLD, "WORLD"),
			new state_info(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.TEXTURE0, "TEXTURE0"),
			/* Material */
			new state_info(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.DIFFUSE, "MaterialDiffuse"),
			new state_info(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.AMBIENT, "MaterialAmbient"), /* 0x80 */
			new state_info(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.SPECULAR, "MaterialSpecular"),
			new state_info(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.EMISSIVE, "MaterialEmissive"),
			new state_info(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.POWER, "MaterialPower"),
			/* Light */
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.TYPE, "LightType"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.DIFFUSE, "LightDiffuse"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.SPECULAR, "LightSpecular"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.AMBIENT, "LightAmbient"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.POSITION, "LightPosition"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.DIRECTION, "LightDirection"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.RANGE, "LightRange"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.FALLOFF, "LightFallOff"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.ATTENUATION0, "LightAttenuation0"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.ATTENUATION1, "LightAttenuation1"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.ATTENUATION2, "LightAttenuation2"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.THETA, "LightTheta"),
			new state_info(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.PHI, "LightPhi"), /* 0x90 */
			/* Ligthenable */
			new state_info(STATE_CLASS.LIGHTENABLE, 0, "LightEnable"),
			/* Vertexshader */
			new state_info(STATE_CLASS.VERTEXSHADER, 0, "Vertexshader"),
			/* Pixelshader */
			new state_info(STATE_CLASS.PIXELSHADER, 0, "Pixelshader"),
            /* GeometryShader */
            new state_info(STATE_CLASS.GEOMETRYSHADER, 0, "GeometryShader"),
            /* DomainShader */
            new state_info(STATE_CLASS.DOMAINSHADER, 0, "DomainShader"),
            /* HullShader */
            new state_info(STATE_CLASS.HULLSHADER, 0, "HullShader"),
			/* Shader constants */
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstantF"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSBOOL,  "VertexShaderConstantB"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSINT,   "VertexShaderConstantI"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant1"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant2"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant3"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant4"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstantF"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSBOOL,  "PixelShaderConstantB"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSINT,   "PixelShaderConstantI"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant1"), /* 0xa0 */
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant2"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant3"),
			new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant4"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSFLOAT, "HullShaderConstantF"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSBOOL,  "HullShaderConstantB"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSINT,   "HullShaderConstantI"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSFLOAT, "HullShaderConstant"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSFLOAT, "HullShaderConstant1"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSFLOAT, "HullShaderConstant2"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSFLOAT, "HullShaderConstant3"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.HSFLOAT, "HullShaderConstant4"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSFLOAT, "DomainShaderConstantF"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSBOOL,  "DomainShaderConstantB"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSINT,   "DomainShaderConstantI"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSFLOAT, "DomainShaderConstant"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSFLOAT, "DomainShaderConstant1"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSFLOAT, "DomainShaderConstant2"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSFLOAT, "DomainShaderConstant3"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.DSFLOAT, "DomainShaderConstant4"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSFLOAT, "GeometryShaderConstantF"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSBOOL,  "GeometryShaderConstantB"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSINT,   "GeometryShaderConstantI"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSFLOAT, "GeometryShaderConstant"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSFLOAT, "GeometryShaderConstant1"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSFLOAT, "GeometryShaderConstant2"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSFLOAT, "GeometryShaderConstant3"),
            new state_info(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.GSFLOAT, "GeometryShaderConstant4"),
			/* Texture */
			new state_info(STATE_CLASS.TEXTURE, 0, "Texture"),
			/* Sampler states */
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ADDRESSU, "AddressU"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ADDRESSV, "AddressV"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ADDRESSW, "AddressW"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.BORDERCOLOR, "BorderColor"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MAGFILTER, "MagFilter"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MINFILTER, "MinFilter"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MIPFILTER, "MipFilter"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MIPMAPLODBIAS, "MipMapLodBias"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MAXMIPLEVEL, "MaxMipLevel"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MAXANISOTROPY, "MaxAnisotropy"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.SRGBTEXTURE, "SRGBTexture"),
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ELEMENTINDEX, "ElementIndex"), /* 0xb0 */
			new state_info(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.DMAPOFFSET, "DMAPOffset"),
			/* Set sampler */
			new state_info(STATE_CLASS.SETSAMPLER, 0, "Sampler"),
		};

        static public EffectParameterClass ToXNAParameterClass( D3DXPARAMETER_CLASS class_ )
        {
			switch (class_) 
            {
			    case D3DXPARAMETER_CLASS.SCALAR:
				    return EffectParameterClass.Scalar;
			    case D3DXPARAMETER_CLASS.VECTOR:
				    return EffectParameterClass.Vector;
			    case D3DXPARAMETER_CLASS.MATRIX_ROWS:
			    case D3DXPARAMETER_CLASS.MATRIX_COLUMNS:
                    return EffectParameterClass.Matrix;
			    case D3DXPARAMETER_CLASS.OBJECT:
                    return EffectParameterClass.Object;
			    case D3DXPARAMETER_CLASS.STRUCT:
                    return EffectParameterClass.Struct;
			    default:
				    throw new NotImplementedException();
			}
        }

        static public EffectParameterType ToXNAParameterType(D3DXPARAMETER_TYPE type)
        {
			switch (type) 
            {
			    case D3DXPARAMETER_TYPE.BOOL:
                    return EffectParameterType.Bool;
			    case D3DXPARAMETER_TYPE.INT:
				    return EffectParameterType.Int32;
			    case D3DXPARAMETER_TYPE.FLOAT:
				    return EffectParameterType.Single;
			    case D3DXPARAMETER_TYPE.STRING:
				    return EffectParameterType.String;
			    case D3DXPARAMETER_TYPE.TEXTURE:
				    return EffectParameterType.Texture;
			    case D3DXPARAMETER_TYPE.TEXTURE1D:
				    return EffectParameterType.Texture1D;
			    case D3DXPARAMETER_TYPE.TEXTURE2D:
				    return EffectParameterType.Texture2D;
			    case D3DXPARAMETER_TYPE.TEXTURE3D:
				    return EffectParameterType.Texture3D;
			    case D3DXPARAMETER_TYPE.TEXTURECUBE:
				    return  EffectParameterType.TextureCube;
                default:
                    throw new NotImplementedException();
			}
        }

        static internal VertexElementUsage ToXNAVertexElementUsage(MojoShader.MOJOSHADER_usage usage)
        {
            switch (usage)
            {
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_POSITION:
                    return VertexElementUsage.Position;
		        case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_BLENDWEIGHT:
                    return VertexElementUsage.BlendWeight;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_BLENDINDICES:
                    return VertexElementUsage.BlendIndices;
		        case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_NORMAL:
                    return VertexElementUsage.Normal;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_POINTSIZE:
                    return VertexElementUsage.PointSize;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_TEXCOORD:
                    return VertexElementUsage.TextureCoordinate;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_TANGENT:
                    return VertexElementUsage.Tangent;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_BINORMAL:
                    return VertexElementUsage.Binormal;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_TESSFACTOR:
                    return VertexElementUsage.TessellateFactor;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_COLOR:
                    return VertexElementUsage.Color;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_FOG:
                    return VertexElementUsage.Fog;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_DEPTH:
                    return VertexElementUsage.Depth;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_SAMPLE:
                    return VertexElementUsage.Sample;

                default:
                    throw new NotImplementedException();
            }
        }


        static public EffectObject CompileEffect(ShaderResult shaderResult, out string errorsAndWarnings)
        {
            var effect = new EffectObject();
            errorsAndWarnings = string.Empty;

            // These are filled out as we process stuff.
            effect.ConstantBuffers = new List<ConstantBufferData>();
            effect.Shaders = new List<ShaderData>();

            // Go thru the techniques and that will find all the 
            // shaders and constant buffers.
            var shaderInfo = shaderResult.ShaderInfo;
            effect.Techniques = new d3dx_technique[shaderInfo.Techniques.Count];
            for (var t = 0; t < shaderInfo.Techniques.Count; t++)
            {
                var tinfo = shaderInfo.Techniques[t]; ;

                var technique = new d3dx_technique();
                technique.name = tinfo.name;
                technique.pass_count = (uint)tinfo.Passes.Count;
                technique.pass_handles = new d3dx_pass[tinfo.Passes.Count];

                for (var p = 0; p < tinfo.Passes.Count; p++)
                {
                    var pinfo = tinfo.Passes[p];

                    var pass = new d3dx_pass();
                    pass.name = pinfo.name ?? string.Empty;

                    pass.blendState = pinfo.blendState;
                    pass.depthStencilState = pinfo.depthStencilState;
                    pass.rasterizerState = pinfo.rasterizerState;

                    pass.state_count = 0;
                    var tempstate = new d3dx_state[5];

                    shaderResult.Profile.ValidateShaderModels(pinfo);

                    if (!string.IsNullOrEmpty(pinfo.psFunction))
                    {
                        pass.state_count += 1;
                        tempstate[pass.state_count - 1] = effect.CreateShader(shaderResult, pinfo.psFunction, pinfo.psModel, ShaderStage.PixelShader, ref errorsAndWarnings);
                    }

                    if (!string.IsNullOrEmpty(pinfo.vsFunction))
                    {
                        pass.state_count += 1;
                        tempstate[pass.state_count - 1] = effect.CreateShader(shaderResult, pinfo.vsFunction, pinfo.vsModel, ShaderStage.VertexShader, ref errorsAndWarnings);
                    }

                    if (!string.IsNullOrEmpty(pinfo.hsFunction))
                    {
                        pass.state_count += 1;
                        tempstate[pass.state_count - 1] = effect.CreateShader(shaderResult, pinfo.hsFunction, pinfo.hsModel, ShaderStage.HullShader, ref errorsAndWarnings);
                    }

                    if (!string.IsNullOrEmpty(pinfo.dsFunction))
                    {
                        pass.state_count += 1;
                        tempstate[pass.state_count - 1] = effect.CreateShader(shaderResult, pinfo.dsFunction, pinfo.dsModel, ShaderStage.DomainShader, ref errorsAndWarnings);
                    }

                    if (!string.IsNullOrEmpty(pinfo.gsFunction))
                    {
                        pass.state_count += 1;
                        tempstate[pass.state_count - 1] = effect.CreateShader(shaderResult, pinfo.gsFunction, pinfo.gsModel, ShaderStage.GeometryShader, ref errorsAndWarnings);
                    }

                    pass.states = new d3dx_state[pass.state_count];
                    for (var s = 0; s < pass.state_count; s++)
                        pass.states[s] = tempstate[s];

                    technique.pass_handles[p] = pass;
                }

                effect.Techniques[t] = technique;
            }

            // In OpenGL, when a single sampler samples from multiple textures, we have to create separate samplers for every texture.
            if (shaderResult.Profile is OpenGLShaderProfile profileGL)
                profileGL.MakeSeparateSamplersForDifferentTextures(effect.Shaders);

            // Make the list of parameters by combining all the
            // constant buffers ignoring the buffer offsets.
            var parameters = new List<d3dx_parameter>();
            for (var c = 0; c < effect.ConstantBuffers.Count; c++)
            {
                var cb = effect.ConstantBuffers[c];

                for (var i = 0; i < cb.Parameters.Count; i++)
                {
                    var param = cb.Parameters[i];

                    var match = parameters.FindIndex(e => e.name == param.name);
                    if (match == -1)
                    {
                        cb.ParameterIndex.Add(parameters.Count);
                        parameters.Add(param);
                    }
                    else
                    {
                        // TODO: Make sure the type and size of 
                        // the parameter match up!
                        cb.ParameterIndex.Add(match);
                    }
                }
            }

            // Add the texture parameters from the samplers.
            foreach (var shader in effect.Shaders)
            {
                for (var s = 0; s < shader._samplers.Length; s++)
                {
                    var sampler = shader._samplers[s];

                    var match = parameters.FindIndex(e => e.name == sampler.parameterName);
                    if (match == -1)
                    {
                        // Store the index for runtime lookup.
                        shader._samplers[s].parameter = parameters.Count;

                        var param = new d3dx_parameter();
                        param.class_ = D3DXPARAMETER_CLASS.OBJECT;
                        param.name = sampler.parameterName;
                        param.semantic = string.Empty;

                        switch (sampler.type)
                        {
                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_1D:
                                param.type = D3DXPARAMETER_TYPE.TEXTURE1D;
                                break;

                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_2D:
                                param.type = D3DXPARAMETER_TYPE.TEXTURE2D;
                                break;

                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_VOLUME:
                                param.type = D3DXPARAMETER_TYPE.TEXTURE3D;
                                break;

                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_CUBE:
                                param.type = D3DXPARAMETER_TYPE.TEXTURECUBE;
                                break;
                        }

                        parameters.Add(param);
                    }
                    else
                    {
                        // TODO: Make sure the type and size of 
                        // the parameter match up!

                        shader._samplers[s].parameter = match;
                    }
                }
            }

            // TODO: Annotations are part of the .FX format and
            // not a part of shaders... we need to implement them
            // in our mgfx parser if we want them back.

            effect.Parameters = parameters.ToArray();

            return effect;
        }

        private d3dx_state CreateShader(ShaderResult shaderResult, string shaderFunction, string shaderProfile, ShaderStage shaderStage, ref string errorsAndWarnings)
        {
            // Compile and create the shader.
            var shaderData = shaderResult.Profile.CreateShader(shaderResult, shaderFunction, shaderProfile, shaderStage, this, ref errorsAndWarnings);

            var state = new d3dx_state();
            state.index = 0;
            state.type = STATE_TYPE.CONSTANT;
            state.parameter = new d3dx_parameter();
            state.parameter.name = string.Empty;
            state.parameter.semantic = string.Empty;
            state.parameter.class_ = D3DXPARAMETER_CLASS.OBJECT;
            state.parameter.rows = 0;
            state.parameter.columns = 0;
            state.parameter.data = shaderData.SharedIndex;

            switch (shaderStage)
            {
                case ShaderStage.VertexShader:
                    state.operation = (uint)146;
                    state.parameter.type = D3DXPARAMETER_TYPE.VERTEXSHADER;
                    break;
                case ShaderStage.PixelShader:
                    state.operation = (uint)147;
                    state.parameter.type = D3DXPARAMETER_TYPE.PIXELSHADER;
                    break;
                case ShaderStage.GeometryShader:
                    state.operation = (uint)148;
                    state.parameter.type = D3DXPARAMETER_TYPE.GEOMETRYSHADER;
                    break;
                case ShaderStage.HullShader:
                    state.operation = (uint)150;
                    state.parameter.type = D3DXPARAMETER_TYPE.HULLSHADER;
                    break;
                case ShaderStage.DomainShader:
                    state.operation = (uint)149;
                    state.parameter.type = D3DXPARAMETER_TYPE.DOMAINSHADER;
                    break;
            }

            return state;
        }
       
        internal static int GetShaderIndex(STATE_CLASS type, d3dx_state[] states)
        {
            foreach (var state in states)
            {
                var operation = state_table[state.operation];
                if (operation.class_ != type)
                    continue;

                if (state.type != STATE_TYPE.CONSTANT)
                    throw new NotSupportedException("We do not support shader expressions!");

                return (int)state.parameter.data;
            }

            return -1;
        }

        public d3dx_parameter[] Objects { get; private set; }

        public d3dx_parameter[] Parameters { get; private set; }

        public d3dx_technique[] Techniques { get; private set; }

        public List<ShaderData> Shaders { get; private set; }

        public List<ConstantBufferData> ConstantBuffers { get; private set; }
    }
}

