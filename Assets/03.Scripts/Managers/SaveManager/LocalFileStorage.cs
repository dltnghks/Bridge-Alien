using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LocalFileStorage : IDataStorage
{
    private readonly string _path;

    public LocalFileStorage()
    {
        _path = Application.persistentDataPath + "/savegame.json";
    }
    public void Delete()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }

    public bool Exists()
    {
        return File.Exists(_path);
    }

    public async Task<string> LoadAsync()
    {
        if (!File.Exists(_path))
            return string.Empty;

        try
        {
            // Task.Run을 이용하여 동기적인 파일 읽기 작업을 백그라운드 스레드에서 실행
            string data = await Task.Run(() => File.ReadAllText(_path));
            Debug.Log("Loaded data from local file.");
            return data;
        }
        catch (IOException ex)
        {
            Debug.LogWarning($"Load failed: {ex}");
            return string.Empty;
        }
    }

    public async Task SaveAsync(string data)
    {
        try
        {
            Debug.Log("Saved data to local file: " + _path);
            // Task.Run을 이용하여 동기적인 파일 쓰기 작업을 백그라운드 스레드에서 실행
            // 이는 UI 프리징을 방지하는 좋은 방법입니다.
            await Task.Run(() => File.WriteAllText(_path, data));
            Debug.Log("Saved data to local file: " + _path);
        }
        catch (IOException ex)
        {
            Debug.LogWarning($"Save failed: {ex}");
        }
    }
}