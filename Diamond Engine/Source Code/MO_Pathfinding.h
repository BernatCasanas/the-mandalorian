#pragma once
#include "Module.h"
#include "Application.h"
#include "RecastNavigation/Recast/Recast.h"
#include "RecastNavigation/Detour/DetourNavMesh.h"
#include "RecastNavigation/Detour/DetourNavMeshQuery.h"

#include "MathGeoLib/include/MathGeoLib.h"

class dtNavMesh;
class dtNavMeshQuery;
class dtQueryFilter;
class GameObject;
class InputGeom;
class NavMeshBuilder;

struct NavAgent 
{
	NavAgent();
	float radius = 0.0f;
	float height = 0.0f;
	float stopHeight = 0.0f;
	int maxSlope = 0;
};

enum class PathType 
{
	SMOOTH,
	STRAIGHT
};

struct Pathfinder
{

	Pathfinder();
	~Pathfinder();
	void Init(NavMeshBuilder* builder);
	void CleanUp();

	bool CalculatePath();
	bool CalculatePath(float3 origin, float3 destination, std::vector<float3>& path);
	void RenderPath();

	PathType pathType;

	dtNavMesh* m_navMesh;
	dtNavMeshQuery* m_navQuery;
	dtQueryFilter m_filter;
	NavMeshBuilder* m_navMeshBuilder;

	int m_straightPathOptions;

	static const int MAX_POLYS = 256;
	static const int MAX_SMOOTH = 2048;

	dtPolyRef m_startRef;
	dtPolyRef m_endRef;
	dtPolyRef m_polys[MAX_POLYS];
	dtPolyRef m_parent[MAX_POLYS];
	int m_npolys;

	int m_nstraightPath;
	float m_straightPath[MAX_POLYS * 3];
	unsigned char m_straightPathFlags[MAX_POLYS];
	dtPolyRef m_straightPathPolys[MAX_POLYS];

	int m_nsmoothPath;
	float m_smoothPath[MAX_SMOOTH * 3];

	float3 startPosition;
	bool startPosSet;
	float3 endPosition;
	bool endPosSet;

	int m_pathIterNum;
	float m_polyPickExt[3];
};

class M_Pathfinding : public Module {
public:
	M_Pathfinding(Application* app, bool start_enabled = true);
	~M_Pathfinding();

	bool Init() override;
	update_status Update(float dt);

	int Save(const char* scene_path);
	void Load(int navMeshResourceUID);

#ifndef STANDALONE
	void DebugDraw();
	void CheckNavMeshIntersection(LineSegment raycast, int clickedMouseButton);
	void CreateWalkabilityTestPoint();
#endif // !STANDALONE

	void ClearNavMeshes();
	bool IsWalkable(float x, float z, float3& hitPoint);

	bool CleanUp() override;

	void BakeNavMesh();
	void AddGameObjectToNavMesh(GameObject* objectToAdd);
	NavMeshBuilder* GetNavMeshBuilder();
	float3 FindRandomPointAround(float3 centerPoint, float radius);
	bool FindPath(float3 origin, float3 destination, std::vector<float3>& path);

private:

public: 
	std::vector<NavAgent> agents;
	NavAgent bakedNav;
	Pathfinder pathfinder;
	bool debugDraw;

private:
	NavMeshBuilder* navMeshBuilder;
	InputGeom* geometry;

	GameObject* walkabilityPoint;
	
	//Random Point Finding
	bool randomPointSet;
	float3 randomPoint;
	float randomRadius;
};
