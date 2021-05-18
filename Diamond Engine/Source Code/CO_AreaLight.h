#ifndef __CO_AREA_LIGHT_H__
#define __CO_AREA_LIGHT_H__	

#include "Component.h"

#include"MathGeoLib/include/Math/float4x4.h"
#include"MathGeoLib/include/Geometry/Frustum.h"

class ResourceShader;
class ResourceMaterial;

class C_AreaLight : public Component
{
public:
	C_AreaLight(GameObject* gameObject);
	~C_AreaLight() override;

	void Update() override;

#ifndef STANDALONE
	bool OnEditor() override;
	void DebugDraw();
#endif // !STANDALONE

	static inline TYPE GetType() { return TYPE::AREA_LIGHT; }; //This will allow us to get the type from a template

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

	void Enable() override;
	void Disable() override;

	void StartPass();
	void PushLightUniforms(ResourceMaterial* material, int lightNumber);
	void EndPass();

	float3 GetPosition() const;

	float GetLightIntensity() const;
	void SetLightIntensity(float lIntensity);

	float GetFadeDistance() const;
	void SetFadeDistance(float fDistance);

	float GetMaxDistance() const;
	void SetMaxDistance(float mDistance);

public:
	Frustum shadowTransforms[6];
	ResourceShader* depthShader = nullptr;

	bool calculateShadows = true;

	unsigned int depthCubemap = 0;
private:
	unsigned int depthCubemapFBO = 0;

	float3 lightColor;
	float3 ambientLightColor;
	float lightIntensity;
	float specularValue;

	float maxDistance;
	float fadeDistance;
};

const float color[] = { 0.95, 0.95, 0.95 };

const float arrayAreaLightVAO[] = {
-1, -1,
1, -1,
-1, 1,
1, -1,
1, 1,
-1, 1,
};

#endif // !__CO_AREA_LIGHT_H__
