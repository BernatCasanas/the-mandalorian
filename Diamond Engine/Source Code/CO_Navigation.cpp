#include "CO_Navigation.h"

#include "Application.h"
#include "MO_GUI.h"
#include "MO_Scene.h"
#include "MO_Input.h"
#include "MO_Editor.h"

#include "GameObject.h"
#include "CO_Button.h"
#include "CO_Checkbox.h"

#include "ImGui/imgui.h"

#include "DETime.h"

#include <assert.h>

C_Navigation::C_Navigation(GameObject* gameObject, Component::TYPE type_of_ui) :Component(gameObject), is_selected(false), button_or_joystick_being_used(BUTTONSANDJOYSTICKS::NO_BUTTON_OR_JOYSTICK),
type_of_ui(type_of_ui)
{
	map_of_buttons_and_joysticks.clear();
	map_of_updated_uints.clear();
	name = "Navigation";
	assert(static_cast<int>(BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_UP) == 15, "Make sure RIGHT_JOYSTICK_UP is the first joystick input in the enum");

}

C_Navigation::~C_Navigation()
{
	std::vector<int>::iterator it = std::find(EngineExternal->moduleGui->uid_gameobject_of_ui_selected.begin(), EngineExternal->moduleGui->uid_gameobject_of_ui_selected.end(), gameObject->UID);
	if (it != EngineExternal->moduleGui->uid_gameobject_of_ui_selected.end())
		EngineExternal->moduleGui->uid_gameobject_of_ui_selected.erase(it);
	map_of_buttons_and_joysticks.clear();
	map_of_updated_uints.clear();
}

void C_Navigation::Enable()
{
	button_or_joystick_being_used = BUTTONSANDJOYSTICKS::BUTTON_OR_JOYSTICK_UNKNOWN;
}

void C_Navigation::Disable()
{
	switch (type_of_ui) {
	case Component::TYPE::BUTTON: {
		static_cast<C_Button*>(gameObject->GetComponent(Component::TYPE::BUTTON))->ReleaseButton();
		break;
	}
	case Component::TYPE::CHECKBOX: {
		static_cast<C_Checkbox*>(gameObject->GetComponent(Component::TYPE::CHECKBOX))->UnpressCheckbox();
		break;
	}
	}
}


void C_Navigation::Update()
{
	if (map_of_updated_uints.size() > 0) {
		for (std::map<BUTTONSANDJOYSTICKS, ActionToRealize>::iterator it = map_of_buttons_and_joysticks.begin(); it != map_of_buttons_and_joysticks.end(); ++it) {
			if (map_of_updated_uints.count(it->second.referenceGO->UID)) {
				it->second.referenceGO = EngineExternal->moduleScene->GetGOFromUID(EngineExternal->moduleScene->root, map_of_updated_uints[it->second.referenceGO->UID]);
			}
		}
		map_of_updated_uints.clear();
	}
	if (!is_selected || map_of_buttons_and_joysticks.size() == 0 || DETime::state == GameState::STOP)
		return;
	KEY_STATE state;
	bool button_active_found = false;
	for (std::map<BUTTONSANDJOYSTICKS, ActionToRealize>::iterator it = map_of_buttons_and_joysticks.begin(); it != map_of_buttons_and_joysticks.end(); ++it) {
		if (it->second.referenceGO == nullptr || it->second.referenceGO->GetComponent(Component::TYPE::NAVIGATION) == nullptr)
			continue;

		if (button_active_found && it->first < BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_UP)
			continue;

		CheckIfButtonOrJoystickIsBeingUsed(it->first, state, it->second.is_key_down, it->second.is_key_up);

		if (button_active_found)
			continue;


		if (button_or_joystick_being_used == BUTTONSANDJOYSTICKS::BUTTON_OR_JOYSTICK_UNKNOWN && (state == KEY_STATE::KEY_DOWN || state == KEY_STATE::KEY_REPEAT)) {
			button_or_joystick_being_used = it->first;
			it->second.is_key_down = true;
			it->second.is_key_up = false;
			button_active_found = true;
			continue;
		}
		else if (button_or_joystick_being_used == it->first && state == KEY_STATE::KEY_UP) {
			button_or_joystick_being_used = BUTTONSANDJOYSTICKS::NO_BUTTON_OR_JOYSTICK;
			DoTheAction(it->second.referenceGO, it->second.action, true);
			button_active_found = true;
			continue;
		}
		else if (button_or_joystick_being_used == BUTTONSANDJOYSTICKS::NO_BUTTON_OR_JOYSTICK && state == KEY_STATE::KEY_DOWN) {
			button_or_joystick_being_used = it->first;
			DoTheAction(it->second.referenceGO, it->second.action, false);
			button_active_found = true;
			continue;
		}

	}
	if (button_or_joystick_being_used == BUTTONSANDJOYSTICKS::BUTTON_OR_JOYSTICK_UNKNOWN) {
		Disable();
		button_or_joystick_being_used = BUTTONSANDJOYSTICKS::NO_BUTTON_OR_JOYSTICK;
	}
}

void C_Navigation::CheckIfButtonOrJoystickIsBeingUsed(BUTTONSANDJOYSTICKS button_or_joystick_to_check, KEY_STATE& state, bool& is_key_down, bool& is_key_up)
{
	switch (button_or_joystick_to_check)
	{
	case BUTTONSANDJOYSTICKS::BUTTON_A:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_A);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_B:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_B);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_X:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_X);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_Y:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_Y);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_BACK:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_BACK);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_GUIDE:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_GUIDE);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_START:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_START);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_LEFTTTRIGGER:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_LEFTSTICK);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_RIGHTTRIGGER:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_RIGHTSTICK);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_LEFTSHOULDER:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_LEFTSHOULDER);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_RIGHTSHOULDER:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_RIGHTSHOULDER);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_DPAD_UP:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_DPAD_UP);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_DPAD_DOWN:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_DPAD_DOWN);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_DPAD_LEFT:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_DPAD_LEFT);
		break;
	case BUTTONSANDJOYSTICKS::BUTTON_DPAD_RIGHT:
		state = EngineExternal->moduleInput->GetGamePadKey(SDL_CONTROLLER_BUTTON_DPAD_RIGHT);
		break;
	case BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_UP:
		if (EngineExternal->moduleInput->GetRightAxisY() >= -30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	case BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_DOWN:
		if (EngineExternal->moduleInput->GetRightAxisY() <= 30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	case BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_LEFT:
		if (EngineExternal->moduleInput->GetRightAxisX() >= -30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	case BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_RIGHT:
		if (EngineExternal->moduleInput->GetRightAxisX() <= 30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	case BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_UP:
		if (EngineExternal->moduleInput->GetLeftAxisY() >= -30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	case BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_DOWN:
		if (EngineExternal->moduleInput->GetLeftAxisY() <= 30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	case BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_LEFT:
		if (EngineExternal->moduleInput->GetLeftAxisX() >= -30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	case BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_RIGHT:
		if (EngineExternal->moduleInput->GetLeftAxisX() <= 30000) {
			if (is_key_down) {
				state = KEY_UP;
				is_key_down = false;
				is_key_up = true;
			}
			else {
				state = KEY_IDLE;
			}
		}
		else {
			if (is_key_up) {
				state = KEY_DOWN;
				is_key_down = true;
				is_key_up = false;
			}
			else {
				state = KEY_REPEAT;
			}
		}
		break;
	}
}

void C_Navigation::DoTheAction(GameObject* gameobject_passed, ACTIONSNAVIGATION action, bool is_key_released)
{
	switch (action)
	{
	case ACTIONSNAVIGATION::MOVE: {
		if (is_key_released)
			return;

		C_Navigation* nav = static_cast<C_Navigation*>(gameobject_passed->GetComponent(Component::TYPE::NAVIGATION));

		nav->Select();

		nav->button_or_joystick_being_used = button_or_joystick_being_used;

		bool joystick = button_or_joystick_being_used >= BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_UP;
		button_or_joystick_being_used = BUTTONSANDJOYSTICKS::NO_BUTTON_OR_JOYSTICK;


		if (nav->map_of_buttons_and_joysticks.count(nav->button_or_joystick_being_used) == 0) {
			nav->button_or_joystick_being_used = BUTTONSANDJOYSTICKS::NO_BUTTON_OR_JOYSTICK;
			return;
		}
		if (!joystick)
			return;

		nav->map_of_buttons_and_joysticks[nav->button_or_joystick_being_used].is_key_down = true;
		nav->map_of_buttons_and_joysticks[nav->button_or_joystick_being_used].is_key_up = false;
		break;
	}

	case ACTIONSNAVIGATION::EXECUTE:
		switch (static_cast<C_Navigation*>(gameobject_passed->GetComponent(Component::TYPE::NAVIGATION))->type_of_ui) {
		case Component::TYPE::BUTTON: {
			C_Button* button = static_cast<C_Button*>(gameobject_passed->GetComponent(Component::TYPE::BUTTON));
			if (is_key_released) {
				button->ReleaseButton();
				return;
			}
			button->ExecuteButton();
			break;
		}
		case Component::TYPE::CHECKBOX: {
			C_Checkbox* checkbox = static_cast<C_Checkbox*>(gameobject_passed->GetComponent(Component::TYPE::CHECKBOX));
			if (is_key_released) {
				checkbox->UnpressCheckbox();
				return;
			}
			checkbox->PressCheckbox();
			break;
		}
		}
		break;
	}
}

void C_Navigation::OnRecursiveUIDChange(std::map<uint, GameObject*> gameObjects)
{
	for (std::map<uint, GameObject*>::const_iterator it = gameObjects.begin(); it != gameObjects.end(); ++it) {
		map_of_updated_uints.emplace(it->first, it->second->UID);
	}
}

void C_Navigation::Select()
{
	if (EngineExternal->moduleGui->uid_gameobject_of_ui_selected.size() != 0) {
		C_Navigation* aux;
		for (std::vector<GameObject*>::const_iterator it = gameObject->parent->children.begin(); it != gameObject->parent->children.end(); ++it)
		{
			aux = static_cast<C_Navigation*>((*it)->GetComponent(Component::TYPE::NAVIGATION));
			if (aux != nullptr && aux->is_selected)
			{
				aux->Deselect();
			}
		}
	}
	EngineExternal->moduleGui->uid_gameobject_of_ui_selected.push_back(gameObject->UID);
	is_selected = true;

	switch (type_of_ui) {
	case Component::TYPE::BUTTON:
		static_cast<C_Button*>(gameObject->GetComponent(Component::TYPE::BUTTON))->is_selected = true;
		break;
	case Component::TYPE::CHECKBOX:
		static_cast<C_Checkbox*>(gameObject->GetComponent(Component::TYPE::CHECKBOX))->is_selected = true;
		break;
	}
}

void C_Navigation::Deselect()
{

	std::vector<int>::iterator it = std::find(EngineExternal->moduleGui->uid_gameobject_of_ui_selected.begin(), EngineExternal->moduleGui->uid_gameobject_of_ui_selected.end(), gameObject->UID);
	if (it != EngineExternal->moduleGui->uid_gameobject_of_ui_selected.end())
		EngineExternal->moduleGui->uid_gameobject_of_ui_selected.erase(it);

	is_selected = false;

	switch (type_of_ui) {
	case Component::TYPE::BUTTON: {
		C_Button* button = static_cast<C_Button*>(gameObject->GetComponent(Component::TYPE::BUTTON));
		button->is_selected = false;
		button->ReleaseButton();
		break;
	}
	case Component::TYPE::CHECKBOX: {
		C_Checkbox* checkbox = static_cast<C_Checkbox*>(gameObject->GetComponent(Component::TYPE::CHECKBOX));
		checkbox->is_selected = false;
		checkbox->UnpressCheckbox();
		break;
	}
	}
}


void C_Navigation::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	JSON_Value* goArray = json_value_init_array();
	JSON_Array* jsArray = json_value_get_array(goArray);
	for (std::map<BUTTONSANDJOYSTICKS, ActionToRealize>::const_iterator it = map_of_buttons_and_joysticks.begin(); it != map_of_buttons_and_joysticks.end(); ++it)
	{
		JSON_Value* nVal = json_value_init_object();
		JSON_Object* nObj_map = json_value_get_object(nVal);
		SaveMapData(nObj_map, it->second, it->first);
		json_array_append_value(jsArray, nVal);
	}
	json_object_set_value(nObj, "Map", goArray);
	DEJson::WriteInt(nObj, "Button Being Used", static_cast<int>(button_or_joystick_being_used));
	DEJson::WriteInt(nObj, "Type Of UI", static_cast<int>(type_of_ui));
	DEJson::WriteBool(nObj, "Is Selected", is_selected);
}

void C_Navigation::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);



	for (size_t i = 0; i < json_array_get_count(json_object_get_array(nObj.nObj, "Map")); ++i)
	{
		DEConfig conf(nullptr);

		conf.nObj = json_array_get_object(json_object_get_array(nObj.nObj, "Map"), i);

		LoadMapaData(conf);

	}

	button_or_joystick_being_used = static_cast<BUTTONSANDJOYSTICKS>(nObj.ReadInt("Button Being Used"));
	is_selected = nObj.ReadBool("Is Selected");
	if (is_selected)
		EngineExternal->moduleGui->uid_gameobject_of_ui_selected.push_back(gameObject->UID);
}

void C_Navigation::SaveMapData(JSON_Object* nObj, const ActionToRealize& action, BUTTONSANDJOYSTICKS map_index)
{
	DEJson::WriteInt(nObj, "Index", static_cast<int>(map_index));
	DEJson::WriteInt(nObj, "UID", (action.referenceGO != nullptr) ? action.referenceGO->UID : 0);
	DEJson::WriteInt(nObj, "Action", static_cast<int>(action.action));
	DEJson::WriteBool(nObj, "Is Key Down", action.is_key_down);
	DEJson::WriteBool(nObj, "Is Key Up", action.is_key_up);

}

void C_Navigation::LoadMapaData(DEConfig& nObj)
{
	ActionToRealize new_action;
	new_action.action = static_cast<ACTIONSNAVIGATION>(nObj.ReadInt("Action"));


	new_action.is_key_down = nObj.ReadBool("Is Key Down");
	new_action.is_key_up = nObj.ReadBool("Is Key Up");
	map_of_buttons_and_joysticks.emplace(static_cast<BUTTONSANDJOYSTICKS>(nObj.ReadInt("Index")), new_action);
	EngineExternal->moduleScene->navigationReferenceMap.emplace(nObj.ReadInt("UID"), &map_of_buttons_and_joysticks[static_cast<BUTTONSANDJOYSTICKS>(nObj.ReadInt("Index"))]);
}

void C_Navigation::SetButtonOrJoystickWithGameobject(ACTIONSNAVIGATION action_to_do, BUTTONSANDJOYSTICKS button_or_joystick, GameObject* gameobject)
{
	if (map_of_buttons_and_joysticks.count(button_or_joystick) != 0) {
		map_of_buttons_and_joysticks[button_or_joystick].action = action_to_do;
		map_of_buttons_and_joysticks[button_or_joystick].referenceGO = gameobject;
	}
	else {
		ActionToRealize action;
		action.action = action_to_do;
		action.referenceGO = gameobject;
		map_of_buttons_and_joysticks.emplace(button_or_joystick, action);
	}

}



#ifndef STANDALONE
bool C_Navigation::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::Separator();


		if (ImGui::Checkbox("Is this UI component selected?", &is_selected)) {
			if (is_selected) {
				Select();
			}
			else
				Deselect();
		}
		ImGui::Columns(3);

		WriteButtonOrJoystickOnEditor("Button A:", BUTTONSANDJOYSTICKS::BUTTON_A);

		WriteButtonOrJoystickOnEditor("Button B:", BUTTONSANDJOYSTICKS::BUTTON_B);

		WriteButtonOrJoystickOnEditor("Button X:", BUTTONSANDJOYSTICKS::BUTTON_X);

		WriteButtonOrJoystickOnEditor("Button Y:", BUTTONSANDJOYSTICKS::BUTTON_Y);

		WriteButtonOrJoystickOnEditor("Button Back:", BUTTONSANDJOYSTICKS::BUTTON_BACK);

		WriteButtonOrJoystickOnEditor("Button Home:", BUTTONSANDJOYSTICKS::BUTTON_GUIDE);

		WriteButtonOrJoystickOnEditor("Button Start:", BUTTONSANDJOYSTICKS::BUTTON_START);

		WriteButtonOrJoystickOnEditor("Button Left Trigger:", BUTTONSANDJOYSTICKS::BUTTON_LEFTTTRIGGER);

		WriteButtonOrJoystickOnEditor("Button Right Trigger:", BUTTONSANDJOYSTICKS::BUTTON_RIGHTTRIGGER);

		WriteButtonOrJoystickOnEditor("Button Left Shoulder:", BUTTONSANDJOYSTICKS::BUTTON_LEFTSHOULDER);

		WriteButtonOrJoystickOnEditor("Button Right Shoulder:", BUTTONSANDJOYSTICKS::BUTTON_RIGHTSHOULDER);

		WriteButtonOrJoystickOnEditor("DPad Up:", BUTTONSANDJOYSTICKS::BUTTON_DPAD_UP);

		WriteButtonOrJoystickOnEditor("DPad Down:", BUTTONSANDJOYSTICKS::BUTTON_DPAD_DOWN);

		WriteButtonOrJoystickOnEditor("DPad Left:", BUTTONSANDJOYSTICKS::BUTTON_DPAD_LEFT);

		WriteButtonOrJoystickOnEditor("DPad Right:", BUTTONSANDJOYSTICKS::BUTTON_DPAD_RIGHT);

		WriteButtonOrJoystickOnEditor("Right Joystick Up:", BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_UP);

		WriteButtonOrJoystickOnEditor("Right Joystick Down:", BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_DOWN);

		WriteButtonOrJoystickOnEditor("Right Joystick Left:", BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_LEFT);

		WriteButtonOrJoystickOnEditor("Right Joystick Right:", BUTTONSANDJOYSTICKS::RIGHT_JOYSTICK_RIGHT);

		WriteButtonOrJoystickOnEditor("Left Joystick Up:", BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_UP);

		WriteButtonOrJoystickOnEditor("Left Joystick Down:", BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_DOWN);

		WriteButtonOrJoystickOnEditor("Left Joystick Left:", BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_LEFT);

		WriteButtonOrJoystickOnEditor("Left Joystick Right:", BUTTONSANDJOYSTICKS::LEFT_JOYSTICK_RIGHT);

		ImGui::Columns(1);

	}

	return true;
}

void C_Navigation::WriteButtonOrJoystickOnEditor(const char* text, BUTTONSANDJOYSTICKS button_or_joystick)
{
	ImGui::Text(text);
	ImGui::NextColumn();
	std::string text_string = "##";
	text_string += static_cast<int>(button_or_joystick);
	assert(static_cast<int>(ACTIONSNAVIGATION::TOTAL_ACTIONS_WITH_NONE) == 3, "Update the references of ACTIONSNAVIGATION");
	const char* items[] = { "None", "Move", "Execute" };
	const char* current_item = items[0];
	if (map_of_buttons_and_joysticks.count(button_or_joystick) != 0) {
		current_item = items[static_cast<int>(map_of_buttons_and_joysticks[button_or_joystick].action)];
	}
	if (ImGui::BeginCombo(text_string.c_str(), current_item))
	{
		for (int n = 0; n < IM_ARRAYSIZE(items); n++)
		{
			bool is_selected = (current_item == items[n]);
			if (ImGui::Selectable(items[n], is_selected)) {
				current_item = items[n];
				if (map_of_buttons_and_joysticks.count(button_or_joystick) != 0) {
					if (n == 0) {
						map_of_buttons_and_joysticks.erase(button_or_joystick);
					}
					else
						map_of_buttons_and_joysticks[button_or_joystick].action = static_cast<ACTIONSNAVIGATION>(n);
				}
				else {
					ActionToRealize action;
					action.action = static_cast<ACTIONSNAVIGATION>(n);
					map_of_buttons_and_joysticks.emplace(button_or_joystick, action);
				}
			}
			if (is_selected)
				ImGui::SetItemDefaultFocus();
		}
		ImGui::EndCombo();
	}
	ImGui::NextColumn();
	if (current_item != items[0]) {
		ImGui::Text("UID:");
		ImGui::SameLine();

		GameObject* ref = map_of_buttons_and_joysticks[button_or_joystick].referenceGO;
		ImGui::Text((ref != nullptr) ? ref->name.c_str() : "Empty");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_GAMEOBJECT"))
			{
				map_of_buttons_and_joysticks[button_or_joystick].referenceGO = EngineExternal->moduleEditor->GetDraggingGO();
			}
			ImGui::EndDragDropTarget();
		}
	}
	ImGui::NextColumn();
}


#endif // !STANDALONE

ActionToRealize::ActionToRealize() :action(ACTIONSNAVIGATION::NONE), referenceGO(nullptr), is_key_down(false), is_key_up(true)
{
}

ActionToRealize::~ActionToRealize()
{
}
