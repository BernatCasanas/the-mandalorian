#include "PE_Spawn_Area.h"
#include "Particle.h"
#include "Application.h"

#include "MathGeoLib/include/Math/float4x4.h"

#include "ImGui/imgui.h"
#include "MO_Camera3D.h"
#include"MO_Renderer3D.h"

PE_SpawnShapeArea::PE_SpawnShapeArea() :PE_SpawnShapeBase(PE_SPAWN_SHAPE_TYPE::AREA)
{
	memset(dimensions, 1.f, sizeof(dimensions));
	angle = 0;
	useDirection = false;
}

PE_SpawnShapeArea::~PE_SpawnShapeArea()
{

}

void PE_SpawnShapeArea::Spawn(Particle& particle, bool hasInitialSpeed, float speed, float4x4& gTrans, float* offset)
{

	//Get a random spawn point inside of a quad defined by a point and a radius

	float3 localPos;
	localPos.x = offset[0] + EngineExternal->GetRandomFloat(-dimensions[0], dimensions[0]);
	localPos.y = offset[1] + EngineExternal->GetRandomFloat(-dimensions[1], dimensions[1]);
	localPos.z = offset[2] + EngineExternal->GetRandomFloat(-dimensions[2], dimensions[2]);
	particle.pos = gTrans.TransformPos(localPos);

	if (hasInitialSpeed)
	{
		float3 localSpeed = (localPos - float3(offset[0], offset[1], offset[2])).Normalized();
		particle.speed = gTrans.TransformDir(localSpeed).Normalized() * speed;

		if (useDirection)
		{
			float3 direction = (localPos - float3(offset[0], offset[1], offset[2]));
			direction = gTrans.TransformDir(direction).Normalized();

#ifndef STANDALONE
			float4x4 cameraView = EngineExternal->moduleCamera->editorCamera.ViewMatrixOpenGL().Transposed();
#else
			float4x4 cameraView = EngineExternal->moduleRenderer3D->GetGameRenderTarget()->ViewMatrixOpenGL().Transposed();
#endif // !STANDALONE

			direction = cameraView.TransformDir(direction);

			float2 directionViewProj = float2(direction.x, direction.y).Normalized();
			float2 xAxis = float2(1, 0);
			float finalAngle = xAxis.AngleBetween(directionViewProj);
			if (directionViewProj.y < 0)
				finalAngle = 360 * DEGTORAD - finalAngle;
			finalAngle += angle * DEGTORAD;

			particle.rotation = finalAngle;
		}
	}


}

#ifndef STANDALONE
void PE_SpawnShapeArea::OnEditor(int emitterIndex)
{
	std::string suffixLabel = "Dimensions##PaShapeArea";
	suffixLabel += emitterIndex;
	ImGui::DragFloat3(suffixLabel.c_str(), dimensions);

	suffixLabel = "Face Direction##PaShapeCone";
	suffixLabel += emitterIndex;
	ImGui::Checkbox(suffixLabel.c_str(), &useDirection);

	if (useDirection)
	{
		suffixLabel = "Set Angle##PaShapeCone";
		suffixLabel += emitterIndex;
		ImGui::DragFloat(suffixLabel.c_str(), &angle);
	}
}
#endif // !STANDALONE


void PE_SpawnShapeArea::SaveData(JSON_Object* nObj)
{
	DEJson::WriteVector3(nObj, "PaShapeAreaDimensions", dimensions);
	DEJson::WriteBool(nObj, "PaShapeSphereDirection", useDirection);
	DEJson::WriteFloat(nObj, "PaShapeSphereAngle", angle);
}


void PE_SpawnShapeArea::LoadData(DEConfig& nObj)
{
	float3 newDimensions = nObj.ReadVector3("PaShapeAreaDimensions");
	dimensions[0] = newDimensions.x;
	dimensions[1] = newDimensions.y;
	dimensions[2] = newDimensions.z;

	useDirection = nObj.ReadBool("PaShapeSphereDirection");
	angle = nObj.ReadFloat("PaShapeSphereAngle");
}