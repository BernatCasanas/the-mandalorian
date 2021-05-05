#include "PostProcessEffect.h"

#include "PostProcessFilter.h"
#include "DE_Advanced_FrameBuffer.h"

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
