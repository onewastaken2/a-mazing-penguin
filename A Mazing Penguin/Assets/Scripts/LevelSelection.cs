﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;     //Keeps track of all level buttons within the level select map

    public int currentLevelSelected;   //Represents the level button that was last pressed by the player


    private void Awake()
    {
        //PlayerData.farthestLevelReached = 3;
        //Debug.Log(PlayerData.farthestLevelReached);
        TurnLevelButtonsOn();
        currentLevelSelected = SceneManager.GetActiveScene().buildIndex;
    }


    //Compares each level button position in array if it is less than farthestLevelReached
    //If so, these buttons become interactable
    void TurnLevelButtonsOn()
    {
        for(int i = 0; i < PlayerData.farthestLevelReached; i++)
        {
            if(levelButtons[i].interactable == false)
            {
                levelButtons[i].interactable = true;
            }
        }
    }


    //Button for starting the current level the player has selected
    public void StartLevel()
    {
        SceneManager.LoadScene(currentLevelSelected);
    }
}