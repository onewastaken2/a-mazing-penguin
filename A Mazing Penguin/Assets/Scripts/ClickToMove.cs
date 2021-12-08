using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    //move player with mouse click
    //player moves across x and z axes

    //player speed
    //player position
    //cursor position
    [SerializeField] private Camera mainCam;

    [SerializeField] private LayerMask groundLayer;

    private float speed = 6;
    private Vector3 targetPos;

    private bool isMoving = false;


    private void Update()
    {
        //RMB to move
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, groundLayer))
            {
                targetPos = raycastHit.point;

                isMoving = true;
            }
        }

        if(isMoving)
        {
            Move();
        }

        //LMB to look in direction
        if(!isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, groundLayer))
                {
                    targetPos = raycastHit.point;


                    transform.rotation = Quaternion.LookRotation(targetPos - transform.position);

                    //Quaternion rotateTowards = Quaternion.LookRotation(transform.position - targetPos);
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetPos.rotation, speed * Time.deltaTime);

                    //transform.rotation = Quaternion.Lerp(transform.rotation, targetPos.rotation, Time.time * speed);
                }
            }
        }
    }


    void Move()
    {
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if(transform.position == targetPos)
        {
            isMoving = false;
        }
    }
}
