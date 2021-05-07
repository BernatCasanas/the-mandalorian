#pragma once

class PostProcessFilterContrastTest;
class PostProcessFilterDepthTest;
class PostProcessFilterAO;
class PostProcessFilterBlurH;
class PostProcessFilterBlurV;
class PostProcessFilterBrighterThan;
class PostProcessFilterCombine;
class PostProcessFilterMultiply;
class PostProcessFilterRender;

class DE_Advanced_FrameBuffer;
class C_Camera;

class PostProcessDataAO;
class PostProcessDataBloom;

class PostProcessEffect
{
public:
	PostProcessEffect();
	~PostProcessEffect();
	
	virtual void Init()=0;
	virtual void CleanUp()=0;
	bool GetActive()const;
	void SetActive(bool active);

protected:
	bool active;
};

class PostProcessEffectBloom : public PostProcessEffect
{
public:
	PostProcessEffectBloom();
	~PostProcessEffectBloom();

	void Init() override;
	void CleanUp() override;
	//returns the index of the color texture that has been rendered
	int Render(int width, int height, int colorTexture, PostProcessDataBloom* aoVars);

private:
	PostProcessFilterBrighterThan* brighterThanFilter;
	PostProcessFilterBlurH* blurHFilter;
	PostProcessFilterBlurV* blurVFilter;
	PostProcessFilterCombine* combineFilter;
};

class PostProcessEffectDepthTest : public PostProcessEffect
{
public:
	PostProcessEffectDepthTest();
	~PostProcessEffectDepthTest();

	void Init() override;
	void CleanUp() override;
	//returns the index of the color texture that has been rendered
	int Render(int width, int height, int colorTexture, int depthTexture);
private:
	PostProcessFilterDepthTest* depthFilter;
};

class PostProcessEffectRender : public PostProcessEffect
{
public:
	PostProcessEffectRender();
	~PostProcessEffectRender();

	void Init() override;
	void CleanUp() override;
	//renders the color texture to the output FBO
	void Render(int width, int height, int colorTexture, DE_Advanced_FrameBuffer& outputFBO);
private:
	PostProcessFilterRender* renderPostProcess;
};


class PostProcessEffectAO: public PostProcessEffect
{
public:
	PostProcessEffectAO();
	~PostProcessEffectAO();

	void Init() override;
	void CleanUp() override;
	//returns the index of the color texture that has been rendered
	int Render(int width, int height, int colorTexture, int depthTexture,C_Camera* camera, PostProcessDataAO* aoVars);
private:
	PostProcessFilterAO* aoFilter;
	PostProcessFilterBlurH* blurHFilter;
	PostProcessFilterBlurV* blurVFilter;
	PostProcessFilterMultiply* multiplyFilter;
};