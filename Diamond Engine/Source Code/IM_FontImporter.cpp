#include "IM_FontImporter.h"
#include "IM_FileSystem.h"

#include "Globals.h"

#include "OpenGL.h"

#include "FreeType/include/freetype/freetype.h"
#include "FreeType/include/ft2build.h"
#include FT_FREETYPE_H

#pragma comment (lib, "FreeType/win32/freetype.lib")

Character::Character(unsigned int advanceX, unsigned int advanceY, float sizeX, float sizeY, float bearingX, float bearingY, float texOffset) :
	textureOffset(texOffset)
{
	advance[0] = advanceX;
	advance[1] = advanceY;

	size[0] = sizeX;
	size[1] = sizeY;

	bearing[0] = bearingX;
	bearing[1] = bearingY;
}


Character::~Character()
{
}


FontDictionary::FontDictionary(unsigned int atlasTexture, const char* name, std::map<char, Character>& characterVec, int atlasWidth, int atlasHeight) :
	atlasTexture(atlasTexture),
	atlasWidth(atlasWidth),
	atlasHeight(atlasHeight),
	name(name),
	characters(characterVec)
{
}


FontDictionary::~FontDictionary()
{
	characters.clear();
}


void FontDictionary::UnloadAtlas()
{
	glDeleteTextures(1, &atlasTexture);

	atlasTexture = 0u;
}


FreeType_Library::FreeType_Library()
{
	FT_Error error = FT_Init_FreeType(&library);
	if (error)
		LOG(LogType::L_ERROR, "Error initializing FreeType");
}

FreeType_Library::~FreeType_Library()
{
	for (std::map<std::string, FontDictionary>::iterator it = fontLibrary.begin(); it != fontLibrary.end(); ++it)
		it->second.UnloadAtlas();

	fontLibrary.clear();
	FT_Done_FreeType(library);
}


void FreeType_Library::ImportNewFont(const char* path, int size)
{
	std::string fileName;
	std::string fileExtension;
	FileSystem::SplitFilePath(path, nullptr, &fileName, &fileExtension);
	std::string libPath = FONTS_PATH + fileName + "." + fileExtension;

	FT_Face new_face;
	FT_Error error = FT_New_Face(library, libPath.c_str(), 0, &new_face);

	if (error)
	{
		LOG(LogType::L_ERROR, "Error while loading font");
	}
	else
	{
		LOG(LogType::L_NORMAL, "The font was loaded correctly");
		FT_Set_Pixel_Sizes(new_face, 0, size);

		glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

		InitFontDictionary(new_face, libPath.c_str());
	}
}


void FreeType_Library::InitFontDictionary(FT_Face& face, const char* fontName)
{
	std::map<char, Character> charVector;
	int width = 0;
	int height = 0;

	CalculateAtlasSize(face, width, height);

	unsigned int atlasTexture = 0u;

	glActiveTexture(GL_TEXTURE0);
	glGenTextures(1, &atlasTexture);
	glBindTexture(GL_TEXTURE_2D, atlasTexture);

	glTexImage2D(GL_TEXTURE_2D, 0, GL_RED, width, height, 0, GL_RED, GL_UNSIGNED_BYTE, 0);

	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

	/* Clamping to edges is important to prevent artifacts when scaling */
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);

	/* Linear filtering usually looks best for text */
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);


	int xAtlasPos = 0;

	for (unsigned char c = 0; c < 128; ++c)
	{
		if (FT_Load_Char(face, c, FT_LOAD_RENDER))
		{
			LOG(LogType::L_ERROR, "Could not load gyph %c", c);
			continue;
		}

		glTexSubImage2D(GL_TEXTURE_2D, 0, xAtlasPos, 0, face->glyph->bitmap.width, face->glyph->bitmap.rows, GL_RED, GL_UNSIGNED_BYTE, face->glyph->bitmap.buffer);

		float texOffset = (float)xAtlasPos / (float)width;

		charVector.insert(std::pair<char, Character>(c, Character(face->glyph->advance.x, face->size->metrics.height, face->glyph->bitmap.width, face->glyph->bitmap.rows, face->glyph->bitmap_left, face->glyph->bitmap_top, texOffset)));

		xAtlasPos += face->glyph->bitmap.width + 1;
	}

	fontLibrary.emplace(std::pair<std::string, FontDictionary>(fontName, FontDictionary(atlasTexture, fontName, charVector, width, height)));
	glBindTexture(GL_TEXTURE_2D, 0);
	FT_Done_Face(face);
}


FontDictionary* FreeType_Library::GetFont(const char* name)
{
	std::string fileName;
	std::string fileExtension;
	FileSystem::SplitFilePath(name, nullptr, &fileName, &fileExtension);
	std::string libPath = FONTS_PATH + fileName + "." + fileExtension;

	std::map<std::string, FontDictionary>::iterator iterator = fontLibrary.find(libPath.c_str());

	if (iterator != fontLibrary.end())
		return &iterator->second;

	else
	{
		ImportNewFont(libPath.c_str(), 72);
		iterator = fontLibrary.find(libPath.c_str());

		return (iterator != fontLibrary.end() ? &iterator->second : nullptr);
	}
}


void FreeType_Library::CalculateAtlasSize(FT_Face& face, int& width, int& height) const
{
	width = 0;
	height = 0;

	for (unsigned char c = 0; c < 128; ++c)
	{
		if (FT_Load_Char(face, c, FT_LOAD_RENDER))
		{
			LOG(LogType::L_ERROR, "Could not load gyph %c", c);
			continue;
		}

		width += face->glyph->bitmap.width + 1;
		height = height > face->glyph->bitmap.rows ? height : face->glyph->bitmap.rows;
	}
}