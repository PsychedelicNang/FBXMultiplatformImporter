using CMath;
using System;
using System.Runtime.InteropServices;

namespace MarshalClient
{
    public static class FbxImporterDll
    {
        [DllImport("bffbxutility")]
        public static extern IntPtr CreateFBXHandler();

        [DllImport("bffbxutility")]
        public static extern void DestroyFBXHandler(IntPtr theFBXHandlerObject);

        [DllImport("bffbxutility", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadFBXFile(IntPtr theFBXHandlerObject, string fbxFilePath);
    }

    class Program
    {
        private static FBXHandler m_managedFBXHandler;
        private static IntPtr m_unmanagedFBXHandler;

        static void Main(string[] args)
        {
            TimeSpan m_durationFileLoad;
            TimeSpan m_durationFileParsed;

            // C++ handler which is unmanaged memory (we need to delete by calling DLLDestroyFBXHandler())
            m_unmanagedFBXHandler = FbxImporterDll.CreateFBXHandler();

            DateTime m_timeBeforeFileLoad = DateTime.Now;
            CRESULT result = (CRESULT)FbxImporterDll.LoadFBXFile(m_unmanagedFBXHandler, "C:/FBXMultiplatformImporter/bf-fbx-utility-vs/Resources/Lamp.fbx");
            DateTime m_timeAfterFileLoad = DateTime.Now;
            Console.WriteLine(result);
            m_durationFileLoad = m_timeAfterFileLoad - m_timeBeforeFileLoad;

            switch (result)
            {
                case CRESULT.CRESULT_SUCCESS:
                    {
                        DateTime m_timeBeforeFileParsed = DateTime.Now;
                        ParseFBXHandler();
                        DateTime m_timeAfterFileParsed = DateTime.Now;

                        m_durationFileParsed = m_timeAfterFileParsed - m_timeBeforeFileParsed;

                        if (m_durationFileLoad.Seconds > 0)
                            Console.WriteLine("The FBX File loaded in: " + m_durationFileLoad.Seconds + "." + m_durationFileLoad.Milliseconds + " s");
                        else
                            Console.WriteLine("The FBX File loaded in: " + m_durationFileLoad.Milliseconds + " ms");

                        if (m_durationFileParsed.Seconds > 0)
                            Console.WriteLine("The FBX File was parsed in: " + m_durationFileParsed.Seconds + "." + m_durationFileParsed.Milliseconds + " s");
                        else
                            Console.WriteLine("The FBX File was parsed in: " + m_durationFileParsed.Milliseconds + " ms");
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

            FbxImporterDll.DestroyFBXHandler(m_unmanagedFBXHandler);

            Console.ReadKey();
        }

        static void ParseFBXHandler()
        {
            m_managedFBXHandler = (FBXHandler)Marshal.PtrToStructure(m_unmanagedFBXHandler, typeof(FBXHandler));

            Console.WriteLine((int)m_managedFBXHandler.FBXScene.m_numberOfObjects);

            for (uint currObjectIndex = 0;
                 currObjectIndex < (int)m_managedFBXHandler.FBXScene.m_numberOfObjects;
                 currObjectIndex++)
            {
                Console.WriteLine(m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Name);

                Console.WriteLine("Object: " + m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Name + "\tMaterial Count: " + m_managedFBXHandler.FBXScene.Objects[currObjectIndex].m_numberOfMaterials + "\tNumber of Children: " + m_managedFBXHandler.FBXScene.Objects[currObjectIndex].m_numberOfChildren);

                Vector3[] vertices = m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Mesh.VertexPositions;
                Vector3[] normals = m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Mesh.Normals;
                Vector2[] uvs = m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Mesh.UVs;

                Vector3[] unityVerts = new Vector3[m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Mesh.m_vertexCount];
                Vector3[] unityNormals = new Vector3[m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Mesh.m_vertexCount];
                Vector2[] unityUVs = new Vector2[m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Mesh.m_vertexCount];

                for (uint currVertex = 0; currVertex < m_managedFBXHandler.FBXScene.Objects[currObjectIndex].Mesh.m_vertexCount; currVertex++)
                {
                    unityVerts[currVertex].x = vertices[currVertex].x;
                    unityVerts[currVertex].y = vertices[currVertex].y;
                    unityVerts[currVertex].z = vertices[currVertex].z;

                    unityNormals[currVertex].x = normals[currVertex].x;
                    unityNormals[currVertex].y = normals[currVertex].y;
                    unityNormals[currVertex].z = normals[currVertex].z;

                    unityUVs[currVertex].x = uvs[currVertex].x;
                    unityUVs[currVertex].y = uvs[currVertex].y;

                    Console.WriteLine($"{unityVerts[currVertex]}, {unityNormals[currVertex]}, {unityUVs[currVertex]}");
                }
            }
        }
    }
}
