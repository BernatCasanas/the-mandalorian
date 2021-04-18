#include "IM_PrefabImporter.h"
#include "Globals.h"
#include "GameObject.h"
#include "DEJsonSupport.h"
#include "Application.h"
#include "MO_ResourceManager.h"
#include "IM_FileSystem.h"
#include "MO_Scene.h"
#include "CO_Transform.h"
#include "CO_Script.h"

#include <string>

int PrefabImporter::SavePrefab(const char* assets_path, GameObject* gameObject)
{
	JSON_Value* file = json_value_init_object();
	DEConfig root_object(json_value_get_object(file));

	JSON_Value* goArray = json_value_init_array();
	gameObject->SaveToJson(json_value_get_array(goArray));

	json_object_set_string(root_object.nObj, "assets_path", assets_path);
	json_object_set_value(root_object.nObj, "Game Objects", goArray);

	//Save file 
	json_serialize_to_file_pretty(file, assets_path);

	int uid = EngineExternal->moduleResources->ImportFile(assets_path, Resource::Type::PREFAB);
	EngineExternal->moduleResources->GenerateMeta(assets_path, EngineExternal->moduleResources->GenLibraryPath(uid, Resource::Type::PREFAB).c_str(),
												  uid, Resource::Type::PREFAB);

	//Free memory
	json_value_free(file);

	return uid;
}

GameObject* PrefabImporter::LoadPrefab(const char* libraryPath, std::vector<GameObject*>& sceneObjects, bool overriding)
{
	int oldSize = EngineExternal->moduleScene->activeScriptsVector.size();

	GameObject* rootObject = sceneObjects.front();

	JSON_Value* prefab = json_parse_file(libraryPath);

	if (prefab == nullptr)
		return nullptr;

	JSON_Object* prefabObj = json_value_get_object(prefab);
	JSON_Array* gameObjectsArray = json_object_get_array(prefabObj, "Game Objects");
	JSON_Object* goJsonObj = json_array_get_object(gameObjectsArray, 0);

	GameObject* parent = rootObject;
	std::map<uint, GameObject*> prefabObjects;

	for (size_t j = 0; j < json_array_get_count(gameObjectsArray); j++)
	{
		bool newObject = true;
		JSON_Object* jsonObject = json_array_get_object(gameObjectsArray, j);
		int uid = json_object_get_number(jsonObject, "UID");

		for (size_t i = 0; i < sceneObjects.size() && newObject; i++)
		{
			if (uid == sceneObjects[i]->prefabReference)
			{
				prefabObjects[sceneObjects[i]->prefabReference] = sceneObjects[i];
				sceneObjects[i]->LoadComponents(json_object_get_array(jsonObject, "Components"));
				newObject = false;
			}
		}

		if (newObject)
		{
			int prefabID = json_object_get_number(jsonObject, "PrefabID");
			int parentID = json_object_get_number(jsonObject, "ParentUID");
			bool parentSet = false;

			for (size_t i = 0; i < sceneObjects.size() && !parentSet; ++i)
			{
				if (sceneObjects[i]->prefabReference == parentID ||sceneObjects[i]->UID == parentID)
				{
					parent = EngineExternal->moduleScene->LoadGOData(jsonObject, parent);
					sceneObjects.push_back(parent);
					parentSet = true;
				}
			}
		}
	}

	if (overriding)
	{
		for (size_t i = 0; i < sceneObjects.size(); i++)
		{
			if (prefabObjects.find(sceneObjects[i]->prefabReference) == prefabObjects.end())
			{
				sceneObjects[i]->Destroy();
			}
		}
	}

	EngineExternal->moduleScene->LoadNavigationData();
	EngineExternal->moduleScene->LoadScriptsData(rootObject);

	//replace the components references with the new GameObjects using their old UIDs
	std::map<uint, GameObject*>::const_iterator it = prefabObjects.begin();
	for (it; it != prefabObjects.end(); it++)
	{
		for (size_t i = 0; i < it->second->components.size(); i++)
		{
			it->second->components[i]->OnRecursiveUIDChange(prefabObjects);
		}
	}

	std::vector<C_Script*> saveCopy; //We need to do this in case someone decides to create an instance inside the awake method
	for (int i = oldSize; i < EngineExternal->moduleScene->activeScriptsVector.size(); ++i)
		saveCopy.push_back(EngineExternal->moduleScene->activeScriptsVector[i]);

	for (size_t i = 0; i < saveCopy.size(); i++)
		saveCopy[i]->OnAwake();

	std::string id_string;
	FileSystem::GetFileName(libraryPath, id_string, false);
	rootObject->prefabID = (uint)atoi(id_string.c_str());

	//Free memory
	json_value_free(prefab);
	prefabObjects.clear();

	return rootObject;
}

GameObject* PrefabImporter::InstantiatePrefab(const char* libraryPath)
{
	int oldSize = EngineExternal->moduleScene->activeScriptsVector.size();

	GameObject* rootObject = nullptr;

	JSON_Value* prefab = json_parse_file(libraryPath);

	if (prefab == nullptr)
		return nullptr;

	JSON_Object* prefabObj = json_value_get_object(prefab);
	JSON_Array* gameObjectsArray = json_object_get_array(prefabObj, "Game Objects");
	JSON_Object* goJsonObj = json_array_get_object(gameObjectsArray, 0);

	rootObject = new GameObject(json_object_get_string(goJsonObj, "name"), EngineExternal->moduleScene->root, json_object_get_number(goJsonObj, "UID"));
	rootObject->LoadFromJson(goJsonObj);

	GameObject* parent = rootObject;

	for (size_t i = 1; i < json_array_get_count(gameObjectsArray); i++)
	{
		parent = LoadGOData(json_array_get_object(gameObjectsArray, i), parent);
	}

	EngineExternal->moduleScene->LoadNavigationData();
	//rootObject->RecursiveUIDRegeneration();
	EngineExternal->moduleScene->LoadScriptsData(rootObject);


	//Save all references to game objects with their old UID
	std::map<uint, GameObject*> gameObjects;
	rootObject->RecursiveUIDRegenerationSavingReferences(gameObjects);

	//replace the components references with the new GameObjects using their old UIDs
	std::map<uint, GameObject*>::const_iterator it = gameObjects.begin();
	for (it; it != gameObjects.end(); it++)
	{
		it->second->prefabReference = it->first;
		for (size_t i = 0; i < it->second->components.size(); i++)
		{
			it->second->components[i]->OnRecursiveUIDChange(gameObjects);
		}
	}

	std::vector<C_Script*> saveCopy; //We need to do this in case someone decides to create an instance inside the awake method
	for (int i = oldSize; i < EngineExternal->moduleScene->activeScriptsVector.size(); ++i)
		saveCopy.push_back(EngineExternal->moduleScene->activeScriptsVector[i]);

	for (size_t i = 0; i < saveCopy.size(); i++)
		saveCopy[i]->OnAwake();

	std::string id_string;
	FileSystem::GetFileName(libraryPath, id_string, false);
	rootObject->prefabID = (uint)atoi(id_string.c_str());

	//Free memory
	json_value_free(prefab);
	gameObjects.clear();

	return rootObject;
}

GameObject* PrefabImporter::InstantiatePrefabAt(uint prefabID, float3 position, Quat rotation, float3 scale)
{
	std::string libraryPath = EngineExternal->moduleResources->GenLibraryPath(prefabID, Resource::Type::PREFAB);
	GameObject* prefab = InstantiatePrefab(libraryPath.c_str());

	C_Transform* oldObjectTransform = prefab->transform;
	prefab->transform->SetTransformMatrix(oldObjectTransform->position, oldObjectTransform->rotation, oldObjectTransform->localScale);
	prefab->transform->updateTransform = true;

	return prefab;
}

GameObject* PrefabImporter::LoadGOData(JSON_Object* goJsonObj, GameObject* parent)
{
	GameObject* originalParent = parent;

	while (parent != nullptr && json_object_get_number(goJsonObj, "ParentUID") != parent->UID)
		parent = parent->parent;

	if (parent == nullptr)
		parent = originalParent;

	parent = new GameObject(json_object_get_string(goJsonObj, "name"), parent, json_object_get_number(goJsonObj, "UID"));
	parent->LoadFromJson(goJsonObj);

	return parent;
}

void PrefabImporter::OverridePrefab(uint prefabID, GameObject* referenceObject)
{
	std::string libraryPath = EngineExternal->moduleResources->GenLibraryPath(prefabID, Resource::Type::PREFAB);

	JSON_Value* prefab = json_parse_file(libraryPath.c_str());
	std::string assets_path;

	if (prefab == nullptr)
	{
		if (!FileSystem::Exists(libraryPath.c_str())) {
			assets_path = "Assets/Prefabs/" + referenceObject->name + ".prefab";
			LOG(LogType::L_ERROR, "The prefab tried to override does not exist, it will be created at: %s", assets_path.c_str());
		}
	}
	else
	{
		JSON_Object* prefabObj = json_value_get_object(prefab);
		assets_path = json_object_get_string(prefabObj, "assets_path");
	}

	referenceObject->prefabID = SavePrefab(assets_path.c_str(), referenceObject);
	//EngineExternal->moduleResources->ImportFile(assets_path.c_str(), Resource::Type::PREFAB);

	json_value_free(prefab);

	EngineExternal->moduleScene->prefabToOverride = prefabID;
}

void PrefabImporter::OverrideGameObject(uint prefabID, GameObject* objectToReplace)
{
	std::string libraryPath = EngineExternal->moduleResources->GenLibraryPath(prefabID, Resource::Type::PREFAB);

	std::vector<GameObject*> childs;
	objectToReplace->CollectChilds(childs);

	LoadPrefab(libraryPath.c_str(), childs, true);
}
