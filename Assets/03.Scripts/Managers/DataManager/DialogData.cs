using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class DialogData
{
    [JsonProperty("DialogueKey")]
    public string DialogKey { get; set; }

    [JsonProperty("Character")]
    public string Character { get; set; }

    [JsonProperty("Dialogue")]
    public string Dialog { get; set; }

    [JsonProperty("Speaker")]
    public string Speaker { get; set; }
}