#include "Globals.h"
#include "Application.h"
#include "ModuleRenderer3D.h"
#include "MaykMath.h"

#include"OpenGL.h"

#pragma comment (lib, "opengl32.lib") /* link Microsoft OpenGL lib   */
#pragma comment (lib, "Glew/libx86/glew32.lib") /* link Microsoft OpenGL lib   */

ModuleRenderer3D::ModuleRenderer3D(Application* app, bool start_enabled) : Module(app, start_enabled), str_CAPS(""),
vsync(false)
{
	GetCAPS(str_CAPS);
}

// Destructor
ModuleRenderer3D::~ModuleRenderer3D()
{}

// Called before render is available
bool ModuleRenderer3D::Init()
{
	LOG("Creating 3D Renderer context");
	bool ret = true;
	
	//ASK: Can i do this inside the MM namespace?
	MaykMath::Init();

	//Create context
	context = SDL_GL_CreateContext(App->window->window);
	if(context == NULL)
	{
		LOG("OpenGL context could not be created! SDL_Error: %s\n", SDL_GetError());
		ret = false;
	}

	GLenum error = glewInit();
	if (error != GL_NO_ERROR)
	{
		LOG("Error initializing glew library! %s", SDL_GetError());
		ret = false;
	}
	else
	{
		LOG("Using Glew %s", glewGetString(GLEW_VERSION));
	}
	
	if(ret == true)
	{
		//Use Vsync
		if(SDL_GL_SetSwapInterval(static_cast<int>(vsync)) < 0)
			LOG("Warning: Unable to set VSync! SDL Error: %s\n", SDL_GetError());

		//Initialize Projection Matrix
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();

		//Check for error
		GLenum error = glGetError();
		if(error != GL_NO_ERROR)
		{
			LOG("Error initializing OpenGL! %s\n", glewGetErrorString(error));
			ret = false;
		}

		//Initialize Modelview Matrix
		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();

		//Check for error
		error = glGetError();
		if(error != GL_NO_ERROR)
		{
			//ASK: Maybe glewGetErrorString is not the same as glutGerErrorString, so errors could be wrong
			LOG("Error initializing OpenGL! %s\n", glewGetErrorString(error));
			ret = false;
		}
		
		glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);
		glClearDepth(1.0f);
		
		//Initialize clear color
		glClearColor(0.f, 0.f, 0.f, 1.f);
		glClear(GL_COLOR_BUFFER_BIT);

		//Check for error
		error = glGetError();
		if(error != GL_NO_ERROR)
		{
			//ASK: Maybe glewGetErrorString is not the same as glutGerErrorString, so errors could be wrong
			LOG("Error initializing OpenGL! %s\n", glewGetErrorString(error));
			ret = false;
		}

		// Blend for transparency
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		glBlendEquation(GL_FUNC_ADD);
		
		GLfloat LightModelAmbient[] = {0.0f, 0.0f, 0.0f, 1.0f};
		glLightModelfv(GL_LIGHT_MODEL_AMBIENT, LightModelAmbient);

		lights[0].ref = GL_LIGHT0;
		lights[0].ambient.Set(0.25f, 0.25f, 0.25f, 1.0f);
		lights[0].diffuse.Set(0.75f, 0.75f, 0.75f, 1.0f);
		lights[0].SetPos(0.0f, 0.0f, 2.5f);
		lights[0].Init();

		GLfloat MaterialAmbient[] = {1.0f, 1.0f, 1.0f, 1.0f};
		glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, MaterialAmbient);

		GLfloat MaterialDiffuse[] = {1.0f, 1.0f, 1.0f, 1.0f};
		glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, MaterialDiffuse);
		
		
		glEnable(GL_DEPTH_TEST);
		glEnable(GL_CULL_FACE);
		lights[0].Active(true);
		glEnable(GL_LIGHTING);
		glEnable(GL_COLOR_MATERIAL);
		glEnable(GL_TEXTURE_2D);
	}

	// Projection matrix for
	OnResize(SCREEN_WIDTH, SCREEN_HEIGHT);

	return ret;
}

// PreUpdate: clear buffer
update_status ModuleRenderer3D::PreUpdate(float dt)
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glLoadIdentity();

	glMatrixMode(GL_MODELVIEW);
	glLoadMatrixf(App->camera->GetViewMatrix());

	//Light 0 on cam pos
	lights[0].SetPos(App->camera->Position.x, App->camera->Position.y, App->camera->Position.z);

	for(uint i = 0; i < MAX_LIGHTS; ++i)
		lights[i].Render();

	return UPDATE_CONTINUE;
}

// PostUpdate present buffer to screen
update_status ModuleRenderer3D::PostUpdate(float dt)
{



	//Render debug geometry
	//Draw editor
	App->moduleEditor->Draw();
	//Swap buffers

	SDL_GL_SwapWindow(App->window->window);

	return UPDATE_CONTINUE;
}

// Called before quitting
bool ModuleRenderer3D::CleanUp()
{
	LOG("Destroying 3D Renderer");

	SDL_GL_DeleteContext(context);

	return true;
}


void ModuleRenderer3D::OnResize(int width, int height)
{
	glViewport(0, 0, width, height);

	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	ProjectionMatrix = perspective(60.0f, (float)width / (float)height, 0.125f, 512.0f);
	glLoadMatrixf(&ProjectionMatrix);

	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	App->window->s_width = width;
	App->window->s_height = height;
}

void ModuleRenderer3D::OnGUI()
{
	if (ImGui::CollapsingHeader("Hardware"))
	{

		//TODO: Store all this info as const char* and dont call the functions every frame
		SDL_version ver;
		SDL_GetVersion(&ver);
		ImGui::Text("SDL Version: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%d.%d.%d", ver.major, ver.minor, ver.patch);

		ImGui::Separator();
		ImGui::Text("CPUs: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%d (Cache: %dkb)", SDL_GetCPUCount(), SDL_GetCPUCacheLineSize());
		ImGui::Text("System RAM: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%.1fGb", SDL_GetSystemRAM() / 1000.f);

#pragma region Caps
		ImGui::Text("Caps:");
		ImGui::SameLine();


		ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), str_CAPS.c_str());

#pragma endregion


		ImGui::Separator();
		ImGui::Text("GPU: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), (const char*)glGetString(GL_VENDOR));
		ImGui::Text("Brand: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), (const char*)glGetString(GL_RENDERER));

		//#define GL_GPU_MEM_INFO_TOTAL_AVAILABLE_MEM_NVX 0x9048
		//#define GL_GPU_MEM_INFO_CURRENT_AVAILABLE_MEM_NVX 0x9049
		//GLint total_mem_kb = 0;
		//glGetIntegerv(GL_GPU_MEM_INFO_TOTAL_AVAILABLE_MEM_NVX, &total_mem_kb);
		//total_mem_kb /= 1000;

		//GLint cur_avail_mem_kb = 0;
		//glGetIntegerv(GL_GPU_MEM_INFO_CURRENT_AVAILABLE_MEM_NVX, &cur_avail_mem_kb);
		//cur_avail_mem_kb /= 1000;

		ImGui::Text("VRAM Budget:");
		ImGui::Text("VRAM Usage:");
		ImGui::Text("VRAM Avaliable:");
		ImGui::Text("VRAM Reserved:");

		if(ImGui::Checkbox("Enable V-Sync", &vsync))
		{
			//Use Vsync
			if (SDL_GL_SetSwapInterval(static_cast<int>(vsync)) < 0)
				LOG("Warning: Unable to set VSync! SDL Error: %s\n", SDL_GetError());
		}

	}
}

void ModuleRenderer3D::GetCAPS(std::string& caps)
{
	caps += (SDL_HasRDTSC()) ? "RDTSC," : "";
	caps += (SDL_HasMMX()) ? "MMX, " : "";
	caps += (SDL_HasSSE()) ? "SSE, " : "";
	caps += (SDL_HasSSE2()) ? "SSE2, " : "";
	caps += (SDL_HasSSE3()) ? "SSE3, " : "";
	caps += "\n";
	caps += (SDL_HasSSE41()) ? "SSE41, " : "";
	caps += (SDL_HasSSE42()) ? "SSE42, " : "";
	caps += (SDL_HasAVX()) ? "AVX, " : "";
	caps += (SDL_HasAltiVec()) ? "AltiVec, " : "";
	caps += (SDL_Has3DNow()) ? "3DNow, " : "";
}
