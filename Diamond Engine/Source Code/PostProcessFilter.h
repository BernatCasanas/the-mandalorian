#pragma once

class DE_Advanced_FrameBuffer;
class ResourceShader;
class ImageRenderer;


class PostProcessFilter
{
public:
	PostProcessFilter(int shaderUID, bool ownFBO);
	~PostProcessFilter();

	void CleanUp();
	int GetOutputTexture()const;
	DE_Advanced_FrameBuffer* GetOutputFBO();
	bool HasOwnFBO()const;
protected:
	ResourceShader* myShader;
	ImageRenderer* quadRenderer;
};

class PostProcessFilterContrastTest : public PostProcessFilter
{
public:
	PostProcessFilterContrastTest();
	~PostProcessFilterContrastTest();
	void Render(int width, int height, unsigned int colorTexture);//ony one render will be needed for real PostProcessFilters
	void Render(DE_Advanced_FrameBuffer* outputFBO, unsigned int colorTexture);

};

class PostProcessFilterDepthTest : public PostProcessFilter
{
public:
	PostProcessFilterDepthTest();
	~PostProcessFilterDepthTest();
	void Render(int width, int height, unsigned int colorTexture, unsigned int depthTexture);//ony one render will be needed for real PostProcessFilters
	void Render(DE_Advanced_FrameBuffer* outputFBO, unsigned int colorTexture, unsigned int depthTexture);

};

class PostProcessFilterRender : public PostProcessFilter
{
public:
	PostProcessFilterRender();
	~PostProcessFilterRender();
	void Render(int width, int height, unsigned int colorTexture);//ony one render will be needed for real PostProcessFilters

};