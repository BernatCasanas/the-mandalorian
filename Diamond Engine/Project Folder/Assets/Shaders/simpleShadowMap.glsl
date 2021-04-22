#ifdef vertex
#version 330 core

const int MAX_JOINTS = 100;
const int MAX_WEIGHTS = 4;

layout (location = 0) in vec3 position;
layout (location = 2) in vec3 normals;
layout (location = 4) in vec4 boneIDs;
layout (location = 5) in vec4 weights;

uniform mat4 lightSpaceMatrix;
uniform mat4 model;

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
	
gl_Position = lightSpaceMatrix * model * posFinal;

}

#endif

#ifdef fragment
#version 330 core

void main()
{
	//gl_FragDepth = gl_FragCoord.z;
}

#endif



