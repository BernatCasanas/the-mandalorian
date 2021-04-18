#pragma once
#include "Component.h"
class ResourceTexture;
class ResourceMaterial;

class C_StencilMaterial : public Component
{
public:
	C_StencilMaterial(GameObject* _gm);
	virtual ~C_StencilMaterial();

#ifndef STANDALONE
	bool OnEditor() override;
#endif // !STANDALONE

	void SetMaterial(ResourceMaterial* newMaterial);
	int GetTextureID();


	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

	bool viewWithCheckers;

	ResourceTexture* matTexture;
	ResourceMaterial* material;

};