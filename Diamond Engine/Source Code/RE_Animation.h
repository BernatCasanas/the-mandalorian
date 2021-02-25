#pragma once


#include  <vector>
#include <string>
#include <map>

#include"MathGeoLib/include/MathGeoLib.h"
#include "DEResource.h"

typedef unsigned int GLuint;
typedef unsigned int uint;

struct Channel
{
	std::string boneName;

	//Time & Value
	std::map<double, float3> positionKeys;
	std::map<double, Quat> rotationKeys;
	std::map<double, float3> scaleKeys;

	//bool HasPosKey() const;
	std::map<double, float3>::const_iterator GetPrevPosKey(double currentKey) const;
	std::map<double, float3>::const_iterator GetNextPosKey(double currentKey) const;

	//bool HasRotKey() const;
	std::map<double, Quat>::const_iterator GetPrevRotKey(double currentKey) const;
	std::map<double, Quat>::const_iterator GetNextRotKey(double currentKey) const;

	//bool HasScaleKey() const;
	std::map<double, float3>::const_iterator GetPrevScaleKey(double currentKey) const;
	std::map<double, float3>::const_iterator GetNextScaleKey(double currentKey) const;
};

class ResourceAnimation : public Resource
{

public:
	ResourceAnimation(unsigned int _uid);
	~ResourceAnimation();

	bool LoadToMemory() override;
	bool UnloadFromMemory() override;

	uint SaveCustomFormat(ResourceAnimation* animation, char** buffer);
	void LoadCustomFormat(const char* path);

public:

	std::string animationName;
	float duration;
	float ticksPerSecond;
	bool loopable = true;
	uint numChannels;
	uint initTimeAnim = 0;

	std::map<std::string, Channel> channels;
};