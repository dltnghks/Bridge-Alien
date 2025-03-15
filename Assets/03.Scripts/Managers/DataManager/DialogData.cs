using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class DialogData
{
    public string DialogID { get; set; }
    public string CharacterName { get; set; }
    public string Script { get; set; }
    public string NextDialogID { get; set; }
    public Define.DialogType Type { get; set; }
}