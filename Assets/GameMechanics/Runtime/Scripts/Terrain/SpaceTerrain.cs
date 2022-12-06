using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTerrain : MonoBehaviour
{
    SerializableHashSet<HexCoordinates> _terrainShape2;
    HashSet<HexCoordinates> _terrainShape;
    [SerializeField] float _cellSize; // In meters

    public HashSet<HexCoordinates> TerrainShape
    {
        get => _terrainShape;
        set
        {
            if (Application.isPlaying) throw new System.Exception("Can't edit terrain at runtime");
            _terrainShape = value;
        }
    }
    public int Size { get => _terrainShape.Count; }

    public float CellSize { get => _cellSize; set => _cellSize = value; }

    private void OnDrawGizmosSelected()
    {
        if (_terrainShape == null) return;
        //Preview terrain
        HexCoordinatesUtilities.GizmosDrawHexCoordinates(_terrainShape, CellSize);
    }
}
