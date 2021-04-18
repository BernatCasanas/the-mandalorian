#ifdef vertex
#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normals;
layout (location = 6) in vec3 colors;

out vec3 fPosition;
out VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} vs_out;

out vec3 vertexColor;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 lightSpaceMatrix;

uniform vec3 lightPosition;
vec3 lightColor = vec3(0.55, 0.35, 0.35);

void main()
{
    vertexColor = colors;
    
    vs_out.FragPos = vec3(model_matrix * vec4(position, 1.0));
    vs_out.Normal = normalize(transpose(inverse(mat3(model_matrix))) * normals);
    vs_out.TexCoords = texCoord;
    vs_out.FragPosLightSpace = lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);

	vec3 fPosition = (model_matrix * vec4(position, 1.0)).xyz;

    gl_Position = projection * view * model_matrix * vec4(position, 1.0);

}
#endif

#ifdef fragment
#version 330 core

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} fs_in;

in vec3 vertexColor; 
in vec3 diffuseColor; 

uniform sampler2D shadowMap;

uniform vec3 lightPos;
uniform vec3 viewPos;
uniform vec3 lightColor;

out vec4 color;

vec3 ambientLight = vec3(0.43, 0.31, 0.29);

float ShadowCalculation(vec4 fragPosLightSpace, vec3 lightDir, vec3 normal)
{
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
    for(int x = -2; x <= 2; ++x)
    {
        for(int y = -2; y <= 2; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
    shadow /= 25.0;
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        shadow = 0.0;
        
    return shadow;  
}

void main()
{
    // diffuse
    vec3 lightDir = normalize(lightPos - vec3(0.0, 0.0, 0.0));
    float diff = max(dot(lightDir, fs_in.Normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(fs_in.Normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;
        
    // calculate shadow
    float shadow = ShadowCalculation(fs_in.FragPosLightSpace, lightDir, fs_in.Normal);
    vec3 lighting = (ambientLight + (1.0 - shadow) * (diffuse + specular)) * 1.6;
    
    color = vec4(lighting * vertexColor, 1.0);
    //color = mix (vec4(diffuseColor + ambientLight, 1.0), vec4(vertexColor, 1.0), 0.7);
}
#endif



