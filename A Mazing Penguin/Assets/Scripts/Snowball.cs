using System.Collections;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] private LayerMask environmnetLayer;   //For detecting walls and impassable objects

    private float moveSpeed = 10f;   //How fast snowballs move


    private void Awake()
    {
        StartCoroutine(Despawn());
    }


    private void Update()
    {
        Move();
    }


    //Snowball will move in single direction going a constant speed
    void Move()
    {
        Vector3 moveDirection = transform.rotation * Vector3.forward;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }


    //Checks if snowball has hit anything
    //If so, it will be deleted from game scene
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }


    //Guarantees snowball will be deleted from game scene overtime
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
