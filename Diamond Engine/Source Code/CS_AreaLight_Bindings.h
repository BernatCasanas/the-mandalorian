#pragma once
#include "Application.h"
#include "CO_AreaLight.h"
#include "MO_MonoManager.h"
#include "GameObject.h"

#include "CS_Transform_Bindings.h"


void CS_ActivateLight(MonoObject* go)
{
	if (go == nullptr)
		return;

	C_AreaLight* areaLight = DECS_CompToComp<C_AreaLight*>(go);

	if (areaLight != nullptr)
		areaLight->ActivateLight();
}

void CS_DeactivateLight(MonoObject* go)
{
	if (go == nullptr)
		return;

	C_AreaLight* areaLight = DECS_CompToComp<C_AreaLight*>(go);

	if (areaLight != nullptr)
		areaLight->DeactivateLight();
}