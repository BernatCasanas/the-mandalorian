#pragma once

#include"mono/metadata/object.h"
#include"CO_Checkbox.h"
#include"CS_Transform_Bindings.h"


void CS_CheckboxChangeActive(MonoObject* comp, int checkboxActive)
{

	C_Checkbox* thisReference = DECS_CompToComp<C_Checkbox*>(comp);

	if (thisReference == nullptr)
		return;

	thisReference->SetAsActive(checkboxActive);
}
