#pragma once
#include "Component.h"
#include "MO_Physics.h"

#include "physx/include/PxPhysicsAPI.h"

class GameObject;
class C_Transform;
class C_RigidBody;
class C_MeshRenderer;

class C_CapsuleCollider : public C_Collider
{
public:

	C_CapsuleCollider();
	C_CapsuleCollider(GameObject* _gm/*, float3 _position, Quat _rotation, float3 _localScale*/);
	virtual ~C_CapsuleCollider();

	void Update() override;



	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;
#ifndef STANDALONE
	bool OnEditor()override;

#endif // !STANDALONE
public:
	float radius;
	float halfHeight;


};