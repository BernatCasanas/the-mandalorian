#include "PE_Spawn_Sphere.h"
#include "Particle.h"
#include "Application.h"
#include "CO_Transform.h"
#include <math.h>
#include "ImGui/imgui.h"


#include "MO_Camera3D.h"

PE_SpawnShapeSphere::PE_SpawnShapeSphere() :PE_SpawnShapeBase(PE_SPAWN_SHAPE_TYPE::SPHERE)
{
	radius = 1.0f;
	angle = 0;
	useDirection = false;
}

PE_SpawnShapeSphere::~PE_SpawnShapeSphere()
{

}

void PE_SpawnShapeSphere::Spawn(Particle& particle, bool hasInitialSpeed, float speed, float4x4& gTrans, float* offset)
{
	//Get a random spawn point inside of a quad defined by a point and a radius
	float u = EngineExternal->GetRandomFloat(-radius, radius);
	float x1 = EngineExternal->GetRandomFloat(-radius, radius);
	float x2 = EngineExternal->GetRandomFloat(-radius, radius);
	float x3 = EngineExternal->GetRandomFloat(-radius, radius);

	float mag = sqrt(x1 * x1 + x2 * x2 + x3 * x3);
	x1 /= mag; x2 /= mag; x3 /= mag;
	float c = cbrt(u);
	float3 localPos;
	localPos.x = offset[0] + x1 * c;
	localPos.y = offset[1] + x2 * c;
	localPos.z = offset[2] + x3 * c;
	particle.pos = gTrans.TransformPos(localPos);
	
	if (hasInitialSpeed)
	{
		float3 localSpeed = (localPos - float3(offset[0], offset[1], offset[2])).Normalized() * speed;
		particle.speed = gTrans.TransformDir(localSpeed).Normalized() * speed;
		
		
		if (useDirection)
		{
			float3 direction = (localPos - float3(offset[0], offset[1], offset[2]));
			direction = gTrans.TransformDir(direction).Normalized();

			float4x4 cameraView = EngineExternal->moduleCamera->editorCamera.ViewMatrixOpenGL().Transposed();

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
void PE_SpawnShapeSphere::OnEditor(int emitterIndex)
{
	std::string suffixLabel = "Radius##PaShapeSphere";
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


void PE_SpawnShapeSphere::SaveData(JSON_Object* nObj)
{
	DEJson::WriteFloat(nObj, "PaShapeSphereRadius", radius);
	DEJson::WriteBool(nObj, "PaShapeSphereDirection", useDirection);
	DEJson::WriteFloat(nObj, "PaShapeSphereAngle", angle);

}


void PE_SpawnShapeSphere::LoadData(DEConfig& nObj)
{
	radius = nObj.ReadFloat("PaShapeSphereRadius");
	useDirection = nObj.ReadBool("PaShapeSphereDirection");
	angle = nObj.ReadFloat("PaShapeSphereAngle");

}