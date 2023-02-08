using UnityEngine;

public class BossSwitch : MonoBehaviour
{
    public bool isActive = false;   //For activating THIS switch to show it is interactable


    private void Awake()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }


    //Detects if player has pressed THIS switch
    private void OnTriggerEnter(Collider other)
    {
        isActive = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
