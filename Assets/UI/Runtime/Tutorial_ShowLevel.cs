using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_ShowLevel : MonoBehaviour
{
    [SerializeField] ResponsiveCamera cameraManager;
    [SerializeField] GameManager gameManager;
    void OnEnable()
    {
        gameManager ??= FindObjectOfType<GameManager>();
        ConfigureCamera();
    }

    private void ConfigureCamera()
    {
        SpaceTerrain terrain = FindObjectOfType<SpaceTerrain>();
        Bounds terrainBounds = HexCoordinatesUtilities.GetBoundingBox(terrain.TerrainShape, terrain.CellSize);
        float extent = Mathf.Max(terrainBounds.extents.x, terrainBounds.extents.y);
        terrainBounds.extents = new Vector3(extent, extent, 0.0f);
        terrainBounds.Expand(0.0f);
        cameraManager.AdaptCameraToTerrain(terrainBounds, 0.8f);
    }
}
