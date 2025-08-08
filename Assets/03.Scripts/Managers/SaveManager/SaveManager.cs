using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveManager
{
    private IDataStorage storage;// 파일 시스템이든, 서버든 상관없이 IDataStorage 타입에 의존
    private List<ISaveable> saveableObjects = new List<ISaveable>();

    // 생성자 또는 Init 메소드를 통해 외부에서 저장 방식(storage)을 주입받음
    public void Init(IDataStorage storage)
    {
        this.storage = storage;
    }

    public void Register(ISaveable saveable)
    {
        if (!saveableObjects.Contains(saveable))
        {
            saveableObjects.Add(saveable);
        }
    }

    public async Task SaveAsync()
    {
        Logger.Log("Save");
        var saveData = new Dictionary<string, object>();
        foreach (var saveable in saveableObjects)
        {
            saveData[saveable.GetType().ToString()] = saveable.CaptureState();
        }

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(saveData, Newtonsoft.Json.Formatting.Indented);
        Logger.Log(json);

        await storage.SaveAsync(json);
    }

    public async Task LoadAsync()
    {
        if (!storage.Exists())
        {
            Logger.LogWarning("No save data found");
            return;
        }

        string json = await storage.LoadAsync();

        if (string.IsNullOrEmpty(json)) return;

        var saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        foreach (var saveable in saveableObjects)
        {
            string key = saveable.GetType().ToString();
            if (saveData.TryGetValue(key, out object value))
            {
                var restoredValue = Newtonsoft.Json.JsonConvert.DeserializeObject(value.ToString(), saveable.CaptureState().GetType());
                saveable.RestoreState(restoredValue);
            }
        }

        Logger.Log("Game Loaded");
    }

    public void Reset()
    {
        storage.Delete();
        Logger.Log("Game Data Reset");
        _ = LoadAsync();
    }

    // public void Save()
    // {

    //     SaveData saveData = new SaveData();
    //     saveData.PlayerData = Managers.Player.PlayerData;
    //     saveData.LastDate = Managers.Daily.CurrentDate;

    //     if (Managers.Daily.PreDailyData is null)
    //         saveData.LastDailyData = Managers.Daily.CurrentDailyData;
    //     else
    //         saveData.LastDailyData = Managers.Daily.PreDailyData;
    //     saveData.MiniGameTutorial = Managers.MiniGame.MiniGameTutorial;

    //     string json = JsonUtility.ToJson(saveData);
    //     File.WriteAllText(path, json);
    //     Debug.Log("Saved to: " + path);
    // }

    // public void Load() {
    //     Logger.Log("Load");
    //     if (File.Exists(path))
    //     {
    //         string json = File.ReadAllText(path);
    //         SaveData loadData = JsonUtility.FromJson<SaveData>(json);
    //         Managers.Player.Init(loadData.PlayerData);
    //         Managers.Daily.Init(loadData.LastDate, loadData.LastDailyData);
    //         Managers.MiniGame.Init(loadData.MiniGameTutorial);
    //         return;
    //     }
    //     Debug.LogWarning("No save file found.");
    // }
}
