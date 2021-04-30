#pragma once
#include <utility>
namespace LookUpTables
{
	class LookUpTable
	{
	public:
		LookUpTable(int resolution);
		~LookUpTable();

	protected:
	};

	class GeometricLUT : LookUpTable
	{
		public:
			GeometricLUT(int resolution);
			~GeometricLUT();

			float GetSinOf(float radAngle)const;
			float GetCosOf(float radAngle)const;

		private:
			float resolutionValue;
			int LUTResolution;
			std::pair<float, float>* sineLUT;
			std::pair<float, float>* cosLUT;
	};

	static const GeometricLUT myLookUpTable = GeometricLUT(360);

	class PowerLUT
	{
	public: 
		PowerLUT();
		~PowerLUT();
	};
}