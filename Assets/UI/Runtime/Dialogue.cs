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
using UnityEngine.Windows.Speech;

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
            lines.Add("Welcome to <color=#ff0000ff>Hamsteroid</color>. I am your <color=#ff0000ff>guide</color> in this game, <color=#ff0000ff>Ham</color>. Let me explain you the game."); //0
            lines.Add("In this game, you will be the <color=#ff0000ff>spaceship</color> and your goal is to <color=#ff0000ff>get to the planet</color>."); //1
            lines.Add("To get to the planet, you will have to <color=#ff0000ff>move your spaceship</color> in this kind of map where a cell is an <color=#ff0000ff>hexagon</color>."); //2
            lines.Add("Your spaceship can move in <color=#ff0000ff>four</color> possible way depending on <color=#ff0000ff>its orientation</color>. It can move ..."); //3
            lines.Add("<color=#ff0000ff>Forward</color> ... By swiping up."); //4
            lines.Add("On <color=#ff0000ff>its left</color> ... By swiping left."); //5
            lines.Add("On <color=#ff0000ff>its right</color> ... By swiping right."); //6
            lines.Add("And <color=#ff0000ff>rotate by 180 degrees</color>. By swiping down."); //7
            lines.Add("You should have noticed that the spaceship <color=#ff0000ff>orientation changes</color> when you move. Remember that, this is an <color=#ff0000ff>important aspect</color> of the game."); //8
            lines.Add("Now let's finish this level. Can you see how to get to the goal in <color=#ff0000ff>just 2 moves</color> ?"); //9
            lines.Add("First move forward ..."); //10
            lines.Add("Then turn right ... "); //11
            lines.Add("Easy, right ?"); //12
            lines.Add("However, when you will be playing the game, it's going to be <color=#ff0000ff>slightly different</color>. You will have a <color=#ff0000ff>limited amount of moves</color> and will have to <color=#ff0000ff>plan</color> all your movements ahead."); //13
            lines.Add("Our spaceship neesd all the coordonate first before moving. Meaning that the spaceship <color=#ff0000ff>won't move as you swipe</color>. "); //14
            lines.Add("You will need to <color=#ff0000ff>register all</color> your movements and <color=#ff0000ff>validate</color> your trajectory. Keep that in mind."); //15
            
            lines.Add("In this game, you will also have to face some obstacles on your way called <color=#ff0000ff>Hamsteroid</color>. Until now, three of them have been discovered. <color=#ff0000ff>Bumpy</color>, <color=#ff0000ff>Ghost</color> and <color=#ff0000ff>Heavy</color>."); //16
            lines.Add("I forgot what their <color=#ff0000ff>specificities</color> are but a <color=#ff0000ff>manual</color> explaining everything available through the main menu."); //17
            
            lines.Add("They will be on your way to the goal and will move <color=#ff0000ff>slightly differently</color> than your spaceship."); // 18
            lines.Add("Unlike the spaceship, the asteroids can move in its 6 direction detailed by <color=#ff0000ff>numbered dots</color>."); //19
            lines.Add("The position of the numbered dots will be the same, but depending on asteroids, it may rotate."); //20
            //2, 4, 5, 6, 1, 3
            lines.Add("You will have a <color=#ff0000ff>panel</color> with asteroids movements information. Their movements will be <color=#ff0000ff>described</color> by numbered dots and asteroids will move acoordingy."); //21
            lines.Add("The <color=#ff0000ff>numbered dots</color> look like this on your panel : <sprite=\"AsteroidMoves\" index=0>| <sprite=\"AsteroidMoves\" index=1>|" +
                " <sprite=\"AsteroidMoves\" index=2>| <sprite=\"AsteroidMoves\" index=3>| <sprite=\"AsteroidMoves\" index=4>| <sprite=\"AsteroidMoves\" index=5>"); //22
            lines.Add("Each one of them indicating one direction of the asteroid. Let's see their movements in action."); //23
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=1> :"); //24
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=3> :"); //25
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=4> :"); //26
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=5> :"); //27
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=0> :"); //28
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=2> :"); //29


            lines.Add("I hope this has been clear. You can <color=#ff0000ff>replay</color> this guide if needed through the <color=#ff0000ff>manual</color>."); //30
            lines.Add("See you later"); // 31
        }
        if(tutorialScene == 2)
        {
            lines.Add("This is the <color=#ff0000ff>screen level</color> you will be in. I will go through everything and explain how it <color=#ff0000ff>works</color>.");
            lines.Add("Above, you have the level number in the middle. You have two buttons. <color=#ff0000ff> Left </color>to go to the <color=#ff0000ff> Level Selector </color>and <color=#800080ff> Right </color>to go to the <color=#800080ff> Main Menu</color>.");
            lines.Add("Just below me is the <color=#ff0000ff> map level </color>. You should be able to recognize your <color=#ff0000ff> spaceship </color>and our <color=#ff0000ff> futur goal</color>.");
            lines.Add("This panel allows you to know your <color=#ff0000ff>current displacements</color>, <color=#ff0000ff>plan</color> and <color=#ff0000ff>asteroids' movements</color>.");
            lines.Add("<color=#ff0000ff>Yours </color>are represented by <color=#ff0000ff> arrows </color>detailing the direction chosen. When swipipng, it will be registered here.");
            lines.Add("<color=#ff0000ff>Asteroids' </color>are reprented by <color=#ff0000ff> dots between 1 and 6 </color>also detailing how they are going to move.");
            lines.Add("The small panel on the bottom left tells you how <color=#ff0000ff> many moves </color>left you have. For example, you have a total of <color=#ff0000ff>6 moves remaining</color> or <color=#ff0000ff>available</color>.");
            lines.Add("Below here are three other buttons.");
            lines.Add("The one on the <color=#ff0000ff>right </color>is to <color=#ff0000ff>reset </color>the level. If you are lost in your calculations, press it and restard again.");
            lines.Add("The <color=#ff0000ff>middle </color>one is to <color=#ff0000ff>caccel your last input</color> .");
            lines.Add("The last one on the <color=#ff0000ff>left </color>is to <color=#ff0000ff>start </color>the game. <color=#ff0000ff>After planning</color> ahead your movements, press this button to see if you got it right.");
            lines.Add("Well I hope my <color=#ff0000ff>explanations</color> alone were clear ! If not, ask the <color=#ff0000ff>creators</color> directly or replay the guide through the <color=#ff0000ff>manual</color>!");
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
                asteroidMap.PlayGameAsync();
            }

            if ((index <= 30) && (index >= 24))
            {
                yield return new WaitForSeconds(0.5f);
                asteroidMap.PlayGameTurn();
                yield return new WaitForSeconds(0.5f);
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




