#ifndef STANDALONE

#include "Window.h"
#include <vector>

class W_EnvLightConfig : public Window
{

public:
	W_EnvLightConfig();
	virtual ~W_EnvLightConfig();

	void Draw() override;

	void SetPaths(char* loadedFaces[6]);
	void SetPaths(std::vector<char*> loadedFaces);
	void ClearPaths();

	std::vector<std::string> cubemapPaths;

};

#endif // !STANDALONE