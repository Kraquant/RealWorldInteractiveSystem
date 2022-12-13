using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLevel : MonoBehaviour {
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
