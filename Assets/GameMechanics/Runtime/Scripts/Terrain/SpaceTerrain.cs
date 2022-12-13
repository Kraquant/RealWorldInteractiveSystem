using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SpaceTerrain : MonoBehaviour
{
    [SerializeField] List<HexCoordinates> _terrainShape;
    [SerializeField] float _cellSize; // In meters
    private HashSet<HexCoordinates> _terrainShapeHS;
    private bool _HSCreated = false;

    public HashSet<HexCoordinates> TerrainShape
    {
        get
        {
            return _HSCreated ? _terrainShapeHS : new HashSet<HexCoordinates>(_terrainShape);
        }

    }
    private void Start()
    {
        //Create the HashSet
        _terrainShapeHS = new HashSet<HexCoordinates>(_terrainShape);
        _HSCreated = true;
    }

    public int Size { get => _terrainShape == null ? 0 : _terrainShape.Count; }

    public float CellSize { get => _cellSize;}

    private void OnDrawGizmosSelected()
    {
        if (_terrainShape == null) return;
        //Preview terrain
        HexCoordinatesUtilities.GizmosDrawHexCoordinates(_terrainShape, CellSize);
        DrawBounds(HexCoordinatesUtilities.GetBoundingBox(_terrainShape, CellSize));
    }

    public void SetTerrain(HashSet<HexCoordinates> terrain, float cellsize)
    {
        if (Application.isPlaying) throw new System.Exception("Can't edit terrain at runtime");
        if (cellsize <= 0) throw new System.Exception("Invalid cell size");
        _terrainShape = new List<HexCoordinates>(terrain);

        _cellSize = cellsize;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }
}
