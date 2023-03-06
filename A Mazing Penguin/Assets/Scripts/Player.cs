using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject[] movingBlocks;                            //Keeps track of each moving block in current level
    [SerializeField] private List<Vector3> blockOriginPos = new List<Vector3>();   //Stores each moving block in array their Vector3 positions

    [SerializeField] private GameObject[] snowPiles;    //Acts as optional new checkpoint locations throughout a level
    [SerializeField] private LayerMask snowPileLayer;   //For detecting any nearby snow piles

    [SerializeField] private GameObject _checkpoint;   //Referencing current level checkpoint for respawn location
    [SerializeField] private LayerMask enemyLayer;     //For detecting enemies and dangers
    [SerializeField] private LayerMask pitLayer;       //For detecting pits and edges
    [SerializeField] private Text playerDeaths;        //For displaying player total deaths
    [SerializeField] private Collider _collider;       //References player hitbox so player deaths do not occur during respawn

    private PlayerMovement playerMovementRef;   //For setting isMoving to false while respawning
    private Vector3 snowPilePos;                //References snow pile position when player has stepped near it

    [SerializeField] private bool isNearSnowPile = false;      //Checks if player is within vicinity of a snow pile
    [SerializeField] private bool canPlaceCheckpoint = true;   //Player can reposition respawn ONE TIME to any nearby snow pile

    private bool isFalling = false;   //For when player has fallen down a pit or edge

    private float fallSpeed = 0f;   //Penguin will gradually pick up speed when falling down a pit or edge

    public bool isRespawning = false;   //To be referenced by other scripts during player dying or resetting


    private void Awake()
    {
        playerMovementRef = GetComponent<PlayerMovement>();
        SetBlockResetPosition();
    }


    private void Start()
    {
        Saving();
    }


    private void Update()
    {
        //Player can still use its one time use ability to move the respawn point for THIS level
        //Player is nearby a snowpile
        //Checks if player has pressed Q
        if(canPlaceCheckpoint && isNearSnowPile && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("placed checkpoint");
            PlaceCheckpoint();
        }

        //Player has fallen down a pit or an edge
        if(isFalling)
        {
            Debug.Log("is falling");
            transform.position -= transform.up * fallSpeed * Time.deltaTime;
            fallSpeed += 0.15f;
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Respawn();
        //}
        //CheckDistanceToSnowPiles();
        //Debug.Log(closestSnowPilePos);
    }


    //Player has chosen its checkpoint position by placing their respawn point on a new snowpile
    //All block reset positions are set to where they are now located
    void PlaceCheckpoint()
    {
        canPlaceCheckpoint = false;
        _checkpoint.transform.position = snowPilePos;

        foreach(GameObject block in movingBlocks)
        {
            int posInList = System.Array.IndexOf(movingBlocks, block);
            blockOriginPos[posInList] = block.transform.position;
        }
    }


    //Game saves after every new level reached
    //Game saves after every player death
    void Saving()
    {
        SaveSystem.SaveGame(this);
        DisplayPlayerDeaths();
    }


    //Shows the total number of deaths to the player
    void DisplayPlayerDeaths()
    {
        playerDeaths.text = "Deaths: " + PlayerData.deathCount.ToString();
    }


    //Each moving block in array gets their original Vector3 position set into a list
    void SetBlockResetPosition()
    {
        foreach(GameObject block in movingBlocks)
        {
            blockOriginPos.Add(block.transform.position);
        }
    }


    //The posInList is set so POSITION IN LIST matches POSITION IN ARRAY
    //Each moving block Vector3 positions are reset to their original Vector3 on player respawn
    void ResetBlocks()
    {
        foreach(GameObject block in movingBlocks)
        {
            int posInList = System.Array.IndexOf(movingBlocks, block);
            block.transform.position = blockOriginPos[posInList];
        }
    }


    //Detects for enemies, hazards, and nearby snow piles
    private void OnTriggerEnter(Collider other)
    {
        if((enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            _collider.enabled = false;
            PlayerData.deathCount++;
            Saving();
            StartCoroutine(Respawn());
        }
        if((pitLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            _collider.enabled = false;
            PlayerData.deathCount++;
            Saving();
            isFalling = true;
            StartCoroutine(Respawn());
        }
        if((snowPileLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            foreach(GameObject snowPile in snowPiles)
            {
                if(snowPile.name.Equals(other.gameObject.name))
                {
                    snowPilePos = snowPile.transform.position;
                    isNearSnowPile = true;
                }
            }
        }
    }


    //Detects for when player has left the area of a snow pile
    private void OnTriggerExit(Collider other)
    {
        if((snowPileLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            isNearSnowPile = false;
        }
    }


    //Player has died and all character movement is stopped
    //Player returns to last checkpoint position
    //Then isRespawning, and blocks are reset to original positions
    //Player respawn logic continues in the CameraMovement script
    IEnumerator Respawn()
    {
        playerMovementRef.enabled = false;
        playerMovementRef.isMoving = false;
        yield return new WaitForSeconds(1f);

        if(isFalling)
        {
            isFalling = false;
            fallSpeed = 0;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("waited for end of frame");
        transform.position = _checkpoint.transform.position;
        _collider.enabled = true;
        isRespawning = true;
        ResetBlocks();
    }
}
