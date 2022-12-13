using UnityEngine;

public class HexVisualizer : MonoBehaviour
{
    public static readonly Vector3[] vertices = new Vector3[7]
    {
        Vector3.zero,
        new Vector3(Mathf.Cos(Mathf.PI / 6), Mathf.Sin(Mathf.PI / 6), 0),
        new Vector3(0.0f, 1.0f, 0),
        new Vector3(-Mathf.Cos(Mathf.PI / 6), Mathf.Sin(Mathf.PI / 6), 0),
        new Vector3(-Mathf.Cos(Mathf.PI / 6), -Mathf.Sin(Mathf.PI / 6), 0),
        new Vector3(0, -1.0f, 0),
        new Vector3(Mathf.Cos(Mathf.PI / 6), -Mathf.Sin(Mathf.PI / 6), 0)
    };
    public static readonly int[] triangles = new int[18]
    {
        0, 1, 6,
        0, 2, 1,
        0, 3, 2,
        0, 4, 3,
        0, 5, 4,
        0, 6, 5,
    };
    public static readonly Vector2[] UV = new Vector2[7]
    {
        0.5f * Vector2.one,
        0.5f * (new Vector2(Mathf.Cos(Mathf.PI / 6), Mathf.Sin(Mathf.PI / 6)) + Vector2.one),
        0.5f * (new Vector2(0.0f, 1.0f) + Vector2.one),
        0.5f * (new Vector2(-Mathf.Cos(Mathf.PI / 6), Mathf.Sin(Mathf.PI / 6)) + Vector2.one),
        0.5f * (new Vector2(-Mathf.Cos(Mathf.PI / 6), -Mathf.Sin(Mathf.PI / 6)) + Vector2.one),
        0.5f * (new Vector2(0, -1.0f) + Vector2.one),
        0.5f * (new Vector2(Mathf.Cos(Mathf.PI / 6), -Mathf.Sin(Mathf.PI / 6)) + Vector2.one)
    };


private void CreateMesh(Vector3 center, float cellSize)
    {
        MeshFilter mf = transform.gameObject.AddComponent<MeshFilter>();
        Mesh hexMesh = new Mesh();
        hexMesh.vertices= vertices;
        hexMesh.triangles = triangles;
        hexMesh.uv = UV;
        mf.mesh = hexMesh;
    }

    private void Start()
    {
        transform.gameObject.AddComponent<MeshRenderer>();
        CreateMesh(Vector3.zero, 1.0f);
    }
}
