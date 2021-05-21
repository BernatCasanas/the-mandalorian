#pragma once

#include"RE_Shader.h"
#include<string>
typedef int GLint;

struct TempShader {
	std::pair<size_t, char*> data;
	int tmpID = 0;
};

namespace ShaderImporter
{
	void Import(char* buffer, int bSize, ResourceShader* res, const char* assetsPath);

	bool CheckForErrors(std::string& glslBuffer, TempShader& vertexShader, TempShader& fragmentShader, TempShader& geometryShaderPair);

	int GetTypeMacro(ShaderType type);

	void CreateBaseShaderFile(const char* path);

	GLuint Compile(char* fileBuffer, ShaderType type, const GLint size);
}