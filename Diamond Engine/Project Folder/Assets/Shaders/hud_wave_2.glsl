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
uniform float t;
uniform float rate;

void main() {
	if(textureCoords.x>length_used){
		fragmentColor=vec4(0,0,0,0);
		}
	else{
		fragmentColor = texture(ourTexture,vec2(textureCoords.x + t*rate, textureCoords.y))* mix(vec4(0.5,0.6,0.6,1), vec4(0.03,0.75,0.62,1), length_used);
	}
}

#endif





