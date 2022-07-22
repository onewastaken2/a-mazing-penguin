using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject[] movingBlocks;                            //Keeps track of each moving block in current level
    [SerializeField] private List<Vector3> blockOriginPos = new List<Vector3>();   //Stores each moving block in array their Vector3 positions

    [SerializeField] private GameObject _checkpoint;   //Referencing current level checkpoint
    [SerializeField] private LayerMask enemyLayer;     //For detecting enemies and dangers
    [SerializeField] private Text playerDeaths;        //For displaying player total deaths

    private PlayerMovement playerMovementRef;   //For setting isMoving to false while respawning

    private Vector3 checkpointPos;   //References checkpoint position for respawn location

    public bool isRespawning = false;


    private void Awake()
    {
        playerMovementRef = GetComponent<PlayerMovement>();
        checkpointPos = _checkpoint.transform.position;
        SetBlockResetPosition();
    }


    private void Start()
    {
        Saving();
    }

    //--TESTING PLAYER MANUAL RESET TO LAST CHECKPOINT--
    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Q))
    //    {
    //        Respawn();
    //    }
    //}


    //Detects for enemies and other dangers
    private void OnTriggerEnter(Collider other)
    {
        if((enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            PlayerData.deathCount++;
            Saving();
            StartCoroutine(Respawn());
        }
    }


    //Player has died or reset and is being sent back to checkpoint
    //Blocks have been reset
    //void Respawn()
    //{
    //    playerMovementRef.isMoving = false;
    //    transform.position = checkpointPos;
    //    ResetBlocks();
    //}


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


    //The PosInList is set so POSITION IN LIST matches POSITION IN ARRAY
    //Each moving block Vector3 positions are reset to their original Vector3 on player respawn
    void ResetBlocks()
    {
        foreach(GameObject block in movingBlocks)
        {
            int posInList = System.Array.IndexOf(movingBlocks, block);
            block.transform.position = blockOriginPos[posInList];
        }
    }


    IEnumerator Respawn()
    {
        playerMovementRef.enabled = false;
        yield return new WaitForSeconds(1f);
        transform.position = checkpointPos;
        isRespawning = true;
        ResetBlocks();
    }

    //player can manually go back to checkpoint to opt to reset blocks
    //player must click the checkpoint flag to prompt player if they wish to go back?
}
