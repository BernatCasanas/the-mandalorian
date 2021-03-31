#pragma once

#include "CO_Image2D.h"
#include "RE_Texture.h"
#include "CO_Material.h"
#include "MO_MonoManager.h"
#include "Application.h"
#include "CS_Transform_Bindings.h"
#include "GameObject.h"
#include "RE_Material.h"

void SwapTwoImages(MonoObject* obj, MonoObject* other_image)
{
	if (EngineExternal == nullptr)
		return;




	C_Image2D* workImag = DECS_CompToComp<C_Image2D*>(obj);

	if (workImag == nullptr)
		return;

	C_Image2D* other_image_module = static_cast<C_Image2D*>(EngineExternal->moduleMono->GameObject_From_CSGO(other_image)->GetComponent(Component::TYPE::IMAGE_2D));//DECS_CompToComp<C_Image2D*>(other_image);

	if (other_image == nullptr)
		return;

	int texture_uid = workImag->GetTexture()->textureID;
	std::string library_path = workImag->GetTexture()->GetLibraryPath();
	workImag->SetTexture(other_image_module->GetTexture());
	other_image_module->SetTexture(texture_uid, library_path.c_str());

}

void AssignLibrary2DTexture(MonoObject* obj, int _id)
{

	C_Image2D* workImag = DECS_CompToComp<C_Image2D*>(obj);
	ResourceTexture* resourceTex = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(_id, Resource::Type::TEXTURE));
	if (resourceTex != nullptr) {
		workImag->SetTexture(resourceTex);
	}

}