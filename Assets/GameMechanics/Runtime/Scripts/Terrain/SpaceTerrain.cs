using System.Collections.Generic;
using UnityEngine;

public class SpaceTerrain : MonoBehaviour
{
    [SerializeField] HashSet<HexCoordinates> _terrainShape;
    [SerializeField] float _cellSize; // In meters
    [SerializeField] int radius;

    public HashSet<HexCoordinates> TerrainShape
    {
        get => _terrainShape;
        set
        {
            if (Application.isPlaying) throw new System.Exception("Can't edit terrain at runtime");
            _terrainShape = value;
        }
    }
    public float CellSize { get => _cellSize; }

    // Start is called before the first frame update
    void Start()
    {
        //Creating a basic terrain
        _terrainShape = new HashSet<HexCoordinates>();
        List<HexCoordinates> terrainList = new List<HexCoordinates>();
        terrainList.AddRange(HexCoordinates.GetHexCircle(0));
        terrainList.AddRange(HexCoordinates.GetHexCircle(1));
        terrainList.AddRange(HexCoordinates.GetHexCircle(2));
        terrainList.AddRange(HexCoordinates.GetHexCircle(3));

        foreach (HexCoordinates coord in terrainList) _terrainShape.Add(coord);

    }

    private void OnDrawGizmosSelected()
    {
        if (_terrainShape == null) return;
        PreviewTerrain();
    }

    private void PreviewTerrain()
    {
        foreach (HexCoordinates coord in _terrainShape)
        {
            DrawGizmosHexagon(coord.GetVector3Position() * _cellSize, _cellSize / 2.0f);
        }
    }

    private void DrawGizmosHexagon(Vector3 center, float radius)
    {
        float cosp6 = Mathf.Sqrt(3) / 2;
        float sinp6 = 0.5f;

        Vector3 P0 = new Vector3(cosp6, sinp6, 0) * radius + center;
        Vector3 P1 = new Vector3(0.0f, 1.0f, 0) * radius + center;
        Vector3 P2 = new Vector3(-cosp6, sinp6, 0) * radius + center;
        Vector3 P3 = new Vector3(-cosp6, -sinp6, 0) * radius + center;
        Vector3 P4 = new Vector3(0, -1.0f, 0) * radius + center;
        Vector3 P5 = new Vector3(cosp6, -sinp6, 0) * radius + center;

        Gizmos.DrawLine(P0, P1);
        Gizmos.DrawLine(P1, P2);
        Gizmos.DrawLine(P2, P3);
        Gizmos.DrawLine(P3, P4);
        Gizmos.DrawLine(P4, P5);
        Gizmos.DrawLine(P5, P0);
    }
}
