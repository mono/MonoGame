
struct Vertex
{
	float4 Position : SV_POSITION;
	float4 LocalPosition : TEXCOORD0;
};


[maxvertexcount(100)]
void GeometryShaderFunction(triangle in Vertex vertex[3], inout TriangleStream<Vertex> triStream)
{
	float3 v0 = vertex[0].LocalPosition.xyz;
	float3 v1 = vertex[1].LocalPosition.xyz;
	float3 v2 = vertex[2].LocalPosition.xyz;

	float size = 0.1;

	for (float t = 0; t <= 1; t += size)
	{
		float3 origin = lerp(v0, v1, t);

		vertex[0].Position = float4(origin + v0 * size, 1);
		vertex[1].Position = float4(origin + v1 * size, 1);
		vertex[2].Position = float4(origin + v2 * size, 1);
		
		triStream.Append(vertex[0]);
		triStream.Append(vertex[1]);
		triStream.Append(vertex[2]);

		triStream.RestartStrip();
	}
}


technique Technique1
{
	pass Pass1
	{
		GeometryShader = compile gs_4_0 GeometryShaderFunction();
	}
}
