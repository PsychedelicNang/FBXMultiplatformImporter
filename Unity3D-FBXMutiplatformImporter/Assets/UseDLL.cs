using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class UseDLL : MonoBehaviour {
    [DllImport("VisualStudio-FBXMultiplatformImporter")]
    private static extern float TestMultiply(float _a, float _b);

    [DllImport("VisualStudio-FBXMultiplatformImporter")]
    static public extern IntPtr CreateCPPClass();

    [DllImport("VisualStudio-FBXMultiplatformImporter")]
    static public extern void DestroyCPPClass(IntPtr pClassNameObject);

    [DllImport("VisualStudio-FBXMultiplatformImporter")]
    static public extern void CPPClassMemberFunciton(IntPtr pClassNameObject, float _float);

    [DllImport("VisualStudio-FBXMultiplatformImporter")]
    static public extern void FillOutArray(IntPtr pClassNameObject, int arrayLength, [MarshalAs(UnmanagedType.LPArray)]int[] incides);
    //static public extern void FillOutArray(IntPtr pClassNameObject);

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
    }

    // Use this for initialization
    void Start () {
        CSSideCPPClass cSSideCPPClass = new CSSideCPPClass();

        print("Native Multiply: " + TestMultiply(5, 4));

        IntPtr m_csClass = CreateCPPClass();

        CPPClassMemberFunciton(m_csClass, 5);

        print("IntPtr: " + m_csClass.ToInt32());

        FillOutArray(m_csClass, 10, cSSideCPPClass.indices);

        print("CPP Class array member variables: ");
        for (int i = 0; i < cSSideCPPClass.m_numberOfIndices; i++)
        {
            print(cSSideCPPClass.indices[i] + ", ");
        }

        DestroyCPPClass(m_csClass);

        m_csClass = IntPtr.Zero;

    }
}
