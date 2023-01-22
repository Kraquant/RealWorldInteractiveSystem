using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.MPE;
using Codice.Client.Common.GameUI;
using UnityEngine.UI;
using UnityEditorInternal;
using static System.TimeZoneInfo;
using static UnityEngine.UI.Selectable;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    [SerializeField] CanvasGroup bubble;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject pressText;
    [SerializeField] GameObject[] hiddenAnimations;
    [SerializeField] Animator movement;
    [SerializeField] GameManager playerMap;
    [SerializeField] GameManager asteroidMap;

    [SerializeField] List<string> lines;

    [SerializeField] Animator transition;
    [SerializeField] GameObject mapAst;
    [SerializeField] GameObject mapPlayer;


    public float transitionTime;
    public float textSpeed;
    public bool hasAnimation;
    public bool noPlayer;
    public int tutorialScene;

    

    private int index;
    private bool proceedNext = false;
    private bool fadingIn, fadingOut;
    private bool animating;
    private bool Up, CenterToDown, DownToCenter, UpToCenter;

    // Start is called before the first frame update
    void Start()
    {
        fadingIn = false;
        fadingOut = false;
        text.text = string.Empty;
        animating = false;
        Up = false; CenterToDown = false; DownToCenter = false; UpToCenter = false;

        asteroidMap.OnGameEnded += GM_OnGameEnded;
        playerMap.OnGameEnded += GM_OnGameEnded;
        if (noPlayer)
        {
            playerMap.PlayGameAsync();
        }
        startDialogue();
    }

    private void GM_OnGameEnded(GameManager.EndGameCondition endCondition)
    {
        Debug.Log("Game Ended");
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
            else if(movement.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && (movement.GetCurrentAnimatorStateInfo(0).IsName("ExplanationCenterToDown")) && !CenterToDown)
            {
                CenterToDown = true;
                fadingIn = true;
                animating = false;
            }
            else if(movement.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && (movement.GetCurrentAnimatorStateInfo(0).IsName("ExplanationDownToCenter")) && !DownToCenter)
            {
                DownToCenter = true;
                fadingIn = true;
                animating = false;
            }
            else if (movement.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && (movement.GetCurrentAnimatorStateInfo(0).IsName("ExplanationUpToCenter")) && !UpToCenter)
            {
                UpToCenter = true;
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
                    loadNextLine();
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && proceedNext && !animating)
        {
            if (hasAnimation) {
                fadingOut = true;;
            }
            else
            {
                loadNextLine();
            }
        }
    }

    #region Dialogue


    private void loadText()
    {
        if(tutorialScene == 1)
        {
            lines.Add("Welcome to Hamsteroid. I am your guide in this game, Ham. Let me explain you the game."); //0
            lines.Add("In this game, you will be the spaceship and your goal will be to get to this planet."); //1
            lines.Add("To get to the planet, you will have to move your spaceship in this kind of map where a cell is an hexagon."); //2
            lines.Add("Your spaceship can move in FOUR possible way depending on ITS ORIENTATION. It can move ..."); //3
            lines.Add("Forward ... By swiping up."); //4
            lines.Add("On its left ... By swiping left."); //5
            lines.Add("On its right ... By swiping right."); //6
            lines.Add("And turn around. By swiping down."); //7
            lines.Add("You should have also noticed that the spaceship orientation changes when you turn left, right or turn around. Remember that, this is an important aspect of the game."); //8
            lines.Add("Now let's finish this level. Can you see how to end it ? "); //9
            lines.Add("You have to move forward ..."); //10
            lines.Add("Then turn right ... "); //11
            lines.Add("Easy, right ?"); //12
            lines.Add("However, when you will be playing the game, it will not be like how I showed you."); //13
            lines.Add("You will have a limited amount of moves and will have to plan all your movements ahead."); //14

            lines.Add("In this game, you will also have to face some obstacles on your way called Asteroid"); //15
            lines.Add("Until now, three of them have been discovered. Bumpy, Ghost and Heavy."); //16
            lines.Add("I forgot what are their specificity but we have a manual that should guide you."); //17

            lines.Add("They will be on your way to the goal and will move slightly differently."); // 18
            lines.Add("Unlike the spaceship, the asteroids can move in its 6 direction detailed by numbered dots."); //19
            lines.Add("The position of the numbered dots will be the same, but depending on asteroids, it may rotate."); //20

            lines.Add("Moreover, thanks to our super technology, all their movements can be analyzed. The number of dots explains the directon they will go. Here their movements are : 2, 4, 5, 6, 1, 3."); //21
            lines.Add("Let's see in action ..."); //22


            lines.Add("I hope this has been clear. You can replay this guide if needed through the manual."); //23
            lines.Add("See you later"); // 24
        }
        if(tutorialScene == 2)
        {
            lines.Add("This is the screen level you will be in. I will go through everything and explain how it works.");
            lines.Add("Above, you have the level number in the middle. You have two buttons. <color=#ff0000ff> Left </color>to go to the <color=#ff0000ff> Level Selector </color>and <color=#800080ff> Right </color>to go to the <color=#800080ff> Main Menu</color>.");
            lines.Add("Just below me is the <color=#ff0000ff> map level </color>. You should be able to recognize your <color=#ff0000ff> spaceship </color>and our <color=#ff0000ff> futur goal</color>.");
            lines.Add("This panel allows you to know your current displacements and asteroids' movements.");
            lines.Add("<color=#ff0000ff>Yours </color>are represented by <color=#ff0000ff> arrows </color>detailing the direction chosen. When swipipng, it will be registered here.");
            lines.Add("<color=#ff0000ff>Asteroids' </color>are reprented by <color=#ff0000ff> dots between 1 and 6 </color>also detailing how they are going to move.");
            lines.Add("The small panel on the bottom left tells you how <color=#ff0000ff> many moves </color>left you have. For example, you have a total of 6 moves available.");
            lines.Add("Below here are three other buttons.");
            lines.Add("The one on the <color=#ff0000ff>right </color>is to <color=#ff0000ff>reset </color>the level. If you are lost in your calculations, press it and restard again.");
            lines.Add("The <color=#ff0000ff>middle </color>one is to <color=#ff0000ff>caccel your last input</color> .");
            lines.Add("The last one on the <color=#ff0000ff>left </color>is to <color=#ff0000ff>start </color>the game. <color=#ff0000ff>After planning</color> ahead your movements, press this button to see if you got it right.");
            lines.Add("Well I hope my explanations alone were clear ! If not, ask the creators directly !");
            lines.Add("I wish you good luck and don't get us killed. I'd very thanksfull for that !");
        }
    }

    void startDialogue()
    {
        loadText();
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

        if(noPlayer)
        {
            if ((index <= 7) && (index >= 4) || (index == 10) || (index == 11))
            {
                yield return new WaitForSeconds(0.5f);
                playerMap.PlayGameTurn();
                yield return new WaitForSeconds(0.5f);
            }

            if (index == 13)
            {
                mapPlayer.SetActive(false);
                mapAst.SetActive(true);
            }

            if (index == 22)
            {
                yield return new WaitForSeconds(0.5f);
                asteroidMap.PlayGameAsync();
                yield return new WaitForSeconds(3.5f);
            }
        }

        proceedNext = true;
        pressText.SetActive(true);
    }
    void nextLine()
    {
        if (index < lines.Count - 1)
        {
            index++;
            text.text = string.Empty;
            if (!hasAnimation)
            {
                StartCoroutine(TypeLine());
            }
        }
        else
        {
            nextScene();
        }
    }

    void loadNextLine()
    {
        elementsManager();
        if (text.text == lines[index])
        {
            nextLine();
        }
        else
        {
            StopAllCoroutines();
        }
    }
    #endregion


    #region transitioning
    private void elementsManager()
    {
        pressText.SetActive(false);
        if (index < lines.Count - 1)
        {
            if (!(hiddenAnimations[index].name == hiddenAnimations[index + 1].name))
            {
                hiddenAnimations[index].SetActive(false);
            }
        }
        if (hasAnimation)
        {
            animating = true;

            switch (index)
            {
                case 0:
                    movement.SetInteger("Position", 1);
                    break;
                case 2:
                    movement.SetInteger("Position", 2);
                    break;
                case 6:
                    movement.SetInteger("Position", 3);
                    break;
                case 10:
                    movement.SetInteger("Position", 4);
                    break;
                default:
                    fadingIn = true;
                    animating = false;
                    break;
            }
        }
        else
        {
            fadingIn = true;
        }
        
    }

    public void nextScene()
    {
        Debug.Log("Loading scene");
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        // Play Animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Loading next level");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("No next level");
            SceneManager.LoadScene("MainMenu");
        }
    }

    #endregion

}




