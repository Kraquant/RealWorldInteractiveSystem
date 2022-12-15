using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // UI
    [SerializeField] GameObject victoryUI;
    [SerializeField] GameObject editingUI;
    [SerializeField] GameObject gameOverUI;

    // Turn On / Off Button
    [SerializeField] GameObject swipeField;
    [SerializeField] List<Sprite> isOn;
    [SerializeField] List<Sprite> isOff;
    private bool swipeAllowed = false;

    // Scripts
    [SerializeField] ResponsiveCamera cameraManager;
    [SerializeField] GameManager gameManager;

    // Button
    public List<Button> gameButtons; // Play, cancel, reset (abort), activate swipe


    private void Awake()
    {
        loadLevel();
    }

    private void Start()
    {
        foreach (Button button in gameButtons)
        {
            if (button.name.Contains("Play")){
                button.onClick.AddListener(playGame);
            }
            else if (button.name.Contains("Cancel")){
                button.onClick.AddListener(cancelMovement);
            }
            else if (button.name.Contains("Reset")){
                button.onClick.AddListener(resetMovements);
            }
            else if (button.name.Contains("Activate"))
            {
                button.onClick.AddListener(() => allowSwipe(gameButtons.IndexOf(button)));
                button.GetComponent<Image>().sprite = isOff[0];
                swipeField.SetActive(false);
            }
            else if (button.name.Contains("Edit") || button.name.Contains("Close")){
                button.onClick.AddListener(editingScreen);
            }else{
                Debug.Log("Cannot recognize button");
            }
        }
    }

    private void Update()
    {
        
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
    private void allowSwipe(int buttonNo)
    {
        Button button = gameButtons[buttonNo];
        if (!swipeAllowed){
            changeSpriteState(isOn, button);
        }
        else {
            changeSpriteState(isOff, button);
        }
        swipeAllowed = !swipeAllowed;
        swipeField.SetActive(!swipeField.activeSelf);
    }

    private void changeSpriteState(List<Sprite> sprites, Button button)
    {
        SpriteState myState;
        myState.pressedSprite = sprites[1];
        button.GetComponent<Image>().sprite = sprites[0];
        button.spriteState = myState;
    }

    private void cancelMovement(){
        //UserInput userInput = FindObjectOfType<UserInput>();

        //if(userInput.playerInputs.Count > 0){
        //    Debug.Log("Removing Action");
        //    Debug.Log(userInput.playerInputs.Count);
        //    //userInput.playerInputs.RemoveAt(userInput.playerInputs.Count - 1);
        //    //userInput.removeText();
        //}
    }

}
