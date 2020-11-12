//-----------------------------------------------------------------------------
// Structurs.fxh
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Vertex shader input structures.

struct VSInput
{
    float4 Position : POSITION;
};

struct VSInputVc
{
    float4 Position : POSITION;
    float4 Color    : COLOR;
};

struct VSInputTx
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD;
};

struct VSInputTxVc
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD;
    float4 Color    : COLOR;
};

struct VSInputNm
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
};

struct VSInputNmVc
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
    float4 Color    : COLOR;
};

struct VSInputNmTx
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD;
};

struct VSInputNmTxVc
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD;
    float4 Color    : COLOR;
};

struct VSInputTx2
{
    float4 Position  : POSITION0;
    float2 TexCoord  : TEXCOORD0;
    float2 TexCoord2 : TEXCOORD1;
};

struct VSInputTx2Vc
{
    float4 Position  : POSITION0;
    float2 TexCoord  : TEXCOORD0;
    float2 TexCoord2 : TEXCOORD1;
    float4 Color     : COLOR;
};

struct VSInputNmTxWeights
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    int4  Indices  : BLENDINDICES0;
    float4 Weights  : BLENDWEIGHT0;
};



// Vertex shader output structures.

struct VSOutput
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float4 Specular   : COLOR1;
};

struct VSOutputNoFog
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
};

struct VSOutputTx
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float4 Specular   : COLOR1;
    float2 TexCoord   : TEXCOORD0;
};

struct VSOutputTxNoFog
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float2 TexCoord   : TEXCOORD0;
};

struct VSOutputPixelLighting
{
    float4 PositionPS : SV_Position;
    float4 PositionWS : TEXCOORD0;
    float3 NormalWS   : TEXCOORD1;
    float4 Diffuse    : COLOR0;
};

struct VSOutputPixelLightingTx
{
    float4 PositionPS : SV_Position;
    float2 TexCoord   : TEXCOORD0;
    float4 PositionWS : TEXCOORD1;
    float3 NormalWS   : TEXCOORD2;
    float4 Diffuse    : COLOR0;
};

struct VSOutputTx2
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float4 Specular   : COLOR1;
    float2 TexCoord   : TEXCOORD0;
    float2 TexCoord2  : TEXCOORD1;
};

struct VSOutputTx2NoFog
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float2 TexCoord   : TEXCOORD0;
    float2 TexCoord2  : TEXCOORD1;
};

struct VSOutputTxEnvMap
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float4 Specular   : COLOR1;
    float2 TexCoord   : TEXCOORD0;
    float3 EnvCoord   : TEXCOORD1;
};

