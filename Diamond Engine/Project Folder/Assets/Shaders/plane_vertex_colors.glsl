#ifdef vertex
#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normals;
layout (location = 3) in vec3 tangents;
layout (location = 6) in vec3 colors;

out VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace[2];
    
    vec3 TangentLightPos[2];
    vec3 TangentViewPos;
    vec3 TangentFragPos;
    
    vec3 vertexColor;
} vs_out;



uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

struct LightInfo 
{
	vec3 lightPosition;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	bool calculateShadows;
	bool active;

};


uniform LightInfo lightInfo[2];

uniform vec3 cameraPosition;

void main()
{
    vs_out.vertexColor = colors;
    
    vs_out.FragPos = vec3(model_matrix * vec4(position, 1.0));
    vs_out.Normal = normalize(transpose(inverse(mat3(model_matrix))) * normals);
    vs_out.TexCoords = texCoord;
    vs_out.FragPosLightSpace[0] = lightInfo[0].lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);
    vs_out.FragPosLightSpace[1] = lightInfo[1].lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);

	
	//Calculate tangent vars
	vec3 T = normalize(vec3(model_matrix * vec4(tangents, 0.0)));
    vec3 N = normalize(vec3(model_matrix * vec4(normals,  0.0)));
    vec3 B = cross(N, T);
	
	mat3 TBN = transpose(mat3(T, B, N));
    vs_out.TangentLightPos[0] = TBN * lightInfo[0].lightPosition;
    vs_out.TangentLightPos[1] = TBN * lightInfo[1].lightPosition;
    vs_out.TangentViewPos  = TBN * cameraPosition;
    vs_out.TangentFragPos  = TBN * vec3(model_matrix * vec4(position, 1.0));

    gl_Position = projection * view * model_matrix * vec4(position, 1.0);

}
#endif

#ifdef fragment
#version 330 core

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace[2];
    
    vec3 TangentLightPos[2];
    vec3 TangentViewPos;
    vec3 TangentFragPos;
    
    vec3 vertexColor;
} fs_in;

in vec3 vertexColor;  

struct LightInfo 
{
	vec3 lightPosition;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	bool calculateShadows;
	bool active;
};


uniform LightInfo lightInfo[2];

uniform sampler2D shadowMap;
uniform sampler2D normalMap;
uniform vec3 cameraPosition;

out vec4 color;



float ShadowCalculation(vec4 fragPosLightSpace, vec3 lightDir, vec3 normal, int iterator)
{

	if (lightInfo[iterator].calculateShadows == false)
		return 0.0;
	
// perform perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
   
    if(projCoords.z > 1.0)
    	return 0.0;
    
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // check whether current frag pos is in shadow
	float bias = max(0.0005 * (1.0 - dot(normal, lightDir)), 0.0005);
	float shadow = 0.0;
	
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
    shadow /= 9.0;
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        shadow = 0.0;
        
    return shadow;  
}


float GetShadowValue()
{
	vec3 normal = texture(normalMap, fs_in.TexCoords).rgb;
    normal = normalize(normal * 2.0 - 1.0);
    
	for	(int i = 0; i < 2; i++)
    {
    	if(lightInfo[i].calculateShadows == true)
    	{
    		vec3 lightDir = normalize(fs_in.TangentLightPos[i] - vec3(0, 0, 0));
    		return ShadowCalculation(fs_in.FragPosLightSpace[i], lightDir, normal, i);
    	}
    }

}


void main()
{
	vec3 normal = texture(normalMap, fs_in.TexCoords).rgb;
    normal = normalize(normal * 2.0 - 1.0);
    
    vec3 lighting = vec3(0.0, 0.0, 0.0);
    
    float shadow = GetShadowValue();
    
    
    for (int i = 0; i < 2; i++)
    {
    	if (lightInfo[i].active == true)
    	{
    		vec3 lightDir = normalize(fs_in.TangentLightPos[i] - vec3(0, 0, 0));
    
   	 	// diffusesssaasw
    		float diff = max(dot(lightDir, normal), 0.0);
    		vec3 diffuse = diff * lightInfo[i].lightColor;
   	 	// specular
   	 		vec3 viewDir = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos[i]);
    		float spec = 0.0;
    		vec3 halfwayDir = normalize(lightDir + viewDir);  
    		spec = pow(max(dot(normal, halfwayDir), 0.0), lightInfo[i].specularValue);
    		vec3 specular = spec * lightInfo[i].lightColor;
        	
    	 // calculate shadow
    		lighting += (lightInfo[i].ambientLightColor + (1.0 - shadow) * (diffuse + specular)) * lightInfo[i].lightIntensity;
    	}
    }
    
    color = vec4(lighting * fs_in.vertexColor, 1.0);
}
#endif






