using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string feedbackForm;   //References feedback form url


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
}
