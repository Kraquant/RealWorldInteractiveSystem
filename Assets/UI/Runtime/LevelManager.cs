using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // UI
    [SerializeField] GameObject victoryUI;
    [SerializeField] GameObject editingUI;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject swipeField;

    // Scripts
    [SerializeField] ResponsiveCamera cameraManager;
    [SerializeField] GameManager gameManager;

    // Button
    public List<Button> gameButtons; // Play, cancel, reset (abort), activate swipe

    //private bool gameOnGoing = false;


    private void Awake()
    {
        loadLevel();
    }

    private void Update()
    {
        foreach (Button button in gameButtons)
        {
            if(button.name.Contains("Play")){
                button.onClick.AddListener(playGame);
            }
            else if(button.name.Contains("Cancel")){
                button.onClick.AddListener(cancelMovement);
            }
            else if (button.name.Contains("Reset")){
                button.onClick.AddListener(resetMovements);
            }
            else if (button.name.Contains("Activate")){
                button.onClick.AddListener(allowSwipe);
            }
            else if (button.name.Contains("Edit") || button.name.Contains("Close"))
            {
                button.onClick.AddListener(editingScreen);
            }
            else
            {
                Debug.Log("Cannot recognize button");
            }
        }
    }


    private void loadLevel()
    {
        Debug.Log("Loading Level : Adapting Camera");
        SpaceTerrain terrain = FindObjectOfType<SpaceTerrain>();
        Bounds terrainBounds = HexCoordinatesUtilities.GetBoundingBox(terrain.TerrainShape, terrain.CellSize);
        //float extent = Mathf.Max(terrainBounds.extents.x, terrainBounds.extents.y);
        //terrainBounds.extents = new Vector3(extent, extent, 0.0f);
        terrainBounds.Expand(0.0f);
        cameraManager.AdaptCameraToTerrain(terrainBounds, 0.8f);
    }

    // Level related functions
    private void editingScreen(){
        editingUI.SetActive(!editingUI.activeSelf);
    }
    private void victoryScreen(){
        victoryUI.SetActive(!victoryUI.activeSelf);
    }
    private void gameOver()
    {
        gameOverUI.SetActive(true);
    }
    private void playGame(){
        Debug.Log("Launching Game");
        InputManager inputManager = FindObjectOfType<InputManager>();
        UserInput userInput = FindObjectOfType<UserInput>();
        inputManager.playerActions = userInput.playerInputs;

        gameManager.PlayGameAsync();
    }

    private void resetMovements(){

    }
    private void allowSwipe()
    {

    }
    private void cancelMovement()
    {

    }

}
