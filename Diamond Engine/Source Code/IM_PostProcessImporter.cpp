#include "IM_PostProcessImporter.h"
#include"Application.h"
#include "MO_ResourceManager.h"
#include"DEJsonSupport.h"

#include "IM_FileSystem.h"

#include "RE_PostProcess.h"
#include "Globals.h"


ResourcePostProcess* PostProcessImporter::CreateBaseProfileFile(const char* path)
{
	JSON_Value* file = json_value_init_object();
	DEConfig root_object(json_value_get_object(file));

	std::string name;
	FileSystem::GetFileName(path, name, false);
	root_object.WriteString("name", name.c_str());

	//JSON_Value* generalVarsArray = json_value_init_array();
	ResourcePostProcess* newProfile = dynamic_cast<ResourcePostProcess*>(EngineExternal->moduleResources->CreateNewResource(path, 0, Resource::Type::POSTPROCESS));
	newProfile->SaveToJson(root_object.nObj);

	//json_object_set_value(root_object.nObj, "GeneralVars", generalVarsArray);

	//Default shader does not have a library path
	if (json_serialize_to_file_pretty(file, path) != JSONFailure)
	{
		LOG(LogType::L_NORMAL, "File saved at: %s", path);
	}
	else
	{
		LOG(LogType::L_ERROR, "Error trying to save at: %s", path);
	}

	json_value_free(file);
	return newProfile;
}

void PostProcessImporter::Save(ResourcePostProcess* postProcess, char** fileBuffer)
{
	JSON_Value* file = json_value_init_object();
	DEConfig root_object(json_value_get_object(file));

	root_object.WriteString("name", postProcess->name);

	//JSON_Value* generalVarsArray = json_value_init_array();
	postProcess->SaveToJson(root_object.nObj);
	//json_object_set_value(root_object.nObj, "GeneralVars", generalVarsArray);

	//TODO: Should be saving Assets material and not library but there is no assets path in the resource :(
	std::string assets_path = "Assets/PostProcessingProfiles/" + std::string(postProcess->name) + ".pprocess";
	json_serialize_to_file_pretty(file, assets_path.c_str());
	json_serialize_to_file_pretty(file, postProcess->GetLibraryPath());

	json_value_free(file);

	LOG(LogType::L_NORMAL, "File saved at: %s", assets_path.c_str());
}
