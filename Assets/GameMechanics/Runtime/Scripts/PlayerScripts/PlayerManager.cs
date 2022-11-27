using Codice.CM.Client.Differences;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerControl controls;
    [SerializeField] Spaceship player;
    [SerializeField] TurnManager turnManager;

    private void Awake()
    {
        controls = new PlayerControl();
    }

    private void Start()
    {
        controls.SpaceshipControl.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        controls.SpaceshipControl.PlayTurn.performed += ctx => PlayTurn();
    }

    private void Move(Vector2 direction)
    {
        SpaceObject.Action action;
        if (direction.x > 0) action = SpaceObject.Action.Right;
        else if (direction.x < 0) action = SpaceObject.Action.Left;
        else if (direction.y > 0) action = SpaceObject.Action.Front;
        else if (direction.y < 0) action = SpaceObject.Action.Turn;
        else throw new System.NotImplementedException();
        player.NextAction = action;

        Debug.Log("New action set");
    }

    private void PlayTurn()
    {
        turnManager.PlayTurnAsync();
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
