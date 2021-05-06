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
	vec3 lightForward;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	float lFadeDistance;
	float lMaxDistance;
	
	bool activeLight;
	bool calculateShadows;
};


uniform AreaLightInfo areaLightInfo[30];

uniform vec3 cameraPosition;

out vec4 color;

vec4 upRight = vec4(1.0, 1.0, 0.0, 1.0);
vec4 downRight = vec4(1.0, -1.0, 0.0, 1.0);
vec4 upLeft = vec4(-1.0, 1.0, 0.0, 1.0);
vec4 downLeft = vec4(-1.0, -1.0, 0.0, 1.0);

vec3 rectVertex[4];

float SignedDistancePlanePoint(vec3 planePoint, vec3 lForward)
{
	return dot(lForward, (fs_in.FragPos - planePoint));
}


vec3 SetVectorLenght(vec3 vector, float dst)
{
	vec3 normVector = normalize(vector);
	
	return (normVector * dst);
}


vec3 ProjectPointOnPlane(vec3 planePoint, vec3 lForward)
{
	float dst = SignedDistancePlanePoint(planePoint, lForward);
	
	//Reverse the sign
	dst *= -1;
	
	
	vec3 translationVector = SetVectorLenght(lForward, dst);

	
	return vec3(fs_in.FragPos + translationVector);
}


bool IsBetween(vec3 value, vec3 b, vec3 c)
{
	float dst = distance(b, c);
	
	if (distance(value, b) <= dst && distance(value, c) <= dst)
		return true;
		
	return false;
}


vec3 NearestPointOnLine(vec3 point, vec3 lineP1, vec3 lineP2)
{
	vec3 lineDir = normalize(lineP2 - lineP1);
	
	vec3 v = point - lineP1;
	float d = dot(v, lineDir);
	
	return vec3(lineP1 + lineDir * d);
}


float LengthScuared(vec3 line)
{
	return (line.x * line.x + line.y * line.y + line.z * line.z);
}


vec3 NearestPointOnFiniteLine(vec3 point, vec3 lineP1, vec3 lineP2)
{
	float dst = LengthScuared(lineP2 - lineP1);
	
	if (dst == 0.0)
		return lineP1;
		
	float t = max(0, min(1, dot(point - lineP1, lineP2 - lineP1) / dst));
	
	return (lineP1 + t * (lineP2 - lineP1));
}


vec3 CalculateClosestPoint(vec3 projectedPoint)
{
	float closestDistance = 1000000000.0;
	int closestPointIndex;
	
	for (int i = 0; i < 4; ++i)
	{
		float dst = distance(projectedPoint, rectVertex[i]);
		
		if (dst < closestDistance)
		{
			closestDistance = dst;
			closestPointIndex = i;
		}
	
	}
	
	vec3 closestVertex = rectVertex[closestPointIndex];
	
	vec3 nextVertex;
	vec3 prevVertex;
	
	if (closestPointIndex == 0)
	{
		nextVertex = rectVertex[1];
		prevVertex = rectVertex[3];
	}
	
	else if (closestPointIndex == 3)
	{
		nextVertex = rectVertex[0];
		prevVertex = rectVertex[2];
	}
	
	else
	{
		nextVertex = rectVertex[closestPointIndex + 1];
		prevVertex = rectVertex[closestPointIndex - 1];
	}
	
	vec3 projectedPoint1 = NearestPointOnLine(projectedPoint, nextVertex, closestVertex);
	vec3 projectedPoint2 = NearestPointOnLine(projectedPoint, prevVertex, closestVertex);
	
	if (IsBetween(projectedPoint1, closestVertex, nextVertex) && IsBetween(projectedPoint2, closestVertex, prevVertex))
		return projectedPoint;
		
	else
	{
		vec3 posiblePoint1 = NearestPointOnFiniteLine(projectedPoint, nextVertex, closestVertex);
		vec3 posiblePoint2 = NearestPointOnFiniteLine(projectedPoint, prevVertex, closestVertex);
			
			
		if (distance(projectedPoint, posiblePoint1) < distance (projectedPoint, posiblePoint2))
			return posiblePoint1;
			
				
		else
			return posiblePoint2;
	}

}


vec3 CalculateLightDirection(int iterator)
{
	rectVertex[0] = vec4(areaLightInfo[iterator].lightSpaceMatrix * upRight).xyz;
	rectVertex[1] = vec4(areaLightInfo[iterator].lightSpaceMatrix * downRight).xyz;
	rectVertex[2] = vec4(areaLightInfo[iterator].lightSpaceMatrix * downLeft).xyz;
	rectVertex[3] = vec4(areaLightInfo[iterator].lightSpaceMatrix * upLeft).xyz;
	
	vec3 projectedPoint = ProjectPointOnPlane(rectVertex[0], areaLightInfo[iterator].lightForward);
	
	vec3 lightPos = CalculateClosestPoint(projectedPoint);

	return (lightPos - fs_in.FragPos);
}


void main()
{
    vec3 lighting = vec3(0.0, 0.0, 0.0);
    
    for (int i = 0; i < 30; i++)
    {
		if (areaLightInfo[i].activeLight == true)
		{
    		vec3 lightDir = CalculateLightDirection(i);
    		float fade = 1.0;
    		float dst = LengthScuared(lightDir);
    		
    		if (dst > areaLightInfo[i].lMaxDistance)
    			continue;
    		
    		else if (dst > areaLightInfo[i].lFadeDistance && dst < areaLightInfo[i].lMaxDistance)
    			fade = 1.0 - ((dst - areaLightInfo[i].lFadeDistance) / (areaLightInfo[i].lMaxDistance - areaLightInfo[i].lFadeDistance));
			
			lightDir = normalize(lightDir);
    		
    		if (dot(lightDir, areaLightInfo[i].lightForward) <= 0.0)
    		{
   	 			// diffuse
    			float diff = max(dot(lightDir, fs_in.Normal), 0.0);
    			vec3 diffuse = diff * areaLightInfo[i].lightColor;
   	 			// specular
   				vec3 viewDir = normalize(cameraPosition - fs_in.FragPos);
    			float spec = 0.0;
    			vec3 halfwayDir = normalize(lightDir + viewDir);  
    			spec = pow(max(dot(fs_in.Normal, halfwayDir), 0.0), areaLightInfo[i].specularValue);
    			vec3 specular = spec * areaLightInfo[i].lightColor;
        	
    	 		// calculate light value
    			lighting += (areaLightInfo[i].ambientLightColor + (diffuse + specular)) * areaLightInfo[i].lightIntensity * fade;
    		}
    	}
    }
    color = vec4(lighting * vec3(0.7, 0.3, 0.3), 1.0);
}
#endif

























