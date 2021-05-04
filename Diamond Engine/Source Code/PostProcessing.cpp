#include "PostProcessing.h"
#include "Glew/include/glew.h"
#include "Application.h"
#include "MO_ResourceManager.h"
#include"RE_Shader.h"

ImageRenderer::ImageRenderer() : myFbo(nullptr)
{
}

ImageRenderer::ImageRenderer(int width, int height) : myFbo(nullptr)
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


PostProcessEffectContrastTest::PostProcessEffectContrastTest() :contrastShader(nullptr)
{
	quadRenderer = new ImageRenderer(1920, 1080);//TODO change this for the constructor with params once we have more than 1 effect
		//TODO post process shader uid here once we have it into lib
	contrastShader = dynamic_cast<ResourceShader*>(EngineExternal->moduleResources->RequestResource(164660139, Resource::Type::SHADER));
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
		EngineExternal->moduleResources->UnloadResource(164660139);	//TODO post process shader uid here once we have it into lib
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

PostProcessing::PostProcessing() :contrastTest(nullptr), quadVAO(0)
{
}

PostProcessing::~PostProcessing()
{
}

//needs to be used externally, post processing won't init itself. Do so after opengl init
void PostProcessing::Init()
{
	//create screen quad here
	// configure VAO
	glGenBuffers(1, &quadVAO);
	glBindBuffer(GL_ARRAY_BUFFER, quadVAO);

	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, sizeof(float) * 2, (GLvoid*)0);

	glBindBuffer(GL_ARRAY_BUFFER, 0);

	//init effects
	contrastTest = new PostProcessEffectContrastTest();
}

void PostProcessing::DoPostProcessing(int width, int height, DE_Advanced_FrameBuffer& outputFBO, unsigned int colorTexture, unsigned int depthTexture, ResourcePostProcess* settings)
{
	Start();

	//do post processing here
	contrastTest->Render(width, height, colorTexture);
	contrastTest->GetOutputFBO()->ResolveToFBO(outputFBO);
	End();
}

//needs to be used externally, post processing won't clean itself. Do so before opengl cleanup
void PostProcessing::CleanUp()
{
	if (quadVAO != 0)
	{
		glDeleteBuffers(1, &quadVAO);
		quadVAO = 0;
	}


	if (contrastTest != nullptr)
	{
		delete(contrastTest);
		contrastTest = nullptr;
	}

}

void PostProcessing::Start()
{
	glBindBuffer(GL_ARRAY_BUFFER, quadVAO);
	glEnableVertexAttribArray(0);
	glDisable(GL_DEPTH_TEST);
}

void PostProcessing::End()
{
	glEnable(GL_DEPTH_TEST);
	glDisableVertexAttribArray(0);
	glBindBuffer(GL_ARRAY_BUFFER, 0);
}

