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

void main()
{
	out_Colour = texture(colourTexture,textureCoords);
}
#endif




















