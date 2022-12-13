using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.TerrainTools;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(TurnManager))]
public class GameManager : MonoBehaviour
{
    #region Delegates Enum
    public enum EndGameCondition
    {
        playerDeath,
        playerWin,
        playerMissedGoal,
        levelAbort,
    }
    public delegate void EndGameEvent(EndGameCondition endCondition);
    public delegate void GameStateEvent();
    #endregion
    #region Attributes
    private InputManager _inputManager;
    private TurnManager _turnManager;
    private bool _isPaused;
    private IPlayer _player;

    [Range(0, 1000)] public int delayBetweenTurns;
    
    #endregion
    #region Properties
    public int CurrentTurn { get; private set; }
    public bool IsPlaying { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (value == _isPaused) return;
            _isPaused = value;
            OnGamePaused?.Invoke();
        }
    }
    #endregion
    #region Events
    // Declare the event.
    public event EndGameEvent OnGameEnded;
    public event GameStateEvent OnGameStarted;
    public event GameStateEvent OnGamePaused;
    public event GameStateEvent OnTurnOver;
    #endregion
    #region Initialization
    private void OnEnable()
    {
        OnGameStarted += GameStartMessage;
        OnGamePaused += GamePausedMessage;
        OnTurnOver += TurnOverMessage;
        OnGameEnded += GameEndMessage;

        _player = FindObjectsOfType<MonoBehaviour>().OfType<IPlayer>().FirstOrDefault();
        if (_player != null)
        {
            _player.OnPlayerWin += MonitorPlayerWin;
            _player.OnPlayerDeath += MonitorPlayerDeath;
        }
    }
    private void OnDisable()
    {
        OnGameStarted -= GameStartMessage;
        OnGamePaused -= GamePausedMessage;
        OnTurnOver -= TurnOverMessage;
        OnGameEnded -= GameEndMessage;
    }
    private void Awake()
    {
        _turnManager = GetComponent<TurnManager>();
        _inputManager = GetComponent<InputManager>();

        //Initialize values
        IsGameOver = false;
        IsPlaying = false;
        CurrentTurn = 0;
        _isPaused = false;

        HexVisualizer terrainVisualizer = GetComponentInChildren<HexVisualizer>();
        terrainVisualizer.CreateShape(_turnManager.Terrain.TerrainShape, _turnManager.Terrain.CellSize);
    } 
    #endregion

    public async void PlayGameAsync()
    {
        if (!PlayableGame(out string error)) throw new System.Exception("Not every condition is met to play the level:\n" + error);

        //Initialize the required variables
        IsPlaying = true;
        CurrentTurn = 0;
        OnGameStarted?.Invoke();
        CancellationTokenSource cts = new CancellationTokenSource();

        for (int i = 0; i < _inputManager.TurnNumber; i++)
        {
            CurrentTurn = i;

            if (IsGameOver)
            {
                IsPlaying = false;
                CurrentTurn = 0;
                return;
            }

            _inputManager.SetActions(CurrentTurn);

            await _turnManager.PlayTurnAsync();
            OnTurnOver?.Invoke();
            await SpaceUtilities.WaitUntilAsync(() => !IsPaused, 100, cts.Token);
            await Task.Delay(delayBetweenTurns);
        }

        //The game is done playing the turns. If the win condition hasn't alreedy been called: the player lost
        IsPlaying = false;
        CurrentTurn = 0;
        OnGameEnded?.Invoke(EndGameCondition.playerMissedGoal);
    }
    public void RequestLevelAbort()
    {
        if (!IsPlaying) return;
        IsGameOver = true;
        OnGameEnded?.Invoke(EndGameCondition.levelAbort);
    }

    #region Private methods
    private bool PlayableGame(out string message)
    {
        if (_turnManager == null)
        {
            message = "Turn manager variable is not assigned";
            return false;
        }

        if (IsPlaying)
        {
            message = "The game is already being played";
            return false;
        }
        if (_player == null)
        {
            message = "The player was not found or is dead";
            return false;
        }
        if (!_player.IsAlive)
        {
            message = "The player is dead";
            return false;
        }
        if (_player.HasWon)
        {
            message = "The player has already won the game";
            return false;
        }

        message = "Game is playable";
        return true;
    }
    private void MonitorPlayerWin()
    {
        IsGameOver = true;
        OnGameEnded?.Invoke(EndGameCondition.playerWin);
    }
    private void MonitorPlayerDeath()
    {
        IsGameOver = true;
        OnGameEnded?.Invoke(EndGameCondition.playerDeath);
    }
    #endregion
    #region GameState Debug Log
    private void GameStartMessage() => Debug.Log("Game has started");
    private void GamePausedMessage() => Debug.Log("Game paused");
    private void TurnOverMessage() => Debug.Log("Turn number " + CurrentTurn + " is over");
    private void GameEndMessage(EndGameCondition endCondition)
    {
        string message = endCondition switch
        {
            EndGameCondition.playerDeath => "Player death",
            EndGameCondition.playerWin => "Player win",
            EndGameCondition.playerMissedGoal => "Player missed the goal",
            EndGameCondition.levelAbort => "Level abort",
            _ => throw new System.NotImplementedException(),
        };
        Debug.Log("Game has ended : " + message);
    }
    #endregion
}
