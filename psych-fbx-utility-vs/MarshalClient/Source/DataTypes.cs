#define USE_MATERIALS
#define USE_MESHES

using System;
using System.Runtime.InteropServices;

namespace Psych
{
    namespace NativeMath
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

    namespace NativeMaterialInfo
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
    public struct ManagedMaterial
    {
        public struct ManagedPropertyData
        {
            public readonly NativeMaterialInfo.PropertyType nativePropertyType;
            private IntPtr nativeTextureRelativeFileName;
            public string ManagedTextureRelativeFileName => Marshal.PtrToStringAnsi(nativeTextureRelativeFileName);

            private IntPtr nativeTextureAbsoluteFilePath;
            public string ManagedTextureAbsoluteFilePath => Marshal.PtrToStringAnsi(nativeTextureAbsoluteFilePath);

            private NativeMath.Vector4 nativeDataColorValues;
        };

        public readonly NativeMaterialInfo.MaterialType nativeMaterialType;
        private IntPtr nativeMaterialProperties;

        public ManagedPropertyData[] ManagedMaterialProperties =>
            MarshalHelpers.MarshalUnmanagedArray2Struct<ManagedPropertyData>(nativeMaterialProperties,
                (int) NativeMaterialInfo.PropertyType.PROPERTYTYPE_COUNT);

        public uint nativeTextureCount;
    }
#endif

#if USE_MESHES
    public struct ManagedMesh
    {
        private IntPtr nativeAllVerticesPositions;

        public NativeMath.Vector3[] ManagedVertexPositions =>
            MarshalHelpers.MarshalUnmanagedArray2Struct<NativeMath.Vector3>(nativeAllVerticesPositions, (int) nativeVertexCount);

        private IntPtr nativeNormals;

        public NativeMath.Vector3[] ManagedNormals =>
            MarshalHelpers.MarshalUnmanagedArray2Struct<NativeMath.Vector3>(nativeNormals, (int) nativeVertexCount);

        private IntPtr nativeUvs;

        public NativeMath.Vector2[] ManagedUVs =>
            MarshalHelpers.MarshalUnmanagedArray2Struct<NativeMath.Vector2>(nativeUvs, (int) nativeVertexCount);

        private IntPtr nativeIndices;

        public uint[] ManagedIndices =>
            MarshalHelpers.MarshalUnmanagedArray2Struct<uint>(nativeIndices, (int) nativeIndexCount);

        private int[] ConvertedIndices =>
            Array.ConvertAll(ManagedIndices, val => checked((int) val));

        public readonly uint nativeVertexCount;
        public readonly uint nativeIndexCount;
    }
#endif

    public struct ManagedObject
    {
        private int nativeParentArrayIndexID;
        private IntPtr nativeChildrenArrayIndexIDs;

#if USE_MESHES
        private IntPtr nativeMesh;

        public ManagedMesh ManagedMesh => Marshal.PtrToStructure<ManagedMesh>(nativeMesh);
#endif
#if USE_MATERIALS
        private IntPtr nativeMaterials;

        public ManagedMaterial[] ManagedMaterials =>
            MarshalHelpers.MarshalUnmanagedArray2Struct<ManagedMaterial>(nativeMaterials, (int) nativeNumberOfMaterials);
#endif

        public readonly uint nativeNumberOfChildren;
        public readonly uint nativeNumberOfMaterials;

        private IntPtr nativeName;

        public string ManagedName =>
            Marshal.PtrToStringAnsi(nativeName);

        public readonly uint nativeArrayIndexID;
    }

    public struct ManagedImportedFbxScene
    {
        private IntPtr nativeObjects;
        public readonly uint numberOfNativeObjects;

        public ManagedObject[] ManagedObjects =>
            MarshalHelpers.MarshalUnmanagedArray2Struct<ManagedObject>(nativeObjects, (int) numberOfNativeObjects);
    }

    public struct ManagedFbxHandler
    {
        private IntPtr nativeFbxScene;

        public ManagedImportedFbxScene ManagedFBXScene => Marshal.PtrToStructure<ManagedImportedFbxScene>(nativeFbxScene);
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
    }
}