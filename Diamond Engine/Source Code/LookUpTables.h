#pragma once
#include <utility>
namespace LookUpTables
{
	class LookUpTable
	{
	public:
		LookUpTable(int resolution);
		~LookUpTable();

		float GetSinOf(float radAngle)const;

	private:
		int sineLUTResolution;
		float resolutionValue;
		std::pair<float, float>* sineLUT;
	};

	static const LookUpTable myLookUpTable = LookUpTable(360);
}