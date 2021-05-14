#include "RE_PostProcess.h"

#include"DEJsonSupport.h"
#include"IM_FileSystem.h"
#include "ImGui/imgui.h"
#include "IM_PostProcessImporter.h"

PostProcessData::PostProcessData(POSTPROCESS_DATA_TYPE type, std::string name) :type(type),
name(name), active(false)
{

}

PostProcessData::~PostProcessData()
{
	type = POSTPROCESS_DATA_TYPE::NONE;
	active = false;
	name.clear();
}
#ifndef STANDALONE


void PostProcessData::DrawEditorStart(std::string suffix)
{
	//TODO put header here and call this method from the child class
	std::string label = name + suffix;
	label = "Active";
	label += suffix;
	ImGui::Checkbox(label.c_str(), &active);

	/*if (!active)
	{
		ImGui::PushStyleColor(ImGuiCol_::ImGuiCol_Text, ImVec4(0.5f,0.5f,0.5f,1.0f));
	}*/
	ImGui::Indent();
	ImGui::Spacing();
	ImGui::Spacing();

}

void PostProcessData::DrawEditorEnd()
{
	//TODO call this method from the child class
	ImGui::Spacing();
	ImGui::Separator();
	ImGui::Spacing();
	ImGui::Unindent();
	/*if (!active)
	{
		ImGui::PopStyleColor();
	}*/
}

void PostProcessData::DrawEditor()
{

}

#endif // !STANDALONE
void PostProcessData::SaveToJson(JSON_Object* nObj)
{
	//TODO call this from child
	DEJson::WriteInt(nObj, "Type", (int)type);
	DEJson::WriteBool(nObj, "Active", active);
	DEJson::WriteString(nObj, "Name", name.c_str());
}

void PostProcessData::LoadFromJson(DEConfig& nObj)
{
	//TODO call this from child
	active = nObj.ReadBool("Active");
	name = nObj.ReadString("Name");
	//TODO load type from parent?
}

POSTPROCESS_DATA_TYPE PostProcessData::GetType() const
{
	return type;
}

PostProcessDataAO::PostProcessDataAO() : PostProcessData(POSTPROCESS_DATA_TYPE::AO, "Screen Space Ambient Oclussion"),
radiusAO(1.0f), blurSpread(0),useBlur(false), bias(0.1f)
{
}

PostProcessDataAO::~PostProcessDataAO()
{
}
#ifndef STANDALONE
void PostProcessDataAO::DrawEditor()
{
	const std::string suffix = "##AO";
	std::string label = name + suffix;
	if (ImGui::CollapsingHeader(label.c_str()))
	{
		PostProcessData::DrawEditorStart(suffix);

		//TODO drawEditorHere

		label = "AO radius";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &radiusAO, 0.0f, 5.0f, "%.3f", 3.0f);
		label = "Bias";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &bias, -1.0f, 1.0f, "%.3f", 2.0f);

		label = "Smooth AO";
		label += suffix;
		ImGui::Checkbox(label.c_str(), &useBlur);
		if (useBlur)
		{
			label = "AO Spread";
			label += suffix;
			ImGui::SliderFloat(label.c_str(), &blurSpread, 0.0f, 20.0f, "%.3f");
		}

		PostProcessData::DrawEditorEnd();
	}
}
#endif

void PostProcessDataAO::SaveToJson(JSON_Object* nObj)
{
	PostProcessData::SaveToJson(nObj);
	//TODO save data here
	DEJson::WriteFloat(nObj, "RadiusAO", radiusAO);
	DEJson::WriteFloat(nObj, "GlowSpread", blurSpread);
	DEJson::WriteBool(nObj, "UseBlur", blurSpread);
	DEJson::WriteFloat(nObj, "Bias", bias);

}

void PostProcessDataAO::LoadFromJson(DEConfig& nObj)
{
	PostProcessData::LoadFromJson(nObj);
	//TODO load data here
	radiusAO = nObj.ReadFloat("RadiusAO");
	blurSpread = nObj.ReadFloat("GlowSpread");
	useBlur = nObj.ReadBool("UseBlur");
	bias = nObj.ReadFloat("Bias");
}


PostProcessDataBloom::PostProcessDataBloom() : PostProcessData(POSTPROCESS_DATA_TYPE::BLOOM, "Bloom"),
brightThreshold(0.7f),
brightnessIntensity(1.0f),
blurSpread(0.0f),
smoothMask(false)
{
}

PostProcessDataBloom::~PostProcessDataBloom()
{
}

#ifndef STANDALONE
void PostProcessDataBloom::DrawEditor()
{
	const std::string suffix = "##Bloom";
	std::string label = name + suffix;
	if (ImGui::CollapsingHeader(label.c_str()))
	{
		PostProcessData::DrawEditorStart(suffix);

		//TODO drawEditorHere

		label = "Bright Threshold";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &brightThreshold, 0.0f, 250.0f, "%.3f", 2.0f);
		label = "Brightness Intensity";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &brightnessIntensity, 0.0f, 10.0f, "%.3f");
		label = "Glow Blur Spread";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &blurSpread, 0.0f, 20.0f, "%.3f");
		label = "Use Smooth Glow";
		label += suffix;
		ImGui::Checkbox(label.c_str(), &smoothMask);

		PostProcessData::DrawEditorEnd();
	}
}
#endif

void PostProcessDataBloom::SaveToJson(JSON_Object* nObj)
{
	PostProcessData::SaveToJson(nObj);
	//TODO save data here
	DEJson::WriteFloat(nObj, "BrightThreshold", brightThreshold);
	DEJson::WriteFloat(nObj, "BrightnessIntensity", brightnessIntensity);
	DEJson::WriteFloat(nObj, "GlowBlurSpread", blurSpread);
	DEJson::WriteBool(nObj, "SmoothGlow", smoothMask);

}

void PostProcessDataBloom::LoadFromJson(DEConfig& nObj)
{
	PostProcessData::LoadFromJson(nObj);
	//TODO load data here
	brightThreshold = nObj.ReadFloat("BrightThreshold");
	brightnessIntensity = nObj.ReadFloat("BrightnessIntensity");
	blurSpread = nObj.ReadFloat("GlowBlurSpread");
	smoothMask = nObj.ReadBool("SmoothGlow");
}

PostProcessDataToneMapping::PostProcessDataToneMapping() : PostProcessData(POSTPROCESS_DATA_TYPE::TONE_MAPPING, "Tone Mapping"),
exposure(1.0f), gamma(2.2f)
{
}

PostProcessDataToneMapping::~PostProcessDataToneMapping()
{
}

#ifndef STANDALONE
void PostProcessDataToneMapping::DrawEditor()
{
	const std::string suffix = "##Tone Mapping";
	std::string label = name + suffix;
	if (ImGui::CollapsingHeader(label.c_str()))
	{
		PostProcessData::DrawEditorStart(suffix);

		//TODO drawEditorHere

		label = "Exposure";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &exposure, 0.0f, 20.0f, "%.3f");
		label = "Gamma";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &gamma, 0.25f, 4.0f, "%.3f");

		PostProcessData::DrawEditorEnd();
	}
}
#endif

void PostProcessDataToneMapping::SaveToJson(JSON_Object* nObj)
{
	PostProcessData::SaveToJson(nObj);
	//TODO save data here
	DEJson::WriteFloat(nObj, "Exposure", exposure);
	DEJson::WriteFloat(nObj, "Gamma", gamma);


}

void PostProcessDataToneMapping::LoadFromJson(DEConfig& nObj)
{
	PostProcessData::LoadFromJson(nObj);
	//TODO load data here
	exposure = nObj.ReadFloat("Exposure");
	gamma = nObj.ReadFloat("Gamma");
}


PostProcessDataVignette::PostProcessDataVignette() : PostProcessData(POSTPROCESS_DATA_TYPE::VIGNETTE, "Vignette"),
intensity(15.0f), extend(0.25f), tint(1.0f, 1.0f, 1.0f, 0.0f), minMaxRadius(0.4f, 0.7f), mode(VIGNETTE_MODE::RECTANGULAR)
{
}

PostProcessDataVignette::~PostProcessDataVignette()
{
}

#ifndef STANDALONE
void PostProcessDataVignette::DrawEditor()
{
	const std::string suffix = "##Vignette";
	std::string label = name + suffix;

	if (ImGui::CollapsingHeader(label.c_str()))
	{
		PostProcessData::DrawEditorStart(suffix);

		//TODO drawEditorHere
		label = "tint";
		label += suffix;
		ImGui::ColorPicker4(label.c_str(), &tint.x, ImGuiColorEditFlags_AlphaBar);
		ImGui::Spacing();

		//=========================================== Combo
		label = "VignetteMode";
		label += suffix;
		const std::string modes[2] = { "RECTANGULAR","CIRCULAR" };

		if (ImGui::BeginCombo(label.c_str(), modes[(int)mode].c_str()))
		{
			for (int i = 0; i < (int)VIGNETTE_MODE::NONE; ++i)
			{
				bool isSelected = ((int)mode == i) ? true : false;
				if (ImGui::Selectable(modes[i].c_str(), isSelected))
				{
					mode = (VIGNETTE_MODE)i;
				}
				// Set the initial focus when opening the combo (scrolling + keyboard navigation focus)
				if (isSelected)
					ImGui::SetItemDefaultFocus();

			}
			ImGui::EndCombo();
		}
		//=========================================== end combo



		ImGui::Indent();
		if (mode == VIGNETTE_MODE::RECTANGULAR)
		{
			label = "intensity";
			label += suffix;
			ImGui::SliderFloat(label.c_str(), &intensity, 0.0f, 15.0f, "%.3f", 2.0f);
			label = "extend";
			label += suffix;
			ImGui::SliderFloat(label.c_str(), &extend, 0.0f, 75.0f, "%.3f", 1.5f);

		}
		else
		{
			label = "minMaxRadius";
			label += suffix;
			ImGui::SliderFloat2(label.c_str(), &minMaxRadius.x, 0.0f, 5.0f, "%.3f", 1.25f);

		}
		ImGui::Unindent();

		PostProcessData::DrawEditorEnd();
	}
}
#endif

void PostProcessDataVignette::SaveToJson(JSON_Object* nObj)
{
	PostProcessData::SaveToJson(nObj);
	//TODO save data here
	DEJson::WriteInt(nObj, "Mode", (int)mode);
	DEJson::WriteVector4(nObj, "Tint", &tint.x);
	DEJson::WriteFloat(nObj, "Intensity", intensity);
	DEJson::WriteFloat(nObj, "Extend", extend);
	DEJson::WriteVector2(nObj, "MinMaxRadius", &minMaxRadius.x);

}

void PostProcessDataVignette::LoadFromJson(DEConfig& nObj)
{
	PostProcessData::LoadFromJson(nObj);
	//TODO load data here
	mode = (VIGNETTE_MODE)nObj.ReadInt("Mode");
	tint = nObj.ReadVector4("Tint");
	intensity = nObj.ReadFloat("Intensity");
	extend = nObj.ReadFloat("Extend");
	minMaxRadius = nObj.ReadVector2("MinMaxRadius");

}

ResourcePostProcess::ResourcePostProcess(unsigned int _uid) : Resource(_uid, Resource::Type::POSTPROCESS)
{
	Init();
}

ResourcePostProcess::~ResourcePostProcess()
{
	CleanUp();
}

bool ResourcePostProcess::LoadToMemory()
{
	CleanUp();
	Init();

	//Load file to buffer [DONE]
	JSON_Value* file = json_parse_file(this->libraryFile.c_str());
	DEConfig root_object(json_value_get_object(file));

	strcpy(name, root_object.ReadString("name"));

	//TODO
	//load here
	JSON_Array* EffectData = root_object.ReadArray("EffectData");
	DEConfig val;
	for (size_t i = 0; i < json_array_get_count(EffectData); i++)
	{
		val.nObj = json_array_get_object(EffectData, i);

		postProcessData[i]->LoadFromJson(val);
	}

	json_value_free(file);

	return false;
}

bool ResourcePostProcess::UnloadFromMemory()
{

	//TODO: Unload resources (uniform and attributes) by reference count 
	CleanUp();
	return true;
}

PostProcessData* ResourcePostProcess::GetDataOfType(POSTPROCESS_DATA_TYPE type)
{
	if (type == POSTPROCESS_DATA_TYPE::NONE)
		return nullptr;

	for (int i = 0; i < postProcessData.size(); ++i)
	{
		if (postProcessData[i]->GetType() == type)
		{
			return postProcessData[i];
		}
	}
	return nullptr;
}

void ResourcePostProcess::Init()
{
	//TODO add new resource data components here
	postProcessData.push_back(dynamic_cast<PostProcessData*>(new PostProcessDataAO()));
	postProcessData.push_back(dynamic_cast<PostProcessData*>(new PostProcessDataBloom()));
	postProcessData.push_back(dynamic_cast<PostProcessData*>(new PostProcessDataToneMapping()));
	postProcessData.push_back(dynamic_cast<PostProcessData*>(new PostProcessDataVignette()));

}

void ResourcePostProcess::CleanUp()
{
	for (int i = 0; i < postProcessData.size(); ++i)
	{
		if (postProcessData[i] != nullptr)
		{
			delete(postProcessData[i]);
			postProcessData[i] = nullptr;
		}
	}
	postProcessData.clear();
}

#ifndef STANDALONE
void ResourcePostProcess::DrawEditor(std::string suffix)
{
	std::string label = "Current Profile: ";
	label += name;
	label += suffix;
	if (ImGui::CollapsingHeader(label.c_str(), ImGuiTreeNodeFlags_Leaf))
	{
		ImGui::Spacing();
		ImGui::Spacing();
		ImGui::Indent();

		for (int i = 0; i < postProcessData.size(); ++i)
		{
			postProcessData[i]->DrawEditor();
		}

		ImGui::Spacing();
		ImGui::Spacing();
		ImGui::Unindent();

		label = "SaveProfile";
		label += suffix;
		if (ImGui::Button(label.c_str()))
		{
			char* fileBuffer = nullptr;
			PostProcessImporter::Save(this, &fileBuffer);
			RELEASE_ARRAY(fileBuffer);
			fileBuffer = nullptr;
		}
	}
}
#endif // !STANDALONE
void ResourcePostProcess::SaveToJson(JSON_Object* nObj)
{
	//Effects=====================================================

	JSON_Value* reArray = json_value_init_array();
	JSON_Array* jsArray = json_value_get_array(reArray);

	for (int i = 0; i < postProcessData.size(); ++i)
	{
		JSON_Value* nVal = json_value_init_object();
		JSON_Object* nObj = json_value_get_object(nVal);

		postProcessData[i]->SaveToJson(nObj);

		json_array_append_value(jsArray, nVal);
	}
	json_object_set_value(nObj, "EffectData", reArray);

}

