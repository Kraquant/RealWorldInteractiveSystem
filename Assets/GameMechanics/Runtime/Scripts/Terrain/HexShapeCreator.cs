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
        HexCoordinatesUtilities.GizmosDrawHexCoordinates(cellList, CellSize);

        //Draw cell currently added
        Gizmos.color = !addStatus ? Color.red : Color.green;
        HexCoordinatesUtilities.GizmosDrawHexCoordinates(addList, CellSize);

        // Draw current target
        if (VectorTarget != null)
        {
            Gizmos.color = cellList.Contains(HexTarget) ? Color.red : Color.green;
            HexCoordinatesUtilities.GizmosDrawHexCoordinates(HexTarget, CellSize);
        }

        SceneView.RepaintAll();
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
