using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator transition;
    public float transitionTime = 0.3f;
    public List<string> sceneName;
    public List<Button> buttonList;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        foreach (Button button in buttonList)
        {
            button.onClick.AddListener(() => ButtonClicked(buttonList.IndexOf(button)));
        }
    }
    void ButtonClicked(int buttonNo)
    {
        //Debug.Log("You have clicked the button!");
        LoadScene(sceneName[buttonNo], buttonList[buttonNo].name);
    }

    private void LoadScene(string sceneName, string buttonName)
    {
        StartCoroutine(LoadLevel(sceneName, buttonName));
    }

    IEnumerator LoadLevel(string sceneName, string buttonName)
    {
        // Play Animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        if (PlayerPrefs.GetInt("LoadSaved") == 1)
        {
            if (buttonName == "Start Button")
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt("SavedScene"));
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            Debug.Log("Loading Scene");
            SceneManager.LoadScene(sceneName);
        }
    }
    public void QuitLevel()
    {
        PlayerPrefs.SetInt("LoadSaved", 1);
        PlayerPrefs.SetInt("SavedScene", SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Quit Level");
    }

    public void LoadPreviousLevel()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("SavedScene"));
    }
}
