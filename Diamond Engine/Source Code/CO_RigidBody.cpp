#include"DETime.h"

#include "CO_RigidBody.h"
#include "CO_Collider.h"
#include "CO_Transform.h"
#include "CO_MeshRenderer.h"

#include "MO_Physics.h"
#include "Globals.h"
#include <vector>
#include"GameObject.h"
#include"Application.h"
#include"MO_Editor.h"


C_RigidBody::C_RigidBody() : Component(nullptr)
{


	name = "Rigidbody";
}


C_RigidBody::C_RigidBody(GameObject* _gm): Component(_gm)

{
	goTransform = dynamic_cast<C_Transform*>(_gm->GetComponent(Component::TYPE::TRANSFORM));
	mesh = dynamic_cast<C_MeshRenderer*>(_gm->GetComponent(Component::TYPE::MESH_RENDERER));
	collider_info = _gm->GetComponentsOfType(Component::TYPE::BOXCOLLIDER);

	std::vector<Component*> meshCollider_info;
	std::vector<Component*> sphereCollider_info;

	meshCollider_info = _gm->GetComponentsOfType(Component::TYPE::MESHCOLLIDER);
	sphereCollider_info = _gm->GetComponentsOfType(Component::TYPE::SPHERECOLLIDER);

	for (int i = 0; i < meshCollider_info.size(); i++)
	{
		collider_info.push_back(meshCollider_info[i]);
	}
	for (int i = 0; i < sphereCollider_info.size(); i++)
	{
		collider_info.push_back(sphereCollider_info[i]);
	}
	/*for (int i = 0; i < collider_info.size(); i++)
	{
		C_Collider* collider = dynamic_cast<C_Collider*>(collider_info[i]);
		collider->rigidbody = this;
	}*/


	//if(!collider_info)
	//	collider_info = dynamic_cast<C_Collider*>(_gm->GetComponent(Component::TYPE::MESHCOLLIDER));


	Quat rot;
	float3 pos, scale, objectpos, pivotpos;
	goTransform->globalTransform.Decompose(pos, rot, scale);
	objectpos = pos;
	if(mesh != nullptr)
	pos = mesh->globalOBB.pos;
	pivotpos = pos;
	offset = pivotpos - objectpos;


	

	rigid_dynamic = EngineExternal->modulePhysics->CreateRigidDynamic(pos, rot);

	float4x4 worldtrans = goTransform->globalTransform;
	float4x4 pivotrans = float4x4::FromTRS(pos, rot, scale);	
	//	pivotrans = global_to_pivot * worldtrans;
	global_to_pivot =  worldtrans.Inverted() * pivotrans;

	for(int i = 0; i < collider_info.size(); i++)
	{
		C_Collider* colliderComponent = dynamic_cast<C_Collider*>(collider_info[i]);
		rigid_dynamic->attachShape(*colliderComponent->colliderShape);
		colliderComponent->rigidbody = this;
	}
	/*else
	{
		EngineExternal->modulePhysics->CreateMeshCollider(rigid_dynamic, this->gameObject);
	}*/

	name = "Rigidbody";

	use_gravity = true;
	use_kinematic = false;

	lock_linearX = false, lock_linearY = false, lock_linearZ = false;
	lock_angularX = false, lock_angularY = false, lock_angularZ = false;

	mass = 10.0f;
	density = 1.0f;

	force = float3::zero;
	torque = float3::zero;
	linear_vel = float3::zero;
	angular_vel = float3::zero;

	linear_damping = 0.0f;
	angular_damping = 0.0f;

	EnableGravity(use_gravity);
	EnableKinematic(use_kinematic);

	SetMass(mass);
	SetDensity(density);

	AddForce(force);
	AddTorque(torque);

	SetLinearVelocity(linear_vel);
	SetAngularVelocity(angular_vel);

	SetLinearDamping(linear_damping);
	SetAngularDamping(angular_damping);

	rigid_dynamic->setRigidDynamicLockFlags(physx::PxRigidDynamicLockFlag::eLOCK_ANGULAR_Z | physx::PxRigidDynamicLockFlag::eLOCK_ANGULAR_X | physx::PxRigidDynamicLockFlag::eLOCK_ANGULAR_Y);

	rigid_dynamic->userData = this->gameObject;

	rigid_dynamic->setActorFlag(physx::PxActorFlag::eVISUALIZATION, true);

	LockLinearX(lock_linearX);
	LockLinearY(lock_linearY);
	LockLinearZ(lock_linearZ);
	LockAngularX(lock_angularX);
	LockAngularY(lock_angularY);
	LockAngularY(lock_angularZ);

}

C_RigidBody::~C_RigidBody()
{
	//LOG(LogType::L_NORMAL, "Deleting Rigidbody");
	for (int i = 0; i < collider_info.size(); i++)
	{
		C_Collider* colliderComponent = dynamic_cast<C_Collider*>(collider_info[i]);
		colliderComponent->rigidbody = nullptr;
	}

	
	//rigid_dynamic->getGlobalPose();
	EngineExternal->modulePhysics->ReleaseActor((physx::PxRigidActor*)rigid_dynamic);
	rigid_dynamic->userData = nullptr;
}

void C_RigidBody::PostUpdate()
{
	//Just update transform if we have rigidbody simulation
	//if (App->timeManager->started) {
	
	
		
		Quat rot;
		float3 pos, scale;	
		goTransform->globalTransform.Decompose(pos, rot, scale);

		if (DETime::state == GameState::PLAY)
		{
			for (size_t i = 0; i < pos.Size; i++)
			{
				pos[i] = Round(pos[i] * 100) / 100;
			}
		}
		pos += offset;
		physx::PxQuat rotation = { rot.x,  rot.y, rot.z, rot.w };
		rigid_dynamic->setGlobalPose(physx::PxTransform({ pos.x, pos.y, pos.z }, rotation));
			

		
		

		
	
		
	/*}
	else {
		if (collider_info != nullptr)
			rigid_dynamic->setGlobalPose(PxTransform({ collider_info->transform->position.x, collider_info->transform->position.y, collider_info->transform->position.z }));
		else
			goTransform->SetPosition({ rigid_dynamic->getGlobalPose().p.x, rigid_dynamic->getGlobalPose().p.y, rigid_dynamic->getGlobalPose().p.z });
	}*/

	//TODO: MOVE RIGID BODY IF GLOBAL POSITION CHANGED
}

void C_RigidBody::Step()
{
	if (DETime::state == GameState::PLAY)
	{
		float4x4 worldtrans = goTransform->globalTransform;

		float3 pos, scale;
		Quat rot;
		worldtrans.Decompose(pos, rot, scale);
		pos = { rigid_dynamic->getGlobalPose().p.x, rigid_dynamic->getGlobalPose().p.y, rigid_dynamic->getGlobalPose().p.z };
		rot = { rigid_dynamic->getGlobalPose().q.x, rigid_dynamic->getGlobalPose().q.y, rigid_dynamic->getGlobalPose().q.z,  rigid_dynamic->getGlobalPose().q.w };


		if (DETime::state == GameState::PLAY)
		{
			for (size_t i = 0; i < pos.Size; i++)
			{
				pos[i] = Round(pos[i] * 100) / 100;
			}
		}

		worldtrans = float4x4::FromTRS(pos, rot, scale);
			
		C_Collider* colliderComponent = nullptr;
			
		if (!collider_info.empty())
			C_Collider* colliderComponent = dynamic_cast<C_Collider*>(collider_info[0]);

		if (colliderComponent && colliderComponent->shape == ColliderShape::MESH)
		{
			goTransform->SetTransformWithGlobal(worldtrans);
		}
		else
		{
			float4x4 pivotrans = global_to_pivot.Inverted() * worldtrans;

			worldtrans = float4x4::FromTRS(pos - offset, rot, scale);
			goTransform->SetTransformWithGlobal(worldtrans);
		}
				
		

		



	}
}
void C_RigidBody::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	DEJson::WriteBool(nObj, "kinematic", use_kinematic);
	DEJson::WriteBool(nObj, "Gravity", use_gravity);
	DEJson::WriteBool(nObj, "LockLinearX", lock_linearX);
	DEJson::WriteBool(nObj, "LockLinearY", lock_linearY);
	DEJson::WriteBool(nObj, "LockLinearZ", lock_linearZ);
	DEJson::WriteBool(nObj, "LockAngularX", lock_angularX);
	DEJson::WriteBool(nObj, "LockAngularY", lock_angularY);
	DEJson::WriteBool(nObj, "LockAngularZ", lock_angularZ);
	DEJson::WriteFloat(nObj, "Mass", mass);
	DEJson::WriteFloat(nObj, "Density", density);

}

void C_RigidBody::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	/*if (nObj.ReadBool("IsEmpty") == true)
		return;*/

	bool temp = use_kinematic;
	use_kinematic = nObj.ReadBool("kinematic");
	if (temp != use_kinematic)
		EnableKinematic(use_kinematic);

	temp = use_gravity;
	use_gravity = nObj.ReadBool("Gravity");
	if (temp != use_gravity)
		EnableGravity(use_gravity);

	temp = lock_linearX;
	lock_linearX = nObj.ReadBool("LockLinearX");
	if (temp != lock_linearX)
		LockLinearX(lock_linearX);

	temp = lock_linearY;
	lock_linearY = nObj.ReadBool("LockLinearY");
	if (temp != lock_linearY)
		LockLinearY(lock_linearY);

	temp = lock_linearZ;
	lock_linearZ = nObj.ReadBool("LockLinearZ");
	if (temp != lock_linearZ)
		LockLinearZ(lock_linearZ);

	lock_angularX = nObj.ReadBool("LockAngularX");
		LockAngularX(lock_angularX);

	lock_angularY = nObj.ReadBool("LockAngularY");
		LockAngularY(lock_angularY);


	lock_angularZ = nObj.ReadBool("LockAngularZ");
		LockAngularZ(lock_angularZ);
	
	mass = nObj.ReadFloat("Mass");
	SetMass(mass);
	density = nObj.ReadFloat("Density");
	SetDensity(density);

}

#ifndef STANDALONE
bool C_RigidBody::OnEditor()
{
	if (Component::OnEditor() == true)
	{

		bool temp = use_kinematic;
		ImGui::Checkbox("Is Static", &use_kinematic);
		if (temp != use_kinematic)
			EnableKinematic(use_kinematic);
		
		bool gravity = use_gravity;
		ImGui::Checkbox("Gravity", &use_gravity);
		if (gravity != use_gravity)
			EnableGravity(use_gravity);
		ImGui::Separator();

		ImGui::Columns(2, "LockBodies");
		bool linearx = lock_linearX;
		ImGui::Checkbox("LockLinearX", &lock_linearX);
		if (linearx != lock_linearX)
			LockLinearX(lock_linearX);

		bool lineary = lock_linearY;
		ImGui::Checkbox("LockLinearY", &lock_linearY);
		if (lineary != lock_linearY)
			LockLinearY(lock_linearY);

		bool linearz = lock_linearZ;
		ImGui::Checkbox("LockLinearZ", &lock_linearZ);
		if (linearz != lock_linearZ)
			LockLinearZ(lock_linearZ);

		ImGui::NextColumn();

		bool angularx = lock_angularX;
		ImGui::Checkbox("LockAngularX", &lock_angularX);
		if (angularx != lock_angularX)
			LockAngularX(lock_angularX);

		bool angulary = lock_angularY;
		ImGui::Checkbox("LockAngularY", &lock_angularY);
		if (angulary != lock_angularY)
			LockAngularY(lock_angularY);

		bool angularz = lock_angularZ;
		ImGui::Checkbox("LockAngularZ", &lock_angularZ);
		if (angularz != lock_angularZ)
			LockAngularZ(lock_angularZ);
		ImGui::Columns(1);
		ImGui::Separator();
		
		
		ImGui::DragFloat("mass", &mass, 0.1f);
		if (ImGui::IsItemActive())
		{
			SetMass(mass);
		}

		ImGui::DragFloat("density", &density, 0.1f);
		if (ImGui::IsItemActive())
		{
			SetDensity(density);
		}
		return true;
	}
	return false;
}
#endif // !STANDALONE

