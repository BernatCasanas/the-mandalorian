#ifndef __ModuleWindow_H__
#define __ModuleWindow_H__

#include "Module.h"
#include "SDL/include/SDL.h"

class Application;

class ModuleWindow : public Module
{
public:

	ModuleWindow(Application* app, bool start_enabled = true);

	// Destructor
	~ModuleWindow();

	bool Init() override;
	bool CleanUp() override;

	void OnGUI() override;

	void SetTitle(const char* title);

public:
	//The window we'll be rendering to
	SDL_Window* window;

	//The surface contained by the window
	SDL_Surface* screen_surface;

	int s_width;
	int s_height;
	float brightness;

	bool fullScreen;
	bool borderless;
	bool resizable;
	bool fullScreenDesktop;

};

#endif // __ModuleWindow_H__