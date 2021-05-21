#pragma once
#include <mono/metadata/object.h>
#include <mono/metadata/object-forward.h>

#include "Application.h"
#include "MO_MonoManager.h"

#include "GameObject.h"

#include "CO_AreaLight.h"

float GetAreaLightIntensity(MonoObject* goObj)
{
	C_AreaLight* light = DECS_CompToComp<C_AreaLight*>(goObj);

	if (light != nullptr)
		return light->GetLightIntensity();
}


void SetAreaLightIntensity(MonoObject* goObj, float lIntensity)
{
	C_AreaLight* light = DECS_CompToComp<C_AreaLight*>(goObj);

	if (light != nullptr)
		light->SetLightIntensity(lIntensity);
}


float GetAreaLightFadeDistance(MonoObject* goObj)
{
	C_AreaLight* light = DECS_CompToComp<C_AreaLight*>(goObj);

	if (light != nullptr)
		return light->GetFadeDistance();
}


void SetAreaLightFadeDistance(MonoObject* goObj, float fDistance)
{
	C_AreaLight* light = DECS_CompToComp<C_AreaLight*>(goObj);

	if (light != nullptr)
		light->SetFadeDistance(fDistance);
}


float GetAreaLightMaxDistance(MonoObject* goObj)
{
	C_AreaLight* light = DECS_CompToComp<C_AreaLight*>(goObj);

	if (light != nullptr)
		return light->GetMaxDistance();
}


void SetAreaLightMaxDistance(MonoObject* goObj, float mDistance)
{
	C_AreaLight* light = DECS_CompToComp<C_AreaLight*>(goObj);

	if (light != nullptr)
		light->SetMaxDistance(mDistance);
}