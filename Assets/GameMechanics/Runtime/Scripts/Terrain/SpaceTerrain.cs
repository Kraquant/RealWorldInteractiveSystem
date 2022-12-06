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
    }

    public void SetTerrain(HashSet<HexCoordinates> terrain, float cellsize)
    {
        if (Application.isPlaying) throw new System.Exception("Can't edit terrain at runtime");
        if (cellsize <= 0) throw new System.Exception("Invalid cell size");
        _terrainShape = new List<HexCoordinates>(terrain);

        _cellSize = cellsize;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
