#ifdef vertex
#version 330 core

layout (location=0) in vec4 posAndTextures;
out vec2 textureCoords;

void main()
{
	gl_Position= vec4(posAndTextures.xy,0.0,1.0);
	textureCoords= posAndTextures.zw;
}
#endif

#ifdef fragment
#version 330 core

in vec2 textureCoords;
out vec4 out_Colour;
uniform sampler2D colourTexture;

void main()
{
	vec4 colorMultiplier= vec4(1,0,0,1);
	vec4 myColor = colorMultiplier * texture(colourTexture,textureCoords);
	out_Colour = vec4(1,0,0,1);
}
#endif






