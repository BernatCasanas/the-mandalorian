#include "CO_AudioListener.h"
#include "MathGeoLib/include/Math/float4x4.h"
#include "GameObject.h"
#include "CO_Transform.h"
#include "ImGui/imgui.h"
#include "Application.h"
#include "MO_AudioManager.h"


C_AudioListener::C_AudioListener(GameObject* _gm, bool defaultListener) :Component(_gm), isDefaultListener(false), myTransform(nullptr)
{
	name = "Audio Listener";
	myTransform = dynamic_cast<C_Transform*>(gameObject->GetComponent(Component::TYPE::TRANSFORM));

	id = static_cast<unsigned int>(EngineExternal->GetRandomInt());
	EngineExternal->moduleAudio->RegisterNewAudioObject(id);
	SetAsDefaultListener(defaultListener);
}


C_AudioListener::~C_AudioListener()
{
	EngineExternal->moduleAudio->UnRegisterAudioObject(id);
	SetAsDefaultListener(false);
	myTransform = nullptr;
}

#ifndef STANDALONE
bool C_AudioListener::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::Separator();

		bool listenerAux = isDefaultListener;
		if (ImGui::Checkbox("Default Listener", &listenerAux))
		{
			SetAsDefaultListener(listenerAux);
		}

		return true;
	}
	return false;
}
#endif // !STANDALONE

void C_AudioListener::Update()
{
	float3 pos = myTransform->globalTransform.TranslatePart();
	EngineExternal->moduleAudio->SetAudioObjTransform(id, pos, myTransform->GetForward(), myTransform->GetUp());
}

void C_AudioListener::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);
	DEJson::WriteBool(nObj, "isDefaultListener", isDefaultListener);

}

void C_AudioListener::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);
	SetAsDefaultListener(nObj.ReadBool("isDefaultListener"));

}



void C_AudioListener::SetVolume(float newVol)
{
	newVol = MIN(newVol, 99.99f);
	newVol = MAX(newVol, 0.0f);

	if (isDefaultListener)
		EngineExternal->moduleAudio->SetBusVolume(newVol);
}

uint C_AudioListener::GetID()
{
	return id;
}

void C_AudioListener::SetID(uint id)//TODO will it be possible to change Component ID after initialization? if not delete SetID()
{
	EngineExternal->moduleAudio->UnRegisterAudioObject(this->id);
	this->id = id;
	EngineExternal->moduleAudio->RegisterNewAudioObject(id);
}

void C_AudioListener::SetAsDefaultListener(bool setDefault)
{
	if (setDefault)
	{
		if (EngineExternal->moduleAudio->defaultListener != nullptr)
		{
			if (EngineExternal->moduleAudio->defaultListener != this)
			{
				EngineExternal->moduleAudio->defaultListener->SetAsDefaultListener(false);
			}

		}
		EngineExternal->moduleAudio->defaultListener = this;
		EngineExternal->moduleAudio->WwiseListnerHasToUpdate();


	}
	else if (EngineExternal->moduleAudio->defaultListener != nullptr)
	{
		if (EngineExternal->moduleAudio->defaultListener == this)
		{
			EngineExternal->moduleAudio->defaultListener = nullptr;
			EngineExternal->moduleAudio->WwiseListnerHasToUpdate();
		}
	}

	isDefaultListener = setDefault;
	SetVolume(EngineExternal->moduleAudio->masterVolume);
}

bool C_AudioListener::IsDefaultListener()
{
	return isDefaultListener;
}
