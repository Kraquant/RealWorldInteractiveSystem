using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Graphs;

public class UserInput : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public List<SpaceObject.Action>  playerInputs;

    [SerializeField] TextMeshProUGUI movementsList;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        if (inputManager == null) throw new System.Exception("Could not fetch the Input Manager");

        playerInputs = new List<SpaceObject.Action>(inputManager.TurnNumber);

        GameManager GM = FindObjectOfType<GameManager>();
        GM.OnGameEnded += GM_OnGameEnded;
        //GM.OnGameStarted 
        //SetInputs();
    }

    private void GM_OnGameEnded(GameManager.EndGameCondition endCondition)
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            startTouchPosition = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){
            endTouchPosition = Input.GetTouch(0).position;

            if(playerInputs.Count >= inputManager.TurnNumber)
            {
                // TBD Show error message to player.
            }else
            {
                SpaceObject.Action playerAction = SwipeInputIs(startTouchPosition, endTouchPosition);
                playerInputs.Add(playerAction);     // Add to actions list
                addActionToText(playerAction);      // Add action to text
            }
        }
    }


    private SpaceObject.Action SwipeInputIs(Vector2 start, Vector2 end)
    {
        SpaceObject.Action action;
        if (Mathf.Abs(start.x - end.x) >Mathf.Abs(start.y - end.y)){
            if (start.x - end.x > 0){
                action = SpaceObject.Action.Left;
                Debug.Log("LEFT");
            }else{
                action = SpaceObject.Action.Right;
                Debug.Log("RIGHT");
            }
        }else{
            if (start.y - end.y > 0){
                action = SpaceObject.Action.Turn;
                Debug.Log("TURN");
            }
            else{
                action = SpaceObject.Action.Front;
                Debug.Log("FORWARD");
            }
        }
        return action;
    }

    private void addActionToText(SpaceObject.Action action)
    {
        switch (action)
        {
            case SpaceObject.Action.Right:
                movementsList.text += " <sprite=0>|";
                break;
            case SpaceObject.Action.Left:
                movementsList.text += " <sprite=1>|";
                break;
            case SpaceObject.Action.Front:
                movementsList.text += " <sprite=2>|";
                break;
            case SpaceObject.Action.Turn:
                movementsList.text += " <sprite=3>|";
                break;
            default:
                break;
        }
    }

    private void SetInputs()
    {
        inputManager.playerActions = new List<SpaceObject.Action>(inputManager.TurnNumber);
    }
}


