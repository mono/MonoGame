uniform sampler2D TextureSampler_s0;
uniform sampler2D OverlaySampler_s1;

uniform vec2 OverlayScroll;

void main()
{
	vec4 tex = gl_Color * texture2D(TextureSampler_s0, gl_TexCoord[0].xy);
	vec4 tex2 = gl_Color * texture2D(OverlaySampler_s1, OverlayScroll + gl_TexCoord[0].xy);
	float fadeSpeed = tex2.x;
	vec4 color = tex;
	color *= clamp((tex.a - fadeSpeed) * 2.5 + 1.0,0.0,1.0);
	gl_FragColor = color;

}
//float2 OverlayScroll;

//sampler TextureSampler : register(s0);
//sampler OverlaySampler : register(s1);


//float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
//{
    //// Look up the texture color.
    //float4 tex = tex2D(TextureSampler, texCoord);
    
    // Look up the fade speed from the scrolling overlay texture.
    //float fadeSpeed = tex2D(OverlaySampler, OverlayScroll + texCoord).x;
    
    // Apply a combination of the input color alpha and the fade speed.
    //tex *= saturate((color.a - fadeSpeed) * 2.5 + 1);
    
    //return tex;
//}
