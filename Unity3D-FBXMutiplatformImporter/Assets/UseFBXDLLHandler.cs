using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class UseFBXDLL : MonoBehaviour
{
    [DllImport("VisualStudio-FBXMultiplatformImporter 1")]
    private static extern float TestMultiply(float _a, float _b);

    [DllImport("VisualStudio-FBXMultiplatformImporter 1")]
    static public extern IntPtr CreateCPPClass();

    [DllImport("VisualStudio-FBXMultiplatformImporter 1")]
    static public extern void DestroyCPPClass(IntPtr pClassNameObject);

    [DllImport("VisualStudio-FBXMultiplatformImporter 1")]
    static public extern void CPPClassMemberFunciton(IntPtr pClassNameObject, float _float);

    [DllImport("VisualStudio-FBXMultiplatformImporter 1")]
    static public extern void FillOutArray(IntPtr pClassNameObject, int arrayLength, [MarshalAs(UnmanagedType.LPArray)]int[] incides);

    [DllImport("VisualStudio-FBXMultiplatformImporter 1")]
    static public extern bool LoadMeshFromFBXFileWithoutTangents(IntPtr pClassNameObject, [MarshalAs(UnmanagedType.BStr)]string filePath);

    CSSideCPPClass cSSideCPPClass;
    IntPtr m_csClass;

    public class CSSideCPPClass
    {
        public CSSideCPPClass()
        {
            m_numberOfIndices = 10;
            indices = new int[m_numberOfIndices];
        }

        public uint m_numberOfIndices;

        [MarshalAs(UnmanagedType.LPArray)]
        public int[] indices;

        //[StructLayout(LayoutKind.Sequential)]
        //public struct Mesh
        //{
        //    [MarshalAs(UnmanagedType.LPArray)]
        //    public uint[] indices;
        //    public IntPtr vertices;
        //    public uint indexCount;
        //    public uint vertexCount;
        //}

        //public Mesh m_mesh;
    }

    // Use this for initialization
    void Start()
    {
        cSSideCPPClass = new CSSideCPPClass();

        print("Native Multiply: " + TestMultiply(5, 4));

        m_csClass = CreateCPPClass();

        CPPClassMemberFunciton(m_csClass, 5);

        print("IntPtr: " + m_csClass.ToInt32());

        FillOutArray(m_csClass, 10, cSSideCPPClass.indices);

        print("CPP Class array member variables: ");
        for (int i = 0; i < cSSideCPPClass.m_numberOfIndices; i++)
        {
            print(cSSideCPPClass.indices[i] + ", ");
        }

        //LoadMeshFromFBXFileWithoutTangents(m_csClass, "C:\\Users\\Brandon\\Desktop\\GameEngineBF\\EngineBJF\\FBXLibraryHandler\\SciFiCharacter\\SciFiCharacter.fbx");
    }

    private void OnDestroy()
    {
        DestroyCPPClass(m_csClass);

        m_csClass = IntPtr.Zero;
    }
}
