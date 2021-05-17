#pragma once

#ifndef __MAYKMATH_H__
#define __MAYKMATH_H__

#include<stdlib.h>
#include<time.h>
#include<vector>
#include<string>
#include <map>


namespace MaykMath 
{
	/*class LookUpTables
	{
	public:
		LookUpTables();
		~LookUpTables();

	private:
		int sineLUTResolution;
		std::map<float, float> sineLUT;
	};*/

	void Init();
	int Random(int minV, int maxV);
	float Random(float minV, float maxV);
	std::string VersionToString(int major, int minor, int patch);

	void FindCentroid(float* A, float* B, float* C, float* r);

	void GeneralDataSet(float* dest, float* src, size_t vecSize);

	//static const LookUpTables lookUpTables;

	float Lerp(float from, float to, float t);
	float InvLerp(float from, float to, float value);
	float Remap(float iMin, float iMax, float oMin, float oMax, float value);

	template <class T>
	void FixedVectorPushBack(std::vector<T>& vec, T& value) {
		if (vec.size() == vec.capacity())
		{
			//Vector is full
			//delete vector's oldest element
			//Move all elements 1 position back
			for (unsigned int i = 0; i < vec.size(); i++)
			{
				if (i + 1 < vec.size())
				{
					float iCopy = vec[i + 1];
					vec[i] = iCopy;
				}
			}
			vec[vec.capacity() - 1] = value;
			//Pushback new element
		}
		else
		{
			//Vector is not full
			vec.push_back(value);
		}
	}
}


#endif //__MAYKMATH_H__