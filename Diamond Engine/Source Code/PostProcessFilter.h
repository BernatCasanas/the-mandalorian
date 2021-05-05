#pragma once

class DE_Advanced_FrameBuffer;
class ResourceShader;
class ImageRenderer;

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

class PostProcessEffectDepthTest
{
public:
	PostProcessEffectDepthTest();
	~PostProcessEffectDepthTest();
	void CleanUp();
	void Render(int width, int height, unsigned int colorTexture, unsigned int depthTexture);
	int GetOutputTexture()const;
	DE_Advanced_FrameBuffer* GetOutputFBO();
private:
	ResourceShader* depthShader;
	ImageRenderer* quadRenderer;
};
