#pragma once

#include<vector>
#include<string>
#include <map>

#include "Component.h"
#include"parson/parson.h"

class C_Transform;
struct SerializedField;

class GameObject
{
public:

	GameObject(const char*, GameObject* parent, int _uid = -1);
	virtual ~GameObject();

	void Update();
	void PostUpdate();

	Component* AddComponent(Component::TYPE _type, const char* params = nullptr);
	Component* GetComponent(Component::TYPE _type, const char* scriptName = nullptr);
	std::vector<Component*> GetComponentsOfType(Component::TYPE type);

	void RecursivePrefabReferenceGeneration();
	void RecursiveUIDRegeneration();
	void RecursiveUIDRegenerationSavingReferences(std::map<uint, GameObject*>& gameObjects);
	void UnlinkFromPrefab();
	void OverrideGameObject(uint prefabID, bool prefabChild = false);

	bool isActive() const;

	void Enable();
	void Disable();

	void EnableTopDown();
	void DisableTopDown();

	bool IsRoot();
	void Destroy();

	void SaveToJson(JSON_Array* _goArray, bool saveAllData = true);
	void SavePrefab(JSON_Array* _goArray, bool saveAllData = true);
	void SaveAsPrefabRoot(JSON_Object* goData, bool prefabInsidePrefab);
	void SaveReducedData(JSON_Array* goArray, bool prefabInsidePrefab = false);
	void LoadFromJson(JSON_Object*);
	void LoadForPrefab(JSON_Object*);
	void CopyObjectData(JSON_Object* jsonObject);
	void GetChildrenUIDs(std::vector<uint>& UIDs);

	void LoadComponents(JSON_Array* componentArray);
	void RemoveComponent(Component* ptr);

	void ChangeParent(GameObject* newParent);
	bool IsChild(GameObject*);

	void RemoveChild(GameObject*);
	void CollectChilds(std::vector<GameObject*>& vector);
	void RemoveCSReference(SerializedField* fieldToRemove);

	bool CompareTag(const char* _tag);

	GameObject* GetChild(std::string& childName);

	template<typename A>
	A* GetComponent()
	{
		return (A*)GetComponent(A::GetType());
	}

	GameObject* parent;
	C_Transform* transform;

	std::vector<GameObject*> children;

	std::vector<Component*> components;

	std::vector<SerializedField*> csReferences;

public:
	std::string name;
	bool active, isStatic;

	//TODO: Temporal tree display bool
	bool showChildren;
	bool toDelete;
	bool dontDestroy;

	int UID;
	uint prefabID;
	uint prefabReference;

	char tag[32];
	char layer[32];

private:
	Component* dumpComponent;

};