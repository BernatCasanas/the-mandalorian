#pragma once
#include "Globals.h"

class GameObject;
#include"parson/parson.h"
#include <vector>
#include "MathGeoLib/include/MathGeoLib.h"

namespace PrefabImporter
{
	int SavePrefab(const char* assets_path, GameObject* gameObject);
	GameObject* LoadPrefab(const char* libraryPath, std::vector<GameObject*>& objects);
	GameObject* InstantiatePrefab(const char* libraryPath);
	GameObject* InstantiatePrefabAt(uint prefabID, float3 position, Quat rotation, float3 scale);
	GameObject* LoadGOData(JSON_Object* goJsonObj, GameObject* parent);

	void OverridePrefab(uint prefabID, GameObject* referenceObject);
	void OverridePrefabGameObjects(uint prefabID, GameObject* gameObject);
	void OverrideGameObject(uint prefabID, GameObject* objectToReplace, GameObject* referenceObject = nullptr);
}