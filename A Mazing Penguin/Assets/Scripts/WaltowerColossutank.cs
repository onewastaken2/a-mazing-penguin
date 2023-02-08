using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaltowerColossutank : MonoBehaviour
{
    private enum State   //All possible states the boss can be in
    {
        Waking,
        PhaseOne,
        PhaseTwo,
        PhaseThree,
        Damaged,
        Recovering,
        Dying
    }

    private State currentState;   //Which state the boss is in currently

    private enum Turret   //Represents which turrets may be shooting a snowball
    {
        LeftTurret,
        RightTurret,
        BothTurrets,
        MouthTurret
    }

    private Turret firingTurret;   //Which turret is firing after having been randomly selected

    [SerializeField] private GameObject playerObj;       //For referencing player position when Waking, PhaseOne, and PhaseTwo
    [SerializeField] private GameObject bossSwitchObj;   //For referencing when to enable bossSwitch when Damaged

    [SerializeField] private GameObject[] floorSwitches;                                      //Stores all floor switch objects
    [SerializeField] private List<BossSwitch> activeFloorSwitches = new List<BossSwitch>();   //Tracks which floor switch from array are enabled

    [SerializeField] private GameObject snowballPrefab;         //References snowball prefab for instantiation
    [SerializeField] private Transform leftTurretSpawnPoint;    //Where snowball will spawn from when fired from the LEFT turret
    [SerializeField] private Transform rightTurretSpawnPoint;   //Where snowball will spawn from when fired from the RIGHT turret
    [SerializeField] private Transform mouthTurretSpawnPoint;   //Where snowball will spawn from when fired from the MIDDLE turret

    private BossSwitch bossSwitch;   //For referencing bossSwitchObj script for if isActive
    private Player playerDiedRef;

    private bool isWaking = false;          //For when player is nearby to start the boss fight
    private bool isRotatingRight = false;   //For when the boss is turning to ITS right
    private bool justFired = false;         //Used to give boss pause after firing a snowball

    private int bossHealth = 9;               //How many times the player needs to hit the bossSwitchObj to defeat the boss
    private int numberOfActiveSwitches = 2;   //The number of switches that will be turned on depending on the phase

    private float followSpeed = 2f;          //How fast boss can turn when following and facing the player
    private float rotateSpeed = 70f;         //How fast boss can turn when rotating
    private float nextRotationChangeTimer;   //How long boss will change rotation after the time has been randomly given between a range
    private float timeBetweenShots = 3f;     //How long before the boss can shoot another snowball
    private float setTimeBetweenShots;       //For setting timeBetweenShots timer depending on the phase
    private float vulnerabilityTime = 10f;   //How long the boss is exposed while Damaged
    private float recoveringTimer = 1f;      //A brief interval between when boss is waking up or had taken damage
    private float setRecoveringTimer;        //For resetting recoveringTimer after timer counted down


    private void Awake()
    {
        setTimeBetweenShots = timeBetweenShots;
        setRecoveringTimer = recoveringTimer;
        bossSwitch = bossSwitchObj.GetComponent<BossSwitch>();
        playerDiedRef = playerObj.GetComponent<Player>();
        currentState = State.Waking;
    }


    private void Update()
    {
        switch(currentState)
        {
            //Boss is currently in an inactive state
            //Checks if player has moved close enough to begin the fight
            //If so, boss will be active and enter its PhaseOne state
            case State.Waking:
                {
                    if(!isWaking)
                    {
                        float distanceToPlayer = (playerObj.transform.position - transform.position).magnitude;

                        if(distanceToPlayer <= 10f)
                        {
                            isWaking = true;
                        }
                    }
                    else if(recoveringTimer > 0.0f)
                    {
                        recoveringTimer -= Time.deltaTime;
                    }
                    else
                    {
                        isWaking = false;
                        ActivateFloorSwitches();
                        currentState = State.PhaseOne;
                        recoveringTimer = setRecoveringTimer;
                    }
                }
                break;

            //Boss is following and facing the player
            //It is shooting snowballs randomly from its turrets
            //Checks for if the 2 out of 10 enabled floor switches are still active
            case State.PhaseOne:
                {
                    FollowPlayer();
                    TrackFloorSwitches();
                }
                break;

            //Boss is following and facing the player
            //It is shooting snowballs randomly from its turrets at a faster rate
            //Snowballs can now be shot from its mouth turret
            //Checks for if the 4 out of 10 enabled floor switches are still active
            case State.PhaseTwo:
                {
                    FollowPlayer();
                    TrackFloorSwitches();
                }
                break;

            //Boss is rotating quickly and chaotically
            //It is shooting snowballs randomly from all possible turrets and at an even faster rate
            //Checks for if the 6 out of 10 enabled floor switches are still active
            case State.PhaseThree:
                {
                    RotationPattern();
                    RandomlyRotating();
                    RandomShooting();
                    TrackFloorSwitches();
                }
                break;

            //All floor switches have been turned off and boss is rotating around
            //The boss switch is revealed and it CAN be damaged 1 HP at a time
            //Boss is vulnerable only within a short window of time
            case State.Damaged:
                {
                    RotationPattern();

                    if(vulnerabilityTime > 0.0f)
                    {
                        vulnerabilityTime -= Time.deltaTime;

                        if(bossSwitch.isActive == false)
                        {
                            bossHealth--;
                            vulnerabilityTime = 0;
                        }
                    }
                    else
                    {
                        bossSwitch.isActive = false;
                        currentState = State.Recovering;
                    }
                }
                break;

            //Boss has either been damaged OR recovered without taking damage while vulnerable
            //Starts at 9 HP, enters PhaseTwo when at 6 HP, and PhaseThree when at 3 HP
            //Boss is defeated when the player manages to bring its HP down to 0
            case State.Recovering:
                {
                    if(recoveringTimer > 0.0f)
                    {
                        recoveringTimer -= Time.deltaTime;
                    }
                    else
                    {
                        if(bossHealth >= 7)
                        {
                            numberOfActiveSwitches = 2;
                            setTimeBetweenShots = 3f;
                            vulnerabilityTime = 10f;
                            ActivateFloorSwitches();
                            currentState = State.PhaseOne;
                        }
                        else if(bossHealth >= 4)
                        {
                            numberOfActiveSwitches = 4;
                            setTimeBetweenShots = 2f;
                            vulnerabilityTime = 6f;
                            ActivateFloorSwitches();
                            currentState = State.PhaseTwo;
                        }
                        else if(bossHealth > 0)
                        {
                            numberOfActiveSwitches = 6;
                            setTimeBetweenShots = 1f;
                            vulnerabilityTime = 3f;
                            ActivateFloorSwitches();
                            currentState = State.PhaseThree;
                        }
                        else
                        {
                            currentState = State.Dying;
                        }
                        timeBetweenShots = setTimeBetweenShots;
                        recoveringTimer = setRecoveringTimer;
                    }
                }
                break;

            case State.Dying:
                {
                    Debug.Log("boss is destroyed");
                    Debug.Log("insert death animation and leave behind rubble");

                    //walls come down to allow player to pass freely to end of level flag
                }
                break;
        }
        if(playerDiedRef.isRespawning)
        {
            RestartBoss();
        }
    }

    
    //Uses Fisher-Yates shuffle to randomize order of floor switches in array
    //Activates a certain number of switches starting from the beginning of the array
    //Each active floor switch is added to the activeFloorSwitches list
    void ActivateFloorSwitches()
    {
        for(int i = floorSwitches.Length - 1; i > 0; i--)
        {
            int randomNumber = Random.Range(0, i + 1);
            GameObject _temp = floorSwitches[i];
            floorSwitches[i] = floorSwitches[randomNumber];
            floorSwitches[randomNumber] = _temp;
        }
        for(int i = 0; i < numberOfActiveSwitches; i++)
        {
            BossSwitch activeFloorSwitch = floorSwitches[i].GetComponent<BossSwitch>();

            activeFloorSwitch.isActive = true;
            activeFloorSwitch.GetComponent<MeshRenderer>().enabled = true;
            activeFloorSwitches.Add(activeFloorSwitch);
        }
    }


    //Keeps track of whether a floor switch has been deactivated by the player
    //Removes any deactivated floor switches from the activeFloorSwitches list
    //Boss becomes vulnerable if player has deactivated all floor switches
    void TrackFloorSwitches()
    {
        for(int i = activeFloorSwitches.Count - 1; i >= 0; i--)
        {
            if(activeFloorSwitches[i].isActive == false)
            {
                activeFloorSwitches.RemoveAt(i);
            }
        }
        if(activeFloorSwitches.Count == 0)
        {
            int randomNumber = Random.Range(0, 2);

            if(randomNumber == 0)
            {
                isRotatingRight = true;
            }
            else
            {
                isRotatingRight = false;
            }
            bossSwitch.isActive = true;
            bossSwitch.GetComponent<MeshRenderer>().enabled = true;
            currentState = State.Damaged;
        }
    }


    //Boss is in Damaged or PhaseThree state
    //Rotates at a constant speed clockwise or counter-clockwise
    void RotationPattern()
    {
        if(isRotatingRight)
        {
            transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }
        else
        {
            transform.Rotate(-new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }
    }


    //Boss is in PhaseThree rotating chaotically
    //Boss may randomly change rotation to break the monotony
    void RandomlyRotating()
    {
        if(nextRotationChangeTimer > 0.0f)
        {
            nextRotationChangeTimer -= Time.deltaTime;
        }
        else
        {
            int nextRotationChangeInterval = Random.Range(3, 7);

            nextRotationChangeTimer = nextRotationChangeInterval;

            if(isRotatingRight)
            {
                isRotatingRight = false;
            }
            else
            {
                isRotatingRight = true;
            }
        }
    }


    //Periodically fires snowballs from turrets over a set time
    //Randomly selected whether from left, right, or both turrets
    //If in PhaseTwo or PhaseThree, also selects from the mouth turret
    void RandomShooting()
    {
        if(!justFired & timeBetweenShots > 0.0f)
        {
            timeBetweenShots -= Time.deltaTime;
        }
        else if(timeBetweenShots <= 0.0f)
        {
            if(currentState == State.PhaseOne)
            {
                firingTurret = (Turret)Random.Range(0, 3);
            }
            else
            {
                firingTurret = (Turret)Random.Range(0, 4);
            }
            GameObject snowballObj;

            switch(firingTurret)
            {
                case Turret.LeftTurret:
                    {
                        snowballObj = Instantiate(snowballPrefab, leftTurretSpawnPoint.position, leftTurretSpawnPoint.rotation) as GameObject;
                    }
                    break;

                case Turret.RightTurret:
                    {
                        snowballObj = Instantiate(snowballPrefab, rightTurretSpawnPoint.position, leftTurretSpawnPoint.rotation) as GameObject;
                    }
                    break;

                case Turret.BothTurrets:
                    {
                        snowballObj = Instantiate(snowballPrefab, leftTurretSpawnPoint.position, leftTurretSpawnPoint.rotation) as GameObject;
                        snowballObj = Instantiate(snowballPrefab, rightTurretSpawnPoint.position, leftTurretSpawnPoint.rotation) as GameObject;
                    }
                    break;

                case Turret.MouthTurret:
                    {
                        snowballObj = Instantiate(snowballPrefab, mouthTurretSpawnPoint.position, mouthTurretSpawnPoint.rotation) as GameObject;
                    }
                    break;
            }
            if(currentState == State.PhaseOne | currentState == State.PhaseTwo)
            {
                justFired = true;
            }
            timeBetweenShots = setTimeBetweenShots;
        }
    }


    //For PhaseOne and PhaseTwo, boss faces and follows directional position of player
    //Only begins shooting when nearly facing player position and had not recently fired
    //Otherwise, boss rotates at a constant speed to make its way to face player position
    void FollowPlayer()
    {
        if(justFired)
        {
            StartCoroutine(TurretsRecharge());
        }
        else
        {
            Vector3 lookDirection = playerObj.transform.position - transform.position;
            lookDirection.y = 0;
            Quaternion rotateTo = Quaternion.LookRotation(lookDirection);
            float _angle = Vector3.Angle(lookDirection, transform.forward);

            if(_angle > 20f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotateSpeed * Time.deltaTime);
                Debug.Log("rotate towards");
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, followSpeed * Time.deltaTime);
                RandomShooting();
                Debug.Log("slerp");
            }
        }
    }


    //Player has just died and is respawning
    //Boss stats and progress has been reset
    void RestartBoss()
    {
        bossHealth = 9;
        numberOfActiveSwitches = 2;
        setTimeBetweenShots = 3f;
        vulnerabilityTime = 10f;
        currentState = State.Waking;

        for(int i = activeFloorSwitches.Count - 1; i >= 0; i--)
        {
            activeFloorSwitches[i].isActive = false;
            activeFloorSwitches[i].GetComponent<MeshRenderer>().enabled = false;
            activeFloorSwitches.RemoveAt(i);
        }
        timeBetweenShots = setTimeBetweenShots;
    }


    //A brief interval of time after when boss fires a snowball shot
    IEnumerator TurretsRecharge()
    {
        yield return new WaitForSeconds(0.4f);
        justFired = false;
    }
}
