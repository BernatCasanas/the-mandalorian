#include "LookUpTables.h"
#include "Globals.h"

LookUpTables::LookUpTable::LookUpTable(int resolution) : sineLUTResolution(resolution), resolutionValue(0.0f),
sineLUT(nullptr)
{
	if (sineLUTResolution <= 0)sineLUTResolution = 1;

	resolutionValue = (float)(2 * PI / sineLUTResolution);
	float currentRadians = 0.0f;
	sineLUT = new std::pair<float, float>[sineLUTResolution];

	for (int i = 0; i < sineLUTResolution; ++i)
	{
		float sine = (float)sin(currentRadians);
		sineLUT[i] = std::pair<float, float>(currentRadians, sine);
		currentRadians += resolutionValue;
		LOG(LogType::L_NORMAL, "Sine of %.2f radians are %.2f", currentRadians, sine);
	}
}

LookUpTables::LookUpTable::~LookUpTable()
{
	if (sineLUT != nullptr)
	{
		delete[] sineLUT;
	}
}

float LookUpTables::LookUpTable::GetSinOf(float radAngle) const
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