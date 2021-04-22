#include "MO_Pathfinding.h"
#include "MO_Camera3D.h"
#include "MO_Renderer3D.h"
#include "MO_Input.h"
#include "MO_Scene.h"

#include "RE_Mesh.h"
#include "CO_Transform.h"
#include "CO_MeshRenderer.h"
#include "GameObject.h"

#include "NavMeshBuilder.h"
#include "IM_NavMeshImporter.h"
#include "MO_ResourceManager.h"
#include "IM_FileSystem.h"

#include "DETime.h"

#include "RecastNavigation/DebugUtils/DetourDebugDraw.h"
#include "RecastNavigation/InputGeom.h"
#include "RecastNavigation/Detour/DetourNavMesh.h"
#include "RecastNavigation/Detour/DetourNavMeshQuery.h"
#include "RecastNavigation/Detour/DetourNavMeshBuilder.h"
#include "RecastNavigation/Detour/DetourCommon.h"
#include "WI_Pathfinding.h"
#include "mmgr/mmgr.h"
#include "RecastNavigation/DebugUtils/SampleInterfaces.h"

M_Pathfinding::M_Pathfinding(Application* app, bool start_enabled) : Module(app, start_enabled),
geometry(nullptr), navMeshBuilder(nullptr), walkabilityPoint(nullptr), debugDraw(false),
randomPointSet(false), randomRadius(0.0f)
{
	geometry = new InputGeom();
	geometry->SetMesh(new ResourceMesh(EngineExternal->GetRandomInt()));
	agents.push_back(NavAgent());

	randomPoint = float3::inf;

#ifndef STANDALONE
	debugDraw = true;
#endif // !STANDALONE
}

M_Pathfinding::~M_Pathfinding()
{
	agents.clear();
}

bool M_Pathfinding::Init()
{
	return true;
}

update_status M_Pathfinding::Update(float dt)
{
#ifndef STANDALONE
	if (navMeshBuilder != nullptr && EngineExternal->moduleInput->GetMouseButton(SDL_BUTTON_LEFT) == KEY_STATE::KEY_DOWN)
	{
		float hitTime;
		float rayStart[3];
		float rayEnd[3];
		bool hit = geometry->raycastMesh(rayStart, rayEnd, hitTime);
		if (hit)
		{
			pathfinder.startPosition = float3(rayEnd[0], rayEnd[1], rayEnd[2]);
			pathfinder.startPosSet = true;

			if (pathfinder.endPosSet)
			{
				pathfinder.CalculatePath();
				std::vector<float3> path;
				FindPath(pathfinder.startPosition, pathfinder.endPosition, path);
			}
		}
	}

	if (navMeshBuilder != nullptr && EngineExternal->moduleInput->GetMouseButton(SDL_BUTTON_RIGHT) == KEY_STATE::KEY_DOWN)
	{
		float hitTime;
		float rayStart[3];
		float rayEnd[3];
		bool hit = geometry->raycastMesh(rayStart, rayEnd, hitTime);

		if (hit)
		{
			pathfinder.endPosition = float3(rayEnd[0], rayEnd[1], rayEnd[2]);
			pathfinder.endPosSet = true;

			if (pathfinder.startPosSet)
			{
				pathfinder.CalculatePath();
				std::vector<float3> path;
				FindPath(pathfinder.startPosition, pathfinder.endPosition, path);
			}
		}
	}


	if (EngineExternal->moduleInput->GetKey(SDL_SCANCODE_F1) == KEY_DOWN)
	{
		randomPoint = FindRandomPointAround(pathfinder.startPosition, 5.0f);
	}
#endif // !STANDALONE

	return UPDATE_CONTINUE;
}

int M_Pathfinding::Save(const char* scene_path)
{
	std::string navMeshPath;
	FileSystem::GetFileName(scene_path, navMeshPath, false);

	navMeshPath = "Assets/NavMeshes/" + navMeshPath + ".nav";

	BuildSettings settings;

	rcVcopy(settings.navMeshBMin, geometry->getNavMeshBoundsMin());
	rcVcopy(settings.navMeshBMax, geometry->getNavMeshBoundsMax());

	if (navMeshBuilder == nullptr)
		return -1;

	navMeshBuilder->CollectSettings(settings);

	return NavMeshImporter::Save(navMeshPath.c_str(), navMeshBuilder->GetNavMesh(), settings);
}

void M_Pathfinding::Load(int navMeshResourceUID)
{
	BuildSettings settings;
	dtNavMesh* navMesh = NavMeshImporter::Load(navMeshResourceUID, settings);

	if (navMesh != nullptr)
		ClearNavMeshes();

	rcVcopy(geometry->m_meshBMin, settings.navMeshBMin );
	rcVcopy(geometry->m_meshBMax, settings.navMeshBMax );

	if (navMeshBuilder == nullptr)
		navMeshBuilder = new NavMeshBuilder();

	navMeshBuilder->SetNavMesh(navMesh);
	navMeshBuilder->SetSettings(settings);
	navMeshBuilder->SetGeometry(geometry);
	pathfinder.Init(navMeshBuilder);
}



#ifndef STANDALONE

void M_Pathfinding::DebugDraw()
{
	if (!debugDraw)
		return;
	
	if (navMeshBuilder == nullptr)
		return;

	navMeshBuilder->DebugDraw();

	float3 debugStartPoint = pathfinder.startPosition;
	debugStartPoint.y += 0.2f;
	float3 debugEndPoint = pathfinder.endPosition;
	debugEndPoint.y += 0.2f;

	if (pathfinder.startPosSet)
		EngineExternal->moduleRenderer3D->AddDebugPoints(debugStartPoint, float3(0.0f, 255.0f, 0.0f));

	if (pathfinder.endPosSet)
		EngineExternal->moduleRenderer3D->AddDebugPoints(debugEndPoint, float3(255.0f, 0.0f, 0.0f));

	if (pathfinder.m_npolys > 0 && pathfinder.m_navMesh != nullptr)
		pathfinder.RenderPath();

	if (walkabilityPoint != nullptr)
	{
		float3 position = walkabilityPoint->transform->position;
		float3 hitPoint;

		if (IsWalkable(position.x, position.z, hitPoint))
		{
			EngineExternal->moduleRenderer3D->AddDebugLines(float3(position.x, 20, position.z), float3(position.x, -20, position.z), float3(0.0f, 255.0f, 0.0f));
			EngineExternal->moduleRenderer3D->AddDebugPoints(hitPoint, float3(255.0f, 255.0f, 255.0f));
		}
		else
		{
			EngineExternal->moduleRenderer3D->AddDebugLines(float3(position.x, 20, position.z), float3(position.x, -20, position.z), float3(255.0f, 0.0f, 0.0f));
		}
	}

	if (randomPointSet)
	{
		DebugDrawGL dd;
		EngineExternal->moduleRenderer3D->AddDebugPoints(randomPoint, float3(255.0f, 255.0f, 255.0f));
		duDebugDrawCircle(&dd, pathfinder.startPosition.x, pathfinder.startPosition.y + 0.2f, pathfinder.startPosition.z, randomRadius, duRGBA(64, 16, 0, 220), 2.0f);
	}

}

void M_Pathfinding::CheckNavMeshIntersection(LineSegment raycast, int clickedMouseButton)
{
	if (navMeshBuilder == nullptr)
		return;

	if (geometry->getChunkyMesh() == nullptr && navMeshBuilder->GetNavMesh() == nullptr)
	{
		//return;
		BakeNavMesh();
		LOG(LogType::L_WARNING, "No chunky mesh set, one has been baked to avoid crashes");
	}

	float hitTime;
	bool hit = geometry->raycastMesh(raycast.a.ptr(), raycast.b.ptr(), hitTime);

	float3 hitPoint;
	hitPoint = raycast.a + (raycast.b - raycast.a) * hitTime;

	if (clickedMouseButton == SDL_BUTTON_LEFT)
	{
		if (hit)
		{
			pathfinder.startPosition = hitPoint;
			pathfinder.startPosSet = true;

			if (pathfinder.endPosSet)
			{
				pathfinder.CalculatePath();
				std::vector<float3> path;
				FindPath(pathfinder.startPosition, pathfinder.endPosition, path);
			}

			randomPointSet = false;
		}
	}
	else if (clickedMouseButton == SDL_BUTTON_RIGHT)
	{
		if (hit)
		{
			pathfinder.endPosition = hitPoint;
			pathfinder.endPosSet = true;

			if (pathfinder.startPosSet)
			{
				pathfinder.CalculatePath();
				std::vector<float3> path;
				FindPath(pathfinder.startPosition, pathfinder.endPosition, path);
			}
		}
	}
	else
	{
		LOG(LogType::L_ERROR, "How did you even get here?");
	}
}

void M_Pathfinding::CreateWalkabilityTestPoint()
{
	if (walkabilityPoint != nullptr)
		walkabilityPoint->Destroy();

	walkabilityPoint = EngineExternal->moduleScene->CreateGameObject("Walkability Test", EngineExternal->moduleScene->root);
}

#endif // !STANDALONE

void M_Pathfinding::ClearNavMeshes()
{
	CleanUp();

	geometry = new InputGeom();
	geometry->SetMesh(new ResourceMesh(EngineExternal->GetRandomInt()));

	pathfinder;
}

bool M_Pathfinding::IsWalkable(float x, float z, float3& hitPoint)
{
	float3 upperPoint = float3(x, 20.0f, z);
	float3 lowerPoint = float3(x, -20, z);
	float hitTime;

	bool walkable = geometry->raycastMesh(upperPoint.ptr(), lowerPoint.ptr(), hitTime);

	hitPoint = upperPoint + (lowerPoint - upperPoint) * hitTime;

	return walkable;
}

bool M_Pathfinding::CleanUp()
{
	if (geometry != nullptr)
	{
		delete geometry;
		geometry = nullptr;
	}

	if (navMeshBuilder != nullptr)
	{
		delete navMeshBuilder;
		navMeshBuilder = nullptr;
	}

	pathfinder.CleanUp();

	return true;
}

void M_Pathfinding::BakeNavMesh()
{
	ClearNavMeshes();

	std::vector<GameObject*> gameObjects;
	EngineExternal->moduleScene->root->CollectChilds(gameObjects);

	for (size_t i = 0; i < gameObjects.size(); i++)
	{
		if (gameObjects[i]->isStatic)
		{
			AddGameObjectToNavMesh(gameObjects[i]);
		}
	}

	pathfinder.Init(navMeshBuilder);
}

void M_Pathfinding::AddGameObjectToNavMesh(GameObject* objectToAdd)
{
	C_MeshRenderer* meshRenderer = dynamic_cast<C_MeshRenderer*>(objectToAdd->GetComponent(Component::TYPE::MESH_RENDERER));

	if (meshRenderer == nullptr)
		return;

	ResourceMesh* mesh = meshRenderer->GetRenderMesh();

	if (mesh == nullptr)
		return;

	float4x4 globalTransform = objectToAdd->transform->globalTransform;

	geometry->AddMesh(mesh, globalTransform);

	if (navMeshBuilder == nullptr)
		navMeshBuilder = new NavMeshBuilder();

	navMeshBuilder->HandleMeshChanged(geometry, bakedNav);
	navMeshBuilder->HandleSettings();
	navMeshBuilder->HandleBuild();

	//pathfinder.Init(navMeshBuilder);
}

NavMeshBuilder* M_Pathfinding::GetNavMeshBuilder()
{
	return navMeshBuilder;
}

static float frand()
{
	return (float)rand() / (float)RAND_MAX;
}

float3 M_Pathfinding::FindRandomPointAround(float3 centerPoint, float radius)
{
	if (navMeshBuilder == nullptr)
		return centerPoint;

	dtPolyRef polyRef;
	navMeshBuilder->GetNavMeshQuery()->findNearestPoly(centerPoint.ptr(), pathfinder.m_polyPickExt, &pathfinder.m_filter, &polyRef, 0);

	float3 randomPoint;

	dtPolyRef ref;
	dtStatus status = navMeshBuilder->GetNavMeshQuery()->findRandomPointAroundCircle(polyRef, centerPoint.ptr(), radius, &pathfinder.m_filter, frand, &ref, randomPoint.ptr());

	if (dtStatusSucceed(status))
	{
		randomPointSet = true;
		randomRadius = radius;
	}

	if (dtStatusFailed(status) || (status & DT_STATUS_DETAIL_MASK))
		return centerPoint;

	return randomPoint;
}

bool M_Pathfinding::FindPath(float3 origin, float3 destination, std::vector<float3>& path)
{
	return pathfinder.CalculatePath(origin, destination, path);
}

Pathfinder::Pathfinder() : m_navQuery(nullptr),
m_navMesh(nullptr), m_navMeshBuilder(nullptr), endPosSet(false), startPosSet(false),
m_npolys(0), m_nsmoothPath(0), pathType(PathType::STRAIGHT),
m_startRef(0), m_endRef(0), 
m_nstraightPath(0), m_pathIterNum(0)
{
	m_polyPickExt[0] = 32.0f;
	m_polyPickExt[1] = 32.0f;
	m_polyPickExt[2] = 32.0f;

	startPosition = float3::zero;
	endPosition = float3::zero;
}

Pathfinder::~Pathfinder()
{
	CleanUp();
}

void Pathfinder::Init(NavMeshBuilder* builder)
{
	if (builder == nullptr)
		return;

	m_navMeshBuilder = builder;

	m_navMesh = builder->GetNavMesh();
	m_navQuery = builder->GetNavMeshQuery();
}

void Pathfinder::CleanUp()
{
	if (m_navQuery != nullptr)
		m_navQuery = nullptr;

	m_navMesh = nullptr;
	m_navQuery = nullptr;
	m_navMeshBuilder = nullptr;
	startPosSet = false;
	endPosSet = false;
}

inline bool InRange(const float* v1, const float* v2, const float r, const float h)
{
	const float dx = v2[0] - v1[0];
	const float dy = v2[1] - v1[1];
	const float dz = v2[2] - v1[2];
	return (dx * dx + dz * dz) < r * r && fabsf(dy) < h;
}

static bool GetSteerTarget(dtNavMeshQuery* navQuery, const float* startPos, const float* endPos,
	const float minTargetDist, const dtPolyRef* path, const int pathSize,
	float* steerPos, unsigned char& steerPosFlag, dtPolyRef& steerPosRef,
	float* outPoints = 0, int* outPointCount = 0)
{
	// Find steer target.
	static const int MAX_STEER_POINTS = 3;
	float steerPath[MAX_STEER_POINTS * 3];
	unsigned char steerPathFlags[MAX_STEER_POINTS];
	dtPolyRef steerPathPolys[MAX_STEER_POINTS];
	int nsteerPath = 0;
	navQuery->findStraightPath(startPos, endPos, path, pathSize,
		steerPath, steerPathFlags, steerPathPolys, &nsteerPath, MAX_STEER_POINTS);
	if (!nsteerPath)
		return false;

	if (outPoints && outPointCount)
	{
		*outPointCount = nsteerPath;
		for (int i = 0; i < nsteerPath; ++i)
			dtVcopy(&outPoints[i * 3], &steerPath[i * 3]);
	}


	// Find vertex far enough to steer to.
	int ns = 0;
	while (ns < nsteerPath)
	{
		// Stop at Off-Mesh link or when point is further than slop away.
		if ((steerPathFlags[ns] & DT_STRAIGHTPATH_OFFMESH_CONNECTION) ||
			!InRange(&steerPath[ns * 3], startPos, minTargetDist, 1000.0f))
			break;
		ns++;
	}
	// Failed to find good point to steer to.
	if (ns >= nsteerPath)
		return false;

	dtVcopy(steerPos, &steerPath[ns * 3]);
	steerPos[1] = startPos[1];
	steerPosFlag = steerPathFlags[ns];
	steerPosRef = steerPathPolys[ns];

	return true;
}

static int fixupCorridor(dtPolyRef* path, const int npath, const int maxPath,
	const dtPolyRef* visited, const int nvisited)
{
	int furthestPath = -1;
	int furthestVisited = -1;

	// Find furthest common polygon.
	for (int i = npath - 1; i >= 0; --i)
	{
		bool found = false;
		for (int j = nvisited - 1; j >= 0; --j)
		{
			if (path[i] == visited[j])
			{
				furthestPath = i;
				furthestVisited = j;
				found = true;
			}
		}
		if (found)
			break;
	}

	// If no intersection found just return current path. 
	if (furthestPath == -1 || furthestVisited == -1)
		return npath;

	// Concatenate paths.	

	// Adjust beginning of the buffer to include the visited.
	const int req = nvisited - furthestVisited;
	const int orig = rcMin(furthestPath + 1, npath);
	int size = rcMax(0, npath - orig);
	if (req + size > maxPath)
		size = maxPath - req;
	if (size)
		memmove(path + req, path + orig, size * sizeof(dtPolyRef));

	// Store visited
	for (int i = 0; i < req; ++i)
		path[i] = visited[(nvisited - 1) - i];

	return req + size;
}

// This function checks if the path has a small U-turn, that is,
// a polygon further in the path is adjacent to the first polygon
// in the path. If that happens, a shortcut is taken.
// This can happen if the target (T) location is at tile boundary,
// and we're (S) approaching it parallel to the tile edge.
// The choice at the vertex can be arbitrary, 
//  +---+---+
//  |:::|:::|
//  +-S-+-T-+
//  |:::|   | <-- the step can end up in here, resulting U-turn path.
//  +---+---+
static int fixupShortcuts(dtPolyRef* path, int npath, dtNavMeshQuery* navQuery)
{
	if (npath < 3)
		return npath;

	// Get connected polygons
	static const int maxNeis = 16;
	dtPolyRef neis[maxNeis];
	int nneis = 0;

	const dtMeshTile* tile = 0;
	const dtPoly* poly = 0;
	if (dtStatusFailed(navQuery->getAttachedNavMesh()->getTileAndPolyByRef(path[0], &tile, &poly)))
		return npath;

	for (unsigned int k = poly->firstLink; k != DT_NULL_LINK; k = tile->links[k].next)
	{
		const dtLink* link = &tile->links[k];
		if (link->ref != 0)
		{
			if (nneis < maxNeis)
				neis[nneis++] = link->ref;
		}
	}

	// If any of the neighbour polygons is within the next few polygons
	// in the path, short cut to that polygon directly.
	static const int maxLookAhead = 6;
	int cut = 0;
	for (int i = dtMin(maxLookAhead, npath) - 1; i > 1 && cut == 0; i--) {
		for (int j = 0; j < nneis; j++)
		{
			if (path[i] == neis[j]) {
				cut = i;
				break;
			}
		}
	}
	if (cut > 1)
	{
		int offset = cut - 1;
		npath -= offset;
		for (int i = 1; i < npath; i++)
			path[i] = path[i + offset];
	}

	return npath;
}

bool Pathfinder::CalculatePath()
{
	bool validPath = true;

	if (m_navMesh == nullptr)
		return false;

	m_navQuery->findNearestPoly(startPosition.ptr(), m_polyPickExt, &m_filter, &m_startRef, nullptr);
	m_navQuery->findNearestPoly(endPosition.ptr(), m_polyPickExt, &m_filter, &m_endRef, nullptr);

	if (pathType == PathType::SMOOTH)
	{
		m_pathIterNum = 0;
		if (startPosSet && endPosSet && m_startRef && m_endRef)
		{
			m_navQuery->findPath(m_startRef, m_endRef, startPosition.ptr(), endPosition.ptr(), &m_filter, m_polys, &m_npolys, MAX_POLYS);

			m_nsmoothPath = 0;

			if (m_npolys)
			{
				// Iterate over the path to find smooth path on the detail mesh surface.
				dtPolyRef polys[MAX_POLYS];
				memcpy(polys, m_polys, sizeof(dtPolyRef) * m_npolys);
				int npolys = m_npolys;

				float iterPos[3], targetPos[3];
				m_navQuery->closestPointOnPoly(m_startRef, startPosition.ptr(), iterPos, 0);
				m_navQuery->closestPointOnPoly(polys[npolys - 1], endPosition.ptr(), targetPos, 0);

				static const float STEP_SIZE = 0.5f;
				static const float SLOP = 0.01f;

				m_nsmoothPath = 0;

				dtVcopy(&m_smoothPath[m_nsmoothPath * 3], iterPos);
				m_nsmoothPath++;

				// Move towards target a small advancement at a time until target reached or
				// when ran out of memory to store the path.
				while (npolys && m_nsmoothPath < MAX_SMOOTH)
				{
					// Find location to steer towards.
					float steerPos[3];
					unsigned char steerPosFlag;
					dtPolyRef steerPosRef;

					if (!GetSteerTarget(m_navQuery, iterPos, targetPos, SLOP,
						polys, npolys, steerPos, steerPosFlag, steerPosRef))
						break;

					bool endOfPath = (steerPosFlag & DT_STRAIGHTPATH_END) ? true : false;
					bool offMeshConnection = (steerPosFlag & DT_STRAIGHTPATH_OFFMESH_CONNECTION) ? true : false;

					// Find movement delta.
					float delta[3], len;
					dtVsub(delta, steerPos, iterPos);
					len = dtMathSqrtf(dtVdot(delta, delta));
					// If the steer target is end of path or off-mesh link, do not move past the location.
					if ((endOfPath || offMeshConnection) && len < STEP_SIZE)
						len = 1;
					else
						len = STEP_SIZE / len;
					float moveTgt[3];
					dtVmad(moveTgt, iterPos, delta, len);

					// Move
					float result[3];
					dtPolyRef visited[16];
					int nvisited = 0;
					m_navQuery->moveAlongSurface(polys[0], iterPos, moveTgt, &m_filter,
						result, visited, &nvisited, 16);

					npolys = fixupCorridor(polys, npolys, MAX_POLYS, visited, nvisited);
					npolys = fixupShortcuts(polys, npolys, m_navQuery);

					float h = 0;
					m_navQuery->getPolyHeight(polys[0], result, &h);
					result[1] = h;
					dtVcopy(iterPos, result);

					// Handle end of path and off-mesh links when close enough.
					if (endOfPath && InRange(iterPos, steerPos, SLOP, 1.0f))
					{
						// Reached end of path.
						dtVcopy(iterPos, targetPos);
						if (m_nsmoothPath < MAX_SMOOTH)
						{
							dtVcopy(&m_smoothPath[m_nsmoothPath * 3], iterPos);
							m_nsmoothPath++;
						}
						break;
					}
					else if (offMeshConnection && InRange(iterPos, steerPos, SLOP, 1.0f))
					{
						// Reached off-mesh connection.
						float startPos[3], endPos[3];

						// Advance the path up to and over the off-mesh connection.
						dtPolyRef prevRef = 0, polyRef = polys[0];
						int npos = 0;
						while (npos < npolys && polyRef != steerPosRef)
						{
							prevRef = polyRef;
							polyRef = polys[npos];
							npos++;
						}
						for (int i = npos; i < npolys; ++i)
							polys[i - npos] = polys[i];
						npolys -= npos;

						// Handle the connection.
						dtStatus status = m_navMesh->getOffMeshConnectionPolyEndPoints(prevRef, polyRef, startPos, endPos);
						if (dtStatusSucceed(status))
						{
							if (m_nsmoothPath < MAX_SMOOTH)
							{
								dtVcopy(&m_smoothPath[m_nsmoothPath * 3], startPos);
								m_nsmoothPath++;
								// Hack to make the dotted path not visible during off-mesh connection.
								if (m_nsmoothPath & 1)
								{
									dtVcopy(&m_smoothPath[m_nsmoothPath * 3], startPos);
									m_nsmoothPath++;
								}
							}
							// Move position at the other side of the off-mesh link.
							dtVcopy(iterPos, endPos);
							float eh = 0.0f;
							m_navQuery->getPolyHeight(polys[0], iterPos, &eh);
							iterPos[1] = eh;
						}
					}

					// Store results.
					if (m_nsmoothPath < MAX_SMOOTH)
					{
						dtVcopy(&m_smoothPath[m_nsmoothPath * 3], iterPos);
						m_nsmoothPath++;
					}
				}
			}

		}
		else
		{
			m_npolys = 0;
			m_nsmoothPath = 0;
		}
	}
	else if (pathType == PathType::STRAIGHT)
	{
		if (startPosSet && endPosSet && m_startRef && m_endRef)
		{
			dtStatus status = m_navQuery->findPath(m_startRef, m_endRef, startPosition.ptr(),
				endPosition.ptr(), &m_filter, m_polys, &m_npolys, MAX_POLYS);

			if (dtStatusPartial(status) || dtStatusFailed(status))
				validPath = false;

			m_nstraightPath = 0;
			if (m_npolys)
			{
				// In case of partial path, make sure the end point is clamped to the last polygon.
				float epos[3];
				dtVcopy(epos, endPosition.ptr());
				if (m_polys[m_npolys - 1] != m_endRef)
					m_navQuery->closestPointOnPoly(m_polys[m_npolys - 1], endPosition.ptr(), epos, 0);

				m_navQuery->findStraightPath(startPosition.ptr(), epos, m_polys, m_npolys,
					m_straightPath, m_straightPathFlags,
					m_straightPathPolys, &m_nstraightPath, MAX_POLYS, m_straightPathOptions);
			}
		}
		else
		{
			m_npolys = 0;
			m_nstraightPath = 0;
			validPath = false;
		}
	}

	if (validPath) {
		LOG(LogType::L_NORMAL, "Valid Path");}
	else {
		LOG(LogType::L_ERROR, "Invalid Path");}

	return validPath;
}

bool Pathfinder::CalculatePath(float3 origin, float3 destination, std::vector<float3>& path)
{
	dtStatus status;
	float startNearest[3];
	float endNearest[3];

	if (m_navQuery == nullptr)
		return false;

	status = m_navQuery->findNearestPoly(origin.ptr(), m_polyPickExt, &m_filter, &m_startRef, startNearest);
	if (dtStatusFailed(status) || (status & DT_STATUS_DETAIL_MASK)) {
		LOG(LogType::L_ERROR, "Could not find a near poly to start path");
		return false;}

	status = m_navQuery->findNearestPoly(destination.ptr(), m_polyPickExt, &m_filter, &m_endRef, endNearest);
	if (dtStatusFailed(status) || (status & DT_STATUS_DETAIL_MASK)) {
		LOG(LogType::L_ERROR, "Could not find a near poly to end path");
		return false;}

	status = m_navQuery->findPath(m_startRef, m_endRef, origin.ptr(),
		destination.ptr(), &m_filter, m_polys, &m_npolys, MAX_POLYS);

	m_nstraightPath = 0;
	if (m_npolys)
	{
		// In case of partial path, make sure the end point is clamped to the last polygon.
		float epos[3];
		dtVcopy(epos, destination.ptr());
		if (m_polys[m_npolys - 1] != m_endRef)
			m_navQuery->closestPointOnPoly(m_polys[m_npolys - 1], destination.ptr(), epos, 0);

		m_navQuery->findStraightPath(origin.ptr(), epos, m_polys, m_npolys,
			m_straightPath, m_straightPathFlags,
			m_straightPathPolys, &m_nstraightPath, MAX_POLYS, m_straightPathOptions);
	}

	if (dtStatusFailed(status) || (status & DT_STATUS_DETAIL_MASK) || m_nstraightPath == 0) {
		//LOG(LogType::L_ERROR, "Could not create straight path");
		path.clear();
		return false;
	}

	path.resize(m_nstraightPath);
	memcpy(path.data(), m_straightPath, sizeof(float) * m_nstraightPath * 3);

	startPosition = origin;
	endPosition = destination;
	startPosSet = true;
	endPosSet = true;
}

void Pathfinder::RenderPath()
{
	DebugDrawGL dd;

	static const unsigned int startCol = duRGBA(128, 25, 0, 192);
	static const unsigned int endCol = duRGBA(51, 102, 0, 129);
	static const unsigned int pathCol = duRGBA(0, 0, 0, 64);

	if (pathType == PathType::SMOOTH)
	{
		duDebugDrawNavMeshPoly(&dd, *m_navMesh, m_startRef, startCol);
		duDebugDrawNavMeshPoly(&dd, *m_navMesh, m_endRef, endCol);

		if (m_npolys)
		{
			for (int i = 0; i < m_npolys; ++i)
			{
				if (m_polys[i] == m_startRef || m_polys[i] == m_endRef)
					continue;
				duDebugDrawNavMeshPoly(&dd, *m_navMesh, m_polys[i], pathCol);
			}
		}

		if (m_nsmoothPath)
		{
			dd.depthMask(false);
			const unsigned int spathCol = duRGBA(0, 0, 0, 220);
			dd.begin(DU_DRAW_LINES, 3.0f);
			for (int i = 0; i < m_nsmoothPath; ++i)
				dd.vertex(m_smoothPath[i * 3], m_smoothPath[i * 3 + 1] + 0.1f, m_smoothPath[i * 3 + 2], spathCol);
			dd.end();
			dd.depthMask(true);
		}
	}
	else if (pathType == PathType::STRAIGHT)
	{
		duDebugDrawNavMeshPoly(&dd, *m_navMesh, m_startRef, startCol);
		duDebugDrawNavMeshPoly(&dd, *m_navMesh, m_endRef, endCol);

		if (m_npolys)
		{
			for (int i = 0; i < m_npolys; ++i)
			{
				if (m_polys[i] == m_startRef || m_polys[i] == m_endRef)
					continue;
				duDebugDrawNavMeshPoly(&dd, *m_navMesh, m_polys[i], pathCol);
			}
		}

		if (m_nstraightPath)
		{
			dd.depthMask(false);
			const unsigned int spathCol = duRGBA(64, 16, 0, 220);
			const unsigned int offMeshCol = duRGBA(128, 96, 0, 220);
			dd.begin(DU_DRAW_LINES, 2.0f);
			for (int i = 0; i < m_nstraightPath - 1; ++i)
			{
				unsigned int col;
				if (m_straightPathFlags[i] & DT_STRAIGHTPATH_OFFMESH_CONNECTION)
					col = offMeshCol;
				else
					col = spathCol;

				dd.vertex(m_straightPath[i * 3], m_straightPath[i * 3 + 1] + 0.4f, m_straightPath[i * 3 + 2], col);
				dd.vertex(m_straightPath[(i + 1) * 3], m_straightPath[(i + 1) * 3 + 1] + 0.4f, m_straightPath[(i + 1) * 3 + 2], col);
			}
			dd.end();
			dd.begin(DU_DRAW_POINTS, 6.0f);
			for (int i = 0; i < m_nstraightPath; ++i)
			{
				unsigned int col;
				if (m_straightPathFlags[i] & DT_STRAIGHTPATH_START)
					col = startCol;
				else if (m_straightPathFlags[i] & DT_STRAIGHTPATH_END)
					col = endCol;
				else if (m_straightPathFlags[i] & DT_STRAIGHTPATH_OFFMESH_CONNECTION)
					col = offMeshCol;
				else
					col = spathCol;
				dd.vertex(m_straightPath[i * 3], m_straightPath[i * 3 + 1] + 0.4f, m_straightPath[i * 3 + 2], col);
			}
			dd.end();
			dd.depthMask(true);
		}
	}
}

NavAgent::NavAgent() :radius(2.0f), height(5.0f), stopHeight(0.2f), maxSlope(45.0f)
{}
