#pragma once

#include "Application.h"
#include"MO_Scene.h"

MonoObject* FindObjectWithTag(MonoString* cs_tag)
{
	if (cs_tag == nullptr)
		return nullptr;

	char* cpp_tag = mono_string_to_utf8(cs_tag);

	GameObject* taggedObject = EngineExternal->moduleScene->FindObjectWithTag(EngineExternal->moduleScene->root, cpp_tag);
	mono_free(cpp_tag);

	return EngineExternal->moduleMono->GoToCSGO(taggedObject);
}

int GetTotalTris() {
	return EngineExternal->moduleScene->totalTris;
}

void CS_LoadScene(int libraryPath)
{
	EngineExternal->moduleScene->holdUID = libraryPath;
}