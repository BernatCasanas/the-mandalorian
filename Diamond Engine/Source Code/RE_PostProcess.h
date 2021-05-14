#pragma once

#include "DEResource.h"
#include "MathGeoLib/include/Math/float2.h"
#include "MathGeoLib/include/Math/float4.h"

#include<vector>
#include "parson/parson.h"

struct DEConfig;

enum class POSTPROCESS_DATA_TYPE
{
	AO,
	BLOOM,
	TONE_MAPPING,
	VIGNETTE,
	NONE
};

enum class VIGNETTE_MODE
{
	RECTANGULAR,
	CIRCULAR,
	NONE
};

class PostProcessData
{
public:
	PostProcessData(POSTPROCESS_DATA_TYPE type, std::string name);
	~PostProcessData();
#ifndef STANDALONE
	virtual void DrawEditor();
#endif
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
#ifndef STANDALONE
	void DrawEditor() override;
#endif
	void SaveToJson(JSON_Object* nObj);
	void LoadFromJson(DEConfig& nObj);
public:
	float radiusAO;
	float blurSpread;
	bool useBlur;
	float bias;
};

class PostProcessDataBloom : public PostProcessData
{
public:
	PostProcessDataBloom();
	~PostProcessDataBloom();
#ifndef STANDALONE
	void DrawEditor() override;
#endif
	void SaveToJson(JSON_Object* nObj);
	void LoadFromJson(DEConfig& nObj);
public:
	float brightThreshold;
	float brightnessIntensity;
	float blurSpread;
	bool smoothMask;
};

class PostProcessDataToneMapping : public PostProcessData
{
public:
	PostProcessDataToneMapping();
	~PostProcessDataToneMapping();
#ifndef STANDALONE
	void DrawEditor() override;
#endif
	void SaveToJson(JSON_Object* nObj);
	void LoadFromJson(DEConfig& nObj);
public:
	float exposure;
	float gamma;
};

class PostProcessDataVignette : public PostProcessData
{
public:
	PostProcessDataVignette();
	~PostProcessDataVignette();
#ifndef STANDALONE
	void DrawEditor() override;
#endif
	void SaveToJson(JSON_Object* nObj);
	void LoadFromJson(DEConfig& nObj);
public:

	float intensity;
	float extend;
	float4 tint;
	float2 minMaxRadius;
	VIGNETTE_MODE mode;
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
