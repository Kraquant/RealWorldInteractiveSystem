using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerControl controls;
    [SerializeField] GameManager gameManager;
    [SerializeField] ResponsiveCamera cameraManager;
    

    private void Awake()
    {
        controls = new PlayerControl();
    }

    private void Start()
    {
        controls.SpaceshipControl.PlayTurn.performed += ctx => PlayGame();
        controls.SpaceshipControl.AdaptCamera.performed += ctx => AdaptCam();
    }

    private void PlayGame()
    {
        gameManager.PlayGameAsync();
    }
    private void AdaptCam()
    {
        Debug.Log("Adapting camera");
        SpaceTerrain terrain = GetComponentInChildren<SpaceTerrain>();
        Bounds terrainBounds = HexCoordinatesUtilities.GetBoundingBox(terrain.TerrainShape, terrain.CellSize);
        terrainBounds.Expand(0.0f);
        cameraManager.AdaptCameraToTerrain(terrainBounds, 0.5f);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
