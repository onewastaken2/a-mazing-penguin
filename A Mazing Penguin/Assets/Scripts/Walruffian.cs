using UnityEngine;

public class Walruffian : MonoBehaviour
{
    [SerializeField] private GameObject[] pathPoints;   //References however many empty GameObjects to patrol between

    private int currentPath = 0;    //What empty GameObject walruffian is moving to currently
    private float moveSpeed = 5f;   //How fast walruffian patrols
    private float _timer;           //How long walruffian stops briefly after reaching an empty GameObject


    private void Update()
    {
        Patrol();
    }


    //Walruffian is moving toward its current empty GameObject
    //Checks if walruffian has reached its current empty GameObject to stop briefly
    //Walruffian goes onto the next empty GameObject in array to move to
    void Patrol()
    {
        if(transform.position == pathPoints[currentPath].transform.position)
        {
            currentPath++;

            if(currentPath >= pathPoints.Length)
            {
                currentPath = 0;
            }
            _timer = 0.06f;
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
