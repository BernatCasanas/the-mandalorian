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

PostProcessEffectBloom::PostProcessEffectBloom() : PostProcessEffect(),
brighterThanFilter(nullptr), blurHFilter(nullptr), blurVFilter(nullptr),
combineFilter(nullptr)
{
	Init();
}

PostProcessEffectBloom::~PostProcessEffectBloom()
{
	CleanUp();
}

void PostProcessEffectBloom::Init()
{
	brighterThanFilter = new PostProcessFilterBrighterThan();
	blurHFilter = new PostProcessFilterBlurH();
	blurVFilter = new PostProcessFilterBlurV();
	combineFilter = new PostProcessFilterCombine();
}

void PostProcessEffectBloom::CleanUp()
{
	if (brighterThanFilter != nullptr)
	{
		delete(brighterThanFilter);
		brighterThanFilter = nullptr;
	}
	if (blurHFilter != nullptr)
	{
		delete(blurHFilter);
		blurHFilter = nullptr;
	}
	if (blurVFilter != nullptr)
	{
		delete(blurVFilter);
		blurVFilter = nullptr;
	}
	if (combineFilter != nullptr)
	{
		delete(combineFilter);
		combineFilter = nullptr;
	}
}

int PostProcessEffectBloom::Render(int width, int height, int colorTexture)
{
	brighterThanFilter->Render(width/2, height/2, colorTexture,0.5f);//TODO change the value for the actual value once we have the resource
	blurVFilter->Render(width/4, height/4, brighterThanFilter->GetOutputTexture());
	blurHFilter->Render(width/4, height/4, blurVFilter->GetOutputTexture());
	blurVFilter->Render(width / 8, height / 8, blurHFilter->GetOutputTexture());
	blurHFilter->Render(width / 8, height / 8, blurVFilter->GetOutputTexture());
	combineFilter->Render(width, height, colorTexture, blurHFilter->GetOutputTexture(), 1.0);//TODO change the value for the actual value once we have the resource
	return combineFilter->GetOutputTexture();
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
	blurHFilter = new PostProcessFilterBlurH();

}

void PostProcessEffectAO::CleanUp()
{
	if (aoFilter != nullptr)
	{
		delete(aoFilter);
		aoFilter = nullptr;
	}
	if (blurHFilter != nullptr)
	{
		delete(blurHFilter);
		blurHFilter = nullptr;
	}
}

int PostProcessEffectAO::Render(int width, int height, int depthTexture,C_Camera* camera)
{
	//aoFilter->Render(width, height, depthTexture,camera);
	blurHFilter->Render(width, height, depthTexture);//TODO not depth texture but ao
	return blurHFilter->GetOutputTexture();
}
