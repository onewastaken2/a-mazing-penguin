using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;     //References player position for when camera returns to player
    [SerializeField] private GameObject levelEndObj;   //References level end position for when player wants to view location

    [SerializeField] private int upBound;      //Camera CANNOT be moved beyond this point in NORTH direction
    [SerializeField] private int downBound;    //Camera CANNOT be moved beyond this point in SOUTH direction
    [SerializeField] private int leftBound;    //Camera CANNOT be moved beyond this point in WEST direction
    [SerializeField] private int rightBound;   //Camera CANNOT be moved beyond this point in EAST direction

    private Player playerRef;                   //For referencing whether isRespawning
    private PlayerMovement playerMovementRef;   //For referencing PlayerMovement.enabled

    private Vector3 camToObj;                   //Camera moves to either level end position or penguin, in center of view
    private Vector3 _velocity = Vector3.zero;   //Represents the current velocity during camera easing, value is modified every function call

    private bool followPlayer = false;    //Checks if space bar is being held so camera panning with WASD cannot occur
    private bool levelEndCheck = false;   //For if player pressed F to pan and check level end position

    private float camY;                 //References camera position on y to keep from moving on y axis
    private float _acceleration = 0f;   //Allows for gradual increase in speed for manual camera panning
    private float panSpeed = 24f;       //How fast camera moves when panning
    private float smoothTime = 0.4f;    //Value for SmoothDamp when centering camera to penguin or end of level, decreases ever frame
    private float originSmoothTime;     //Stores original smoothTime value so it can be reset
    private float _threshold = 0.05f;   //Value for getting an approximation rather than relying on float precision for SmoothDamp


    private void Awake()
    {
        playerRef = playerObj.GetComponent<Player>();
        playerMovementRef = playerObj.GetComponent<PlayerMovement>();
        camY = transform.position.y;
        originSmoothTime = smoothTime;
        CameraLock();
    }


    private void Update()
    {
        //Checks if player is viewing end of level location with F
        //If not, WASD and space bar can be used freely to move camera
        //If respawning, player is locked from controlling camera temporarily
        if(!playerRef.isRespawning)
        {
            if(levelEndCheck)
            {
                ViewEndOfLevel();
            }
            else
            {
                MoveCamera();
                CameraFollow();
            }
        }
        else
        {
            if(levelEndCheck)
            {
                smoothTime = originSmoothTime;
                levelEndCheck = false;
            }
            _acceleration = 0f;
            CameraEaseToPlayer();
        }
    }


    //Checks if player is NOT holding space bar for camera lock
    //If so, checks if player pressed W to pan up, A to pan left, S to pan down, and D to pan right
    //Checks if mouse cursor is hitting screen borders for alternative panning
    void MoveCamera()
    {
        if(!followPlayer)
        {
            float moveX = 0f;
            float moveZ = 0f;
            float edgeSize = 10f;

            if(Input.GetKey(KeyCode.W) && transform.position.z < upBound ||
                Input.mousePosition.y > Screen.height - edgeSize && transform.position.z < upBound)
            {
                if(_acceleration < panSpeed)
                {
                    _acceleration += 0.75f;
                }
                moveZ += 1f;
            }
            if(Input.GetKey(KeyCode.A) && transform.position.x > leftBound ||
                Input.mousePosition.x < edgeSize && transform.position.x > leftBound)
            {
                if(_acceleration < panSpeed)
                {
                    _acceleration += 0.75f;
                }
                moveX -= 1f;
            }
            if(Input.GetKey(KeyCode.S) && transform.position.z > downBound ||
                Input.mousePosition.y < edgeSize && transform.position.z > downBound)
            {
                if(_acceleration < panSpeed)
                {
                    _acceleration += 0.75f;
                }
                moveZ -= 1f;
            }
            if(Input.GetKey(KeyCode.D) && transform.position.x < rightBound ||
                Input.mousePosition.x > Screen.width - edgeSize && transform.position.x < rightBound)
            {
                if(_acceleration < panSpeed)
                {
                    _acceleration += 0.75f;
                }
                moveX += 1f;
            }
            else if(moveX == 0 & moveZ == 0)
            {
                if(_acceleration > 0)
                {
                    _acceleration -= 0.75f;
                }
            }
            Vector3 camMove = new Vector3(moveX, 0, moveZ).normalized;
            transform.position += camMove * _acceleration * Time.deltaTime;
        }
    }


    //Checks if player HAD pressed space bar to recenter camera to penguin
    //Then checks if space bar is BEING HELD DOWN for manual camera lock to penguin
    //Also checks if player pressed F to pan camera and view level end position
    void CameraFollow()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CameraLock();
        }
        if(Input.GetKey(KeyCode.Space))
        {
            followPlayer = true;
            CameraEaseToPlayer();
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            followPlayer = false;
        }
        if(Input.GetKey(KeyCode.F))
        {
            levelEndCheck = true;
        }
    }


    //Camera snaps and centered to position of penguin
    void CameraLock()
    {
        camToObj = new Vector3(playerObj.transform.position.x, camY, playerObj.transform.position.z - 5f);
        _velocity = Vector3.zero;
        transform.position = camToObj;
    }


    //Camera will move and follow player gradually
    //If respawning, the move speed of camera will quickly increase
    //While respawning, player can move once camera is centered on penguin
    void CameraEaseToPlayer()
    {
        camToObj = new Vector3(playerObj.transform.position.x, camY, playerObj.transform.position.z - 5f);
        transform.position = Vector3.SmoothDamp(transform.position, camToObj, ref _velocity, smoothTime);

        if(playerRef.isRespawning)
        {
            smoothTime -= 0.0015f;

            if((transform.position - camToObj).sqrMagnitude < _threshold)
            {
                smoothTime = originSmoothTime;
                playerRef.isRespawning = false;
                playerMovementRef.enabled = true;
            }
        }
    }


    //Checks if player HAD pressed F to center camera to end of level
    void ViewEndOfLevel()
    {
        camToObj = new Vector3(levelEndObj.transform.position.x, camY, levelEndObj.transform.position.z - 5f);
        transform.position = Vector3.SmoothDamp(transform.position, camToObj, ref _velocity, smoothTime);

        if(levelEndCheck)
        {
            smoothTime -= 0.0015f;

            if((transform.position - camToObj).sqrMagnitude < _threshold)
            {
                smoothTime = originSmoothTime;
                levelEndCheck = false;
            }
        }
    }
    
    //add pan effect rather than snap with CameraLock()
    //add CameraLock() option for player during respawn to move freely more quickly
}
