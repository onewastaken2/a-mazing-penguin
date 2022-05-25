using UnityEngine;

public class Rookram : MonoBehaviour
{
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject playerObj;

    private Vector3 originPos;

    private bool isCharging = false;
    private bool resetEnemy = false;
    //private bool hitPlayer = false;

    private float moveSpeed = 2f;
    private float maxSpeed = 10f;
    private float originSpeed;


    private void Awake()
    {
        originPos = transform.position;
        originSpeed = moveSpeed;
    }


    private void Update()
    {
        if(!isCharging && !resetEnemy)
        {
            RaycastHit _hit;

            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _hit, 20f))
            {
                if(_hit.collider.gameObject == playerObj)
                {
                    //gameObject.layer = LayerMask.NameToLayer("Enemy");
                    isCharging = true;
                }
            }
        }

        if(isCharging)
        {
            ChargeForward();
        }

        if(resetEnemy)
        {
            ResetPosition();
        }
    }


    void ChargeForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if(moveSpeed < maxSpeed)
        {
            moveSpeed += 0.10f;
        }
    }


    void ResetPosition()
    {
        if(transform.position == originPos)
        {
            resetEnemy = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, moveSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    hitPlayer = true;
        //}

        if((environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer ||
            (enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            isCharging = false;
            moveSpeed = originSpeed;
            resetEnemy = true;
            //gameObject.layer = LayerMask.NameToLayer("Environment");

            //if (hitPlayer)
            //{
            //    hitPlayer = false;
            //    resetEnemy = true;
            //}
        }
    }
}
