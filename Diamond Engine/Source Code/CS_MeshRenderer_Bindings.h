#pragma once

#include "CO_MeshRenderer.h"

bool GetDrawStencil(MonoObject* obj)
{
	bool ret = false;

	C_MeshRenderer* meshRenderer = DECS_CompToComp<C_MeshRenderer*>(obj);

	if (meshRenderer != nullptr)
	{
		ret = meshRenderer->drawStencil;
	}
	return ret;
}

void SetDrawStencil(MonoObject* obj, bool drawStencil)
{
	C_MeshRenderer* meshRenderer = DECS_CompToComp<C_MeshRenderer*>(obj);

	if (meshRenderer != nullptr)
	{
		meshRenderer->drawStencil = drawStencil;
	}
}

MonoObject* GetMeshColor(MonoObject* obj)
{
	C_MeshRenderer* meshRenderer = DECS_CompToComp<C_MeshRenderer*>(obj);

	float3 color = float3(1.0f, 1.0f, 1.0f);
	if (meshRenderer != nullptr)
	{
		color = meshRenderer->alternColor;
	}
	return EngineExternal->moduleMono->Float3ToCS(color);
}

void SetMeshColor(MonoObject* obj, MonoObject* color)
{
	C_MeshRenderer* meshRenderer = DECS_CompToComp<C_MeshRenderer*>(obj);

	if (meshRenderer != nullptr)
	{
		meshRenderer->alternColor = M_MonoManager::UnboxVector(color);
	}
}

MonoObject* GetMeshStencilColor(MonoObject* obj)
{
	C_MeshRenderer* meshRenderer = DECS_CompToComp<C_MeshRenderer*>(obj);

	float3 color = float3(1.0f, 1.0f, 1.0f);
	if (meshRenderer != nullptr)
	{
		color = meshRenderer->alternColorStencil;
	}
	return EngineExternal->moduleMono->Float3ToCS(color);
}

void SetMeshStencilColor(MonoObject* obj, MonoObject* color)
{
	C_MeshRenderer* meshRenderer = DECS_CompToComp<C_MeshRenderer*>(obj);

	if (meshRenderer != nullptr)
	{
		meshRenderer->alternColorStencil = M_MonoManager::UnboxVector(color);
	}
}
