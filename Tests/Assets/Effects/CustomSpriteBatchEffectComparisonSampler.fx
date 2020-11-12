// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

Texture2D SourceTexture : register(t0);

SamplerComparisonState SourceSampler : register(s0);

struct VSOutput
{
    float4 position : SV_Position;
    float4 color    : COLOR0;
    float2 uv       : TEXCOORD0;
};

float4 PS_Main(VSOutput input) : SV_TARGET
{
    float comparisonResult = SourceTexture.SampleCmpLevelZero(SourceSampler, input.uv, 0.5f);
    return float4(comparisonResult, 0, 0, 1);
}

#if SM4
#define PS_PROFILE ps_4_0
#define VS_PROFILE vs_4_0
#else
#define PS_PROFILE ps_3_0
#define VS_PROFILE vs_3_0
#endif

technique
{
    pass
    {
        PixelShader = compile PS_PROFILE PS_Main();
    }
}