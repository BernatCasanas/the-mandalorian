#include "WI_Pathfinding.h"
#include "MO_Pathfinding.h"
#include "MO_Scene.h"
#include "Application.h"
#include "MO_Editor.h"
#include <stdlib.h>
#include "NavMeshBuilder.h"
#include "MO_Pathfinding.h"


W_Pathfinding::W_Pathfinding() : Window(), showAgents(true), showBuild(false), selectedNav(nullptr)
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
		ImGui::Columns(3, NULL, false);
		ImGui::NextColumn();
		if (ImGui::Button("Agents")) {
			showAgents = true;
			showBuild = false;
		}
		ImGui::SameLine();
		if (ImGui::Button("Bake")) {
			showBuild = true;
			showAgents = false;
		}
		ImGui::NextColumn();
		ImGui::NextColumn();
		ImGui::Dummy({ 0,20 });

		if (showAgents)
		{
			DrawAgentsTab();
		}
		if (showBuild)
		{
			DrawBakingTab();
		}

		ImGui::Separator();
	}
	ImGui::End();
}

void W_Pathfinding::DrawAgentsTab()
{
	if (ImGui::BeginCombo("##agent", selectedNav->name.c_str()))
	{
		for (int t = 0; t < EngineExternal->modulePathfinding->agents.size(); t++)
		{
			bool is_selected = strcmp(selectedNav->name.c_str(), EngineExternal->modulePathfinding->agents[t].name.c_str()) == 0;
			if (ImGui::Selectable(EngineExternal->modulePathfinding->agents[t].name.c_str(), is_selected)) {
				selectedNav = &EngineExternal->modulePathfinding->agents[t];
			}

			if (is_selected)
				ImGui::SetItemDefaultFocus();
		}
		if (ImGui::BeginMenu("Add Agent"))
		{
			static char agentName[32];
			ImGui::InputText("", agentName, IM_ARRAYSIZE(agentName));

			if (ImGui::Button("Save Agent")) {
				NavAgent newAgent;
				newAgent.name = agentName;
				EngineExternal->modulePathfinding->agents.push_back(newAgent);
				agentName[0] = '\0';
				selectedNav = &EngineExternal->modulePathfinding->agents.back();
			}
			ImGui::EndMenu();
		}

		int agent_to_remove = -1;
		if (ImGui::BeginMenu("Remove Agent"))
		{
			for (int t = 0; t < EngineExternal->modulePathfinding->agents.size(); t++)
			{
				if (ImGui::Selectable(EngineExternal->modulePathfinding->agents[t].name.c_str(), false)) {
					agent_to_remove = t;

				}
			}
			ImGui::EndMenu();
		}

		if (agent_to_remove != -1)
			EngineExternal->modulePathfinding->agents.erase(EngineExternal->modulePathfinding->agents.begin() + agent_to_remove);

		ImGui::EndCombo();
	}
	ImGui::Dummy({ 0,50 });

	char buffer[50];

	ImGui::Columns(2, NULL, FALSE);
	ImGui::Spacing();
	ImGui::Text("Name");
	ImGui::Spacing();
	ImGui::Text("Radius");
	ImGui::Spacing();
	ImGui::Text("Height");
	ImGui::Spacing();
	ImGui::Text("Step Height");
	ImGui::Spacing();
	ImGui::Text("Max Slop");
	ImGui::Spacing();
	ImGui::NextColumn();

	strcpy(buffer, selectedNav->name.c_str());
	if (ImGui::InputText("##Agent", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0')
			selectedNav->name = buffer;
	}
	sprintf_s(buffer, 50, "%.2f", selectedNav->radius);
	if (ImGui::InputText("##Radius", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			selectedNav->radius = strtod(buffer, NULL);
		}
	}
	sprintf_s(buffer, 50, "%.2f", selectedNav->height);
	if (ImGui::InputText("##Height", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			selectedNav->height = strtod(buffer, NULL);
		}
	}
	sprintf_s(buffer, 50, "%.2f", selectedNav->stopHeight);
	if (ImGui::InputText("##StopHeight", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			selectedNav->stopHeight = strtod(buffer, NULL);
		}
	}
	sprintf_s(buffer, 50, "%d", selectedNav->maxSlope);
	if (ImGui::InputText("##Slope", &buffer[0], sizeof(buffer)))
	{
		if (buffer[0] != '\0') {
			selectedNav->maxSlope = strtod(buffer, NULL);
		}
	}

	ImGui::NextColumn(); 
}

void W_Pathfinding::DrawBakingTab()
{
	ImGui::Dummy({ 0,50 });

	char buffer[50];

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


	ImGui::Separator();

	ImGui::Dummy({ 0, 5 });

	ImGui::Text("Input Mesh");
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

	NavMeshBuilder* navMeshBuilder = EngineExternal->modulePathfinding->GetNavMeshBuilder();
	if (navMeshBuilder != nullptr)
	{
		navMeshBuilder->OnEditor();
	}

	if(ImGui::Button("Create Walkability Test"))
	{
		EngineExternal->modulePathfinding->CreateWalkabilityTestPoint();
	}

	ImGui::Separator();

	ImGui::Text("Path Type");

	if (ImGui::RadioButton("Smooth Path", EngineExternal->modulePathfinding->pathfinder.pathType == PathType::SMOOTH))
		EngineExternal->modulePathfinding->pathfinder.pathType = PathType::SMOOTH;
	
	ImGui::SameLine();
	if(ImGui::RadioButton("Straight Path", EngineExternal->modulePathfinding->pathfinder.pathType == PathType::STRAIGHT))
		EngineExternal->modulePathfinding->pathfinder.pathType = PathType::STRAIGHT;

}

