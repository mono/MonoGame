
struct HullOut
{
	float4 Position : BEZIERPOS;
};

struct PatchConstantOut
{
	float Edges[3] : SV_TessFactor;
	float Inside   : SV_InsideTessFactor;
};

struct VertexOut
{
	float4 Position : SV_POSITION;
};


[domain("tri")]
VertexOut DomainShaderFunction(const OutputPatch<HullOut, 3> patch, float3 barycentric : SV_DomainLocation, PatchConstantOut patchConst)
{
	VertexOut output;

	output.Position =
		patch[0].Position * barycentric.x +
		patch[1].Position * barycentric.y +
		patch[2].Position * barycentric.z;

	return output;
}

technique Technique1
{
	pass Pass1
	{
		DomainShader = compile ds_5_0 DomainShaderFunction();
	}
}

