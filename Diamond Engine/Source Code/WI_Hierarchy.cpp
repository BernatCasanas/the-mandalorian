#ifndef STANDALONE

#include "WI_Hierarchy.h"

#include "Globals.h"
#include "GameObject.h"
#include "MO_Scene.h"
#include "MO_Editor.h"
#include "Application.h"
#include "WI_Inspector.h"
#include "MO_Input.h"
#include"AssetDir.h"

W_Hierarchy::W_Hierarchy(M_Scene* _scene) : Window(), cSceneReference(_scene), dropTarget(nullptr)
{
	name = "Hierarchy";
}

W_Hierarchy::~W_Hierarchy()
{
	cSceneReference = nullptr;
}


void W_Hierarchy::Draw()
{
	ImGui::PushStyleVar(ImGuiStyleVar_WindowPadding, ImVec2(0, 0));
	if (ImGui::Begin(name.c_str(), NULL /*| ImGuiWindowFlags_NoResize*/)) 
	{
		selected = ImGui::IsWindowFocused();

		ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, ImVec2(0, 0));
		ImGui::Columns(2, "HierarchyColumns", false);
		ImGui::SetColumnWidth(0, ImGui::GetWindowContentRegionMax().x - 30.0f);

		bool tree_open = false;
		ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags_DefaultOpen | ImGuiTreeNodeFlags_Selected;

		if (EngineExternal->moduleScene->current_scene_name[0] == '\0') {
			tree_open = ImGui::TreeNodeEx("untitled scene", flags);}
		else {
			tree_open = ImGui::TreeNodeEx(EngineExternal->moduleScene->current_scene_name, flags); }

		if (tree_open)
		{
			if (cSceneReference != nullptr && cSceneReference->root != nullptr)
			{
				DrawGameObjectsTree(cSceneReference->root, false);
			}
			ImGui::TreePop();
		}

		ImGui::NextColumn();

		ImGui::BeginChild("Empty space");
		ImGui::EndChild();
		if (ImGui::IsItemHovered())
		{
			ImGui::PushStyleVar(ImGuiStyleVar_WindowPadding, ImVec2(5, 5));
			ImGui::BeginTooltip();
			ImGui::Text("Drop an item here to unparent them");
			ImGui::EndTooltip();
			ImGui::PopStyleVar();
		}
		ImGui::PopStyleVar();

		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_GAMEOBJECT"))
			{
				dropTarget->ChangeParent(EngineExternal->moduleScene->root);
				LOG(LogType::L_NORMAL, "%s", dropTarget->name.c_str());
				dropTarget = nullptr;
			}
			ImGui::EndDragDropTarget();
		}

		ImGui::PushStyleVar(ImGuiStyleVar_WindowPadding, ImVec2(5, 5));

		if (ImGui::BeginPopupContextWindow())
		{
			EngineExternal->moduleEditor->DrawCreateMenu();
			if (ImGui::Selectable("Create Empty"))
			{
				GameObject* parent = (EngineExternal->moduleEditor->GetSelectedGO() != nullptr) ? EngineExternal->moduleEditor->GetSelectedGO() : EngineExternal->moduleScene->root;
				EngineExternal->moduleScene->CreateGameObject("Empty", parent);

				ImGui::CloseCurrentPopup();
			}
			ImGui::EndPopup();
		}
		ImGui::PopStyleVar();
	}
	ImGui::End();
	ImGui::PopStyleVar();
}

void W_Hierarchy::SetCurrentScene(M_Scene* _scene)
{
	cSceneReference = _scene;
}

void W_Hierarchy::DrawGameObjectsTree(GameObject* node, bool drawAsDisabled)
{

	if (drawAsDisabled == false)
		drawAsDisabled = !node->isActive();

	ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags_OpenOnArrow;// | ImGuiTreeNodeFlags_DefaultOpen;

	if (node->children.size() == 0)
		flags |= ImGuiTreeNodeFlags_Leaf | ImGuiTreeNodeFlags_NoTreePushOnOpen;

	//if (EngineExternal->moduleEditor->IsGOSelected(node))
	if (node == EngineExternal->moduleEditor->GetSelectedGO())
		flags |= ImGuiTreeNodeFlags_::ImGuiTreeNodeFlags_Selected;

	if(node->prefabReference != 0u)
		ImGui::PushStyleColor(ImGuiCol_Text, ImVec4(0.0f, 0.7f, 0.9f, 1.0f));
	else if (drawAsDisabled)
		ImGui::PushStyleColor(ImGuiCol_Text, ImGui::GetStyle().Colors[ImGuiCol_TextDisabled]);

	bool nodeOpen = ImGui::TreeNodeEx(node, flags, node->name.c_str());

	if (node->prefabReference != 0u || drawAsDisabled)
		ImGui::PopStyleColor();

	//Only can use if this is not the root node
	//ASK: Should the root node really be a gameobject? Problems with checks
	if (!node->IsRoot()) 
	{
		//Start drag for reparent
		if (ImGui::BeginDragDropSource(/*ImGuiDragDropFlags_SourceNoDisableHover*/))
		{
			ImGui::SetDragDropPayload("_GAMEOBJECT", &node->UID, sizeof(int*));

			dropTarget = node;

			ImGui::Text("Change parent to...");
			ImGui::EndDragDropSource();
		}

		if (ImGui::IsItemHovered() && ImGui::IsMouseReleased(ImGuiMouseButton_::ImGuiMouseButton_Left))
		{
			EngineExternal->moduleEditor->SetSelectedGO(node);
			/*if (EngineExternal->moduleInput->GetKey(SDL_SCANCODE_LSHIFT) == KEY_REPEAT)
				EngineExternal->moduleEditor->AddSelectedGameObject(node);
			else
				EngineExternal->moduleEditor->SetSelectedGO(node);*/
			
			if (EngineExternal->moduleEditor->GetSelectedAsset() != nullptr)
				EngineExternal->moduleEditor->SetSelectedAsset(nullptr);
		}
	}

	node->showChildren = (node->children.size() == 0) ? false : nodeOpen;
	
	//All nodes can be a drop target
	if (ImGui::BeginDragDropTarget())
	{
		if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_GAMEOBJECT")) 
		{
			//GameObject* dropGO = static_cast<GameObject*>(payload->Data);
			//memcpy(dropGO, payload->Data, payload->DataSize);
			dropTarget->ChangeParent(node);
			LOG(LogType::L_NORMAL, "%s", dropTarget->name.c_str());
			dropTarget = nullptr;
		}
		ImGui::EndDragDropTarget();
	}


	if (node->showChildren == true)
	{

		for (unsigned int i = 0; i < node->children.size(); i++)
		{
			DrawGameObjectsTree(node->children[i], drawAsDisabled);
		}
		ImGui::TreePop();
	}
}

#endif // !STANDALONE