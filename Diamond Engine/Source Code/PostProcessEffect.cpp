#include "PostProcessEffect.h"

#include "PostProcessFilter.h"
#include "DE_Advanced_FrameBuffer.h"
#include "CO_Camera.h"
#include "RE_PostProcess.h"

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

int PostProcessEffectBloom::Render(bool isHDR, int width, int height, int colorTexture, PostProcessDataBloom* bloomVars)
{
	brighterThanFilter->Render(isHDR, width / 2, height / 2, colorTexture, bloomVars->brightThreshold, bloomVars->smoothMask);
	blurVFilter->Render(isHDR, width / 4, height / 4, brighterThanFilter->GetOutputTexture(), bloomVars->blurSpread);
	blurHFilter->Render(isHDR, width / 4, height / 4, blurVFilter->GetOutputTexture(), bloomVars->blurSpread);
	blurVFilter->Render(isHDR, width / 8, height / 8, blurHFilter->GetOutputTexture(), bloomVars->blurSpread);
	blurHFilter->Render(isHDR, width / 8, height / 8, blurVFilter->GetOutputTexture(), bloomVars->blurSpread);
	combineFilter->Render(isHDR, width, height, colorTexture, blurHFilter->GetOutputTexture(), bloomVars->brightnessIntensity);//TODO change the value for the actual value once we have the resource
	return combineFilter->GetOutputTexture();
}

PostProcessEffectDepthTest::PostProcessEffectDepthTest() : PostProcessEffect(),
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

int PostProcessEffectDepthTest::Render(bool isHDR, int width, int height, int colorTexture, int depthTexture)
{
	depthFilter->Render(isHDR, width, height, colorTexture, depthTexture);
	return depthFilter->GetOutputTexture();
}

PostProcessEffectRender::PostProcessEffectRender() : PostProcessEffect(),
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

void PostProcessEffectRender::Render(bool isHDR, int width, int height, int colorTexture, DE_Advanced_FrameBuffer& outputFBO)
{
	renderPostProcess->Render(isHDR, width, height, colorTexture);
	renderPostProcess->GetOutputFBO()->ResolveToFBO(outputFBO);
}

PostProcessEffectAO::PostProcessEffectAO() : PostProcessEffect(),
aoFilter(nullptr), blurHFilter(nullptr), blurVFilter(nullptr),
multiplyFilter(nullptr)
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
	blurVFilter = new PostProcessFilterBlurV();
	multiplyFilter = new PostProcessFilterMultiply();
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
	if (blurVFilter != nullptr)
	{
		delete(blurVFilter);
		blurVFilter = nullptr;
	}
	if (multiplyFilter != nullptr)
	{
		delete(multiplyFilter);
		multiplyFilter = nullptr;
	}
}

int PostProcessEffectAO::Render(bool isHDR, int width, int height, int colorTexture, int depthTexture, C_Camera* camera, PostProcessDataAO* aoVars)
{
	aoFilter->Render(isHDR, width, height, depthTexture, camera, aoVars->radiusAO,aoVars->bias,aoVars->fastAO);
	if (aoVars->useBlur)
	{
		blurVFilter->Render(isHDR, width, height, aoFilter->GetOutputTexture(), aoVars->blurSpread);
		blurHFilter->Render(isHDR, width, height, blurVFilter->GetOutputTexture(), aoVars->blurSpread);
		multiplyFilter->Render(isHDR, width, height, colorTexture, blurHFilter->GetOutputTexture());
	}
	else
	{
		multiplyFilter->Render(isHDR, width, height, colorTexture, aoFilter->GetOutputTexture());
	}

	return multiplyFilter->GetOutputTexture();

	//Test Code
	/*aoFilter->Render(isHDR,width, height, depthTexture,camera,aoVars->radiusAO);
	return aoFilter->GetOutputTexture();*/
}

PostProcessEffectToneMapping::PostProcessEffectToneMapping() : PostProcessEffect(),
toneMappingFilter(nullptr)
{
	Init();
}

PostProcessEffectToneMapping::~PostProcessEffectToneMapping()
{
	CleanUp();
}

void PostProcessEffectToneMapping::Init()
{
	toneMappingFilter = new PostProcessFilterToneMapping();

}

void PostProcessEffectToneMapping::CleanUp()
{
	if (toneMappingFilter != nullptr)
	{
		delete(toneMappingFilter);
		toneMappingFilter = nullptr;
	}
}

int PostProcessEffectToneMapping::Render(bool isHDR, int width, int height, int colorTexture, PostProcessDataToneMapping* toneMappingVars)
{
	toneMappingFilter->Render(isHDR, width, height, colorTexture, toneMappingVars->exposure, toneMappingVars->gamma);
	return toneMappingFilter->GetOutputTexture();
}


PostProcessEffectVignette::PostProcessEffectVignette() : PostProcessEffect(),
vignetteFilter(nullptr)
{
	Init();
}

PostProcessEffectVignette::~PostProcessEffectVignette()
{
	CleanUp();
}

void PostProcessEffectVignette::Init()
{
	vignetteFilter = new PostProcessFilterVignette();

}

void PostProcessEffectVignette::CleanUp()
{
	if (vignetteFilter != nullptr)
	{
		delete(vignetteFilter);
		vignetteFilter = nullptr;
	}
}

int PostProcessEffectVignette::Render(bool isHDR, int width, int height, int colorTexture, PostProcessDataVignette* vignetteVars)
{
	vignetteFilter->Render(isHDR, width, height, colorTexture, vignetteVars);
	return vignetteFilter->GetOutputTexture();
}