#pragma once

#include"MathGeoLib/include/Math/float2.h"
#include"OpenGL.h"
#include"Globals.h"

enum class DEPTH_BUFFER_TYPE
{
	NONE,
	DEPTH_TEXTURE,
	DEPTH_RENDER_BUFER,
	DEPTH_RENDER_STENCIl_BUFER
};

class DE_Advanced_FrameBuffer
{
public:
	//normal buffer constructor
	DE_Advanced_FrameBuffer(int width,int height, DEPTH_BUFFER_TYPE depthBufferType);
	//Multisampled buffer constructor
	DE_Advanced_FrameBuffer(int width, int height,int msaaSamples);
	~DE_Advanced_FrameBuffer();

	void ClearBuffer();

	void InitializeFrameBuffer(DEPTH_BUFFER_TYPE depthType);
	void ReGenerateBuffer(int w, int h,int msaaSamples=1);

	void BindFrameBuffer();
	void UnbindFrameBuffer();
	void BindToRead();

	void ResolveToFBO(DE_Advanced_FrameBuffer& outputFbo);
	void ResolveToScreen();

	inline unsigned int GetFrameBuffer() { return framebuffer; }
	inline unsigned int GetColorBuffer() { return colorBufferAttachment; }
	inline unsigned int GetDepthBuffer() { return depthBufferAttachment; }

	inline unsigned int GetColorTexture() { return colorTexture; }
	inline unsigned int GetDepthTexture() { return depthTexture; }

private:
	int CreateFramBuffer();
	void CreateMultisampleColorAttachment();
	void CreateTextureAttachment();
	void CreateDepthBufferAttachment();//TODO we do not support stencil for the moment
	void CreateDepthStencilBufferAttachment();//TODO we do not support stencil for the moment
	void CreateDepthTextureAttachment();//TODO we do not support stencil for the moment



public:
	float2 texBufferSize; //texture buffer dimensions
private:

	unsigned int framebuffer; //index of the frame buffer
	unsigned int colorTexture; //index of the color texture
	unsigned int depthTexture; //index of the depth texture
	
	unsigned int colorBufferAttachment; //index of the Color buffer
	unsigned int depthBufferAttachment; //index of the Depth buffer

	bool alreadyInitialized;
	bool isMultisample;
	int msaaSamples;
	DEPTH_BUFFER_TYPE myDepthType;
};