#include "PostProcessEffect.h"

#include "PostProcessFilter.h"
#include "DE_Advanced_FrameBuffer.h"
#include "CO_Camera.h"

//A post processing effect consists of 1 or more filters (Ex. Blur needs horizontal blur filter then Vertical blur filter)
PostProcessEffect::PostProcessEffect() :active(true)
{
}

PostProcessEffect::~PostProcessEffect()
{
}

bool PostProcessEffect::GetActive() const
{
	return active;
}

void PostProcessEffect::SetActive(bool active)
{
	this->active = active;
}

PostProcessEffectInvertTest::PostProcessEffectInvertTest() : PostProcessEffect(),
invertFilter(nullptr)
{
	Init();
}

PostProcessEffectInvertTest::~PostProcessEffectInvertTest()
{
	CleanUp();
}

void PostProcessEffectInvertTest::Init()
{
	invertFilter = new PostProcessFilterContrastTest();
}

void PostProcessEffectInvertTest::CleanUp()
{
	if (invertFilter != nullptr)
	{
		delete(invertFilter);
		invertFilter = nullptr;
	}
}

int PostProcessEffectInvertTest::Render(int width, int height, int colorTexture)
{
	invertFilter->Render(width, height, colorTexture);
	return invertFilter->GetOutputTexture();
}

PostProcessEffectDepthTest::PostProcessEffectDepthTest(): PostProcessEffect(),
depthFilter(nullptr)
{
	Init();
}

PostProcessEffectDepthTest::~PostProcessEffectDepthTest()
{
	CleanUp();
}

void PostProcessEffectDepthTest::Init()
{
	depthFilter = new PostProcessFilterDepthTest();
}

void PostProcessEffectDepthTest::CleanUp()
{
	if (depthFilter != nullptr)
	{
		delete(depthFilter);
		depthFilter = nullptr;
	}
}

int PostProcessEffectDepthTest::Render(int width, int height, int colorTexture, int depthTexture)
{
	depthFilter->Render(width, height, colorTexture, depthTexture);
	return depthFilter->GetOutputTexture();
}

PostProcessEffectRender::PostProcessEffectRender(): PostProcessEffect(),
renderPostProcess(nullptr)
{
	Init();
}

PostProcessEffectRender::~PostProcessEffectRender()
{
	CleanUp();
}

void PostProcessEffectRender::Init()
{
	renderPostProcess = new PostProcessFilterRender();
}

void PostProcessEffectRender::CleanUp()
{
	if (renderPostProcess != nullptr)
	{
		delete(renderPostProcess);
		renderPostProcess = nullptr;
	}
}

void PostProcessEffectRender::Render(int width, int height, int colorTexture, DE_Advanced_FrameBuffer& outputFBO)
{
	renderPostProcess->Render(width, height, colorTexture);
	renderPostProcess->GetOutputFBO()->ResolveToFBO(outputFBO);
}

PostProcessEffectAO::PostProcessEffectAO(): PostProcessEffect(),
aoFilter(nullptr)
{
	Init();
}

PostProcessEffectAO::~PostProcessEffectAO()
{
	CleanUp();
}

void PostProcessEffectAO::Init()
{
	aoFilter = new PostProcessFilterAO();
}

void PostProcessEffectAO::CleanUp()
{
	if (aoFilter != nullptr)
	{
		delete(aoFilter);
		aoFilter = nullptr;
	}
}

int PostProcessEffectAO::Render(int width, int height, int depthTexture,C_Camera* camera)
{
	aoFilter->Render(width, height, depthTexture,camera);
	return aoFilter->GetOutputTexture();
}
