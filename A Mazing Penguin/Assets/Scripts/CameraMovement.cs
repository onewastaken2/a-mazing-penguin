using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject _player;   //References player position for when camera returns to player

    [SerializeField] private int upBound;      //Camera CANNOT be moved beyond this point in NORTH direction
    [SerializeField] private int downBound;    //Camera CANNOT be moved beyond this point in SOUTH direction
    [SerializeField] private int leftBound;    //Camera CANNOT be moved beyond this point in WEST direction
    [SerializeField] private int rightBound;   //Camera CANNOT be moved beyond this point in EAST direction

    private Vector3 camToPlayer;   //Camera moves back to player, with player in center of view

    private float camY;             //References camera position on y to keep from moving on y axis
    private float panSpeed = 10f;   //How fast camera moves when panning


    private void Awake()
    {
        camY = transform.position.y;
        ReturnToPlayer();
    }


    private void Update()
    {
        //Checks if player pressed W to pan up
        if(Input.GetKey(KeyCode.W) && transform.position.z < upBound)
        {
            transform.position += Vector3.forward * panSpeed * Time.deltaTime;
        }

        //Checks if player pressed A to pan left
        if(Input.GetKey(KeyCode.A) && transform.position.x > leftBound)
        {
            transform.position += Vector3.left * panSpeed * Time.deltaTime;
        }

        //Checks if player pressed S to pan down
        if(Input.GetKey(KeyCode.S) && transform.position.z > downBound)
        {
            transform.position += Vector3.back * panSpeed * Time.deltaTime;
        }

        //Checks if player pressed D to pan right
        if(Input.GetKey(KeyCode.D) && transform.position.x < rightBound)
        {
            transform.position += Vector3.right * panSpeed * Time.deltaTime;
        }

        //Checks if player pressed space bar to recenter camera onto penguin
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToPlayer();
        }
    }
    

    //Player has pressed space bar
    //Camera is positioned and centered to penguin
    void ReturnToPlayer()
    {
        camToPlayer = new Vector3(_player.transform.position.x, camY, _player.transform.position.z - 5f);
        transform.position = camToPlayer;
    }
}
