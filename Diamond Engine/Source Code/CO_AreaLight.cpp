#include "CO_AreaLight.h"

#include "Application.h"
#include "MO_Renderer3D.h"
#include "MO_Window.h"

#include "GameObject.h"
#include "CO_Transform.h"
#include "CO_Camera.h"

#include "RE_Material.h"
#include "RE_Shader.h"

#include "ImGui/imgui.h"

const unsigned int SHADOW_WIDTH = 1024, SHADOW_HEIGHT = 1024;

C_AreaLight::C_AreaLight(GameObject* gameObject) : Component(gameObject),
	spaceMatrixOpenGL(float4x4::identity),
	depthCubemap(0u),
	depthCubemapFBO(0u),
	calculateShadows(true),
	lightColor(float3::one),
	ambientLightColor(float3::one),
	lightIntensity(1.0f),
	specularValue(1.0f),
	maxDistance(600.f),
	fadeDistance(50.f)
{
	name = "Area Light";

	//Prepare depth cubemap
	glGenFramebuffers(1, &depthCubemapFBO);

	glGenTextures(1, &depthCubemap);
	glBindTexture(GL_TEXTURE_CUBE_MAP, depthCubemap);

	for (unsigned int i = 0; i < 6; ++i)
		glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, GL_DEPTH_COMPONENT, SHADOW_WIDTH, SHADOW_HEIGHT, 0, GL_DEPTH_COMPONENT, GL_FLOAT, NULL);

	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);

	glBindFramebuffer(GL_FRAMEBUFFER, depthCubemapFBO);
	glFramebufferTexture(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, depthCubemap, 0);
	glDrawBuffer(GL_NONE);
	glReadBuffer(GL_NONE);
	glBindFramebuffer(GL_FRAMEBUFFER, 0);

	for (int i = 0; i < 6; ++i)
	{
		shadowTransforms[i].type = FrustumType::PerspectiveFrustum;
		shadowTransforms[i].nearPlaneDistance = 1.0f;
		shadowTransforms[i].farPlaneDistance = 500.0f;
		shadowTransforms[i].pos = float3::zero;
	}
	
	C_Camera::LookAt(shadowTransforms[0], float3(1.0,  0.0,  0.0));
	C_Camera::LookAt(shadowTransforms[1], float3(-1.0, 0.0,  0.0));
	C_Camera::LookAt(shadowTransforms[2], float3(0.0,  1.0,  0.0));
	C_Camera::LookAt(shadowTransforms[3], float3(0.0, -1.0,  0.0));
	C_Camera::LookAt(shadowTransforms[4], float3(0.0,  0.0,  1.0));
	C_Camera::LookAt(shadowTransforms[5], float3(0.0,  0.0,  -1.0));

	//Add light
	EngineExternal->moduleRenderer3D->AddAreaLight(this);
}


C_AreaLight::~C_AreaLight()
{
	if (depthCubemapFBO != 0)
		glDeleteFramebuffers(1, (GLuint*)&depthCubemapFBO);

	if (depthCubemap != 0)
		glDeleteTextures(1, (GLuint*)&depthCubemap);

	//Remove light
	EngineExternal->moduleRenderer3D->RemoveAreaLight(this);
}


void C_AreaLight::Update()
{
	for (int i = 0; i < 6; ++i)
		shadowTransforms[i].pos = gameObject->transform->globalTransform.TranslatePart();
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

		ImGui::DragFloat("Light fade distance", &fadeDistance, 0.1, 0.0f);
		ImGui::DragFloat("Light max distance", &maxDistance, 0.1, 0.0f);

		ImGui::NewLine();

		ImGui::Checkbox("Calculate shadows", &calculateShadows);

		return true;
	}

	return false;
}


void C_AreaLight::DebugDraw()
{
	glPushMatrix();
	glMultMatrixf(gameObject->transform->GetGlobalTransposed());

	glLineWidth(5.f);

	glBegin(GL_LINE_STRIP);
	for (size_t i = 0; i < 12; i += 2)
	{
		float4 position;
		position.x = arrayAreaLightVAO[i];
		position.y = arrayAreaLightVAO[i + 1];
		position.z = 0.0f;

		glColor3fv(color);
		glVertex3fv(position.ptr());
	}

	for (size_t i = 0; i < 12; i += 4)
	{
		float4 position;
		position.x = arrayAreaLightVAO[i];
		position.y = arrayAreaLightVAO[i + 1];
		position.z = 0.0f;

		glColor3fv(color);
		glVertex3fv(position.ptr());
	}

	float4 position;
	position.x = arrayAreaLightVAO[1];
	position.y = arrayAreaLightVAO[1];
	position.z = 0.0f;

	glColor3fv(color);
	glVertex3fv(position.ptr());

	glEnd();

	glPopMatrix();
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
	data.WriteFloat("fadeDistance", fadeDistance);
	data.WriteFloat("maxDistance", maxDistance);

	data.WriteBool("calculateShadows", calculateShadows);
}


void C_AreaLight::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	lightColor = nObj.ReadVector3("lightColor");
	ambientLightColor = nObj.ReadVector3("ambientLightColor");
	lightIntensity = nObj.ReadFloat("lightIntensity");
	specularValue = nObj.ReadFloat("specularValue");
	fadeDistance = nObj.ReadFloat("fadeDistance");
	maxDistance = nObj.ReadFloat("maxDistance");

	calculateShadows = nObj.ReadBool("calculateShadows");
}


void C_AreaLight::Enable()
{
	Component::Enable();

	EngineExternal->moduleRenderer3D->AddAreaLight(this);
}

void C_AreaLight::Disable()
{
	Component::Disable();

	EngineExternal->moduleRenderer3D->RemoveAreaLight(this);
}


void C_AreaLight::StartPass()
{
	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);

	glDisable(GL_CULL_FACE);
	glViewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);

	glBindFramebuffer(GL_FRAMEBUFFER, depthCubemapFBO);

	glClear(GL_DEPTH_BUFFER_BIT);

	//Bind depth shader
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

	sprintf(buffer, "areaLightInfo[%i].lFadeDistance", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform1f(modelLoc, fadeDistance);

	sprintf(buffer, "areaLightInfo[%i].lMaxDistance", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform1f(modelLoc, maxDistance);

	//glUniform1i(glGetUniformLocation(material->shader->shaderProgramID, shadowMap), used_textures);
	sprintf(buffer, "areaLightInfo[%i].calculateShadows", lightNumber);
	modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
	glUniform1i(modelLoc, calculateShadows);

	if (calculateShadows == true)
	{
		sprintf(buffer, "areaLightInfo[%i].shadowMap", lightNumber);
		glActiveTexture(GL_TEXTURE5);
		modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
		glUniform1i(modelLoc, 5);
		glBindTexture(GL_TEXTURE_CUBE_MAP, depthCubemap);
	}
}


void C_AreaLight::EndPass()
{
	glBindFramebuffer(GL_FRAMEBUFFER, 0);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glActiveTexture(GL_TEXTURE5);
	glBindTexture(GL_TEXTURE_CUBE_MAP, 0);

	glActiveTexture(GL_TEXTURE0);

	//Unbind depth shader

	glEnable(GL_CULL_FACE);
	glViewport(0, 0, EngineExternal->moduleWindow->s_width, EngineExternal->moduleWindow->s_height);
}


float3 C_AreaLight::GetPosition() const
{
	return gameObject->transform->position;
}


void C_AreaLight::ActivateLight()
{
	active = true;
	EngineExternal->moduleRenderer3D->AddAreaLight(this);
}


void C_AreaLight::DeactivateLight()
{
	active = false;
	EngineExternal->moduleRenderer3D->RemoveAreaLight(this);
}