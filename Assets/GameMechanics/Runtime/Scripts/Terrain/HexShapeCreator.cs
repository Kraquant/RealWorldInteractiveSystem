using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class HexShapeCreator : MonoBehaviour
{
    #region Attributes
    //Private attributes
    private Vector3 _vectorTarget;
    private HexCoordinates _hexTarget;

    // Public attributes
    public HashSet<HexCoordinates> cellList;
    public HashSet<HexCoordinates> addList;
    public bool isActive;
    public bool addStatus; // True for add, false for remove
    public float cellSize; 
    #endregion

    #region Public Properties
    public Vector3 VectorTarget
    {
        get => _vectorTarget;
        set
        {
            _vectorTarget = value;
            _hexTarget = HexCoordinates.VectorToPointyHex(_vectorTarget, cellSize);
        }

    }
    public float CellSize
    {
        get => cellSize;
        set
        {
            if (value == 0) throw new System.Exception("Cellsize value cannot be null");
            cellSize = value;
            _hexTarget = HexCoordinates.VectorToPointyHex(VectorTarget, cellSize);
        }

    }
    public HexCoordinates HexTarget { get => _hexTarget; }
    public HashSet<HexCoordinates> HexShape { get => cellList; }
    #endregion

    private void Awake()
    {
        if (CellSize == 0) CellSize = 1;
        VectorTarget = Vector3.zero;
        isActive = true;
        cellList ??= new HashSet<HexCoordinates>();
        addList ??= new HashSet<HexCoordinates>();
    }

    private void OnDrawGizmosSelected()
    {
        if (!isActive) return;
        cellList ??= new HashSet<HexCoordinates>();
        addList ??= new HashSet<HexCoordinates>();

        //Draw current terrain
        Gizmos.color = Color.white;
        foreach (HexCoordinates cell in cellList)
        {
            DrawGizmosHexagon(2 * CellSize * cell.GetVector3Position(), CellSize);
        }

        //Draw cell currently added
        Gizmos.color = !addStatus ? Color.red : Color.green;
        foreach (HexCoordinates cell in addList)
        {
            DrawGizmosHexagon(2 * CellSize * cell.GetVector3Position(), CellSize);
        }

        // Draw current target
        if (VectorTarget != null)
        {
            Vector3 hexCenter = 2 * CellSize * HexTarget.GetVector3Position();

            Gizmos.color = cellList.Contains(HexTarget) ? Color.red : Color.green;
            DrawGizmosHexagon(hexCenter, CellSize);     
        }

        SceneView.RepaintAll();
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

    public void AddNewCells()
    {
        foreach (HexCoordinates cell in addList)
        {
            if (addStatus)
            {
                cellList.Add(cell);
            }
            else
            {
                cellList.Remove(cell);
            }
        }

        addList.Clear();
    }
}
