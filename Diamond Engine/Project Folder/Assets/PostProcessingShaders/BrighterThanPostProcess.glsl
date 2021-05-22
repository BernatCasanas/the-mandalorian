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
uniform float brightnessTreshold;
uniform bool useSmoothMask;

void main()
{
	vec4 myColor = texture(colourTexture,textureCoords);
	//float brightness = (myColor.r * 0.2126)+(myColor.g * 0.7152)+(myColor.b * 0.0722); //Luma conversion to make the average
	float brightness = (myColor.r + myColor.g + myColor.b)/3.0;

	if(useSmoothMask)
	{
		out_Colour=clamp(myColor*brightness*brightness*brightness*(1/brightnessTreshold),0,1);
	}
	else if(brightness >= brightnessTreshold)
	{
		out_Colour=myColor;
	}
	
}
#endif

























