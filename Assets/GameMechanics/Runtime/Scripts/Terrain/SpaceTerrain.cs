using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTerrain : MonoBehaviour
{
    SerializableHashSet<HexCoordinates> _terrainShape;
    [SerializeField] float _cellSize; // In meters

    public HashSet<HexCoordinates> TerrainShape
    {
        get => _terrainShape.HashSet;
        set
        {
            if (Application.isPlaying) throw new System.Exception("Can't edit terrain at runtime");
            _terrainShape.HashSet = value;
        }
    }
    public int Size { get => _terrainShape == null ? 0 : _terrainShape.Count; }

    public float CellSize { get => _cellSize; set => _cellSize = value; }

    private void OnDrawGizmosSelected()
    {
        if (_terrainShape == null) return;
        //Preview terrain
        HexCoordinatesUtilities.GizmosDrawHexCoordinates(_terrainShape.HashSet, CellSize);
    }
}
