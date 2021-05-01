#include "CO_AreaLight.h"

#include "Application.h"
#include"MO_Renderer3D.h"

#include"GameObject.h"
#include"CO_Transform.h"

#include "RE_Material.h"
#include "RE_Shader.h"

#include "ImGui/imgui.h"


C_AreaLight::C_AreaLight(GameObject* gameObject) : Component(gameObject),
	spaceMatrixOpenGL(float4x4::identity),
	lightColor(float3::one),
	ambientLightColor(float3::one),
	lightIntensity(1.0f),
	specularValue(1.0f)
{
	name = "Area Light";

	//Add light
	EngineExternal->moduleRenderer3D->AddAreaLight(this);
}


C_AreaLight::~C_AreaLight()
{
	//Remove light
	EngineExternal->moduleRenderer3D->RemoveAreaLight(this);
}


void C_AreaLight::Update()
{

}


#ifndef STANDALONE
bool C_AreaLight::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::ColorPicker3("Color", lightColor.ptr());

		ImGui::NewLine();

		ImGui::ColorPicker3("Ambient color", ambientLightColor.ptr());

		ImGui::NewLine();

		ImGui::DragFloat("Light intensity", &lightIntensity, 0.05, 0.0f);
		ImGui::DragFloat("Specular value", &specularValue, 0.1, 0.0f);

		ImGui::NewLine();

		if (ImGui::Button("Set light source", ImVec2(0, 50)))
			EngineExternal->moduleRenderer3D->AddAreaLight(this);

		return true;
	}

	return false;
}


void C_AreaLight::DebugDraw()
{
	glBegin(GL_TRIANGLES);
	for (size_t i = 0; i < 12; i += 2)
	{
		float4 position;
		position.x = arrayAreaLightVAO[i];
		position.y = arrayAreaLightVAO[i + 1];
		position.z = 0.0f;
		position.w = 1.0f;

		position += gameObject->transform->GetCurrentGlobalMatrix() * position;

		glColor3fv(color);
		glVertex3fv(position.ptr());
	}
	glEnd();
}
#endif // !STANDALONE

void C_AreaLight::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	DEConfig data(nObj);
	data.WriteVector3("lightColor", lightColor.ptr());
	data.WriteVector3("ambientLightColor", ambientLightColor.ptr());
	data.WriteFloat("lightIntensity", lightIntensity);
	data.WriteFloat("specularValue", specularValue);
}


void C_AreaLight::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	lightColor = nObj.ReadVector3("lightColor");
	ambientLightColor = nObj.ReadVector3("ambientLightColor");
	lightIntensity = nObj.ReadFloat("lightIntensity");
	specularValue = nObj.ReadFloat("specularValue");
}


void C_AreaLight::StartPass()
{
	//Shadows thingy
}


void C_AreaLight::PushLightUniforms(ResourceMaterial* material, int lightNumber)
{
	char buffer[64];
	sprintf(buffer, "areaLightInfo[%i].lightSpaceMatrix", lightNumber);

	GLint modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniformMatrix4fv(modelLoc, 1, GL_FALSE, gameObject->transform->GetGlobalTransposed());

	sprintf(buffer, "areaLightInfo[%i].lightPos", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform3fv(modelLoc, 1, &gameObject->transform->position.x);

	sprintf(buffer, "areaLightInfo[%i].lightPosition", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform3fv(modelLoc, 1, &gameObject->transform->position.x);

	sprintf(buffer, "areaLightInfo[%i].lightForward", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	float3 forward = gameObject->transform->GetForward();
	glUniform3fv(modelLoc, 1, &forward.x);

	sprintf(buffer, "areaLightInfo[%i].lightColor", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform3fv(modelLoc, 1, &lightColor.x);

	sprintf(buffer, "areaLightInfo[%i].ambientLightColor", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform3fv(modelLoc, 1, &ambientLightColor.x);

	sprintf(buffer, "areaLightInfo[%i].lightIntensity", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform1f(modelLoc, lightIntensity);

	sprintf(buffer, "areaLightInfo[%i].specularValue", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform1f(modelLoc, specularValue);

	//glUniform1i(glGetUniformLocation(material->shader->shaderProgramID, shadowMap), used_textures);
	sprintf(buffer, "areaLightInfo[%i].calculateShadows", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform1i(modelLoc, false);

	/*if (calculateShadows == true)
	{
		glActiveTexture(GL_TEXTURE5);
		modelLoc = glGetUniformLocation(material->shader->shaderProgramID, "shadowMap");
		glUniform1i(modelLoc, 5);
		glBindTexture(GL_TEXTURE_2D, depthMap);
	}*/
}


void C_AreaLight::EndPass()
{
	//Shadows thingy
}


float3 C_AreaLight::GetPosition() const
{
	return gameObject->transform->position;
}