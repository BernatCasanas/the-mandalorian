#pragma once

#include "Application.h"
#include "MO_MonoManager.h"
#include "GameObject.h"
#include "CS_Transform_Bindings.h"
#include "CO_Material.h"
#include "RE_Material.h"
#include "RE_Texture.h"
#include "CO_RigidBody.h"

MonoString* GetTag(MonoObject* cs_Object)
{
	GameObject* cpp_gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(cs_Object);

	return mono_string_new(EngineExternal->moduleMono->domain, cpp_gameObject->tag);
}

void SetVelocity(MonoObject* cs_GameObject, MonoObject* cs_Velocity)
{
	float3 velocity;
	GameObject* cpp_gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(cs_GameObject);
	velocity = M_MonoManager::UnboxVector(cs_Velocity);
	
	C_RigidBody* rigidbody = dynamic_cast<C_RigidBody*>(cpp_gameObject->GetComponent(Component::TYPE::RIGIDBODY));
	
	if (rigidbody)
	rigidbody->SetLinearVelocity(velocity);

}


void AddForce(MonoObject* cs_GameObject, MonoObject* cs_Force)
{
	float3 force;
	GameObject* cpp_gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(cs_GameObject);
	force = M_MonoManager::UnboxVector(cs_Force);

	C_RigidBody* rigidbody = dynamic_cast<C_RigidBody*>(cpp_gameObject->GetComponent(Component::TYPE::RIGIDBODY));
	
	if(rigidbody)
	rigidbody->AddForce(force);

}


MonoObject* FindObjectWithName(MonoString* name) {

	std::vector<GameObject*> gameObjectVec;
	EngineExternal->moduleScene->root->CollectChilds(gameObjectVec);

	if (name == NULL) {
		assert("The name you passed is null. >:/");
		return nullptr;
	}

	char* _name = mono_string_to_utf8(name);
	mono_free(_name);

	for (int i = 0; i < gameObjectVec.size(); i++) {

		if(strcmp(gameObjectVec[i]->name.c_str(), _name) == 0){
		
			return EngineExternal->moduleMono->GoToCSGO(gameObjectVec[i]);

		}

	}

	assert("The object you searched for doesn't exist. :/");
	return nullptr;

}


void CS_SetParent(MonoObject* gameObject, MonoObject* newParent) {

	GameObject* cpp_gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(gameObject);
	GameObject* cpp_parent = EngineExternal->moduleMono->GameObject_From_CSGO(newParent);
	cpp_gameObject->ChangeParent(cpp_parent);

}

int GetUid(MonoObject* cs_Object)
{
	GameObject* cpp_gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(cs_Object);
	int ret = -1;

	if (cpp_gameObject != nullptr)
	{
		ret = cpp_gameObject->UID;
	}

	return ret;
}

void AssignLibraryTextureToMaterial(MonoObject* obj, int _id, MonoString* textureName)
{

	GameObject* cpp_gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(obj);
	C_Material* imageMaterial = dynamic_cast<C_Material*>(cpp_gameObject->GetComponent(Component::TYPE::MATERIAL));

	if (imageMaterial == nullptr) {
		return;
	}

	ResourceMaterial* resource = imageMaterial->material;

	if (resource != nullptr) {

		char* uniformName = mono_string_to_utf8(textureName);

		for (int i = 0; i < resource->uniforms.size(); i++) {

			if (strcmp(resource->uniforms[i].name, uniformName) == 0) {

				if (resource->uniforms[i].data.textureValue != nullptr) {

					EngineExternal->moduleResources->UnloadResource(resource->uniforms[i].data.textureValue->GetUID());

				}

				resource->uniforms[i].data.textureValue = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(_id, Resource::Type::TEXTURE));

			}

		}

		mono_free(uniformName);

	}

}


//---------- RAY CAST ----------//
MonoObject* FindGOWithRayCastBetweenTwoGO(MonoObject* gameObject, MonoObject* goal_gameobject) {

	GameObject* cpp_gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(gameObject);
	GameObject* cpp_goal_gameobject = EngineExternal->moduleMono->GameObject_From_CSGO(goal_gameobject);

	LineSegment ray (cpp_gameObject->transform->position, cpp_goal_gameobject->transform->position);
	return EngineExternal->moduleMono->GoToCSGO(EngineExternal->moduleRenderer3D->RayToMeshQueueIntersection(ray));

}