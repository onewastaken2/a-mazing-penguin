using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject[] floorSwitchObjs;   //All floor switches associated with THIS door

    private List<FloorSwitch> floorSwitches = new List<FloorSwitch>();   //If isLocked, references floorSwitchObjs[] scripts

    private MeshRenderer meshRef;

    public bool isLocked = false;   //Whether THIS door requires all floor switches to be pressed or not to open


    private void Awake()
    {
        if(isLocked)
        {
            for(int i = 0; i < floorSwitchObjs.Length; i++)
            {
                floorSwitches.Add(floorSwitchObjs[i].GetComponent<FloorSwitch>());
            }
        }
        meshRef = gameObject.GetComponent<MeshRenderer>();
    }


    //Whenever a floor switch is pressed
    //Door will open if closed, and close if open
    public void DoorOpeningOrClosing()
    {
        if(meshRef.enabled == false)
        {
            meshRef.enabled = true;
        }
        else
        {
            meshRef.enabled = false;
        }
    }


    //When a floor switch is pressed, checks if all associated floor switches are being pressed
    //If so, THIS locked door will open
    //Otherwise it will remain closed
    public void LockedDoorOpeningOrClosing()
    {
        int heldSwitches = 0;
        int totalSwitches = floorSwitches.Count;

        foreach(FloorSwitch floorSwitch in floorSwitches)
        {
            if(floorSwitch.isPressed == true)
            {
                heldSwitches++;
            }
        }
        if(heldSwitches == totalSwitches)
        {
            meshRef.enabled = false;
        }
        else
        {
            meshRef.enabled = true;
        }
    }
}
