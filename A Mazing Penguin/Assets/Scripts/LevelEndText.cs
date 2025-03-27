using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndText : MonoBehaviour
{
    [SerializeField] private Text flavorText;   //References this text component so text may be written through this script

    private int currentLevelEndText;  //For finding current level so its level end text can be set


    //THIRD BUILD ONLY
    [SerializeField] private string feedbackForm;


    private void Awake()
    {
        currentLevelEndText = SceneManager.GetActiveScene().buildIndex;
        Text();
    }


    //Current level has been set
    //Flavor text for that level will be set
    void Text()
    {
        switch(currentLevelEndText)
        {
            case 1:
                {
                    flavorText.text = "<b>End of level 1!</b>\n- click to continue -";
                    break;
                }
            case 2:
                {
                    flavorText.text = "<b>End of level 2!</b>\n- click to continue -";
                    break;
                }
            case 3:
                {
                    flavorText.text = "<b>End of level 3!</b>\n- click to continue -";
                    break;
                }
            case 4:
                {
                    flavorText.text = "<b>End of level 4!</b>\n- click to continue -";
                    break;
                }
            case 5:
                {
                    flavorText.text = "End of build. Thank you for playing!";
                    break;
                }
        }
    }


    //THIRD BUILD ONLY
    public void Feedback()
    {
        Application.OpenURL(feedbackForm);
    }
}
