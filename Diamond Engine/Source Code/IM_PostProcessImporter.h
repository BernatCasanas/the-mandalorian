#pragma once

class ResourcePostProcess;

namespace PostProcessImporter
{
	ResourcePostProcess* CreateBaseProfileFile(const char* path);
	void Save(ResourcePostProcess* postProcess, char** fileBuffer);
}