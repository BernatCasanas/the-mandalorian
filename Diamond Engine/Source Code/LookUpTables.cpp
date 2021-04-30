#include "LookUpTables.h"
#include "Globals.h"

LookUpTables::LookUpTable::LookUpTable(int resolution)
{}

LookUpTables::LookUpTable::~LookUpTable()
{}

LookUpTables::GeometricLUT::GeometricLUT(int resolution) : LookUpTable(resolution), LUTResolution(resolution), resolutionValue(0.0f),
sineLUT(nullptr)
{
	if (LUTResolution <= 0)
		LUTResolution = 1;

	resolutionValue = (float)(2 * PI / LUTResolution);

	//Sine 
	float currentRadians = 0.0f;
	sineLUT = new std::pair<float, float>[LUTResolution];

	for (int i = 0; i < LUTResolution; ++i)
	{
		float sine = (float)sin(currentRadians);
		sineLUT[i] = std::pair<float, float>(currentRadians, sine);
		currentRadians += resolutionValue;
		LOG(LogType::L_NORMAL, "Sine of %.2f radians are %.2f", currentRadians, sine);
	}

	//Cosine
	currentRadians = 0.0f;
	cosLUT = new std::pair<float, float>[LUTResolution];
	for (int i = 0; i < LUTResolution; ++i)
	{
		float cosine = (float)cos(currentRadians);
		cosLUT[i] = std::pair<float, float>(currentRadians, cosine);
		currentRadians += resolutionValue;
		LOG(LogType::L_NORMAL, "Cosine of %.2f radians are %.2f", currentRadians, cosine);
	}
}

LookUpTables::GeometricLUT::~GeometricLUT()
{
	if (sineLUT != nullptr)
	{
		delete[] sineLUT;
		sineLUT = nullptr;
	}
	

	if (cosLUT != nullptr)
	{
		delete[] cosLUT;
		cosLUT = nullptr;
	}
	
}

float LookUpTables::GeometricLUT::GetSinOf(float radAngle) const
{
	//0 Get a valid angle between 0 and 2PI
	float newAngle = (radAngle < 2 * PI && radAngle >= 0.0f) ? radAngle : fmodf(radAngle, 2 * PI);
	//1 transform angle into an array index
	float newIndexF = newAngle / resolutionValue;
	int newIndex = (int)newIndexF;

	//if the angle is in the lutTable return it
	if ((newIndexF - newIndex) == 0.0f)
	{
		return sineLUT[newIndex].second;
	}

	/*bool isEdgeCase = false;
	if (newIndex <= 0)
	{
		newIndex = 0;
		isEdgeCase = true;
	}
	else if (newIndex >= sineLUTResolution-1)
	{
		newIndex = sineLUTResolution - 1;
		isEdgeCase = true;
	}*/

	//2 get the 2 nearest points
	//if (!isEdgeCase)
	//{
	//3 remap those 2 values and get an aproximation of sin
	float t = (newIndexF - newIndex);
	//Lerp
	return (1.0f - t) * sineLUT[newIndex].second + sineLUT[newIndex + 1].second * t;
	//}
	//else
	//{
		//return sineLUT[newIndex].second;
	//}
}

float LookUpTables::GeometricLUT::GetCosOf(float radAngle) const
{
	//0 Get a valid angle between 0 and 2PI
	float newAngle = (radAngle < 2 * PI && radAngle >= 0.0f) ? radAngle : fmodf(radAngle, 2 * PI);
	//1 transform angle into an array index
	float newIndexF = newAngle / resolutionValue;
	int newIndex = (int)newIndexF;

	//if the angle is in the lutTable return it
	if ((newIndexF - newIndex) == 0.0f)
	{
		return cosLUT[newIndex].second;
	}

	/*bool isEdgeCase = false;
	if (newIndex <= 0)
	{
		newIndex = 0;
		isEdgeCase = true;
	}
	else if (newIndex >= sineLUTResolution-1)
	{
		newIndex = sineLUTResolution - 1;
		isEdgeCase = true;
	}*/

	//2 get the 2 nearest points
	//if (!isEdgeCase)
	//{
	//3 remap those 2 values and get an aproximation of sin
	float t = (newIndexF - newIndex);
	//Lerp
	return (1.0f - t) * cosLUT[newIndex].second + cosLUT[newIndex + 1].second * t;
	//}
	//else
	//{
		//return sineLUT[newIndex].second;
	//}
}

LookUpTables::PowerLUT::PowerLUT()
{
}

LookUpTables::PowerLUT::~PowerLUT()
{
}
