#ifdef vertex
#version 330 core

const int GAUSSIAN_KERNEL_SIZE=11;//GAUSSIAN KERNEL MUST BE AN ODD NUMBER

layout (location=0) in vec3 pos;
uniform float targetWidth;
out vec2 blurTextureCoords[GAUSSIAN_KERNEL_SIZE];
void main()
{
	gl_Position= vec4(pos,1);
	vec2 centerTextureCoords =vec2(pos.xy*0.5+0.5);
	float pixelSize = 1.0/targetWidth; //width of the image in texture coords / number of pixels in the width

	for(int i=-(GAUSSIAN_KERNEL_SIZE-1)/2; i<=(GAUSSIAN_KERNEL_SIZE-1)/2; i++)
	{
		blurTextureCoords[i+(GAUSSIAN_KERNEL_SIZE-1)/2] = centerTextureCoords + vec2(pixelSize*i,0.0);
	}


}
#endif

#ifdef fragment
#version 330 core

const int GAUSSIAN_KERNEL_SIZE=11;//GAUSSIAN KERNEL MUST BE AN ODD NUMBER

in vec2 blurTextureCoords[GAUSSIAN_KERNEL_SIZE];
uniform float blurTextureWeights[GAUSSIAN_KERNEL_SIZE];

out vec4 out_Colour;
uniform sampler2D colourTexture;

void main()
{
	out_Colour= vec4(0.0);

	for(int i=0; i<GAUSSIAN_KERNEL_SIZE; i++)
	{
		out_Colour+=texture(colourTexture,blurTextureCoords[i])*blurTextureWeights[i];
	}

}
#endif





















