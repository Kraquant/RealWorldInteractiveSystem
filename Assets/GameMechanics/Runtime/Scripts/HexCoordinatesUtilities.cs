using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class HexCoordinatesUtilities
{
    public static void GizmosDrawHexCoordinates(IEnumerable<HexCoordinates> hexCoordList, float cellSize)
    {
        foreach (HexCoordinates coord in hexCoordList)
        {
            GizmosDrawHexCoordinates(coord, cellSize);
        }
    }
    public static void GizmosDrawHexCoordinates(HexCoordinates coord, float cellSize = 1)
    {
        Vector3 hexCenter = 2 * cellSize * coord.GetVector3Position();
        DrawGizmosHexagon(hexCenter, cellSize);
    }

    public static void DrawGizmosHexagon(Vector3 center, float radius)
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
}
