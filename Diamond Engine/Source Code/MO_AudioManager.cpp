#include "MO_AudioManager.h"
#include "Wwise_Includes.h"
#include "Wwise/low_level_IO/Win32/AkFilePackageLowLevelIOBlocking.h"
#include "CO_AudioListener.h"
#include "CO_AudioSource.h"
#include "MathGeoLib/include/MathGeoLib.h"
#include "DEJsonSupport.h"

#include <assert.h>

CAkFilePackageLowLevelIOBlocking g_lowLevelIO;


ModuleAudioManager::ModuleAudioManager(Application* app, bool start_enabled): Module(app,start_enabled),wwiseListenerHasToUpdate(false), defaultListener(nullptr)
{
	//TODO listener code here
}

ModuleAudioManager::~ModuleAudioManager()
{	

}

bool ModuleAudioManager::Init()
{
	//Init Memory Manager
	AkMemSettings memSettings;

	AK::MemoryMgr::GetDefaultSettings(memSettings);

	if (AK::MemoryMgr::Init(&memSettings) != AK_Success)
	{
		assert(!"Could not create the memory manager.");
		return false;
	}

	//Init Stream Manager
	AkStreamMgrSettings stmSettings;

	AK::StreamMgr::GetDefaultSettings(stmSettings);

	// Customize the Stream Manager settings here.

	if (!AK::StreamMgr::Create(stmSettings))
	{
		assert(!"Could not create the Streaming Manager.");
		return false;
	}

	// Create a streaming device with blocking low-level I/O handshaking.
	AkDeviceSettings deviceSettings;

	AK::StreamMgr::GetDefaultDeviceSettings(deviceSettings);

	if (g_lowLevelIO.Init(deviceSettings) != AK_Success)
	{
		assert(!"Could not create the streaming device and Low-Level I/O system.");
		return false;
	}

	//Init Sound Engine
	AkInitSettings initSettings;
	AkPlatformInitSettings platformInitSettings;

	AK::SoundEngine::GetDefaultInitSettings(initSettings);
	AK::SoundEngine::GetDefaultPlatformInitSettings(platformInitSettings);

	if (AK::SoundEngine::Init(&initSettings, &platformInitSettings) != AK_Success)
	{
		assert(!"Could not initialize the Sound Engine.");
		return false;
	}

	//Init Music Engine
	AkMusicSettings musicInit;

	AK::MusicEngine::GetDefaultInitSettings(musicInit);

	if (AK::MusicEngine::Init(&musicInit) != AK_Success)
	{
		assert(!"Could not initialize the Music Engine.");
		return false;
	}

	//Load Soundbanks
	// TODO: Check BasePath for different versions. What if there are banks in library as resources? Use assets or library
	g_lowLevelIO.SetBasePath(AKTEXT("Assets/SoundBanks/"));

	AK::StreamMgr::SetCurrentLanguage(AKTEXT("English(US)"));



	AkBankID bankID;
	AKRESULT ret = AK::SoundEngine::LoadBank("Init.bnk", bankID);
	if (ret != AK_Success)
	{
		assert(!"Could not initialize Init.bnk.");
		return false;
	}

	return true;
}

bool ModuleAudioManager::Start()
{
	if (!LoadBanksInfo())
	{
		LOG(LogType::L_ERROR, "Audio Manager couldn't load data from SoundbanksInfo.json");
	}
	return true;
}

update_status ModuleAudioManager::Update(float dt)
{
	if (wwiseListenerHasToUpdate)
	{
		UpdateWwiseListener();
		wwiseListenerHasToUpdate = false;
	}
	return UPDATE_CONTINUE;
}

update_status ModuleAudioManager::PostUpdate(float dt)
{

	// Process bank requests, events, positions, RTPC, etc.
	AK::SoundEngine::RenderAudio();

	return UPDATE_CONTINUE;
}

bool ModuleAudioManager::CleanUp()
{
	AK::SoundEngine::UnregisterAllGameObj();
	AK::SoundEngine::ClearBanks();

	// Terminate the music engine
	AK::MusicEngine::Term();

	// Terminate the sound engine
	AK::SoundEngine::Term();

	// that lives in the Stream Manager, and unregisters itself as the File Location Resolver.
	g_lowLevelIO.Term();

	if (AK::IAkStreamMgr::Get())
		AK::IAkStreamMgr::Get()->Destroy();

	// Terminate the Memory Manager
	AK::MemoryMgr::Term();

	audio_sources.clear();

	return true;
}

void ModuleAudioManager::RegisterNewAudioObject(unsigned int id)
{
	AK::SoundEngine::RegisterGameObj(id);
}

void ModuleAudioManager::UnRegisterAudioObject(unsigned int id)
{
	AK::SoundEngine::UnregisterGameObj(id);
}

void ModuleAudioManager::StopAllSounds() const
{
	AK::SoundEngine::StopAll();
}

void ModuleAudioManager::PauseAllSounds() const
{
	std::vector<C_AudioSource*>::const_iterator it;
	for (it = audio_sources.begin(); it != audio_sources.end(); ++it)
	{
		(*it)->PauseEvent();
	}
}

void ModuleAudioManager::ResumeAllSounds() const
{
	std::vector<C_AudioSource*>::const_iterator it;
	for (it = audio_sources.begin(); it != audio_sources.end(); ++it)
	{
		(*it)->ResumeEvent();
	}
}

void ModuleAudioManager::PlayEvent(unsigned int id, std::string& eventName)
{
	AK::SoundEngine::PostEvent(eventName.c_str() , id);
}

void ModuleAudioManager::StopEvent(unsigned int id) const
{
	AK::SoundEngine::ExecuteActionOnPlayingID(AK::SoundEngine::AkActionOnEventType_Stop, id);
}

void ModuleAudioManager::PauseEvent(unsigned int id) const
{
	AK::SoundEngine::ExecuteActionOnPlayingID(AK::SoundEngine::AkActionOnEventType_Pause, id);
}

void ModuleAudioManager::ResumeEvent(unsigned int id) const
{
	AK::SoundEngine::ExecuteActionOnPlayingID(AK::SoundEngine::AkActionOnEventType_Resume, id);
}

void ModuleAudioManager::ChangeRTPCValue(unsigned int id, std::string& RTPCname, float value)
{
	AK::SoundEngine::SetRTPCValue(RTPCname.c_str(), value, id);
}

bool ModuleAudioManager::LoadBanksInfo()
{
	JSON_Value* banksInfo = json_parse_file("Assets/SoundBanks/SoundbanksInfo.json");

	if (banksInfo == NULL)
		return false;

	DEConfig banksObject(json_value_get_object(banksInfo));
	DEConfig banksData(banksObject.ReadObject("SoundBanksInfo"));
	JSON_Array* banksArray = banksData.ReadArray("SoundBanks");

	for (unsigned int cursor = 0; cursor < json_array_get_count(banksArray); ++cursor)
	{
		DEConfig tmp(json_array_get_object(banksArray, cursor));
		if (strcmp(tmp.ReadString("ShortName"), "Init") != 0)
		{
			AudioBank* tmpBank = new AudioBank;
			JSON_Array* tmpEvents;

			tmpBank->bank_name = tmp.ReadString("ShortName");
			
			tmpEvents = tmp.ReadArray("IncludedEvents");

			// ec stands for event cursor
			for (unsigned int ec = 0; ec < json_array_get_count(tmpEvents); ++ec)
			{
				DEConfig aux(json_array_get_object(tmpEvents, ec));
				if (!((std::string)aux.ReadString("Name")).find("Play"))
				{
					tmpBank->events[std::stoull(aux.ReadString("Id"))] = aux.ReadString("Name");
				}
				else
				{
					tmpBank->actions[std::stoull(aux.ReadString("Id"))] = aux.ReadString("Name");
				}
			}
			banks.push_back(tmpBank);
		}
	}


	return true;
}

bool ModuleAudioManager::LoadBank(std::string& name)
{
	AkBankID id;
	AKRESULT eResult;

	eResult = AK::SoundEngine::LoadBank(name.c_str(), id);
	if (eResult == AK_Success)
	{
		LOG(LogType::L_NORMAL,"Bank 'Main.bnk' has been loaded successfully");
		return true;
	}
	else
	{
		LOG(LogType::L_ERROR,"Error loading 'Main.bnk'");
		return false;
	}

}

void ModuleAudioManager::WwiseListnerHasToUpdate()
{
	wwiseListenerHasToUpdate = true;
}

void ModuleAudioManager::AddAudioSource(C_AudioSource* new_source)
{
	audio_sources.push_back(new_source);
}

void ModuleAudioManager::RemoveAudioSource(C_AudioSource* source)
{
	std::vector<C_AudioSource*>::const_iterator it;
	for (it = audio_sources.begin(); it != audio_sources.end(); ++it)
	{
		if ((*it) == source)
		{
			audio_sources.erase(it);
			return;
		}
	}
}

void ModuleAudioManager::SetAudioObjTransform(unsigned int id, float4x4& transform)
{
	float3 pos;
	float3 scale;
	float3x3 rot;
	float3 front(0.0f, 0.0f, 1.0f);
	float3 up(0.0f, 1.0f, 0.0f);

	transform.Decompose(pos, rot, scale);

	//TODO take orientation too?
	up = up * rot;
	front = front * rot;

	AkSoundPosition newPos;
	newPos.SetPosition(pos.x, pos.y, pos.z);
	newPos.SetOrientation(front.x, front.y, front.z, up.x, up.y, up.z);

	AK::SoundEngine::SetPosition(id, newPos);
}

//this updates the listener that Wwise uses to be the Module Audio default listener
void ModuleAudioManager::UpdateWwiseListener()
{
	AkGameObjectID id = 0;

	if (defaultListener != nullptr)
	{
		id = defaultListener->GetID();
	}
	else
	{
		LOG(LogType::L_WARNING,"There is no audio listener component active. Sounds won't be heard");
	}

	// Set one listener as the default.
	AK::SoundEngine::SetDefaultListeners(&id, 1);
}
