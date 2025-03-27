using System.Collections;
using UnityEngine;

public class Rookram : MonoBehaviour
{
    [SerializeField] private LayerMask environmentLayer;   //For detecting objects that stop movement
    [SerializeField] private LayerMask enemyLayer;         //For detecting other enemies to stop movement
    [SerializeField] private GameObject playerObj;         //References player for checking if player is in sight
    [SerializeField] private Collider _collider;           //Referencing collider to be used for BoxCasting for player
    [SerializeField] private float detectRange;            //Variable that represents THIS Rookram BoxCast distance

    private Vector3 originPos;   //References Rookram original position upon start of level

    private bool isCharging = false;   //Player has stepped in front of THIS Rookram, and so it is now charging
    private bool resetEnemy = false;   //Rookram is currently moving back to its starting position
    private bool isBlocked = false;    //Rookram has just hit a non-player object and is now stopped

    private float moveSpeed = 2f;   //Represents Rookram starting speed before ramping up
    private float maxSpeed = 7f;    //Represents Rookram maximum speed during charging forward
    private float originSpeed;      //For resetting moveSpeed back to its original state


    private void Awake()
    {
        originPos = transform.position;
        originSpeed = moveSpeed;
    }


    private void Update()
    {
        RaycastHit _hit;

        //If player steps in front of THIS Rookram, it will begin charging
        if(!isBlocked && Physics.BoxCast(_collider.bounds.center, new Vector3(0.5f, 0.5f, 0.5f),
            transform.TransformDirection(Vector3.forward), out _hit, transform.rotation, detectRange))
        {
            if(_hit.collider.gameObject == playerObj)
            {
                resetEnemy = false;
                isCharging = true;
            }
        }

        //Checks if it can charge to charge in direction it is facing
        //Checks if it has collided with environment to reset position
        if(isCharging)
        {
            ChargeForward();
        }
        if(resetEnemy)
        {
            ResetPosition();
        }
    }


    //Player has moved within detectRange of this Rookram, and is now rapidly moving forward
    void ChargeForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if(moveSpeed < maxSpeed)
        {
            moveSpeed += 0.10f;
        }
    }


    //Begins moving backwards into its original position at start of level
    void ResetPosition()
    {
        if(transform.position == originPos)
        {
            resetEnemy = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, moveSpeed * Time.deltaTime);
        }
    }


    //Rookram will stop and reset position after it has collided with environment
    //Rookram will stop resetting if it backs up into a moving block while it is resetting position
    private void OnTriggerEnter(Collider other)
    {
        if(!resetEnemy && (environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer ||
            (enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            StartCoroutine(StopBeforeReset());
        }
        if(resetEnemy && (environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            resetEnemy = false;
        }
    }


    //Rookram has been blocked and has stopped moving
    //Stops briefly before resetting back to its starting position
    //Momentarily, Rookram will not charge again during the start of resetting
    IEnumerator StopBeforeReset()
    {
        isBlocked = true;
        isCharging = false;
        moveSpeed = originSpeed;
        yield return new WaitForSeconds(1f);
        resetEnemy = true;
        yield return new WaitForSeconds(2f);
        isBlocked = false;
    }
}
