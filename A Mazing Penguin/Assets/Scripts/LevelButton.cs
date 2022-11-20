using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectionObj;   //References gameObject for accessing currentLevelSelected variable
    [SerializeField] private int levelNumber;                //The level THIS button represents on the level select map

    private LevelSelection levelSelectionRef;   //For updating currentLevelSelected based on THIS button levelNumber


    private void Awake()
    {
        levelSelectionRef = levelSelectionObj.GetComponent<LevelSelection>();
    }


    //Button to display THIS levelNumber information
    //The button for starting a level has been updated to start THIS levelNumber
    public void LevelChosen()
    {
        levelSelectionRef.currentLevelSelected = levelNumber;
    }
}
