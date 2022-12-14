using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;
using System;
using UnityEngine.UI;

public class UserInput : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private InputManager inputManager;
    private LevelManager levelManager;
    public List<SpaceObject.Action>  playerInputs;

    [SerializeField] TextMeshProUGUI movementsList;
    [SerializeField] TextMeshProUGUI moveNumberText;
    [SerializeField] GameObject warningUI;
    private int movesLeft;

    // Swipe area
    private BoxCollider2D box;
    private Vector3 center, extents;
    private Single minY, maxY;

    private float startTime;

    private void Awake()
    {
        warningUI.SetActive(false);
    }

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        if (inputManager == null) throw new System.Exception("Could not fetch the Input Manager");

        levelManager = FindObjectOfType<LevelManager>();

        playerInputs = new List<SpaceObject.Action>(inputManager.TurnNumber);

        movesLeft = inputManager.TurnNumber;
        moveNumberText.text = "Moves : " + movesLeft.ToString();

        // Swipe boundaries
        box = GetComponent<BoxCollider2D>();
        center = box.bounds.center;
        extents = box.bounds.extents;
        minY = center.y - extents.y;
        maxY = center.y + extents.y;
    }

    private void Update()
    {
        if (levelManager.swipeAllowed && !levelManager.gameOnGoing)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {
                
                if (Input.GetMouseButtonDown(0))
                {
                    startTouchPosition = Input.mousePosition;
                }
                else
                {
                    startTouchPosition = Input.GetTouch(0).position;
                }
                startTime = Time.time;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    endTouchPosition = Input.mousePosition;
                }
                else
                {
                    endTouchPosition = Input.GetTouch(0).position;
                }
                if (inTheBox(startTouchPosition) && Time.time - startTime > 0.1)// && (Mathf.Abs(startTouchPosition.x - endTouchPosition.x) > 50 || (startTouchPosition.y - endTouchPosition.y) > 50))
                {
                    if (playerInputs.Count >= inputManager.TurnNumber){
                        Debug.Log("Too much inputs");
                        // Show message with saying too much inputs.
                        StartCoroutine(ShowMessage(warningUI, 1));
                    }
                    else{
                        SpaceObject.Action playerAction = SwipeInputIs(startTouchPosition, endTouchPosition);
                        playerInputs.Add(playerAction);     // Add to actions list
                        addActionToText(playerAction);      // Add action to text
                    }
                }
            }
        }
    }

    #region Input functions
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
        movesLeft -= 1;
        moveNumberText.text = "Moves : " + movesLeft.ToString();
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

    IEnumerator ShowMessage(GameObject warningMessage, float delay)
    {
        warningMessage.SetActive(true);

        warningMessage.transform.GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0, delay, false);
        warningMessage.GetComponent<Image>().CrossFadeAlpha(0, delay, false);
        yield return new WaitForSeconds(delay);
        warningMessage.SetActive(false);
    }

    #endregion
    public void removeText()
    {
        movementsList.text = movementsList.text.Substring(0, movementsList.text.Length - 12);
        movesLeft += 1;
        moveNumberText.text = "Moves : " + movesLeft.ToString();
    }

    private bool inTheBox(Vector2 position)
    {
        if(position.y < maxY && minY < position.y){
            return true;
        }
        else{
            return false;
        }
    }

    private void SetInputs()
    {
        inputManager.playerActions = new List<SpaceObject.Action>(inputManager.TurnNumber);
    }
}


