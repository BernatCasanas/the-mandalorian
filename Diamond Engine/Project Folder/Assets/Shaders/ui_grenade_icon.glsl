#ifdef vertex
#version 330 core
layout (location = 0) in vec2 aPos;
out vec2 textureCoords;

uniform mat4 model_matrix;

void main() {
	gl_Position = model_matrix * vec4(aPos, 0.0, 1.0);
	textureCoords = vec2((aPos.x + 1.0) * 0.5,(aPos.y + 1.0) * 0.5);

}
#endif

#ifdef fragment
#version 330 core
in vec2 textureCoords;

out vec4 fragmentColor;

uniform sampler2D ourTexture;

uniform float maxGrenadeCooldown;
uniform float currentGrenadeCooldown;

vec4 darkenImage = vec4(0.2, 0.2, 0.2, 0.7);

void main() 
{
	float value = currentGrenadeCooldown / maxGrenadeCooldown;

	if (textureCoords.y > value)
		fragmentColor = texture(ourTexture,textureCoords) - darkenImage;
		
	else
		fragmentColor = texture(ourTexture,textureCoords);
}

#endif
