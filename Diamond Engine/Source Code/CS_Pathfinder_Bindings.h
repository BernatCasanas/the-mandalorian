#pragma once
#include "MO_Pathfinding.h"
#include "MO_MonoManager.h"
#include "CO_NavMeshAgent.h"
#include "CS_Transform_Bindings.h"
#include <math.h>
#include <vector>

bool CS_CalculateRandomPath(MonoObject* go, MonoObject* startPos, float radius)
{
	if (EngineExternal == nullptr || go == nullptr)
		return false;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	if (comp == nullptr)
		return false;

	comp->path.clear();
	MonoClass* klass = mono_object_get_class(startPos);

	const char* name = mono_class_get_name(klass);
	float3 destination = EngineExternal->modulePathfinding->FindRandomPointAround(EngineExternal->moduleMono->UnboxVector(startPos), radius);
	if (!EngineExternal->modulePathfinding->FindPath(EngineExternal->moduleMono->UnboxVector(startPos), destination, comp->path))
	{
		comp->path.push_back(destination);
	}
	return EngineExternal->modulePathfinding->FindPath(EngineExternal->moduleMono->UnboxVector(startPos), destination, comp->path);
}

bool CS_CalculatePath(MonoObject* go, MonoObject* startPos, MonoObject* endPos)
{
	if (EngineExternal == nullptr || go == nullptr)
		return false;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	if (comp == nullptr)
		return false;

	std::vector<float3> possibleVector;

	if (EngineExternal->modulePathfinding->FindPath(EngineExternal->moduleMono->UnboxVector(startPos), EngineExternal->moduleMono->UnboxVector(endPos), possibleVector))
	{
		comp->path = possibleVector;
		return true;
	}

	return false;
}

int CS_GetPathSize(MonoObject* go)
{
	if (EngineExternal == nullptr || go == nullptr)
		return 0;

	C_NavMeshAgent* agent = DECS_CompToComp<C_NavMeshAgent*>(go);

	if (agent == nullptr)
		return 0;

	return agent->path.size();
}

MonoObject* CS_GetPointAt(MonoObject* cs_agent, int index)
{
	if (EngineExternal == nullptr || cs_agent == nullptr)
		return nullptr;

	C_NavMeshAgent* agent = DECS_CompToComp< C_NavMeshAgent*>(cs_agent);

	if (agent == nullptr || agent->path.size() < index)
		return nullptr;

	float3 position = agent->path[index];

	return EngineExternal->moduleMono->Float3ToCS(position);
}

void CS_ClearPath(MonoObject* go)
{
	if (EngineExternal == nullptr || go == nullptr)
		return;

	C_NavMeshAgent* agent = DECS_CompToComp<C_NavMeshAgent*>(go);

	if (agent == nullptr)
		return;

	agent->path.clear();
}

MonoObject* CS_GetDestination(MonoObject* go)
{
	if (EngineExternal == nullptr || go == nullptr)
		return nullptr;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	C_Transform* trans = comp->GetGO()->transform;

	if (comp == nullptr)
		return nullptr;

	if (comp->path.size() <= 0) 
		return EngineExternal->moduleMono->Float3ToCS(trans->position);

	float3 distance = comp->path.front() - trans->position;
	if (Sqrt(distance.x * distance.x + distance.y * distance.y + distance.z * distance.z) < comp->properties.stoppingDistance) 
	{
		if (comp->path.size() > 1)
			comp->path.erase(comp->path.begin());
		
		assert(!comp->path.empty());
	}
	return EngineExternal->moduleMono->Float3ToCS(comp->path.front());
}

MonoObject* CS_GetLastVector(MonoObject* go)
{
	if (EngineExternal == nullptr || go == nullptr)
		return nullptr;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	C_Transform* trans = comp->GetGO()->transform;

	if (comp == nullptr)
		return nullptr;

	if (comp->path.size() <= 1)
		return EngineExternal->moduleMono->Float3ToCS(trans->position);

	
	return EngineExternal->moduleMono->Float3ToCS(comp->path.back()-comp->path[comp->path.size()-2]);
}

float CS_GetSpeed(MonoObject* obj)
{
	if (EngineExternal == nullptr || obj == nullptr)
		return 0.0f;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(obj);

	if (comp == nullptr)
		return 0.0f;

	return comp->properties.speed;
}

float CS_GetAngularSpeed(MonoObject* obj)
{
	if (EngineExternal == nullptr || obj == nullptr)
		return 0.0f;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(obj);

	if (comp == nullptr)
		return 0.0f;

	return comp->properties.angularSpeed;
}

float CS_GetStoppingDistance(MonoObject* obj)
{
	if (EngineExternal == nullptr || obj == nullptr)
		return 0.0f;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(obj);

	if (comp == nullptr)
		return 0.0f;

	return comp->properties.stoppingDistance;
}

void CS_SetSpeed(MonoObject* obj, float value)
{
	if (EngineExternal == nullptr || obj == nullptr)
		return;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(obj);

	if (comp == nullptr)
		return;

	comp->properties.speed = value;
}

void CS_SetAngularSpeed(MonoObject* obj, float value)
{
	if (EngineExternal == nullptr || obj == nullptr)
		return;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(obj);

	if (comp == nullptr)
		return;

	comp->properties.angularSpeed = value;
}

void CS_SetStoppingDistance(MonoObject* obj, float value)
{
	if (EngineExternal == nullptr || obj == nullptr)
		return;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(obj);

	if (comp == nullptr)
		return;

	comp->properties.stoppingDistance = value;
}