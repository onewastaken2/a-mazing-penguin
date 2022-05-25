using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;   //For when ESC is pressed and game time is stopped

    public GameObject pauseMenuUI;   //For all pause menu contents


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


    //Button for unpausing and returning to the game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }


    //Player has pressed ESC to pause
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
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
        Application.Quit();
    }
}
