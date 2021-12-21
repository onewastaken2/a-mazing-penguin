using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;          //References game camera for raycasting mouse position
    [SerializeField] private LayerMask groundLayer;   //For detecting when penguin is walking on ground terrain
    [SerializeField] private LayerMask iceLayer;      //For detecting when penguin is sliding on ice terrain
    [SerializeField] private Collider _collider;      //References collider for raycast origin for terrain detection

    private Vector3 clickPos;         //References where cursor is based on mouse click
    private Quaternion clickPosRot;   //References mouse click position and rotates penguin to face it

    private bool isMoving = false;    //For when player has clicked to move
    private bool isSliding = false;   //For when penguin is moving over ice

    private float acceleration = 0;   //Allows for gradual increase in movement speed
    private float walkSpeed = 5;      //How fast penguin walks on ground
    private float slideSpeed = 8;     //How fast penguin slides on ice
    private float turnSpeed = 6;      //How quickly penguin turns to face new given direction


    private void Update()
    {
        RaycastHit hit;
        
        //Player is currently sliding on ice, and will check if penguin reaches ground terrain to stop movement
        if(isSliding)
        {
            if(Physics.Raycast(_collider.bounds.center, transform.TransformDirection(Vector3.down), out hit, 1f, groundLayer))
            {
                if(hit.collider != null)
                {
                    isSliding = false;
                    isMoving = false;
                    acceleration = 0;
                }
            }
        }

        //Player is currently walking on ground terrain, and will check when penguin steps on ice to begin sliding
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

        //Checks if player has pressed RMB, then penguin will move
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, float.MaxValue, groundLayer | iceLayer))
            {
                clickPos = hit.point;
                clickPosRot = Quaternion.LookRotation(clickPos - transform.position);
                isMoving = true;
            }
        }

        //Checks if player has pressed LMB while not moving, then penguin will turn to face new direction
        if(!isMoving)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, float.MaxValue, groundLayer | iceLayer))
                {
                    clickPos = hit.point;
                    clickPosRot = Quaternion.LookRotation(clickPos - transform.position);
                }
            }
            LookTowards();
        }

        //Checks if player has moved the penguin, then penguin will move based on terrain
        if(isMoving && isSliding)
        {
            Slide();
        }
        else if(isMoving)
        {
            Move();
        }
    }


    //Player has pressed RMB, and penguin will turn and move in the direction and position of mouse cursor
    private void Move()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, clickPosRot, turnSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, clickPos, acceleration * Time.deltaTime);

        if(acceleration < walkSpeed)
        {
            acceleration += 0.25f;
        }

        if(transform.position == clickPos)
        {
            isMoving = false;
            acceleration = 0;
        }
    }


    //Penguin is now moving on ice, and will make gradual turns while RMB is pressed and move forward constantly
    private void Slide()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, clickPosRot, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * acceleration * Time.deltaTime;

        if(acceleration < slideSpeed)
        {
            acceleration += 0.25f;
        }
    }


    //Player has pressed LMB, and penguin will turn to face direction of mouse cursor
    private void LookTowards()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, clickPosRot, turnSpeed * Time.deltaTime);
    }
}
