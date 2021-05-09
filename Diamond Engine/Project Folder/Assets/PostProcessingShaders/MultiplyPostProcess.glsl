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
uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
	vec4 myColor1 = texture(texture1,textureCoords);
	vec4 myColor2 = texture(texture2,textureCoords);

	out_Colour= myColor1*myColor2;
}
#endif





















