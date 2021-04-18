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
uniform float limbo;

void main() {
	vec4 color;
	if(textureCoords.x > length_used && textureCoords.x < limbo){
		fragmentColor = texture(ourTexture, textureCoords) *vec4(0,0,0,1) + vec4(1,1,1,0);
	}
	else if (textureCoords.x>length_used)
	{
		fragmentColor=texture(ourTexture, textureCoords) * vec4(0,0,0,0.7);
	}
	else
	{
		fragmentColor = texture(ourTexture, textureCoords);
	}
}

#endif





