using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class UseFBXDLLHandler : MonoBehaviour
{
    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern IntPtr CreateFBXHandler();

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern void DestroyFBXHandler(IntPtr pClassNameObject);

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern bool SetFloat(IntPtr pClassNameObject, float _float);

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern void FillOutArray(IntPtr pClassNameObject, int arrayLength, [MarshalAs(UnmanagedType.LPArray)]int[] incides);

    [DllImport("FBXImporterDLL_WINDOWS")]
    static public extern void FillOutMesh(IntPtr pClassNameObject);

    CSImportedFBXScene  m_csImportedFBXScene;
    IntPtr              m_cppImportedFBXScene;

    [StructLayout(LayoutKind.Sequential)]
    public class Mesh
    {
        public uint vertexCount;
        public uint indexCount;
    }

    Mesh m_mesh;

    [StructLayout(LayoutKind.Sequential)]
    public class CSImportedFBXScene
    {
        public CSImportedFBXScene()
        {
            m_CPPMeshPtr = IntPtr.Zero;
        }

        public IntPtr m_CPPMeshPtr;
    }

    // Use this for initialization
    void Start()
    {
        m_csImportedFBXScene = new CSImportedFBXScene();

        // I have my C++ handler which is unmanaged memory (we need to delete)
        m_cppImportedFBXScene = CreateFBXHandler();

        FillOutMesh(m_cppImportedFBXScene);

        m_csImportedFBXScene = (CSImportedFBXScene)Marshal.PtrToStructure(m_cppImportedFBXScene, typeof(CSImportedFBXScene));

        m_mesh = (Mesh)Marshal.PtrToStructure(m_csImportedFBXScene.m_CPPMeshPtr, typeof(Mesh));
        print(m_mesh.vertexCount);
        print(m_mesh.indexCount);
    }

    private void OnDestroy()
    {
        DestroyFBXHandler(m_cppImportedFBXScene);

        m_cppImportedFBXScene = IntPtr.Zero;
    }
}
