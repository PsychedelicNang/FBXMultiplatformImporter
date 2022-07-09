#define USE_MATERIALS
#define USE_MESHES

using System;
using System.Runtime.InteropServices;

namespace CMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public float x;
        public float y;

        public override string ToString()
        {
            return $"{x}, {y}";
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public override string ToString()
        {
            return $"{x}, {y}, {z}";
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public override string ToString()
        {
            return $"{x}, {y}, {z}, {w}";
        }
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

#if USE_MATERIALS
public struct Material
{
    public struct PropertyData
    {
        public readonly CMaterialInfo.PropertyType m_propertyType;
        private IntPtr m_textureRelativeFileName;
        public string TextureRelativeFileName => Marshal.PtrToStringAnsi(m_textureRelativeFileName);

        private IntPtr m_textureAbsoluteFilePath;
        public string TextureAbsoluteFilePath => Marshal.PtrToStringAnsi(m_textureAbsoluteFilePath);

        private CMath.Vector4 m_dataColorValues;
    };

    public readonly CMaterialInfo.MaterialType m_materialType;
    private IntPtr m_materialProperties;

    public PropertyData[] MaterialProperties =>
        MarshalHelpers.MarshalUnmanagedArray2Struct<PropertyData>(m_materialProperties, (int)CMaterialInfo.PropertyType.PROPERTYTYPE_COUNT);

    //public Texture2D[] Textures
    //{
    //    get
    //    {
    //        Texture2D[] result = new Texture2D[m_textureCount];

    //        var props = MaterialProperties;
    //        for (int i = 0; i < props.Length; i++)
    //        {
    //            result[i] = MarshalHelpers.LoadPNG(props[i].TextureAbsoluteFilePath);
    //        }

    //        return result;
    //    }
    //}

    public uint m_textureCount;
}
#endif

#if USE_MESHES
public struct Mesh
{
    private IntPtr m_allVerticesPositions;
    public CMath.Vector3[] VertexPositions =>
        MarshalHelpers.MarshalUnmanagedArray2Struct<CMath.Vector3>(m_allVerticesPositions, (int)m_vertexCount);

    private IntPtr m_normals;

    public CMath.Vector3[] Normals =>
        MarshalHelpers.MarshalUnmanagedArray2Struct<CMath.Vector3>(m_normals, (int)m_vertexCount);

    private IntPtr m_uvs;

    public CMath.Vector2[] UVs =>
        MarshalHelpers.MarshalUnmanagedArray2Struct<CMath.Vector2>(m_uvs, (int)m_vertexCount);

    private IntPtr m_indices;

    public uint[] Indices =>
        MarshalHelpers.MarshalUnmanagedArray2Struct<uint>(m_indices, (int)m_indexCount);

    private int[] ConvertedIndices =>
        Array.ConvertAll(Indices, val => checked((int)val));

    public readonly uint m_vertexCount;
    public readonly uint m_indexCount;
}
#endif

public struct Object
{
    private int m_parentArrayIndexID;
    private IntPtr m_childrenArrayIndexIDs;

#if USE_MESHES
    private IntPtr m_mesh;

    public Mesh Mesh => Marshal.PtrToStructure<Mesh>(m_mesh);
#endif
#if USE_MATERIALS
    private IntPtr m_materials;

    public Material[] Materials =>
        MarshalHelpers.MarshalUnmanagedArray2Struct<Material>(m_materials, (int)m_numberOfMaterials);
#endif

    public readonly uint m_numberOfChildren;
    public readonly uint m_numberOfMaterials;

    private IntPtr m_name;

    public string Name =>
        Marshal.PtrToStringAnsi(m_name);

    public readonly uint m_arrayIndexID;
}

public struct ImportedFBXScene
{
    private IntPtr m_objects;
    public readonly uint m_numberOfObjects;

    public Object[] Objects =>
        MarshalHelpers.MarshalUnmanagedArray2Struct<Object>(m_objects, (int)m_numberOfObjects);
}

public struct FBXHandler
{
    private IntPtr m_fbxScene;

    public ImportedFBXScene FBXScene => Marshal.PtrToStructure<ImportedFBXScene>(m_fbxScene);
}

public class MarshalHelpers
{
    public static void MarshalUnmanagedArray2Struct<T>(IntPtr unmanagedArray, int length, out T[] managedArray)
    {
        var size = Marshal.SizeOf(typeof(T));
        managedArray = new T[length];

        for (int i = 0; i < length; i++)
        {
            IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
            managedArray[i] = Marshal.PtrToStructure<T>(ins);
        }
    }

    public static T[] MarshalUnmanagedArray2Struct<T>(IntPtr unmanagedArray, int length)
    {
        var size = Marshal.SizeOf(typeof(T));
        T[] managedArray = new T[length];

        for (int i = 0; i < length; i++)
        {
            IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
            managedArray[i] = Marshal.PtrToStructure<T>(ins);
        }

        return managedArray;
    }

    //public static Texture2D LoadPNG(string _filePath)
    //{
    //    Texture2D tex = null;
    //    byte[] fileData;

    //    if (File.Exists(_filePath))
    //    {
    //        fileData = File.ReadAllBytes(_filePath);
    //        tex = new Texture2D(1, 1);
    //        tex.LoadImage(fileData); // LoadImage() auto resizes the texture dimensions.
    //    }

    //    else
    //    {
    //        return null;
    //    }

    //    return tex;
    //}
}