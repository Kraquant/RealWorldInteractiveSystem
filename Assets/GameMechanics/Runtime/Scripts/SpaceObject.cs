using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    public enum Orientation
    {
        E,
        NE,
        NW,
        W,
        SW,
        SE
    }
    public enum Action
    {
        Front,
        Left,
        Right,
        Turn,
        Hold,
    }

    #region Private Fields
    [SerializeField] HexCoordinates[] _shape;
    private HexCoordinates _center;
    private Orientation _objectOrientation;
    #endregion

    #region Properties
    public HexCoordinates[] Shape { get => _shape; }
    public HexCoordinates Center { get => _center; }
    public Orientation ObjectOrientation { get => _objectOrientation; }
    #endregion

    protected void Awake()
    {
        _center = new HexCoordinates();
        _objectOrientation = Orientation.E;
    }

    public void MoveCoordinate(Action direction)
    {
        Tuple<HexCoordinates, Orientation> newCoords = PreviewNextCoordinate(direction);
        _center = newCoords.Item1;
        _objectOrientation = newCoords.Item2;
    }

    public Tuple<HexCoordinates, Orientation> PreviewNextCoordinate(Action direction)
    {
        HexCoordinates coord;
        Orientation orient;
        switch (direction)
        {
            case Action.Front:
                orient = _objectOrientation;
                coord = _center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.Left:
                orient = (Orientation)(((int)_objectOrientation + 1 + 6) % 6);
                coord = _center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.Right:
                orient = (Orientation)(((int)_objectOrientation - 1 + 6) % 6);
                coord = _center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.Turn:
                orient = (Orientation)(((int)_objectOrientation + 3) % 6);
                coord = _center;
                break;
            case Action.Hold:
                orient = _objectOrientation;
                coord = _center;
                break;
            default:
                throw new System.Exception("Direction cannot be null");
        }

        return new Tuple<HexCoordinates, Orientation>(coord, orient);
    }

    public void UpdateSpaceObjectTransform(float cellSize)
    {
        Vector3 lookToVec = HexCoordinates.direction_vectors[(int)_objectOrientation].GetVector3Position();

        transform.position = _center.GetVector3Position()*cellSize;
        transform.right = lookToVec;
    }
}
