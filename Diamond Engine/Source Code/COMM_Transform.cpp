#include "COMM_Transform.h"

#include "Application.h"
#include "MO_Scene.h"

#include "GameObject.h"
#include "CO_Transform.h"

#include "MathGeoLib/include/Math/float4x4.h"

COMM_Transform::COMM_Transform(int agentUid, float* nextMat, float* previousMat) : Command(agentUid)
{
	for (int i = 0; i < 16; i++)
	{
		nextMatrix[i] = nextMat[i];
		previousMatrix[i] = previousMat[i];
	}
}


COMM_Transform::~COMM_Transform()
{
}


void COMM_Transform::Execute()
{
	float4x4 mat;
	mat.Set(nextMatrix);

	GameObject* agent = EngineExternal->moduleScene->GetGOFromUID(EngineExternal->moduleScene->root, agentUid);

	if (agent != nullptr)
		agent->transform->SetTransformWithGlobal(mat);

	else
		LOG(LogType::L_ERROR, "Couldn't redo transform, game object not found");
}


void COMM_Transform::Undo()
{
	float4x4 mat;
	mat.Set(previousMatrix);
	GameObject* agent = EngineExternal->moduleScene->GetGOFromUID(EngineExternal->moduleScene->root, agentUid);

	if (agent != nullptr)
		agent->transform->SetTransformWithGlobal(mat);

	else
		LOG(LogType::L_ERROR, "Couldn't undo transform, game object not found");
}