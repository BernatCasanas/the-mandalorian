#pragma once
#include "RE_PostProcess.h"
#include "RE_Shader.h"

class DE_Advanced_FrameBuffer;

class PostProcessEffectBloom;
class PostProcessEffectAO;
class PostProcessEffectRender;
class PostProcessEffectToneMapping;
class PostProcessEffectVignette;

class C_Camera;

class PostProcessing
{
public:
	PostProcessing();
	~PostProcessing();

	//create screen quad here
	void Init();
	void DoPostProcessing(int width, int height, DE_Advanced_FrameBuffer& outputFBO, unsigned int colorTexture, unsigned int depthTexture, C_Camera* sceneCam);
	void CleanUp();

private:
	void Start();
	void End();

	PostProcessEffectBloom* bloomEffect;
	PostProcessEffectAO* aoEffect;
	PostProcessEffectRender* renderFilter;
	PostProcessEffectToneMapping* toneMappingEffect;
	PostProcessEffectVignette* vignetteEffect;

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