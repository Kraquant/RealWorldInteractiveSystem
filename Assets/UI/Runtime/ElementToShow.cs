using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;
using UnityEngine.SceneManagement;

public class ElementToShow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Animator transition;
    [SerializeField] float timing;


    private void Update()
    {
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        // Play Animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(timing);
    }
}
