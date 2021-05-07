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
radiusAO(1.0f), blurSpread(0)
{
}

PostProcessDataAO::~PostProcessDataAO()
{
}

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
		ImGui::SliderFloat(label.c_str(), &radiusAO, 0.0f, 100.0f, "%.3f", 2.0f);
		label = "Glow Spread";
		label += suffix;
		ImGui::SliderFloat(label.c_str(), &blurSpread, 0.0f, 20.0f, "%.3f");

		PostProcessData::DrawEditorEnd();
	}
}

void PostProcessDataAO::SaveToJson(JSON_Object* nObj)
{
	PostProcessData::SaveToJson(nObj);
	//TODO save data here
	DEJson::WriteFloat(nObj, "RadiusAO", radiusAO);
	DEJson::WriteFloat(nObj, "GlowSpread", blurSpread);

}

void PostProcessDataAO::LoadFromJson(DEConfig& nObj)
{
	PostProcessData::LoadFromJson(nObj);
	//TODO load data here
	radiusAO = nObj.ReadFloat("RadiusAO");
	blurSpread = nObj.ReadFloat("GlowSpread");

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

		/*if (val.ReadInt("type") == GL_SAMPLER_2D && val.ReadInt("value") != 0)
		{
			for (size_t k = 0; k < postProcessData.size(); ++k)
			{
				if (strcmp(val.ReadString("name"), postProcessData[k]->name) == 0)
					postProcessData[k].data.textureValue = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(val.ReadInt("value"), Resource::Type::TEXTURE));
			}
		}
		else if (val.ReadInt("type") == GL_FLOAT) {
			for (size_t k = 0; k < postProcessData.size(); ++k)
			{
				if (strcmp(val.ReadString("name"), postProcessData[k].name) == 0)
					postProcessData[k].data.floatValue = val.ReadFloat("value");
			}
		}*/
	}

	json_value_free(file);

	return false;
}

bool ResourcePostProcess::UnloadFromMemory()
{

	//TODO: Unload resources (uniform and attributes) by reference count 

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
	postProcessData.push_back(dynamic_cast<PostProcessData*>(new PostProcessDataAO()));
	postProcessData.push_back(dynamic_cast<PostProcessData*>(new PostProcessDataBloom()));

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

