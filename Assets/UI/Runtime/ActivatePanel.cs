using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivatePanel : MonoBehaviour
{

    public GameObject scene;

    public void activateScene()
    {
        scene.SetActive(false);
    }

    public void desactivateScene()
    {
        scene.SetActive(false);
    }
}

