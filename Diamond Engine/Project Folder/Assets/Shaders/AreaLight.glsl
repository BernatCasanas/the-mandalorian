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
} vs_out;



uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

struct AreaLightInfo 
{
	vec3 lightPosition;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	bool active;

};


uniform AreaLightInfo areaLightInfo[5];

uniform vec3 cameraPosition;

void main()
{
    vs_out.FragPos = vec3(model_matrix * vec4(position, 1.0));
    vs_out.Normal = normalize(transpose(inverse(mat3(model_matrix))) * normals);

    gl_Position = projection * view * model_matrix * vec4(position, 1.0);

}
#endif

#ifdef fragment
#version 330 core

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
} fs_in;

struct AreaLightInfo 
{
	vec3 lightPosition;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	bool active;
};


uniform AreaLightInfo areaLightInfo[5];

uniform vec3 cameraPosition;

out vec4 color;


void main()
{
    vec3 lighting = vec3(0.0, 0.0, 0.0);
    
    for (int i = 0; i < 5; i++)
    {
		if (areaLightInfo[i].active == true)
		{
    		vec3 lightDir = normalize(areaLightInfo[i].lightPosition - fs_in.FragPos);
    
   	 		// diffusesssaasw
    		float diff = max(dot(lightDir, fs_in.Normal), 0.0);
    		vec3 diffuse = diff * areaLightInfo[i].lightColor;
   	 		// specular
   			vec3 viewDir = normalize(cameraPosition - fs_in.FragPos);
    		float spec = 0.0;
    		vec3 halfwayDir = normalize(lightDir + viewDir);  
    		spec = pow(max(dot(fs_in.Normal, halfwayDir), 0.0), areaLightInfo[i].specularValue);
    		vec3 specular = spec * areaLightInfo[i].lightColor;
        	
    	 // calculate shadow
    		lighting += (areaLightInfo[i].ambientLightColor + (diffuse + specular)) * areaLightInfo[i].lightIntensity;
    	}
    }
    color = vec4(lighting * vec3(0.7, 0.3, 0.3), 1.0);
}
#endif




