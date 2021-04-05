#include "CO_Button.h"
#include "CO_Script.h"
#include "CO_Image2D.h"

#include "RE_Texture.h"

#include "GameObject.h"
#include "CO_AudioSource.h"

#include "Application.h"
#include "MO_ResourceManager.h"

#include "IM_FileSystem.h"

#include "ImGui/imgui.h"



C_Button::C_Button(GameObject* gameObject) :Component(gameObject), sprite_button_pressed(nullptr), sprite_button_hovered(nullptr), sprite_button_unhovered(nullptr), script_name(""),
num_sprite_used(BUTTONSTATE::BUTTONUNHOVERED), is_selected(false)
{
	name = "Button";
	thisAudSource = new C_AudioSource(gameObject);
#ifndef STANDALONE
	sprites_freezed = false;
#endif // !STANDALONE


}

C_Button::~C_Button()
{
	if (sprite_button_pressed != nullptr) {
		EngineExternal->moduleResources->UnloadResource(sprite_button_pressed->GetUID(), !LOADING_SCENE);
	}
	if (sprite_button_hovered != nullptr) {
		EngineExternal->moduleResources->UnloadResource(sprite_button_hovered->GetUID(), !LOADING_SCENE);
	}
	if (sprite_button_unhovered != nullptr) {
		EngineExternal->moduleResources->UnloadResource(sprite_button_unhovered->GetUID(), !LOADING_SCENE);
	}
	delete thisAudSource;
	thisAudSource = nullptr;
}

void C_Button::Update()
{
#ifndef STANDALONE
	if (script_name != "" && gameObject->GetComponent(Component::TYPE::SCRIPT, script_name.c_str()) == nullptr)
		script_name = "";
	if (sprites_freezed)
		return;
#endif // !STANDALONE

	switch (num_sprite_used)
	{
	case BUTTONSTATE::BUTTONHOVERED:
		if (!is_selected)
			ChangeTexture(BUTTONSTATE::BUTTONUNHOVERED);
		break;
	case BUTTONSTATE::BUTTONUNHOVERED:
		if (is_selected)
		{
			thisAudSource->SetEventName(std::string("Play_UI_Button_Hover"));
			thisAudSource->PlayEvent();
			ChangeTexture(BUTTONSTATE::BUTTONHOVERED);
		}
		break;
	}

}

void C_Button::ExecuteButton()
{
	thisAudSource->SetEventName(std::string("Play_UI_Button_Play"));
	thisAudSource->PlayEvent();
	ChangeTexture(BUTTONSTATE::BUTTONPRESSED);
	C_Script* script = static_cast<C_Script*>(gameObject->GetComponent(Component::TYPE::SCRIPT, script_name.c_str()));
	if (script != nullptr)
		script->ExecuteButton();
}

void C_Button::ReleaseButton()
{
	if (is_selected)
		ChangeTexture(BUTTONSTATE::BUTTONHOVERED);
	else
		ChangeTexture(BUTTONSTATE::BUTTONUNHOVERED);
}

void C_Button::ChangeTexture(BUTTONSTATE new_num_sprite)
{
	num_sprite_used = new_num_sprite;
	switch (new_num_sprite)
	{
	case BUTTONSTATE::BUTTONPRESSED:
	{
		if (sprite_button_pressed == nullptr)
		{
			return;
		}
		C_Image2D* img = static_cast<C_Image2D*>(gameObject->GetComponent(TYPE::IMAGE_2D));

		if (img != nullptr)
			img->SetTexture(sprite_button_pressed);
		break;
	}

	case BUTTONSTATE::BUTTONHOVERED:
	{
		if (sprite_button_hovered == nullptr) {
			return;
		}

		C_Image2D* img = static_cast<C_Image2D*>(gameObject->GetComponent(TYPE::IMAGE_2D));

		if (img != nullptr)
			img->SetTexture(sprite_button_hovered);
		break;
	}

	case BUTTONSTATE::BUTTONUNHOVERED:
	{
		if (sprite_button_unhovered == nullptr) {
			return;
		}

		C_Image2D* img = static_cast<C_Image2D*>(gameObject->GetComponent(TYPE::IMAGE_2D));

		if (img != nullptr)
			img->SetTexture(sprite_button_unhovered);
		break;
	}
	}
}



void C_Button::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	if (sprite_button_pressed != nullptr)
	{
		DEJson::WriteString(nObj, "Pressed_AssetsPath", sprite_button_pressed->GetAssetPath());
		DEJson::WriteString(nObj, "Pressed_LibraryPath", sprite_button_pressed->GetLibraryPath());
		DEJson::WriteInt(nObj, "Pressed_UID", sprite_button_pressed->GetUID());
	}
	if (sprite_button_hovered != nullptr)
	{
		DEJson::WriteString(nObj, "Hovered_AssetsPath", sprite_button_hovered->GetAssetPath());
		DEJson::WriteString(nObj, "Hovered_LibraryPath", sprite_button_hovered->GetLibraryPath());
		DEJson::WriteInt(nObj, "Hovered_UID", sprite_button_hovered->GetUID());
	}
	if (sprite_button_unhovered != nullptr)
	{
		DEJson::WriteString(nObj, "Unhovered_AssetsPath", sprite_button_unhovered->GetAssetPath());
		DEJson::WriteString(nObj, "Unhovered_LibraryPath", sprite_button_unhovered->GetLibraryPath());
		DEJson::WriteInt(nObj, "Unhovered_UID", sprite_button_unhovered->GetUID());
	}
	if (!script_name.empty())
	{
		DEJson::WriteString(nObj, "Script_Name", script_name.c_str());
	}
	DEJson::WriteInt(nObj, "ButtonState", static_cast<int>(num_sprite_used));
	DEJson::WriteBool(nObj, "Is Selected", is_selected);
}

void C_Button::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	std::string texName = nObj.ReadString("Pressed_LibraryPath");
	std::string assetsName = nObj.ReadString("Pressed_AssetsPath");

	if (texName != "") {
		sprite_button_pressed = static_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("Pressed_UID"), texName.c_str()));
		if (sprite_button_pressed == nullptr) {
			LOG(LogType::L_ERROR, "the sprite button pressed couldn't be created");
		}
		else {
			sprite_button_pressed->SetAssetsPath(assetsName.c_str());
		}
	}

	texName = nObj.ReadString("Hovered_LibraryPath");
	assetsName = nObj.ReadString("Hovered_AssetsPath");

	if (texName != "") {
		sprite_button_hovered = static_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("Hovered_UID"), texName.c_str()));
		if (sprite_button_hovered == nullptr) {
			LOG(LogType::L_ERROR, "the sprite button hovered couldn't be created");
		}
		else {
			sprite_button_hovered->SetAssetsPath(assetsName.c_str());
		}
	}


	texName = nObj.ReadString("Unhovered_LibraryPath");
	assetsName = nObj.ReadString("Unhovered_AssetsPath");

	if (texName != "") {
		sprite_button_unhovered = static_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("Unhovered_UID"), texName.c_str()));
		if (sprite_button_unhovered == nullptr) {
			LOG(LogType::L_ERROR, "the sprite button unhovered couldn't be created");
		}
		else {
			sprite_button_unhovered->SetAssetsPath(assetsName.c_str());
		}
	}

	texName = nObj.ReadString("Script_Name");

	if (texName != "")
		script_name = nObj.ReadString("Script_Name");

	num_sprite_used = static_cast<BUTTONSTATE>(nObj.ReadInt("ButtonState"));
	is_selected = nObj.ReadBool("Is Selected");
}


#ifndef STANDALONE

void C_Button::ChangeSprite(BUTTONSTATE num_sprite, ResourceTexture* sprite)
{
	switch (num_sprite)
	{
	case BUTTONSTATE::BUTTONPRESSED:
		if (sprite_button_pressed != nullptr) {
			EngineExternal->moduleResources->UnloadResource(sprite_button_pressed->GetUID());
		}
		sprite_button_pressed = sprite;
		break;
	case BUTTONSTATE::BUTTONHOVERED:
		if (sprite_button_hovered != nullptr) {
			EngineExternal->moduleResources->UnloadResource(sprite_button_hovered->GetUID());
		}
		sprite_button_hovered = sprite;
		break;
	case BUTTONSTATE::BUTTONUNHOVERED:
		if (sprite_button_unhovered != nullptr) {
			EngineExternal->moduleResources->UnloadResource(sprite_button_unhovered->GetUID());
		}
		sprite_button_unhovered = sprite;
		break;
	}
}


void C_Button::ChangeScript(const char* new_script_name)
{
	if (!script_name.empty() && gameObject != nullptr) {
		gameObject->RemoveComponent(gameObject->GetComponent(Component::TYPE::SCRIPT, script_name.c_str()));
	}
	dynamic_cast<C_Script*>(gameObject->AddComponent(TYPE::SCRIPT, new_script_name));

	script_name = new_script_name;
}

bool C_Button::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::Separator();
		ImGui::Checkbox("Freeze sprites", &sprites_freezed);

		if (sprite_button_pressed != nullptr) {
			ImGui::Text("%s", sprite_button_pressed->GetAssetPath());
		}
		ImGui::Columns(2);
		ImGui::Text("Drop here to change sprite 'P'");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_TEXTURE"))
			{
				std::string assetsPath = *(std::string*)payload->Data;
				std::string str_name = "";
				FileSystem::SplitFilePath(assetsPath.c_str(), &assetsPath, &str_name);
				assetsPath += str_name;

				ChangeSprite(BUTTONSTATE::BUTTONPRESSED, dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestFromAssets(assetsPath.c_str())));
			}
			ImGui::EndDragDropTarget();
		}
		ImGui::NextColumn();
		if (ImGui::Button("Edit Sprite 'P'")) {
			if (sprite_button_pressed == nullptr) {
				LOG(LogType::L_WARNING, "The sprite 'P' is nullptr");
			}
			else {
				ChangeTexture(BUTTONSTATE::BUTTONPRESSED);
			}
		}
		ImGui::Columns(1);
		ImGui::Separator();

		if (sprite_button_hovered != nullptr) {
			ImGui::Text("%s", sprite_button_hovered->GetAssetPath());
		}

		ImGui::Columns(2);

		ImGui::Text("Drop here to change sprite 'H'");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_TEXTURE"))
			{
				std::string assetsPath = *(std::string*)payload->Data;
				std::string str_name = "";
				FileSystem::SplitFilePath(assetsPath.c_str(), &assetsPath, &str_name);
				assetsPath += str_name;

				ChangeSprite(BUTTONSTATE::BUTTONHOVERED, dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestFromAssets(assetsPath.c_str())));
			}
			ImGui::EndDragDropTarget();
		}
		ImGui::NextColumn();

		if (ImGui::Button("Edit Sprite 'H'")) {
			if (sprite_button_hovered == nullptr) {
				LOG(LogType::L_WARNING, "The sprite 'H' is nullptr");
			}
			else {
				ChangeTexture(BUTTONSTATE::BUTTONHOVERED);
			}
		}
		ImGui::Columns(1);
		ImGui::Separator();

		if (sprite_button_unhovered != nullptr) {
			ImGui::Text("%s", sprite_button_unhovered->GetAssetPath());
		}

		ImGui::Columns(2);

		ImGui::Text("Drop here to change sprite 'U'");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_TEXTURE"))
			{
				std::string assetsPath = *(std::string*)payload->Data;
				std::string str_name = "";
				FileSystem::SplitFilePath(assetsPath.c_str(), &assetsPath, &str_name);
				assetsPath += str_name;

				ChangeSprite(BUTTONSTATE::BUTTONUNHOVERED, dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestFromAssets(assetsPath.c_str())));
			}
			ImGui::EndDragDropTarget();
		}
		ImGui::NextColumn();

		if (ImGui::Button("Edit Sprite 'U'")) {
			if (sprite_button_unhovered == nullptr) {
				LOG(LogType::L_WARNING, "The sprite 'U' is nullptr");
			}
			else {
				ChangeTexture(BUTTONSTATE::BUTTONUNHOVERED);
			}
		}
		ImGui::Columns(1);
		ImGui::Separator();

		ImGui::Text(script_name.c_str());

		ImGui::Text("Drop here to change the script");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_SCRIPT"))
			{
				std::string* assetsPath = (std::string*)payload->Data;
				std::string file_name;
				FileSystem::GetFileName(assetsPath->c_str(), file_name, false);

				ChangeScript(file_name.c_str());
			}
			ImGui::EndDragDropTarget();
		}


	}
	return true;
}

#endif // !STANDALONE