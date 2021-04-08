#pragma once

#include "CO_Navigation.h"

void SelectNavigation(MonoObject* obj)
{
	if (EngineExternal == nullptr)
		return;




	C_Navigation* workNav = DECS_CompToComp<C_Navigation*>(obj);

	if (workNav == nullptr)
		return;

	
	workNav->Select();
}

void SetLeftNavButton(MonoObject* obj, MonoObject* other_obj)
{
	if (EngineExternal == nullptr)
		return;

	C_Navigation* workNav = DECS_CompToComp<C_Navigation*>(obj);

	GameObject* button_mapped = EngineExternal->moduleMono->GameObject_From_CSGO(other_obj);

	if (workNav == nullptr)
		return;


	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE,BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_LEFT,button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::BUTTON_DPAD_LEFT, button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_LEFT, button_mapped);
}

void SetRightNavButton(MonoObject* obj, MonoObject* other_obj)
{
	if (EngineExternal == nullptr)
		return;

	C_Navigation* workNav = DECS_CompToComp<C_Navigation*>(obj);

	GameObject* button_mapped = EngineExternal->moduleMono->GameObject_From_CSGO(other_obj);

	if (workNav == nullptr)
		return;


	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_RIGHT, button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::BUTTON_DPAD_RIGHT, button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_RIGHT, button_mapped);
}

void SetUpNavButton(MonoObject* obj, MonoObject* other_obj)
{
	if (EngineExternal == nullptr)
		return;

	C_Navigation* workNav = DECS_CompToComp<C_Navigation*>(obj);

	GameObject* button_mapped = EngineExternal->moduleMono->GameObject_From_CSGO(other_obj);

	if (workNav == nullptr)
		return;


	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_UP, button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::BUTTON_DPAD_UP, button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_UP, button_mapped);
}

void SetDownNavButton(MonoObject* obj, MonoObject* other_obj)
{
	if (EngineExternal == nullptr)
		return;

	C_Navigation* workNav = DECS_CompToComp<C_Navigation*>(obj);

	GameObject* button_mapped = EngineExternal->moduleMono->GameObject_From_CSGO(other_obj);

	if (workNav == nullptr)
		return;


	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_DOWN, button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::BUTTON_DPAD_DOWN, button_mapped);
	workNav->SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION::MOVE, BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_DOWN, button_mapped);
}