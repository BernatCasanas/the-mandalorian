#pragma once

class DE_Advanced_FrameBuffer;
class ResourceShader;
class ImageRenderer;


class PostProcessFilter
{
public:
	PostProcessFilter(int shaderUID);
	~PostProcessFilter();

	void CleanUp();
	int GetOutputTexture()const;
	DE_Advanced_FrameBuffer* GetOutputFBO();

protected:
	ResourceShader* myShader;
	ImageRenderer* quadRenderer;
};

class PostProcessFilterContrastTest : public PostProcessFilter
{
public:
	PostProcessFilterContrastTest();
	~PostProcessFilterContrastTest();
	void Render(int width, int height, unsigned int colorTexture);
};

class PostProcessFilterDepthTest : public PostProcessFilter
{
public:
	PostProcessFilterDepthTest();
	~PostProcessFilterDepthTest();
	void Render(int width, int height, unsigned int colorTexture, unsigned int depthTexture);

};
