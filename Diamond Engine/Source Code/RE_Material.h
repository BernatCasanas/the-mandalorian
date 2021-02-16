#pragma once

#include "DEResource.h"
#include<vector>

#include"MathGeoLib/include/Math/float4x4.h"
#include"MathGeoLib/include/Math/float2.h"
#include"MathGeoLib/include/Math/float3.h"
#include"MathGeoLib/include/Math/float4.h"

typedef unsigned int GLuint;
typedef unsigned int GLenum;
typedef int GLint;
typedef int GLsizei;

class ResourceShader;

struct ShaderVariable
{
	ShaderVariable();
	~ShaderVariable();

	GLuint vIndex;
	GLenum vType;
	GLint vSize;

	GLsizei nameLength;
	char name[25];

	union ShdrValue {
		ShdrValue();
		int intValue;
		float floatValue;
		float2 vector2Value;
		float3 vector3Value;
		float4 vector4Value;
		GLuint textureValue;
		float4x4* matrixValue;
	} data;
};

static const char* defaultUniforms[] = { "model_matrix", "view", "projection", "normalMatrix", "time", "ourTexture","altColor"};

class ResourceMaterial : public Resource {
public:
	ResourceMaterial(unsigned int _uid);
	~ResourceMaterial();

	bool LoadToMemory() override;
	bool UnloadFromMemory() override;

	void FillVariables();

	void PushUniforms();

#ifndef STANDALONE
	void DrawEditor();
#endif // !STANDALONE

public:

	std::vector<ShaderVariable> attributes;
	std::vector<ShaderVariable> uniforms;

	ResourceShader* shader;

};
