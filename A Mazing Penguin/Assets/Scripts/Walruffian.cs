using UnityEngine;

public class Walruffian : MonoBehaviour
{
    [SerializeField] private GameObject[] pathPoints;   //References however many path points to patrol between

    private int currentPath = 0;    //What path point walruffian is moving to currently

    private float moveSpeed = 5f;     //How fast walruffian patrols
    private float stopTime = 0.08f;   //How long walruffian stops briefly after reaching a path point
    private float _timer;             //Counts down to 0 before walruffian continues to next path point


    private void Update()
    {
        Patrol();
    }


    //Walruffian is moving toward its current path point
    //Checks if walruffian has reached its current path point to stop briefly
    //Walruffian goes onto the next path point in array to move to
    void Patrol()
    {
        if(transform.position == pathPoints[currentPath].transform.position)
        {
            currentPath++;

            if(currentPath >= pathPoints.Length)
            {
                currentPath = 0;
            }
            _timer = stopTime;
        }
        if(_timer > 0.0f)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPath].transform.position, moveSpeed * Time.deltaTime);
        }
    }
}
