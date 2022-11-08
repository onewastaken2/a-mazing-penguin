using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;   //References player position for when camera returns to player

    [SerializeField] private int upBound;      //Camera CANNOT be moved beyond this point in NORTH direction
    [SerializeField] private int downBound;    //Camera CANNOT be moved beyond this point in SOUTH direction
    [SerializeField] private int leftBound;    //Camera CANNOT be moved beyond this point in WEST direction
    [SerializeField] private int rightBound;   //Camera CANNOT be moved beyond this point in EAST direction

    private Player playerRef;                   //For referencing whether isRespawning
    private PlayerMovement playerMovementRef;   //For referencing PlayerMovement.enabled

    private Vector3 camToPlayer;                //Camera moves back to player with penguin in center of view
    private Vector3 _velocity = Vector3.zero;   //Represents the current velocity during camera easing, value is modified every function call

    private bool followPlayer = false;   //Checks if space bar is being held so camera panning with WASD cannot occur

    private float camY;                //References camera position on y to keep from moving on y axis
    private float panSpeed = 22f;      //How fast camera moves when panning
    private float smoothTime = 0.4f;   //Time it takes for camera to center to penguin during respawn, value decreases every frame
    private float originSmoothTime;    //Stores original smoothTime value so it resets back AFTER respawning


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
        //Checks if player is not respawning so WASD and space bar can be used freely
        //If respawning, player is locked from controlling camera temporarily
        if(!playerRef.isRespawning)
        {
            MoveCamera();
            CameraFollow();
        }
        else
        {
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
                Input.mousePosition.y > Screen.height - edgeSize)
            {
                moveZ += 1f;
            }
            if(Input.GetKey(KeyCode.A) && transform.position.x > leftBound ||
                Input.mousePosition.x < edgeSize)
            {
                moveX -= 1f;
            }
            if(Input.GetKey(KeyCode.S) && transform.position.z > downBound ||
                Input.mousePosition.y < edgeSize)
            {
                moveZ -= 1f;
            }
            if(Input.GetKey(KeyCode.D) && transform.position.x < rightBound ||
                Input.mousePosition.x > Screen.width - edgeSize)
            {
                moveX += 1f;
            }
            Vector3 camMove = new Vector3(moveX, 0, moveZ).normalized;
            transform.position += camMove * panSpeed * Time.deltaTime;
        }
    }


    //Checks if player HAD pressed space bar to recenter camera to penguin
    //Then checks if space bar is BEING HELD DOWN for manual camera lock to penguin
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
    }


    //Camera snaps and centered to position of penguin
    void CameraLock()
    {
        camToPlayer = new Vector3(playerObj.transform.position.x, camY, playerObj.transform.position.z - 5f);
        transform.position = camToPlayer;
    }


    //Camera will move and follow player gradually
    //If respawning, the move speed of camera will quickly increase
    //While respawning, player can move once camera is centered on penguin
    void CameraEaseToPlayer()
    {
        camToPlayer = new Vector3(playerObj.transform.position.x, camY, playerObj.transform.position.z - 5f);
        transform.position = Vector3.SmoothDamp(transform.position, camToPlayer, ref _velocity, smoothTime);

        if(playerRef.isRespawning)
        {
            smoothTime -= 0.0015f;

            if(transform.position == camToPlayer)
            {
                smoothTime = originSmoothTime;
                playerRef.isRespawning = false;
                playerMovementRef.enabled = true;
            }
        }
    }
}
