using UnityEngine;

namespace __OasisBlitz.Utility
{
    public static class MeshInverter
    {
        // public void Start()
        // {
        //     MeshFilter meshFilter = GetComponent<MeshFilter>();
        //     if (meshFilter != null)
        //     {
        //         meshFilter.mesh = InvertMesh(meshFilter.mesh);
        //     }
        // }

        public static Mesh InvertMesh(Mesh originalMesh)
        {
            Mesh invertedMesh = new Mesh();

            // Copy vertices, normals, uv, etc.
            invertedMesh.vertices = originalMesh.vertices;
            invertedMesh.normals = originalMesh.normals;
            invertedMesh.uv = originalMesh.uv;
            invertedMesh.colors = originalMesh.colors;

            // Invert triangle winding order
            for (int subMesh = 0; subMesh < originalMesh.subMeshCount; subMesh++)
            {
                int[] triangles = originalMesh.GetTriangles(subMesh);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                invertedMesh.SetTriangles(triangles, subMesh);
            }

            // Recalculate bounds and normals (if needed)
            invertedMesh.RecalculateBounds();
            invertedMesh.RecalculateNormals();

            return invertedMesh;
        }
    }
}
