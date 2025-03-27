using System.Collections;
using UnityEngine;

public class ShardGenerator : MonoBehaviour
{
    [SerializeField] private Transform playerRef;   //References player distance from THIS
    [SerializeField] private LayerMask pitLayer;    //References pits so shards can ignore colliders

    [SerializeField] private float limitRange;   //The farthest player can be to activate THIS shard
    [SerializeField] private float maxSpeed;     //Fastest shard can move when chasing player

    private Vector3 spawnPos;    //Position of where shard will begin forming if player is nearby

    private bool createShard = false;   //Shard is being created or is created and currently chasing player

    private float distanceToPlayer;     //Checks how close player is to shard spawn point
    private float _acceleration = 0f;   //Allows shard to ramp up in speed when chasing player


    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
        spawnPos = transform.position;
    }


    private void Update()
    {
        distanceToPlayer = (playerRef.transform.position - transform.position).magnitude;

        if(!createShard)
        {
            CheckPlayerToShard();
        }
        else
        {
            FollowPlayer();
        }
    }


    //Checks if player has stepped close enough to activate THIS
    //If so, shard will begin forming before chasing the player
    void CheckPlayerToShard()
    {
        if(distanceToPlayer <= limitRange)
        {
            enabled = false;
            StartCoroutine(ShardIsForming());
        }
    }


    //Shard is currently moving towards the position of player
    void FollowPlayer()
    {
        Vector3 playerPos = new Vector3(playerRef.transform.position.x, transform.position.y, playerRef.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, playerPos, _acceleration * Time.deltaTime);

        if(_acceleration < maxSpeed)
        {
            _acceleration += 0.01f;
        }
        if(distanceToPlayer > limitRange * 2)
        {
            ShardReset();
        }
    }


    //Shard has either hit something or player is well beyond its range
    void ShardReset()
    {
        createShard = false;
        _acceleration = 0f;
        GetComponent<MeshRenderer>().enabled = false;
        transform.position = spawnPos;
    }


    //Checks whether THIS collided with anything except pit colliders
    //If so, shard will reset and resume checking player distance from spawnPos
    private void OnTriggerEnter(Collider other)
    {
        if((pitLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            return;
        }
        else
        {
            ShardReset();
        }
    }


    //Player got near, so shard will begin forming
    IEnumerator ShardIsForming()
    {
        GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(1.5f);
        createShard = true;
        enabled = true;
    }
}
