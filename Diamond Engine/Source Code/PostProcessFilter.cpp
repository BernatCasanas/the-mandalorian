#include "PostProcessFilter.h"
#include "RE_Shader.h"
#include "ImageRenderer.h"

#include "Glew/include/glew.h"
#include "Application.h"
#include "MO_ResourceManager.h"



PostProcessFilter::PostProcessFilter(int shaderUID):myShader(nullptr),quadRenderer(nullptr)
{
	quadRenderer = new ImageRenderer(1920, 1080);
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
	if (quadRenderer == nullptr)
		return 0;

	return quadRenderer->GetOutputTexture();
}

DE_Advanced_FrameBuffer* PostProcessFilter::GetOutputFBO()
{
	if (quadRenderer == nullptr)
		return nullptr;

	return quadRenderer->GetOutputFBO();
}

PostProcessFilterContrastTest::PostProcessFilterContrastTest() : PostProcessFilter(1381112348)
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

PostProcessFilterDepthTest::PostProcessFilterDepthTest(): PostProcessFilter(454495126)
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

	//glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_COMPARE_MODE, GL_NONE);
	quadRenderer->RenderQuad();
	myShader->Unbind();
}