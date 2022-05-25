using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Light mainLight;        //References main light for scene transitions
    [SerializeField] private GameObject playerObj;   //References player gameObject for accessing controller

    private PlayerMovement playerMovement;   //For whether power ups are active based on level

    private int currentLevel;   //Keeps track of current level player is on right now
    

    private void Awake()
    {
        mainLight.intensity = 0;
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        LevelSetup();
    }


    private void Start()
    {
        StartCoroutine(FadeOut());
    }


    //Updates player progress if current level is a new level reached
    //Checks if power ups are enabled based on current level
    void LevelSetup()
    {
        if(currentLevel > PlayerData.farthestLevelReached)
        {
            PlayerData.farthestLevelReached++;
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


    //Detects for player, then ends level
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerObj)
        {
            if(SceneManager.GetActiveScene().buildIndex != 5)
            {
                StartCoroutine(FadeIn());
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }


    //Player has reach end of level
    //Scene goes dims to black
    //Loads the next level
    IEnumerator FadeIn()
    {
        float _duration = 2f;
        float _interval = 0.1f;
        
        while(_duration > 0.0f)
        {
            mainLight.intensity -= 0.05f;
            _duration -= _interval;
            yield return new WaitForSeconds(_interval);
        }
        SceneManager.LoadScene(currentLevel + 1);
    }


    //Player has started a new level
    //Scene brightens up from black screen
    IEnumerator FadeOut()
    {
        float _duration = 2f;
        float _interval = 0.1f;

        while(_duration > 0.0f)
        {
            mainLight.intensity += 0.05f;
            _duration -= _interval;
            yield return new WaitForSeconds(_interval);
        }
    }
}
