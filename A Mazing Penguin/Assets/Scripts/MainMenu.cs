using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string feedbackForm;     //References feedback form url
    [SerializeField] private Button continueButton;   //For disabling should there be no saved game on file


    private void Awake()
    {
        LoadSavedData();

        if(PlayerData.farthestLevelReached == 0)
        {
            continueButton.interactable = false;
        }
    }


    //Button for loading in player to their game in progress
    //Player will start game in latest level reached
    public void ContinueGame()
    {
        SceneManager.LoadScene(PlayerData.farthestLevelReached);
    }


    //Button for starting a fresh run from the beginning
    //Overrides any saved data, deleting all progress made in a previous game
    public void NewGame()
    {
        PlayerData.farthestLevelReached = 0;
        PlayerData.deathCount = 0;
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


    //Sets up PlayerData information from local saved file
    //All saved information is loaded upon game startup
    private void LoadSavedData()
    {
        PlayerData _data = SaveSystem.LoadGame();
        PlayerData.farthestLevelReached = _data.gameProgress;
        PlayerData.deathCount = _data.totalDeaths;
    }
}
