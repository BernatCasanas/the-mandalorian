#include "CO_AudioSource.h"
#include "Application.h"
#include "MO_AudioManager.h"
#include "GameObject.h"
#include "CO_Transform.h"

C_AudioSource::C_AudioSource(GameObject* _gm) : Component(_gm), audBankReference(nullptr), evName(""), isMuted(false), pitch(1.0f), playOnAwake(false), volume(50.0f)
{
	this->id = unsigned int(EngineExternal->GetRandomInt());
	gameObjectTransform = (C_Transform*)gameObject->GetComponent(Component::Type::Transform);
}

C_AudioSource::~C_AudioSource()
{
	EngineExternal->moduleAudio->RemoveAudioSource(this);
}

#ifndef STANDALONE
bool C_AudioSource::OnEditor()
{
	return false;
}
#endif // !STANDALONE

void C_AudioSource::Update()
{
	EngineExternal->moduleAudio->SetAudioObjTransform(this->id, gameObjectTransform->GetCurrentGlobalMatrix());
}

void C_AudioSource::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	DEJson::WriteString(nObj, "evName", this->evName.c_str());
	DEJson::WriteString(nObj, "audBankReference", this->audBankReference->bank_name.c_str());
	DEJson::WriteFloat(nObj, "volume", this->volume);
	DEJson::WriteFloat(nObj, "pitch", this->pitch);
	DEJson::WriteBool(nObj, "playOnAwake", this->playOnAwake);
	DEJson::WriteBool(nObj, "isMuted", this->isMuted);
}

void C_AudioSource::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	this->evName = nObj.ReadString("evName");
	SetVolume(nObj.ReadFloat("volume"));
	SetPitch(nObj.ReadFloat("pitch"));
	this->playOnAwake = nObj.ReadBool("playOnAwake");
	SetMuted(nObj.ReadBool("isMuted"));

	std::string bankName = nObj.ReadString("audBankReference");

	// Iterate and assign audio bank. If not loaded, load
	std::vector<AudioBank*>::iterator it;
	for (it = EngineExternal->moduleAudio->banks.begin(); it != EngineExternal->moduleAudio->banks.end(); ++it)
	{
		if ((*it)->bank_name == bankName)
		{
			audBankReference = (*it);
			if (!(*it)->loaded_in_heap)
			{
				EngineExternal->moduleAudio->LoadBank(bankName);
				(*it)->loaded_in_heap = true;
			}
			return;
		}
	}
	audBankReference = nullptr;
	LOG(LogType::L_WARNING, "Audio Bank called %s, has not been found during scene loading...", bankName);
}

std::string& C_AudioSource::GetEventName(AudioBank* reference)
{
	reference = audBankReference;
	return evName;
}

void C_AudioSource::SetEventName(std::string& newEventName)
{
	this->evName = newEventName;
}

void C_AudioSource::SetBankReference(AudioBank* ref)
{
	this->audBankReference = ref;
}

float C_AudioSource::GetVolume()
{
	return this->volume;
}

void C_AudioSource::SetVolume(float newVol)
{
	this->volume = newVol;
	// TODO: change audio rtpc
}

float C_AudioSource::GetPitch()
{
	return this->pitch;
}

void C_AudioSource::SetPitch(float newPitch)
{
	this->pitch = newPitch;
	// TODO: change audio rtpc
}

void C_AudioSource::PlayEvent()
{
	if (this->IsActive())
		EngineExternal->moduleAudio->PlayEvent(this->id, this->evName);
}

void C_AudioSource::PauseEvent()
{
	EngineExternal->moduleAudio->PauseEvent(this->id);
}

void C_AudioSource::ResumeEvent()
{
	EngineExternal->moduleAudio->ResumeEvent(this->id);
}

void C_AudioSource::StopEvent()
{
	EngineExternal->moduleAudio->StopEvent(this->id);
}

bool C_AudioSource::IsMuted()
{
	return this->isMuted;
}

void C_AudioSource::SetMuted(bool muted)
{
	// TODO: change audio rtpc (set volume to 0 or saved volume level)
}
