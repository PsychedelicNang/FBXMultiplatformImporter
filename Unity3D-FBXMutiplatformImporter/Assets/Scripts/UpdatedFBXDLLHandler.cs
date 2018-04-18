using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

namespace CMath
{
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

public class UpdatedFBXDLLHandler : MonoBehaviour {
    /***************** Member Variables *****************/
    CSImportedFBXScene  m_csImportedFBXScene;
    IntPtr              m_cppImportedFBXScene;

    MeshFilter m_unityMeshFilter;
    Mesh m_unityMesh;
    MeshRenderer m_unityMeshRenderer;
    Transform m_unityObjectTransform;
    /***************** Member Variables *****************/

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

            [MarshalAs(UnmanagedType.LPStr)]
            public string m_textureRelativeFileName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string m_textureAbsoluteFilePath;

            public CMath.Vector4 m_dataColorValues;
        };

        public CMaterialInfo.MaterialType m_materialType;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct)]
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
        public Vector3[] m_allVerticesPositions;
        public Vector3[] m_allVerticesNormals;
        public Vector2[] m_allVerticesUVs;

        public uint[] m_indices;

        public uint m_vertexCount;
        public uint m_indexCount;
    }

    public struct CPPObject
    {
        public IntPtr m_meshes;
        public IntPtr m_materials;

        public uint m_numberOfMeshes;
        public uint m_numberOfMaterials;
    }

    public struct CSObject
    {
        public CSMesh[]     m_meshes;
        public CSMaterial[] m_materials;

        public uint         m_numberOfMeshes;
        public uint         m_numberOfMaterials;
    }

    public struct CPPImportedFBXScene
    {
        IntPtr m_objects;
    }

    public struct CSImportedFBXScene
    {
        CSObject[] m_objects;
    }
    /***************** Class Definitions *****************/


    /***************** Class Functions *****************/
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /***************** Class Functions *****************/
}
