using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    //Sets up a game file for saving player data
    //Creates new object from constructor within PlayerData class
    //Secures data by converting information into binary
    public static void SaveGame(Player _player)
    {
        BinaryFormatter _formatter = new BinaryFormatter();
        string _path = Application.persistentDataPath + "/savedata.save";
        FileStream _file = new FileStream(_path, FileMode.OpenOrCreate);
        PlayerData _data = new PlayerData(_player);
        _formatter.Serialize(_file, _data);
        _file.Close();
    }


    //Searches the file path and naming convention of saved player data
    //Opens file to extract from binary and is now readable
    public static PlayerData LoadGame()
    {
        string _path = Application.persistentDataPath + "/savedata.save";

        if(File.Exists(_path))
        {
            BinaryFormatter _formatter = new BinaryFormatter();
            FileStream _file = new FileStream(_path, FileMode.Open);
            PlayerData _data = _formatter.Deserialize(_file) as PlayerData;
            _file.Close();
            return _data;
        }
        else
        {
            Debug.LogError("Save file not found in " + _path);
            return null;
        }
    }
}
