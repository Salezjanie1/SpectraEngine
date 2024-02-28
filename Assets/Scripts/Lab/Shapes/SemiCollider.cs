using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class SemiCollider : MonoBehaviour
{
    public float radius = 1f;
    public int segments = 20;
    public float thickness = 0.1f;
    public float rotationDegrees = 0f;

    private void Awake()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = CreateSemicircleMesh();
    }

    private Mesh CreateSemicircleMesh()
    {
        Mesh mesh = new Mesh();

        int totalVertices = (segments * 2 + 2) * 2; // Double the vertices
        int totalTriangles = segments * 6 * 2; // Double the triangles

        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[totalTriangles];

        float angleStep = Mathf.PI / segments;
        int vertexIndex = 0;
        int triangleIndex = 0;

        Quaternion rotation = Quaternion.Euler(0f, 0f, rotationDegrees);

        // Create vertices for both sides
        for (int i = 0; i <= segments; i++)
        {
            float angle = angleStep * i;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            Vector3 baseVertex = new Vector3(x, y, -thickness / 2);
            vertices[vertexIndex] = rotation * baseVertex;
            baseVertex.z = thickness / 2;
            vertices[vertexIndex + 1] = rotation * baseVertex;

            // Duplicate vertices for the other side
            vertices[vertexIndex + segments * 2 + 2] = vertices[vertexIndex];
            vertices[vertexIndex + segments * 2 + 3] = vertices[vertexIndex + 1];

            if (i < segments)
            {
                // Triangles for one side
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 2;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + 2;
                triangles[triangleIndex + 5] = vertexIndex + 3;

                // Triangles for the other side, reverse order
                int offset = segments * 2 + 2;
                triangles[triangleIndex + 6] = vertexIndex + 2 + offset;
                triangles[triangleIndex + 7] = vertexIndex + offset;
                triangles[triangleIndex + 8] = vertexIndex + 1 + offset;

                triangles[triangleIndex + 9] = vertexIndex + 3 + offset;
                triangles[triangleIndex + 10] = vertexIndex + 2 + offset;
                triangles[triangleIndex + 11] = vertexIndex + 1 + offset;

                vertexIndex += 2;
                triangleIndex += 12; // Increment by 12 as we're adding two sets of triangles
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
