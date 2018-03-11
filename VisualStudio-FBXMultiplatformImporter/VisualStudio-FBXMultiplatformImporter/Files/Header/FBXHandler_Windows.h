#pragma once

#ifdef FBXIMPORTER_WINDOWSDLL_EXPORTS
#define FBXIMPORTER_WINDOWSDLL_API __declspec(dllexport)
#else
#define FBXIMPORTER_WINDOWSDLL_API __declspec(dllimport)
#endif

#include "fbxsdk.h"

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

	struct Mesh {
		Mesh();
		~Mesh();
		Vector3*	m_allVerticesPositions;
		Vector3*	m_allVerticesNormals;
		Vector2*	m_allVerticesUVs;
		unsigned*	m_indices;
		unsigned	m_vertexCount;
		unsigned	m_indexCount;
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
	};
}