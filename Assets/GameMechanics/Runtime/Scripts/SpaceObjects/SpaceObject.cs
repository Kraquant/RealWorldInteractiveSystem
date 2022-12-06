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
        LeftBehind,
        RightBehind,
        Hold,
    }

    #region Attributes
    private HashSet<HexCoordinates> _shape;
    [SerializeField] HexCoordinates _center;
    [SerializeField] Orientation _orientation;
    #endregion

    #region Properties
    public HashSet<HexCoordinates> Shape
    {
        get
        {
            if (_shape is null) return new HashSet<HexCoordinates> { HexCoordinates.zero};
            if (_shape.Count == 0) return new HashSet<HexCoordinates> { HexCoordinates.zero };
            return _shape;
        }
    }
    public HexCoordinates Center { get => _center; set => _center = value; }
    public Orientation ObjectOrientation { get => _orientation; set => _orientation = value; }
    #endregion


    public void MoveCoordinate(Action direction)
    {
        Tuple<HexCoordinates, Orientation> newCoords = PreviewNextCoordinate(direction);
        Center = newCoords.Item1;
        ObjectOrientation = newCoords.Item2;
    }

    public Tuple<HexCoordinates, Orientation> PreviewNextCoordinate(Action direction)
    {
        HexCoordinates coord;
        Orientation orient;
        switch (direction)
        {
            case Action.Front:
                orient = ObjectOrientation;
                coord = Center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.Left:
                orient = (Orientation)(((int)ObjectOrientation + 1 + 6) % 6);
                coord = Center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.Right:
                orient = (Orientation)(((int)ObjectOrientation - 1 + 6) % 6);
                coord = Center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.Turn:
                orient = (Orientation)(((int)ObjectOrientation + 3) % 6);
                coord = Center;
                break;
            case Action.LeftBehind:
                orient = (Orientation)(((int)ObjectOrientation + 2 + 6) % 6);
                coord = Center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.RightBehind:
                orient = (Orientation)(((int)ObjectOrientation - 2 + 6) % 6);
                coord = Center + HexCoordinates.direction_vectors[(int)orient];
                break;
            case Action.Hold:
                orient = ObjectOrientation;
                coord = Center;
                break;
            default:
                throw new System.Exception("Direction cannot be null");
        }

        return new Tuple<HexCoordinates, Orientation>(coord, orient);
    }

    public void UpdateSpaceObjectTransform(float cellSize)
    {
        Vector3 lookToVec = HexCoordinates.direction_vectors[(int)ObjectOrientation].GetVector3Position();

        transform.position = Center.GetVector3Position()*cellSize;
        transform.right = lookToVec;
    }

    public static Quaternion OrientationToQuaternion(Orientation orient)
    {
        float angle = orient switch
        {
            Orientation.E => 0,
            Orientation.NE => 60,
            Orientation.NW => 120,
            Orientation.W => 180,
            Orientation.SW => 240,
            Orientation.SE => 300,
            _ => throw new NotImplementedException(),
        };
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
