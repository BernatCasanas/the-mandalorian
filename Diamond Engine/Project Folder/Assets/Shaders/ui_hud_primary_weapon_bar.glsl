#ifdef vertex
#version 330 core
layout (location = 0) in vec2 aPos;
out vec2 textureCoords;

uniform mat4 model_matrix;

void main() {
	gl_Position = model_matrix * vec4(aPos, 0, 1.0);
	textureCoords = vec2((aPos.x + 1.0) * 0.5,(aPos.y + 1.0) * 0.5);

}
#endif

#ifdef fragment
#version 330 core
in vec2 textureCoords;

out vec4 fragmentColor;

uniform sampler2D ourTexture;
uniform float length_used;
uniform float colorLerpLength;


uniform vec3 textureColorModStart;
uniform vec3 textureColorModEnd;

float Lerp(float a, float b, float t)
{
return (1.0 - t) * a + b * t;
}

float InvLerp(float from,float to,float value)
{
return (value - from) / (to - from);
}

float Remap (float iMin,float iMax,float oMin, float oMax,float v)
{
float t = InvLerp(iMin, iMax, v);
return Lerp(oMin,oMax,t);
}

void main() {

	if(textureCoords.x>length_used )
	{
		fragmentColor=vec4(0,0,0,0);
	}
	else
	{
		vec3 color;
		color.r = Remap(0, 1, textureColorModStart.r, textureColorModEnd.r, colorLerpLength);
		color.g = Remap(0, 1, textureColorModStart.g, textureColorModEnd.g, colorLerpLength);
		color.b = Remap(0, 1, textureColorModStart.b, textureColorModEnd.b, colorLerpLength);
		
		color.g = min(color.g, 0.85);

		fragmentColor = vec4(color,1);
	}
	
}

#endif









