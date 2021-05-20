#pragma once
#include "Component.h"
#include <map>
#include"MathGeoLib/include/Geometry/AABB.h"
#include"MathGeoLib/include/Geometry/OBB.h"

class ResourceMesh;
class ResourceTexture;
class C_Transform;

class C_MeshRenderer : public Component
{
public:
	C_MeshRenderer(GameObject* _gm);
	virtual ~C_MeshRenderer();

	void Update() override;

	void RenderMesh(bool rTex = false);
	void RenderMeshStencil(bool rTex = false);

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

#ifndef STANDALONE
	bool OnEditor() override;
#endif // !STANDALONE

	void SetRootBone(GameObject* _rootBone);
	void SetRenderMesh(ResourceMesh* mesh);
	ResourceMesh* GetRenderMesh();
	float4x4 CalculateDeltaMatrix(float4x4 globalMat, float4x4 invertMat);
	void GetBoneMapping();
	void DrawDebugVertices();
	void TryCalculateBones();
	OBB globalOBB;
	AABB globalAABB;
	bool faceNormals, vertexNormals, showAABB, showOBB;

	GameObject* rootBone = nullptr;
	void SetStencilEmissionAmmount(float ammount);
	float GetStencilEmssionAmmount() const;

	bool GetDrawShadows()const;

public:
	bool drawStencil;
	float3 alternColor;
	float3 alternColorStencil;
private:
	ResourceMesh* _mesh = nullptr;
	ResourceTexture* normalMap = nullptr;
	ResourceTexture* specularMap = nullptr;

	float bumpDepth = 1.0f;

	bool drawDebugVertices;
	C_Transform* gameObjectTransform = nullptr;
	std::vector<C_Transform*> bonesMap;
	std::vector<float4x4> boneTransforms;
	bool calculatedBonesThisFrame;
	float stencilEmissionAmmount;
	bool drawShadows;
};