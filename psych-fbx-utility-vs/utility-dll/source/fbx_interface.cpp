#include "fbxsdk.h" // <- Must be first
#include "fbx_interface.h"
#include <string>

using namespace CMath;
using namespace ObjectInfo;

Vector3::Vector3() {
	x = 0.f;
	y = 0.f;
	z = 0.f;
}

Vector3::~Vector3() {
	x = 0.f;
	y = 0.f;
	z = 0.f;
}

Vector2::Vector2() {
	x = 0.f;
	y = 0.f;
}

Vector2::~Vector2() {
	x = 0.f;
	y = 0.f;
}

Vector4::Vector4() {
	x = 0.f;
	y = 0.f;
	z = 0.f;
	w = 0.0f;
}

Vector4::~Vector4() {
	x = 0.f;
	y = 0.f;
	z = 0.f;
	w = 0.0f;
}

#if USE_MATERIALS
Material::PropertyData::PropertyData() {
	m_propertyType = PropertyType::PROPERTYTYPE_EMISSIVE;
	m_textureRelativeFileName = 0;
	m_textureAbsoluteFilePath = 0;
}

Material::PropertyData::~PropertyData() {
	if (m_textureRelativeFileName)
	{
		delete[] m_textureRelativeFileName;
		m_textureRelativeFileName = 0;
	}
	if (m_textureAbsoluteFilePath)
	{
		delete[]m_textureAbsoluteFilePath;
		m_textureAbsoluteFilePath = 0;
	}
}

Material::Material() {
	m_materialType = MaterialType::MATERIALTYPE_LAMBERT;
	m_materialProperties = new PropertyData[PropertyType::PROPERTYTYPE_COUNT];
	m_textureCount = 0;
}

Material::~Material() {
	if (m_materialProperties) {
		delete[] m_materialProperties;
		m_materialProperties = 0;
	}

	m_textureCount = 0;
}
#endif

#if USE_MESHES
Mesh::Mesh() {
	m_allVerticesPositions = 0;
	m_allVerticesNormals = 0;
	m_allVerticesUVs = 0;
	m_indices = 0;
	m_vertexCount = 0;
	m_indexCount = 0;
}

Mesh::~Mesh() {
	if (m_allVerticesPositions)
	{
		delete[] m_allVerticesPositions;
		m_allVerticesPositions = 0;
	}

	if (m_allVerticesNormals)
	{
		delete[] m_allVerticesNormals;
		m_allVerticesNormals = 0;
	}

	if (m_allVerticesUVs)
	{
		delete[] m_allVerticesUVs;
		m_allVerticesUVs = 0;
	}

	if (m_indices)
	{
		delete[] m_indices;
		m_indices = 0;
	}

	m_vertexCount = 0;
	m_indexCount = 0;
}
#endif

Object::Object() {
	m_parentArrayIndexID = 0;
	m_childrenArrayIndexIDs = 0;
#if USE_MESHES
	m_mesh = 0;
#endif

#if USE_MATERIALS
	m_materials = { 0 };
#endif

	m_childrenCount = 0;
	m_materialCount = 0;
	m_name = 0;
	m_arrayIndexID = 0;
}

Object::~Object() {
#if USE_MESHES
	if (m_mesh)
	{
		delete m_mesh;
		m_mesh = 0;
	}
#endif

	m_parentArrayIndexID = 0;

	if (m_childrenArrayIndexIDs) {
		delete[] m_childrenArrayIndexIDs;
		m_childrenArrayIndexIDs = 0;
	}

#if USE_MATERIALS
	if (m_materials)
	{
		delete[] m_materials;
		m_materials = 0;
	}
#endif

	if (m_name)
	{
		delete[]m_name;
		m_name = 0;
	}

	m_childrenCount = 0;

	m_materialCount = 0;
	m_arrayIndexID = 0;
}

Scene::Scene() {
	m_objects = { 0 };
	m_numberOfObjects = 0;
}

Scene::~Scene() {
	if (m_objects)
	{
		delete[] m_objects;
		m_objects = 0;
	}

	m_numberOfObjects = 0;
}

FBXHandler::FBXHandler() {
	m_scene = 0;
}

FBXHandler::~FBXHandler() {
	if (m_scene)
	{
		delete m_scene;
		m_scene = 0;
	}
}

CRESULT FBXHandler::LoadFBXFile(const char * _filePath)
{
	// Initialize the SDK manager. This object handles all our memory management.
	FbxManager* lSdkManager = FbxManager::Create();

	// Create the IO settings object.
	FbxIOSettings *ios = FbxIOSettings::Create(lSdkManager, IOSROOT);
	lSdkManager->SetIOSettings(ios);

	// Create an importer using the SDK manager.
	FbxImporter* lImporter = FbxImporter::Create(lSdkManager, "");

	// Use the first argument as the filename for the importer.
	if (!lImporter->Initialize(_filePath, -1, lSdkManager->GetIOSettings())) {
		return CRESULT_INCORRECT_FILE_PATH;
	}

	// Create a new scene so that it can be populated by the imported file.
	FbxScene* lScene = FbxScene::Create(lSdkManager, "myScene");

	m_scene = new Scene();

	// Import the contents of the file into the scene.
	lImporter->Import(lScene);

	// The file is imported; so get rid of the importer.
	lImporter->Destroy();

	LoadFBXScene(lScene);

	// Destroy the SDK manager and all the other objects it was handling.
	lSdkManager->Destroy();

	return CRESULT_SUCCESS;
}

// Takes in the current index and returns the new index
CRESULT FBXHandler::LoadSceneHelperFunction(int& _objectIndex, Scene* _scene, FbxNode* _inOutFbxNode, unsigned& _currentRootNodeIndex, unsigned& _numberOfChildrenPassed, unsigned& _previousCallsParent) {

	CRESULT result = CRESULT_SUCCESS;

	const char* name = _inOutFbxNode->GetName();

	unsigned childCount = _inOutFbxNode->GetChildCount();

	unsigned previousChildrenPassed = _numberOfChildrenPassed;

	_scene->m_objects[_currentRootNodeIndex + _numberOfChildrenPassed].m_childrenArrayIndexIDs = new int[childCount];

	_scene->m_objects[_currentRootNodeIndex + _numberOfChildrenPassed].m_childrenCount = childCount;

	for (unsigned currIndex = 0; currIndex < childCount; currIndex++)
	{
		FbxNode* currentChild = _inOutFbxNode->GetChild(currIndex);

		const char* currentChildName = currentChild->GetName();

		result = RunTasks(_objectIndex, _scene, currentChild);

		if (result != CRESULT_SUCCESS)
			return result;

		if (currIndex != 0)
		{
			_scene->m_objects[_objectIndex].m_parentArrayIndexID = _previousCallsParent + previousChildrenPassed;

			_scene->m_objects[_currentRootNodeIndex + previousChildrenPassed].m_childrenArrayIndexIDs[currIndex] = _objectIndex;
		}
		else
		{
			_scene->m_objects[_objectIndex].m_parentArrayIndexID = _currentRootNodeIndex + _numberOfChildrenPassed;

			_scene->m_objects[_currentRootNodeIndex + _numberOfChildrenPassed].m_childrenArrayIndexIDs[currIndex] = _objectIndex;
		}

		++_numberOfChildrenPassed;
		++_objectIndex;

		int childCountAgain = currentChild->GetChildCount();

		if (childCountAgain > 0)
		{
			result = LoadSceneHelperFunction(_objectIndex, _scene, currentChild, _currentRootNodeIndex, _numberOfChildrenPassed, _previousCallsParent);

			if (result != CRESULT_SUCCESS)
				return result;
		}
	}

	return result;
}

CRESULT FBXHandler::FillOutMesh(int& _objectIndex, Scene* _scene, FbxNode* _fbxNode)
{
	FbxGeometry* geometry = (FbxGeometry*)_fbxNode->GetNodeAttribute();
	FbxNodeAttribute::EType geoNodeType = geometry->GetAttributeType();
	if (FbxNodeAttribute::eMesh == geoNodeType)
	{
		FbxMesh* theMesh = (FbxMesh*)geometry;

		//_scene->m_objects[_objectIndex] = new Object();

#if USE_MESHES
		_scene->m_objects[_objectIndex].m_mesh = new Mesh();
#endif

		_scene->m_objects[_objectIndex].m_arrayIndexID = _objectIndex;

		const char* name = _fbxNode->GetName();
		m_scene->m_objects[_objectIndex].m_name = new char[strlen(name) + 1];
		strncpy(m_scene->m_objects[_objectIndex].m_name, name, strlen(name) + 1);

#if USE_MESHES
		Mesh* currentMesh = _scene->m_objects[_objectIndex].m_mesh;

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

		currentMesh->m_vertexCount = mPolygonVertexCount;
		currentMesh->m_indexCount = mPolygonCount * TRIANGLE_VERTEX_COUNT;

		currentMesh->m_allVerticesPositions = new Vector3[mPolygonVertexCount];
		currentMesh->m_indices = new unsigned[mPolygonCount * TRIANGLE_VERTEX_COUNT];

		if (mHasNormal)
		{
			currentMesh->m_allVerticesNormals = new Vector3[mPolygonVertexCount];
		}

		if (mHasUV)
		{
			currentMesh->m_allVerticesUVs = new Vector2[mPolygonVertexCount];
		}

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
					currentMesh->m_indices[lIndexOffset + mVerticeIndex] = static_cast<unsigned int>(mControlPointIndex);
				}
				// Populate the array with vertex attribute, if by polygon vertex.
				else
				{
					currentMesh->m_indices[lIndexOffset + mVerticeIndex] = static_cast<unsigned int>(mVertexCount);

					mCurrentVertex = mControlPoints[mControlPointIndex];

					currentMesh->m_allVerticesPositions[mVertexCount].x = static_cast<float>(mCurrentVertex[0]);
					currentMesh->m_allVerticesPositions[mVertexCount].y = static_cast<float>(mCurrentVertex[1]);
					currentMesh->m_allVerticesPositions[mVertexCount].z = static_cast<float>(mCurrentVertex[2]);

					if (mHasNormal)
					{
						theMesh->GetPolygonVertexNormal(mPolygonIndex, mVerticeIndex, mCurrentNormal);

						currentMesh->m_allVerticesNormals[mVertexCount].x = static_cast<float>(mCurrentNormal[0]);
						currentMesh->m_allVerticesNormals[mVertexCount].y = static_cast<float>(mCurrentNormal[1]);
						currentMesh->m_allVerticesNormals[mVertexCount].z = static_cast<float>(mCurrentNormal[2]);
					}

					if (mHasUV)
					{
						bool lUnmappedUV;
						theMesh->GetPolygonVertexUV(mPolygonIndex, mVerticeIndex, mUVName, mCurrentUV, lUnmappedUV);

						currentMesh->m_allVerticesUVs[mVertexCount].x = static_cast<float>(mCurrentUV[0]);
						currentMesh->m_allVerticesUVs[mVertexCount].y = static_cast<float>(mCurrentUV[1]);
					}
				}
				++mVertexCount;
			}
			mSubMeshes[lMaterialIndex]->TriangleCount += 1;
		}
		for (int i = 0; i < mSubMeshes.Size(); i++)
		{
			delete mSubMeshes[i];
		}
		mSubMeshes.Clear();
#endif

		return CRESULT_SUCCESS;
	}

	else
	{
		// delete everything and say the scene can only contain geometry nodes

		return CRESULT_NODE_WAS_NOT_GEOMETRY_TYPE;
	}
}

CRESULT FBXHandler::FillOutMaterial(int & _objectIndex, Scene * _scene, FbxNode * _fbxNode)
{
	_scene->m_objects[_objectIndex].m_arrayIndexID = _objectIndex;

	unsigned materialCount = _fbxNode->GetMaterialCount();

	if (materialCount <= 0)
		return CRESULT_SUCCESS;

	m_scene->m_objects[_objectIndex].m_materialCount = materialCount;
#if USE_MATERIALS
	m_scene->m_objects[_objectIndex].m_materials = new Material[materialCount];
#endif

#if USE_MATERIALS
	for (unsigned currMaterialIndex = 0; currMaterialIndex < materialCount; currMaterialIndex++)
	{
		//m_scene->m_objects[_objectIndex].m_materials[currMaterialIndex] = new Material();
		Material* currentMaterial = &m_scene->m_objects[_objectIndex].m_materials[currMaterialIndex];

		FbxSurfaceMaterial* currentFBXMaterial = _fbxNode->GetMaterial(currMaterialIndex);
		if (!currentFBXMaterial) break;

		if (currentFBXMaterial->Is<FbxSurfacePhong>()) currentMaterial->m_materialType = Material::MATERIALTYPE_PHONG;
		else if (currentFBXMaterial->Is<FbxSurfaceLambert>()) currentMaterial->m_materialType = Material::MATERIALTYPE_LAMBERT;

		/************* FbxString comparison returns 0 if the strings are equal *************/
		FbxProperty lProperty = currentFBXMaterial->GetFirstProperty();
		while (lProperty.IsValid())
		{
			Material::PropertyData* currentProperty = nullptr;

			FbxString fbxName = lProperty.GetName();

			const char * cPtr = fbxName.Buffer();
			std::string strName = std::string(cPtr);

			if (currentFBXMaterial->Is<FbxSurfacePhong>())
			{
				if ("SpecularColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_SPECULAR];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_SPECULAR;
				}
				else if ("SpecularFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_SPECULAR];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_SPECULAR;
				}
				else if ("Shininess" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_SHININESS];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_SHININESS;
				}
				else if ("ReflectionColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_REFLECTION];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_REFLECTION;
				}
				else if ("ReflectionFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_REFLECTION];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_REFLECTION;
				}
				else if ("EmissiveColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_EMISSIVE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_EMISSIVE;
				}
				else if ("EmissiveFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_EMISSIVE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_EMISSIVE;
				}
				else if ("AmbientColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_AMBIENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_AMBIENT;
				}
				else if ("AmbientFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_AMBIENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_AMBIENT;
				}
				else if ("DiffuseColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DIFFUSE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DIFFUSE;
				}
				else if ("DiffuseFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DIFFUSE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DIFFUSE;
				}
				else if ("Bump" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_BUMP];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_BUMP;
				}
				else if ("BumpFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_BUMP];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_BUMP;
				}
				else if ("NormalMap" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_NORMAL];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_NORMAL;
				}
				else if ("TransparentColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_TRANSPARENCY];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_TRANSPARENCY;
				}
				else if ("TransparencyFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_TRANSPARENCY];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_TRANSPARENCY;
				}
				else if ("DisplacementColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DISPLACEMENT;
				}
				else if ("DisplacementFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DISPLACEMENT;
				}
				else if ("VectorDisplacementColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT;
				}
				else if ("VectorDisplacementFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT;
				}
			}

			else if (currentFBXMaterial->Is<FbxSurfaceLambert>())
			{
				if ("EmissiveColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_EMISSIVE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_EMISSIVE;
				}
				else if ("EmissiveFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_EMISSIVE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_EMISSIVE;
				}
				else if ("AmbientColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_AMBIENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_AMBIENT;
				}
				else if ("AmbientFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_AMBIENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_AMBIENT;
				}
				else if ("DiffuseColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DIFFUSE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DIFFUSE;
				}
				else if ("DiffuseFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DIFFUSE];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DIFFUSE;
				}
				else if ("Bump" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_BUMP];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_BUMP;
				}
				else if ("BumpFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_BUMP];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_BUMP;
				}
				else if ("NormalMap" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_NORMAL];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_NORMAL;
				}
				else if ("TransparentColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_TRANSPARENCY];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_TRANSPARENCY;
				}
				else if ("TransparencyFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_TRANSPARENCY];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_TRANSPARENCY;
				}
				else if ("DisplacementColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DISPLACEMENT;
				}
				else if ("DisplacementFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_DISPLACEMENT;
				}
				else if ("VectorDisplacementColor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT;
				}
				else if ("VectorDisplacementFactor" == strName) {
					currentProperty = &currentMaterial->m_materialProperties[Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT];
					currentProperty->m_propertyType = Material::PropertyType::PROPERTYTYPE_VECTOR_DISPLACEMENT;
				}
			}

			if (currentProperty)
			{
				if (lProperty.GetPropertyDataType().GetType() == eFbxDouble3) {
					FbxDouble3 val = lProperty.Get<FbxDouble3>();
					currentProperty->m_dataColorValues.x = (float)val[0];
					currentProperty->m_dataColorValues.y = (float)val[1];
					currentProperty->m_dataColorValues.z = (float)val[2];
				}
				else if (lProperty.GetPropertyDataType().GetType() == eFbxDouble) {
					currentProperty->m_dataColorValues.w = (float)lProperty.Get<FbxDouble>();
				}

				if (currentProperty->m_textureAbsoluteFilePath == nullptr && currentProperty->m_textureRelativeFileName == nullptr)
				{
					FbxFileTexture* lFileTexture = lProperty.GetSrcObject<FbxFileTexture>();
					if (lFileTexture != nullptr)
					{
						const char * relativeFileName = lFileTexture->GetRelativeFileName();
						const char * absoluteFileName = lFileTexture->GetFileName();

						currentProperty->m_textureAbsoluteFilePath = new char[strlen(absoluteFileName) + 1];
						currentProperty->m_textureRelativeFileName = new char[strlen(relativeFileName) + 1];

						strncpy(currentProperty->m_textureAbsoluteFilePath, absoluteFileName, strlen(absoluteFileName) + 1);
						strncpy(currentProperty->m_textureRelativeFileName, relativeFileName, strlen(relativeFileName) + 1);
						++currentMaterial->m_textureCount;
					}
				}
			}

			lProperty = currentFBXMaterial->GetNextProperty(lProperty);
		}
	}
#endif

	return CRESULT_SUCCESS;
}

CRESULT FBXHandler::RunTasks(int & _objectIndex, Scene * _scene, FbxNode * _fbxNode)
{
	CRESULT result;

	result = FillOutMesh(_objectIndex, _scene, _fbxNode);

	if (result != CRESULT_SUCCESS)
		return result;

	result = FillOutMaterial(_objectIndex, _scene, _fbxNode);

	if (result != CRESULT_SUCCESS)
		return result;

	return CRESULT_SUCCESS;
}

CRESULT FBXHandler::LoadFBXScene(FbxScene* _fbxScene)
{
	CRESULT result;

	if (m_scene == nullptr)
		m_scene = new Scene();

	FbxNode* lRootNode = _fbxScene->GetRootNode();
	if (lRootNode)
	{
		int objectIndex = 0;

		m_scene->m_numberOfObjects = _fbxScene->GetGeometryCount();

		if (m_scene->m_numberOfObjects <= 0)
			return CRESULT_NO_OBJECTS_IN_SCENE;

		m_scene->m_objects = new Object[m_scene->m_numberOfObjects];

		unsigned numberOfChildrenPassed = 0;

		unsigned geometryRootNodeIndex = 0;

		unsigned childCount = lRootNode->GetChildCount();

		for (unsigned i = 0; i < childCount; i++)
		{
			FbxNode* tNode = lRootNode->GetChild(i);

			if (!(FbxGeometry*)tNode->GetNodeAttribute())
			{
				++geometryRootNodeIndex;
				continue;
			}

			const char* tName = tNode->GetName();

			result = RunTasks(objectIndex, m_scene, tNode);
			if (result != CRESULT_SUCCESS)
				return result;

			int check = tNode->GetChildCount();

			m_scene->m_objects[objectIndex].m_childrenCount = check;

			++objectIndex;

			if (check > 0)
			{
				unsigned cannotUseIBecauseOfReference = geometryRootNodeIndex;

				result = LoadSceneHelperFunction(objectIndex, m_scene, tNode, geometryRootNodeIndex, numberOfChildrenPassed, cannotUseIBecauseOfReference);

				if (result != CRESULT_SUCCESS)
					return result;
			}

			++geometryRootNodeIndex;
		}

		return CRESULT_SUCCESS;
	}

	return CRESULT_ROOT_NODE_NOT_FOUND;
}