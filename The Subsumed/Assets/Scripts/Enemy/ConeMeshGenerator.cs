using UnityEngine;

public static class ConeMeshGenerator
{
    public static Mesh CreateCone(float height, float radius, int segments)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3 * 2];

        // Apex
        vertices[0] = Vector3.zero;

        // Base circle
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i + 1] = new Vector3(x, -height, z);
        }

        // Center point at base
        vertices[segments + 1] = new Vector3(0, -height, 0);

        // Triangles - Sides
        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segments + 1;

            // Side
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = next;
            triangles[i * 3 + 2] = current;
        }

        // Triangles - Base
        int offset = segments * 3;
        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segments + 1;

            triangles[offset + i * 3] = segments + 1;
            triangles[offset + i * 3 + 1] = current;
            triangles[offset + i * 3 + 2] = next;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}