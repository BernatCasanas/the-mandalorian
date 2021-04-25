#pragma once

#include "CO_Material.h"
#include "RE_Material.h"

void SetFloatUniform(MonoObject* obj, MonoString* name_uniform, float value)
{
	if (EngineExternal == nullptr)
		return;

	


	C_Material* workMat = DECS_CompToComp<C_Material*>(obj);

	if (workMat == nullptr)
		return;
	char* text = mono_string_to_utf8(name_uniform);
	int i;
	bool uniform_found = false;
	for (i = 0; i < workMat->material->uniforms.size();i++) {
		if (strcmp(workMat->material->uniforms[i].name, text) == 0) {
			uniform_found = true;
			break;
		}
	}
	if (!uniform_found)
		return;
	workMat->material->uniforms[i].data.floatValue = value;

}

void SetIntUniform(MonoObject* obj, MonoString* name_uniform, int value)
{
	if (EngineExternal == nullptr)
		return;




	C_Material* workMat = DECS_CompToComp<C_Material*>(obj);

	if (workMat == nullptr)
		return;
	char* text = mono_string_to_utf8(name_uniform);
	int i;
	bool uniform_found = false;
	for (i = 0; i < workMat->material->uniforms.size(); i++) {
		if (strcmp(workMat->material->uniforms[i].name, text) == 0) {
			uniform_found = true;
			break;
		}
	}
	if (!uniform_found)
		return;
	workMat->material->uniforms[i].data.intValue = value;

}

void SetVectorUniform(MonoObject* obj, MonoString* name_uniform, MonoObject* objVector)
{
	if (EngineExternal == nullptr)
		return;




	C_Material* workMat = DECS_CompToComp<C_Material*>(obj);

	if (workMat == nullptr)
		return;
	char* text = mono_string_to_utf8(name_uniform);
	int i;
	bool uniform_found = false;
	for (i = 0; i < workMat->material->uniforms.size(); i++) {
		if (strcmp(workMat->material->uniforms[i].name, text) == 0) {
			uniform_found = true;
			break;
		}
	}
	if (!uniform_found)
		return;

	float3 newVector = M_MonoManager::UnboxVector(objVector);
	workMat->material->uniforms[i].data.vector3Value = newVector;

}