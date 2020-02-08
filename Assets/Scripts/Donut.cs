using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Donut : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Range(3, 100)]
    private int donutSections;

    [SerializeField]
    [Range(2, 100)]
    private int edgesPerSection;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float thickness;
    
    [SerializeField]
    private Vector2Int textureSize;

    [SerializeField]
    private Vector3 center;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private int verticesPerSectionCircle;
    private int verticesPerDonutCircle;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        UpdateDonut();
    }

    private void Update()
    {
        verticesPerSectionCircle = edgesPerSection + 1;
        verticesPerDonutCircle = donutSections + 1;
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
        vertices = new Vector3[verticesPerDonutCircle * (edgesPerSection + 1)];

        for (int i = 0; i < verticesPerDonutCircle; i++)
            for (int j = 0; j < verticesPerSectionCircle; j++)
            {
                Vector3 pos = new Vector3
                {
                    x = center.x + thickness * Mathf.Cos(2 * Mathf.PI / edgesPerSection * j) * Mathf.Cos(2 * Mathf.PI / donutSections * i) + radius * Mathf.Cos(2 * Mathf.PI / donutSections * i),
                    y = center.y + thickness * Mathf.Sin(2 * Mathf.PI / edgesPerSection * j),
                    z = center.z + thickness * Mathf.Cos(2 * Mathf.PI / edgesPerSection * j) * Mathf.Sin(2 * Mathf.PI / donutSections * i) + radius * Mathf.Sin(2 * Mathf.PI / donutSections * i)
                };
                vertices[i * verticesPerSectionCircle + j] = pos;
            }
    }

    private void FindUV()
    {
        uv = new Vector2[verticesPerDonutCircle * verticesPerSectionCircle];

        for (int i = 0; i < verticesPerDonutCircle; i++)
            for (int j = 0; j < verticesPerSectionCircle; j++)
            {
                Vector2 newUV = new Vector2(((float) textureSize.x / donutSections) * i, ((float) textureSize.y / edgesPerSection) * j); //Problem is that if value goes over 1, between vertex 9 and 0, it will go from e.g. uv 20 to 0 in a single face. Fix problem by adding extra set of vertices by making for loop <=, remove modulo from FindTriangles to instead of wrap make it to the last vertices.
                uv[i * verticesPerSectionCircle + j] = newUV;
            }
    }
    
    private void FindTriangles()
    {
        triangles = new int[donutSections * edgesPerSection * 6];

        for (int i = 0; i < donutSections; i++)
            for (int j = 0; j < edgesPerSection; j++)
            {
                triangles[(((i * edgesPerSection) + j) * 6)] = (i * verticesPerSectionCircle + j);
                triangles[(((i * edgesPerSection) + j) * 6) + 1] = (i * verticesPerSectionCircle + j + 1);
                triangles[(((i * edgesPerSection) + j) * 6) + 2] = (i * verticesPerSectionCircle + j + verticesPerSectionCircle);
                
                triangles[(((i * edgesPerSection) + j) * 6) + 3] = (i * verticesPerSectionCircle + j + verticesPerSectionCircle);
                triangles[(((i * edgesPerSection) + j) * 6) + 4] = (i * verticesPerSectionCircle + j + 1);
                triangles[(((i * edgesPerSection) + j) * 6) + 5] = (i * verticesPerSectionCircle + j + 1 + verticesPerSectionCircle);
            }

    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = new Color(0, (float)i % (float)(edgesPerSection + 1) / (float)(edgesPerSection + 1), 0);
            Gizmos.DrawSphere(vertices[i], 0.5f);
        }
    }
}
