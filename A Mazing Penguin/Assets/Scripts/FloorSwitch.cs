using UnityEngine;

public class FloorSwitch : MonoBehaviour
{
    [SerializeField] private GameObject doorObj;   //Reference to door THIS floor switch is associated to

    private Door _door;   //Accesses door functions and checks if isLocked

    private int numberOfObjsOnSwitch = 0;   //Tracks the number of objects are currently on top of THIS floor switch

    public bool isPressed = false;   //Whether THIS floor switch is currently being pressed or not


    private void Awake()
    {
        _door = doorObj.GetComponent<Door>();
    }


    //Detects for any number of objects that are pressing this switch down
    //Calls a door function if switch is not already being pressed
    private void OnTriggerEnter(Collider other)
    {
        numberOfObjsOnSwitch++;

        if(!isPressed)
        {
            isPressed = true;
            gameObject.GetComponent<MeshRenderer>().enabled = false;

            if(_door.isLocked)
            {
                _door.LockedDoorOpeningOrClosing();
            }
            else
            {
                _door.DoorOpeningOrClosing();
            }
        }
    }


    //Detects for any number of objects that may have been removed from pressing down this switch
    //This switch is no longer being pressed if no objects are currently on it
    //Calls a LOCKED door function should this switch no longer be pressed
    private void OnTriggerExit(Collider other)
    {
        numberOfObjsOnSwitch--;

        if(numberOfObjsOnSwitch < 1)
        {
            isPressed = false;
            gameObject.GetComponent<MeshRenderer>().enabled = true;

            if(_door.isLocked)
            {
                _door.LockedDoorOpeningOrClosing();
            }
        }
    }
}
