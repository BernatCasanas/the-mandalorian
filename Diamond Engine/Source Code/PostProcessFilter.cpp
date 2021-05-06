#include "PostProcessFilter.h"
#include "RE_Shader.h"
#include "ImageRenderer.h"

#include "Glew/include/glew.h"
#include "Application.h"
#include "MO_ResourceManager.h"
#include "DE_Advanced_FrameBuffer.h"
#include "MathGeoLib/include/Math/float4x4.h"
#include "MathGeoLib/include/Algorithm/Random/LCG.h"

#include "CO_Camera.h"

//A post process filter consists of a shader that renders onto a screen quad applying a single shader effect (Ex. Invert Image Colors)
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

PostProcessFilterContrastTest::PostProcessFilterContrastTest() : PostProcessFilter(1381112348, true)//this bool tells the filter whether it needs to create an fbo (but we have to pass that fbo to the render) //TODO only for testing, after testing only 1 approach will be chosen
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

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, 0);
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


	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, 0);
	myShader->Unbind();
}

PostProcessFilterDepthTest::PostProcessFilterDepthTest() : PostProcessFilter(454495126, true) //this bool tells the filter whether it needs to create an fbo (but we have to pass that fbo to the render) //TODO only for testing, after testing only 1 approach will be chosen
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

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, 0);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, 0);
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

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, 0);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, 0);

	myShader->Unbind();
}

PostProcessFilterRender::PostProcessFilterRender() : PostProcessFilter(2143344219, true) //this bool tells the filter whether it needs to create an fbo (but we have to pass that fbo to the render) //TODO only for testing, after testing only 1 approach will be chosen
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
	
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, 0);
	myShader->Unbind();
}

PostProcessFilterAO::PostProcessFilterAO() : PostProcessFilter(1926059054, true)
{
	PopulateKernel();
}

PostProcessFilterAO::~PostProcessFilterAO()
{
}

void PostProcessFilterAO::Render(int width, int height, unsigned int depthTexture, C_Camera* currCam)
{
	quadRenderer->RegenerateFBO(width, height);
	myShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, depthTexture);
	glUniform1i(glGetUniformLocation(myShader->shaderProgramID, "depthTexture"), 0);

	bool isOrthograpic = false;
	if (currCam->camFrustrum.type == math::FrustumType::OrthographicFrustum)
	{
		isOrthograpic = true;
	}
	GLint uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "isOrthographic");
	glUniform1i(uniformLoc, isOrthograpic);

	if (!isOrthograpic)
	{

		uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "tanHalfFoVx");
		glUniform1f(uniformLoc, tanf(currCam->camFrustrum.horizontalFov * 0.5f));
		uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "tanHalfFoVy");
		glUniform1f(uniformLoc, tanf(currCam->camFrustrum.verticalFov * 0.5f));
	}
	else
	{
		uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "tanHalfFoVx");
		glUniform1f(uniformLoc, currCam->camFrustrum.orthographicWidth*0.5f );
		uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "tanHalfFoVy");
		glUniform1f(uniformLoc, currCam->camFrustrum.orthographicHeight*0.5f);
	}

	uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "sampleRad");
	glUniform1f(uniformLoc,0.01f);//TODO radius here

	uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "projectionMat");
	glUniformMatrix4fv(uniformLoc, 1, GL_FALSE, currCam->ProjectionMatrixOpenGL().ptr());
	uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "kernel");
	glUniform3fv(uniformLoc, 64, &kernelAO[0].x);

	uniformLoc = glGetUniformLocation(myShader->shaderProgramID, "cameraSize");
	glUniform1f(uniformLoc,currCam->orthoSize);

	quadRenderer->RenderQuad();

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, 0);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, 0);
	myShader->Unbind();
}

void PostProcessFilterAO::PopulateKernel()
{
	LCG randomizer;
	for (int i = 0; i < 64; ++i)//TODO this number is hardcoded for the moment
	{
		kernelAO.push_back(float3::RandomSphere(randomizer, float3::zero, 1));
	}
}
