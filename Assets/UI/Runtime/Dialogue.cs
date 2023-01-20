using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.MPE;
using Codice.Client.Common.GameUI;
using UnityEngine.UI;
using UnityEditorInternal;

public class Dialogue : MonoBehaviour
{
    [SerializeField] CanvasGroup bubble;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject pressText;
    [SerializeField] GameObject[] hiddenAnimations;
    [SerializeField] Animator movement;

    public float textSpeed;
    public string[] lines;

    private int index;
    private bool proceedNext = false;
    private bool fadingIn, fadingOut;
    private bool animating;
    private bool Up, UpToDown, DownToCenter;

    // Start is called before the first frame update
    void Start()
    {
        fadingIn = false;
        fadingOut = false;
        text.text = string.Empty;
        animating = false;
        Up = false; UpToDown = false; DownToCenter = false;
        startDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (animating)
        {
            if (movement.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f &&
                ((movement.GetCurrentAnimatorStateInfo(0).IsName("ExplanationUp"))) && !Up)
            {
                Up = true;
                fadingIn = true;
                animating = false;
            }
            else if(movement.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && (movement.GetCurrentAnimatorStateInfo(0).IsName("ExplanationUpToDown")) && !UpToDown){
                UpToDown = true;
                fadingIn = true;
                animating = false;
            }
            else if(movement.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && (movement.GetCurrentAnimatorStateInfo(0).IsName("ExplanationDownToCenter")) && !DownToCenter)
            {
                DownToCenter = true;
                fadingIn = true;
                animating = false;
            }
        }

        if (fadingIn)
        {
            if (bubble.alpha < 1)
            {
                bubble.alpha += Time.deltaTime;
                if (bubble.alpha == 1)
                {
                    fadingIn = false;
                    StartCoroutine(TypeLine());
                }
            }
        }

        if (fadingOut)
        {
            if (bubble.alpha > 0)
            {
                bubble.alpha -= Time.deltaTime;
                if(bubble.alpha == 0)
                {
                    fadingOut = false;
                    elementsManager();
                    if (text.text == lines[index])
                    {
                        nextLine();
                    }
                    else
                    {
                        StopAllCoroutines();
                        text.text = lines[index];
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && proceedNext && !animating)
        {
            fadingOut = true;
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
        hiddenAnimations[index].SetActive(true);
        foreach (char c in lines[index].ToCharArray())
        {
            text.text += c;
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
            text.text = string.Empty;
        }
    }
    #endregion


    #region transitioning
    private void elementsManager()
    {
        pressText.SetActive(false);
        hiddenAnimations[index].SetActive(false);
        animating = true;

        switch (index)
        {
            case 0:
                movement.SetInteger("Position", 1);
                break;
            case 2:
                movement.SetInteger("Position", 2);
                break;
            default:
                fadingIn = true;
                animating = false;
                break;
        }
    }

    void startFadeIn()
    {
    }
    void startFadeOut(){
    }

    #endregion
}




