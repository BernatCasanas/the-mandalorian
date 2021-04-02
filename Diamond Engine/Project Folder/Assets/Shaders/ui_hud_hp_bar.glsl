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
uniform float last_hp;

void main() {
	float g = length_used*length_used*1.5;
	if (g >= 0.8)g=0.8;
	if (textureCoords.x > last_hp)
	{
		fragmentColor=vec4(0,0,0,0);
	}
	else if(textureCoords.x<last_hp && textureCoords.x > length_used)
	{
		fragmentColor = texture(ourTexture,textureCoords)*vec4(0.5,0,0.1,1);
	}
	else
	{
		if(length_used > 0.5)
		{
			fragmentColor = texture(ourTexture,textureCoords)*mix(vec4(1,0.51,0,1),vec4(0.03,0.6,0.05,1),(length_used-0.5)*2);
		}
		else if(length_used <= 0.5)
		{
			fragmentColor = texture(ourTexture,textureCoords)*mix(vec4(1,0,0,1),vec4(1,0.51,0,1),length_used*2);
		}
	}
	/*
	else if(length_used<0.15){
		fragmentColor = texture(ourTexture,textureCoords)*vec4(0.6,0,0.1,1);
	}
	else if(length_used<=0.5){
		fragmentColor = texture(ourTexture,textureCoords)*(vec4(0,0.3,0,1)+vec4(0.7,0.2,0,1));
	}
	*/
}

#endif




