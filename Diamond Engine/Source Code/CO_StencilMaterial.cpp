#include "CO_StencilMaterial.h"
#include "ImGui/imgui.h"
#include "Application.h"

#include"MO_ResourceManager.h"
#include"MO_Renderer3D.h"
#include"MO_Scene.h"

#include"RE_Texture.h"
#include"RE_Shader.h"
#include"RE_Material.h"


C_StencilMaterial::C_StencilMaterial(GameObject* _gm) : Component(_gm), viewWithCheckers(false), matTexture(nullptr),
material(nullptr)
{
	name = "StencilMaterial";
	material = (EngineExternal->moduleScene->defaultMaterial != nullptr) ? dynamic_cast<ResourceMaterial*>(EngineExternal->moduleResources->RequestResource(EngineExternal->moduleScene->defaultMaterial->GetUID())) : NULL;
}

C_StencilMaterial::~C_StencilMaterial()
{
	if (matTexture != nullptr)
		EngineExternal->moduleResources->UnloadResource(matTexture->GetUID());

	if (material != nullptr)
		EngineExternal->moduleResources->UnloadResource(material->GetUID());
}

#ifndef STANDALONE
bool C_StencilMaterial::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::Separator();
		ImGui::Text("Current material: ");

		ImGui::Text("Drop here to change material");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload * payload = ImGui::AcceptDragDropPayload("_MATERIAL"))
			{
				std::string* assetsPath = (std::string*)payload->Data;

				ResourceMaterial* newMaterial = dynamic_cast<ResourceMaterial*>(EngineExternal->moduleResources->RequestFromAssets(assetsPath->c_str()));

				SetMaterial(newMaterial);
			}
			ImGui::EndDragDropTarget();
		}

		ImGui::SameLine();

		if (material == nullptr)
			ImGui::TextColored(ImVec4(1.0f, 1.0f, 0.0f, 1.0f), "No Material");
		else
			material->DrawEditor("##StencilMat");

		if (material && material->shader)
		{
			ImGui::Dummy(ImVec2(0, 15));
			ImGui::Text("Using shader: %s", material->shader->GetAssetPath());
		}

		return true;
	}
	return false;
}
#endif // !STANDALONE

void C_StencilMaterial::SetMaterial(ResourceMaterial* newMaterial)
{
	if (material != nullptr)
		EngineExternal->moduleResources->UnloadResource(material->GetUID());

	material = newMaterial;
}

int C_StencilMaterial::GetTextureID()
{
	return (viewWithCheckers == false && (matTexture && matTexture->textureID != 0)) ? matTexture->textureID : EngineExternal->moduleRenderer3D->checkersTexture;
	//return matTexture->textureID;
}

void C_StencilMaterial::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	DEJson::WriteBool(nObj, "IsEmpty", (matTexture == nullptr) ? true : false);
	if (matTexture != nullptr)
	{
		DEJson::WriteString(nObj, "AssetPath", matTexture->GetAssetPath());
		DEJson::WriteString(nObj, "LibraryPath", matTexture->GetLibraryPath());
		DEJson::WriteInt(nObj, "UID", matTexture->GetUID());
	}
	if (material != nullptr)
	{
		DEJson::WriteString(nObj, "MaterialAssetPath", material->GetAssetPath());
		DEJson::WriteString(nObj, "MaterialLibraryPath", material->GetLibraryPath());
		DEJson::WriteInt(nObj, "MaterialUID", material->GetUID());
	}
}

void C_StencilMaterial::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	//if (nObj.ReadBool("IsEmpty") == true)
	//	return;


	int w, h;
	w = h = 0;
	std::string texPath = nObj.ReadString("AssetPath");
	std::string texName = nObj.ReadString("LibraryPath");

	if (texName != "")
		matTexture = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("UID"), texName.c_str()));


	if (material != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(material->GetUID());
		material = nullptr;
	}

	if (nObj.ReadInt("MaterialUID") != 0)
		material = dynamic_cast<ResourceMaterial*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("MaterialUID"), Resource::Type::MATERIAL));
}