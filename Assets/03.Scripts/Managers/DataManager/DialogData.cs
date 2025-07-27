using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[System.Serializable]
public class DialogData
{
    public string DialogID;
    public string CharacterName;
    public string Script;
    public string NextDialogID;
    public Define.DialogType Type;
    public Define.DialogSpeakerType SpeakerType;
    public Define.DialogSpeakerPosType SpeakerPosType;
}
