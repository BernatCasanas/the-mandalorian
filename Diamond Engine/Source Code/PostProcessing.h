#pragma once
#include "RE_PostProcess.h"
#include "RE_Shader.h"

class DE_Advanced_FrameBuffer;
class PostProcessFilterContrastTest;
class PostProcessFilterDepthTest;
class PostProcessFilterRender;

class PostProcessing
{
public:
	PostProcessing();
	~PostProcessing();

	//create screen quad here
	void Init();
	void DoPostProcessing(int width, int height, DE_Advanced_FrameBuffer& outputFBO, unsigned int colorTexture, unsigned int depthTexture, ResourcePostProcess* settings);
	void CleanUp();

private:
	void Start();
	void End();

	PostProcessFilterContrastTest* contrastTest;
	PostProcessFilterDepthTest* depthTest;
	PostProcessFilterRender* renderFilter;
	unsigned int quadVAO;
	unsigned int quadVBO;
};

const float vertices[] = {
		-1.0f, -1.0f,  0.0f,
		 1.0f, -1.0f,  0.0f,
		-1.0f,  1.0f,  0.0f,
		 1.0f, -1.0f,  0.0f,
		 1.0f,  1.0f,  0.0f,
		-1.0f,  1.0f,  0.0f,
};