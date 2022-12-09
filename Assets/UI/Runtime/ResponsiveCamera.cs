using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

[RequireComponent(typeof(Camera))]
public class ResponsiveCamera : MonoBehaviour
{
    public void AdaptCameraToTerrain(Bounds terrainBounds)
    {
        var (center, size) = AdjustOrthoCamera(terrainBounds);
        Camera cam = GetComponent<Camera>();
        cam.transform.position = center;
        cam.orthographicSize = size;
    }
    private (Vector3 center, float size) AdjustOrthoCamera(Bounds bounds)
    {
        Camera cam = GetComponent<Camera>();
        float vertical = bounds.size.y;
        float horizontal = bounds.size.x * cam.pixelHeight/cam.pixelWidth;
        float size = Mathf.Max(horizontal, vertical) * 0.5f;

        Vector3 center = bounds.center + new Vector3(0, 0, -10);
        return (center,size);
    }
}
