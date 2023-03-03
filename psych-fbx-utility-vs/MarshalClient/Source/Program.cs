using System;
using System.Runtime.InteropServices;
using Psych;
using Psych.NativeMath;

namespace MarshalClient
{
    public static class FbxImporterDll
    {
#if DEBUG
        [DllImport("psychfbxutilityd")]
#else
        [DllImport("psychfbxutility")]
#endif
        public static extern IntPtr CreateFBXHandler();

#if DEBUG
        [DllImport("psychfbxutilityd")]
#else
        [DllImport("psychfbxutility")]
#endif
        public static extern void DestroyFBXHandler(IntPtr fbxHandlerObject);


#if DEBUG
        [DllImport("psychfbxutilityd")]
#else
        [DllImport("psychfbxutility", CallingConvention = CallingConvention.Cdecl)]
#endif

        public static extern int LoadFBXFile(IntPtr fbxHandlerObject, string fbxFilePath);
    }

    class Program
    {
        private static ManagedFbxHandler managedFbxHandler;
        private static IntPtr unmanagedFbxHandler;

        static void Main(string[] args)
        {
            // C++ handler which is unmanaged memory (we need to delete by calling DLLDestroyFBXHandler())
            unmanagedFbxHandler = FbxImporterDll.CreateFBXHandler();

            DateTime timeBeforeFileLoad = DateTime.Now;
            CRESULT result = (CRESULT)FbxImporterDll.LoadFBXFile(unmanagedFbxHandler, "../../../../resources/SM_PistolArnold.fbx");
            DateTime timeAfterFileLoad = DateTime.Now;
            Console.WriteLine(result);
            var durationFileLoad = timeAfterFileLoad - timeBeforeFileLoad;

            switch (result)
            {
                case CRESULT.CRESULT_SUCCESS:
                    {
                        DateTime timeBeforeFileParsed = DateTime.Now;
                        ParseFbxHandler();
                        DateTime timeAfterFileParsed = DateTime.Now;

                        var durationFileParsed = timeAfterFileParsed - timeBeforeFileParsed;

                        if (durationFileLoad.Seconds > 0)
                            Console.WriteLine("The FBX File loaded in: " + durationFileLoad.Seconds + "." + durationFileLoad.Milliseconds + " s");
                        else
                            Console.WriteLine("The FBX File loaded in: " + durationFileLoad.Milliseconds + " ms");

                        if (durationFileParsed.Seconds > 0)
                            Console.WriteLine("The FBX File was parsed in: " + durationFileParsed.Seconds + "." + durationFileParsed.Milliseconds + " s");
                        else
                            Console.WriteLine("The FBX File was parsed in: " + durationFileParsed.Milliseconds + " ms");
                    }
                    break;
                case CRESULT.CRESULT_INCORRECT_FILE_PATH:
                    Console.WriteLine("Incorrect File Path.");
                    break;
                case CRESULT.CRESULT_NO_OBJECTS_IN_SCENE:
                    Console.WriteLine("There were no objects in the given FBX scene.");
                    break;
                case CRESULT.CRESULT_NODE_WAS_NOT_GEOMETRY_TYPE:
                    Console.WriteLine("Please, make sure the FBX scene only contains geometry nodes.");
                    break;
                case CRESULT.CRESULT_ROOT_NODE_NOT_FOUND:
                    Console.WriteLine("The root node of the FBX file was not found.");
                    break;
                default:
                    break;
            }

            FbxImporterDll.DestroyFBXHandler(unmanagedFbxHandler);

            Console.ReadKey();
        }

        static void ParseFbxHandler()
        {
            managedFbxHandler = (ManagedFbxHandler)Marshal.PtrToStructure(unmanagedFbxHandler, typeof(ManagedFbxHandler));

            Console.WriteLine((int)managedFbxHandler.ManagedFBXScene.numberOfNativeObjects);

            for (uint currObjectIndex = 0;
                 currObjectIndex < (int)managedFbxHandler.ManagedFBXScene.numberOfNativeObjects;
                 currObjectIndex++)
            {
                Console.WriteLine(managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedName);

                Console.WriteLine("ManagedObject: " + managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedName + "\tMaterial Count: " + managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].nativeNumberOfMaterials + "\tNumber of Children: " + managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].nativeNumberOfChildren);

                Vector3[] vertices = managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedMesh.ManagedVertexPositions;
                Vector3[] normals = managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedMesh.ManagedNormals;
                Vector2[] uvs = managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedMesh.ManagedUVs;

                Vector3[] unityVerts = new Vector3[managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedMesh.nativeVertexCount];
                Vector3[] unityNormals = new Vector3[managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedMesh.nativeVertexCount];
                Vector2[] unityUVs = new Vector2[managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedMesh.nativeVertexCount];

                for (uint currVertex = 0; currVertex < managedFbxHandler.ManagedFBXScene.ManagedObjects[currObjectIndex].ManagedMesh.nativeVertexCount; currVertex++)
                {
                    unityVerts[currVertex].x = vertices[currVertex].x;
                    unityVerts[currVertex].y = vertices[currVertex].y;
                    unityVerts[currVertex].z = vertices[currVertex].z;

                    unityNormals[currVertex].x = normals[currVertex].x;
                    unityNormals[currVertex].y = normals[currVertex].y;
                    unityNormals[currVertex].z = normals[currVertex].z;

                    unityUVs[currVertex].x = uvs[currVertex].x;
                    unityUVs[currVertex].y = uvs[currVertex].y;
                }
            }
        }
    }
}
