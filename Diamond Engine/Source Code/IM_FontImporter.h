#pragma once

#include <map>
#include <vector>
#include <string>

struct FT_FaceRec_;
typedef FT_FaceRec_* FT_Face;
struct FT_LibraryRec_;
typedef FT_LibraryRec_* FT_Library;

struct Character
{
public:
	Character(unsigned int advanceX, unsigned int advanceY, float sizeX, float sizeY, float bearingX, float bearingY, float texCoordx);
	~Character();

public:
	unsigned int advance[2];

	float size[2];
	float bearing[2];

	float textureOffset = 0.0f;
};


struct FontDictionary
{
public:
	FontDictionary(unsigned int atlasTexture, const char* name, std::map<char, Character>& characterVec, int atlasWidth, int atlasHeight);
	~FontDictionary();

	void UnloadAtlas();

public:
	unsigned int atlasTexture = 0u;

	int atlasWidth = 0;
	int atlasHeight = 0;

	std::string name;
	std::map<char, Character> characters;
};


class FreeType_Library 
{
public:
	FreeType_Library();
	~FreeType_Library();

	void ImportNewFont(const char* path, int size);
	void InitFontDictionary(FT_Face& face, const char* fontName);
	FontDictionary* GetFont(const char* name);

private:
	void CalculateAtlasSize(FT_Face& face, int& width, int& height) const;

private:
	FT_Library library;
	
	//Font name	---- Leters of the font
	std::map<std::string, FontDictionary> fontLibrary;
};

