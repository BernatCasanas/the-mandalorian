#ifdef vertex
#version 330 core

const int MAX_JOINTS = 100;
const int MAX_WEIGHTS = 4;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normals;
layout (location = 3) in vec3 tangents;
layout (location = 4) in vec4 boneIDs;
layout (location = 5) in vec4 weights;
layout (location = 6) in vec3 colors;

out vec3 influenceColor;
out vec3 Normal;
out vec3 fPosition;
out vec3 vertexColor;
out vec3 diffuseColor;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

uniform float time;

uniform mat4 jointTransforms[MAX_JOINTS];

uniform vec3 lightPosition;
vec3 lightColor;

void main()
{
	lightColor = vec3(0.225, 0.150, 0.120);
	vec4 totalPosition = vec4(0.0);
	
	for(int i= 0; i < MAX_WEIGHTS; i++){
	
	  if(boneIDs[i] == -1 || weights[i] == 0)
	   	continue;
	   	
	  if(boneIDs[i] >= MAX_JOINTS)
	  {
	    totalPosition = vec4(position, 1.0f);
	    break;
	  }
	  
	  vec4 localPosition = jointTransforms[int(boneIDs[i])] * vec4(position, 1.0f);
	  totalPosition += localPosition * weights[i];
	  vec3 localNormal = mat3(jointTransforms[int(boneIDs[i])]) * normals;
	
	}
	
	mat4 viewModel = view * model_matrix;
	gl_Position = projection * viewModel * totalPosition;
	influenceColor = vec3(weights.x, weights.y, weights.z);
	//ourColor = vec3(boneIDs.x / 30, boneIDs.y / 30, boneIDs.z / 30);
	vertexColor = colors;
	
	lightColor = vec3(0.225, 0.150, 0.120);
	vec3 lightDirection = vec3(lightPosition - totalPosition.xyz);
	diffuseColor = vec3(max(dot(lightDirection, -normals), 0) * lightColor);
    
}
#endif

#ifdef fragment
#version 330 core

in vec3 influenceColor;
in vec3 vertexColor; 
in vec3 diffuseColor; 

out vec4 color;

uniform float damaged;

void main()
{
 	//color = vec4(influenceColor, 1.0);
 	color = vec4(vertexColor + diffuseColor, 1.0);
 	color = vec4(color.x*(1+damaged), color.y*(1+damaged), color.z*(1+damaged), 1);
}
#endif
