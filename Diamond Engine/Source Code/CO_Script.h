#pragma once
#include "Component.h"
#include<vector>
#include<string>

#include"MO_MonoManager.h"
#include <mono/metadata/object-forward.h>

class GameObject;
typedef struct _MonoClassField MonoClassField;

class C_Script : public Component
{
public:

	C_Script(GameObject* _gm, const char* scriptName);
	virtual ~C_Script();

	//void Start() override;
	void Update() override;

#ifndef STANDALONE
	bool OnEditor() override;
#endif // !STANDALONE

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

	void SetField(MonoClassField* field, GameObject* value);
	void DropField(SerializedField& fieldName, const char* dropType);

	void OnRecursiveUIDChange(std::map<uint, GameObject*> gameObjects) override;
	void LoadScriptData(const char*);

	void CollisionCallback(bool isTrigger, GameObject* collidedGameObject);
	void CollisionPersistCallback(GameObject* collidedGameObject);
	void CollisionExitCallback(bool isTrigger, GameObject* collidedGameObject);

	void ExecuteButton();
	void ExecuteCheckbox(bool checkbox_active);

	void OnStart();
	void OnAwake();

	void OnApplicationQuit();

	std::vector<std::string> methods;
	std::vector<SerializedField> fields;

	MonoMethod* updateMethod;

	MonoMethod* onAwake;
	MonoMethod* onStart;

	MonoMethod* onCollisionEnter;
	MonoMethod* onTriggerEnter;
	MonoMethod* onCollisionStay;
	MonoMethod* onCollisionExit;
	MonoMethod* onTriggerExit;


	MonoMethod* onApplicationQuit;
	
	MonoMethod* onExecuteButton;
	MonoMethod* onExecuteCheckbox;
	uint32_t noGCobject;

	static C_Script* runningScript;
};