using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Donut : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Range(2, 100)]
    private int verticesPerCircle;

    [SerializeField]
    [Range(3, 100)]
    private int sections;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float thickness;
    
    [SerializeField]
    private Vector2 textureSize; // make it so its a Vector2Int and it determines how many times the texture will wrap, regardless of vertexcount

    [SerializeField]
    private Vector3 center;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        UpdateDonut();
    }

    private void Update()
    {
        UpdateDonut();
    }

    private void UpdateDonut()
    {
        mesh.Clear();
        
        FindVertices();
        FindUV();
        FindTriangles();
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
    }

    private void FindVertices()
    {
        vertices = new Vector3[(sections + 1) * (verticesPerCircle + 1)];

        for (int i = 0; i < (sections + 1); i++)
            for (int j = 0; j <= verticesPerCircle; j++)
            {
                Vector3 pos = new Vector3
                {
                    x = center.x + thickness * Mathf.Cos(2 * Mathf.PI / verticesPerCircle * j) * Mathf.Cos(2 * Mathf.PI / sections * i) + radius * Mathf.Cos(2 * Mathf.PI / sections * i),
                    y = center.y + thickness * Mathf.Sin(2 * Mathf.PI / verticesPerCircle * j),
                    z = center.z + thickness * Mathf.Cos(2 * Mathf.PI / verticesPerCircle * j) * Mathf.Sin(2 * Mathf.PI / sections * i) + radius * Mathf.Sin(2 * Mathf.PI / sections * i)
                };
                vertices[i * verticesPerCircle + j] = pos;
            }
    }

    private void FindUV()
    {
        uv = new Vector2[(sections + 1) * (verticesPerCircle + 1)];

        for (int i = 0; i < (sections + 1); i++)
            for (int j = 0; j <= verticesPerCircle; j++)
            {
                Vector2 newUV = new Vector2(i / textureSize.x, j / textureSize.y); //Problem is that if value goes over 1, between vertex 9 and 0, it will go from e.g. uv 20 to 0 in a single face. Fix problem by adding extra set of vertices by making for loop <=, remove modulo from FindTriangles to instead of wrap make it to the last vertices.
                uv[i * verticesPerCircle + j] = newUV;
            }
    }
    
    private void FindTriangles()
    {
        triangles = new int[sections * verticesPerCircle * 6];
        
        for (int i = 0; i < sections; i++)
        {
            for (int j = 0; j < verticesPerCircle; j++)
            {
                triangles[(((i * verticesPerCircle) + j) * 6)] = (i * verticesPerCircle + j);
                triangles[(((i * verticesPerCircle) + j) * 6) + 1] = (i * verticesPerCircle + ((j + 1)));
                triangles[(((i * verticesPerCircle) + j) * 6) + 2] = (i * verticesPerCircle + j + verticesPerCircle);
                
                triangles[(((i * verticesPerCircle) + j) * 6) + 3] = (i * verticesPerCircle + j + verticesPerCircle);
                triangles[(((i * verticesPerCircle) + j) * 6) + 4] = (i * verticesPerCircle + ((j + 1)));
                triangles[(((i * verticesPerCircle) + j) * 6) + 5] = (i * verticesPerCircle + ((j + 1)) + verticesPerCircle);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = new Color(0, (float)i / vertices.Length, 0);
            Gizmos.DrawSphere(vertices[i], 0.5f);
        }
    }
}
