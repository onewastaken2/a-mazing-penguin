using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;     //For detecting enemies and dangers
    [SerializeField] private GameObject _checkpoint;   //Referencing current level checkpoint
    [SerializeField] private Text playerDeaths;        //For displaying player total deaths

    private PlayerMovement playerMovement;   //For saying isMoving is false while respawning

    private Vector3 checkpointPos;   //References checkpoint position for respawn location


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        checkpointPos = _checkpoint.transform.position;
    }


    private void Start()
    {
        Saving();
    }


    //Detects for enemies and other dangers
    private void OnTriggerEnter(Collider other)
    {
        if((enemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            PlayerData.deathCount++;
            Saving();
            Respawn();
        }
    }


    //Player has died and is being sent back to checkpoint
    void Respawn()
    {
        playerMovement.isMoving = false;
        transform.position = checkpointPos;
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
}
