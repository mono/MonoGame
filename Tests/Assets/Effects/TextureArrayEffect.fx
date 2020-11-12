// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

matrix WorldViewProj;

Texture2DArray Texture : register(t0);
SamplerState TextureSampler : register(s0);

struct VSOutput
{
    float4 PositionPS : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

struct PSInput
{
    float4 PositionSS : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

VSOutput VS_Main(uint VertexID : SV_VertexID)
{
    VSOutput output;
    output.TexCoord = float2((VertexID << 1) & 2, VertexID & 2);
    output.PositionPS = float4(output.TexCoord * float2(2.0, -2.0) + float2(-1.0, 1.0), 0.0f, 1.0f);
    return output;
}

float4 PS_Main(PSInput input) : SV_TARGET
{
    return Texture.Sample(TextureSampler, float3(input.TexCoord, (uint) input.PositionSS.x % 4));
}

technique
{
    pass
    {
        VertexShader = compile vs_4_0 VS_Main();
        PixelShader = compile ps_4_0 PS_Main();
    }
}