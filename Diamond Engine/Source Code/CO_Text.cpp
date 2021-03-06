#include "CO_Text.h"
#include "CO_Transform2D.h"

#include "Application.h"

#include "RE_Material.h"
#include "RE_Shader.h"

#include "MO_FileSystem.h"

#include "IM_FontImporter.h"
#include <map>

#include "MathGeoLib/include/MathGeoLib.h"
#include "ImGui/imgui.h"
#include "OpenGL.h"

C_Text::C_Text(GameObject* gameObject) : Component(gameObject), 
	font_path(""), 
	text(""),
	font(nullptr)
{
	memset(textColor, 0, sizeof(textColor));
	name = "Text";
	font = EngineExternal->moduleFileSystem->free_type_library->GetFont("Arial");
}


C_Text::~C_Text()
{
	font = nullptr;
}


void C_Text::RenderText(C_Transform2D* transform, ResourceMaterial* material, unsigned int VAO, unsigned int VBO)
{
	material->shader->Bind();
	material->PushUniforms();
	glActiveTexture(GL_TEXTURE0);
	glBindVertexArray(VAO);

	float x = 0;
	float y = 0;

	for (int i = 0; i < text.size(); ++i)
	{
		std::map<char, Character>::iterator it = font->characters.find(text[i]);

		if (it != font->characters.end())
		{
			const Character& character = it->second;

			glBindTexture(GL_TEXTURE_2D, character.textureId);
			GLint modelLoc = glGetUniformLocation(material->shader->shaderProgramID, "model_matrix");
			glUniformMatrix4fv(modelLoc, 1, GL_FALSE, transform->GetGlobal2DTransform().ptr());

			modelLoc = glGetUniformLocation(material->shader->shaderProgramID, "altColor");
			glUniform3f(modelLoc, textColor[0], textColor[1], textColor[2]);

			float posX = (x + character.bearing[0]) / 100.f;
			float posY = (y - (character.size[1] - character.bearing[1])) / 100.f;

			float width = character.size[0] / 100.f;
			float height = character.size[1] / 100.f;

			float vertices[6][4] = {
				{ posX,			posY + height,  0.0f, 0.0f },
				{ posX,			posY,			0.0f, 1.0f },
				{ posX + width, posY,			1.0f, 1.0f },

				{ posX,			posY + height,  0.0f, 0.0f },
				{ posX + width, posY,			1.0f, 1.0f },
				{ posX + width, posY + height,  1.0f, 0.0f }
			};

			glBindBuffer(GL_ARRAY_BUFFER, VBO);
			glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(vertices), vertices);

			glEnableVertexAttribArray(0);
			glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(float), (GLvoid*)0);

			// render quad
			glDrawArrays(GL_TRIANGLES, 0, 6);

			x += (character.advance >> 6);	//Bitshift by 6 to get size in pixels
		}	
	}
	
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);
	glBindTexture(GL_TEXTURE_2D, 0);

	if (material->shader)
		material->shader->Unbind();
}


#ifndef STANDALONE
bool C_Text::OnEditor()
{
	//EngineExternal->moduleFileSystem->free_type_library
	if (Component::OnEditor() == true)
	{
		char inputText[MAX_CHARACTER_TEXT];
		strcpy(inputText, text.c_str());
		ImGui::InputText("Text to print", &inputText[0], sizeof(inputText));

		text = inputText;
		
		ImGui::DragFloat3("##ltextCol", &textColor[0], 0.1f);
	}
	return true;
}
#endif // !STANDALONE


void C_Text::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	DEJson::WriteString(nObj, "text", text.c_str());

	if (font != nullptr)
		DEJson::WriteString(nObj, "fontName", font->name.c_str());
	
	DEJson::WriteVector3(nObj, "textColor", textColor);
}


void C_Text::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	text = nObj.ReadString("text");

	font = EngineExternal->moduleFileSystem->free_type_library->GetFont(nObj.ReadString("fontName"));

	float3 col = nObj.ReadVector3("textColor");
	textColor[0] = col.x;
	textColor[1] = col.y;
	textColor[2] = col.z;
}