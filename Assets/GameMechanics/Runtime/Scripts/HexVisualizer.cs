using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexVisualizer : MonoBehaviour
{
    #region Hex Mesh definition
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
    #endregion

    public Material displayMat;
    private Mesh CreateMesh(Vector3 center, float cellSize)
    {
        Mesh hexMesh = new Mesh();

        Vector3[] vert = new Vector3[7];
        for (int i = 0; i < vertices.Length; i++) vert[i] = cellSize * center + vertices[i];

        hexMesh.vertices = vert;
        hexMesh.triangles = triangles;
        hexMesh.uv = UV;
        return hexMesh;
    }

    private void Start()
    {
        CreateMesh(Vector3.zero, 1.0f);
        GetOrAdd<MeshRenderer>().material = displayMat;
    }

    public void CreateShape(HashSet<HexCoordinates> shape, float cellSize)
    {
        if (cellSize != 1.0f) throw new System.NotImplementedException();

        Mesh[] hexMesh = new Mesh[shape.Count];
        int i = 0;
        foreach (HexCoordinates coord in shape)
        {
            hexMesh[i] = CreateMesh(2 * cellSize * coord.GetVector3Position(), cellSize);
            i++;
        }
        CombineMesh(hexMesh.ToArray());
    }

    private void CombineMesh(Mesh[] meshes)
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<GameObject> generatedGameObjects = new List<GameObject>();

        for (int i = 0; i < meshes.Length; i++)
        {
            GameObject childObj = new GameObject();
            childObj.transform.parent = this.gameObject.transform;
            MeshFilter currentMeshFilter = childObj.AddComponent<MeshFilter>();
            currentMeshFilter.mesh = meshes[i];
            meshFilters.Add(currentMeshFilter);
            generatedGameObjects.Add(childObj);

        }

        //Combining Meshes
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        for (int i = 0; i < combine.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        ClearShape();
        GetOrAdd<MeshFilter>().sharedMesh.CombineMeshes(combine);
        foreach (GameObject gameObject in generatedGameObjects) DestroyImmediate(gameObject);
    }

    public void ClearShape() => GetOrAdd<MeshFilter>().mesh = new Mesh();

    private T GetOrAdd<T>() where T : Component
    {
        if (!TryGetComponent<T>(out var component))
            component = gameObject.AddComponent<T>();
        return component;
    }

}
