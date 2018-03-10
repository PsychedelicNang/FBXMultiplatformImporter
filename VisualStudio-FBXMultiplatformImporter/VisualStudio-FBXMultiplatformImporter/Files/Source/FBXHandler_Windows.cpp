#include "../Header/FBXHandler_Windows.h"

FBXHandler::FBXHandler()
{
	/*m_float = 0;
	m_numberOfIndices = 0;*/
	//m_indices = 0;

	//m_mesh = new Mesh();
	m_mesh = 0;
}

FBXHandler::~FBXHandler()
{
	//m_float = 0;
	//m_numberOfIndices = 0;

	/*if (m_indices)
	{
		delete[] m_indices;
		m_indices = 0;
	}*/

	if (m_mesh)
	{
		delete m_mesh;
		m_mesh = 0;
	}
}

void FBXHandler::FillOutMesh()
{
	if (!m_mesh)
	{
		m_mesh = new Mesh();
	}

	else
	{
		m_mesh->m_vertexCount = 10;
	}
}

MeshComponentsAdvanced::Mesh::Mesh()
{
	//m_vertices = 0;
	//m_indices = 0;
	m_vertexCount = 1000;
	m_indexCount = 43;
}

MeshComponentsAdvanced::Mesh::~Mesh()
{
	//if (m_vertices)
	//{
	//	delete[] m_vertices;
	//	m_vertices = 0;
	//}
	//
	//if (m_vertices)
	//{
	//	delete[] m_vertices;
	//	m_vertices = 0;
	//}

	m_vertexCount = 0;
	m_indexCount = 0;
}
