using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndText : MonoBehaviour
{
    [SerializeField] private Text flavorText;   //References this text component so text may be written through this script

    private int currentLevelEndText;  //For finding current level so its level end text can be set


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
                    flavorText.text = "end of level 1";
                    break;
                }
            case 2:
                {
                    flavorText.text = "end of level 2";
                    break;
                }
        }
    }


}
