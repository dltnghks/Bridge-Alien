using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundEvent))]
public class SoundEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundEvent soundEvent = (SoundEvent)target;

        soundEvent.InitEventDict();

        base.OnInspectorGUI();

    }
}
