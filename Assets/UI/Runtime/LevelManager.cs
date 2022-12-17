using System.Collections.Generic;
using TMPro;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // UI
    [SerializeField] GameObject victoryUI;
    [SerializeField] GameObject editingUI;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject resetUI;

    // Turn On / Off Button
    [SerializeField] List<Sprite> isOn;
    [SerializeField] List<Sprite> isOff;
    public bool swipeAllowed;

    // Scripts
    [SerializeField] ResponsiveCamera cameraManager;
    [SerializeField] GameManager gameManager;

    // gameOver text
    [SerializeField] TextMeshProUGUI gameOverText;

    // Button
    public List<Button> gameButtons; // Play, cancel, reset (abort), activate swipe
    public bool gameOnGoing;


    private void Awake()
    {
        loadLevel();
    }

    private void Start()
    {
        GameManager GM = FindObjectOfType<GameManager>();
        GM.OnGameEnded += GM_OnGameEnded;
        //GM.OnGameStarted 

        foreach (Button button in gameButtons)
        {
            if (button.name.Contains("Play"))
            {
                button.onClick.AddListener(playGame);
            }
            else if (button.name.Contains("Cancel"))
            {
                button.onClick.AddListener(cancelMovement);
            }
            else if (button.name.Contains("Reset") || (button.name.Contains("No")) || (button.name.Contains("Redo")))
            {
                button.onClick.AddListener(resetScreenOnOff);
            }
            else if (button.name.Contains("Activate"))
            {
                button.onClick.AddListener(() => allowSwipe(gameButtons.IndexOf(button)));
                button.GetComponent<Image>().sprite = isOff[0];
                swipeAllowed = false;
            }
            else if (button.name.Contains("Edit") || button.name.Contains("Close"))
            {
                button.onClick.AddListener(editingScreen);
            }
            else if (button.name.Contains("Yes") || button.name.Contains("Retry"))
            {
                button.onClick.AddListener(resetLevel);
            }
            else if (button.name.Contains("Continue"))
            {
                button.onClick.AddListener(nextLevel);
            }
            else
            {
                Debug.Log("Cannot recognize button");
            }
        }

        gameOnGoing = false;
    }

    private void Update()
    {
        
    }

    #region GameEnded

    private void GM_OnGameEnded(GameManager.EndGameCondition endCondition)
    {
        Debug.Log("Game Ended");
        switch (endCondition)
        {
            case GameManager.EndGameCondition.playerDeath:
                gameOver(endCondition);
                break;
            case GameManager.EndGameCondition.playerWin:
                victory();
                break;
            case GameManager.EndGameCondition.playerMissedGoal:
                gameOver(endCondition);
                break;
            case GameManager.EndGameCondition.levelAbort:
                break;
            default:
                Debug.Log("Game Ended condition not found");
                break;
        }
        //throw new System.NotImplementedException();
    }

    private void victory()
    {
        Debug.Log("Victory");
        victoryUI.SetActive(!victoryUI.activeSelf);
    }
    private void gameOver(GameManager.EndGameCondition endCondition)
    {
        Debug.Log("Game Over");
        if (endCondition == GameManager.EndGameCondition.playerMissedGoal){
            gameOverText.text = "You missed the goal ! Better luck next time";
        }
        else{
            gameOverText.text = "Are you alive ? The calculation were a bit odd";
        }
        gameOverUI.SetActive(true);
    }

    #endregion

    #region Loading & Resetting Level
    private void loadLevel()
    {
        Debug.Log("Loading Level : Adapting Camera");
        SpaceTerrain terrain = FindObjectOfType<SpaceTerrain>();
        Bounds terrainBounds = HexCoordinatesUtilities.GetBoundingBox(terrain.TerrainShape, terrain.CellSize);
        float extent = Mathf.Max(terrainBounds.extents.x, terrainBounds.extents.y);
        terrainBounds.extents = new Vector3(extent, extent, 0.0f);
        terrainBounds.Expand(0.0f);
        cameraManager.AdaptCameraToTerrain(terrainBounds, 0.8f);
    }

    private void resetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void nextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if(nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }

    #endregion

    private void editingScreen(){
        if (gameOnGoing)
        {
            Debug.Log("Tried to edit movements but game is on going");
        }
        else
        {
            editingUI.SetActive(!editingUI.activeSelf);
        }     
    }

    private void playGame(){

        Debug.Log("Launching Game");
        InputManager inputManager = FindObjectOfType<InputManager>();
        UserInput userInput = FindObjectOfType<UserInput>();

        // Remove asteroid remaining actions
        int turnRemaning = inputManager.TurnNumber - userInput.playerInputs.Count;
        for (int i = 0; i < turnRemaning; i++)
        {
            Debug.Log("Removing Asteroid action");
            inputManager.asteroidsActions.RemoveAt(inputManager.asteroidsActions.Count - 1); // TBD Morgan : Hold action for Asteroid
        }

        while (userInput.playerInputs.Count < inputManager.TurnNumber){
            userInput.playerInputs.Add(SpaceObject.Action.Hold);
        }
        inputManager.playerActions = userInput.playerInputs;

        gameOnGoing = true;
        gameManager.PlayGameAsync();
    }

    private void resetScreenOnOff()
    {
        resetUI.SetActive(!resetUI.activeSelf);
    }

    private void resetMovements(){
        // Reset playerInputsList + Text
        if (gameOnGoing)
        {
            Debug.Log("TBD");
        }
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
    }

    private void changeSpriteState(List<Sprite> sprites, Button button)
    {
        SpriteState myState;
        myState.pressedSprite = sprites[1];
        button.GetComponent<Image>().sprite = sprites[0];
        button.spriteState = myState;
    }

    private void cancelMovement(){
        UserInput userInput = FindObjectOfType<UserInput>();
        if(gameOnGoing)
        {
            Debug.Log("Tried to change inputs but game is on going");
        }
        else
        {
            if (userInput.playerInputs.Count > 0)
            {
                userInput.playerInputs.RemoveAt(userInput.playerInputs.Count - 1);
                userInput.removeText();
            }
        }
        
    }
}
