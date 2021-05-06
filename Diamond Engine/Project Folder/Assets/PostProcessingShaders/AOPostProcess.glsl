#ifdef vertex
#version 330 core

layout (location=0) in vec3 pos;
out vec2 textureCoords;
out vec2 viewRay;

uniform float tanHalfFoVx;
uniform float tanHalfFoVy;

void main()
{
	gl_Position= vec4(pos,1);
	textureCoords = vec2((pos.x + 1.0) * 0.5,(pos.y + 1.0) * 0.5);
	
	viewRay.x = pos.x * -tanHalfFoVx;
    viewRay.y = pos.y * -tanHalfFoVy;
}
#endif

#ifdef fragment
#version 330 core

in vec2 textureCoords;
in vec2 viewRay;

out vec4 out_Colour;

uniform sampler2D depthTexture;
uniform mat4 projectionMat;
uniform float sampleRad;
uniform int isOrthographic;


const int MAX_KERNEL_SIZE = 64;
uniform vec3 kernel[MAX_KERNEL_SIZE];

float CalcViewZ(vec2 coords)
{
    float depth = texture(depthTexture, coords).x;
    float viewZ = projectionMat[3][2] / (2 * depth -1 - projectionMat[2][2]);
    return viewZ;
}


void main()
{
	float viewZ = CalcViewZ(textureCoords);

    float viewX = viewRay.x * viewZ;
    float viewY = viewRay.y * viewZ;

    vec3 position = vec3(viewX, viewY, viewZ);


	float AO = 0.0;

    for (int i = 0 ; i < MAX_KERNEL_SIZE ; i++) {
        vec3 samplePos = position + kernel[i]*sampleRad;// generate a random point
        vec4 offset = vec4(samplePos, 1.0); // make it a 4-vector
        offset = projectionMat * offset; // project on the near clipping plane
        offset.xy /= offset.w; // perform perspective divide
        offset.xy = offset.xy * 0.5 + vec2(0.5); // transform to (0,1) range

        float sampleDepth = CalcViewZ(offset.xy);

        if (abs(position.z - sampleDepth) < sampleRad) {
            AO += step(sampleDepth,samplePos.z);
            
        }
    }

    AO = 1- AO / MAX_KERNEL_SIZE;
	//AO = clamp(AO,0.0,0.5);
    out_Colour = vec4(pow(AO,2));
    //out_Colour = vec4(pos,1.0);


}
#endif

















