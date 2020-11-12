// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#include "include.fxh"

sampler s0 : register(s0);
Texture2D tex : register(t0);

float4 PixelShaderFunction( float4 inPosition : SV_Position,
			    float4 inColor : COLOR0,
			    float2 coords : TEXCOORD0 ) : SV_TARGET
{
    float4 color = tex.Sample(s0, coords);
    color -= tex.Sample(s0, coords - 0.002) * 2.5f;
    color += tex.Sample(s0, coords + 0.002) * 2.5f;

    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile PS_PROFILE PixelShaderFunction();
    }
}