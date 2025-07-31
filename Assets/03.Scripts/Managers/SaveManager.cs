using System.IO;
using UnityEngine;

public class SaveManager
{
    string path;
    
    public void Init()
    {
        path = Application.persistentDataPath + "/player.json";
    }

    public void Reset()
    {
        SaveData saveData = new SaveData();
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(path, json);

        Load();
    }

    public void Save()
    {

        SaveData saveData = new SaveData();
        saveData.PlayerData = Managers.Player.PlayerData;
        saveData.LastDate = Managers.Daily.CurrentDate;
        saveData.LastDailyData = Managers.Daily.PreDailyData;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(path, json);
        Debug.Log("Saved to: " + path);
    }

    public PlayerData Load() {
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData loadData = JsonUtility.FromJson<SaveData>(json);
            Managers.Player.Init(loadData.PlayerData);
            Managers.Daily.Init(loadData.LastDate, loadData.LastDailyData);
        }
        Debug.LogWarning("No save file found.");
        return new PlayerData();
    }
}
