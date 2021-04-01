#include "CO_Canvas.h"

#include "GameObject.h"

#include "Application.h"
#include "MO_GUI.h"

#include "ImGui/imgui.h"

C_Canvas::C_Canvas(GameObject* gameObject) : Component(gameObject)
{
	name = "Canvas";
	EngineExternal->moduleGui->SetCanvas(gameObject->UID);
}

C_Canvas::~C_Canvas()
{
	EngineExternal->moduleGui->EraseCanvas();
}

void C_Canvas::OnRecursiveUIDChange(std::map<uint, GameObject*> gameObjects)
{
	EngineExternal->moduleGui->EraseCanvas();
	EngineExternal->moduleGui->SetCanvas(gameObject->UID);
}

#ifndef STANDALONE
bool C_Canvas::OnEditor()
{
	if (Component::OnEditor() == true)
		ImGui::Separator();

	return true;
}
#endif // !STANDALONE