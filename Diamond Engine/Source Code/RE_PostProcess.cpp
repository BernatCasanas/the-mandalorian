#include "RE_PostProcess.h"

#include"DEJsonSupport.h"
#include"IM_FileSystem.h"
#include "ImGui/imgui.h"

ResourcePostProcess::ResourcePostProcess(unsigned int _uid) : Resource(_uid, Resource::Type::POSTPROCESS)
{}

ResourcePostProcess::~ResourcePostProcess()
{
}

bool ResourcePostProcess::LoadToMemory()
{
	//Load file to buffer [DONE]
	JSON_Value* file = json_parse_file(this->libraryFile.c_str());
	DEConfig base(json_value_get_object(file));

	strcpy(name, base.ReadString("name"));

	
	//load here

	json_value_free(file);

	return false;
}

bool ResourcePostProcess::UnloadFromMemory()
{

	//TODO: Unload resources (uniform and attributes) by reference count 

	return true;
}

#ifndef STANDALONE
void ResourcePostProcess::DrawEditor(std::string suffix)
{
	std::string label = "";
	label += suffix;
}
#endif // !STANDALONE
void ResourcePostProcess::SaveToJson(JSON_Array* uniformsArray)
{

}