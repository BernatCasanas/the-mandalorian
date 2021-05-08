#ifdef vertex
#version 330 core

layout (location=0) in vec3 pos;
out vec2 textureCoords;

void main()
{
	gl_Position= vec4(pos,1);
	textureCoords = vec2((pos.x + 1.0) * 0.5,(pos.y + 1.0) * 0.5);
}
#endif

#ifdef fragment
#version 330 core

in vec2 textureCoords;
out vec4 out_Colour;

uniform sampler2D colourTexture;
uniform vec2 screenRes;//resolution of the screen in pixels

uniform float intensity;//15 def
uniform float extend; //0.25 def

uniform vec4 tint;//0.0 def

uniform float outerRadius; //0.5 (half width)
uniform float innerRadius; //0.4 (4/5 the screen)
uniform float vignetteMode;

void main()
{
	float vignette = 1.0;
	vec4 myColor = texture(colourTexture,textureCoords)*vec4(tint.rgb,1.0);

	if(vignetteMode==0)//rectuangular vignette
	{
		vec2 uv = gl_FragCoord.xy / screenRes.xy;
		uv *= (1.0 - uv.yx);
		vignette = uv.x*uv.y * intensity;
		vignette = pow(vignette, extend);
	}
	else //circular vignette
	{
		vec2 relativePos = gl_FragCoord.xy / screenRes.xy -0.5;
		float len = length(relativePos);
		vignette = smoothstep(outerRadius,innerRadius,len);
	}

	myColor.rgb = mix(myColor.rgb*vignette,myColor.rgb,tint.a);
	
	out_Colour=myColor;
}
#endif
























