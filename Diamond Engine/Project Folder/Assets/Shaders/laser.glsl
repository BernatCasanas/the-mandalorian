#ifdef vertex
#version 330 core
layout (location = 0) in vec3 position;
layout (location = 2) in vec3 normals;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;
uniform float time;
uniform vec3 cameraPosition;

out vec3 FragPos;
out vec3 Normal;
out float vertexIntensity;

void main()
{
	FragPos = vec3(model_matrix * vec4(position, 1.0));
	Normal = normalMatrix * normals;
	
	vertexIntensity = dot(cameraPosition - FragPos, normals);
	gl_Position = projection * view * vec4(FragPos, 1.0);
}
#endif

#ifdef fragment
#version 330 core

uniform vec3 cameraPosition;
uniform vec3 altColor;
uniform float time;

out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in float vertexIntensity;

void main()
{
	//vec3 color = 
	float intensity = dot(normalize(cameraPosition - FragPos), Normal);
	intensity += (sin(time * 1.25) + 1.0) * 0.4;
    FragColor = vec4(altColor + intensity * 0.25, 1.0);
    //FragColor = vec4(altColor, 1.0);
}
#endif






