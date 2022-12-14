using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] AudioSource buttonSound;
    public void playThisSoundEffect()
    {
        buttonSound.Play();
    }
}
