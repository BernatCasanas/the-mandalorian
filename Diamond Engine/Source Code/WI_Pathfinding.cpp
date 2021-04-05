#include "WI_Pathfinding.h"
#include "MO_Pathfinding.h"
#include "MO_Scene.h"
#include "Application.h"
#include "MO_Editor.h"
#include <stdlib.h>
#include "NavMeshBuilder.h"
#include "MO_Pathfinding.h"


W_Pathfinding::W_Pathfinding() : Window(), selectedNav(nullptr)
{
	name = "Navigation";
	selectedNav = EngineExternal->modulePathfinding->agents.begin()._Ptr;
}

W_Pathfinding::~W_Pathfinding()
{
	selectedNav = nullptr;
}

void W_Pathfinding::Draw()
{
	if (ImGui::Begin(name.c_str(), &active))
	{
		selected = ImGui::IsWindowFocused();
		DrawBakingTab();;
	}
	ImGui::End();
}

void W_Pathfinding::DrawBakingTab()
{
	char buffer[50];

	ImGui::Text("Agent Properties");

	ImGui::Spacing();
	ImGui::Separator();
	ImGui::Spacing();

	ImGui::Columns(2, NULL, FALSE);
	ImGui::Spacing();
	ImGui::Text("Radius");
	ImGui::Spacing();
	ImGui::Text("Height");
	ImGui::Spacing();
	ImGui::Text("Step Height");
	ImGui::Spacing();
	ImGui::Text("Max Slope");
	ImGui::Spacing();
	ImGui::NextColumn();

	sprintf_s(buffer, 50, "%.2f", EngineExternal->modulePathfinding->bakedNav.radius);
	if (ImGui::InputText("##Radius", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			EngineExternal->modulePathfinding->bakedNav.radius = strtod(buffer, NULL);
		}
	}
	sprintf_s(buffer, 50, "%.2f", EngineExternal->modulePathfinding->bakedNav.height);
	if (ImGui::InputText("##Height", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			EngineExternal->modulePathfinding->bakedNav.height = strtod(buffer, NULL);
		}
	}
	sprintf_s(buffer, 50, "%.2f", EngineExternal->modulePathfinding->bakedNav.stopHeight);
	if (ImGui::InputText("##StopHeight", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			EngineExternal->modulePathfinding->bakedNav.stopHeight = strtod(buffer, NULL);
		}
	}
	sprintf_s(buffer, 50, "%d", EngineExternal->modulePathfinding->bakedNav.maxSlope);
	if (ImGui::InputText("##Slope", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			EngineExternal->modulePathfinding->bakedNav.maxSlope = strtod(buffer, NULL);
		}
	}

	ImGui::Columns(1);

	ImGui::Dummy({ 0,10 });

	if (ImGui::Button("Calculate"))
	{
		EngineExternal->modulePathfinding->BakeNavMesh();
	}

	ImGui::SameLine();
	if (ImGui::Button("Clear"))
	{
		EngineExternal->modulePathfinding->ClearNavMeshes();
	}

	/*ImGui::Text("Input Mesh");
	ImGui::Button("Drop mesh here");
	if (ImGui::BeginDragDropTarget())
	{
		if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_GAMEOBJECT"))
		{
			int uid = *(int*)payload->Data;

			GameObject* droppedGO = EngineExternal->moduleScene->GetGOFromUID(EngineExternal->moduleScene->root, uid);
			EngineExternal->modulePathfinding->AddGameObjectToNavMesh(droppedGO);
		}
		ImGui::EndDragDropTarget();
	}

	ImGui::Text("Current NavMesh");
	*/

	NavMeshBuilder* navMeshBuilder = EngineExternal->modulePathfinding->GetNavMeshBuilder();
	if (navMeshBuilder != nullptr)
	{
		navMeshBuilder->OnEditor();
	}

	/*if(ImGui::Button("Create Walkability Test"))
	{
		EngineExternal->modulePathfinding->CreateWalkabilityTestPoint();
	}*/

	ImGui::Text("Path Type");

	if (ImGui::RadioButton("Smooth Path", EngineExternal->modulePathfinding->pathfinder.pathType == PathType::SMOOTH))
		EngineExternal->modulePathfinding->pathfinder.pathType = PathType::SMOOTH;
	
	ImGui::SameLine();
	if(ImGui::RadioButton("Straight Path", EngineExternal->modulePathfinding->pathfinder.pathType == PathType::STRAIGHT))
		EngineExternal->modulePathfinding->pathfinder.pathType = PathType::STRAIGHT;

}

