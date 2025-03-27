using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;   //For when ESC is pressed and game time is stopped

    public GameObject pauseMenuUI;   //For all pause menu contents

    [SerializeField] private GameObject levelSelectionScreen;   //THIRD BUILD ONLY


    private void Update()
    {
        //Player has pressed ESC to pause/unpause the game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    //Player has pressed ESC to pause
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }


    //Button for unpausing and returning to the game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }


    //PUT FADE IN HERE?
    //Button for restarting the current level
    public void Restart()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    //THIRD BUILD ONLY
    public void LevelSelect()
    {
        levelSelectionScreen.SetActive(true);
    }


    //Button for returning to the main menu
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(0);
    }


    //Button for closing the application
    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }


    //level select button
    //enable level select gameObject

    //separate level select script
    //access isPaused bool for quick exit w/ ESC
    //access level deaths and total deaths to display as text
    //script serves to store the various levels as interactable buttons
    //^--have a horizontal scrollable image where buttons and graphics will go
    //^--buttons are enabled only if player has reached those levels

    //get farthestLevelReached, enable buttons based on this
    //get level deaths and totalDeaths, display them through text
}
