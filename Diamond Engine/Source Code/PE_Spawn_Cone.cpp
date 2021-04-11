#include "PE_Spawn_Cone.h"
#include "Particle.h"
#include "Application.h"
#include "CO_Transform.h"
#include <math.h>

#include "ImGui/imgui.h"
#include "MO_Camera3D.h"
#include"MO_Renderer3D.h"

PE_SpawnShapeCone::PE_SpawnShapeCone() :PE_SpawnShapeBase(PE_SPAWN_SHAPE_TYPE::CONE)
{
	height = 1.0f;
	radius = 1.0f;
	angle = 0;
	useDirection = false;
}

PE_SpawnShapeCone::~PE_SpawnShapeCone()
{

}

void PE_SpawnShapeCone::Spawn(Particle& particle, bool hasInitialSpeed, float speed, float4x4& gTrans, float* offset)
{
	//TODO: CHANGE SPAWN ALGORITHM TO NOT EAT THE PERFORMANCE FOR BREAKFAST. UPDATE: this is not that much of a performance hit

	float h = height * EngineExternal->GetRandomFloat(0.0f, 1.0f);
	
	float r = (radius / height) * h * sqrt(EngineExternal->GetRandomFloat(0.0f, 1.0f));

	float t = 2 * PI * EngineExternal->GetRandomFloat(0.0f, 1.0f);
	
	float3 localPos;
	localPos.x = (r * cos(t))+ offset[0];
	localPos.y = h + offset[1];
	localPos.z = (r * sin(t))+ offset[2];
	particle.pos = gTrans.TransformPos(localPos);

	if (hasInitialSpeed)
	{
		float3 localSpeed = (localPos - float3(offset[0], offset[1], offset[2])).Normalized() * speed;
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
void PE_SpawnShapeCone::OnEditor(int emitterIndex)
{
	std::string suffixLabel = "Cone Height##PaShapeCone"; //TODO consider putting 
	suffixLabel += emitterIndex;
	ImGui::DragFloat(suffixLabel.c_str(), &height);
	
	suffixLabel = "Cone Radius##PaShapeCone";
	suffixLabel += emitterIndex;
	ImGui::DragFloat(suffixLabel.c_str(), &radius);
	
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


void PE_SpawnShapeCone::SaveData(JSON_Object* nObj)
{
	DEJson::WriteFloat(nObj, "PaShapeConeRadius", radius);
	DEJson::WriteFloat(nObj, "PaShapeConeHeight", height);
	DEJson::WriteBool(nObj, "PaShapeSphereDirection", useDirection);
	DEJson::WriteFloat(nObj, "PaShapeSphereAngle", angle);
}


void PE_SpawnShapeCone::LoadData(DEConfig& nObj)
{
	radius = nObj.ReadFloat("PaShapeConeRadius");
	height = nObj.ReadFloat("PaShapeConeHeight");
	useDirection = nObj.ReadBool("PaShapeSphereDirection");
	angle = nObj.ReadFloat("PaShapeSphereAngle");
}