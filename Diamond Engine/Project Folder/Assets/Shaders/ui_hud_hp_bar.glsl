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
uniform float t;

void main() {
	float half_len = (1-length_used)/2;
	float half_hp = (1-last_hp)/2;
	if (textureCoords.x > (1-half_hp) || textureCoords.x < half_hp)
	{
		fragmentColor=vec4(0,0,0,0);
	}
	else if(textureCoords.x<(1-half_hp) && textureCoords.x > (1-half_len) || textureCoords.x > half_hp && textureCoords.x < half_len)
	{
		fragmentColor = texture(ourTexture,textureCoords)*vec4(0.5,0,0.1,1);
	}
	else
	{
		if(length_used > 0.5)
		{
			fragmentColor = texture(ourTexture,textureCoords)*mix(vec4(1*t,0.51*t,0,1),vec4(0.03*t,0.6*t,0.05*t,1),(length_used-0.5)*2);
		}
		else if(length_used <= 0.5)
		{
			fragmentColor = texture(ourTexture,textureCoords)*mix(vec4(1*t,0,0,1),vec4(1*t,0.51*t,0,1),length_used*2);
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






