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

    CSSideCPPClass cppImitationClass;
    IntPtr m_cppClass;

    [StructLayout(LayoutKind.Sequential)]
    public class Mesh
    {
        public uint vertexCount;
        public uint indexCount;
    }

    Mesh m_mesh;

    [StructLayout(LayoutKind.Sequential)]
    public class CSSideCPPClass
    {
        public CSSideCPPClass()
        {
            //m_numberOfIndices = 10;
            //indices = new int[m_numberOfIndices];
            //m_mesh = new Mesh();
            //Marshal.StructureToPtr(m_mesh, cppMesh, false);
        }



        public float m_float;
        public uint m_numberOfIndices;
        //public Mesh m_mesh;
        public IntPtr m_CPPMeshPtr;
        //[MarshalAs(UnmanagedType.LPStruct)]
        //public Mesh m_mesh;
        //[MarshalAs(UnmanagedType.LPArray)]
        //public int[] indices;

        //[StructLayout(LayoutKind.Sequential)]
        //public class Mesh
        //{
        //    public Mesh()
        //    {
        //        //vertices = IntPtr.Zero;
        //        //indices = new uint[1];
        //        indexCount = 0;
        //        vertexCount = 0;
        //        //public IntPtr vertices;
        //        //[MarshalAs(UnmanagedType.LPArray)]
        //        //public uint[] indices;
        //        //public uint indexCount;
        //        //public uint vertexCount;
        //    }

        //    //public IntPtr vertices;
        //    //[MarshalAs(UnmanagedType.LPArray)]
        //    //public uint[] indices;
        //    public uint indexCount;
        //    public uint vertexCount;
        //}

        //public Mesh m_mesh;
        //public IntPtr cppMesh;
    }

    // Use this for initialization
    void Start()
    {
        cppImitationClass = new CSSideCPPClass();

        // I have my C++ handler which is unmanaged memory (we need to delete)
        m_cppClass = CreateFBXHandler();

        Marshal.StructureToPtr(cppImitationClass, m_cppClass, false);
        bool result = SetFloat(m_cppClass, 5);

        FillOutMesh(m_cppClass);

        if (result)
        {
            //print("IntPtr: " + m_cppClass.ToInt32());
            cppImitationClass = (CSSideCPPClass)Marshal.PtrToStructure(m_cppClass, typeof(CSSideCPPClass));
            //print(cppImitationClass.m_float);
            //print(cppImitationClass.m_mesh.indexCount);
            //print(cppImitationClass.m_mesh.vertexCount);

            m_mesh = (Mesh)Marshal.PtrToStructure(cppImitationClass.m_CPPMeshPtr, typeof(Mesh));
            print(m_mesh.vertexCount);
            print(m_mesh.indexCount);
        }


        // m_mesh = (Mesh)Marshal.PtrToStructure(cppImitationClass.m_CPPMeshPtr, typeof(Mesh));

        //print(m_mesh.vertexCount);
        //print(m_mesh.indexCount);
        //FillOutArray(m_cppClass, 10, cppImitationClass.indices);

        //print("CPP Class array member variables: ");
        //for (int i = 0; i < cppImitationClass.m_numberOfIndices; i++)
        //{
        //    print(cppImitationClass.indices[i] + ", ");
        //}

        //Marshal.StructureToPtr(cppImitationClass.m_mesh, cppMesh, false);


        //cppImitationClass.m_mesh = (CSSideCPPClass.Mesh)Marshal.PtrToStructure(cppImitationClass.cppMesh, typeof(CSSideCPPClass.Mesh));

        //print(cppImitationClass.m_mesh.vertexCount);
        //LoadMeshFromFBXFileWithoutTangents(m_csClass, "C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\SciFiCharacter.fbx");
    }

    private void OnDestroy()
    {
        DestroyFBXHandler(m_cppClass);

        m_cppClass = IntPtr.Zero;
    }
}
