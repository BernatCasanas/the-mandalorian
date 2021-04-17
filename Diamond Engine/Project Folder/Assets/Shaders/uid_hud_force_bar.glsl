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

void main() {
	float half_len = (1-length_used)/2;
	if(textureCoords.x>(1-half_len) || textureCoords.x < half_len){
		fragmentColor=vec4(0,0,0,0);
		}
	else{
		fragmentColor = texture(ourTexture,vec2(textureCoords.x, textureCoords.y))* mix(vec4(0.5,0.5,0.5,1), vec4(0.2,0.3,1,1), length_used);
	}
}

#endif























