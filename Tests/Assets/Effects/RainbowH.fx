// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#include "include.fxh"

sampler s0 : register(s0);
Texture2D tex : register(t0);

float4 red    = float4(1,0,0,1);
float4 orange = float4(1,.5,0,1);
float4 yellow = float4(1,1,0,1);
float4 green  = float4(0,1,0,1);
float4 blue   = float4(0,0,1,1);
float4 indigo = float4(.3,0,.8,1);
float4 violet = float4(1,.8,1,1);

float step = 1.0/7;

float4 PixelShaderFunction( float4 inPosition : SV_Position,
			    float4 inColor : COLOR0,
			    float2 coords : TEXCOORD0 ) : SV_TARGET
{
    float4 color = tex.Sample(s0, coords);

	if (!any(color)) return color;

	if      (coords.x < (step * 1)) color = red;
	else if (coords.x < (step * 2)) color = orange;
	else if (coords.x < (step * 3)) color = yellow;
	else if (coords.x < (step * 4)) color = green;
	else if (coords.x < (step * 5)) color = blue;
	else if (coords.x < (step * 6)) color = indigo;
	else                            color = violet;

    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile PS_PROFILE PixelShaderFunction();
    }
}