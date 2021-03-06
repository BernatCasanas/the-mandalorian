#pragma once
#include "Component.h"

class ResourceTexture;
class C_Script;

enum class CHECKBOXSTATE {
	CHECKBOXACTIVE,
	CHECKBOXACTIVEHOVERED,
	CHECKBOXACTIVEPRESSED,
	CHECKBOXUNACTIVE,
	CHECKBOXUNACTIVEHOVERED,
	CHECKBOXUNACTIVEPRESSED
};

class C_Checkbox :public Component {
public:
	C_Checkbox(GameObject* gameObject);
	~C_Checkbox() override;

	void Update() override;

	void PressCheckbox();
	void UnpressCheckbox();

	void ChangeTexture(CHECKBOXSTATE new_num_sprite);

#ifndef STANDALONE
	void ChangeSprite(CHECKBOXSTATE num_sprite, ResourceTexture* sprite);
	void ChangeScript(C_Script* script);
	bool OnEditor() override;
#endif // !STANDALONE

private:
	ResourceTexture* sprite_checkbox_active;
	ResourceTexture* sprite_checkbox_active_hovered;
	ResourceTexture* sprite_checkbox_active_pressed;
	ResourceTexture* sprite_checkbox_unactive;
	ResourceTexture* sprite_checkbox_unactive_hovered;
	ResourceTexture* sprite_checkbox_unactive_pressed;
	CHECKBOXSTATE num_sprite_used;
	C_Script* script;
	bool checkbox_active;
};