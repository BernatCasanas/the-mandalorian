﻿#include "Globals.h"
#include "Application.h"
#include "ModuleCamera3D.h"
#include "ImGui/imgui.h"
#include "ModuleInput.h"
#include "ModuleWindow.h"
#include "M_Editor.h"
#include "GameObject.h"

#include"C_Transform.h"
#include"MathGeoLib/include/Math/float4.h"

ModuleCamera3D::ModuleCamera3D(Application* app, bool start_enabled) : Module(app, start_enabled), mouseSensitivity(0.50f), cameraSpeed(4.f), cameraMovement(0.f, 0.f, 0.f)
{
	editorCamera.camFrustrum.pos = float3(8.0f, 3.0f, 8.0f);
	Direction.Set(0, 1, 0, 0);
	//Reference = float3(0.0f, 0.0f, 0.0f);
}

ModuleCamera3D::~ModuleCamera3D()
{}

// -----------------------------------------------------------------
bool ModuleCamera3D::Start()
{
	LOG(LogType::L_NORMAL, "Setting up the camera");
	bool ret = true;

	editorCamera.LookAt(float3(0.f, 0.f, 0.f));
	editorCamera.camFrustrum.WorldMatrix().Decompose(float3(), Direction, float3());

	//LookAt(float3(0.f, 1.f, 0.f));

	return ret;
}

// -----------------------------------------------------------------
bool ModuleCamera3D::CleanUp()
{
	LOG(LogType::L_NORMAL, "Cleaning camera");

	return true;
}

void ModuleCamera3D::OnGUI()
{
	if (ImGui::CollapsingHeader("3D Input Settings", ImGuiTreeNodeFlags_DefaultOpen))
	{
		ImGui::TextWrapped("Camera Rotation Speed"); ImGui::SameLine(); ImGui::PushItemWidth(100.f); ImGui::DragFloat("##crs", &mouseSensitivity, 0.01f, 0.f, 999.f);
		ImGui::TextWrapped("Camera Movement Speed"); ImGui::SameLine(); ImGui::PushItemWidth(100.f); ImGui::DragFloat("##cms", &cameraSpeed, 0.01f, 0.f, 999.f);
	}
}

// -----------------------------------------------------------------
update_status ModuleCamera3D::Update(float dt)
{

	//ASK: This should be here to move camera with code but idk its expensive
	//TODO: Maybe dont do this every frame?
	if (App->moduleInput->GetMouseButton(SDL_BUTTON_RIGHT) == KEY_REPEAT) {
		float4x4 mat = editorCamera.camFrustrum.WorldMatrix();
		mat.SetRotatePart(Direction.Normalized());
		editorCamera.camFrustrum.SetWorldMatrix(mat.Float3x4Part());
	}


	editorCamera.Move(cameraMovement);
	cameraMovement = float3::zero;

	return UPDATE_CONTINUE;
}

void ModuleCamera3D::ProcessSceneKeyboard()
{
	cameraMovement.Set(0.f, 0.f, 0.f);
	const float dt = App->GetDT();

	float speed = cameraSpeed * dt;
	if (App->moduleInput->GetKey(SDL_SCANCODE_LSHIFT) == KEY_REPEAT)
		speed *= 2.f;

	//if (App->moduleInput->GetKey(SDL_SCANCODE_R) == KEY_REPEAT) cameraMovement.y += speed;
	//if (App->input->GetKey(SDL_SCANCODE_F) == KEY_REPEAT) newPos.y -= speed;

	if (App->moduleInput->GetKey(SDL_SCANCODE_W) == KEY_REPEAT) cameraMovement += editorCamera.camFrustrum.front * speed;
	if (App->moduleInput->GetKey(SDL_SCANCODE_S) == KEY_REPEAT) cameraMovement -= editorCamera.camFrustrum.front * speed;


	if (App->moduleInput->GetKey(SDL_SCANCODE_A) == KEY_REPEAT) cameraMovement -= editorCamera.camFrustrum.WorldRight() * speed;
	if (App->moduleInput->GetKey(SDL_SCANCODE_D) == KEY_REPEAT) cameraMovement += editorCamera.camFrustrum.WorldRight() * speed;

	if (App->moduleInput->GetMouseZ() != 0)
	{
		cameraMovement += editorCamera.camFrustrum.front * speed * App->moduleInput->GetMouseZ() * 250;
	}

	// Mouse motion ----------------

	if (App->moduleInput->GetMouseButton(SDL_BUTTON_RIGHT) == KEY_DOWN /*|| App->input->GetMouseButton(SDL_BUTTON_LEFT) == KEY_DOWN*/)
	{
		SDL_SetRelativeMouseMode(SDL_TRUE);
		SDL_WarpMouseInWindow(App->moduleWindow->window, SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
	}
	else if (App->moduleInput->GetMouseButton(SDL_BUTTON_RIGHT) == KEY_UP /*|| App->input->GetMouseButton(SDL_BUTTON_LEFT) == KEY_UP*/)
	{
		SDL_SetRelativeMouseMode(SDL_FALSE);
		SDL_WarpMouseInWindow(App->moduleWindow->window, SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
	}

	//ASK: Is this really the best way to rotate the camera? Maybe i should use a matrix
	//TODO: Camera rotation should not be affected by the program framerate
	if (App->moduleInput->GetMouseButton(SDL_BUTTON_RIGHT) == KEY_REPEAT)
	{
		FreeRotation(dt);
	}

	//Rotate around 0,0,0
	//ASK: Should i also include Right alt?
	//Maybe we could use quaternions?
	if (App->moduleInput->GetKey(SDL_SCANCODE_LALT) == KEY_REPEAT && App->moduleInput->GetMouseButton(SDL_BUTTON_LEFT) == KEY_REPEAT) 
	{
		float3 target(0.f, 0.f, 0.f);
		if (App->moduleEditor->GetSelectedGO() != nullptr)
		{
			float3 maTogl = App->moduleEditor->GetSelectedGO()->transform->globalTransform.TranslatePart();
			target.Set(maTogl.x, maTogl.y, maTogl.z);
		}
		//OrbitalRotation(target, dt);
	}


	if (App->moduleInput->GetKey(SDL_SCANCODE_F) == KEY_DOWN) 
	{
		float3 target(0.f, 0.f, 0.f);
		if (App->moduleEditor->GetSelectedGO() != nullptr) 
		{
			float3 maTogl = App->moduleEditor->GetSelectedGO()->transform->globalTransform.TranslatePart();
			target.Set(maTogl.x, maTogl.y, maTogl.z);
		}
		FocusCamera(target, 10.f);
		editorCamera.camFrustrum.WorldMatrix().Decompose(float3(), Direction, float3());
	}

	if (App->moduleInput->GetMouseButton(SDL_BUTTON_MIDDLE) == KEY_REPEAT)
		PanCamera(dt);
}

////Could be a good idea to use quaternions? Would it be faster?
////BUG: Camera will bug when the the horizontal rotation is almost 
////equal as the target X and Z coords
//void ModuleCamera3D::OrbitalRotation(float3 center, float dt)
//{
//	int dx = -App->moduleInput->GetMouseXMotion();
//	int dy = -App->moduleInput->GetMouseYMotion();
//
//	if (dx != 0)
//	{
//		float DeltaX = (float)dx * mouseSensitivity;
//
//		//Get vector diference
//		float distance = length(editorCamera.camFrustrum.pos - center);
//		float3 ref = normalize(Position - center);
//
//		//Rotate diference
//		ref = rotate(ref, DeltaX, float3(0.0f, 1.0f, 0.0f));
//
//		//Move camera position to new diference
//		Position = center + ref * distance;
//
//	}
//	if (dy != 0)
//	{
//
//		//BUG: Weird bug when looking at the same Y
//		float DeltaY = (float)dy * mouseSensitivity;
//
//		float distance = length(editorCamera.camFrustrum.pos - center);
//		float3 ref = normalize(Position - center);
//		ref = rotate(ref, DeltaY, -cross(ref, float3(0.0f, 1.0f, 0.0f)));
//
//		//debug = center + ref * distance;
//
//		Position = center + ref * distance;
//	}
//
//
//	//Reference = center;
//	LookAt(center);
//}

void ModuleCamera3D::FreeRotation(float dt)
{
	int dx = -App->moduleInput->GetMouseXMotion();
	int dy = -App->moduleInput->GetMouseYMotion();

	if (dy != 0)
	{
		float DeltaY = (float)dy * mouseSensitivity;

		Quat Y = Quat::identity;
		Y.SetFromAxisAngle(float3(1, 0, 0), DeltaY * DEGTORAD);

		Direction = Direction * Y;
	}

	if (dx != 0)
	{
		float DeltaX = (float)dx * mouseSensitivity;

		Quat X = Quat::identity;
		X.SetFromAxisAngle(float3(0, 1, 0), DeltaX * DEGTORAD);

		Direction = X * Direction;
	}
}

void ModuleCamera3D::FocusCamera(float3 center, float offset)
{
	editorCamera.LookAt(center);
	editorCamera.camFrustrum.pos = center + (((editorCamera.camFrustrum.pos - center).Normalized()) * offset);
}

void ModuleCamera3D::PanCamera(float dt)
{

	//WARNING: Need to normalize movement
	int dx = -App->moduleInput->GetMouseXMotion();
	int dy = -App->moduleInput->GetMouseYMotion();

	if (dx != 0 || dy != 0) 
	{
		float3 movVector((editorCamera.camFrustrum.WorldRight() * dx) + (-editorCamera.camFrustrum.up * dy));
		editorCamera.camFrustrum.pos += movVector * dt;
	}
}
