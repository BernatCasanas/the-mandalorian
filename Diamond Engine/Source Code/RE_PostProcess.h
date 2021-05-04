#pragma once

#include "DEResource.h"

#include<vector>
#include "parson/parson.h"

typedef unsigned int GLuint;
typedef unsigned int GLenum;
typedef int GLint;
typedef int GLsizei;

class ResourceShader;
class ResourceTexture;

class ResourcePostProcess : public Resource {
public:
	ResourcePostProcess(unsigned int _uid);
	~ResourcePostProcess();

	bool LoadToMemory() override;
	bool UnloadFromMemory() override;

#ifndef STANDALONE
	void DrawEditor(std::string suffix);
#endif // !STANDALONE
	void SaveToJson(JSON_Array* json);

public:

};
