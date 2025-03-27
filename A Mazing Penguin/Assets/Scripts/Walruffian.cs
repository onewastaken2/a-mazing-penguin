using System.Collections;
using UnityEngine;

public class Walruffian : MonoBehaviour
{
    [SerializeField] private GameObject[] pathPoints;      //References however many path points to patrol between
    [SerializeField] private LayerMask environmentLayer;   //For detecting when walruffian runs into a moving block

    private bool goBack = false;      //Walruffian is currently going in the reverse patrol pattern
    private bool isBlocked = false;   //Walruffian ran into a moving block and will stop before continuing patrolling

    private int currentPath = 0;    //What path point walruffian is moving to currently

    private float moveSpeed = 4f;     //How fast walruffian patrols
    private float stopTime = 0.15f;   //How long walruffian stops briefly after reaching a path point
    private float _timer;             //Is set and counts down before walruffian continues to next path point


    private void Update()
    {
        if(!isBlocked)
        {
            Patrol();
        }
    }


    //Walruffian is moving toward its current path point
    //Checks if walruffian has reached its current path point to stop briefly
    //Walruffian goes onto the next path point in array to move to
    void Patrol()
    {
        if(transform.position == pathPoints[currentPath].transform.position)
        {
            NewPath();
        }
        if(_timer > 0.0f)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(pathPoints[currentPath].transform.position - transform.position);
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPath].transform.position, moveSpeed * Time.deltaTime);
        }
    }


    //Checks if walruffian is currently patrolling its original pattern or reverse pattern
    //The next path point in array is given based on the above information
    void NewPath()
    {
        if(goBack)
        {
            currentPath--;

            if(currentPath < 0)
            {
                currentPath = pathPoints.Length - 1;
            }
        }
        else
        {
            currentPath++;

            if(currentPath >= pathPoints.Length)
            {
                currentPath = 0;
            }
        }
        _timer = stopTime;
    }


    //Checks IF walruffian has ran into a moving block
    //Walruffian will turn around and patrol the other direction
    private void OnTriggerEnter(Collider other)
    {
        if((environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            if(goBack)
            {
                goBack = false;
            }
            else
            {
                goBack = true;
            }
            NewPath();
            StartCoroutine(HasBeenBlocked());
        }
    }
    

    //Walruffian HAS walked into a moving block and will wait before moving again
    IEnumerator HasBeenBlocked()
    {
        isBlocked = true;
        yield return new WaitForSeconds(1f);
        isBlocked = false;
    }
}
