#include "PE_Spawn_Circumference.h"
#include "Particle.h"
#include "Application.h"
#include "CO_Transform.h"
#include <math.h>
#include "ImGui/imgui.h"

#include"MO_Renderer3D.h"
#include "MO_Camera3D.h"

PE_SpawnShapeCircumference::PE_SpawnShapeCircumference() :PE_SpawnShapeBase(PE_SPAWN_SHAPE_TYPE::CIRCUMFERENCE)
{
	innerRadius = 1.0f;
	outterRadius = 1.0f;
	angle = 0;
	useDirection = false;
}

PE_SpawnShapeCircumference::~PE_SpawnShapeCircumference()
{

}

void PE_SpawnShapeCircumference::Spawn(Particle& particle, bool hasInitialSpeed, float speed, float4x4& gTrans, float* offset)
{
	//Get a random spawn point inside of a quad defined by a point and a innerRadius
	float r = EngineExternal->GetRandomFloat(innerRadius, outterRadius);
	float alpha = EngineExternal->GetRandomFloat(0.0f, 360.0f);
	float x = cosf(alpha);
	float y = sinf(alpha);

	float mag = sqrt(x * x + y * y);
	x /= mag; y /= mag;
	float3 localPos;
	localPos.x = offset[0] + x * r;
	localPos.y = offset[1];
	localPos.z = offset[2] + y * r;
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
void PE_SpawnShapeCircumference::OnEditor(int emitterIndex)
{
	std::string suffixLabel = "Radius##PaShapeCircumference";
	suffixLabel += emitterIndex;
	ImGui::DragFloatRange2("Inner and Outter", &innerRadius, &outterRadius, 0.2f, 0.01f, 100.0f, "Min: %.01f", "Max: %.01f");

	suffixLabel = "Face Direction##PaShapeCircumference";
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


void PE_SpawnShapeCircumference::SaveData(JSON_Object* nObj)
{
	DEJson::WriteFloat(nObj, "PaShapeCircumferenceinnerRadius", innerRadius);
	DEJson::WriteFloat(nObj, "PaShapeCircumferenceoutterRadius", outterRadius);
	DEJson::WriteBool(nObj, "PaShapeCircumferenceDirection", useDirection);
	DEJson::WriteFloat(nObj, "PaShapeCircumferenceAngle", angle);

}


void PE_SpawnShapeCircumference::LoadData(DEConfig& nObj)
{
	innerRadius = nObj.ReadFloat("PaShapeCircumferenceinnerRadius");
	outterRadius = nObj.ReadFloat("PaShapeCircumferenceoutterRadius");
	useDirection = nObj.ReadBool("PaShapeCircumferenceDirection");
	angle = nObj.ReadFloat("PaShapeCircumferenceAngle");

}