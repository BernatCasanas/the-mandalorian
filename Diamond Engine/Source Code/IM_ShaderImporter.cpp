#include "IM_ShaderImporter.h"
#include "OpenGL.h"
#include<string>
#include"Globals.h"
#include"IM_FileSystem.h"
#include "WI_TextEditor.h"
#include "MO_Editor.h"

#include "mmgr/mmgr.h"

void ShaderImporter::Import(char* buffer, int bSize, ResourceShader* res, const char* assetsPath)
{
	//Get every shader typs string from the glsl
	//Buffer is now a complete shader with 2 types
	//Get a string for every shader type with find
	//Compile string
	//Save binary as type sizes + shader codes
	if (buffer == NULL)
		return;

	std::string bufferString(buffer);
	TempShader vertexShaderPair;
	TempShader fragmentShaderPair; 
	TempShader geometryShaderPair;

	CheckForErrors(bufferString, vertexShaderPair, fragmentShaderPair, geometryShaderPair);

	if (vertexShaderPair.tmpID != 0 && fragmentShaderPair.tmpID != 0)
	{
		res->shaderObjects[(int)ShaderType::SH_Vertex] = vertexShaderPair.tmpID;
		res->shaderObjects[(int)ShaderType::SH_Frag] = fragmentShaderPair.tmpID;
		res->shaderObjects[(int)ShaderType::SH_Geometry] = geometryShaderPair.tmpID;

		res->LinkToProgram();

		char* saveBuffer = res->SaveShaderCustomFormat(vertexShaderPair.data.second, vertexShaderPair.data.first, fragmentShaderPair.data.second, fragmentShaderPair.data.first, geometryShaderPair.data.second, geometryShaderPair.data.first);
		
		std::string shaderFileName = SHADERS_PATH;
		shaderFileName += std::to_string(res->GetUID());
		shaderFileName += ".shdr";
		
		FileSystem::Save(shaderFileName.c_str(), saveBuffer, 12 + vertexShaderPair.data.first + fragmentShaderPair.data.first + geometryShaderPair.data.first, false);

		//res->LoadShaderCustomFormat(shaderFileName.c_str());

		RELEASE_ARRAY(saveBuffer);
	}
}

bool ShaderImporter::CheckForErrors(std::string& glslBuffer, TempShader& vertexShader, TempShader& fragmentShader, TempShader& geometryShader)
{
	size_t startByte = glslBuffer.find("#ifdef vertex");
	if (startByte != std::string::npos)
	{
		startByte += sizeof("#ifdef vertex");
		vertexShader.data.first = glslBuffer.find("#endif", startByte) - startByte - 1;
		vertexShader.data.second = &glslBuffer[startByte];
	}
	startByte = std::string::npos;

	startByte = glslBuffer.find("#ifdef fragment");
	if (startByte != std::string::npos)
	{
		startByte += sizeof("#ifdef fragment");
		fragmentShader.data.first = glslBuffer.find("#endif", startByte) - startByte - 1;
		fragmentShader.data.second = &glslBuffer[startByte];
	}
	startByte = std::string::npos;

	startByte = glslBuffer.find("#ifdef geometry");
	if (startByte != std::string::npos)
	{
		startByte += sizeof("#ifdef geometry");
		geometryShader.data.first = glslBuffer.find("#endif", startByte) - startByte - 1;
		geometryShader.data.second = &glslBuffer[startByte];

		geometryShader.tmpID = Compile(geometryShader.data.second, ShaderType::SH_Geometry, geometryShader.data.first);
	}

	vertexShader.tmpID = Compile(vertexShader.data.second, ShaderType::SH_Vertex, vertexShader.data.first);
	fragmentShader.tmpID = Compile(fragmentShader.data.second, ShaderType::SH_Frag, fragmentShader.data.first);
	

	//RELEASE_ARRAY(buffer);
	return (vertexShader.tmpID == 0 && fragmentShader.tmpID == 0) ? false : true;
}

/*Method to import different file shaders, not in use for now*/
//void ShaderImporter::Import(char* buffer, int bSize, ResourceShader* res, const char* assetsPath)
//{
//	//Get every shader typs string from the glsl
//
//	std::string pairShader = "";
//	char* pairBuffer = nullptr;
//
//	ShaderType type = GetAssetsObjType(assetsPath, pairShader);
//
//	int fobSize = FileSystem::LoadToBuffer(pairShader.c_str(), &pairBuffer);
//	if (fobSize == 0)
//		return;
//
//	GLuint vertexShader = 0;
//	GLuint fragmentShader = 0;
//
//	vertexShader = Compile(buffer, type, bSize);
//	fragmentShader = Compile(pairBuffer, ShaderType::SH_Frag, fobSize);
//
//	if (vertexShader != 0 && fragmentShader != 0)
//	{
//		res->shaderObjects[(int)ShaderType::SH_Vertex] = vertexShader;
//		res->shaderObjects[(int)ShaderType::SH_Frag] = fragmentShader;
//
//		res->LinkToProgram();
//
//		char* saveBuffer = res->SaveShaderCustomFormat(buffer, bSize, pairBuffer, fobSize);
//
//		std::string shaderFileName = SHADERS_PATH;
//		shaderFileName += std::to_string(res->GetUID());
//		shaderFileName += ".shdr";
//
//		FileSystem::Save(shaderFileName.c_str(), saveBuffer, 8 + bSize + fobSize, false);
//
//		//res->LoadShaderCustomFormat(shaderFileName.c_str());
//
//		RELEASE_ARRAY(saveBuffer);
//	}
//
//	//RELEASE_ARRAY(buffer);
//	if (pairBuffer == nullptr) {
//		LOG(LogType::L_ERROR, "One of the shader objects does not exist");
//	}
//	else
//		RELEASE_ARRAY(pairBuffer);
//}

GLuint ShaderImporter::Compile(char* fileBuffer, ShaderType type, const GLint size)
{
	GLuint compileShader = 0;

	switch (type)
	{
	case ShaderType::SH_Vertex:
		compileShader = glCreateShader(GL_VERTEX_SHADER);
		break;

	case ShaderType::SH_Frag:
		compileShader = glCreateShader(GL_FRAGMENT_SHADER);
		break;

	case ShaderType::SH_Geometry:
		compileShader = glCreateShader(GL_GEOMETRY_SHADER);
		break;
	}
	
	glShaderSource(compileShader, 1, &fileBuffer, &size);
	glCompileShader(compileShader);

	int success = 0;
	char infoLog[512];
	glGetShaderiv(compileShader, GL_COMPILE_STATUS, &success);
	if (success == 0)
	{
		glGetShaderInfoLog(compileShader, 512, NULL, infoLog);

		switch (type)
		{
		case ShaderType::SH_Vertex:
			LOG(LogType::L_ERROR, "Error compilating vertex shader: %s", infoLog);
			break;

		case ShaderType::SH_Frag:
			LOG(LogType::L_ERROR, "Error compilating fragment shader: %s", infoLog);
			break;

		case ShaderType::SH_Geometry:
			LOG(LogType::L_ERROR, "Error compilating geometry shader: %s", infoLog);
			break;

		case ShaderType::SH_Max:
		default:
			LOG(LogType::L_ERROR, "Invalid shader type");
			break;
		}			

		glDeleteShader(compileShader);

#ifndef STANDALONE
		W_TextEditor* textEditorWindow = static_cast<W_TextEditor*>(EngineExternal->moduleEditor->GetEditorWindow(EditorWindow::TEXTEDITOR));
		textEditorWindow->SetErrorsOnScreen(infoLog);
#endif // !STANDALONE


		return 0;
	}

	return compileShader;
}

int ShaderImporter::GetTypeMacro(ShaderType type)
{
	int ret = 0;
	switch (type)
	{
	case ShaderType::SH_Vertex:
		ret = GL_VERTEX_SHADER;
		break;
	case ShaderType::SH_Frag:
		ret = GL_FRAGMENT_SHADER;
		break;
	case ShaderType::SH_Geometry:
		ret = GL_GEOMETRY_SHADER;
		break;
	case ShaderType::SH_Max:
		ret = 0;
		break;
	default:
		ret = 0;
		break;
	}

	return ret;
}


void ShaderImporter::CreateBaseShaderFile(const char* path)
{
	std::string defaultFile;

	defaultFile = "#ifdef vertex\n#version 330 core\n\n#endif\n\n#ifdef fragment\n#version 330 core\n\n#endif";

	FileSystem::Save(path, &defaultFile[0], defaultFile.length(), false);
}
