using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipAnimationEvent : MonoBehaviour
{
    public Spaceship spaceship;

    // Start is called before the first frame update
    void Start()
    {
        spaceship = transform.parent.gameObject.GetComponent<Spaceship>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndCollideWithAsteroid()
    {
        Debug.Log("End animation for collide with asteroid");
        spaceship.DestroySpaceObject();
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.IsPaused = false;
    }
}
