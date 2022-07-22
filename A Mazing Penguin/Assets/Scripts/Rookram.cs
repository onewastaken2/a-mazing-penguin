using System.Collections;
using UnityEngine;

public class Rookram : MonoBehaviour
{
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private Collider _collider;
    [SerializeField] private float detectRange;

    private Vector3 originPos;

    public bool isCharging = false;
    public bool resetEnemy = false;
    public bool isBlocked = false;

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
        RaycastHit _hit;

        //
        if(!isBlocked && Physics.BoxCast(_collider.bounds.center, new Vector3(0.5f, 0.5f, 0.5f),
            transform.TransformDirection(Vector3.forward), out _hit, transform.rotation, detectRange))
        {
            if(_hit.collider.gameObject == playerObj)
            {
                resetEnemy = false;
                isCharging = true;
            }
        }

        //
        if(isCharging)
        {
            ChargeForward();
        }
        if(resetEnemy)
        {
            ResetPosition();
        }
    }


    //
    void ChargeForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if(moveSpeed < maxSpeed)
        {
            moveSpeed += 0.10f;
        }
    }


    //
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


    //
    private void OnTriggerEnter(Collider other)
    {
        if (!resetEnemy && (environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer ||
            (enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            StartCoroutine(StopBeforeReset());
        }
        if(resetEnemy && (environmentLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            resetEnemy = false;
        }
    }


    //
    IEnumerator StopBeforeReset()
    {
        isBlocked = true;
        isCharging = false;
        moveSpeed = originSpeed;
        yield return new WaitForSeconds(1f);
        resetEnemy = true;
        yield return new WaitForSeconds(2f);
        isBlocked = false;
    }
}
