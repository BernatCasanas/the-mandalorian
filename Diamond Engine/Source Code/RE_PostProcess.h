#pragma once

#include "DEResource.h"

#include<vector>
#include "parson/parson.h"

struct DEConfig;

enum class POSTPROCESS_DATA_TYPE
{
	AO,
	BLOOM,
	NONE
};

class PostProcessData
{
public:
	PostProcessData(POSTPROCESS_DATA_TYPE type, std::string name);
	~PostProcessData();
	virtual void DrawEditor();
	virtual void SaveToJson(JSON_Object* nObj);
	virtual void LoadFromJson(DEConfig& nObj);
	POSTPROCESS_DATA_TYPE GetType()const;
protected:
	void DrawEditorStart(std::string suffix);
	void DrawEditorEnd();

public:
	bool active;
protected:
	POSTPROCESS_DATA_TYPE type;
	std::string name;
};

class PostProcessDataAO: public PostProcessData
{
public:
	PostProcessDataAO();
	~PostProcessDataAO();
	void DrawEditor() override;
	void SaveToJson(JSON_Object* nObj);
	void LoadFromJson(DEConfig& nObj);
public:
	float radiusAO;
	float blurSpread;
};

class PostProcessDataBloom : public PostProcessData
{
public:
	PostProcessDataBloom();
	~PostProcessDataBloom();
	void DrawEditor() override;
	void SaveToJson(JSON_Object* nObj);
	void LoadFromJson(DEConfig& nObj);
public:
	float brightThreshold;
	float brightnessIntensity;
	float blurSpread;
	bool smoothMask;
};

class ResourcePostProcess : public Resource {
public:
	ResourcePostProcess(unsigned int _uid);
	~ResourcePostProcess();

	bool LoadToMemory() override;
	bool UnloadFromMemory() override;

	PostProcessData* GetDataOfType(POSTPROCESS_DATA_TYPE type);
	
	void Init();
	void CleanUp();

#ifndef STANDALONE
	void DrawEditor(std::string suffix);
#endif // !STANDALONE
	void SaveToJson(JSON_Object* nObj);

public:
	std::vector<PostProcessData*> postProcessData;
};
