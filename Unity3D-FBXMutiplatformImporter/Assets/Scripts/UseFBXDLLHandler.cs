using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class UseFBXDLLHandler : MonoBehaviour
{
    /***************** Import DLL Functions *****************/
    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern IntPtr CPPDLLCreateFBXHandler();

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern void CPPDLLDestroyFBXHandler(IntPtr pClassNameObject);

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern void CPPDLLFillOutMesh(IntPtr pClassNameObject);

    [DllImport("FBXImporterDLL_WINDOWS", CallingConvention = CallingConvention.Cdecl)]
    static public extern int CPPDLLLoadMeshFromFBXFile(IntPtr pClassNameObject, string filePath);

    [DllImport("FBXImporterDLL_WINDOWS", CallingConvention = CallingConvention.Cdecl)]
    static public extern int CPPDLLLoadMaterialFromFBXFile(IntPtr pClassNameObject, string filePath);
    /***************** Import DLL Functions *****************/


    /***************** Member Variables *****************/
    CSImportedFBXScene  m_csImportedFBXScene;
    IntPtr              m_cppImportedFBXScene;
    CSMesh              m_csMesh;
    MeshFilter          m_meshFilter;
    Mesh                m_unityMesh;
    MeshRenderer        m_meshRenderer;
    Transform           m_objectTransform;
    /***************** Member Variables *****************/


    /***************** Class Definitions *****************/
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

    public struct CPPMaterial
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PropertyData
        {
            public PropertyType m_propertyType;
            public IntPtr m_textureRelativeFileName;
            public IntPtr m_textureAbsoluteFilePath;
            public CSMesh.Vector4 m_dataColorValues;
        };

        public MaterialType m_materialType;
        public IntPtr m_materialProperties;
    }

    public struct CSMaterial
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PropertyData
        {
            public PropertyType m_propertyType;
            [MarshalAs(UnmanagedType.LPStr)]
            public string m_textureRelativeFileName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string m_textureAbsoluteFilePath;
            public CSMesh.Vector4 m_dataColorValues;
        };

        public MaterialType m_materialType;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct)]
        public PropertyData[] m_materialProperties;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CSMesh
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

        public Vector3[] m_allVerticesPositions;
        public Vector3[] m_allVerticesNormals;
        public Vector2[] m_allVerticesUVs;

        public uint[] m_indices;

        public uint m_vertexCount;
        public uint m_indexCount;

        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)]
        public CSMaterial[] m_materials;

        public Texture2D[] m_textures;

        public uint m_materialCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CSImportedFBXScene
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct CPPMesh
        {
            public IntPtr m_allVerticesPositions;
            public IntPtr m_normals;
            public IntPtr m_uvs;
            public IntPtr m_indices;
            public uint m_vertexCount;
            public uint m_indexCount;
            public IntPtr m_materials;
            public uint m_materialCount;
        }

        public CSImportedFBXScene()
        {
            m_CPPMeshPtr = IntPtr.Zero;
        }

        public IntPtr m_CPPMeshPtr;

        /*******CS member variables (must be created last because of Marshaling*******/
        public CPPMesh m_mesh;
    }
    /***************** Class Definitions *****************/


    /***************** Class Functions *****************/

    void Start()
    {
        m_meshFilter = gameObject.AddComponent<MeshFilter>();
        m_unityMesh = m_meshFilter.mesh;
        m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
        m_objectTransform = gameObject.GetComponent<Transform>();

        m_csMesh = new CSMesh();

        m_csImportedFBXScene = new CSImportedFBXScene();

        // C++ handler which is unmanaged memory (we need to delete)
        m_cppImportedFBXScene = CPPDLLCreateFBXHandler();

        int result = CSLoadFBXFile("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbx");

        if (1 == result)
        {
            // If the file loaded a mesh successfully, let's fill out the mesh component of this game object.
            FillOutGameObjectMesh();

            if (m_csMesh.m_allVerticesUVs != null)
            {
                int result2 = CSLoadMaterialFromFBXFile("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbx");
                //m_csMesh.m_textures = new Texture2D[4];
                //m_csMesh.m_textures[0] = LoadPNG("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbm\\SciFi_Character_Base_Color.png");
                //m_csMesh.m_textures[1] = LoadPNG("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbm\\SciFi_Character_Emissive.png");
                //m_csMesh.m_textures[2] = LoadPNG("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbm\\SciFi_Character_Metallic.png");
                //m_csMesh.m_textures[3] = LoadPNG("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbm\\SciFi_Character_Normal.png");

                //m_meshRenderer.material.EnableKeyword("_MainTex");
                //m_meshRenderer.material.SetTexture("_MainTex", m_csMesh.m_textures[0]);

                //m_meshRenderer.material.EnableKeyword("_EMISSION");
                //m_meshRenderer.material.SetTexture("_EMISSION", m_csMesh.m_textures[1]);

                //m_meshRenderer.material.EnableKeyword("_METALLICGLOSSMAP");
                //m_meshRenderer.material.SetTexture("_METALLICGLOSSMAP", m_csMesh.m_textures[2]);

                //m_meshRenderer.material.EnableKeyword("_NORMALMAP");
                //m_meshRenderer.material.SetTexture("_NORMALMAP", m_csMesh.m_textures[3]);
            }
        }

        // If the file did not load, lets check if there is a specific error message we can display to the user.
        switch (result)
        {
            case 0:
                print("File path did not load");
                break;
            case -1:
                print("File did not contain mesh data");
                break;
            default:
                break;
        }

        m_objectTransform.Translate(new Vector3(0.0f, -10.0f));         // For testing
        m_objectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);   // For testing
    }

    private int CSLoadFBXFile(string _fbxFilePath)
    {

        int result = CPPDLLLoadMeshFromFBXFile(m_cppImportedFBXScene, _fbxFilePath);
        int result2 = CPPDLLLoadMaterialFromFBXFile(m_cppImportedFBXScene, _fbxFilePath);

        if (result == 0)
        {
            print("Incorrect file path");
            return 0;
        }

        else if (result == 1)
        {
            m_csImportedFBXScene = (CSImportedFBXScene)Marshal.PtrToStructure(m_cppImportedFBXScene, typeof(CSImportedFBXScene));

            m_csImportedFBXScene.m_mesh = (CSImportedFBXScene.CPPMesh)Marshal.PtrToStructure(m_csImportedFBXScene.m_CPPMeshPtr, typeof(CSImportedFBXScene.CPPMesh));

            uint vertexCount = m_csImportedFBXScene.m_mesh.m_vertexCount;
            uint indexCount = m_csImportedFBXScene.m_mesh.m_indexCount;
            uint materialCount = m_csImportedFBXScene.m_mesh.m_materialCount;

            m_csMesh.m_vertexCount = vertexCount;
            m_csMesh.m_indexCount = indexCount;
            m_csMesh.m_materialCount = materialCount;

            if (vertexCount > 0)
            {
                m_csMesh.m_allVerticesPositions = new CSMesh.Vector3[vertexCount];
                m_csMesh.m_indices = new uint[indexCount];

                unsafe
                {
                    CSMesh.Vector3* vertices = (CSMesh.Vector3*)m_csImportedFBXScene.m_mesh.m_allVerticesPositions.ToPointer();
                    if (vertices != null)
                    {
                        for (int i = 0; i < vertexCount; i++)
                        {
                            m_csMesh.m_allVerticesPositions[i].x = vertices[i].x;
                            m_csMesh.m_allVerticesPositions[i].y = vertices[i].y;
                            m_csMesh.m_allVerticesPositions[i].z = vertices[i].z;
                        }
                    }

                    else
                    {
                        print("The mesh VERTEX POSITION data did not get read properly.");
                    }

                    uint* indices = (uint*)m_csImportedFBXScene.m_mesh.m_indices.ToPointer();
                    if (indices != null)
                    {
                        for (int i = 0; i < indexCount; i++)
                        {
                            m_csMesh.m_indices[i] = indices[i];
                        }
                    }

                    else
                    {
                        print("The mesh INDICE data did not get read properly.");
                    }

                    CSMesh.Vector3* normals = (CSMesh.Vector3*)m_csImportedFBXScene.m_mesh.m_normals.ToPointer();
                    if (normals != null)
                    {
                        m_csMesh.m_allVerticesNormals = new CSMesh.Vector3[vertexCount];

                        for (int i = 0; i < vertexCount; i++)
                        {
                            m_csMesh.m_allVerticesNormals[i].x = normals[i].x;
                            m_csMesh.m_allVerticesNormals[i].y = normals[i].y;
                            m_csMesh.m_allVerticesNormals[i].z = normals[i].z;
                        }
                    }
                    else
                    {
                        print("This mesh did not have NORMALS");
                    }

                    CSMesh.Vector2* uvs = (CSMesh.Vector2*)m_csImportedFBXScene.m_mesh.m_uvs.ToPointer();
                    if (uvs != null)
                    {
                        m_csMesh.m_allVerticesUVs = new CSMesh.Vector2[vertexCount];

                        for (int i = 0; i < vertexCount; i++)
                        {
                            m_csMesh.m_allVerticesUVs[i].x = uvs[i].x;
                            m_csMesh.m_allVerticesUVs[i].y = uvs[i].y;
                        }

                        if (materialCount > 0)
                        {
                            m_csMesh.m_materials = new CSMaterial[materialCount];
                            CPPMaterial** materials = (CPPMaterial**)m_csImportedFBXScene.m_mesh.m_materials.ToPointer();

                            for (int i = 0; i < materialCount; i++)
                            {
                                m_csMesh.m_materials[i].m_materialType = materials[i]->m_materialType;
                                m_csMesh.m_materials[i].m_materialProperties = new CSMaterial.PropertyData[(int)PropertyType.PROPERTYTYPE_COUNT];
                                for (int j = 0; j < (int)PropertyType.PROPERTYTYPE_COUNT; j++)
                                {
                                    CPPMaterial.PropertyData** propertyData = (CPPMaterial.PropertyData**)materials[i]->m_materialProperties.ToPointer();
                                    m_csMesh.m_materials[i].m_materialProperties[j].m_propertyType = propertyData[j]->m_propertyType;
                                    m_csMesh.m_materials[i].m_materialProperties[j].m_textureRelativeFileName = Marshal.PtrToStringAnsi(propertyData[j]->m_textureRelativeFileName);
                                    m_csMesh.m_materials[i].m_materialProperties[j].m_textureAbsoluteFilePath = Marshal.PtrToStringAnsi(propertyData[j]->m_textureAbsoluteFilePath);
                                    m_csMesh.m_materials[i].m_materialProperties[j].m_dataColorValues = propertyData[j]->m_dataColorValues;
                                }
                            }
                        }

                        if (materialCount == 1)
                            print("This mesh had 1 material.");
                        else
                            print("This mesh had " + materialCount + " materials.");
                    }
                    else
                    {
                        print("This mesh did not have UVS");
                    }
                }

                return 1;
            }

            // Return false if the file loaded but some of the mesh data was incorrect
            return -1;
        }

        else
        {
            return result;
        }
    }

    private int CSLoadMaterialFromFBXFile(string _filePath)
    {
        int result = CPPDLLLoadMaterialFromFBXFile(m_cppImportedFBXScene, _filePath);

        return result;
    }

    private void FillOutGameObjectMesh()
    {
        Vector3[] t_unityVerts = new Vector3[m_csMesh.m_vertexCount];
        int[] t_unityIndices = new int[m_csMesh.m_indexCount];
        Vector3[] t_unityNormals = null;
        Vector2[] t_unityUVs = null;

        for (int i = 0; i < m_csMesh.m_vertexCount; i++)
        {
            t_unityVerts[i].x = m_csMesh.m_allVerticesPositions[i].x;
            t_unityVerts[i].y = m_csMesh.m_allVerticesPositions[i].y;
            t_unityVerts[i].z = m_csMesh.m_allVerticesPositions[i].z;
        }

        m_unityMesh.vertices = t_unityVerts;

        for (int i = 0; i < m_csMesh.m_indexCount; i++)
        {
            t_unityIndices[i] = (int)m_csMesh.m_indices[i];
        }

        m_unityMesh.triangles = t_unityIndices;

        if (m_csMesh.m_allVerticesNormals != null)
        {
            t_unityNormals = new Vector3[m_csMesh.m_vertexCount];

            for (int i = 0; i < m_csMesh.m_vertexCount; i++)
            {
                t_unityNormals[i].x = m_csMesh.m_allVerticesNormals[i].x;
                t_unityNormals[i].y = m_csMesh.m_allVerticesNormals[i].y;
                t_unityNormals[i].z = m_csMesh.m_allVerticesNormals[i].z;
            }

            m_unityMesh.normals = t_unityNormals;
        }

        if (m_csMesh.m_allVerticesUVs != null)
        {
            t_unityUVs = new Vector2[m_csMesh.m_vertexCount];

            for (int i = 0; i < m_csMesh.m_vertexCount; i++)
            {
                t_unityUVs[i].x = m_csMesh.m_allVerticesUVs[i].x;
                t_unityUVs[i].y = m_csMesh.m_allVerticesUVs[i].y;
            }

            m_unityMesh.uv = t_unityUVs;
        }

        m_meshFilter.mesh = m_unityMesh;
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

        return tex;
    }

    private void OnDestroy()
    {
        // Delete C++ handler which is unmanaged memory.
        CPPDLLDestroyFBXHandler(m_cppImportedFBXScene);

        m_cppImportedFBXScene = IntPtr.Zero;
    }

    /***************** Class Functions *****************/
}
