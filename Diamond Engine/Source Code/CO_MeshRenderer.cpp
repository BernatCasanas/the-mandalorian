#include "CO_MeshRenderer.h"
#include "RE_Mesh.h"

#include "RE_Material.h"
#include "RE_Shader.h"
#include "RE_Texture.h"

#include "Application.h"
#include "MO_Renderer3D.h"
#include "MO_ResourceManager.h"
#include "MO_Scene.h"

#include "IM_FileSystem.h"

#include "GameObject.h"
#include "CO_Material.h"
#include "CO_StencilMaterial.h"
#include "CO_Transform.h"
#include "CO_Camera.h"

#include "ImGui/imgui.h"

#include "OpenGL.h"

#include "DEJsonSupport.h"

#include "MathGeoLib/include/Geometry/Frustum.h"
#include "MathGeoLib/include/Geometry/Plane.h"


C_MeshRenderer::C_MeshRenderer(GameObject* _gm) : Component(_gm), _mesh(nullptr), normalMap(nullptr), specularMap(nullptr), bumpDepth(0.0f),
faceNormals(false), vertexNormals(false), showAABB(false), showOBB(false), drawDebugVertices(false), drawStencil(false),
calculatedBonesThisFrame(false), boneTransforms(), stencilEmissionAmmount(0.9f),drawShadows(true)
{
	name = "Mesh Renderer";
	alternColor = float3::one;
	alternColorStencil = float3::one;
	gameObjectTransform = dynamic_cast<C_Transform*>(gameObject->GetComponent(Component::TYPE::TRANSFORM));
}

C_MeshRenderer::~C_MeshRenderer()
{
	rootBone = nullptr;
	bonesMap.clear();
	boneTransforms.clear();

	if (_mesh != nullptr)
	{
		EngineExternal->moduleScene->totalTris -= _mesh->indices_count / 3;
		EngineExternal->moduleResources->UnloadResource(_mesh->GetUID());
		_mesh = nullptr;
	}

	if (normalMap != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(normalMap->GetUID());
		normalMap = nullptr;
	}

	if (specularMap != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(specularMap->GetUID());
		specularMap = nullptr;
	}

	gameObjectTransform = nullptr;
}

void C_MeshRenderer::Update()
{
	calculatedBonesThisFrame = false;
	boneTransforms.clear();

	if (drawStencil)
	{
		EngineExternal->moduleRenderer3D->renderQueueStencil.push_back(this);
		EngineExternal->moduleRenderer3D->renderQueuePostStencil.push_back(this);
	}
	else
	{
		EngineExternal->moduleRenderer3D->renderQueue.push_back(this);
	}


#ifndef STANDALONE
	if (showAABB == true)
	{
		float3 points[8];
		globalAABB.GetCornerPoints(points);
		ModuleRenderer3D::DrawBox(points, float3(0.2f, 1.f, 0.101f));
	}

	if (showOBB == true)
	{

		float3 points[8];
		globalOBB.GetCornerPoints(points);
		ModuleRenderer3D::DrawBox(points);
	}
#endif // !STANDALONE

}

void C_MeshRenderer::RenderMesh(bool rTex)
{
	if (_mesh == nullptr)
		return;

	C_Transform* transform = gameObject->transform;

	//TODO IMPORTANT: Optimize this, save this pointer or something
	C_Material* material = dynamic_cast<C_Material*>(gameObject->GetComponent(Component::TYPE::MATERIAL));
	GLuint id = 0;

	if (material != nullptr)
	{
		if (material->IsActive())
			id = material->GetTextureID();
	}
	/*else
	{
		material = dynamic_cast<C_Material*>(gameObject->AddComponent(Component::TYPE::MATERIAL));

		if (_mesh->HasVertexColors())
			material->SetMaterial(dynamic_cast<ResourceMaterial*>(EngineExternal->moduleResources->RequestResource(82053990, Resource::Type::MATERIAL)));
	}*/

	TryCalculateBones();

	if (drawDebugVertices)
		DrawDebugVertices();

	_mesh->RenderMesh(id, alternColor, rTex, (material && material->material != nullptr) ? material->material : EngineExternal->moduleScene->defaultMaterial, transform, normalMap, specularMap, bumpDepth, stencilEmissionAmmount);

	if (vertexNormals || faceNormals)
		_mesh->RenderMeshDebug(&vertexNormals, &faceNormals, transform->GetGlobalTransposed());

}

void C_MeshRenderer::RenderMeshStencil(bool rTex)
{
	if (_mesh == nullptr)
		return;

	C_Transform* transform = gameObject->transform;
	bool hasStencilMatActive = false;
	//TODO will take the normal material for the moment
	C_StencilMaterial* stencilMaterial = dynamic_cast<C_StencilMaterial*>(gameObject->GetComponent(Component::TYPE::STENCIL_MATERIAL));
	C_Material* material = dynamic_cast<C_Material*>(gameObject->GetComponent(Component::TYPE::MATERIAL));
	GLuint id = 0;

	if (stencilMaterial != nullptr && stencilMaterial->IsActive())
	{
		hasStencilMatActive = true;
		id = stencilMaterial->GetTextureID();
	}
	else if (material != nullptr && material->IsActive())
	{
		id = material->GetTextureID();
	}

	TryCalculateBones();

	if (hasStencilMatActive)
		_mesh->RenderMesh(id, alternColorStencil, rTex, (stencilMaterial && stencilMaterial->material != nullptr) ? stencilMaterial->material : EngineExternal->moduleScene->defaultMaterial, transform, nullptr, nullptr,1.0f, stencilEmissionAmmount);
	else
		_mesh->RenderMesh(id, alternColorStencil, rTex, (material && material->material != nullptr) ? material->material : EngineExternal->moduleScene->defaultMaterial, transform, nullptr, nullptr,1.0f, stencilEmissionAmmount);

}

void C_MeshRenderer::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	if (_mesh) //TODO: I don't think this is a good idea
	{
		DEJson::WriteString(nObj, "Path", _mesh->GetLibraryPath());
		DEJson::WriteInt(nObj, "UID", _mesh->GetUID());
	}

	if (normalMap != nullptr)
	{
		DEJson::WriteString(nObj, "NormalMapAssetPath", normalMap->GetAssetPath());
		DEJson::WriteString(nObj, "NormalMapLibraryPath", normalMap->GetLibraryPath());
		DEJson::WriteInt(nObj, "NormalMapUID", normalMap->GetUID());
	}

	if (specularMap != nullptr)
	{
		DEJson::WriteString(nObj, "SpecularMapAssetPath", specularMap->GetAssetPath());
		DEJson::WriteString(nObj, "SpecularMapLibraryPath", specularMap->GetLibraryPath());
		DEJson::WriteInt(nObj, "SpecularMapUID", specularMap->GetUID());
	}

	DEJson::WriteFloat(nObj, "bumpDepth", bumpDepth);
	DEJson::WriteVector3(nObj, "alternColor", &alternColor.x);
	DEJson::WriteVector3(nObj, "alternColorStencil", &alternColorStencil.x);
	DEJson::WriteFloat(nObj, "stencilEmission", stencilEmissionAmmount);
	bool doNotDrawStencil = !drawStencil; //We do that because Json defaults to true if doesn't find a certain property and we don't want every object to be drawn with stencil
	DEJson::WriteBool(nObj, "doNotDrawStencil", doNotDrawStencil);
	DEJson::WriteBool(nObj, "drawShadows", drawShadows);

}

void C_MeshRenderer::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	SetRenderMesh(dynamic_cast<ResourceMesh*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("UID"), nObj.ReadString("Path"))));

	alternColor = nObj.ReadVector3("alternColor");
	alternColorStencil = nObj.ReadVector3("alternColorStencil");
	SetStencilEmissionAmmount(nObj.ReadFloat("stencilEmission"));
	drawStencil = !nObj.ReadBool("doNotDrawStencil");
	if (_mesh == nullptr)
		return;

	_mesh->generalWireframe = &EngineExternal->moduleRenderer3D->wireframe;

	std::string texPath = nObj.ReadString("NormalMapAssetPath");
	std::string texName = nObj.ReadString("NormalMapLibraryPath");

	if (texName != "")
		normalMap = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("NormalMapUID"), texName.c_str()));

	texPath = nObj.ReadString("SpecularMapAssetPath");
	texName = nObj.ReadString("SpecularMapLibraryPath");

	if (texName != "")
		specularMap = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("SpecularMapUID"), texName.c_str()));

	bumpDepth = nObj.ReadFloat("bumpDepth");
	drawShadows = nObj.ReadBool("drawShadows");

	gameObject->transform->UpdateBoxes();
}

#ifndef STANDALONE
bool C_MeshRenderer::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::Separator();

		if (_mesh != nullptr)
		{
			//ImGui::Image((ImTextureID)_mesh->textureID, ImVec2(128, 128));
			ImGui::Text("Vertices: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%i", _mesh->vertices_count);
			ImGui::Text("Indices: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%i", _mesh->indices_count);

			ImGui::Spacing();
			ImGui::Text("Path: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%s", _mesh->GetLibraryPath());
		}

		ImGui::Button("Drop .mmh to change mesh", ImVec2(180, 40));
		//TODO: Maybe move this into a function?
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_MESH"))
			{
				std::string* libraryDrop = (std::string*)payload->Data;

				if (_mesh != nullptr) {
					EngineExternal->moduleScene->totalTris -= _mesh->indices_count / 3;
					EngineExternal->moduleResources->UnloadResource(_mesh->GetUID());
					_mesh = nullptr;
				}

				//TODO: Almost done come on TEMPORAL: This is the only way until fbx's .meta files
				//Store an array of meshes to reference
				std::string stID = "";
				FileSystem::GetFileName(libraryDrop->c_str(), stID, false);

				//ATOI is C++11 only, maybe not a good idea to use it
				int UID = std::atoi(stID.c_str());
				SetRenderMesh(dynamic_cast<ResourceMesh*>(EngineExternal->moduleResources->RequestResource(UID, libraryDrop->c_str())));
				LOG(LogType::L_WARNING, "Mesh %s changed", (*libraryDrop).c_str());
			}
			ImGui::EndDragDropTarget();
		}

		ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "Normal map");
		if (normalMap != nullptr)
			ImGui::Image((ImTextureID)normalMap->textureID, ImVec2(64, 64), ImVec2(0, 1), ImVec2(1, 0));

		else
			ImGui::Image((ImTextureID)EngineExternal->moduleRenderer3D->defaultNormalMap, ImVec2(64, 64), ImVec2(0, 1), ImVec2(1, 0));

		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_TEXTURE"))
			{
				//Drop asset from Asset window to scene window
				std::string* metaFileDrop = (std::string*)payload->Data;

				if (normalMap != nullptr)
					EngineExternal->moduleResources->UnloadResource(normalMap->GetUID());

				std::string libraryName = EngineExternal->moduleResources->LibraryFromMeta(metaFileDrop->c_str());

				normalMap = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(EngineExternal->moduleResources->GetMetaUID(metaFileDrop->c_str()), libraryName.c_str()));
				LOG(LogType::L_WARNING, "File %s loaded to scene", (*metaFileDrop).c_str());
			}
			ImGui::EndDragDropTarget();
		}

		if (normalMap != nullptr)
		{
			char name[32];
			sprintf_s(name, "Remove normal map: %d", normalMap->GetUID());
			if (ImGui::Button(name))
			{
				EngineExternal->moduleResources->UnloadResource(normalMap->GetUID());
				normalMap = nullptr;
			}
		}

		//Specular map
		ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "Specular map");
		if (specularMap != nullptr)
			ImGui::Image((ImTextureID)specularMap->textureID, ImVec2(64, 64), ImVec2(0, 1), ImVec2(1, 0));

		else
			ImGui::Image((ImTextureID)EngineExternal->moduleRenderer3D->defaultSpecularMap, ImVec2(64, 64), ImVec2(0, 1), ImVec2(1, 0));

		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_TEXTURE"))
			{
				//Drop asset from Asset window to scene window
				std::string* metaFileDrop = (std::string*)payload->Data;

				if (specularMap != nullptr)
					EngineExternal->moduleResources->UnloadResource(specularMap->GetUID());

				std::string libraryName = EngineExternal->moduleResources->LibraryFromMeta(metaFileDrop->c_str());

				specularMap = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(EngineExternal->moduleResources->GetMetaUID(metaFileDrop->c_str()), libraryName.c_str()));
				LOG(LogType::L_WARNING, "File %s loaded to scene", (*metaFileDrop).c_str());
			}
			ImGui::EndDragDropTarget();
		}

		if (specularMap != nullptr)
		{
			char name[32];
			sprintf_s(name, "Remove specular map: %d", specularMap->GetUID());
			if (ImGui::Button(name))
			{
				EngineExternal->moduleResources->UnloadResource(specularMap->GetUID());
				specularMap = nullptr;
			}
		}

		ImGui::NewLine();
		ImGui::DragFloat("Bump depth", &bumpDepth, 0.001f, -1, 1);
		ImGui::NewLine();
		ImGui::Separator();

		ImGui::Checkbox("Vertex Normals", &vertexNormals);
		ImGui::SameLine();
		ImGui::Checkbox("Show AABB", &showAABB);
		ImGui::Checkbox("Face Normals", &faceNormals);
		ImGui::SameLine();
		ImGui::Checkbox("Show OBB", &showOBB);
		ImGui::Checkbox("Draw Vertices", &drawDebugVertices);
		ImGui::Checkbox("Draw Stencil", &drawStencil);
		ImGui::DragFloat("Stencil Emission Ammount", &stencilEmissionAmmount, 0.01f, 0.0f, 1.0f);
		ImGui::ColorPicker3("No texture color: ", &alternColor.x);

		if (drawStencil)
			ImGui::ColorPicker3("No texture Stencil color: ", &alternColorStencil.x);

		ImGui::Checkbox("Draw Shadows", &drawShadows);

		return true;
	}
	return false;
}
#endif // !STANDALONE

void C_MeshRenderer::SetRootBone(GameObject* _rootBone)
{
	if (_rootBone == nullptr) {
		LOG(LogType::L_ERROR, "Trying to assign null root bone");
		return;
	}

	rootBone = _rootBone;

	//Get all the bones
	GetBoneMapping();

	//Set bone Transforms array size using original bones transform array size
	_mesh->boneTransforms.resize(_mesh->bonesOffsets.size());

	if (bonesMap.size() != _mesh->bonesMap.size())
	{
		for (size_t i = 0; i < _mesh->boneTransforms.size(); i++)
		{
			_mesh->boneTransforms[i] = float4x4::identity;
		}
	}
}

void C_MeshRenderer::SetRenderMesh(ResourceMesh* mesh)
{
	_mesh = mesh;

	if (mesh == nullptr)
		return;

	EngineExternal->moduleScene->totalTris += _mesh->indices_count / 3;

	globalOBB = _mesh->localAABB;
	globalOBB.Transform(gameObject->transform->globalTransform);

	// Generate global AABB
	globalAABB.SetNegativeInfinity();
	globalAABB.Enclose(globalOBB);

	_mesh->generalWireframe = &EngineExternal->moduleRenderer3D->wireframe;
}

ResourceMesh* C_MeshRenderer::GetRenderMesh()
{
	return _mesh;
}


float4x4 C_MeshRenderer::CalculateDeltaMatrix(float4x4 globalMat, float4x4 invertMat)
{
	return invertMat * globalMat;
}

void C_MeshRenderer::GetBoneMapping()
{
	std::vector<GameObject*> gameObjects;
	rootBone->CollectChilds(gameObjects);

	bonesMap.resize(_mesh->bonesMap.size());
	for (uint i = 0; i < gameObjects.size(); ++i)
	{
		std::map<std::string, uint>::iterator it = _mesh->bonesMap.find(gameObjects[i]->name);
		if (it != _mesh->bonesMap.end())
		{
			bonesMap[it->second] = dynamic_cast<C_Transform*>(gameObjects[i]->GetComponent(Component::TYPE::TRANSFORM));
		}
	}
}

void C_MeshRenderer::DrawDebugVertices()
{
	if (_mesh->boneTransforms.size() > 0)
	{
		for (uint v = 0; v < _mesh->vertices_count; ++v)
		{
			float3 vertex;
			vertex.x = _mesh->vertices[v * VERTEX_ATTRIBUTES];
			vertex.y = _mesh->vertices[v * VERTEX_ATTRIBUTES + 1];
			vertex.z = _mesh->vertices[v * VERTEX_ATTRIBUTES + 2];

			//For each set of 4 bones for bertex
			for (uint b = 0; b < 4; ++b)
			{
				//Get bone identificator and weights from arrays
				int bone_ID = _mesh->vertices[v * VERTEX_ATTRIBUTES + BONES_ID_OFFSET + b];
				float boneWeight = _mesh->vertices[v * VERTEX_ATTRIBUTES + WEIGHTS_OFFSET + b];

				//Meaning boneWeight will be 0
				if (bone_ID == -1)
					continue;

				//Transforming original mesh vertex with bone transformation matrix
				float3 vertexTransform;
				vertexTransform.x = _mesh->vertices[v * VERTEX_ATTRIBUTES];
				vertexTransform.y = _mesh->vertices[v * VERTEX_ATTRIBUTES + 1];
				vertexTransform.z = _mesh->vertices[v * VERTEX_ATTRIBUTES + 2];
				//float3 Inc = _mesh->boneTransforms[bone_ID].TransformPos(vertexTransform);

				float4 Inc = _mesh->boneTransforms[bone_ID].Transposed().Mul(float4(vertexTransform, 1.0));

				vertex.x += Inc.x * boneWeight;
				vertex.y += Inc.y * boneWeight;
				vertex.z += Inc.z * boneWeight;
			}

			glPushMatrix();
			glMultMatrixf(gameObject->transform->globalTransform.Transposed().ptr());
			glPointSize(4.0f);
			glBegin(GL_POINTS);
			glVertex3fv(vertex.ptr());
			glEnd();
			glPointSize(1.0f);
			glPopMatrix();
		}
	}
}

void C_MeshRenderer::TryCalculateBones()
{

	if (rootBone == nullptr)
		return;

	//Mesh array with transform matrix of each bone
	if (calculatedBonesThisFrame == false)
	{
		//float4x4 invertedMatrix = dynamic_cast<C_Transform*>(gameObject->GetComponent(Component::TYPE::TRANSFORM))->globalTransform.Inverted();
		float4x4 invertedMatrix = gameObjectTransform->globalTransform.Inverted();

		boneTransforms.reserve(_mesh->bonesMap.size());

		//Get each bone
		for (int i = 0; i < _mesh->bonesMap.size(); ++i)
		{
			C_Transform* bone = bonesMap[i];

			if (bone != nullptr)
			{
				//Calcule of Delta Matrix
				float4x4 Delta = CalculateDeltaMatrix(bone->globalTransform, invertedMatrix);
				Delta = Delta * _mesh->bonesOffsets[i];

				//Storage of Delta Matrix (Transformation applied to each bone)
				//_mesh->boneTransforms[i] = Delta.Transposed();
				boneTransforms.push_back(Delta.Transposed());
			}
		}
		calculatedBonesThisFrame = true;
	}

	if (_mesh->boneTransforms.size() != boneTransforms.size())
		return;

	for (int i = 0; i < _mesh->bonesMap.size(); ++i)
	{
		_mesh->boneTransforms[i] = boneTransforms[i];
	}

}

void C_MeshRenderer::SetStencilEmissionAmmount(float ammount)
{
	stencilEmissionAmmount = max(0.0f, ammount);
	stencilEmissionAmmount = min(stencilEmissionAmmount, 1.0f);
}

float C_MeshRenderer::GetStencilEmssionAmmount() const
{
	return stencilEmissionAmmount;
}

bool C_MeshRenderer::GetDrawShadows() const
{
	return drawShadows;
}
