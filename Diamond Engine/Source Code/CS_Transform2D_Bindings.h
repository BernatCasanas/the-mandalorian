#pragma once
#include "CO_Transform2D.h"

MonoObject* Get2Dpos(MonoObject* obj)
{
	if (EngineExternal == nullptr)
		return nullptr;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return nullptr;

	return EngineExternal->moduleMono->Float3ToCS(float3(trans2D->position[0], trans2D->position[1], 0.0f));

}

float Get2Drot(MonoObject* obj)
{
	if (EngineExternal == nullptr)
		return negInf;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return negInf;

	return trans2D->rotation;
}


MonoObject* Get2DlPos(MonoObject* obj)
{
	if (EngineExternal == nullptr)
		return nullptr;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return nullptr;

	return EngineExternal->moduleMono->Float3ToCS(float3(trans2D->localPos[0], trans2D->localPos[1], 0.0f));

}

float Get2DlRot(MonoObject* obj)
{
	if (EngineExternal == nullptr)
		return negInf;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return negInf;

	return trans2D->localRotation;
}

MonoObject* Get2Dsize(MonoObject* obj)
{
	if (EngineExternal == nullptr)
		return nullptr;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return nullptr;

	return EngineExternal->moduleMono->Float3ToCS(float3(trans2D->size[0], trans2D->size[1], 0.0f));

}

void Set2DlPos(MonoObject* obj, MonoObject* vec3)
{

	if (EngineExternal == nullptr)
		return;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return;

	float3 tempPos = EngineExternal->moduleMono->UnboxVector(vec3);

	trans2D->localPos[0] = tempPos.x;
	trans2D->localPos[1] = tempPos.y;

	trans2D->UpdateTransform();

}

void Set2DlRot(MonoObject* obj, float rot)
{

	if (EngineExternal == nullptr)
		return;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return;

	trans2D->localRotation = rot;
	trans2D->UpdateTransform();

}

void Set2DSize(MonoObject* obj, MonoObject* vec3)
{

	if (EngineExternal == nullptr)
		return;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return;

	float3 tempSize = EngineExternal->moduleMono->UnboxVector(vec3);

	trans2D->size[0] = tempSize.x;
	trans2D->size[1] = tempSize.y;

	trans2D->UpdateTransform();

}

void SetLocalTransform2D(MonoObject* obj, MonoObject* posVec3,float rot,MonoObject* sizeVec3)
{

	if (EngineExternal == nullptr)
		return;

	C_Transform2D* trans2D = DECS_CompToComp<C_Transform2D*>(obj);

	if (trans2D == nullptr)
		return;

	float3 tempPos = EngineExternal->moduleMono->UnboxVector(posVec3);

	float3 tempSize = EngineExternal->moduleMono->UnboxVector(sizeVec3);


	trans2D->SetTransform(tempPos.x, tempPos.y, rot, tempSize.x, tempSize.y);

	trans2D->UpdateTransform();

}