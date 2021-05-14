#pragma once
#include <vector>
#include "MathGeoLib/include/MathGeoLibFwd.h"

class DE_Advanced_FrameBuffer;
class ResourceShader;
class ImageRenderer;
class C_Camera;

class PostProcessDataVignette;

class PostProcessFilter
{
public:
	PostProcessFilter(int shaderUID, bool ownFBO);
	~PostProcessFilter();

	void CleanUp();
	int GetOutputTexture()const;
	DE_Advanced_FrameBuffer* GetOutputFBO();
	bool HasOwnFBO()const;
	bool TryLoadShader();//This is here in case we erase lib
protected:
	unsigned int myShaderUID;
	ResourceShader* myShader;
	ImageRenderer* quadRenderer;
};

class PostProcessFilterContrastTest : public PostProcessFilter
{
public:
	PostProcessFilterContrastTest();
	~PostProcessFilterContrastTest();
	void Render(bool isHDR,int width, int height, unsigned int colorTexture);//ony one render will be needed for real PostProcessFilters
	void Render(DE_Advanced_FrameBuffer* outputFBO, unsigned int colorTexture);

};

class PostProcessFilterDepthTest : public PostProcessFilter
{
public:
	PostProcessFilterDepthTest();
	~PostProcessFilterDepthTest();
	void Render(bool isHDR,int width, int height, unsigned int colorTexture, unsigned int depthTexture);//ony one render will be needed for real PostProcessFilters
	void Render(DE_Advanced_FrameBuffer* outputFBO, unsigned int colorTexture, unsigned int depthTexture);

};

class PostProcessFilterAO : public PostProcessFilter
{
public:
	PostProcessFilterAO();
	~PostProcessFilterAO();
	void Render(bool isHDR, int width, int height, unsigned int depthTexture, C_Camera* currCam, float sampleRad, float bias, bool fastAO);
	void PopulateKernel();
	void GenerateNoiseTexture();
	private:
	std::vector<float3> kernelAO;
	unsigned int noiseTexture; //random rotation vectors for the kernel
};

class PostProcessFilterRender : public PostProcessFilter
{
public:
	PostProcessFilterRender();
	~PostProcessFilterRender();
	void Render(bool isHDR,int width, int height, unsigned int colorTexture);
};

class PostProcessFilterBlurH : public PostProcessFilter
{
public:
	PostProcessFilterBlurH();
	~PostProcessFilterBlurH();
	void Render(bool isHDR,int width, int height, unsigned int texture, float blurSpread);

	void PopulateKernel();
	std::vector<float> gaussianKernel;
};


class PostProcessFilterBlurV : public PostProcessFilter
{
public:
	PostProcessFilterBlurV();
	~PostProcessFilterBlurV();
	void Render(bool isHDR,int width, int height, unsigned int texture, float blurSpread);

	void PopulateKernel();
	std::vector<float> gaussianKernel;
};

class PostProcessFilterBrighterThan : public PostProcessFilter
{
public:
	PostProcessFilterBrighterThan();
	~PostProcessFilterBrighterThan();
	void Render(bool isHDR,int width, int height, unsigned int colorTexture,float brightnessTreshold,bool useSmoothMask);//smooth mask lets you choose between a gradient in the glowing areas and binary glow/no glow
};


class PostProcessFilterCombine : public PostProcessFilter
{
public:
	PostProcessFilterCombine();
	~PostProcessFilterCombine();
	void Render(bool isHDR,int width, int height, unsigned int colorTexture, unsigned int brightnessTexture, float brightnessIntensity);
};


class PostProcessFilterMultiply : public PostProcessFilter
{
public:
	PostProcessFilterMultiply();
	~PostProcessFilterMultiply();
	void Render(bool isHDR,int width, int height, unsigned int texture1, unsigned int texture2);
};

class PostProcessFilterToneMapping : public PostProcessFilter
{
public:
	PostProcessFilterToneMapping();
	~PostProcessFilterToneMapping();
	void Render(bool isHDR, int width, int height, unsigned int colorTexture, float exposure, float gamma);
};

class PostProcessFilterVignette : public PostProcessFilter
{
public:
	PostProcessFilterVignette();
	~PostProcessFilterVignette();
	void Render(bool isHDR, int width, int height, unsigned int colorTexture, PostProcessDataVignette* vignetteData);
};