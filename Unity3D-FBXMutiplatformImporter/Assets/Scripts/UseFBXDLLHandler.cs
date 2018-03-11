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
        public struct Vector3
        {
            public float x;
            public float y;
            public float z;
        }

        public Vector3[] m_allVerticesPositions;

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
            }

            public IntPtr m_allVerticesPositions;
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

        // I have my C++ handler which is unmanaged memory (we need to delete)
        m_cppImportedFBXScene = CreateFBXHandler();
        
        bool result = LoadFBXFile("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbx");

        if (result)
        {
            Vector3[] unityVerts = new Vector3[m_csMesh.m_vertexCount];
            int[] unityIndices = new int[m_csMesh.m_indexCount];

            for (int i = 0; i < m_csMesh.m_vertexCount; i++)
            {
                unityVerts[i].x = m_csMesh.m_allVerticesPositions[i].x;
                unityVerts[i].y = m_csMesh.m_allVerticesPositions[i].y;
                unityVerts[i].z = m_csMesh.m_allVerticesPositions[i].z;
            }

            for (int i = 0; i < m_csMesh.m_indexCount; i++)
            {
                unityIndices[i] = (int)m_csMesh.m_indices[i];
            }

            m_unityMesh.vertices = unityVerts;
            m_unityMesh.triangles = unityIndices;
            m_meshFilter.mesh = m_unityMesh;


            //m_csMesh.m_texture = LoadPNG("C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\NewSciFiCharacter.fbm\\SciFi_Character_Base_Color.png");
            //
            //m_meshRenderer.material.mainTexture = m_csMesh.m_texture;
        }

        //unityMesh.RecalculateBounds();
        //m_unityMesh.RecalculateNormals();

        m_objectTransform.Translate(new Vector3(0.0f, -10.0f));
        m_objectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    private bool LoadFBXFile(string _fbxFilePath)
    {
        int result = LoadMeshFromFBXFile(m_cppImportedFBXScene, _fbxFilePath);

        if (result == 1)
        {
            m_csImportedFBXScene = (CSImportedFBXScene)Marshal.PtrToStructure(m_cppImportedFBXScene, typeof(CSImportedFBXScene));

            m_csImportedFBXScene.m_mesh = (CSImportedFBXScene.CPPMesh)Marshal.PtrToStructure(m_csImportedFBXScene.m_CPPMeshPtr, typeof(CSImportedFBXScene.CPPMesh));

            uint vertexCount = m_csImportedFBXScene.m_mesh.m_vertexCount;
            uint indexCount = m_csImportedFBXScene.m_mesh.m_indexCount;

            //unityMesh.vertices = new Vector3[vertexCount];
            //unityMesh.triangles = new int[indexCount];

            m_csMesh.m_vertexCount = vertexCount;
            m_csMesh.m_indexCount = indexCount;

            m_csMesh.m_allVerticesPositions = new CSMesh.Vector3[vertexCount];
            m_csMesh.m_indices = new uint[indexCount];

            unsafe
            {
                CSMesh.Vector3* vertices = (CSMesh.Vector3*)m_csImportedFBXScene.m_mesh.m_allVerticesPositions.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    //unityMesh.vertices[i].x = vertices[i].x;
                    //unityMesh.vertices[i].y = vertices[i].y;
                    //unityMesh.vertices[i].z = vertices[i].z;

                     m_csMesh.m_allVerticesPositions[i].x = vertices[i].x;
                     m_csMesh.m_allVerticesPositions[i].y = vertices[i].y;
                     m_csMesh.m_allVerticesPositions[i].z = vertices[i].z;
                }

                uint* indices = (uint*)m_csImportedFBXScene.m_mesh.m_indices.ToPointer();
                for (int i = 0; i < indexCount; i++)
                {
                    //unityMesh.triangles[i] = (int)indices[i];
                    m_csMesh.m_indices[i] = indices[i];
                }
            }

            //print(m_csImportedFBXScene.m_mesh.m_vertexCount);
            //print(m_csImportedFBXScene.m_mesh.m_indexCount);

            //unityMesh.RecalculateNormals();

            return true;
        }

        if (result == -1)
        {
            print("File path did not load");
        }


        if (result == -2)
        {
            print("File path loaded but root node was not found");
        }

        if (result == -3)
        {
            print("well...");
        }

        // Returns false if the file was not loaded
        return false;
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
        DestroyFBXHandler(m_cppImportedFBXScene);

        m_cppImportedFBXScene = IntPtr.Zero;
    }

    /***************** Class Functions *****************/
}
