#pragma once

#ifdef FBXIMPORTER_WINDOWSDLL_EXPORTS
#define FBXIMPORTER_WINDOWSDLL_API __declspec(dllexport)
#else
#define FBXIMPORTER_WINDOWSDLL_API __declspec(dllimport)
#endif

#include "fbxsdk.h"
#include <vector>
#include <string>

namespace MeshComponentsAdvanced {
	struct Vector2 {
		Vector2();
		~Vector2();
		float x;
		float y;
	};

	struct Vector3 {
		Vector3();
		~Vector3();
		float x;
		float y;
		float z;
	};

	struct Vector4 {
		Vector4();
		~Vector4();
		float x;
		float y;
		float z;
		float w;
	};

	struct Material {
		Material();
		~Material();

		enum PropertyType {
			PROPERTYTYPE_EMISSIVE = 0,
			PROPERTYTYPE_AMBIENT,
			PROPERTYTYPE_DIFFUSE,
			PROPERTYTYPE_NORMAL,
			PROPERTYTYPE_BUMP,
			PROPERTYTYPE_TRANSPARENCY,
			PROPERTYTYPE_DISPLACEMENT,
			PROPERTYTYPE_VECTOR_DISPLACEMENT,
			PROPERTYTYPE_SPECULAR,
			PROPERTYTYPE_SHININESS,
			PROPERTYTYPE_REFLECTION,
			PROPERTYTYPE_COUNT
		};

		enum MaterialType {
			MATERIALTYPE_PHONG = 0,
			MATERIALTYPE_LAMBERT
		};

		struct PropertyData {
			PropertyData();
			~PropertyData();
			PropertyType	m_propertyType;
			char*			m_textureRelativeFileName;
			char*			m_textureAbsoluteFilePath;
			Vector4			m_dataColorValues;
		};

		MaterialType	m_materialType;
		//std::vector<PropertyData*>	m_materialProperties;
		PropertyData**	m_materialProperties;
	};

	struct Mesh {
		Mesh();
		~Mesh();
		Vector3*	m_allVerticesPositions;
		Vector3*	m_allVerticesNormals;
		Vector2*	m_allVerticesUVs;
		unsigned*	m_indices;
		unsigned	m_vertexCount;
		unsigned	m_indexCount;
		Material**	m_materials;
		//std::vector<Material*> m_materials;
		unsigned	m_materialCount;
	};
}

extern "C" {
	using namespace MeshComponentsAdvanced;
	class FBXIMPORTER_WINDOWSDLL_API FBXHandler {
		MeshComponentsAdvanced::Mesh*	m_mesh;

	public:
		FBXHandler();
		~FBXHandler();
		void FillOutMesh();
		int LoadMeshFromFBXFile(const char* _filePath);
		int LoadMaterialFromFBXFile(const char* _filePath);
	};
}