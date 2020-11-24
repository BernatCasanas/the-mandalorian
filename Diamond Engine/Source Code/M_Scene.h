#pragma once

#include "Module.h"
#include<vector>

#include"parson/parson.h"
class GameObject;
class C_Camera;

class M_Scene : public Module
{

public:
	M_Scene(Application* app, bool start_enabled = true);
	virtual ~M_Scene();

	bool Init() override;
	bool Start() override;

	update_status PreUpdate(float dt) override;
	update_status Update(float dt) override;

	bool CleanUp() override;

	GameObject* CreateGameObject(const char* name, GameObject* parent, int _uid = -1);

	void SaveScene();
	void LoadScene(const char* name);

	GameObject* root;

	std::vector<GameObject*> destroyList;

	void SetGameCamera(C_Camera* cam);

private:
	void Destroy(GameObject* gm);

	void UpdateGameObjects();
	void RecursiveUpdate(GameObject* parent);
};