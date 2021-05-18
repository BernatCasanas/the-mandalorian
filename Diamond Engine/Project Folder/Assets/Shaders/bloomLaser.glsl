#ifdef vertex
#version 330 core
layout (location = 0) in vec3 position;
layout (location = 2) in vec3 normals;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;

void main()
{
	FragPos = vec3(model_matrix * vec4(position, 1.0));
	gl_Position = projection * view * vec4(FragPos, 1.0);
}
#endif

#ifdef fragment
#version 330 core

uniform vec3 altColor;

out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in float vertexIntensity;
uniform float bloomEmissiveIntensity;//1 for normal color, above 1 will multiply the color and hence will have a higher threshold for the bloom effect

void main()
{

    FragColor = vec4(altColor*bloomEmissiveIntensity, 1.0);
}
#endif








