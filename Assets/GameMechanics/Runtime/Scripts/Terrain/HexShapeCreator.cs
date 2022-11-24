using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class HexShapeCreator : MonoBehaviour
{
    public float cellSize;
    public Vector3 target;
    public bool activateTarget;
    private GameObject targetSphere;

    private void Awake()
    {
        target = Vector3.zero;
        targetSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        targetSphere.transform.parent = this.transform;
        targetSphere.name = "Target Sphere";
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            targetSphere.SetActive(true);
            targetSphere.transform.position = target;
        }
        else
        {
            targetSphere.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Vector3 hexCenter = HexCoordinates.VectorToPointyHex(target, cellSize).GetVector3Position()*cellSize;
        DrawGizmosHexagon(hexCenter, cellSize);
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
}
