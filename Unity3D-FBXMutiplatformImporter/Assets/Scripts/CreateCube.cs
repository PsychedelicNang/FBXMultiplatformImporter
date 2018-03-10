using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCube : MonoBehaviour {

    // Use this for initialization
    void Start () {
        MeshFilter m_meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer m_meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Mesh m_mesh = m_meshFilter.mesh;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
        vertices[1] = new Vector3(0.0f, 40.0f, 0.0f);
        vertices[2] = new Vector3(40.0f, 40.0f, 0.0f);
        vertices[3] = new Vector3(40.0f, 0.0f, 0.0f);

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        Vector2[] uvs = new Vector2[4];

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(0, 1);
        uvs[3] = new Vector2(1, 1);

        //m_mesh.uv = uvs;
        m_mesh.vertices = vertices;
        m_mesh.triangles = triangles;
        m_meshFilter.mesh = m_mesh;

        //m_mesh.RecalculateBounds();
        m_mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update () {
     }
}

