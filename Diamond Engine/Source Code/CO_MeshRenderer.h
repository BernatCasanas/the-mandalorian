#pragma once
#include "Component.h"
#include <map>
#include"MathGeoLib/include/Geometry/AABB.h"
#include"MathGeoLib/include/Geometry/OBB.h"

class ResourceMesh;
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

	bool IsInsideFrustum(Frustum* camFrustum);

	void SetRootBone(GameObject* _rootBone);
	void SetRenderMesh(ResourceMesh* mesh);
	ResourceMesh* GetRenderMesh();
	float4x4 CalculateDeltaMatrix(float4x4 globalMat , float4x4 invertMat);
	void GetBoneMapping();
	void DrawDebugVertices();

	OBB globalOBB;
	AABB globalAABB;
	bool faceNormals, vertexNormals, showAABB, showOBB;

	GameObject* rootBone = nullptr;

private:
	ResourceMesh* _mesh;
	float3 alternColor;
	bool drawDebugVertices;
	C_Transform* gameObjectTransform;
	std::vector<C_Transform*> bonesMap;
	bool drawStencil;
};