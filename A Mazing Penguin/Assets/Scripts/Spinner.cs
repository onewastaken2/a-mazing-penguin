﻿using System.Collections;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] private LayerMask environmentLayer;   //For detecting when spinner collides with environment
    [SerializeField] private LayerMask enemyLayer;         //For detecting when spinner collides with spike walls and other spinners
    [SerializeField] private Collider _collider;           //For referencing collider to turn OFF/ON after collisions
    [SerializeField] private bool goBack = false;          //For when spinner is now moving opposite of original direction
    [SerializeField] private float directionX;             //Assigns spinner initial direction to move along x axis
    [SerializeField] private float directionZ;             //Assigns spinner initial direction to move along z axis
    [SerializeField] private float currentSpeed;           //Tracks whether going normal speed or has ricocheted

    private Vector3 startingDirection;   //Adds directionX and directionZ from original position of x and z axes
    private Vector3 moveTo;              //Calculates startingDirection and original position to move towards

    private float ricochetSpeed = 8.5f;   //How fast spinner moves when having collided
    private float moveSpeed = 5f;         //How fast spinner moves normally
    

    private void Awake()
    {
        startingDirection.x = transform.position.x + directionX;
        startingDirection.z = transform.position.z + directionZ;
        startingDirection.y = transform.position.y;
        moveTo = (startingDirection - transform.position).normalized;
        currentSpeed = moveSpeed;
    }


    private void Update()
    {
        Move();
        transform.Rotate(0, 180 * Time.deltaTime, 0);
    }


    //Checks if spinner has recently bounced off something
    //Spinner is either moving in original or opposite direction
    void Move()
    {
        if(currentSpeed > moveSpeed)
        {
            currentSpeed -= 0.5f;
        }
        if(goBack)
        {
            transform.position -= moveTo * currentSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += moveTo * currentSpeed * Time.deltaTime;
        }
    }


    //Detects for environment objects and other spinners to bounce off of
    private void OnTriggerEnter(Collider other)
    {
        if((environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer
            || (enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            currentSpeed = ricochetSpeed;

            if(goBack)
            {
                goBack = false;
            }
            else
            {
                goBack = true;
            }
            StartCoroutine(TurnIsTriggerOffOn());
        }
    }


    //Briefly switches isTrigger OFF/ON after colliding with environment or an enemy
    //Work around to avoid OnTriggerEnter() to occur more than once during one Update()
    IEnumerator TurnIsTriggerOffOn()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        _collider.enabled = true;
        _collider.isTrigger = true;
    }
}
