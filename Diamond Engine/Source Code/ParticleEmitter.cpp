#include "ParticleEmitter.h"
#include "Globals.h"

#include "PE_Spawn_Area.h"
#include "PE_Force_Over_Lifetime.h"
#include "PE_Rotate_Over_Lifetime.h"
#include "PE_Size_Over_Lifetime.h"
#include "PE_Color_Over_Lifetime.h"
#include "PE_Velocity_Over_Lifetime.h"

#include <string>

#include "Application.h"
#include "MO_ResourceManager.h"
#include "MO_Renderer3D.h"

#include "RE_Texture.h"

#include "CO_Camera.h"
#include "CO_Transform.h"

#include "OpenGL.h"
#include "ImGui/imgui.h"
#include "MathGeoLib/include/Math/float4x4.h"

#include <string>

Emitter::Emitter() :
	toDelete(false),
	texture(nullptr),
	particlesPerSec(0.0f),
	lastParticeTime(0),
	particlesColor(1.0f, 1.0f, 1.0f, 1.0f),
	myParticles(),
	myEffects(),
	objTransform(nullptr),
	lastUsedParticle(0)
{
	memset(particlesLifeTime, 0.1f, sizeof(particlesLifeTime));
	memset(particlesSpeed, 0.0f, sizeof(particlesSpeed));
	memset(particlesSize, 1.0f, sizeof(particlesSize));


	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &instanceVBO);
	glGenBuffers(1, &vertexVBO);

	glBindVertexArray(VAO);

	glBindBuffer(GL_ARRAY_BUFFER, vertexVBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(particleVertices), particleVertices, GL_STATIC_DRAW);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), 0);

	glBindBuffer(GL_ARRAY_BUFFER, instanceVBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(float) * MAX_PARTICLES * INSTANCE_DATA_LENGHT, NULL, GL_DYNAMIC_DRAW);
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(1, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), 0);
	glVertexAttribDivisor(1, 1);
	glEnableVertexAttribArray(2);
	glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), (void*)(4 * sizeof(float)));
	glVertexAttribDivisor(2, 1);
	glEnableVertexAttribArray(3);
	glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), (void*)(8 * sizeof(float)));
	glVertexAttribDivisor(3, 1);
	glEnableVertexAttribArray(4);
	glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), (void*)(12 * sizeof(float)));
	glVertexAttribDivisor(4, 1);

	glDisableVertexAttribArray(0);
	glDisableVertexAttribArray(1);
	glDisableVertexAttribArray(2);
	glDisableVertexAttribArray(3);
	glDisableVertexAttribArray(4);

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);

	CalculatePoolSize();
}

Emitter::~Emitter()
{
	if (texture != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(texture->GetUID());
		texture = nullptr;
	}
	VAO = 0u;

	for (int i = myEffects.size() - 1; i >= 0; --i)
	{
		if (myEffects[i] != nullptr)
		{
			delete(myEffects[i]);
			myEffects[i] = nullptr;
		}
		myEffects.erase(myEffects.begin() + i);
	}
	myEffects.clear();
	myParticles.clear();
	objTransform = nullptr;
}


void Emitter::Update(float dt, bool systemActive)
{
	if (systemActive)
		ThrowParticles(dt);

	for (int i = 0; i < myParticles.size(); ++i)
	{
		if (myParticles[i].currentLifetime >= 0.0f)
		{
			myParticles[i].currentLifetime -= dt;
			for (int j = 0; j < myEffects.size(); ++j)
			{
				myEffects[j]->Update(myParticles[i], dt);
			}

			myParticles[i].speed += myParticles[i].accel * dt;
			myParticles[i].pos += myParticles[i].speed * dt;
		}
	}
}

void Emitter::Draw(unsigned int shaderId)
{
	if (myParticles.empty() == true)
		return;

	if (texture != nullptr)
		glBindTexture(GL_TEXTURE_2D, texture->textureID);

	GLint modelLoc = glGetUniformLocation(shaderId, "view");
	glUniformMatrix4fv(modelLoc, 1, GL_FALSE, EngineExternal->moduleRenderer3D->activeRenderCamera->ViewMatrixOpenGL().ptr());

	modelLoc = glGetUniformLocation(shaderId, "projection");
	glUniformMatrix4fv(modelLoc, 1, GL_FALSE, EngineExternal->moduleRenderer3D->activeRenderCamera->ProjectionMatrixOpenGL().ptr());

	int particlesAlive = 0;
	std::vector<float> vboInfo;
	vboInfo.reserve(sizeof(float) * myParticles.size() * 16);

	int particlesCount = myParticles.size();
	for (int i = 0; i < particlesCount; ++i)	//Need to order particles
	{
		if (myParticles[i].currentLifetime > 0)
		{
			float4x4 matrix = float4x4::FromTRS(myParticles[i].pos, Quat::FromEulerXYZ(0, 0, myParticles[i].rotation), float3(1, 1, 1)).Transposed();
			vboInfo.push_back(matrix[0][0]);
			vboInfo.push_back(matrix[0][1]);
			vboInfo.push_back(matrix[0][2]);
			vboInfo.push_back(matrix[0][3]);
			vboInfo.push_back(matrix[1][0]);
			vboInfo.push_back(matrix[1][1]);
			vboInfo.push_back(matrix[1][2]);
			vboInfo.push_back(matrix[1][3]);
			vboInfo.push_back(matrix[2][0]);
			vboInfo.push_back(matrix[2][1]);
			vboInfo.push_back(matrix[2][2]);
			vboInfo.push_back(matrix[2][3]);
			vboInfo.push_back(matrix[3][0]);
			vboInfo.push_back(matrix[3][1]);
			vboInfo.push_back(matrix[3][2]);
			vboInfo.push_back(matrix[3][3]);

			particlesAlive++;
		}
	}

	if (vboInfo.empty())
		return;

	glBindVertexArray(VAO);

	//glBindBuffer(GL_ARRAY_BUFFER, vertexVBO);
	glEnableVertexAttribArray(0);
	//glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), 0);

	glBindBuffer(GL_ARRAY_BUFFER, instanceVBO);
	glBufferSubData(GL_ARRAY_BUFFER, 0, vboInfo.size() * sizeof(float), &vboInfo[0]);

	//glBindBuffer(GL_ARRAY_BUFFER, instanceVBO);
	//glBufferData(GL_ARRAY_BUFFER, sizeof(float) * MAX_PARTICLES * INSTANCE_DATA_LENGHT, NULL, GL_DYNAMIC_DRAW);
	glEnableVertexAttribArray(1);
	//glVertexAttribPointer(1, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), 0);
	//glVertexAttribDivisor(1, 1);
	glEnableVertexAttribArray(2);
	//glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), (void*)(4 * sizeof(float)));
	//glVertexAttribDivisor(2, 1);
	glEnableVertexAttribArray(3);
	//glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), (void*)(8 * sizeof(float)));
	//glVertexAttribDivisor(3, 1);
	glEnableVertexAttribArray(4);
	//glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, 16 * sizeof(float), (void*)(12 * sizeof(float)));
	//glVertexAttribDivisor(4, 1);

	glDrawArraysInstanced(GL_TRIANGLES, 0, 6, particlesAlive);


	glBindBuffer(GL_ARRAY_BUFFER, 0);
}


#ifndef STANDALONE
void Emitter::OnEditor(int emitterIndex)
{
	std::string guiName = "Emitter";
	std::string suffixLabel = "##";
	suffixLabel += emitterIndex;

	if (ImGui::CollapsingHeader(guiName.c_str(), ImGuiTreeNodeFlags_DefaultOpen))
	{
		bool poolToEdit = false;

		guiName = "Particle Lifetime ##" + suffixLabel;
		if (ImGui::DragFloatRange2("Lifetime", &particlesLifeTime[0], &particlesLifeTime[1], 0.25f, 0.1f, 100.0f, "Min: %.1f", "Max: %.1f"))
		{
			poolToEdit = true;
		}

		guiName = "Particles per Second ##" + suffixLabel;
		if (ImGui::DragFloat(guiName.c_str(), &particlesPerSec))
		{
			poolToEdit = true;
		}

		if (poolToEdit)
		{
			CalculatePoolSize();
		}


		for (int i = (int)PARTICLE_EFFECT_TYPE::NONE + 1; i < (int)PARTICLE_EFFECT_TYPE::MAX; ++i)
		{
			int index = DoesEffectExist((PARTICLE_EFFECT_TYPE)i);
			if (index != -1)
			{
				myEffects[index]->OnEditor(emitterIndex);
			}
			else
			{
				guiName = (ParticleEffectEnumToString((PARTICLE_EFFECT_TYPE)i)) + suffixLabel;
				if (ImGui::Button(guiName.c_str()))
				{
					CreateEffect((PARTICLE_EFFECT_TYPE)i);
				}
			}
		}

		if (texture != nullptr)
			ImGui::Image((ImTextureID)texture->textureID, ImVec2(64, 64), ImVec2(0, 1), ImVec2(1, 0));

		else
			ImGui::Image((ImTextureID)0, ImVec2(64, 64), ImVec2(0, 1), ImVec2(1, 0));

		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload * payload = ImGui::AcceptDragDropPayload("_TEXTURE"))
			{
				//Drop asset from Asset window to scene window
				std::string* metaFileDrop = (std::string*)payload->Data;

				if (texture != nullptr)
					EngineExternal->moduleResources->UnloadResource(texture->GetUID());

				std::string libraryName = EngineExternal->moduleResources->LibraryFromMeta(metaFileDrop->c_str());

				texture = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(EngineExternal->moduleResources->GetMetaUID(metaFileDrop->c_str()), libraryName.c_str()));
				LOG(LogType::L_WARNING, "File %s loaded to scene", (*metaFileDrop).c_str());
			}
			ImGui::EndDragDropTarget();
		}
		ImGui::Separator();
		ImGui::Spacing();
	}
}
#endif // !STANDALONE

void Emitter::CalculatePoolSize()
{
	int poolSize = ceilf(particlesPerSec * max(particlesLifeTime[0], particlesLifeTime[1]));

	if (poolSize < 0)
		poolSize = 0;

	SetPoolSize(poolSize);
}

void Emitter::SetPoolSize(unsigned int poolSize)
{
	myParticles.resize(poolSize);
}

void Emitter::CreateParticles(unsigned int particlesToAdd)
{
	//TODO this method is too similar to throw particles, consider merging these into a single method

	unsigned int lastIndex = myParticles.size();
	myParticles.resize(lastIndex + particlesToAdd);
	float3 startingPos = objTransform->globalTransform.TranslatePart();

	for (int i = lastIndex; i < myParticles.size(); ++i)
	{
		PrepareParticleToSpawn(myParticles[i], startingPos);

		for (int j = 0; j < myEffects.size(); ++j)
		{
			myEffects[j]->Spawn(myParticles[i]);
		}
	}


}

void Emitter::ThrowParticles(float dt)
{
	//find particles to spawn this frame
	float timeSinceLastThrow = dt + lastParticeTime;

	float numberOfParticlesToSpawnF = timeSinceLastThrow * particlesPerSec;
	int numberOfParticlesToSpawn = numberOfParticlesToSpawnF;
	float extraParticle = numberOfParticlesToSpawnF - numberOfParticlesToSpawn;

	if (particlesPerSec > 0.0f)
	{
		lastParticeTime = extraParticle / particlesPerSec; //TODO should we optimize this division?
	}
	else
	{
		lastParticeTime = 0.0f;
	}


	//LOG(LogType::L_NORMAL, "PARTICLES SPAWNED THIS FRAME %i", numberOfParticlesToSpawn);

	float3 startingPos = objTransform->globalTransform.TranslatePart();
	//spawn particles


	for (int i = 0; i < numberOfParticlesToSpawn; ++i)
	{
		int unusedIndex= FindUnusedParticle();
		PrepareParticleToSpawn(myParticles[unusedIndex], startingPos);

		for (int j = 0; j < myEffects.size(); ++j)
		{
			myEffects[j]->Spawn(myParticles[unusedIndex]);
		}
	}
}

int Emitter::DoesEffectExist(PARTICLE_EFFECT_TYPE type)
{
	for (int i = 0; i < myEffects.size(); ++i)
	{
		if (myEffects[i] != nullptr && myEffects[i]->type == type)
		{
			return i;
		}
	}

	return -1;
}

std::string Emitter::ParticleEffectEnumToString(PARTICLE_EFFECT_TYPE type)
{
	std::string ret = "";
	switch (type)
	{
	case PARTICLE_EFFECT_TYPE::NONE:
		break;
	case PARTICLE_EFFECT_TYPE::AREA_SPAWN:
		ret = "Spawn Shape Effect";
		break;
	case PARTICLE_EFFECT_TYPE::FORCE_OVER_LIFETIME:
		ret = "Force Over Lifetime Effect";
		break;
	case PARTICLE_EFFECT_TYPE::ROTATE_OVER_LIFETIME:
		ret = "Rotate Over Lifetime Effect";
		break;
	case PARTICLE_EFFECT_TYPE::SIZE_OVER_LIFETIME:
		ret = "Size Over Lifetime Effect";
		break;
	case PARTICLE_EFFECT_TYPE::COLOR_OVER_LIFETIME:
		ret = "Color Over Lifetime Effect";
		break;
	case PARTICLE_EFFECT_TYPE::VELOCITY_OVER_LIFETIME:
		ret = "Velocity Over Lifetime Effect";
		break;
	case PARTICLE_EFFECT_TYPE::MAX:
		break;
	default:
		break;
	}
	return ret;
}

void Emitter::CreateEffect(PARTICLE_EFFECT_TYPE type)
{
	ParticleEffect* newEffect = nullptr;

	switch (type)
	{
	case PARTICLE_EFFECT_TYPE::NONE:
		break;
	case PARTICLE_EFFECT_TYPE::AREA_SPAWN:
		newEffect = new PE_SpawnArea();
		break;
	case PARTICLE_EFFECT_TYPE::FORCE_OVER_LIFETIME:
		newEffect = new PE_ForceOverLifetime();
		break;
	case PARTICLE_EFFECT_TYPE::ROTATE_OVER_LIFETIME:
		newEffect = new PE_RotateOverLifetime();
		break;
	case PARTICLE_EFFECT_TYPE::SIZE_OVER_LIFETIME:
		newEffect = new PE_SizeOverLifetime();
		break;
	case PARTICLE_EFFECT_TYPE::COLOR_OVER_LIFETIME:
		newEffect = new PE_ColorOverLifetime();
		break;
	case PARTICLE_EFFECT_TYPE::VELOCITY_OVER_LIFETIME:
		newEffect = new PE_VelocityOverLifetime();
		break;
	case PARTICLE_EFFECT_TYPE::MAX:
		break;
	default:
		break;
	}

	if (newEffect == nullptr)
	{
		LOG(LogType::L_WARNING, "Couldn't create a particle effect of type %i", (int)type);
		return;
	}


	if (myEffects.empty() || myEffects[myEffects.size() - 1]->type < type) //if vector is empty or type greater than the last element
	{
		myEffects.push_back(newEffect);
	}
	else if (myEffects[0]->type > type) //if type goes before first element in the vector
	{
		myEffects.insert(myEffects.begin(), newEffect);
	}
	else//general case
	{
		for (int i = 1; i < myEffects.size(); ++i)
		{
			if (type > myEffects[i - 1]->type && type < myEffects[i]->type)
			{
				myEffects.insert(myEffects.begin() + i, newEffect);
			}
		}
	}
}

void Emitter::PrepareParticleToSpawn(Particle& p, float3& startingPos)
{
	//we reset all particle variables
	p.accel = { 0.0f,0.0f,0.0f };
	p.rotation = 0.0f;
	p.rotationSpeed = 0.0f;
	p.speed = { 0.0f,0.0f,0.0f };
	p.maxLifetime = p.currentLifetime = EngineExternal->GetRandomFloat(particlesLifeTime[0], particlesLifeTime[1]);
	p.size = EngineExternal->GetRandomFloat(particlesSize[0], particlesSize[1]);
	p.color = particlesColor;
	p.pos = startingPos; //particles start at the center of the obj (but spawn methods modify this pos for the time being)

}

int Emitter::FindUnusedParticle()
{
	// Finds a Particle in ParticlesContainer which isn't used yet.
// (i.e. life < 0);
	int maxParticles = myParticles.size();

	for (int i = lastUsedParticle; i < maxParticles; ++i)
	{
		if (myParticles[i].currentLifetime < 0.0f) {
			lastUsedParticle = i;
			return i;
		}
	}

	for (int i = 0; i < lastUsedParticle; ++i)
	{
		if (myParticles[i].currentLifetime < 0.0f)
		{
			lastUsedParticle = i;
			return i;
		}
	}

	return 0; // All particles are taken, override the first one
}

void Emitter::AssignTransform(C_Transform* objTransform)
{
	this->objTransform = objTransform;
}
