using UnityEngine;

public class PopupRequestInfo
{
    public string Name { get; private set; }
    public System.Type Type { get; private set; }
    public Transform Parent { get; private set; }
    public object Data { get; private set; } // 팝업에 전달할 데이터

    public PopupRequestInfo(System.Type type, string name, Transform parent = null, object data = null)
    {
        Type = type;
        Name = name;
        Parent = parent;
        Data = data;
    }
}