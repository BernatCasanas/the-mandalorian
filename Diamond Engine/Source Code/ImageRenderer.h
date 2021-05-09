#pragma once

class DE_Advanced_FrameBuffer;

class ImageRenderer //renders an image to a framebuffer or to scren
{
public:
	ImageRenderer();//renders image to screen
	ImageRenderer(int width, int height);//renders image to a framebuffer
	~ImageRenderer();
	void CleanUp();
	//in case an fbo exists it will change its dimensions
	void RegenerateFBO(int width, int height, bool isHDR);
	//a quad must be bound before rendering (see post processing init to check a quad)
	void RenderQuad();
	int GetOutputTexture() const;
	DE_Advanced_FrameBuffer* GetOutputFBO();
private:
	DE_Advanced_FrameBuffer* myFbo;
};