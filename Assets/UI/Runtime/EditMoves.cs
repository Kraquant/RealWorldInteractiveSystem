using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMoves : MonoBehaviour
{
    [SerializeField] GameObject moveEditorPrefab;

    private void OnEnable()
    {
    }


    void Start()
    {
        var Movement = Instantiate<GameObject>(moveEditorPrefab, new Vector3(0, 50, 0), Quaternion.identity);
        Movement.transform.parent = GameObject.Find("Moves Panel").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
