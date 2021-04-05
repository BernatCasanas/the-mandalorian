#include "CO_NavMeshAgent.h"
#include "ImGui/imgui.h"
#include "Application.h"
#include "WI_Pathfinding.h"

C_NavMeshAgent::C_NavMeshAgent(GameObject* _gm) : Component(_gm)
{
	name = "Nav Mesh Agent";
}

C_NavMeshAgent::~C_NavMeshAgent()
{
	selectedNav = nullptr;
	path.clear();
}

bool C_NavMeshAgent::OnEditor()
{
	if (Component::OnEditor() == true) {

		ImGui::Separator();
		ImGui::Columns(2, NULL, false);
		ImGui::Spacing();
		ImGui::Text("Steering");
		ImGui::Spacing();
		ImGui::Spacing();
		ImGui::Text("Speed");
		ImGui::Spacing();
		ImGui::Text("Angular Speed");
		ImGui::Spacing();
		ImGui::Text("Stopping Distance");
		ImGui::Spacing();
		ImGui::NextColumn();

		char buffer[50];

		ImGui::Dummy({ 0,17 });
		sprintf_s(buffer, 50, "%.2f", properties.speed);
		if (ImGui::InputText("##speed", &buffer[0], sizeof(buffer)))
		{
			if (buffer[0] != '\0') {
				properties.speed = strtod(buffer, NULL);
			}
		}

		sprintf_s(buffer, 50, "%.2f", properties.angularSpeed);
		if (ImGui::InputText("##angularSpeed", &buffer[0], sizeof(buffer)))
		{
			if (buffer[0] != '\0') {
				properties.angularSpeed = strtod(buffer, NULL);
			}
		}

		sprintf_s(buffer, 50, "%.2f", properties.stoppingDistance);
		if (ImGui::InputText("##stoppingDistance", &buffer[0], sizeof(buffer)))
		{
			if (buffer[0] != '\0') {
				properties.stoppingDistance = strtod(buffer, NULL);
			}
		}


		ImGui::NextColumn();
	}

	
	return true;
}

void C_NavMeshAgent::SaveData(JSON_Object* nObj)
{
	DEJson::WriteInt(nObj, "Type", (int)type);
	DEJson::WriteBool(nObj, "Active", active);
	DEJson::WriteFloat(nObj, "Speed", properties.speed);
	DEJson::WriteFloat(nObj, "Angular Speed", properties.angularSpeed);
	DEJson::WriteFloat(nObj, "Stopping Distance", properties.stoppingDistance);
}

void C_NavMeshAgent::LoadData(DEConfig& nObj)
{
	active = nObj.ReadBool("Active");
	properties.speed = nObj.ReadFloat("Speed");
	properties.angularSpeed = nObj.ReadFloat("Angular Speed");
	properties.stoppingDistance = nObj.ReadFloat("Stopping Distance");
}
