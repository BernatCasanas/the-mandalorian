#include "PostProcessFilter.h"
#include "RE_Shader.h"
#include "ImageRenderer.h"

#include "Glew/include/glew.h"
#include "Application.h"
#include "MO_ResourceManager.h"
#include "DE_Advanced_FrameBuffer.h"


PostProcessFilter::PostProcessFilter(int shaderUID, bool ownFBO) :myShader(nullptr), quadRenderer(nullptr)
{
	if (ownFBO)
		quadRenderer = new ImageRenderer(1920, 1080);
	else
		quadRenderer = new ImageRenderer();

	myShader = dynamic_cast<ResourceShader*>(EngineExternal->moduleResources->RequestResource(shaderUID, Resource::Type::SHADER));
}

PostProcessFilter::~PostProcessFilter()
{
	CleanUp();
}

void PostProcessFilter::CleanUp()
{
	if (quadRenderer != nullptr)
	{
		delete(quadRenderer);
		quadRenderer = nullptr;
	}

	if (myShader != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(myShader->GetUID());	//TODO post process shader uid here once we have it into lib
		myShader = nullptr;
	}
}

int PostProcessFilter::GetOutputTexture() const
{
	if (!HasOwnFBO())
		return 0;

	return quadRenderer->GetOutputTexture();
}

DE_Advanced_FrameBuffer* PostProcessFilter::GetOutputFBO()
{
	if (!HasOwnFBO())
		return nullptr;

	return quadRenderer->GetOutputFBO();
}

bool PostProcessFilter::HasOwnFBO() const
{
	if (quadRenderer == nullptr)
		return false;
	else
		return true;
}

PostProcessFilterContrastTest::PostProcessFilterContrastTest() : PostProcessFilter(1381112348,true)//this bool tells the filter whether it needs to create an fbo (but we have to pass that fbo to the render) //TODO only for testing, after testing only 1 approach will be chosen
{

}

PostProcessFilterContrastTest::~PostProcessFilterContrastTest()
{

}

void PostProcessFilterContrastTest::Render(int width, int height, unsigned int colorTexture)
{
	quadRenderer->RegenerateFBO(width, height);
	myShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, colorTexture);
	quadRenderer->RenderQuad();
	myShader->Unbind();
}

void PostProcessFilterContrastTest::Render(DE_Advanced_FrameBuffer* outputFBO, unsigned int colorTexture)
{
	myShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, colorTexture);

	outputFBO->BindFrameBuffer();
	quadRenderer->RenderQuad();
	outputFBO->UnbindFrameBuffer();

	myShader->Unbind();
}

PostProcessFilterDepthTest::PostProcessFilterDepthTest() : PostProcessFilter(454495126,true) //this bool tells the filter whether it needs to create an fbo (but we have to pass that fbo to the render) //TODO only for testing, after testing only 1 approach will be chosen
{

}

PostProcessFilterDepthTest::~PostProcessFilterDepthTest()
{

}


void PostProcessFilterDepthTest::Render(int width, int height, unsigned int colorTexture, unsigned int depthTexture)
{
	quadRenderer->RegenerateFBO(width, height);
	myShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, colorTexture);
	glUniform1i(glGetUniformLocation(myShader->shaderProgramID, "colourTexture"), 0);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, depthTexture);
	glUniform1i(glGetUniformLocation(myShader->shaderProgramID, "depthTexture"), 1);

	quadRenderer->RenderQuad();
	myShader->Unbind();
}

void PostProcessFilterDepthTest::Render(DE_Advanced_FrameBuffer* outputFBO, unsigned int colorTexture, unsigned int depthTexture)
{

	myShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, colorTexture);
	glUniform1i(glGetUniformLocation(myShader->shaderProgramID, "colourTexture"), 0);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, depthTexture);
	glUniform1i(glGetUniformLocation(myShader->shaderProgramID, "depthTexture"), 1);

	outputFBO->BindFrameBuffer();
	quadRenderer->RenderQuad();
	outputFBO->UnbindFrameBuffer();

	myShader->Unbind();
}

PostProcessFilterRender::PostProcessFilterRender(): PostProcessFilter(2143344219, true) //this bool tells the filter whether it needs to create an fbo (but we have to pass that fbo to the render) //TODO only for testing, after testing only 1 approach will be chosen
{

}

PostProcessFilterRender::~PostProcessFilterRender()
{

}

void PostProcessFilterRender::Render(int width, int height, unsigned int colorTexture)
{
	quadRenderer->RegenerateFBO(width, height);
	myShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, colorTexture);
	glUniform1i(glGetUniformLocation(myShader->shaderProgramID, "colourTexture"), 0);
	quadRenderer->RenderQuad();
	myShader->Unbind();
}
