#pragma once

#include "PE__Spawn_Shape_Base.h"

class PE_SpawnShapeCircumference : public PE_SpawnShapeBase
{
public:
	PE_SpawnShapeCircumference();
	~PE_SpawnShapeCircumference() override;

	void Spawn(Particle& particle, bool hasInitialSpeed, float speed, float4x4& gTrans, float* offset) override; //Spawns in area

#ifndef STANDALONE
	void OnEditor(int emitterIndex) override;
#endif // !STANDALONE

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;
private:
	float innerRadius;
	float outterRadius;
	float angle;
	bool useDirection;
};