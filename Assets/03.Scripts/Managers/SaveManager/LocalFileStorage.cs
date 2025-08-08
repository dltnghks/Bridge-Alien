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
        // Task.Run을 사용해 동기적인 파일 읽기 작업을 백그라운드 스레드에서 실행
        string data = await Task.Run(() => File.ReadAllText(_path));
        Debug.Log("Loaded data from local file.");
        return data;
    }

    public async Task SaveAsync(string data)
    {
        Debug.Log("Saved data to local file: " + _path);
        // Task.Run을 사용해 동기적인 파일 쓰기 작업을 백그라운드 스레드에서 실행
        // 이는 UI 멈춤을 방지하는 좋은 습관입니다.
        await Task.Run(() => File.WriteAllText(_path, data));
        Debug.Log("Saved data to local file: " + _path);
    }
}