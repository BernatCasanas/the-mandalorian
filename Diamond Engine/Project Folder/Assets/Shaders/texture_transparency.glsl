#ifdef vertex
#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

out vec2 TexCoord;

void main()
{
	TexCoord = texCoord;

	gl_Position = projection * view * model_matrix * vec4(position, 1.0f);
}

#endif

#ifdef fragment
#version 330 core

in vec2 TexCoord;

uniform sampler2D diffuseTexture;

out vec4 color;

void main()
{
	color = texture(diffuseTexture, TexCoord);
}

#endif






