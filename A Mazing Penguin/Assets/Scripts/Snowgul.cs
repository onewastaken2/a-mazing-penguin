using System.Collections;
using UnityEngine;

public class Snowgul : MonoBehaviour
{
    [SerializeField] private LayerMask environmentLayer;   //For detecting if obstacle is in the way while attacking player
    [SerializeField] private GameObject playerObj;         //References penguin to determine IF player is in range of snowgul
    [SerializeField] private GameObject snowballPrefab;    //References snowball prefab for instantiation
    [SerializeField] private Transform spawnPoint;         //Where snowballs are instantiated when shot

    [SerializeField] private float attackRange;   //Determines snowgul attack radius size

    private bool isAttacking = false;   //If player is currently within range for snowgul to attack

    private float turnSpeed = 6f;   //How quickly snowgul turns to face player
    private float originTimer;      //For resetting timer back to its original amount
    private float _timer = 3f;      //How long it takes before shooting a snowball after spotting player


    private void Awake()
    {
        originTimer = _timer;
    }


    private void Update()
    {
        CheckDistanceToPlayer();

        if(isAttacking)
        {
            if(_timer > 0.0f)
            {
                StartCoroutine(SpitSnowball());
            }
        }
        else if(_timer != originTimer)
        {
            ResetTimer();
        }
    }


    //Checks if player has entered snowgul attack radius
    //This is determined based on player position to it
    //Checks if obstacle is in way to determine whether to attack or not
    void CheckDistanceToPlayer()
    {
        float distanceToPlayer = (playerObj.transform.position - transform.position).magnitude;

        if(distanceToPlayer <= attackRange)
        {
            RaycastHit _hit;
            Vector3 playerPos = new Vector3(playerObj.transform.position.x, transform.position.y, playerObj.transform.position.z);
            Ray rayToPlayer = new Ray(transform.position, playerPos - transform.position);

            if(Physics.Raycast(rayToPlayer, out _hit, distanceToPlayer, environmentLayer))
            {
                isAttacking = false;
            }
            else
            {
                isAttacking = true;
            }
        }
        else
        {
            isAttacking = false;
        }
    }


    //Player is within range, and snowgul turns to face penguin
    //Snowgul readies for an attack, and shoots a snowball
    //Snowgul repeats this until player is out of range or out of sight
    IEnumerator SpitSnowball()
    {
        Turn();
        _timer -= Time.deltaTime;

        if(_timer <= 0.0f)
        {
            GameObject snowballObj = Instantiate(snowballPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
            yield return new WaitForSeconds(2f);
            ResetTimer();
        }
    }


    //Snowgul has spotted the penguin
    //It will turn to face direction of player
    void Turn()
    {
        Vector3 lookDirection = playerObj.transform.position - transform.position;
        lookDirection.y = 0;
        Quaternion rotateTo = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, turnSpeed * Time.deltaTime);
    }


    //Snowgul has finished its attack pattern, or player has left snowgul attack radius
    void ResetTimer()
    {
        _timer = originTimer;
    }
}
