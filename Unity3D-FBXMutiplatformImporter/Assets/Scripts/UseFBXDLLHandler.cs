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
    static public extern IntPtr CreateFBXHandler();

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern void DestroyFBXHandler(IntPtr pClassNameObject);

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern void FillOutMesh(IntPtr pClassNameObject);

    [DllImport("FBXImporterDLL_WINDOWS", CallingConvention = CallingConvention.Cdecl)]
    static public extern int LoadMeshFromFBXFile(IntPtr pClassNameObject, string filePath);
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
    public class CSMesh
    {
        public CSMesh()
        {
            m_allVerticesPositions = null;
            m_allVerticesNormals = null;
            m_allVerticesUVs = null;
            m_indices = null;
        }
        public struct Vector2
        {
            public float x;
            public float y;
        }
        
        public struct Vector3
        {
            public float x;
            public float y;
            public float z;
        }

        public Vector3[] m_allVerticesPositions;
        public Vector3[] m_allVerticesNormals;
        public Vector2[] m_allVerticesUVs;

        public uint[] m_indices;

        public uint m_vertexCount;
        public uint m_indexCount;

        public Texture2D m_texture;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CSImportedFBXScene
    {
        [StructLayout(LayoutKind.Sequential)]
        public class CPPMesh
        {
            public CPPMesh()
            {
                m_allVerticesPositions  = IntPtr.Zero;
                m_indices               = IntPtr.Zero;
                m_normals               = IntPtr.Zero;
                m_uvs                   = IntPtr.Zero;
            }

            public IntPtr m_allVerticesPositions;
            public IntPtr m_normals;
            public IntPtr m_uvs;
            public IntPtr m_indices;
            public uint m_vertexCount;
            public uint m_indexCount;
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
        m_cppImportedFBXScene = CreateFBXHandler();

        int result = LoadFBXFile("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbx");

        if (1 == result)
        {
            // If the file loaded a mesh successfully, let's fill out the mesh component of this game object.
            FillOutGameObjectMesh();

            //m_csMesh.m_texture = LoadPNG("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbm\\SciFi_Character_Base_Color.png");
            //
            //m_meshRenderer.material.mainTexture = m_csMesh.m_texture;
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

    private int LoadFBXFile(string _fbxFilePath)
    {
        int result = LoadMeshFromFBXFile(m_cppImportedFBXScene, _fbxFilePath);

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

            m_csMesh.m_vertexCount = vertexCount;
            m_csMesh.m_indexCount = indexCount;

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
            tex = new Texture2D(2048, 1024);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    private void OnDestroy()
    {
        // Delete C++ handler which is unmanaged memory.
        DestroyFBXHandler(m_cppImportedFBXScene);

        m_cppImportedFBXScene = IntPtr.Zero;
    }

    /***************** Class Functions *****************/
}
