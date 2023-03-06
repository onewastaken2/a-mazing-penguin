using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;               //References game camera for raycasting mouse position
    [SerializeField] private LayerMask clickableLayer;     //For detecting player input mouse positions
    [SerializeField] private LayerMask groundLayer;        //For detecting when penguin is walking on ground terrain
    [SerializeField] private LayerMask iceLayer;           //For detecting when penguin is sliding on ice terrain
    [SerializeField] private LayerMask fastIceLayer;       //For detecting when penguin is sliding on FAST ice terrain
    [SerializeField] private LayerMask reverseIceLayer;    //For detecting when penguin is sliding on REVERSE ice terrain
    [SerializeField] private LayerMask environmentLayer;   //For detecting walls and impassable objects
    [SerializeField] private Collider _collider;           //References collider for raycast origin
    [SerializeField] private GameObject pushBlockHitbox;   //References hitbox for pushing moving blocks
    [SerializeField] private GameObject clickImage;        //References sprite image for a clicking animation

    private Vector3 clickPos;         //References where cursor is based on mouse click
    private Vector3 slideTowards;     //Finds direction to slide to on mouse click without skates
    private Quaternion clickPosRot;   //Rotates penguin towards mouse click based on current position

    private List<Vector3> savedClicks = new List<Vector3>();   //Used for storing queued shift-click positions when on ground

    public bool isMoving = false;     //For when player has clicked to move
    public bool isSliding = false;    //For when penguin is moving over ice
    public bool cannotStop = false;   //For when penguin is on ice without skates and CANNOT turn
    public bool hasSkates = false;    //For when penguin is on ice with skates and CAN turn
    public bool inReverse = false;    //For when penguin is on REVERSE ice and clickPos is calculated to be inverted

    private float _acceleration = 0f;   //Allows for gradual increase in movement speed
    private float walkSpeed = 4f;       //How fast penguin walks on ground
    private float slideSpeed = 8f;      //How fast penguin slides on ice
    private float turnSpeed = 5f;       //How quickly penguin turns while sliding WITH skates
    private float fastSlide = 12f;      //How fast penguin slides on FAST ice
    private float originSlideSpeed;     //For resetting slideSpeed when no longer on fast ice


    private void Awake()
    {
        pushBlockHitbox.SetActive(false);
        originSlideSpeed = slideSpeed;
    }


    private void Update()
    {
        RaycastHit _hit;

        //Player is currently sliding on ice
        //Checks if penguin is on fast ice to slide faster
        //Checks if penguin has reached ground terrain to stop movement
        if(isSliding)
        {
            if(Physics.Raycast(_collider.bounds.center, transform.TransformDirection(Vector3.down), out _hit, 1f, fastIceLayer))
            {
                slideSpeed = fastSlide;
            }
            else
            {
                slideSpeed = originSlideSpeed;
            }
            if(Physics.Raycast(_collider.bounds.center, transform.TransformDirection(Vector3.down), out _hit, 1f, reverseIceLayer))
            {
                inReverse = true;
            }
            else
            {
                inReverse = false;
            }
            if(Physics.Raycast(_collider.bounds.center, transform.TransformDirection(Vector3.down), out _hit, 1f, groundLayer))
            {
                if(_hit.collider != null)
                {
                    savedClicks.Clear();
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
            if(Physics.Raycast(_collider.bounds.center, transform.TransformDirection(Vector3.down), out _hit, 1f, iceLayer | fastIceLayer | reverseIceLayer))
            {
                if(_hit.collider != null)
                {
                    savedClicks.Clear();
                    isSliding = true;
                }
            }
        }

        //Player is currently moving the penguin
        //Checks if the penguin is being blocked by something
        if(isMoving)
        {
            if(Physics.SphereCast(_collider.bounds.center, 0.3f, transform.TransformDirection(Vector3.forward), out _hit, 0.5f, environmentLayer))
            {
                if(_hit.collider != null)
                {
                    savedClicks.Clear();
                    isMoving = false;

                    if(cannotStop)
                    {
                        cannotStop = false;
                    }
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
        else
        {
            _acceleration = 0;
        }

        //Checks if player has pressed RMB, then penguin will move
        //Move commands may be queued by the player when holding SHIFT when walking
        //Penguin will not turn and move in new direction if on ice without skates
        if (!cannotStop && Input.GetMouseButtonDown(1))
        {
            Ray _ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(_ray, out _hit, float.MaxValue, clickableLayer))
            {
                if(!isSliding && Input.GetKey(KeyCode.LeftShift))
                {
                    savedClicks.Add(_hit.point);
                }
                else
                {
                    savedClicks.Clear();
                    savedClicks.Add(_hit.point);
                }
                clickPos = savedClicks[0];
                StartCoroutine(ClickAnimation());

                if(inReverse)
                {
                    InverseDirection();
                }
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
        if(!isMoving && Input.GetMouseButtonDown(0))
        {
            Ray _ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(_ray, out _hit, float.MaxValue, clickableLayer))
            {
                savedClicks.Add(_hit.point);
                clickPos = savedClicks[0];
                transform.rotation = Quaternion.LookRotation(_hit.point - transform.position);
            }
            StartCoroutine(ClickAnimation());
            savedClicks.Clear();
        }

        //Checks if player has pressed E for pushing movable blocks
        //Penguin will change in one of FOUR directions based on current facing direction
        if(!isMoving || !isSliding && isMoving || isSliding && !isMoving)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                isMoving = false;
                SetRotation();
                StartCoroutine(PushBlock());
            }
        }
    }


    //Player has pressed RMB while on ground
    //Penguin will turn and move in new direction and position of mouse cursor
    //If player pressed RMB while holding SHIFT, penguin will continue to move to next click position
    //Penguin will stop moving if player pressed F
    void Move()
    {
        transform.rotation = clickPosRot;
        transform.position = Vector3.MoveTowards(transform.position, clickPos, _acceleration * Time.deltaTime);

        if(_acceleration < walkSpeed)
        {
            _acceleration += 0.25f;
        }
        if(transform.position == clickPos)
        {
            savedClicks.RemoveAt(0);

            if(savedClicks.Count > 0)
            {
                clickPos = savedClicks[0];
                clickPosRot = Quaternion.LookRotation(clickPos - transform.position);
            }
            else
            {
                isMoving = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            savedClicks.Clear();
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
            transform.rotation = Quaternion.Slerp(transform.rotation, clickPosRot, turnSpeed * Time.deltaTime);
            transform.position += transform.forward * _acceleration * Time.deltaTime;

            if(_acceleration < slideSpeed)
            {
                _acceleration += 0.25f;
            }
        }
        else
        {
            cannotStop = true;
            transform.position += slideTowards * _acceleration * Time.deltaTime;

            if(_acceleration < slideSpeed)
            {
                _acceleration += 0.25f;
            }
        }
    }


    //Penguin is currently sliding on reverse ice
    //Penguin will slide toward the opposite direction of where player clicked
    void InverseDirection()
    {
        float xDistance = Mathf.Abs(clickPos.x - transform.position.x);
        float zDistance = Mathf.Abs(clickPos.z - transform.position.z);

        if(clickPos.x < transform.position.x)
        {
            clickPos.x = transform.position.x + xDistance;
        }
        else
        {
            clickPos.x = transform.position.x - xDistance;
        }
        if(clickPos.z < transform.position.z)
        {
            clickPos.z = transform.position.z + zDistance;
        }
        else
        {
            clickPos.z = transform.position.z - zDistance;
        }
    }


    //Player has pressed E
    //Finds current rotation and turns to face: up left, up right, down left, or down right
    //Penguin facing direction coincides with moving block movements
    void SetRotation()
    {
        Quaternion faceDirection;
        Vector3 rotateTo = new Vector3(0, 0, 0);
        float currentRotation = transform.eulerAngles.y;

        if(currentRotation >= 270f && currentRotation <= 360f)
        {
            rotateTo = transform.position + new Vector3(-1, 0, 1);
        }
        else if(currentRotation >= 0f && currentRotation <= 90f)
        {
            rotateTo = transform.position + new Vector3(1, 0, 1);
        }
        else if(currentRotation >= 180f && currentRotation < 270f)
        {
            rotateTo = transform.position + new Vector3(-1, 0, -1);
        }
        else
        {
            rotateTo = transform.position + new Vector3(1, 0, -1);
        }
        faceDirection = Quaternion.LookRotation(rotateTo - transform.position);
        transform.rotation = faceDirection;
    }


    //Player has pressed E and facing direction has changed
    //Hitbox for triggering moving block is turned on briefly
    IEnumerator PushBlock()
    {
        pushBlockHitbox.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        pushBlockHitbox.SetActive(false);
    }


    //Player has pressed RMB
    //An animation will play briefly for visual feedback where player clicked
    IEnumerator ClickAnimation()
    {
        Vector3 clickAnimPos;
        clickAnimPos.x = savedClicks[savedClicks.Count - 1].x;
        clickAnimPos.y = 5f;
        clickAnimPos.z = savedClicks[savedClicks.Count - 1].z - 2.85f;
        clickImage.transform.position = clickAnimPos;
        Color _temp = clickImage.GetComponent<SpriteRenderer>().color;
        _temp.a = 1f;
        clickImage.GetComponent<SpriteRenderer>().color = _temp;
        yield return new WaitForSeconds(0.05f);
        _temp.a = 0.60f;
        clickImage.GetComponent<SpriteRenderer>().color = _temp;
        yield return new WaitForSeconds(0.05f);
        _temp.a = 0.30f;
        clickImage.GetComponent<SpriteRenderer>().color = _temp;
        yield return new WaitForSeconds(0.05f);
        _temp.a = 0f;
        clickImage.GetComponent<SpriteRenderer>().color = _temp;
    }


    //player death does not clear savedClicks list
    //check if all savedClicks.Clear() are necessary
}
