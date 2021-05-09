#pragma once
#include "Application.h"
#include "Module.h"
#include "Light.h"

#include <queue>

#include "OpenGL.h"
#include"MathGeoLib/include/Math/float3.h"
#include"MathGeoLib/include/Geometry/LineSegment.h"

#include"DE_Cubemap.h"
#include "PostProcessing.h"

#include<map>

class ResourceMesh;
class ResourceTexture;
class ResourceMaterial;

class GameObject;
class C_MeshRenderer;
class C_Camera;
class C_DirectionalLight;
class C_AreaLight;

#define MAX_DIRECTIONAL_LIGHTS 2	//IF YOU MODIFY THIS VALUE, YOU MUST MODIFY THE SIZE OF THE ARRAY IN THE SHADER
#define MAX_AREA_LIGHTS 5			//IF YOU MODIFY THIS VALUE, YOU MUST MODIFY THE SIZE OF THE ARRAY IN THE SHADER

#define SQUARE_TEXTURE_W 256
#define SQUARE_TEXTURE_H 256

struct LineRender
{
	LineRender(float3& _a, float3& _b, float3& _color, float _width = 10.f) : a(_a), b(_b), color(_color), width(_width){}
	float3 a, b, color;
	float width;
};

struct DebugTriangle
{
	DebugTriangle(float3& _a, float3& _b, float3& _c, float3& _color) : a(_a), b(_b), c(_c), color(_color) {}
	float3 a, b, c, color;
};

struct DebugPoint
{
	DebugPoint(float3& _position, float3& _color) : position(_position), color(_color) {}
	float3 position, color;
};

class ModuleRenderer3D : public Module
{
public:
	ModuleRenderer3D(Application* app, bool start_enabled = true);
	virtual ~ModuleRenderer3D();

	bool Init() override;
	update_status PreUpdate(float dt) override;
	update_status PostUpdate(float dt) override;
	bool CleanUp() override;

	void OnResize(int width, int height);

#ifndef STANDALONE
	void OnGUI() override;

	void DrawDebugLines();
	void AddDebugLines(float3& a, float3& b, float3& color);
	void AddDebugTriangles(float3& a, float3& b, float3& c, float3& color);
	void AddDebugPoints(float3& position, float3& color);
	static void DrawBox(float3* points, float3 color = float3::one);
#endif // !STANDALONE

	
	void AddRay(float3& a, float3& b, float3& color, float& rayWidth);
	void DrawRays();
	void RayToMeshQueueIntersection(LineSegment& ray);

	C_Camera* GetGameRenderTarget()const;
	void SetGameRenderTarget(C_Camera* cam);

	void ClearAllRenderData();

	bool IsWalkable(float3 pointToCheck);

	void AddDirectionalLight(C_DirectionalLight* light);
	void RemoveDirectionalLight(C_DirectionalLight* light);

	void AddAreaLight(C_AreaLight* light);
	void RemoveAreaLight(C_AreaLight* light);

	void PushLightUniforms(ResourceMaterial* material);

private:

	void RenderWithOrdering(bool rTex = false);
	void RenderStencilWithOrdering(bool rTex = false);

#ifndef STANDALONE
	void DebugLine(LineSegment& line);
#endif // !STANDALONE
	
	void GetCAPS(std::string& caps);

	void DrawParticleSystems();

public:
	std::vector<C_MeshRenderer*> renderQueue;
	std::vector<C_MeshRenderer*> renderQueueStencil;
	std::vector<C_MeshRenderer*> renderQueuePostStencil;
	std::multimap<float, C_MeshRenderer*> renderQueueMap;
	std::multimap<float, C_MeshRenderer*> renderQueueMapStencil;
	std::multimap<float, C_MeshRenderer*> renderQueueMapPostStencil;

	std::vector<GameObject*> particleSystemQueue;

	std::vector<LineSegment> walkablePoints;

	Light lights[1];
	SDL_GLContext context;

	C_Camera* activeRenderCamera = nullptr; //TODO: This is temporal
	DE_Cubemap skybox;

	std::vector<C_DirectionalLight*> directLightVector;
	std::vector<C_AreaLight*> areaLightVector;

public:
	bool vsync, wireframe, cull, lightng, color_material, texture_2d;

	GLuint checkersTexture;
	GLubyte checkerImage[SQUARE_TEXTURE_W][SQUARE_TEXTURE_H][4];

	GLuint defaultNormalMap;
	GLubyte defaultNormalMapImage[SQUARE_TEXTURE_W][SQUARE_TEXTURE_H][4];

	GLuint defaultSpecularMap;
	GLubyte defaultSpecularMapImage[SQUARE_TEXTURE_W][SQUARE_TEXTURE_H][4];


	unsigned int resolution;
	bool debugDraw;

	PostProcessing postProcessing;
private:
	std::vector<LineRender> lines;
	std::vector<DebugTriangle> triangles;
	std::vector<DebugPoint> points;
	std::vector<LineRender> rays;
	C_Camera* gameCamera;
	LineSegment pickingDebug;
	std::string str_CAPS;

};