#pragma once

#ifdef FBXIMPORTER_WINDOWSDLL_EXPORTS
#define FBXIMPORTER_WINDOWSDLL_API __declspec(dllexport)
#else
#define FBXIMPORTER_WINDOWSDLL_API __declspec(dllimport)
#endif

#include <fbxsdk.h>

namespace MeshComponentsAdvanced {
	struct Vector4 {
		float x, y, z, w;
	};

	struct VertexPositionNormalUV {
		float position[4];
		float normals[4];
		float uvs[2];
	};

	struct Mesh {
		Mesh();
		~Mesh();
		//VertexPositionNormalUV*	m_vertices;
		//unsigned*				m_indices;
		unsigned				m_vertexCount;
		unsigned				m_indexCount;
	};
}

extern "C" {
	using namespace MeshComponentsAdvanced;
	class FBXIMPORTER_WINDOWSDLL_API FBXHandler {
		float	m_float;
		int		m_numberOfIndices;
		//int*	m_indices;
		MeshComponentsAdvanced::Mesh*	m_mesh;

	public:
		FBXHandler();
		~FBXHandler();
		void FillOutArray(int _length, int _array[]);
		bool SetFloat(float _float);
		void FillOutMesh();
	};
}