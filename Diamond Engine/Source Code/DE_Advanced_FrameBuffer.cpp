#include "DE_Advanced_FrameBuffer.h"

#include "Application.h"
#include "MO_Window.h"

DE_Advanced_FrameBuffer::DE_Advanced_FrameBuffer(int width, int height, DEPTH_BUFFER_TYPE depthBufferType) :
	framebuffer(0), colorTexture(0), depthTexture(0), depthBufferAttachment(0), colorBufferAttachment(0), texBufferSize(float2::zero),
	isMultisample(false), msaaSamples(0), myDepthType(depthBufferType), alreadyInitialized(false)
{
	texBufferSize.x = width;
	texBufferSize.y = height;
	//InitializeFrameBuffer(depthBufferType);
}

DE_Advanced_FrameBuffer::DE_Advanced_FrameBuffer(int width, int height, int msaaSamples) :
	framebuffer(0), colorTexture(0), depthTexture(0), depthBufferAttachment(0), colorBufferAttachment(0), texBufferSize(float2::zero),
	isMultisample(true), msaaSamples(msaaSamples), myDepthType(DEPTH_BUFFER_TYPE::DEPTH_RENDER_STENCIl_BUFER), alreadyInitialized(false)
{
	texBufferSize.x = width;
	texBufferSize.y = height;
	//InitializeFrameBuffer(DEPTH_BUFFER_TYPE::DEPTH_RENDER_BUFER); //multisample will only be needed during scene render (no later sampling) so we can optimize it by making the depth buffer a render buffer instead of a texture


}

DE_Advanced_FrameBuffer::~DE_Advanced_FrameBuffer()
{
	ClearBuffer();
}

void DE_Advanced_FrameBuffer::ClearBuffer()
{
	if (framebuffer != 0)
	{
		glDeleteFramebuffers(1, (GLuint*)& framebuffer);
		framebuffer = 0;
	}

	if (colorTexture != 0)
	{
		glDeleteTextures(1, (GLuint*)& colorTexture);
		colorTexture = 0;
	}

	if (depthTexture != 0)
	{
		glDeleteTextures(1, (GLuint*)& depthTexture);
		depthTexture = 0;
	}

	if (colorBufferAttachment != 0)
	{
		glDeleteRenderbuffers(1, (GLuint*)& colorBufferAttachment);
		colorBufferAttachment = 0;
	}

	if (depthBufferAttachment != 0)
	{
		glDeleteRenderbuffers(1, (GLuint*)& depthBufferAttachment);
		depthBufferAttachment = 0;
	}
}

void DE_Advanced_FrameBuffer::InitializeFrameBuffer(DEPTH_BUFFER_TYPE depthType)
{
	alreadyInitialized = true;
	myDepthType = depthType;
	CreateFramBuffer();
	if (isMultisample)
	{
		CreateMultisampleColorAttachment();
	}
	else
	{
		CreateTextureAttachment();
	}

	//TODO change this for a switch?
	if (depthType == DEPTH_BUFFER_TYPE::DEPTH_RENDER_BUFER)
	{
		CreateDepthBufferAttachment();
	}
	else if (depthType == DEPTH_BUFFER_TYPE::DEPTH_TEXTURE)
	{
		CreateDepthTextureAttachment();
	}
	else if (depthType == DEPTH_BUFFER_TYPE::DEPTH_RENDER_STENCIl_BUFER)
	{
		CreateDepthStencilBufferAttachment();
	}
	UnbindFrameBuffer();
}

void DE_Advanced_FrameBuffer::ReGenerateBuffer(int w, int h, int msaaSamples)
{
	texBufferSize = float2(w, h);
	this->msaaSamples = msaaSamples;
	ClearBuffer();
	InitializeFrameBuffer(myDepthType);

	if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
		LOG(LogType::L_ERROR, "ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
	glBindFramebuffer(GL_FRAMEBUFFER, 0);
}

//Binds frame buffer to draw
void DE_Advanced_FrameBuffer::BindFrameBuffer()
{
	if (!alreadyInitialized)
		InitializeFrameBuffer(myDepthType);

	//glBindTexture(GL_TEXTURE_2D, 0); //TODO needed? (here just to make sure the texture isn't bound)
	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, framebuffer);
	glViewport(0, 0, texBufferSize.x, texBufferSize.y);
}

//Sets current draw frame buffer to default
void DE_Advanced_FrameBuffer::UnbindFrameBuffer()
{
	if (!alreadyInitialized)
		InitializeFrameBuffer(myDepthType);

	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
	glViewport(0, 0, EngineExternal->moduleWindow->s_width, EngineExternal->moduleWindow->s_height);
}

//Binds the current FBO to be read from
void DE_Advanced_FrameBuffer::BindToRead()
{
	if (!alreadyInitialized)
		InitializeFrameBuffer(myDepthType);

	glBindTexture(GL_TEXTURE_2D, 0);
	glBindFramebuffer(GL_READ_BUFFER, framebuffer);
	glReadBuffer(GL_COLOR_ATTACHMENT0);
}

//renders fbo into another fbo
void DE_Advanced_FrameBuffer::ResolveToFBO(DE_Advanced_FrameBuffer& outputFbo)
{
	if (!alreadyInitialized)
		InitializeFrameBuffer(myDepthType);

	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, outputFbo.framebuffer);
	glBindFramebuffer(GL_READ_FRAMEBUFFER, framebuffer);
	glBlitFramebuffer(0, 0, texBufferSize.x, texBufferSize.y, 0, 0, outputFbo.texBufferSize.x, outputFbo.texBufferSize.y,
		GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT |GL_STENCIL_BUFFER_BIT, GL_NEAREST); //TODO is stencil needed here?
	UnbindFrameBuffer();
}

//renders fbo to screen
void DE_Advanced_FrameBuffer::ResolveToScreen()
{
	if (!alreadyInitialized)
		InitializeFrameBuffer(myDepthType);

	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
	glBindFramebuffer(GL_READ_FRAMEBUFFER, framebuffer);
	//glDrawBuffer(GL_BACK); //TODO is this needed here??
	glBlitFramebuffer(0, 0, texBufferSize.x, texBufferSize.y, 0, 0, EngineExternal->moduleWindow->s_width, EngineExternal->moduleWindow->s_height,
		GL_COLOR_BUFFER_BIT, GL_NEAREST);
	UnbindFrameBuffer();
}

int DE_Advanced_FrameBuffer::CreateFramBuffer()
{
	glGenFramebuffers(1, (GLuint*)& framebuffer);
	glBindFramebuffer(GL_FRAMEBUFFER, framebuffer);
	//glDrawBuffer(GL_COLOR_ATTACHMENT0); //attachment to wich the render buffer will be rendering to

	return framebuffer;
}

void DE_Advanced_FrameBuffer::CreateTextureAttachment()
{
	glGenTextures(1, (GLuint*)& colorTexture);

	glBindTexture(GL_TEXTURE_2D, colorTexture);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, texBufferSize.x, texBufferSize.y, 0, GL_RGB, GL_UNSIGNED_BYTE, NULL);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_MIRRORED_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_MIRRORED_REPEAT);

	// attach it to currently bound framebuffer object
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, colorTexture, 0);
}

void DE_Advanced_FrameBuffer::CreateDepthTextureAttachment()
{
	glGenTextures(1, (GLuint*)& depthTexture);

	glBindTexture(GL_TEXTURE_2D, depthTexture);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_DEPTH_COMPONENT24, texBufferSize.x, texBufferSize.y, 0, GL_DEPTH_COMPONENT, GL_FLOAT, NULL);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	// attach it to currently bound framebuffer object
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, depthTexture, 0);
}

void DE_Advanced_FrameBuffer::CreateMultisampleColorAttachment()
{
	/*glGenRenderbuffers(1, (GLuint*)& colorBufferAttachment);
	glBindRenderbuffer(GL_RENDERBUFFER, colorBufferAttachment);
	glRenderbufferStorageMultisample(GL_RENDERBUFFER, msaaSamples, GL_RGB, texBufferSize.x, texBufferSize.y);
	glFramebufferRenderbuffer(GL_RENDERBUFFER, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, colorBufferAttachment);*/

	glGenTextures(1, (GLuint*)& colorTexture);

	glBindTexture(GL_TEXTURE_2D_MULTISAMPLE, colorTexture);
	glTexImage2DMultisample(GL_TEXTURE_2D_MULTISAMPLE, msaaSamples, GL_RGB, texBufferSize.x, texBufferSize.y, GL_TRUE);
	glTexParameteri(GL_TEXTURE_2D_MULTISAMPLE, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D_MULTISAMPLE, GL_TEXTURE_MAG_FILTER, GL_NEAREST);


	// attach it to currently bound framebuffer object
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D_MULTISAMPLE, colorTexture, 0);
}

void DE_Advanced_FrameBuffer::CreateDepthBufferAttachment()
{
	glGenRenderbuffers(1, (GLuint*)& depthBufferAttachment);
	glBindRenderbuffer(GL_RENDERBUFFER, depthBufferAttachment);
	if (isMultisample)
	{
		glRenderbufferStorageMultisample(GL_RENDERBUFFER, msaaSamples, GL_DEPTH_COMPONENT24, texBufferSize.x, texBufferSize.y);
	}
	else
	{
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT24, texBufferSize.x, texBufferSize.y);
	}
	glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, depthBufferAttachment);
}

void DE_Advanced_FrameBuffer::CreateDepthStencilBufferAttachment()
{
	glGenRenderbuffers(1, (GLuint*)& depthBufferAttachment);
	glBindRenderbuffer(GL_RENDERBUFFER, depthBufferAttachment);
	if (isMultisample)
	{
		glRenderbufferStorageMultisample(GL_RENDERBUFFER, msaaSamples, GL_DEPTH24_STENCIL8, texBufferSize.x, texBufferSize.y);
	}
	else
	{
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, texBufferSize.x, texBufferSize.y);
	}
	glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, depthBufferAttachment);
}

