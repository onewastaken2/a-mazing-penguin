using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;               //References game camera for raycasting mouse position
    [SerializeField] private LayerMask groundLayer;        //For detecting when penguin is walking on ground terrain
    [SerializeField] private LayerMask iceLayer;           //For detecting when penguin is sliding on ice terrain
    [SerializeField] private LayerMask environmentLayer;   //For detecting walls and impassable objects
    [SerializeField] private LayerMask hazardLayer;        //For detecting enemies and dangers
    [SerializeField] private Collider _collider;           //References collider for raycast origin
    [SerializeField] private GameObject _checkpoint;       //Referencing current level checkpoint

    private Vector3 clickPos;         //References where cursor is based on mouse click
    private Vector3 slideTowards;     //Finds direction to slide to on mouse click without skates
    private Vector3 checkpointPos;    //References checkpoint position for respawn location
    private Quaternion clickPosRot;   //Rotates penguin towards mouse click based on current position with skates

    public bool isMoving = false;     //For when player has clicked to move
    public bool isSliding = false;    //For when penguin is moving over ice
    public bool cannotStop = false;   //For when penguin is on ice without skates and CANNOT turn
    public bool hasSkates = false;    //For when penguin is on ice with skates and CAN turn

    private float acceleration = 0;   //Allows for gradual increase in movement speed
    private float walkSpeed = 5;      //How fast penguin walks on ground
    private float slideSpeed = 8;     //How fast penguin slides on ice


    private void Awake()
    {
        checkpointPos = _checkpoint.transform.position;
    }


    private void Update()
    {
        RaycastHit hit;

        //Player is currently sliding on ice
        //Checks if penguin reaches ground terrain to stop movement
        if(isSliding)
        {
            if(Physics.Raycast(_collider.bounds.center, transform.TransformDirection(Vector3.down), out hit, 1f, groundLayer))
            {
                if(hit.collider != null)
                {
                    isSliding = false;
                    isMoving = false;

                    if(cannotStop)
                    {
                        cannotStop = false;
                    }
                }
            }
        }

        //Player is currently walking on ground terrain
        //Checks when penguin steps on ice to begin sliding
        if(!isSliding)
        {
            if(Physics.Raycast(_collider.bounds.center, transform.TransformDirection(Vector3.down), out hit, 1f, iceLayer))
            {
                if(hit.collider != null)
                {
                    isSliding = true;
                }
            }
        }

        //Player is currently moving the penguin
        //Checks if the penguin is being blocked by something
        if(isMoving)
        {
            if(Physics.SphereCast(_collider.bounds.center, 0.3f, transform.TransformDirection(Vector3.forward), out hit, 0.5f, environmentLayer))
            {
                if(hit.collider != null)
                {
                    isMoving = false;

                    if(cannotStop)
                    {
                        cannotStop = false;
                    }
                }
            }
        }

        //Checks if player has pressed RMB, then penguin will move
        //Penguin will not turn and move in new direction if on ice without skates
        if(Input.GetMouseButtonDown(1) && !cannotStop)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, float.MaxValue, groundLayer | iceLayer))
            {
                clickPos = hit.point;
                clickPosRot = Quaternion.LookRotation(clickPos - transform.position);

                if(!hasSkates || hasSkates && !isMoving)
                {
                    transform.rotation = clickPosRot;
                    slideTowards = (clickPos - transform.position).normalized;
                }
                isMoving = true;
            }
        }

        //Checks if player has pressed LMB while not moving
        //Penguin will turn to face new direction
        if(!isMoving)
        {
            acceleration = 0;

            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, float.MaxValue, groundLayer | iceLayer))
                {
                    transform.rotation = Quaternion.LookRotation(hit.point - transform.position);
                }
            }
        }

        //Checks if player has moved the penguin
        //Penguin will move based on terrain
        if(isMoving && isSliding)
        {
            Slide();
        }
        else if(isMoving)
        {
            Move();
        }
    }


    //Player has pressed RMB
    //Penguin will turn and move in new direction and position of mouse cursor
    void Move()
    {
        transform.rotation = clickPosRot;
        transform.position = Vector3.MoveTowards(transform.position, clickPos, acceleration * Time.deltaTime);

        if(acceleration < walkSpeed)
        {
            acceleration += 0.25f;
        }

        if(transform.position == clickPos)
        {
            isMoving = false;
        }
    }


    //Penguin is now moving forward constantly on ice
    //Makes gradual turns while RMB is pressed with skates
    //Cannot turn at all without skates
    void Slide()
    {
        if(hasSkates)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, clickPosRot, 5.5f * Time.deltaTime);
            transform.position += transform.forward * acceleration * Time.deltaTime;

            if(acceleration < slideSpeed)
            {
                acceleration += 0.25f;
            }
        }
        else
        {
            cannotStop = true;
            transform.position += slideTowards * acceleration * Time.deltaTime;

            if(acceleration < slideSpeed)
            {
                acceleration += 0.25f;
            }
        }
    }


    //Detects for enemies and other dangers
    private void OnTriggerEnter(Collider other)
    {
        if((hazardLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            Respawn();
        }
    }


    //Player has died and is being sent back to checkpoint
    void Respawn()
    {
        isMoving = false;
        transform.position = checkpointPos;
    }
}
