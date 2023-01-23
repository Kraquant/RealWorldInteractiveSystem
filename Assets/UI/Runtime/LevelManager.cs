using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // UI
    [SerializeField] GameObject victoryUI;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject resetUI;

    // Turn On / Off Button
    [SerializeField] List<Sprite> isOn;
    [SerializeField] List<Sprite> isOff;
    public bool swipeAllowed;

    // Scripts
    [SerializeField] ResponsiveCamera cameraManager;
    [SerializeField] GameManager gameManager;
    // text
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI asteroidMovesText;

    // Button
    public List<Button> gameButtons; // Play, cancel, reset (abort), activate swipe
    public bool gameOnGoing;


    private void Awake()
    {
        ConfigureCamera();
        victoryUI.SetActive(false);
        gameOverUI.SetActive(false);
        resetUI.SetActive(false);
        gameOnGoing = false;
  
        InputManager inputManager = FindObjectOfType<InputManager>();
        gameManager ??= FindObjectOfType<GameManager>();
        gameManager.OnGameEnded += GM_OnGameEnded;
        ConfigureButtons();
        PutAsteroidMovements(inputManager.asteroidsActions);
    }

    private void PutAsteroidMovements(List<Asteroid.AsteroidAction> asteroidActions)
    {
        asteroidMovesText.text = "<sprite=\"Asteroids\" index=0>:";

        foreach (Asteroid.AsteroidAction action in asteroidActions)
        {
            switch (action)
            {
                case Asteroid.AsteroidAction.O1:
                    asteroidMovesText.text += " <sprite=\"AsteroidMoves\" index=0>| ";
                    break;
                case Asteroid.AsteroidAction.O2:
                    asteroidMovesText.text += " <sprite=\"AsteroidMoves\" index=1>| ";
                    break;
                case Asteroid.AsteroidAction.O3:
                    asteroidMovesText.text += " <sprite=\"AsteroidMoves\" index=2>| ";
                    break;
                case Asteroid.AsteroidAction.O4:
                    asteroidMovesText.text += " <sprite=\"AsteroidMoves\" index=3>| ";
                    break;
                case Asteroid.AsteroidAction.O5:
                    asteroidMovesText.text += " <sprite=\"AsteroidMoves\" index=4>| ";
                    break;
                case Asteroid.AsteroidAction.O6:
                    asteroidMovesText.text += " <sprite=\"AsteroidMoves\" index=5>| ";
                    break;
                default:
                    break;
            }
            
        }
    }
    private void ConfigureButtons()
    {
        foreach (Button button in gameButtons)
        {
            if (button.name.Contains("Play Button"))
            {
                button.onClick.AddListener(PlayGame);
            }
            else if (button.name.Contains("Cancel"))
            {
                button.onClick.AddListener(CancelMovement);
            }
            else if (button.name.Contains("Reset") || (button.name.Contains("No")) || (button.name.Contains("Redo")))
            {
                button.onClick.AddListener(ResetScreenOnOff);
            }
            else if (button.name.Contains("Activate"))
            {
                button.onClick.AddListener(() => AllowSwipe(gameButtons.IndexOf(button)));
                button.GetComponent<Image>().sprite = isOff[0];
                swipeAllowed = false;
            }
            else if (button.name.Contains("Yes") || button.name.Contains("Retry"))
            {
                button.onClick.AddListener(ResetLevel);
            }
            else if (button.name.Contains("Continue"))
            {
                button.onClick.AddListener(NextLevel);
            }
            else if (button.name.Contains("CloseGameOver"))
            {
                button.onClick.AddListener(closeGameOverScreen);
            }
            else
            {
                Debug.Log("Cannot recognize button");
            }
        }
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
        //victoryUI.SetActive(!victoryUI.activeSelf);
    }

    private void closeGameOverScreen()
    {
        Debug.Log("Closing game over screen");
        gameOverUI.SetActive(false);
    }

    #endregion

    #region Loading & Resetting Level
    private void ConfigureCamera()
    {
        //Debug.Log("Loading Level : Adapting Camera");
        SpaceTerrain terrain = FindObjectOfType<SpaceTerrain>();
        Bounds terrainBounds = HexCoordinatesUtilities.GetBoundingBox(terrain.TerrainShape, terrain.CellSize);
        float extent = Mathf.Max(terrainBounds.extents.x, terrainBounds.extents.y);
        terrainBounds.extents = new Vector3(extent, extent, 0.0f);
        terrainBounds.Expand(0.0f);
        cameraManager.AdaptCameraToTerrain(terrainBounds, 0.8f);
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void NextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if(nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Loading next level");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("No next level");
            SceneManager.LoadScene("MainMenu");
        }
    }

    #endregion
    private void PlayGame(){

        Debug.Log("Launching Game");
        UserInput userInput = FindObjectOfType<UserInput>();
        InputManager inputManager = FindObjectOfType<InputManager>();
        
        // Remove asteroid remaining actions
        int turnRemaning = inputManager.TurnNumber - userInput.playerInputs.Count;
        for (int i = 0; i < turnRemaning; i++)
        {
            inputManager.asteroidsActions.RemoveAt(inputManager.asteroidsActions.Count - 1); // TBD Morgan : Hold action for Asteroid
        }

        while (userInput.playerInputs.Count < inputManager.TurnNumber){
            userInput.playerInputs.Add(SpaceObject.Action.Hold);
            inputManager.asteroidsActions.Add(Asteroid.AsteroidAction.OO);
        }
        inputManager.playerActions = userInput.playerInputs;

        gameOnGoing = true;
        gameManager.PlayGameAsync();
    }

    private void ResetScreenOnOff()
    {
        resetUI.SetActive(!resetUI.activeSelf);
    }

    private void AllowSwipe(int buttonNo)
    {
        Button button = gameButtons[buttonNo];
        if (!swipeAllowed){
            ChangeSpriteState(isOn, button);
        }
        else {
            ChangeSpriteState(isOff, button);
        }
        swipeAllowed = !swipeAllowed;
    }

    private void ChangeSpriteState(List<Sprite> sprites, Button button)
    {
        SpriteState myState;
        myState.pressedSprite = sprites[1];
        button.GetComponent<Image>().sprite = sprites[0];
        button.spriteState = myState;
    }

    private void CancelMovement(){
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
