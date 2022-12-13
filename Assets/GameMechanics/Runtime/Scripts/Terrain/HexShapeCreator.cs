using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<HexCoordinates> cellList = new List<HexCoordinates>();
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
    public HashSet<HexCoordinates> HexShape { get => new HashSet<HexCoordinates>(cellList); }
    #endregion

    private void Awake()
    {
        if (CellSize == 0) CellSize = 1;
        VectorTarget = Vector3.zero;
        isActive = true;
        //cellList ??= new List<HexCoordinates>();
        addList ??= new HashSet<HexCoordinates>();
    }

    private void OnDrawGizmosSelected()
    {
        if (!isActive) return;
        addList ??= new HashSet<HexCoordinates>();

        //Draw current terrain
        Gizmos.color = Color.white;
        HexCoordinatesUtilities.GizmosDrawHexCoordinates(HexShape, CellSize);

        //Draw cell currently added
        Gizmos.color = !addStatus ? Color.red : Color.green;
        HexCoordinatesUtilities.GizmosDrawHexCoordinates(addList, CellSize);

        // Draw current target
        if (VectorTarget != null)
        {
            Gizmos.color = HexShape.Contains(HexTarget) ? Color.red : Color.green;
            HexCoordinatesUtilities.GizmosDrawHexCoordinates(HexTarget, CellSize);
        }

        SceneView.RepaintAll();
    }

    public void AddNewCells()
    {
        HashSet<HexCoordinates> hashSet = new HashSet<HexCoordinates>(cellList);
        foreach (HexCoordinates cell in addList)
        {
            if (addStatus)
            {
                hashSet.Add(cell);
            }
            else
            {
                hashSet.Remove(cell);
            }
        }

        cellList = hashSet.ToList();

        addList.Clear();
    }
}
