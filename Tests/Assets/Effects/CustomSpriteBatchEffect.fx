// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#include "include.fxh"

Texture2D SourceTexture : register(t0);
Texture2D OtherTexture : register(t1);

sampler SourceSampler : register(s0);
sampler OtherSampler : register(s1);

float4 PS_Main(float2 uv : TEXCOORD0) : SV_TARGET
{
    return SourceTexture.Sample(SourceSampler, uv) + OtherTexture.Sample(OtherSampler, uv);
}

technique
{
    pass
    {
        PixelShader = compile PS_PROFILE PS_Main();
    }
}