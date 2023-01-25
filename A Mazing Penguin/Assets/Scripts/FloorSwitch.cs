using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSwitch : MonoBehaviour
{
    //used for waltower colossutank boss

    //used for switches with doors to reference

    //only ever becomes active from other scripts

    [SerializeField] private GameObject playerObj;

    public bool isActive = false;


    private void Update()
    {
        if(isActive)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerObj && isActive)
        {
            isActive = false;
        }
    }
}
