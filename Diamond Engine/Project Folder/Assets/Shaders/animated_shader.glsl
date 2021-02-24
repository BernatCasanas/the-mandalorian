#ifdef vertex
#version 330 core

const int MAX_JOINTS = 50;
const int MAX_WEIGHTS = 4;

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 color;
layout (location = 2) in vec2 texCoord;
layout (location = 3) in vec3 normals;
layout (location = 4) in ivec4 boneIDs;
layout (location = 5) in vec4 weights;

out vec3 ourColor;
out vec2 TexCoord;
out vec3 Normal;
out vec3 fPosition;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

uniform float time;

uniform mat4 jointTransforms[MAX_JOINTS];

void main()
{
Normal = mat3(transpose(inverse(model_matrix))) * normals;

vec4 totalLocalPos = vec4(0.0);
vec4 totalNormal = vec4(0.0);

for (int i=0; i < MAX_JOINTS; i++){
	vec4 localPosition = jointTransforms[boneIDs[i]] * vec4(position, 1.0);
	totalLocalPos += localPosition * weights[i];
	
	vec4 worldNormal = jointTransforms[boneIDs[i]] * vec4(normals, 0.0);
	totalNormal += worldNormal * weights[i];
}

vec4 pos = model_matrix * vec4(position, 1.0);
fPosition = pos.xyz;

gl_Position = projection * view * model_matrix * vec4(position, 1.0f);

ourColor = vec3(weights.x, weights.x, weights.x);
TexCoord = texCoord;
Normal = totalNormal.xyz;
}
#endif

#ifdef fragment
#version 330 core
in vec3 ourColor;
in vec2 TexCoord;
out vec4 color;
uniform sampler2D ourTexture;

in vec3 fPosition;
in vec3 Normal;

uniform vec3 altColor;
uniform float time;

vec2 blinnPhongDir(vec3 lightDir, float lightInt, float Ka, float Kd, float Ks, float shininess)
{
vec3 s = normalize(lightDir);
vec3 v = normalize(-fPosition);
vec3 n = normalize(Normal);
vec3 h = normalize(v+s);
float diffuse = Ka + Kd * lightInt * max(0.0, dot(n, s));
float spec = Ks * pow(max(0.0, dot(n,h)), shininess);
return vec2(diffuse, spec);
}

void main()
{
vec3 lcolor = vec3(1.0,1.0,1.0);
vec2 inten = blinnPhongDir(vec3(0.8,1,0.2), 0.5, 0.2, 0.8, 0.3, 80.0);
gl_FragColor = vec4(lcolor * inten.x + vec3(1.0) * inten.y, 1.0) * vec4(ourColor, 1.0);
}
#endif














