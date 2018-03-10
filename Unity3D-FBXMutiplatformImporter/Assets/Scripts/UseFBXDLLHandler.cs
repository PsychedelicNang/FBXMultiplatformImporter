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
    /***************** Import DLL Functions *****************/


    /***************** Member Variables *****************/
    CSImportedFBXScene  m_csImportedFBXScene;
    IntPtr              m_cppImportedFBXScene;
    CSMesh              m_csMesh;
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
        m_csMesh = new CSMesh();

        m_csImportedFBXScene = new CSImportedFBXScene();

        // I have my C++ handler which is unmanaged memory (we need to delete)
        m_cppImportedFBXScene = CreateFBXHandler();

        FillOutMesh(m_cppImportedFBXScene);

        m_csImportedFBXScene = (CSImportedFBXScene)Marshal.PtrToStructure(m_cppImportedFBXScene, typeof(CSImportedFBXScene));

        m_csImportedFBXScene.m_mesh = (CSImportedFBXScene.CPPMesh)Marshal.PtrToStructure(m_csImportedFBXScene.m_CPPMeshPtr, typeof(CSImportedFBXScene.CPPMesh));

        uint vertexCount = m_csImportedFBXScene.m_mesh.m_vertexCount;
        uint indexCount = m_csImportedFBXScene.m_mesh.m_indexCount;

        m_csMesh.m_allVerticesPositions = new CSMesh.Vector3[vertexCount];
        m_csMesh.m_indices = new uint[indexCount];

        unsafe
        {
            Vector3* vertices = (Vector3*)m_csImportedFBXScene.m_mesh.m_allVerticesPositions.ToPointer();

            for (int i = 0; i < vertexCount; i++)
            {
                m_csMesh.m_allVerticesPositions[i].x = vertices[i].x;
                m_csMesh.m_allVerticesPositions[i].y = vertices[i].y;
                m_csMesh.m_allVerticesPositions[i].z = vertices[i].z;
            }

            uint* indices = (uint*)m_csImportedFBXScene.m_mesh.m_indices.ToPointer();
            for (int i = 0; i < indexCount; i++)
            {
                m_csMesh.m_indices[i] = indices[i];
            }
        }

        print(m_csImportedFBXScene.m_mesh.m_vertexCount);
        print(m_csImportedFBXScene.m_mesh.m_indexCount);

        for (int i = 0; i < m_csImportedFBXScene.m_mesh.m_vertexCount; i++)
        {
            print(m_csMesh.m_allVerticesPositions[i].x);
            print(m_csMesh.m_allVerticesPositions[i].y);
            print(m_csMesh.m_allVerticesPositions[i].z);
        }

        for (int i = 0; i < m_csImportedFBXScene.m_mesh.m_indexCount; i++)
        {
            print(m_csMesh.m_indices[i]);
        }
    }

    private void OnDestroy()
    {
        DestroyFBXHandler(m_cppImportedFBXScene);

        m_cppImportedFBXScene = IntPtr.Zero;
    }

    /***************** Class Functions *****************/
}
