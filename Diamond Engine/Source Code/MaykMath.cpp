#include"MaykMath.h"
#include "Globals.h"
#include<assert.h>

void MaykMath::Init()
{
	srand(time(NULL));
}

//ASK: Is this good?
int MaykMath::Random(int minV, int maxV)
{
	assert(minV <= maxV);
	return (rand() % (maxV - minV + 1)) + minV;
}

float MaykMath::Random(float minV, float maxV)
{
	assert(minV <= maxV);
	float random = ((float)rand()) / (float)RAND_MAX;
	return minV + (random * (maxV - minV));
}

std::string MaykMath::VersionToString(int major, int minor, int patch)
{
	std::string ret(std::to_string(major) + "." + std::to_string(minor) + "." + std::to_string(patch));
	return ret;
}

void MaykMath::FindCentroid(float* A, float* B, float* C, float* r)
{
	for (int i = 0; i < 3; ++i)
	{
		r[i] = (A[i] + B[i] + C[i]) / 3;
	}
}

void MaykMath::GeneralDataSet(float* dest, float* src, size_t vecSize)
{
	for (size_t i = 0; i < vecSize; i++)
	{
		dest[i] = src[i];
	}
}

//template <class T>
//void MaykMath::FixedVectorPushBack(std::vector<T>& vec, T& value)
//{
//	//if (vec.size() == vec.capacity())
//	//{
//	//	//Vector is full
//	//	//delete vector's oldest element
//	//	//Move all elements 1 position back
//	//	for (unsigned int i = 0; i < vec.size(); i++)
//	//	{
//	//		if (i + 1 < vec.size())
//	//		{
//	//			float iCopy = vec[i + 1];
//	//			vec[i] = iCopy;
//	//		}
//	//	}
//	//	vec[vec.capacity() - 1] = value;
//	//	//Pushback new element
//	//}
//	//else
//	//{
//	//	//Vector is not full
//	//	vec.push_back(value);
//	//}
//}

//MaykMath::LookUpTables::LookUpTables() : sineLUTResolution(8.0f)
//{
//	float radians = (float)(2 *  PI/ sineLUTResolution);
//	float currentRadians = 0.0f;
//
//	for (int i = 0; i < sineLUTResolution; ++i)
//	{
//		float sine = (float)sin(currentRadians);
//		sineLUT.emplace(currentRadians, sine);
//		currentRadians += radians;
//		LOG(LogType::L_NORMAL, "Sine of %.2f radians are %.2f", currentRadians, sine);
//	}
//}
//
//MaykMath::LookUpTables::~LookUpTables()
//{
//	sineLUT.clear();
//}
