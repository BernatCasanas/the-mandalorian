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
	//ho faig fora? esq nose. q collons
	comp->targetPosition.clear();
	MonoClass* klass = mono_object_get_class(startPos);

	const char* name = mono_class_get_name(klass);
	comp->targetPosition.push_back(EngineExternal->modulePathfinding->FindRandomPointAround(EngineExternal->moduleMono->UnboxVector(startPos), radius));
}

void CS_CalculatePath(MonoObject* go, MonoObject* startPos, MonoObject* endPos)
{
	if (EngineExternal == nullptr || go == nullptr)
		return;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	if (comp == nullptr)
		return;

	comp->targetPosition.clear();
	EngineExternal->modulePathfinding->FindPath(EngineExternal->moduleMono->UnboxVector(startPos), EngineExternal->moduleMono->UnboxVector(endPos), comp->targetPosition);
}

MonoObject* CS_GetDestination(MonoObject* go)
{
	if (EngineExternal == nullptr || go == nullptr)
		return nullptr;

	C_NavMeshAgent* comp = DECS_CompToComp<C_NavMeshAgent*>(go);

	C_Transform* trans = DECS_CompToComp<C_Transform*>(go);

	if (comp == nullptr)
		return nullptr;

	float3 distance = comp->targetPosition[0] - trans->position;
	if (Sqrt(distance.x * distance.x + distance.y * distance.y + distance.z * distance.z) < comp->properties.stoppingDistance) {
		assert(!comp->targetPosition.empty());
		comp->targetPosition.erase(comp->targetPosition.begin());
	}
	return EngineExternal->moduleMono->Float3ToCS(comp->targetPosition.front());
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