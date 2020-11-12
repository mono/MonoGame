//-----------------------------------------------------------------------------
// Lighting.fxh
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


struct ColorPair
{
    float3 Diffuse;
    float3 Specular;
};


ColorPair ComputeLights(float3 eyeVector, float3 worldNormal, uniform int numLights)
{
    float3x3 lightDirections = 0;
    float3x3 lightDiffuse = 0;
    float3x3 lightSpecular = 0;
    float3x3 halfVectors = 0;
      
    if (numLights > 0)
    {
        lightDirections[0] = DirLight0Direction;
        lightDiffuse[0]    = DirLight0DiffuseColor;
        lightSpecular[0]   = DirLight0SpecularColor;
        halfVectors[0] = normalize(eyeVector - lightDirections[0]);
    }
    
    if (numLights > 1)
    {
        lightDirections[1] = DirLight1Direction;
        lightDiffuse[1]    = DirLight1DiffuseColor;
        lightSpecular[1]   = DirLight1SpecularColor;
        halfVectors[1] = normalize(eyeVector - lightDirections[1]);
    }
    
    if (numLights > 2)
    {
        lightDirections[2] = DirLight2Direction;
        lightDiffuse[2]    = DirLight2DiffuseColor;
        lightSpecular[2]   = DirLight2SpecularColor;
        halfVectors[2] = normalize(eyeVector - lightDirections[2]);
    }

    float3 dotL = mul(lightDirections*-1, worldNormal);
    float3 dotH = mul(halfVectors, worldNormal);
    
    float3 zeroL = step(float3(0,0,0), dotL);

    float3 diffuse  = zeroL * dotL;
    float3 specular = pow(max(dotH, 0) * zeroL, SpecularPower);

    ColorPair result;
    
    result.Diffuse  = mul(diffuse,  lightDiffuse)  * DiffuseColor.rgb + EmissiveColor;
    result.Specular = mul(specular, lightSpecular) * SpecularColor;

    return result;
}


CommonVSOutput ComputeCommonVSOutputWithLighting(float4 position, float3 normal, uniform int numLights)
{
    CommonVSOutput vout;
    
    float4 pos_ws = mul(position, World);
    float3 eyeVector = normalize(EyePosition - pos_ws.xyz);
    float3 worldNormal = normalize(mul(normal, WorldInverseTranspose));

    ColorPair lightResult = ComputeLights(eyeVector, worldNormal, numLights);
    
    vout.Pos_ps = mul(position, WorldViewProj);
    vout.Diffuse = float4(lightResult.Diffuse, DiffuseColor.a);
    vout.Specular = lightResult.Specular;
    vout.FogFactor = ComputeFogFactor(position);
    
    return vout;
}


struct CommonVSOutputPixelLighting
{
    float4 Pos_ps;
    float3 Pos_ws;
    float3 Normal_ws;
    float  FogFactor;
};


CommonVSOutputPixelLighting ComputeCommonVSOutputPixelLighting(float4 position, float3 normal)
{
    CommonVSOutputPixelLighting vout;
    
    vout.Pos_ps = mul(position, WorldViewProj);
    vout.Pos_ws = mul(position, World).xyz;
    vout.Normal_ws = normalize(mul(normal, WorldInverseTranspose));
    vout.FogFactor = ComputeFogFactor(position);
    
    return vout;
}


#define SetCommonVSOutputParamsPixelLighting \
    vout.PositionPS = cout.Pos_ps; \
    vout.PositionWS = float4(cout.Pos_ws, cout.FogFactor); \
    vout.NormalWS = cout.Normal_ws;
