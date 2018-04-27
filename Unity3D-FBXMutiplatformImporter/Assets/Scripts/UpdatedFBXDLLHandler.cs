using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

namespace CMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public float x;
        public float y;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
}

namespace CMaterialInfo
{
    public enum PropertyType
    {
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

    public enum MaterialType
    {
        MATERIALTYPE_PHONG = 0,
        MATERIALTYPE_LAMBERT
    };
}

enum CRESULT
{
    CRESULT_SUCCESS = 0,
    CRESULT_INCORRECT_FILE_PATH,
    CRESULT_NO_OBJECTS_IN_SCENE,
    CRESULT_NODE_WAS_NOT_GEOMETRY_TYPE,
    CRESULT_ROOT_NODE_NOT_FOUND
};

public class UpdatedFBXDLLHandler : MonoBehaviour {

    /***************** Import DLL Functions *****************/
	//[DllImport("FBXImporterDLL_WINDOWS")]
	[DllImport("FBXImporterBundle_OSX")]
    static public extern IntPtr CPPDLLCreateFBXHandler();

	//[DllImport("FBXImporterDLL_WINDOWS")]
	[DllImport("FBXImporterBundle_OSX")]
    static public extern void CPPDLLDestroyFBXHandler(IntPtr theFBXHandlerObject);

	//[DllImport("FBXImporterDLL_WINDOWS", CallingConvention = CallingConvention.Cdecl)]
	[DllImport("FBXImporterBundle_OSX", CallingConvention = CallingConvention.Cdecl)]
    static public extern int CPPDLLLoadFBXFile(IntPtr theFBXHandlerObject, string fbxFilePath);
    /***************** Import DLL Functions *****************/

    /***************** Class Definitions *****************/
    public struct CPPMaterial
    {
        public struct CPPPropertyData
        {
            public CMaterialInfo.PropertyType m_propertyType;
            public IntPtr m_textureRelativeFileName;
            public IntPtr m_textureAbsoluteFilePath;
            public CMath.Vector4 m_dataColorValues;
        };

        public CMaterialInfo.MaterialType m_materialType;
        public IntPtr m_materialProperties;
        public uint m_textureCount;
    }

    public struct CSMaterial
    {
        public struct CSPropertyData
        {
            public CMaterialInfo.PropertyType m_propertyType;

            public string m_textureRelativeFileName;

            public string m_textureAbsoluteFilePath;

            public CMath.Vector4 m_dataColorValues;
        };

        public CMaterialInfo.MaterialType m_materialType;

        public CSPropertyData[] m_materialProperties;

        public Texture2D[] m_textures;
        public uint m_textureCount;
    }

    public struct CPPMesh
    {
        public IntPtr m_allVerticesPositions;
        public IntPtr m_normals;
        public IntPtr m_uvs;

        public IntPtr m_indices;

        public uint m_vertexCount;
        public uint m_indexCount;
    }

    public struct CSMesh
    {
        public CSMesh(uint vertexCount, uint indexCount)
        {
            m_allVerticesPositions = new CMath.Vector3[vertexCount];
            m_allVerticesNormals = new CMath.Vector3[vertexCount];
            m_allVerticesUVs = new CMath.Vector2[vertexCount];

            m_indices = new uint[indexCount];

            m_vertexCount = vertexCount;
            m_indexCount = indexCount;
        }

        public CMath.Vector3[] m_allVerticesPositions;
        public CMath.Vector3[] m_allVerticesNormals;
        public CMath.Vector2[] m_allVerticesUVs;

        public uint[] m_indices;

        public uint m_vertexCount;
        public uint m_indexCount;
    }

    public struct CPPObject
    {
        public int m_parentArrayIndexID;
        public IntPtr m_childrenArrayIndexIDs;
        //public IntPtr m_parent;
        //public IntPtr m_children;
        public IntPtr m_mesh;
        public IntPtr m_materials;

        public uint m_numberOfChildren;
        public uint m_numberOfMaterials;
        
        public IntPtr m_name;
        public uint m_arrayIndexID;
    }

    // Cannot be a struct...
    public class CSObject
    {
        public CSObject(uint numberOfChildren, uint numberOfMaterials, string name)
        {
            m_numberOfChildren = numberOfChildren;
            m_numberOfMaterials = numberOfMaterials;
            m_name = name;
            m_materials = new CSMaterial[m_numberOfMaterials];
        }
        // public uint m_parentIndex;
        //public CSObject m_parent;

        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct)]
        //public CSObject[] m_children;

        public int m_parentArrayIndexID;
        public int[] m_childrenArrayIndexIDs;

        public CSMesh m_mesh;

        public CSMaterial[] m_materials;

        public uint m_numberOfChildren;
        public uint m_numberOfMaterials;

        public string m_name;
        public uint m_arrayIndexID;
    }

    public struct CPPImportedFBXScene
    {
        public IntPtr m_objects;
        public uint m_numberOfObjects;
    }

    public struct CSImportedFBXScene
    {
        public CSObject[] m_objects;
        public uint m_numberOfObjects;
    }

    public struct CPPFBXHandler
    {
        public IntPtr m_fbxScene;
    }

    public struct CSFBXHandler
    {
        public CSImportedFBXScene m_fbxScene;
    }
    /***************** Class Definitions *****************/

    /***************** Member Variables *****************/
    CSFBXHandler m_csFBXHandler;
    IntPtr              m_cppFBXHandler;

    MeshFilter          m_unityMeshFilter;
    Mesh                m_unityMesh;
    MeshRenderer        m_unityMeshRenderer;
    Transform           m_unityObjectTransform;
   
    public List<GameObject>    m_unityGameObjects;
    /***************** Member Variables *****************/

    /***************** Class Functions *****************/
    // Use this for initialization
    void Start () {
        TimeSpan m_durationFileLoad;
        TimeSpan m_durationFileParsed;

        m_unityMeshFilter = gameObject.AddComponent<MeshFilter>();
        m_unityMesh = m_unityMeshFilter.mesh;
        m_unityMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        m_unityObjectTransform = gameObject.GetComponent<Transform>();

        m_csFBXHandler = new CSFBXHandler();

        // C++ handler which is unmanaged memory (we need to delete by calling CPPDLLDestroyFBXHandler())
        m_cppFBXHandler = CPPDLLCreateFBXHandler();

        DateTime m_timeBeforeFileLoad = DateTime.Now;
        //CRESULT result = (CRESULT)CPPDLLLoadFBXFile(m_cppFBXHandler, "C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\TestSciFiWithHierarchyNoAnimTriangulated.fbx");
        //CRESULT result = (CRESULT)CPPDLLLoadFBXFile(m_cppFBXHandler, "C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\TestSciFiWithHierarchy.fbx");
        //CRESULT result = (CRESULT)CPPDLLLoadFBXFile(m_cppFBXHandler, "C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\CyberPunksMap\\StressTestFBXLoader.fbx");
		CRESULT result = (CRESULT)CPPDLLLoadFBXFile(m_cppFBXHandler, "/Users/doghead1/Documents/GitHub/FBXMultiplatformImporter/StressTestFBXLoader.fbx");
		DateTime m_timeAfterFileLoad = DateTime.Now;

        m_durationFileLoad = m_timeAfterFileLoad - m_timeBeforeFileLoad;

        switch (result)
        {
            case CRESULT.CRESULT_SUCCESS:
                {
                    DateTime m_timeBeforeFileParsed = DateTime.Now;
                    CSParseFBXHandler();
                    DateTime m_timeAfterFileParsed = DateTime.Now;

                    m_durationFileParsed = m_timeAfterFileParsed - m_timeBeforeFileParsed;

                    if (m_durationFileLoad.Seconds > 0)
                        print("The FBX File loaded in: " + m_durationFileLoad.Seconds + "." + m_durationFileLoad.Milliseconds + " s");
                    else
                        print("The FBX File loaded in: " + m_durationFileLoad.Milliseconds + " ms");

                    if (m_durationFileParsed.Seconds > 0)
                        print("The FBX File was parsed in: " + m_durationFileParsed.Seconds + "." + m_durationFileParsed.Milliseconds + " s");
                    else
                        print("The FBX File was parsed in: " + m_durationFileParsed.Milliseconds + " ms");
                }
                break;
            case CRESULT.CRESULT_INCORRECT_FILE_PATH:
                print("Incorrect File Path.");
                break;
            case CRESULT.CRESULT_NO_OBJECTS_IN_SCENE:
                print("There were no objects in the given FBX scene.");
                break;
            case CRESULT.CRESULT_NODE_WAS_NOT_GEOMETRY_TYPE:
                print("Please, make sure the FBX scene only contains geometry nodes.");
                break;
            case CRESULT.CRESULT_ROOT_NODE_NOT_FOUND:
                print("The root node of the FBX file was not found.");
                break;
            default:
                break;
        }

        m_unityObjectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        m_unityObjectTransform.Translate(new Vector3(0.0f, 3.0f, 0.0f));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CSParseFBXHandler()
    {
        // If only I could just do vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv :'(
        //m_csFBXHandler = (CSFBXHandler)Marshal.PtrToStructure(m_cppFBXHandler, typeof(CSFBXHandler));

        unsafe
        {
            CPPImportedFBXScene* tempFbxSCene = (CPPImportedFBXScene*)(((CPPFBXHandler*)m_cppFBXHandler.ToPointer())->m_fbxScene);

            m_csFBXHandler.m_fbxScene.m_numberOfObjects = tempFbxSCene->m_numberOfObjects;
            m_csFBXHandler.m_fbxScene.m_objects = new CSObject[tempFbxSCene->m_numberOfObjects];

            CPPObject** sceneObjects = (CPPObject**)tempFbxSCene->m_objects;

            m_unityGameObjects = new List<GameObject>((int)m_csFBXHandler.m_fbxScene.m_numberOfObjects);

            for (uint currObjectIndex = 0; currObjectIndex < m_csFBXHandler.m_fbxScene.m_numberOfObjects; currObjectIndex++)
            {
                m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex] = new CSObject(sceneObjects[currObjectIndex]->m_numberOfChildren, sceneObjects[currObjectIndex]->m_numberOfMaterials, Marshal.PtrToStringAnsi(sceneObjects[currObjectIndex]->m_name));
                m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_arrayIndexID = sceneObjects[currObjectIndex]->m_arrayIndexID;
                m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_parentArrayIndexID = sceneObjects[currObjectIndex]->m_parentArrayIndexID;

                m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_childrenArrayIndexIDs = new int[m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_numberOfChildren];

                int* children = (int*)sceneObjects[currObjectIndex]->m_childrenArrayIndexIDs.ToPointer();

                for (int currChildIndex = 0; currChildIndex < m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_numberOfChildren; currChildIndex++)
                {
                    m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_childrenArrayIndexIDs[currChildIndex] = children[currChildIndex];
                }

                CPPMesh* currentMesh = ((CPPMesh*)sceneObjects[currObjectIndex]->m_mesh.ToPointer());

                GameObject unityGameObject = new GameObject();
                m_unityGameObjects.Add(unityGameObject);

                MeshFilter currMeshFilter = unityGameObject.AddComponent<MeshFilter>();
                MeshRenderer currMeshRenderer = unityGameObject.AddComponent<MeshRenderer>();

                unityGameObject.name = m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_name;

                print("Object: " + m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_name + "\tMaterial Count: " + sceneObjects[currObjectIndex]->m_numberOfMaterials + "\tNumber of Children: " + sceneObjects[currObjectIndex]->m_numberOfChildren);

                if (currentMesh->m_vertexCount > 0)
                {
                    m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh = new CSMesh(currentMesh->m_vertexCount, currentMesh->m_indexCount);

                    CMath.Vector3* vertices = (CMath.Vector3*)currentMesh->m_allVerticesPositions.ToPointer();
                    CMath.Vector3* normals = (CMath.Vector3*)currentMesh->m_normals.ToPointer();
                    CMath.Vector2* uvs = (CMath.Vector2*)currentMesh->m_uvs.ToPointer();

                    Vector3[] unityVerts = new Vector3[m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_vertexCount];
                    Vector3[] unityNormals = new Vector3[m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_vertexCount];
                    Vector2[] unityUVs = new Vector2[m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_vertexCount];

                    int[] unityIndices = new int[m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_indexCount];

                    for (uint currVertex = 0; currVertex < currentMesh->m_vertexCount; currVertex++)
                    {
                        unityVerts[currVertex].x = vertices[currVertex].x;
                        unityVerts[currVertex].y = vertices[currVertex].y;
                        unityVerts[currVertex].z = vertices[currVertex].z;

                        unityNormals[currVertex].x = normals[currVertex].x;
                        unityNormals[currVertex].y = normals[currVertex].y;
                        unityNormals[currVertex].z = normals[currVertex].z;

                        unityUVs[currVertex].x = uvs[currVertex].x;
                        unityUVs[currVertex].y = uvs[currVertex].y;

                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesPositions[currVertex].x = vertices[currVertex].x;
                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesPositions[currVertex].y = vertices[currVertex].y;
                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesPositions[currVertex].z = vertices[currVertex].z;

                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesNormals[currVertex].x = normals[currVertex].x;
                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesNormals[currVertex].y = normals[currVertex].y;
                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesNormals[currVertex].z = normals[currVertex].z;

                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesUVs[currVertex].x = uvs[currVertex].x;
                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_allVerticesUVs[currVertex].y = uvs[currVertex].y;
                    }

                    uint* indices = (uint*)currentMesh->m_indices.ToPointer();

                    for (uint currIndex = 0; currIndex < currentMesh->m_indexCount; currIndex++)
                    {
                        unityIndices[currIndex] = (int)indices[currIndex];

                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_mesh.m_indices[currIndex] = indices[currIndex];
                    }

                    currMeshFilter.mesh.vertices = unityVerts;
                    currMeshFilter.mesh.normals = unityNormals;
                    currMeshFilter.mesh.uv = unityUVs;
                    currMeshFilter.mesh.triangles = unityIndices;
                }

                if (sceneObjects[currObjectIndex]->m_numberOfMaterials > 0)
                {
                    currMeshRenderer.materials = new Material[sceneObjects[currObjectIndex]->m_numberOfMaterials];

                    for (int currMaterialIndex = 0; currMaterialIndex < sceneObjects[currObjectIndex]->m_numberOfMaterials; currMaterialIndex++)
                    {
                        CPPMaterial** materials = (CPPMaterial**)sceneObjects[currObjectIndex]->m_materials.ToPointer();

                        currMeshRenderer.materials[currMaterialIndex].shader = Shader.Find("Standard");

                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialType = materials[currMaterialIndex]->m_materialType;
                        m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties = new CSMaterial.CSPropertyData[(int)CMaterialInfo.PropertyType.PROPERTYTYPE_COUNT];
                        if (materials[currMaterialIndex]->m_textureCount > 0)
                            m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_textures = new Texture2D[materials[currMaterialIndex]->m_textureCount];

                        uint currentTextureIndex = 0;

                        for (int propertyIndex = 0; propertyIndex < (int)CMaterialInfo.PropertyType.PROPERTYTYPE_COUNT; propertyIndex++)
                        {
                            CPPMaterial.CPPPropertyData** propertyData = (CPPMaterial.CPPPropertyData**)materials[currMaterialIndex]->m_materialProperties.ToPointer();
                            m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties[propertyIndex].m_propertyType = propertyData[propertyIndex]->m_propertyType;
                            m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties[propertyIndex].m_textureRelativeFileName = Marshal.PtrToStringAnsi(propertyData[propertyIndex]->m_textureRelativeFileName);
                            m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties[propertyIndex].m_textureAbsoluteFilePath = Marshal.PtrToStringAnsi(propertyData[propertyIndex]->m_textureAbsoluteFilePath);
                            m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties[propertyIndex].m_dataColorValues = propertyData[propertyIndex]->m_dataColorValues;

                            if (m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties[propertyIndex].m_textureRelativeFileName != null ||
                                m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties[propertyIndex].m_textureAbsoluteFilePath != null)
                            {
                                m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_textures[currentTextureIndex] = LoadPNG(m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_materialProperties[propertyIndex].m_textureAbsoluteFilePath);

                                Color color;
                                color.r = propertyData[propertyIndex]->m_dataColorValues.x;
                                color.g = propertyData[propertyIndex]->m_dataColorValues.y;
                                color.b = propertyData[propertyIndex]->m_dataColorValues.z;
                                color.a = propertyData[propertyIndex]->m_dataColorValues.w;

                                switch (propertyData[propertyIndex]->m_propertyType)
                                {
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_EMISSIVE:
                                        {
                                            
                                            currMeshRenderer.materials[currMaterialIndex].EnableKeyword("_EMISSION");
                                            currMeshRenderer.materials[currMaterialIndex].SetTexture("_EmissionMap", m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_textures[currentTextureIndex]);
                                            currMeshRenderer.materials[currMaterialIndex].SetColor("_EmissionColor", color);
                                        }
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_AMBIENT:
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_DIFFUSE:
                                        {
                                            currMeshRenderer.materials[currMaterialIndex].EnableKeyword("_MainTex");
                                            currMeshRenderer.materials[currMaterialIndex].SetTexture("_MainTex", m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_textures[currentTextureIndex]);
                                            currMeshRenderer.materials[currMaterialIndex].SetColor("_MainTex", color);
                                        }
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_NORMAL:
                                        {
                                            currMeshRenderer.materials[currMaterialIndex].EnableKeyword("_BumpMap");
                                            currMeshRenderer.materials[currMaterialIndex].SetTexture("_BumpMap", m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_textures[currentTextureIndex]);
                                            currMeshRenderer.materials[currMaterialIndex].SetFloat("_BumpScale", color.a);
                                        }
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_BUMP:
                                        {
                                            currMeshRenderer.materials[currMaterialIndex].EnableKeyword("_BumpMap");
                                            currMeshRenderer.materials[currMaterialIndex].SetTexture("_BumpMap", m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_textures[currentTextureIndex]);
                                            currMeshRenderer.materials[currMaterialIndex].SetFloat("_BumpScale", color.a);
                                        }
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_TRANSPARENCY:
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_DISPLACEMENT:
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_VECTOR_DISPLACEMENT:
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_SPECULAR:
                                        {
                                            currMeshRenderer.materials[currMaterialIndex].EnableKeyword("_METALLICGLOSSMAP");
                                            currMeshRenderer.materials[currMaterialIndex].SetTexture("_MetallicGlossMap", m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_materials[currMaterialIndex].m_textures[currentTextureIndex]);
                                            currMeshRenderer.materials[currMaterialIndex].SetFloat("Metallic", color.a);
                                        }
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_SHININESS:
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_REFLECTION:
                                        {
                                            currMeshRenderer.materials[currMaterialIndex].EnableKeyword("_Glossiness");
                                            currMeshRenderer.materials[currMaterialIndex].SetFloat("Smoothness", color.a);
                                        }
                                        break;
                                    case CMaterialInfo.PropertyType.PROPERTYTYPE_COUNT:
                                        break;
                                    default:
                                        break;
                                }

                                ++currentTextureIndex;
                            }
                        }
                    }
                }
            }

            for (uint currObjectIndex = 0; currObjectIndex < m_csFBXHandler.m_fbxScene.m_numberOfObjects; currObjectIndex++)
            {
                m_unityGameObjects[(int)currObjectIndex].transform.parent = m_unityGameObjects[(int)m_csFBXHandler.m_fbxScene.m_objects[currObjectIndex].m_parentArrayIndexID].transform;
            }

            m_unityGameObjects[0].transform.parent = m_unityObjectTransform;
        }
    }
    
    private static Texture2D LoadPNG(string _filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(_filePath))
        {
            fileData = File.ReadAllBytes(_filePath);
            tex = new Texture2D(1, 1);
            tex.LoadImage(fileData); // LoadImage() auto resizes the texture dimensions.
        }

        else
        {
            return null;
        }

        return tex;
    }

    private void OnDestroy()
    {
        // Delete C++ handler which is unmanaged memory.
        CPPDLLDestroyFBXHandler(m_cppFBXHandler);

        m_cppFBXHandler = IntPtr.Zero;
    }

    /***************** Class Functions *****************/
}
