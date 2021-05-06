#include "PostProcessing.h"
#include "Glew/include/glew.h"
#include "Application.h"
#include "MO_ResourceManager.h"
#include"RE_Shader.h"

#include "PostProcessEffect.h"
#include "DE_Advanced_FrameBuffer.h"

#include "CO_Camera.h"

PostProcessing::PostProcessing() :
	quadVAO(0), quadVBO(0),
	contrastTest(nullptr),
	depthTest(nullptr),
	aoEffect(nullptr)
{
}

PostProcessing::~PostProcessing()
{
}

//needs to be used externally, post processing won't init itself. Do so after opengl init
void PostProcessing::Init()
{
	//create screen quad here
	// configure VAO / VBO
	glGenVertexArrays(1, &quadVAO);
	glGenBuffers(1, &quadVBO);

	glBindVertexArray(quadVAO);
	glBindBuffer(GL_ARRAY_BUFFER, quadVBO);

	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 3, (GLvoid*)0);
	glEnableVertexAttribArray(0);

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);

	//init effects
	contrastTest = new PostProcessEffectInvertTest();
	depthTest = new PostProcessEffectDepthTest();
	renderFilter = new PostProcessEffectRender();
	aoEffect = new PostProcessEffectAO();
}

void PostProcessing::DoPostProcessing(int width, int height, DE_Advanced_FrameBuffer& outputFBO, unsigned int colorTexture, unsigned int depthTexture,C_Camera* sceneCam, ResourcePostProcess* settings)
{

	Start();

	int currentColTexIndex = colorTexture;


	//do post processing here

	if (false)
	{
		currentColTexIndex = contrastTest->Render(width, height, currentColTexIndex);
	}
	if (false)
	{
		currentColTexIndex = depthTest->Render(width, height, currentColTexIndex, depthTexture);
	}
	if (true)
	{
		currentColTexIndex = aoEffect->Render(width, height, depthTexture,sceneCam);
	}

	//end of postprocessing

	if (currentColTexIndex != colorTexture) //only if any effect has been applied
	{
		//Post process filter to resolve to fbo MUST BE AT THE END
		renderFilter->Render(width, height, currentColTexIndex, outputFBO);
	}
	End();
}

//needs to be used externally, post processing won't clean itself. Do so before opengl cleanup
void PostProcessing::CleanUp()
{
	if (quadVBO != 0)
	{
		glDeleteBuffers(1, &quadVBO);
		quadVBO = 0;
	}
	if (quadVAO != 0)
	{
		glDeleteVertexArrays(1, &quadVAO);
		quadVAO = 0;
	}

	if (contrastTest != nullptr)
	{
		delete(contrastTest);
		contrastTest = nullptr;
	}

	if (depthTest != nullptr)
	{
		delete(depthTest);
		depthTest = nullptr;
	}

	if (renderFilter != nullptr)
	{
		delete(renderFilter);
		renderFilter = nullptr;
	}
}

void PostProcessing::Start()
{
	glBindVertexArray(quadVAO);
	glEnableVertexAttribArray(0);
	glDisable(GL_DEPTH_TEST);
}

void PostProcessing::End()
{
	glEnable(GL_DEPTH_TEST);
	glDisableVertexAttribArray(0);
	glBindVertexArray(0);
}

