using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.TerrainTools;
using UnityEngine;

[RequireComponent(typeof(HexShapeCreator))]
[ExecuteInEditMode]
public class LevelCreator : MonoBehaviour
{
    public enum PlacingState
    {
        None,
        Placing,
        Orienting,
        Deleting
    }

    #region Attributes
    //Private attributes
    private HexCoordinates _placementCoord;
    private SpaceObject.Orientation _placementOrientation;
    private Vector3 _vectorTarget;
    private HexCoordinates _hexTarget;
    [SerializeField] HashSet<HexCoordinates> _terrainShape;
    [SerializeField] float _cellSize = 1.0f;
    [SerializeField] bool _vizualizeTerrain = true;

    // Public attributes
    public List<SpaceObjectList> objectsList;
    public List<GameObject> placedObjects;
    public PlacingState placingState = PlacingState.None;
    #endregion

    #region Properties
    public Vector3 VectorTarget
    {
        get => _vectorTarget;
        set
        {
            _vectorTarget = value;
            _hexTarget = HexCoordinates.VectorToPointyHex(_vectorTarget, _cellSize);
        }

    }
    public float CellSize
    {
        get => _cellSize;
        set
        {
            if (value == 0) throw new System.Exception("Cellsize value cannot be null");
            _cellSize = value;
            _hexTarget = HexCoordinates.VectorToPointyHex(VectorTarget, _cellSize);
        }

    }
    public HexCoordinates HexTarget { get => _hexTarget; }
    #endregion


    private void OnDrawGizmosSelected()
    {
        if (_vizualizeTerrain && _terrainShape != null)
        {
            Gizmos.color = Color.magenta;
            foreach (HexCoordinates cell in _terrainShape)
            {
                DrawGizmosHexagon(2 * CellSize * cell.GetVector3Position(), CellSize);
            }
            Gizmos.color = Color.white;
        }

        switch (placingState)
        {
            case PlacingState.None:
                break;
            case PlacingState.Placing:
                
                Color hexColor = CanBePlaced(HexTarget) ? Color.green : Color.red;
                DrawCurrentTarget(hexColor);
                
                break;
            case PlacingState.Orienting:

                Gizmos.color = Color.green;
                Gizmos.DrawLine(2 *CellSize * _placementCoord.GetVector3Position(), VectorTarget);

                DrawOrientationTriangle(
                    2 * CellSize * _placementCoord.GetVector3Position(),
                    2 * CellSize,
                    GetTargetOrientation());

                break;
            case PlacingState.Deleting:
                break;
            default:
                break;
        }
    }

    
    private void DrawCurrentTarget(Color color)
    {
        Color currentColor = Gizmos.color;
        Gizmos.color = color;
        if (HexTarget != null)
        {
            Vector3 hexCenter = 2 * CellSize * HexTarget.GetVector3Position();
            DrawGizmosHexagon(hexCenter, CellSize);
        }
        Gizmos.color = currentColor;
    }
    public void GetTerrainFromCreator()
    {
        HexShapeCreator creator = GetComponent<HexShapeCreator>();
        _terrainShape = new HashSet<HexCoordinates>(creator.HexShape);
        CellSize = creator.CellSize;
        creator.isActive = false;
    }
    public int CurrentTerrainSize()
    {
        if (_terrainShape == null) return 0;
        return _terrainShape.Count;
    }
    public bool CanBePlaced(HexCoordinates coord)
    {
        if (_terrainShape == null) return false;
        if (!_terrainShape.Contains(coord)) return false;
        foreach (GameObject spaceGO in placedObjects)
        {
            SpaceObject spaceObjectScript = spaceGO.GetComponent<SpaceObject>();
            foreach (HexCoordinates shapeCoord in spaceObjectScript.Shape)
            {
                if (shapeCoord + spaceObjectScript.Center == coord) return false;
            }
        }
        return true;
    }
    public bool CanBePlaced(HexCoordinates[] coords)
    {
        foreach (HexCoordinates coord in coords)
        {
            if (!CanBePlaced(coord)) return false;
        }
        return true;
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
    private void DrawOrientationTriangle(Vector3 center, float radius, SpaceObject.Orientation orientation)
    {
        float cosp6 = Mathf.Sqrt(3) / 2;
        float sinp6 = 0.5f;

        Vector3 P0, P1, P2;
        P0 = center;

        switch (orientation)
        {
            case SpaceObject.Orientation.E:
                P1 = new Vector3(cosp6, -sinp6, 0) * radius + center;
                P2 = new Vector3(cosp6, sinp6, 0) * radius + center;
                break;
            case SpaceObject.Orientation.NE:
                P1 = new Vector3(cosp6, sinp6, 0) * radius + center;
                P2 = new Vector3(0.0f, 1.0f, 0) * radius + center;
                break;
            case SpaceObject.Orientation.NW:
                P1 = new Vector3(0.0f, 1.0f, 0) * radius + center;
                P2 = new Vector3(-cosp6, sinp6, 0) * radius + center;
                break;
            case SpaceObject.Orientation.W:
                P1 = new Vector3(-cosp6, sinp6, 0) * radius + center;
                P2 = new Vector3(-cosp6, -sinp6, 0) * radius + center;
                break;
            case SpaceObject.Orientation.SW:
                P1 = new Vector3(-cosp6, -sinp6, 0) * radius + center;
                P2 = new Vector3(0, -1.0f, 0) * radius + center;
                break;
            case SpaceObject.Orientation.SE:
                P1 = new Vector3(0, -1.0f, 0) * radius + center;
                P2 = new Vector3(cosp6, -sinp6, 0) * radius + center;
                break;
            default:
                throw new System.NotImplementedException();

        }
        Gizmos.DrawLine(P0, P1);
        Gizmos.DrawLine(P1, P2);
        Gizmos.DrawLine(P2, P0);
    }

    private SpaceObject.Orientation GetTargetOrientation()
    {
        Vector3 inputAxis = VectorTarget - 2 * CellSize *_placementCoord.GetVector3Position();

        float angle = Vector3.SignedAngle(Vector3.right, inputAxis, Vector3.forward);

        if (angle > 0)
        {
            if (angle < 30) return SpaceObject.Orientation.E;
            if (angle < 90) return SpaceObject.Orientation.NE;
            if (angle < 150) return SpaceObject.Orientation.NW;
            if (angle <= 180) return SpaceObject.Orientation.W;
        }
        else
        {
            if (angle > -30) return SpaceObject.Orientation.E;
            if (angle > -90) return SpaceObject.Orientation.SE;
            if (angle > -150) return SpaceObject.Orientation.SW;
            if (angle >= -180) return SpaceObject.Orientation.W;
        }

        throw new System.NotImplementedException();
    }

    #region Object placing methods

    public void PlaceObject()
    {
        if (placingState != PlacingState.None) throw new System.Exception("Can't enter placement state: you are currently in another state");
        placingState = PlacingState.Placing;
    }

    /// <summary>
    /// Transition between placement state and orientation state
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void ConfirmPlacement()
    {
        if (placingState != PlacingState.Placing) throw new System.Exception("Can't enter placement: you are currently placing anything");

        if (CanBePlaced(HexTarget))
        {
            placingState = PlacingState.Orienting;
            _placementCoord = HexTarget;
            _placementOrientation = SpaceObject.Orientation.E;
        }
        else
        {
            Debug.Log("You can't place anything here");
        }
    }
    public void ConfirmOrientation()
    {
        if (placingState != PlacingState.Orienting)
            throw new System.Exception("Can't enter placement: you are not currently orienting anything");
        _placementOrientation = GetTargetOrientation();
        placingState = PlacingState.None;
        Debug.Log("Space object added");
    }
    public void CancelPlacement()
    {
        if (placingState != PlacingState.Placing &&
            placingState != PlacingState.Orienting) 
            throw new System.Exception("Can't cancel placement: you are not currently placing or orienting anything");
        
        placingState = PlacingState.None;
    }
    #endregion
}
