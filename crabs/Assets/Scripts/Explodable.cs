// adapted from: https://github.com/unitycoder/SimpleMeshExploder/blob/master/Assets/Scripts/SimpleMeshExploder.cs
using UnityEngine;

public class Explodable : MonoBehaviour
{
    [SerializeField]
    private Material PieceMaterial = null; // must be transparent for fader

    public void Explode(Transform target)
    {
        Color color = target.GetComponent<MeshRenderer>().material.color;
        Mesh mesh = target.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;
        int index = 0;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 averageNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]).normalized;
            Vector3 size = target.GetComponent<Renderer>().bounds.size;
            float extrudeSize = ((size.x + size.y + size.z) / 3) * 0.3f;
            float scale = transform.lossyScale.x; // assume x scale is representitive
            CreateMeshPiece(extrudeSize, target.transform.position, color, index, averageNormal, vertices[triangles[i]] * scale, vertices[triangles[i + 1]] * scale, vertices[triangles[i + 2]] * scale, uvs[triangles[i]] * scale, uvs[triangles[i + 1]] * scale, uvs[triangles[i + 2]] * scale);
            index++;
        }

        Destroy(target.gameObject);
    }

    private void CreateMeshPiece(float extrudeSize, Vector3 position, Color color, int index, Vector3 faceNormal, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        GameObject piece = new GameObject("piece_" + index);

        Mesh mesh = piece.AddComponent<MeshFilter>().mesh;
        piece.AddComponent<MeshRenderer>();
        Renderer renderer = piece.GetComponent<Renderer>();
        renderer.material = PieceMaterial;
        renderer.material.color = color;

        piece.transform.position = position;

        Vector3[] vertices = new Vector3[3 * 4];
        int[] triangles = new int[3 * 4];
        Vector2[] uvs = new Vector2[3 * 4];

        // get centroid
        Vector3 v4 = (v1 + v2 + v3) / 3;
        // extend to backwards
        v4 += (-faceNormal) * extrudeSize;

        // not shared vertices
        // orig face
        //vertices[0] = (v1);
        vertices[0] = (v1);
        vertices[1] = (v2);
        vertices[2] = (v3);
        // right face
        vertices[3] = (v1);
        vertices[4] = (v2);
        vertices[5] = (v4);
        // left face
        vertices[6] = (v1);
        vertices[7] = (v3);
        vertices[8] = (v4);
        // bottom face
        vertices[9] = (v2);
        vertices[10] = (v3);
        vertices[11] = (v4);

        // orig face
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        //  right face
        triangles[3] = 5;
        triangles[4] = 4;
        triangles[5] = 3;
        //  left face
        triangles[6] = 6;
        triangles[7] = 7;
        triangles[8] = 8;
        //  bottom face
        triangles[9] = 11;
        triangles[10] = 10;
        triangles[11] = 9;

        // orig face
        uvs[0] = uv1;
        uvs[1] = uv2;
        uvs[2] = uv3;
        // right face
        uvs[3] = uv1;
        uvs[4] = uv2;
        uvs[5] = uv3;
        // left face
        uvs[6] = uv1;
        uvs[7] = uv3;
        uvs[8] = uv3;
        // bottom face
        uvs[9] = uv1;
        uvs[10] = uv2;
        uvs[11] = uv1;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        CalculateMeshTangents(mesh);

        piece.AddComponent<Rigidbody>();
        MeshCollider mc = piece.AddComponent<MeshCollider>();

        mc.sharedMesh = mesh;
        mc.convex = true;

        piece.AddComponent<MeshFader>();
    }

    // source: http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
    private void CalculateMeshTangents(Mesh mesh)
    {
        //speed up math by copying the mesh arrays
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;

        //variable definitions
        int triangleCount = triangles.Length;
        int vertexCount = vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3)
        {
            long i1 = triangles[a + 0];
            long i2 = triangles[a + 1];
            long i3 = triangles[a + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }

        for (int a = 0; a < vertexCount; ++a)
        {
            Vector3 n = normals[a];
            Vector3 t = tan1[a];
            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;
            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }
        mesh.tangents = tangents;
    }
}