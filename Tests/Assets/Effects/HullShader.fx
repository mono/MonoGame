struct VertexIn
{
	float4 Position : SV_POSITION;
};

struct HullOut
{
	float4 Position : BEZIERPOS;
};

struct PatchConstantOut
{
	float Edges[3] : SV_TessFactor;
	float Inside   : SV_InsideTessFactor;
};

PatchConstantOut PatchConstantFunc(
	InputPatch<VertexIn, 3> ip,
	uint patchID : SV_PrimitiveID)
{
	PatchConstantOut output;

	output.Edges[0] = 10;
	output.Edges[1] = 10;
	output.Edges[2] = 10;

	output.Inside = 10;

	return output;
}

[domain("tri")]  
[partitioning("fractional_even")]  
[outputtopology("triangle_cw")]  
[outputcontrolpoints(3)]
[patchconstantfunc("PatchConstantFunc")]
[maxtessfactor(30.0)]
HullOut HullShaderFunction(InputPatch<VertexIn, 3> ip, uint i : SV_OutputControlPointID, uint patchID : SV_PrimitiveID)
{
	HullOut output;

	output.Position = ip[i].Position;

	return output;
}

technique Technique1
{
	pass Pass1
	{
		HullShader = compile hs_5_0 HullShaderFunction();
	}
}

