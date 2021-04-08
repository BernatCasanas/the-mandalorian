#pragma once

#include"mono/metadata/object.h"
#include"CO_Button.h"
#include"CS_Transform_Bindings.h"
#include"RE_Texture.h"


void CS_ButtonChangeSprites(MonoObject* comp, int pressed, int hovered, int notHovered)
{

	C_Button* thisReference = DECS_CompToComp<C_Button*>(comp);

	if (thisReference == nullptr)
		return;

	if (pressed > 0) 
	{
		ResourceTexture* inputRes = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(pressed, Resource::Type::TEXTURE));
		thisReference->ChangeSprite(BUTTONSTATE::BUTTONPRESSED, inputRes);
	}

	if (hovered > 0) 
	{
		ResourceTexture* inputRes = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(hovered, Resource::Type::TEXTURE));
		thisReference->ChangeSprite(BUTTONSTATE::BUTTONHOVERED, inputRes);
	}

	if (notHovered > 0) 
	{
		ResourceTexture* inputRes = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(notHovered, Resource::Type::TEXTURE));
		thisReference->ChangeSprite(BUTTONSTATE::BUTTONUNHOVERED, inputRes);
	}
}


