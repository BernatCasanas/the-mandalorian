#include "ImageRenderer.h"
#include "DE_Advanced_FrameBuffer.h"


ImageRenderer::ImageRenderer() : myFbo(nullptr) //doesn't use its own FBO, renders directly to bind fbo or screen
{
}

ImageRenderer::ImageRenderer(int width, int height) : myFbo(nullptr) //renders to its own fbo
{
	myFbo = new DE_Advanced_FrameBuffer(width, height, DEPTH_BUFFER_TYPE::NONE);
}

ImageRenderer::~ImageRenderer()
{
	CleanUp();
}

void ImageRenderer::CleanUp()
{
	if (myFbo != nullptr)
	{
		delete(myFbo);
		myFbo = nullptr;
	}
}

void ImageRenderer::RegenerateFBO(int width, int height)
{
	if (myFbo == nullptr)
		return;
	if (myFbo->texBufferSize.x != width || myFbo->texBufferSize.y != height)
		myFbo->ReGenerateBuffer(width, height);
}

void ImageRenderer::RenderQuad()
{
	if (myFbo != nullptr)
	{
		myFbo->BindFrameBuffer();
	}

	glClear(GL_COLOR_BUFFER_BIT);
	glDrawArrays(GL_TRIANGLES, 0, 6);

	if (myFbo != nullptr)
	{
		myFbo->UnbindFrameBuffer();
	}
}

int ImageRenderer::GetOutputTexture() const
{
	if (myFbo != nullptr)
		return myFbo->GetColorTexture();
	else
		return 0;
}

DE_Advanced_FrameBuffer* ImageRenderer::GetOutputFBO()
{
	return myFbo;
}
