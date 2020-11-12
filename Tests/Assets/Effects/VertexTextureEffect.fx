// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

matrix WorldViewProj;

float HeightMapSize;
Texture2D HeightMapTexture;

sampler HeightMapSampler = sampler_state
{
    Texture = (HeightMapTexture);
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = NONE;
};

struct VSOutput
{
    float4 PositionPS : SV_Position;
    float4 Color : COLOR0;
};

VSOutput VS_Main(float2 xy : POSITION)
{
    float height = HeightMapTexture.SampleLevel(HeightMapSampler, (xy + 0.5) / HeightMapSize, 0).r;
    float3 worldPosition = float3(xy.x, height, xy.y);

    VSOutput output;
    output.PositionPS = mul(float4(worldPosition, 1), WorldViewProj);
    output.Color = float4(xy.x / HeightMapSize, xy.y / HeightMapSize, 0, 1);

    return output;
}

float4 PS_Main(VSOutput input) : SV_TARGET
{
    return input.Color;
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
        VertexShader = compile VS_PROFILE VS_Main();
        PixelShader = compile PS_PROFILE PS_Main();
    }
}