using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.MPE;

public class Dialogue : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI dialogue;
    [SerializeField] GameObject pressText;
    public float textSpeed;
    public string[] lines;

    private int index;
    private bool proceedNext = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogue.text = string.Empty;
        startDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && proceedNext)
        {
            pressText.SetActive(false);
            if (dialogue.text == lines[index])
            {
                nextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogue.text = lines[index];
            }
        }
    }

    #region Dialogue

    void startDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {
        proceedNext = false;
        foreach (char c in lines[index].ToCharArray())
        {
            dialogue.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        proceedNext = true;
        pressText.SetActive(true);
    }
    void nextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            dialogue.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }
}

    #endregion


