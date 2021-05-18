#ifndef STANDALONE
#include "WI_EnvLightConfig.h"
#include"IM_TextureImporter.h"
#include"Application.h"
#include"MO_Renderer3D.h"

W_EnvLightConfig::W_EnvLightConfig()
{
	name = "Environmental Lights";

	for (size_t i = 0; i < 6; ++i)
	{
		cubemapPaths.push_back("\0");
		cubemapPaths[i].reserve(50);
	}
}

W_EnvLightConfig::~W_EnvLightConfig()
{
	for (size_t i = 0; i < cubemapPaths.size(); ++i)
	{
		cubemapPaths[i].clear();
	}
	cubemapPaths.clear();
}

void W_EnvLightConfig::Draw()
{
	if (ImGui::Begin(name.c_str(), NULL/*, ImGuiWindowFlags_::ImGuiWindowFlags_NoCollapse | ImGuiWindowFlags_NoResize*/))
	{

		for (size_t i = 0; i < cubemapPaths.size(); ++i)
		{
			std::string label = std::string("Path " + std::to_string(i) + ":");

			ImGui::Text(label.c_str());
			ImGui::SameLine();
			ImGui::InputText(std::string("##" + label).c_str(), &cubemapPaths[i][0], 50);

			label.clear();
		}

		ImGui::ColorPicker4("##skyboxPicker", &EngineExternal->moduleRenderer3D->clearColor[0]);

		if(ImGui::Button("Refresh"))
		{
			char* paths[6];

			for (size_t i = 0; i < 6; i++)
			{
				paths[i] = &cubemapPaths[i][0];
			}

			TextureImporter::LoadCubeMap(paths, EngineExternal->moduleRenderer3D->skybox);
		}
		ImGui::SameLine();
		if (ImGui::Button("Clear data"))
		{
			for (size_t i = 0; i < cubemapPaths.size(); ++i)
			{
				cubemapPaths[i].clear();
			}
			EngineExternal->moduleRenderer3D->skybox.ClearTextureMemory();
		}
	}

	ImGui::End();
}

void W_EnvLightConfig::SetPaths(char* loadedFaces[6])
{
	for (size_t i = 0; i < 6; i++)
	{
		cubemapPaths[i] = loadedFaces[i];
	}
}
void W_EnvLightConfig::SetPaths(std::vector<char*> loadedFaces)
{
	for (size_t i = 0; i < 6; i++)
	{
		cubemapPaths[i] = loadedFaces[i];
	}
}

void W_EnvLightConfig::ClearPaths()
{
	for (size_t i = 0; i < 6; i++)
	{
		cubemapPaths[i] = "\0";
	}
}

#endif // !STANDALONE