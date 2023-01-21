using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Xml.Xsl;

public class LevelScenesLoader : MonoBehaviour
{
    [SerializeField] string levelName;

    private void Awake()
    {
        SceneManager.LoadScene("LevelUI", LoadSceneMode.Additive);

        SceneManager.sceneLoaded += SetLevelText;
    }

    private void SetLevelText(Scene _, LoadSceneMode loadmode)
    {
        if (loadmode != LoadSceneMode.Additive) return;

        GameObject titleGO = GameObject.Find("Scene Title");
        titleGO.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = levelName;
    }
}
