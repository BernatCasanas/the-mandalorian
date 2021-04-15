#include"Application.h"
#include "Globals.h"
#include"GameObject.h"

#include "MO_Physics.h"
#include "MO_Renderer3D.h"
#include"MO_Editor.h"

#include "CO_RigidBody.h"
#include "CO_Collider.h"
#include "CO_CapsuleCollider.h"
#include "CO_Transform.h"
#include "CO_MeshRenderer.h"

#include <vector>




C_CapsuleCollider::C_CapsuleCollider() : C_Collider(nullptr)
{
	name = "Capsule Collider_" + std::to_string(-1);


}


C_CapsuleCollider::C_CapsuleCollider(GameObject* _gm/*, float3 _position, Quat _rotation, float3 _localScale*/) : C_Collider(_gm)/*,
position(_position), rotation(_rotation), localScale(_localScale)*/
{

	int indexNum = _gm != nullptr ? _gm->GetComponentsOfType(Component::TYPE::CAPSULECOLLIDER).size() + _gm->GetComponentsOfType(Component::TYPE::COLLIDER).size() : 0;
	name = "Capsule Collider_" + std::to_string(indexNum);
	isTrigger = false;
	shape = ColliderShape::CAPSULE;

	//Checks if component does have any owner and get additional data to be created


	transform = dynamic_cast<C_Transform*>(_gm->GetComponent(Component::TYPE::TRANSFORM));
	rigidbody = dynamic_cast<C_RigidBody*>(_gm->GetComponent(Component::TYPE::RIGIDBODY));
	mesh = dynamic_cast<C_MeshRenderer*>(_gm->GetComponent(Component::TYPE::MESH_RENDERER));



	//We first initialize material to create shape later
	colliderMaterial = EngineExternal->modulePhysics->CreateMaterial();
	localTransform = float4x4::identity;
	colliderShape = nullptr;

	//If gameObject does have mesh we apply measures directly to collider from OBB
	
	if (rigidbody != nullptr)
	{
		if (mesh != nullptr) {

			colliderSize = mesh->globalOBB.Size() / 2;

			halfHeight = 0;
			for (int i = 0; i < 3; i++)
			{
				if (halfHeight < colliderSize[i])
					halfHeight = colliderSize[i];
			}
			halfHeight /= 2;
			radius = colliderSize[0];

			for (int i = 1; i < 3; i++)
			{
				if (radius > colliderSize[i])
					radius = colliderSize[i];
			}

			if (colliderSize.y <= 0.0f)
				colliderSize.y = 0.01f;
			colliderShape = EngineExternal->modulePhysics->CreateCapsuleCollider(radius, halfHeight, colliderMaterial);

		}
		else {
			colliderSize = { 0.5f, 0.5f, 0.5f };
			radius = 0.5;
			halfHeight = 0.5;
			colliderShape = EngineExternal->modulePhysics->CreateCapsuleCollider(radius, halfHeight, colliderMaterial);

		}
	}
	
	

	//We attach shape to a static or dynamic rigidbody to be collidable.
	if (rigidbody != nullptr && colliderShape != nullptr) {
		rigidbody->rigid_dynamic->attachShape(*colliderShape);
		rigidbody->collider_info.push_back(this);

	}
	

	colliderShape->setFlag(physx::PxShapeFlag::eVISUALIZATION, true);

}

C_CapsuleCollider::~C_CapsuleCollider()
{
	//LOG(LogType::L_NORMAL, "Deleting Box Collider");

	if (colliderMaterial != nullptr)
		colliderMaterial->release();
	rigidbody = dynamic_cast<C_RigidBody*>(gameObject->GetComponent(Component::TYPE::RIGIDBODY));

	if (rigidbody != nullptr)
	{
		rigidbody->rigid_dynamic->detachShape(*colliderShape);
		for (int i = 0; i < rigidbody->collider_info.size(); i++)
		{
			if (rigidbody->collider_info[i] == this)
			{
				rigidbody->collider_info.erase(rigidbody->collider_info.begin() + i);
				i--;
			}

		}
	}

	if (colliderShape != nullptr)
		colliderShape->release();

}


#ifndef STANDALONE

void C_CapsuleCollider::Update()
{

	if (rigidbody == nullptr)
		rigidbody = dynamic_cast<C_RigidBody*>(gameObject->GetComponent(Component::TYPE::RIGIDBODY));

	if (colliderShape != nullptr)
	{
		//EngineExternal->modulePhysics->DrawCollider(this);

		float4x4 trans;
		if (rigidbody != nullptr && rigidbody->rigid_dynamic)
			trans = EngineExternal->modulePhysics->PhysXTransformToF4F(rigidbody->rigid_dynamic->getGlobalPose());
		else
			trans = transform->globalTransform;

		trans = trans * localTransform;
		float3 pos, scale;
		Quat rot;
		trans.Decompose(pos, rot, scale);
		//scale.Set(radius, radius, radius);
		//trans = trans.FromTRS(pos, rot, scale);
		//SetPosition(pos);


		GLfloat x, y, z, alpha, beta; // Storage for coordinates and angles        
		int gradation = 10;


		
		

		float delta_angle = 360.0f / 50.0f;
		float half_delta_angle = 180.f / 25.0f;
		float curr_angle = 0.f;

		LineSegment drawLine;

		
		for (int i = 0; i <= 50; ++i) {
			curr_angle = delta_angle * i;

			x = radius * cosf(DEGTORAD * curr_angle);
			y = halfHeight;
			z = radius * sinf(DEGTORAD * curr_angle);
			drawLine.a.Set(x, y, z);
			if (i != 0.0)
				EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));
			drawLine.b.Set(x, y, z);
		}

		
		for (int i = 0; i <= 25; ++i) {
			curr_angle = half_delta_angle * i;
			x = radius * cosf(DEGTORAD * curr_angle);
			y = radius * sinf(DEGTORAD * curr_angle) + halfHeight;
			z = 0.0f;
			drawLine.a.Set(x, y, z);

			if (i != 0.0)
				EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));
			drawLine.b.Set(x, y, z);

		}
	

		for (int i = 0; i <= 25; ++i) {
			curr_angle = half_delta_angle * i;

			x = 0.0f;
			y = radius * sinf(DEGTORAD * curr_angle) + halfHeight;
			z = radius * cosf(DEGTORAD * curr_angle);
			drawLine.a.Set(x, y, z);

			if (i != 0.0)
				EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));
			drawLine.b.Set(x, y, z);
		}


		for (int i = 0; i < 50; ++i) {
			curr_angle = delta_angle * i;
			x = radius * cosf(DEGTORAD * curr_angle);
			y = -halfHeight;
			z = radius * sinf(DEGTORAD * curr_angle);
			drawLine.a.Set(x, y, z);

			if (i != 0.0)
				EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));
			drawLine.b.Set(x, y, z);

		}

		for (int i = 0; i <= 25.0; ++i) {
			curr_angle = 180.F + half_delta_angle * i;
			x = radius * cosf(DEGTORAD * curr_angle);
			y = radius * sinf(DEGTORAD * curr_angle) - halfHeight;
			z = 0.0f;
			drawLine.a.Set(x, y, z);

			if (i != 0.0)
				EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));
			drawLine.b.Set(x, y, z);
		}
	

		for (int i = 0; i <= 25; ++i) {
			curr_angle = 180.F + half_delta_angle * i;

			x = 0.0f;
			y = radius * sinf(DEGTORAD * curr_angle) - halfHeight;
			z = radius * cosf(DEGTORAD * curr_angle);
			drawLine.a.Set(x, y, z);

			if (i != 0.0)
				EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));
			drawLine.b.Set(x, y, z);
		}

		drawLine.a.Set(0.f, halfHeight, -radius);
		drawLine.b.Set(0.f, -halfHeight, -radius);

		EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));


		drawLine.a.Set(0.f, halfHeight, radius);
		drawLine.b.Set(0.f, -halfHeight, radius);

		EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));

		drawLine.a.Set(-radius, halfHeight, 0.f);
		drawLine.b.Set(-radius, -halfHeight, 0.f);

		EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));
		drawLine.a.Set(radius, halfHeight, 0.f);
		drawLine.b.Set(radius, -halfHeight, 0.f);

		EngineExternal->moduleRenderer3D->AddDebugLines(trans.MulPos(drawLine.a), trans.MulPos(drawLine.b), float3(0.0f, 1.0f, 0.0f));

		

		
	}

}
#endif // !STANDALONE






void C_CapsuleCollider::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	//	DEJson::WriteBool(nObj, "kinematic", use_kinematic);
	float3 pos, scale;
	Quat rot;
	localTransform.Decompose(pos, rot, scale);

	Component::SaveData(nObj);

	DEJson::WriteBool(nObj, "isTrigger", isTrigger);

	DEJson::WriteVector3(nObj, "Position", pos.ptr());
	DEJson::WriteQuat(nObj, "Rotation", rot.ptr());
	DEJson::WriteFloat(nObj, "Radius", radius);
	DEJson::WriteFloat(nObj, "HalfHeight", halfHeight);


}

void C_CapsuleCollider::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);
	float3 pos;
	Quat rot;
	bool trigger;

	trigger = nObj.ReadBool("isTrigger");
	if (trigger != isTrigger)
	{
		SetTrigger(trigger);
		isTrigger = trigger;
	}

	pos = nObj.ReadVector3("Position");
	rot = nObj.ReadQuat("Rotation");
	radius = nObj.ReadFloat("Radius");
	halfHeight = nObj.ReadFloat("HalfHeight");


	physx::PxTransform physTrans;
	physTrans.p = physx::PxVec3(pos.x, pos.y, pos.z);
	physTrans.q = physx::PxQuat(rot.x, rot.y, rot.z, rot.w) * physx::PxQuat(0, 0, 0.7071068, 0.7071068);
	float3 newSize;
	
	
	colliderShape->setGeometry(physx::PxCapsuleGeometry(radius, halfHeight));
	colliderShape->setLocalPose(physTrans);

	localTransform = float4x4::FromTRS(pos, rot, float3(1, 1, 1));

}


#ifndef STANDALONE
bool C_CapsuleCollider::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGuiTreeNodeFlags node_flags = ImGuiTreeNodeFlags_DefaultOpen | ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_SpanAvailWidth;

		std::vector<Component*>::iterator it = std::find(rigidbody->collider_info.begin(), rigidbody->collider_info.end(), this);
		int index = std::distance(rigidbody->collider_info.begin(), it);
		bool trigger = isTrigger;
		std::string suffix = "isTrigger##" + std::to_string(index);
		ImGui::Checkbox(suffix.c_str(), &trigger);

		if (trigger != isTrigger)
		{
			SetTrigger(trigger);
			isTrigger = trigger;
		}

		ImGui::Separator();
		if (ImGui::TreeNodeEx(std::string("Edit Collider: " + name + "##" + std::to_string(index)).c_str(), node_flags))
		{


			ImGui::Columns(4, "Collidercolumns");
			ImGui::Separator();

			ImGui::Text("Position"); ImGui::Spacing(); ImGui::Spacing();// ImGui::NextColumn();
			ImGui::Text("Rotation"); ImGui::Spacing(); ImGui::Spacing(); //ImGui::NextColumn();
			ImGui::Text("Radius"); 
			ImGui::Text("Half Heigth"); ImGui::NextColumn();

			// Position
			float3 pos, scale;
			Quat rot;
			localTransform.Decompose(pos, rot, scale);
			float3 rotation = rot.ToEulerXYZ();

			float t = pos.x;
			ImGui::SetNextItemWidth(50);
			suffix = "##Posx" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &t);
			if (ImGui::IsItemActive())
			{
				pos.x = t;
				localTransform = float4x4::FromTRS(pos, rot, scale);

				SetPosition(pos);


			}
			ImGui::SetNextItemWidth(50);

			//Rotation
			float r1 = rotation.x;
			ImGui::SetNextItemWidth(50);
			suffix = "##Rotx" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &r1);
			if (ImGui::IsItemActive())
			{
				float newrot = r1 - rotation.x;

				rotation.x = r1;
				float3 axis(1, 0, 0);
				Quat newquat = Quat::RotateAxisAngle(axis, newrot * pi / 180);
				rot = rot * newquat;
				localTransform = float4x4::FromTRS(pos, rot, scale);

				SetRotation(rot);

			}
			//Radius
			ImGui::SetNextItemWidth(50);

			float rad = radius;
			suffix = "##radius" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &rad);
			if (ImGui::IsItemActive())
			{
				radius = rad;
				colliderShape->setGeometry(physx::PxCapsuleGeometry(radius, halfHeight));
			}

			ImGui::SetNextItemWidth(50);
			float h = halfHeight;
			suffix = "##halfheigth" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &h);
			if (ImGui::IsItemActive())
			{
				halfHeight = h;
				colliderShape->setGeometry(physx::PxCapsuleGeometry(radius, halfHeight));
			}
			ImGui::NextColumn();

			// Position

			float t1 = pos.y;
			ImGui::SetNextItemWidth(50);
			suffix = "##Posy" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &t1);
			if (ImGui::IsItemActive())
			{
				pos.y = t1;
				localTransform = float4x4::FromTRS(pos, rot, scale);

				SetPosition(pos);
			}
		
			float r2 = rotation.y;
			ImGui::SetNextItemWidth(50);
			suffix = "##Roty" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &r2);
			if (ImGui::IsItemActive())
			{
				float newrot = r2 - rotation.y;

				rotation.y = r2;
				float3 axis(0, 1, 0);
				Quat newquat = Quat::RotateAxisAngle(axis, newrot * pi / 180);
				rot = rot * newquat;
				localTransform = float4x4::FromTRS(pos, rot, scale);

				SetRotation(rot);

			}
			//Scale
			float s2 = scale.y;
			ImGui::SetNextItemWidth(50);

			
			
			ImGui::NextColumn();

			// Position
			float t2 = pos.z;
			ImGui::SetNextItemWidth(50);
			suffix = "##Posz" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &t2);
			if (ImGui::IsItemActive())
			{
				pos.z = t2;
				localTransform = float4x4::FromTRS(pos, rot, scale);

				SetPosition(pos);
			}
			
			// Rotation
			float r3 = rotation.z;
			ImGui::SetNextItemWidth(50);
			suffix = "##Rotz" + std::to_string(index);
			ImGui::DragFloat(suffix.c_str(), &r3);
			if (ImGui::IsItemActive())
			{
				float newrot = r3 - rotation.z;

				rotation.z = r3;
				float3 axis(0, 0, 1);
				Quat newquat = Quat::RotateAxisAngle(axis, newrot * pi / 180);
				rot = rot * newquat;
				localTransform = float4x4::FromTRS(pos, rot, scale);

				SetRotation(rot);

			}
			ImGui::NextColumn();

			ImGui::Columns(1);
			ImGui::TreePop();

		}
		ImGui::Separator();

		//Editar materiales
		//colliderMaterial

		return true;
	}
	return false;
}


void C_CapsuleCollider::SetRotation(Quat rotation) {



	physx::PxTransform transformation = colliderShape->getLocalPose();
	transformation.q = physx::PxQuat(rotation.x, rotation.y, rotation.z, rotation.w) * physx::PxQuat(0, 0, 0.7071068, 0.7071068);

	colliderShape->setLocalPose(transformation); //Set new Transformation Values

}

#endif // !STANDALONE



