#pragma once
#include <mono/metadata/object.h>
#include <mono/metadata/object-forward.h>
#include "CO_Animator.h"

void Play(MonoObject* goObj,  MonoString* animationString, float speed = 1.0f)
{
	if (animationString == NULL)
		return;

	GameObject* gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(goObj);
	C_Animator* animator = dynamic_cast<C_Animator*>(gameObject->GetComponent(Component::TYPE::Animator));

	if (animator != nullptr)
	{
		char* animationName = mono_string_to_utf8(animationString);
		animator->Play(std::string(animationName), 0.1f, speed);
		mono_free(animationName);
	}
}

void Pause(MonoObject* goObj)
{
	GameObject* gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(goObj);
	C_Animator* animator = dynamic_cast<C_Animator*>(gameObject->GetComponent(Component::TYPE::Animator));

	if (animator != nullptr)
	{
		animator->Pause();
	}
}

void Resume(MonoObject* goObj)
{
	GameObject* gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(goObj);
	C_Animator* animator = dynamic_cast<C_Animator*>(gameObject->GetComponent(Component::TYPE::Animator));

	if (animator != nullptr)
	{
		animator->Resume();
	}
}
MonoString* GetCurrentAnimation(MonoObject* goObj)
{
	GameObject* gameObject = EngineExternal->moduleMono->GameObject_From_CSGO(goObj);
	C_Animator* animator = dynamic_cast<C_Animator*>(gameObject->GetComponent(Component::TYPE::Animator));

	if (animator != nullptr)
	{
		std::string animationName = animator->GetCurrentAnimation();
		return mono_string_new(EngineExternal->moduleMono->domain, animationName.c_str());
		
	}
}



