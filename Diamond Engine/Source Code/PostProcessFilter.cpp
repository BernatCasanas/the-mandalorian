#include "PostProcessFilter.h"
#include "RE_Shader.h"
#include "ImageRenderer.h"

#include "Glew/include/glew.h"
#include "Application.h"
#include "MO_ResourceManager.h"


PostProcessEffectContrastTest::PostProcessEffectContrastTest() :contrastShader(nullptr)
{
	quadRenderer = new ImageRenderer(1920, 1080);//TODO change this for the constructor with params once we have more than 1 effect
		//TODO post process shader uid here once we have it into lib
	contrastShader = dynamic_cast<ResourceShader*>(EngineExternal->moduleResources->RequestResource(1381112348, Resource::Type::SHADER));
}

PostProcessEffectContrastTest::~PostProcessEffectContrastTest()
{
	CleanUp();
}

void PostProcessEffectContrastTest::CleanUp()
{
	if (quadRenderer != nullptr)
		delete(quadRenderer);

	if (contrastShader != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(1381112348);	//TODO post process shader uid here once we have it into lib
		contrastShader = nullptr;
	}

}

void PostProcessEffectContrastTest::Render(int width, int height, unsigned int colorTexture)
{
	quadRenderer->RegenerateFBO(width, height);
	contrastShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, colorTexture);
	quadRenderer->RenderQuad();
	contrastShader->Unbind();
}

int PostProcessEffectContrastTest::GetOutputTexture() const
{
	return quadRenderer->GetOutputTexture();
}

DE_Advanced_FrameBuffer* PostProcessEffectContrastTest::GetOutputFBO()
{
	return quadRenderer->GetOutputFBO();
}

PostProcessEffectDepthTest::PostProcessEffectDepthTest()
{
	quadRenderer = new ImageRenderer(1920, 1080);
	//TODO post process shader uid here once we have it into lib
	depthShader = dynamic_cast<ResourceShader*>(EngineExternal->moduleResources->RequestResource(454495126, Resource::Type::SHADER));

}

PostProcessEffectDepthTest::~PostProcessEffectDepthTest()
{
	CleanUp();
}

void PostProcessEffectDepthTest::CleanUp()
{
	if (quadRenderer != nullptr)
		delete(quadRenderer);

	if (depthShader != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(454495126);	//TODO post process shader uid here once we have it into lib
		depthShader = nullptr;
	}
}

void PostProcessEffectDepthTest::Render(int width, int height, unsigned int colorTexture, unsigned int depthTexture)
{
	quadRenderer->RegenerateFBO(width, height);
	depthShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, colorTexture);
	glUniform1i(glGetUniformLocation(depthShader->shaderProgramID, "colourTexture"), 0);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, depthTexture);
	glUniform1i(glGetUniformLocation(depthShader->shaderProgramID, "depthTexture"), 1);

	//glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_COMPARE_MODE, GL_NONE);
	quadRenderer->RenderQuad();
	depthShader->Unbind();
}

int PostProcessEffectDepthTest::GetOutputTexture() const
{
	return quadRenderer->GetOutputTexture();
}

DE_Advanced_FrameBuffer* PostProcessEffectDepthTest::GetOutputFBO()
{
	return quadRenderer->GetOutputFBO();
}
