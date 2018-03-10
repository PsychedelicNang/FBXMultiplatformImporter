#include "../Header/FBXHandler_Windows.h"

FBXHandler::FBXHandler()
{
	m_mesh = 0;
}

FBXHandler::~FBXHandler()
{
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
}

MeshComponentsAdvanced::Mesh::Mesh()
{
	m_vertexCount = 10;
	m_indexCount = m_vertexCount * 3;

	m_allVerticesPositions = new Vector3[m_vertexCount];

	for (unsigned i = 0; i < m_vertexCount; i++)
	{
		m_allVerticesPositions[i].x = i / 5.2f;
		m_allVerticesPositions[i].y = i / 5.2f;
		m_allVerticesPositions[i].z = i / 5.2f;
	}
	
	m_indices = new unsigned[m_indexCount];

	for (unsigned i = 0; i < m_indexCount; i++)
	{
		m_indices[i] = i;
	}
}

MeshComponentsAdvanced::Mesh::~Mesh()
{
	if (m_allVerticesPositions)
	{
		delete[] m_allVerticesPositions;
		m_allVerticesPositions = 0;
	}

	if (m_indices)
	{
		delete[] m_indices;
		m_indices = 0;
	}

	m_vertexCount = 0;
	m_indexCount = 0;
}

MeshComponentsAdvanced::Vector3::Vector3()
{
	x = 0.f;
	y = 0.f;
	z = 0.f;
}

MeshComponentsAdvanced::Vector3::~Vector3()
{
	x = 0.f;
	y = 0.f;
	z = 0.f;
}