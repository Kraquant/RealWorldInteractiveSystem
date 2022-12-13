using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementsInput : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public enum Entry
    {
        FORWARD,
        LEFT,
        RIGHT,
        TURN
    }

    public Entry entry;
    private double max;


    private void Update()
    {
        if(Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPosition = Input.GetTouch(0).position;
        }

        if(Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPosition = Input.GetTouch(0).position;

            if (Mathf.Abs(startTouchPosition.x - endTouchPosition.x) >
            Mathf.Abs(startTouchPosition.y - endTouchPosition.y))
            {
                if (startTouchPosition.x - endTouchPosition.x > 0)
                {
                    entry = Entry.LEFT;
                    Debug.Log("LEFT");
                }
                else
                {
                    entry = Entry.RIGHT;
                    Debug.Log("RIGHT");
                }
            }
            else
            {
                if (startTouchPosition.y - endTouchPosition.y > 0)
                {
                    entry = Entry.TURN;
                    Debug.Log("TURN");
                }
                else
                {
                    entry = Entry.FORWARD;
                    Debug.Log("FORWARD");
                }
            }
        }

        
    }
}
