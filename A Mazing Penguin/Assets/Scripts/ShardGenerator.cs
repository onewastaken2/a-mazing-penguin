using System.Collections;
using UnityEngine;

public class ShardGenerator : MonoBehaviour
{
    [SerializeField] private Transform playerPos;   //References player position for when checking player distance from THIS

    private Vector3 spawnPos;   //Position of where shard will begin forming if player is nearby

    private bool createShard = false;   //Shard is being created or is created and currently chasing player

    private float limitRange = 8f;      //The farthest player can be to activate THIS shard
    private float _acceleration = 0f;   //Allows shard to ramp up in speed when chasing player
    private float maxSpeed = 7.5f;      //Fastest shard can move when chasing player


    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
        spawnPos = transform.position;
    }


    private void Update()
    {
        if(!createShard)
        {
            CheckDistanceToPlayer();
        }
        else
        {
            FollowPlayer();
        }
    }


    //Checks if player has stepped close enough to activate THIS
    //If so, shard will begin forming before chasing the player
    void CheckDistanceToPlayer()
    {
        float distanceToPlayer = (playerPos.position - transform.position).magnitude;

        if(distanceToPlayer <= limitRange)
        {
            StartCoroutine(ShardIsForming());
        }
    }


    //Shard is currently moving towards the position of player
    void FollowPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerPos.position, _acceleration * Time.deltaTime);

        if(_acceleration < maxSpeed)
        {
            _acceleration += 0.05f;
        }
    }


    //Checks whether THIS collided with anything, including the player
    //If so, shard will reset and resume checking player distance to THIS
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject)
        {
            _acceleration = 0f;
            GetComponent<MeshRenderer>().enabled = false;
            transform.position = spawnPos;
            createShard = false;
        }
    }


    //Player got near, so shard will begin forming
    IEnumerator ShardIsForming()
    {
        GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(2f);
        createShard = true;
    }
}
