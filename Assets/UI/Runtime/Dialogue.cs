using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    private bool speedUp;
    private bool pressToDisplayText;
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
        speedUp = false;
        text.text = string.Empty;
        animating = false;
        pressToDisplayText = false;
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
                if (bubble.alpha == 0)
                {
                    fadingOut = false;
                    loadNextLine();
                }
            }
        }

        if(pressToDisplayText)
        {
            if (Input.GetMouseButtonDown(0))
            {
                speedUp = true;
            }
        }

        if (Input.GetMouseButtonDown(0) && proceedNext && !animating)
        {
            proceedNext = false;
            if (hasAnimation) {
                fadingOut = true;
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
            lines.Add("Welcome to <color=#800080ff>Hamsteroid</color>. My name is <color=#800080ff>Ham</color> and I'll be your guide in this adventure"); //0
            lines.Add("In this game, you will be the <color=#800080ff>spaceship</color> and your goal is to <color=#800080ff>get to the planet</color>."); //1
            lines.Add("To get to the planet, you will have to <color=#800080ff>move your spaceship</color> in this kind of map with <color=#800080ff>hexagon</color> cells."); //2
            lines.Add("Your spaceship can move in <color=#800080ff>four</color> directions depending on <color=#800080ff>its orientation</color>. It can move ..."); //3
            lines.Add("<color=#800080ff>Forward</color> ... By swiping up."); //4
            lines.Add("On <color=#800080ff>its left</color> ... By swiping left."); //5
            lines.Add("On <color=#800080ff>its right</color> ... By swiping right."); //6
            lines.Add("And <color=#800080ff>rotate by 180 degrees</color>. By swiping down."); //7
            lines.Add("You should have noticed that the spaceship <color=#800080ff>orientation changes</color> when you move. Remember that, its an <color=#800080ff>important aspect</color> of the game."); //8
            lines.Add("Now let's finish this level. Can you guess how to get to the goal in <color=#800080ff>just 2 moves</color> ?"); //9
            lines.Add("First move forward (swipe up)..."); //10
            lines.Add("Then turn right (swipe right)... "); //11
            lines.Add("Easy, right ?"); //12
            lines.Add("However, when you will be playing, it's going to be <color=#800080ff>slightly different</color>. You will have a <color=#800080ff>limited amount of moves</color>."); //13
            lines.Add("Plus the spaceship <color=#800080ff>won't move as you swipe</color>. "); //14
            lines.Add("You will need to <color=#800080ff>register all</color> your movements and <color=#800080ff>validate</color> your trajectory. Keep that in mind."); //15
            
            lines.Add("In this game, you will also have to face some obstacles on your way called <color=#800080ff>Steroids</color>. Until now, three of them have been discovered. <color=#800080ff>Bumpy</color>, <color=#800080ff>Ghost</color> and <color=#800080ff>Heavy</color>."); //16
            lines.Add("I forgot what their <color=#800080ff>specificities</color> are but a <color=#800080ff>manual</color> explaining everything is available through the main menu."); //17
            
            lines.Add("They will be in your way to the goal and will move <color=#800080ff>slightly differently</color> than your spaceship."); // 18
            lines.Add("Unlike the spaceship, the steroids can move in 6 directions detailed by <color=#800080ff>numbered dots</color>."); //19
            lines.Add("The position of the numbered dots will be the same, but steroids way be rotated."); //20
            //2, 4, 5, 6, 1, 3
            lines.Add("You will have a <color=#800080ff>panel</color> with steroids movements information. Their movements will be <color=#800080ff>described</color> by numbered dots and steroids will move acordingly."); //21
            lines.Add("The <color=#800080ff>numbered dots</color> look like this on your panel : <sprite=\"AsteroidMoves\" index=0>| <sprite=\"AsteroidMoves\" index=1>|" +
                " <sprite=\"AsteroidMoves\" index=2>| <sprite=\"AsteroidMoves\" index=3>| <sprite=\"AsteroidMoves\" index=4>| <sprite=\"AsteroidMoves\" index=5>"); //22
            lines.Add("Each one of them indicates one direction of the asteroid. Let's see them action."); //23
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=1> :"); //24
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=3> :"); //25
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=4> :"); //26
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=5> :"); //27
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=0> :"); //28
            lines.Add("If you have  <sprite=\"AsteroidMoves\" index=2> :"); //29


            lines.Add("I hope this has been clear. You can <color=#800080ff>replay</color> this guide if needed through the <color=#800080ff>manual</color>."); //30
        }
        if(tutorialScene == 2)
        {
            lines.Add("This is the <color=#800080ff>screen level</color> you will be in. I will go through everything and explain how it <color=#800080ff>works</color>.");
            lines.Add("Above, you have the level number in the middle. You have two buttons. <color=#800080ff> Left </color>to go to the <color=#800080ff> Level Selector </color>and <color=#800080ff> Right </color>to go to the <color=#800080ff> Main Menu</color>.");
            lines.Add("Just below me is the <color=#800080ff>map level</color>. You should be able to recognize your <color=#800080ff>spaceship</color> and our <color=#800080ff>futur goal</color>.");
            lines.Add("This panel allows you to visualize your <color=#800080ff>current plan</color> and <color=#800080ff>steroid's movements</color>.");
            lines.Add("<color=#800080ff>Yours</color> are represented by <color=#800080ff>arrows</color> detailing the directions chosen. When swipipng, it will be registered here.");
            lines.Add("<color=#800080ff>Steroids</color> are reprented by <color=#800080ff> dots between 1 and 6 </color>also detailing how they are going to move.");
            lines.Add("The small panel on the bottom left tells you how <color=#800080ff>many moves</color> you have left. Here, you have a total of <color=#800080ff>6 moves remaining</color>.");
            lines.Add("Below here are three other buttons.");
            lines.Add("The one on the <color=#800080ff>right</color> is to <color=#800080ff>reset</color> the level. If you are lost in your calculations, press it and restart again.");
            lines.Add("The <color=#800080ff>middle</color> one is to <color=#800080ff>cancel your last input</color>.");
            lines.Add("The last one on the <color=#800080ff>left</color> is to <color=#800080ff>start</color> the game. <color=#800080ff>After planning</color> your movements, press this button to see if you got it right.");
            lines.Add("Well I hope my <color=#800080ff>explanations</color> alone were clear ! If not, ask the <color=#800080ff>creators</color> directly or replay the guide through the <color=#800080ff>manual</color>!");
            lines.Add("I wish you good luck and don't get us killed. That would be great!");
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
        int numberOfCharacterDisplayed = 0;
        proceedNext = false;
        speedUp = false;
        hiddenAnimations[index].SetActive(true);
        foreach (char c in lines[index].ToCharArray())
        {
            text.text += c;

            if(numberOfCharacterDisplayed < 10)
            {
                numberOfCharacterDisplayed++;
                
            }
            else
            {
                pressToDisplayText = true;
            }


            if(!speedUp)
            {
                yield return new WaitForSeconds(textSpeed);
            }
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
                yield return new WaitForSeconds(1.0f);
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
        Debug.Log("Loading Manual");
        SceneManager.LoadScene("Manua");
    }

    #endregion

}




