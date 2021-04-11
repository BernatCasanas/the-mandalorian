#pragma once
#pragma once
#include "MO_Pathfinding.h"
#include "MO_MonoManager.h"
#include "CO_NavMeshAgent.h"
#include "CS_Transform_Bindings.h"
#include <math.h>
#include <vector>

void CS_CalculateRandomPath(MonoObject* go, MonoObject* startPos, float radius)
{
	if (EngineExternal == nullptr || go == nullptr)
		return;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	if (comp == nullptr)
		return;
	comp->path.clear();
	MonoClass* klass = mono_object_get_class(startPos);

	const char* name = mono_class_get_name(klass);
	float3 destination = EngineExternal->modulePathfinding->FindRandomPointAround(EngineExternal->moduleMono->UnboxVector(startPos), radius);
	if (!EngineExternal->modulePathfinding->FindPath(EngineExternal->moduleMono->UnboxVector(startPos), destination, comp->path))
	{
		comp->path.push_back(destination);
	}
}

void CS_CalculatePath(MonoObject* go, MonoObject* startPos, MonoObject* endPos)
{
	if (EngineExternal == nullptr || go == nullptr)
		return;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	if (comp == nullptr)
		return;

	comp->path.clear();
	EngineExternal->modulePathfinding->FindPath(EngineExternal->moduleMono->UnboxVector(startPos), EngineExternal->moduleMono->UnboxVector(endPos), comp->path);
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