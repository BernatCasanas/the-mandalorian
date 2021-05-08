#include "ImageRenderer.h"
#include "DE_Advanced_FrameBuffer.h"


ImageRenderer::ImageRenderer() : myFbo(nullptr) //doesn't use its own FBO, renders directly to bind fbo or screen
{
}

ImageRenderer::ImageRenderer(int width, int height) : myFbo(nullptr) //renders to its own fbo
{
	myFbo = new DE_Advanced_FrameBuffer(width, height, DEPTH_BUFFER_TYPE::NONE,false);
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

void ImageRenderer::RegenerateFBO(int width, int height,bool isHDR)
{
	if (myFbo == nullptr)
		return;
	if (myFbo->texBufferSize.x != width || myFbo->texBufferSize.y != height || myFbo->IsHDR()!=isHDR)
		myFbo->ReGenerateBuffer(width, height,1,isHDR);
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
