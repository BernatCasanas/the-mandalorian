#ifdef vertex
#version 330 core

layout (location=0) in vec3 pos;
out vec2 textureCoords;
out vec2 viewRay;

uniform float tanHalfFoVx;
uniform float tanHalfFoVy;
uniform float cameraSize;
uniform int isOrthographic;
void main()
{
	gl_Position= vec4(pos,1); //-1,1
	textureCoords = vec2((pos.x + 1.0) * 0.5,(pos.y + 1.0) * 0.5);//0,1
	
	if(isOrthographic==1)
	{
		viewRay.x = pos.x * tanHalfFoVx;
		viewRay.y = pos.y * tanHalfFoVy;
	}
	else
	{
		viewRay.x = pos.x * -tanHalfFoVx;
		viewRay.y = pos.y * -tanHalfFoVy;
	}
}
#endif

#ifdef fragment
#version 330 core

in vec2 textureCoords;
in vec2 viewRay;

out vec4 out_Colour;

uniform sampler2D depthTexture;
uniform sampler2D noiseTexture;
uniform mat4 projectionMat;
uniform float sampleRad;
uniform int isOrthographic;

uniform float cameraSize;
const int MAX_KERNEL_SIZE = 64;
uniform vec3 kernel[MAX_KERNEL_SIZE];
uniform vec2 depthDimensions;
uniform float bias;

float CalcViewZ(vec2 coords) //converts depth at coords from clip space to view space
{
	float depth = texture(depthTexture, coords).x; 
	
	if(isOrthographic==1)
	{
		return (2*depth-1-projectionMat[3][2])/projectionMat[2][2];
	}
	else
	{
		return projectionMat[3][2] / (2 * depth -1 - projectionMat[2][2]);
	}
    
}

vec3 CalculateFlatNormal(vec2 coords) //Note this should make artifacts on geometry edges
{
	vec2 uv0 = coords; // center
	vec2 uv1 = coords + vec2(1, 0) / depthDimensions; // right 
	vec2 uv2 = coords + vec2(0, 1) / depthDimensions; // top

	float depth0 = texture(depthTexture, uv0, 0).r;
	float depth1 = texture(depthTexture, uv1, 0).r;
	float depth2 = texture(depthTexture, uv2, 0).r;
	
	vec3 P0 = vec3(uv0, depth0);
	vec3 P1 = vec3(uv1, depth1);
	vec3 P2 = vec3(uv2, depth2);
	
	vec3 normal = normalize(cross(P2 - P0, P1 - P0));

	return normal;
}

vec3 CalculateSmoothNormal(vec2 coords,float depth0) //More performance intesive, no edge case errors
{
	vec2 uv0 = coords; // center
	vec2 uv1 = coords + vec2(1, 0) / depthDimensions; // right 
	vec2 uv2 = coords + vec2(0, 1) / depthDimensions; // top
	vec2 uv3 = coords + vec2(-1, 0) / depthDimensions; // left
	vec2 uv4 = coords + vec2(0, -1) / depthDimensions; // bottom

	//float depth0 = CalcViewZ(uv0);
	float depth1 = CalcViewZ(uv1);
	float depth2 = CalcViewZ(uv2);
	float depth3 = CalcViewZ(uv3);
	float depth4 = CalcViewZ(uv4);

	//float depth0 = texture(depthTexture, uv0, 0).r;
	//float depth1 = texture(depthTexture, uv1, 0).r;
	//float depth2 = texture(depthTexture, uv2, 0).r;
	//float depth3 = texture(depthTexture, uv3, 0).r;
	//float depth4 = texture(depthTexture, uv4, 0).r;

	int bestHorizontal = ( abs(depth1-depth0) < abs(depth3-depth0) ) ? 1 : 3; //1 right 3 left
	int bestVertical = ( abs(depth2-depth0) < abs(depth4-depth0) ) ? 2 : 4; //2 up 4 bottom

	vec3 P0 = vec3(uv0, depth0);
	vec3 P1 = vec3(0);
	vec3 P2 = vec3(0);

	if(bestHorizontal==1)
	{
		if(bestVertical==2)
		{
			P1 = vec3(uv1, depth1);
			P2 = vec3(uv2, depth2);
		}
		else //best vertical ==4
		{
			P1 = vec3(uv4, depth4);
			P2 = vec3(uv1, depth1);
		}
	}
	else //best horizontal ==3
	{
		if(bestVertical==2)
		{
			P1 = vec3(uv2, depth2);
			P2 = vec3(uv3, depth3);
		}
		else //best vertical ==4
		{
			P1 = vec3(uv3, depth3);
			P2 = vec3(uv4, depth4);
		}
	}

	vec3 normal = normalize(cross(P2 - P0, P1 - P0));

	return normal;
}


void main()
{

	vec3 position= vec3(0,0,0);
	float AO = 0.0;
	if(isOrthographic==1)
	{	
		// tile noise texture over screen, based on screen dimensions divided by noise size
		vec2 noiseScale = vec2(depthDimensions.x/4.0, depthDimensions.y/4.0); // noise 4x4 texture

		float viewZ  = 	CalcViewZ(textureCoords); //get view space depth from texture coords
 
    	float viewX = viewRay.x;
    	float viewY = viewRay.y;

    	position = vec3(viewX, viewY, viewZ); //position in view space

		//Construct orthogonal basis (use to rotate kernel)

		vec3 randomVec = texture(noiseTexture,textureCoords*noiseScale).xyz;

		vec3 normal = CalculateSmoothNormal(textureCoords,viewZ);
		vec3 tangent   = normalize(randomVec - normal * dot(randomVec, normal));
		vec3 bitangent = cross(normal, tangent);
		mat3 TBN       = mat3(tangent, bitangent, normal);  

		//Iterate all kernel samples

		for (int i = 0 ; i < MAX_KERNEL_SIZE ; i++) 
		{
        	vec3 samplePos = TBN*kernel[i];//rotate the kernel by TBN rotation matrix
      		samplePos = position + samplePos * sampleRad;// generate a random point

			vec4 offset= vec4(samplePos,1);
			offset = projectionMat * offset; //change from view to clip space
			//offset.xyz /= offset.w;               // perspective divide only in perspective?
			offset.xyz  = offset.xyz * 0.5 + 0.5; // transform to range 0.0 - 1.0

        	float sampleDepth = CalcViewZ(offset.xy);

			float rangeCheck = smoothstep(0.0, 1.0, sampleRad / abs(position.z - sampleDepth));
			AO += (sampleDepth >= samplePos.z + bias  ? 1.0 : 0.0) * rangeCheck; 


    	}

    	AO = 1.0 - (AO / MAX_KERNEL_SIZE);

	}
	else
	{
		float viewZ = CalcViewZ(textureCoords);

    	float viewX = viewRay.x * viewZ;
    	float viewY = viewRay.y * viewZ;

    	position = vec3(viewX, viewY, viewZ);


    	for (int i = 0 ; i < MAX_KERNEL_SIZE ; i++) 
		{
        	vec3 samplePos = position + kernel[i]*sampleRad;// generate a random point
      	
        	vec4 offset = vec4(samplePos,1); // make it a 4-vector
        	offset = projectionMat * offset; // project on the near clipping plane
        	offset.xy /= offset.w; // perform perspective divide
        	offset.xy = offset.xy * 0.5 + vec2(0.5); // transform to (0,1) range

        	float sampleDepth = CalcViewZ(offset.xy);

        	if ((position.z-sampleDepth) < sampleRad) 
			{
            	AO += step(sampleDepth,samplePos.z);
            
        	}
    	}

    	AO = 1 - (AO / MAX_KERNEL_SIZE);
    	//AO*=2;
		//AO = clamp(AO,0.0,1.0);


	}

    	out_Colour = vec4(AO,AO,AO,1);
    	//out_Colour = vec4(CalculateSmoothNormal(textureCoords),1.0);
}
#endif








































