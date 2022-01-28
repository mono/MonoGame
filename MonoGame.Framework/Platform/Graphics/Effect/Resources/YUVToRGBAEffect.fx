/* YUV-to-RGBA Effect
 * Written by Ethan "flibitijibibo" Lee
 * http://www.flibitijibibo.com/
 *
 * This effect is based on the YUV-to-RGBA GLSL shader found in SDL.
 * Thus, it also released under the zlib license:
 * http://libsdl.org/license.php
 */

texture yuvx; // texture 0 => screen render
sampler2D yuvxSampler = sampler_state
{
	Texture = (yuvx);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture yuvy; // texture 0 => screen render
sampler2D yuvySampler = sampler_state
{
	Texture = (yuvy);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture yuvz; // texture 0 => screen render
sampler2D yuvzSampler = sampler_state
{
	Texture = (yuvz);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};


void VS(inout float2 tex : TEXCOORD0,
	inout float4 pos : SV_Position)
{
	pos.w = 1.0;
}

float4 PS(float2 tex : TEXCOORD0) : SV_Target0
{
	const float3 offset = float3(-0.0625, -0.5, -0.5);

/* More info about colorspace conversion:
 * http://www.equasys.de/colorconversion.html
 * http://www.equasys.de/colorformat.html
 */
#if 1
 /* ITU-R BT.709 */
 const float3 Rcoeff = float3(1.164,  0.000,  1.793);
 const float3 Gcoeff = float3(1.164, -0.213, -0.533);
 const float3 Bcoeff = float3(1.164,  2.112,  0.000);
#else
 /* ITU-R BT.601 */
 const float3 Rcoeff = float3(1.164,  0.000,  1.596);
 const float3 Gcoeff = float3(1.164, -0.391, -0.813);
 const float3 Bcoeff = float3(1.164,  2.018,  0.000);
#endif
 //return tex2D(yuvxSampler, tex);
 float3 yuv;
 yuv.x = tex2D(yuvxSampler, tex).x;
 yuv.y = tex2D(yuvySampler, tex).x;
 yuv.z = tex2D(yuvzSampler, tex).x;
 yuv += offset;

 float4 rgba;
 rgba.x = dot(yuv, Rcoeff);
 rgba.y = dot(yuv, Gcoeff);
 rgba.z = dot(yuv, Bcoeff);
 rgba.w = 1.0;
 return rgba;
}

Technique T
{
	Pass P
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}
