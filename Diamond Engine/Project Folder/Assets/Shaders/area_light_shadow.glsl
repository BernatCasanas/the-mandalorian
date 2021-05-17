#ifdef vertex
#version 330 core

layout (location = 0) in vec3 aPosition;

uniform mat4 model_matrix;

void main()
{
	gl_Position = model_matrix * vec4(aPosition, 1.0);
}

#endif

#ifdef fragment
#version 330 core

in vec4 fragPos;

uniform vec3 lightPosition;
uniform float farPlane;
uniform mat4 lightSpaceMatrix;

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






