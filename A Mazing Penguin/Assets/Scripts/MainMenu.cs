using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string feedbackForm;   //References feedback form url


    private void Awake()
    {
        LoadGame();
    }


    //Button for loading the game
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    //Button to take player to feedback form
    public void Feedback()
    {
        Application.OpenURL(feedbackForm);
    }


    //Button for closing the application
    public void QuitGame()
    {
        Application.Quit();
    }


    //Button for loading in saved data
    //Starts session at latest level reached
    public void LoadGame()
    {
        PlayerData _data = SaveSystem.LoadGame();
        PlayerData.farthestLevelReached = _data.gameProgress;
        PlayerData.deathCount = _data.totalDeaths;
    }
}
