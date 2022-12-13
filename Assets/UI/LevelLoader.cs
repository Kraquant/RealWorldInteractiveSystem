using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 0.3f;
    public string sceneName;
    public Button button;
    bool pressed = false;

    // Update is called once per frame
    void Update()
    {
        button.onClick.AddListener(TaskOnClick);
        if (pressed)
        {
            LoadNextLevel(sceneName);
        }
    }
    void TaskOnClick()
    {
        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("You have clicked the button!");
        pressed = true;
    }

    public void LoadNextLevel(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        // Play Animation
        transition.SetTrigger("Start");
        
        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        if (PlayerPrefs.GetInt("LoadSaved") == 1)
        {
            if (button.name == "Start Button")
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt("SavedScene"));
            }
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

}
