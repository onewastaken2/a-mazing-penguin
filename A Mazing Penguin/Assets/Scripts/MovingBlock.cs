using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    private enum Direction   //Determines direction THIS can move: up left, up right, down left, or down right
    {
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    private Direction currentDirection;   //Which direction THIS is now moving based on where player pushed THIS

    [SerializeField] private GameObject pushBlockHitbox;   //References player when pressing E for moving THIS
    [SerializeField] private LayerMask environmentLayer;   //For detecting walls and impassable objects
    [SerializeField] private LayerMask enemyLayer;         //For detecting if enemy is in the way
    [SerializeField] private LayerMask iceLayer;           //For detecting if on ice to begin sliding
    [SerializeField] private Collider _collider;           //References collider for boxcast origin

    private Vector3 currentPos;        //Finds THIS position when player has pushed THIS
    private Vector3 playerPos;         //Finds PLAYER position when player has pushed THIS
    private Vector3 moveToPos;         //Based on playerPos and currentPos determines direction THIS should go
    private Vector3 movingDirection;   //For when THIS is on ice and movement continues beyond moveToPos

    private RaycastHit _hit;   //For boxcast checking ice and collisions

    private bool onIce = false;   //For when moving block is sliding on ice

    private float currentSpeed;      //Keeps track of moving block speed as it quickly slows down
    private float maxSpeed = 6.5f;   //The fastest THIS can go


    private void Awake()
    {
        //Turns off Update() function for this script
        //Is turned on only when moving block is being pushed
        enabled = false;
    }


    private void Update()
    {
        //Player was close enough to push moving block
        //Checks if THIS is currently on top of ice or not
        if(Physics.BoxCast(_collider.bounds.center, new Vector3(0.2f, 0.2f, 0.2f),
        transform.TransformDirection(Vector3.down), out _hit, transform.rotation, 1f, iceLayer))
        {
            onIce = true;
        }
        else
        {
            onIce = false;
        }
        Move();
    }


    //Player has pressed E
    //Finds currentPos and playerPos and sets a direction for THIS to move towards
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == pushBlockHitbox)
        {
            currentPos = transform.position;
            playerPos = pushBlockHitbox.transform.position;

            if(playerPos.x > currentPos.x && playerPos.z < currentPos.z)
            {
                moveToPos = currentPos + new Vector3(-1, 0, 1);
                currentDirection = Direction.UpLeft;
            }
            else if(playerPos.x < currentPos.x && playerPos.z < currentPos.z)
            {
                moveToPos = currentPos + new Vector3(1, 0, 1);
                currentDirection = Direction.UpRight;
            }
            else if(playerPos.x > currentPos.x && playerPos.z > currentPos.z)
            {
                moveToPos = currentPos + new Vector3(-1, 0, -1);
                currentDirection = Direction.DownLeft;
            }
            else
            {
                moveToPos = currentPos + new Vector3(1, 0, -1);
                currentDirection = Direction.DownRight;
            }
            movingDirection = (moveToPos - currentPos).normalized;
            currentSpeed = maxSpeed;
            enabled = true;
        }
    }

    
    //A direction to move to has been set
    //Checks if there are any obstructions in direction THIS is headed
    //Moves a unit over while on ground, and continuously while on ice
    void Move()
    {
        switch(currentDirection)
        {
            case Direction.UpLeft:
                if(Physics.BoxCast(_collider.bounds.center, new Vector3(0.1f, 0.5f, 0.5f),
                transform.TransformDirection(-Vector3.right), out _hit, transform.rotation, 0.6f, enemyLayer | environmentLayer))
                {
                    enabled = false;
                }
                else if(onIce)
                {
                    transform.position += movingDirection * maxSpeed * Time.deltaTime;
                }
                else
                {
                    EaseOut();

                    if(transform.position == moveToPos || transform.position.x < moveToPos.x)
                    {
                        enabled = false;
                    }
                }
                break;

            case Direction.UpRight:
                if(Physics.BoxCast(_collider.bounds.center, new Vector3(0.5f, 0.5f, 0.1f),
                transform.TransformDirection(Vector3.forward), out _hit, transform.rotation, 0.6f, enemyLayer | environmentLayer))
                {
                    enabled = false;
                }
                else if(onIce)
                {
                    transform.position += movingDirection * maxSpeed * Time.deltaTime;
                }
                else
                {
                    EaseOut();

                    if(transform.position == moveToPos || transform.position.x > moveToPos.x)
                    {
                        enabled = false;
                    }
                }
                break;

            case Direction.DownLeft:
                if(Physics.BoxCast(_collider.bounds.center, new Vector3(0.5f, 0.5f, 0.1f),
                transform.TransformDirection(-Vector3.forward), out _hit, transform.rotation, 0.6f, enemyLayer | environmentLayer))
                {
                    enabled = false;
                }
                else if(onIce)
                {
                    transform.position += movingDirection * maxSpeed * Time.deltaTime;
                }
                else
                {
                    EaseOut();

                    if(transform.position == moveToPos || transform.position.x < moveToPos.x)
                    {
                        enabled = false;
                    }
                }
                break;

            case Direction.DownRight:
                if(Physics.BoxCast(_collider.bounds.center, new Vector3(0.1f, 0.5f, 0.5f),
                transform.TransformDirection(Vector3.right), out _hit, transform.rotation, 0.6f, enemyLayer | environmentLayer))
                {
                    enabled = false;
                }
                else if(onIce)
                {
                    transform.position += movingDirection * maxSpeed * Time.deltaTime;
                }
                else
                {
                    EaseOut();

                    if(transform.position == moveToPos || transform.position.x > moveToPos.x)
                    {
                        enabled = false;
                    }
                }
                break;
        }
    }


    //Is currently moving while on ground
    //Moving block begins moving at maxSpeed, then currentSpeed drops to give weight in movement
    void EaseOut()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveToPos, currentSpeed * Time.deltaTime);

        if(currentSpeed > 2.5f)
        {
            currentSpeed -= 0.15f;
        }
    }
}
