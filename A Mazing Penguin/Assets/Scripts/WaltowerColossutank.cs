using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaltowerColossutank : MonoBehaviour
{
    //player aproaches
    //camera pans to boss - boss and a switch activate

    //boss is turning back and forth and shooting randomly from its two arms
    //player must step onto switch, causing boss to face away and reveal button on its back
    //there is a certain window of time to do this

    //button is hit
    //boss is turning and shooting more quickly
    //now two switches appear
    
    //player manages to step onto the two switches, and button is hit
    //boss now follows player, and alternates shooting from its arms and now mouth
    //three switches appear for the player to step on
    
    //player hits button for the third time, and defeats the boss

    //----------- QUESTIONS --------------------

    //do bosses reset upon player death? yes
    //do players get a certain number of tries on a boss level (lives)? no
    

    //----------- PSEUDOCODE BELOW -----------------

    //switch case check for which stage
    //use coroutines for stages 1 and 2
    //stage three face and follow player

    //int to check number of button presses

    //Update() for boss stage

    //instantiate giant snowballs
    //reference the three spawn positions: two arms and mouth

    //boss is in a different state (stage) when turning its back to player
}
