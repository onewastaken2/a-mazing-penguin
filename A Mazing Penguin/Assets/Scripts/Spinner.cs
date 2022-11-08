using System.Collections;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] GameObject playerObj;                 //References player death so spinner will start moving again
    [SerializeField] private LayerMask environmentLayer;   //For detecting when spinner collides with environment
    [SerializeField] private LayerMask enemyLayer;         //For detecting when spinner collides with spike walls and other spinners
    [SerializeField] private Collider _collider;           //For referencing collider to turn OFF/ON after collisions

    [SerializeField] private float directionX;   //Assigns spinner initial direction to move along x axis
    [SerializeField] private float directionZ;   //Assigns spinner initial direction to move along z axis

    private Player playerRef;   //For referencing whether isRespawning

    private Vector3 startingDirection;   //Adds directionX and directionZ from original position of x and z axes
    private Vector3 moveTo;              //Calculates startingDirection and original position to move towards

    private bool goBack = false;        //For when spinner is now moving opposite of original direction
    private bool isStopped = false;     //This has been boxed in between two objects
    private bool isColliding = false;   //OnTriggerEnter() has occurred, and will not be called again on same frame

    private float currentSpeed;           //Tracks whether going normal speed or has ricocheted
    private float moveSpeed = 5f;         //How fast spinner moves normally
    private float ricochetSpeed = 8.5f;   //How fast spinner moves when having collided


    private void Awake()
    {
        playerRef = playerObj.GetComponent<Player>();
        startingDirection.x = transform.position.x + directionX;
        startingDirection.z = transform.position.z + directionZ;
        startingDirection.y = transform.position.y;
        moveTo = (startingDirection - transform.position).normalized;
        currentSpeed = moveSpeed;
    }


    private void Update()
    {
        if(isStopped)
        {
            if(playerRef.isRespawning)
            {
                isStopped = false;
            }
        }
        else
        {
            Move();

            if(currentSpeed > moveSpeed)
            {
                currentSpeed -= 0.5f;
            }
        }
    }


    //Checks if spinner has recently bounced off something
    //Spinner is either moving in original or opposite direction
    void Move()
    {
        if(goBack)
        {
            transform.position -= moveTo * currentSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += moveTo * currentSpeed * Time.deltaTime;
        }
    }


    //Detects for environment objects and other enemies it can bounce off of
    //Casts BoxCast behind itself to see if something may be blocking it
    //If so stop movement, otherwise ricochet and tell it to move in opposite direction
    private void OnTriggerEnter(Collider other)
    {
        if((environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer
            || (enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            if(isColliding)
            {
                return;
            }
            isColliding = true;
            RaycastHit _hit;

            if(goBack)
            {
                if(Physics.BoxCast(_collider.bounds.center, new Vector3(0.5f, 0.5f, 0.5f),
                    moveTo, out _hit, transform.rotation, 0.35f, environmentLayer | enemyLayer))
                {
                    isStopped = true;
                }
                else
                {
                    goBack = false;
                }
            }
            else
            {
                if(Physics.BoxCast(_collider.bounds.center, new Vector3(0.5f, 0.5f, 0.5f),
                    -moveTo, out _hit, transform.rotation, 0.35f, environmentLayer | enemyLayer))
                {
                    isStopped = true;
                }
                else
                {
                    goBack = true;
                }
            }
            currentSpeed = ricochetSpeed;
            StartCoroutine(TurnColliderOn());
        }
    }


    //Turns collider back on after having collided with environment or enemy
    //Work around to avoid OnTriggerEnter() to occur more than once during one Update()
    IEnumerator TurnColliderOn()
    {
        yield return new WaitForEndOfFrame();
        isColliding = false;
    }
}
