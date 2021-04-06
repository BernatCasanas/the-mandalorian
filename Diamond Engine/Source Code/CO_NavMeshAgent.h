#pragma once
#include "Component.h"
#include "MO_Pathfinding.h"
#include <vector>

struct NavAgentProperties {
	float speed = 0.0f;
	float angularSpeed = 0.0f;
	float stoppingDistance = 0.0f;

};

class C_NavMeshAgent : public Component
{
public:
	C_NavMeshAgent(GameObject* _gm);
	virtual ~C_NavMeshAgent();

#ifndef STANDALONE
	bool OnEditor() override;
#endif // !STANDALONE

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;
	std::vector<float3> path;
	NavAgentProperties properties;

private:
	NavAgent* selectedNav = nullptr;
};