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

// Returns 1 if success... Negative numbers for error codes
int FBXHandler::LoadMeshFromFBXFile(const char* _filePath)
{
	int result = 0;

	// Initialize the SDK manager. This object handles all our memory management.
	FbxManager* lSdkManager = FbxManager::Create();

	// Create the IO settings object.
	FbxIOSettings *ios = FbxIOSettings::Create(lSdkManager, IOSROOT);
	lSdkManager->SetIOSettings(ios);

	// Create an importer using the SDK manager.
	FbxImporter* lImporter = FbxImporter::Create(lSdkManager, "");
	
	result = 1;

	// Use the first argument as the filename for the importer.
	if (!lImporter->Initialize(_filePath, -1, lSdkManager->GetIOSettings())) {
		result = -1;
		return result;
	}

	// Create a new scene so that it can be populated by the imported file.
	FbxScene* lScene = FbxScene::Create(lSdkManager, "myScene");
	result = -2;

	// Import the contents of the file into the scene.
	lImporter->Import(lScene);

	// The file is imported; so get rid of the importer.
	lImporter->Destroy();

	FbxNode* lRootNode = lScene->GetRootNode();
	if (lRootNode)
	{
		result = -3;
		for (int i = 0; i < lRootNode->GetChildCount(); i++)
		{
			FbxNode* tNode = lRootNode->GetChild(i);
			FbxGeometry* geometry = (FbxGeometry*)tNode->GetNodeAttribute();
			FbxNodeAttribute::EType geoNodeType = geometry->GetAttributeType();
			if (FbxNodeAttribute::eMesh == geoNodeType)
			{
				FbxMesh* theMesh = (FbxMesh*)geometry;
				//MeshComponentsAdvanced::OutInformationAdvanced meshInst;
				m_mesh = new Mesh();

				int mPolygonCount = theMesh->GetPolygonCount();
				int mPolygonVertexCount = theMesh->GetControlPointsCount();

				bool mAllByControlPoint = true;
				bool mHasNormal = theMesh->GetElementNormalCount() > 0;
				bool mHasUV = theMesh->GetElementUVCount() > 0;

				FbxGeometryElement::EMappingMode mNormalMappingMode = FbxGeometryElement::eNone;
				FbxGeometryElement::EMappingMode mUVMappingMode = FbxGeometryElement::eNone;

				FbxLayerElementArrayTemplate<int>* lMaterialIndice = NULL;
				FbxGeometryElement::EMappingMode lMaterialMappingMode = FbxGeometryElement::eNone;
				struct SubMesh
				{
					SubMesh() : IndexOffset(0), TriangleCount(0) {}

					int IndexOffset;
					int TriangleCount;
				};
				FbxArray<SubMesh*> mSubMeshes;
				if (theMesh->GetElementMaterial())
				{
					lMaterialIndice = &theMesh->GetElementMaterial()->GetIndexArray();
					lMaterialMappingMode = theMesh->GetElementMaterial()->GetMappingMode();
					if (lMaterialIndice && lMaterialMappingMode == FbxGeometryElement::eByPolygon)
					{
						//FBX_ASSERT(lMaterialIndice->GetCount() == lPolygonCount);
						if (lMaterialIndice->GetCount() == mPolygonCount)
						{
							// Count the faces of each material
							for (int lPolygonIndex = 0; lPolygonIndex < mPolygonCount; ++lPolygonIndex)
							{
								const int lMaterialIndex = lMaterialIndice->GetAt(lPolygonIndex);
								if (mSubMeshes.GetCount() < lMaterialIndex + 1)
								{
									mSubMeshes.Resize(lMaterialIndex + 1);
								}
								if (mSubMeshes[lMaterialIndex] == NULL)
								{
									mSubMeshes[lMaterialIndex] = new SubMesh;
								}
								mSubMeshes[lMaterialIndex]->TriangleCount += 1;
							}

							// Make sure we have no "holes" (NULL) in the mSubMeshes table. This can happen
							// if, in the loop above, we resized the mSubMeshes by more than one slot.
							for (int i = 0; i < mSubMeshes.GetCount(); i++)
							{
								if (mSubMeshes[i] == NULL)
									mSubMeshes[i] = new SubMesh;
							}

							// Record the offset (how many vertex)
							const int lMaterialCount = mSubMeshes.GetCount();
							int lOffset = 0;
							for (int lIndex = 0; lIndex < lMaterialCount; ++lIndex)
							{
								mSubMeshes[lIndex]->IndexOffset = lOffset;
								lOffset += mSubMeshes[lIndex]->TriangleCount * 3;
								// This will be used as counter in the following procedures, reset to zero
								mSubMeshes[lIndex]->TriangleCount = 0;
							}
							//FBX_ASSERT(lOffset == lPolygonCount * 3);
						}
					}
				}

				// All faces will use the same material.
				if (mSubMeshes.GetCount() == 0)
				{
					mSubMeshes.Resize(1);
					mSubMeshes[0] = new SubMesh();
				}

				const int TRIANGLE_VERTEX_COUNT = 3;

				//// Four floats for every position.
				//const int VERTEX_STRIDE = 4;
				//// Three floats for every normal.
				//const int NORMAL_STRIDE = 3;
				//// Two floats for every UV.
				//const int UV_STRIDE = 2;

				if (mHasNormal)
				{
					mNormalMappingMode = theMesh->GetElementNormal(0)->GetMappingMode();
					if (mNormalMappingMode == FbxGeometryElement::eNone)
						mHasNormal = false;
					if (mHasNormal && mNormalMappingMode != FbxGeometryElement::eByControlPoint)
						mAllByControlPoint = false;
				}
				if (mHasUV)
				{
					mUVMappingMode = theMesh->GetElementUV(0)->GetMappingMode();
					if (mUVMappingMode == FbxGeometryElement::eNone)
						mHasUV = false;
					if (mHasUV && mUVMappingMode != FbxGeometryElement::eByControlPoint)
						mAllByControlPoint = false;
				}
				if (!mAllByControlPoint)
				{
					mPolygonVertexCount = mPolygonCount * TRIANGLE_VERTEX_COUNT;
				}

				FbxStringList mUVNames;
				theMesh->GetUVSetNames(mUVNames);
				const char * mUVName = NULL;
				if (mHasUV && mUVNames.GetCount())
				{
					mUVName = mUVNames[0];
				}

				//int indexCount = theMesh->GetPolygonVertexCount();
				//meshInst.indices.resize(mPolygonCount * TRIANGLE_VERTEX_COUNT);

				m_mesh->m_vertexCount = mPolygonVertexCount;
				m_mesh->m_indexCount = mPolygonCount * TRIANGLE_VERTEX_COUNT;

				m_mesh->m_allVerticesPositions = new Vector3[mPolygonVertexCount];
				m_mesh->m_indices = new unsigned[mPolygonCount * TRIANGLE_VERTEX_COUNT];

				const FbxVector4 * mControlPoints = theMesh->GetControlPoints();
				FbxVector4 mCurrentVertex;
				FbxVector4 mCurrentNormal;
				FbxVector2 mCurrentUV;
				int mVertexCount = 0;
				for (int mPolygonIndex = 0; mPolygonIndex < mPolygonCount; ++mPolygonIndex)
				{
					int lMaterialIndex = 0;
					if (lMaterialIndice && lMaterialMappingMode == FbxGeometryElement::eByPolygon)
					{
						lMaterialIndex = lMaterialIndice->GetAt(mPolygonIndex);
					}

					// Where should I save the vertex attribute index, according to the material
					const int lIndexOffset = mSubMeshes[lMaterialIndex]->IndexOffset +
						mSubMeshes[lMaterialIndex]->TriangleCount * 3;
					for (int mVerticeIndex = 0; mVerticeIndex < TRIANGLE_VERTEX_COUNT; mVerticeIndex++)
					{
						const int mControlPointIndex = theMesh->GetPolygonVertex(mPolygonIndex, mVerticeIndex);

						if (mAllByControlPoint)
						{
							m_mesh->m_indices[lIndexOffset + mVerticeIndex] = static_cast<unsigned int>(mControlPointIndex);
							//meshInst.indices[lIndexOffset + mVerticeIndex] = static_cast<unsigned int>(mControlPointIndex);
						}
						// Populate the array with vertex attribute, if by polygon vertex.
						else
						{
							//MeshComponentsAdvanced::VertexAdvanced currVert;
							//meshInst.indices[lIndexOffset + mVerticeIndex] = static_cast<unsigned int>(mVertexCount);

							m_mesh->m_indices[lIndexOffset + mVerticeIndex] = static_cast<unsigned int>(mVertexCount);

							mCurrentVertex = mControlPoints[mControlPointIndex];

							m_mesh->m_allVerticesPositions[mVertexCount].x = static_cast<float>(mCurrentVertex[0]);
							m_mesh->m_allVerticesPositions[mVertexCount].y = static_cast<float>(mCurrentVertex[1]);
							m_mesh->m_allVerticesPositions[mVertexCount].z = static_cast<float>(mCurrentVertex[2]);
							//currVert.position[0] = static_cast<float>(mCurrentVertex[0]);
							//currVert.position[1] = static_cast<float>(mCurrentVertex[1]);
							//currVert.position[2] = static_cast<float>(mCurrentVertex[2]);
							//currVert.position[3] = 1;

							/*if (mHasNormal)
							{
								theMesh->GetPolygonVertexNormal(mPolygonIndex, mVerticeIndex, mCurrentNormal);

								currVert.normals[0] = static_cast<float>(mCurrentNormal[0]);
								currVert.normals[1] = static_cast<float>(mCurrentNormal[1]);
								currVert.normals[2] = static_cast<float>(mCurrentNormal[2]);
								currVert.normals[3] = 1.f;
							}

							if (mHasUV)
							{
								bool lUnmappedUV;
								theMesh->GetPolygonVertexUV(mPolygonIndex, mVerticeIndex, mUVName, mCurrentUV, lUnmappedUV);
								currVert.uvs[0] = static_cast<float>(mCurrentUV[0]);
								currVert.uvs[1] = static_cast<float>(mCurrentUV[1]);
							}*/
							//meshInst.vertices.push_back(currVert);
						}
						++mVertexCount;
					}
					mSubMeshes[lMaterialIndex]->TriangleCount += 1;
				}
				//_outVector.push_back(meshInst);
				for (int i = 0; i < mSubMeshes.Size(); i++)
				{
					delete mSubMeshes[i];
				}
				mSubMeshes.Clear();
			}
		}
		
		result = 1;
	}

	// Destroy the SDK manager and all the other objects it was handling.
	lSdkManager->Destroy();
	return result;
}

MeshComponentsAdvanced::Mesh::Mesh()
{
	m_allVerticesPositions = 0;
	m_indices = 0;
	m_vertexCount = 0;
	m_indexCount = 0;

	//m_vertexCount = 10;
	//m_indexCount = m_vertexCount * 3;

	//m_allVerticesPositions = new Vector3[m_vertexCount];

	//for (unsigned i = 0; i < m_vertexCount; i++)
	//{
	//	m_allVerticesPositions[i].x = i / 5.2f;
	//	m_allVerticesPositions[i].y = i / 5.2f;
	//	m_allVerticesPositions[i].z = i / 5.2f;
	//}
	//
	//m_indices = new unsigned[m_indexCount];

	//for (unsigned i = 0; i < m_indexCount; i++)
	//{
	//	m_indices[i] = i;
	//}
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