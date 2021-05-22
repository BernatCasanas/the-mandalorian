#ifdef vertex
#version 330 core

const int MAX_JOINTS = 100;
const int MAX_WEIGHTS = 4;

layout (location = 0) in vec3 position;
layout (location = 2) in vec3 normals;
layout (location = 4) in vec4 boneIDs;
layout (location = 5) in vec4 weights;

uniform mat4 model_matrix;

uniform mat4 jointTransforms[MAX_JOINTS];

uniform int hasAnimations;


vec4 CalculateAnimations()
{
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
	
	return totalPosition;
}

vec4 NoAnimation()
{
	return vec4(position, 1.0);
}

void main()
{
	vec4 posFinal = vec4(0.0);
	
	if(hasAnimations == 1)
	{
	posFinal = CalculateAnimations();
	}else
	{
	posFinal = NoAnimation();
	}
	
gl_Position = model_matrix * posFinal;

}

#endif

#ifdef fragment
#version 330 core

in vec4 fragPos;

uniform vec3 lightPosition;
uniform float farPlane;

void main()
{
	float lightDistance = length(fragPos.xyz - lightPosition);
	lightDistance = lightDistance / farPlane;
	
	gl_FragDepth = lightDistance;
}

#endif


#ifdef geometry
#version 330 core

layout (triangles) in;
layout (triangle_strip, max_vertices = 18) out;

uniform mat4 shadowMatrices[6];


out vec4 fragPos;

void main()
{
	for (int face = 0; face < 6; ++face)
	{
		gl_Layer = face;
		
		for (int i = 0; i < 3; ++i)
		{
			fragPos = gl_in[i].gl_Position;
			gl_Position = shadowMatrices[face] * fragPos;
			EmitVertex();
		}
	 
		EndPrimitive();
	} 
}

#endif











