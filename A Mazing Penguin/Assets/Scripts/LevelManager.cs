using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;   //References player gameObject for accessing controller
    [SerializeField] private Image screenFade;       //Black image for level transition fade

    private PlayerMovement playerMovement;   //For whether power ups are active based on level

    private int currentLevel;   //Keeps track of current level player is on right now

    public GameObject levelEndScreen;   //For referencing when to enable end level text

    public GameObject levelSelectionScreen;                 //THIRD BUILD ONLY
    private LevelSelection levelSelectionRef;               //THIRD BUILD ONLY
    

    private void Awake()
    {
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        //playerMovement.enabled = true;
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        LevelSetup();

        //THIRD BUILD ONLY
        levelSelectionRef = levelSelectionScreen.GetComponent<LevelSelection>();
    }


    private void Start()
    {
        StartCoroutine(FadeOut());
    }


    //Updates player progress if current level is a new level reached
    //Checks if power ups are enabled based on current level
    void LevelSetup()
    {
        if(currentLevel >= 2)
        {
            playerMovement.canPushBlocks = true;
        }
        if(currentLevel >= 3)
        {
            playerMovement.hasSkates = true;
        }
        else
        {
            playerMovement.hasSkates = false;
        }
    }


    //THIRD BUILD ONLY
    public void LevelEndToLevelSelect()
    {
        levelEndScreen.SetActive(false);
        levelSelectionScreen.SetActive(true);
        levelSelectionRef.TurnLevelButtonsOn();
    }


    //Level end text is displayed
    //Player can click to continue to next level
    public void LevelExit()
    {
        levelEndScreen.SetActive(false);
        StartCoroutine(FadeIn());
    }


    //Detects for if player has reached end of level
    //Level end text appears for player to read
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerObj)
        {
            if(SceneManager.GetActiveScene().buildIndex != 5)
            {
                if(currentLevel + 1 > PlayerData.farthestLevelReached)
                {
                    PlayerData.farthestLevelReached++;
                }
                levelEndScreen.SetActive(true);
                playerMovement.enabled = false;
            }
            else
            {
                levelEndScreen.SetActive(true);
                playerMovement.enabled = false;
            }
        }
    }


    //Player reached end of level
    //Scene goes dims to black
    //Loads the next level
    public IEnumerator FadeIn()
    {
        float _duration = 2f;
        float _interval = 0.07f;

        Color _color = screenFade.color;
        float newAlpha = 0f;

        while(_duration > 0.0f)
        {
            newAlpha += 0.05f;
            _color.a = newAlpha;
            screenFade.color = _color;

            _duration -= _interval;
            yield return new WaitForSeconds(_interval);
        }
        //SceneManager.LoadScene(currentLevel + 1);
    }


    //Player has started a new level
    //Scene transitions from black screen
    IEnumerator FadeOut()
    {
        float _duration = 2f;
        float _interval = 0.07f;

        Color _color = screenFade.color;
        float newAlpha = 1f;

        while(_duration > 0.0f)
        {
            newAlpha -= 0.05f;
            _color.a = newAlpha;
            screenFade.color = _color;

            _duration -= _interval;
            yield return new WaitForSeconds(_interval);
        }
    }
}
