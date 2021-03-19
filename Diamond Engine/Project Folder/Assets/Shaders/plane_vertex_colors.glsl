#ifdef vertex
#version 330 core

layout (location = 0) in vec3 position;
layout (location = 2) in vec3 normals;
layout (location = 6) in vec3 colors;

out vec3 vertexColor;
out vec3 diffuseColor;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

uniform vec3 lightPosition;
vec3 lightColor = vec3(0.45, 0.35, 0.30);

void main()
{
	vertexColor = colors;
	
	vec3 fPosition = (model_matrix * vec4(position, 1.0)).xyz;
	vec3 lightDirection = normalize(vec3(lightPosition - fPosition));
	diffuseColor = vec3(max(dot(lightDirection, normals), 0) * lightColor);
	
	gl_Position = projection * view * model_matrix * vec4(position, 1.0);
    
}
#endif

#ifdef fragment
#version 330 core

in vec3 vertexColor; 
in vec3 diffuseColor; 

vec3 ambientLight = vec3(0.30, 0.15, 0.10);

out vec4 color;

void main()
{
 	color = mix (vec4(diffuseColor + ambientLight, 1.0), vec4(vertexColor, 1.0), 0.65);
}
#endif













































































