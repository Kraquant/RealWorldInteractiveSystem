using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will change the inputs of the different objects depending on the turn
/// </summary>
public class InputManager : MonoBehaviour
{
    private Spaceship _playerSpaceship;
    private List<Asteroid> _asteroids;

    [Range(1, 20)][SerializeField] int _turnNumber = 1;
    public List<SpaceObject.Action> playerActions;
    public List<Asteroid.AsteroidAction> asteroidsActions;

    public int TurnNumber { get => _turnNumber; }

    public void SetActions(int turnNumber)
    {
        if (turnNumber < 0) throw new System.Exception("The turn number cannot be negative");
        if (turnNumber > TurnNumber) throw new System.Exception("The turn number is too high");

        SetPlayerInput(turnNumber);
        SetAsteroidAction(turnNumber);
    }

    private void Awake()
    {
        _playerSpaceship = GetComponentInChildren<Spaceship>();
        _asteroids = new List<Asteroid>(GetComponentsInChildren<Asteroid>());
        if (_playerSpaceship == null) throw new System.Exception("Could not find the player spaceship");
    }

    private void SetPlayerInput(int turnNumber)
    {
        _playerSpaceship.NextAction = playerActions[turnNumber];
    }
    private void SetAsteroidAction(int turnNumber)
    {
        _asteroids.RemoveAll(item => item == null);
        foreach (Asteroid asteroid in _asteroids) asteroid.NextAsteroidAction = asteroidsActions[turnNumber];
    }
}
