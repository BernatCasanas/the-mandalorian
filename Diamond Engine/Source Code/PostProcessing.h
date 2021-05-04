#pragma once
#include "RE_PostProcess.h"
#include "RE_Shader.h"
#include "DE_Advanced_FrameBuffer.h"

class ImageRenderer //renders an image to a framebuffer or to scren
{
public:
	ImageRenderer();//renders image to screen
	ImageRenderer(int width, int height);//renders image to a framebuffer
	~ImageRenderer();
	void CleanUp();
	//in case an fbo exists it will change its dimensions
	void RegenerateFBO(int width, int height);
	//a quad must be bound before rendering (see post processing init to check a quad)
	void RenderQuad();
	int GetOutputTexture() const;
	DE_Advanced_FrameBuffer* GetOutputFBO();
private:
	DE_Advanced_FrameBuffer* myFbo;
};


class PostProcessEffectContrastTest
{
public:
	PostProcessEffectContrastTest();
	~PostProcessEffectContrastTest();
	void CleanUp();
	void Render(int width, int height, unsigned int colorTexture);
	int GetOutputTexture()const;
	DE_Advanced_FrameBuffer* GetOutputFBO();
private:
	ResourceShader* contrastShader;
	ImageRenderer* quadRenderer;
};


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

	PostProcessEffectContrastTest* contrastTest;

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