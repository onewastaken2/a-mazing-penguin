using System.Collections;
using UnityEngine;

public class IcicleGenerator : MonoBehaviour
{
    [SerializeField] private GameObject icicleObj;        //Reference to icicle object position, rigidbody, and if active
    [SerializeField] private GameObject shadowImage;      //Reference to shadow being cast from icicle as it falls nearer
    [SerializeField] private GameObject shatterZone;      //A sphere collider to be turned on and off during icicle shattering
    [SerializeField] private float spawnTimer = 0;        //Variable for how long between when icicle shatters and when it respawns
    [SerializeField] private float startDelayTimer = 0;   //Variable for how long before THIS generates icicles at start of level

    private Rigidbody icicleRigidbody;   //For referencing icicle object physics components
    private Vector3 rbVelocity;          //References rigidbody so that its velocity is reset after falling
    private Vector3 spawnPos;            //Position of where icicle spawns, set to 25f in Awake()

    private float originSpawnTimer;         //Variable for resetting spawnTimer back after icicle despawning
    private float minDistance = 0.1f;       //Closest distance between icicle and shadow before shadow size STOPS increasing
    private float maxDistance;              //Farthest distance between icicle and shadow before shadow size STARTS increasing
    private float minDistanceSize = 7f;     //Shadow will be THIS LARGE in scale when icicle is nearest to ground
    private float maxDistanceSize = 0.5f;   //Shadow will be THIS SMALL in scale when icicle is farthest to ground


    private void Awake()
    {
        icicleRigidbody = icicleObj.GetComponent<Rigidbody>();
        rbVelocity.y = 0f;
        spawnPos = new Vector3(transform.position.x, transform.position.y + 25, transform.position.z);
        originSpawnTimer = spawnTimer;
        spawnTimer = startDelayTimer;
        maxDistance = spawnPos.y - 4f;
    }


    private void Update()
    {
        //While icicle is free falling, distance between it and shadow is calculated
        //Otherwise spawn timer runs to reset the loop
        if(icicleObj.activeSelf)
        {
            CheckIcicleDistanceToGround();
        }
        else
        {
            if(spawnTimer <= 0)
            {
                SpawnIcicle();
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }
        }
    }


    //Spawn timer finished
    //Icicle object repositioned
    //Icicle object and shadow object enabled
    void SpawnIcicle()
    {
        icicleObj.transform.position = spawnPos;
        icicleObj.SetActive(true);
        shadowImage.SetActive(true);
    }


    //Icicle has hit the ground
    //Icicle object disabled, position and velocity reset
    //Shadow object disabled, size reset
    //Spawn timer reset
    void ResetIcicle()
    {
        icicleObj.SetActive(false);
        shadowImage.SetActive(false);
        shadowImage.transform.localScale = Vector3.one * maxDistanceSize;
        icicleRigidbody.velocity = rbVelocity;
        spawnTimer = originSpawnTimer;
    }


    //Measures distance between icicle to ground and alters shadow cast size
    //Takes a 0 to 1 value of the furthest distance = 0, to the closest distance = 1
    //(18 - 21) / (0.1 - 21) = 0.1435 is a far distance; (4 - 21) / (0.1 - 21) = 0.8134 is a close distance
    //Converts floats into Vector3 so that transform.scale can be used for lerping
    //Shadow size will gradually increase the closer icicle gets to its position
    void CheckIcicleDistanceToGround()
    {
        float _distance = (icicleObj.transform.position - shadowImage.transform.position).magnitude;
        float _norm = (_distance - maxDistance) / (minDistance - maxDistance);
        _norm = Mathf.Clamp01(_norm);
        Vector3 minDistanceScale = Vector3.one * minDistanceSize;
        Vector3 maxDistanceScale = Vector3.one * maxDistanceSize;
        shadowImage.transform.localScale = Vector3.Lerp(maxDistanceScale, minDistanceScale, _norm);
    }


    //Checks for when icicle object has collided with THIS
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == icicleObj)
        {
            ResetIcicle();
            StartCoroutine(IcicleShatter());
        }
    }


    //Icicle object has hit THIS collider
    //An enemy hitbox is briefly enabled during this
    IEnumerator IcicleShatter()
    {
        shatterZone.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        shatterZone.SetActive(false);
    }
}
