[System.Serializable]
public class PlayerData
{
    public static int farthestLevelReached;   //Updates from LevelManager class
    public static int deathCount;             //Updates from Player class

    public int gameProgress;   //How far player has reached in level progression
    public int totalDeaths;    //Accumulated player deaths throughout their game


    //Constructor for holding player data to be saved
    public PlayerData(Player _player)
    {
        gameProgress = farthestLevelReached;
        totalDeaths = deathCount;
    }
}
