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

        //soundEvent.InitEventDict();


        // Dictionary의 Key 리스트를 복사
        var soundTypeKeys = new List<SoundType>(soundEvent.EventDict.Keys);

        for (int i = 0; i < soundTypeKeys.Count; i++)
        {
            SoundType soundTypeKey = soundTypeKeys[i];

            EditorGUILayout.LabelField($"Sound Type: {soundTypeKey}", EditorStyles.boldLabel);

            // 내부 Dictionary 가져오기
            SerializableDictionary<string, string> innerDict = soundEvent.EventDict[soundTypeKey];

            // 내부 Dictionary의 Key 리스트 복사
            var innerKeys = new List<string>(innerDict.Keys);

            for (int j = 0; j < innerKeys.Count; j++)
            {
                string key = innerKeys[j];

                EditorGUILayout.BeginHorizontal();
                // Key와 Value 표시
                EditorGUILayout.LabelField($"{key}", GUILayout.MaxWidth(200));
                innerDict[key] = EditorGUILayout.TextField(innerDict[key]);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }

        // Dictionary에 새로운 SoundType 추가
        if (GUILayout.Button("Init"))
        {
            soundEvent.InitEventDict();
        }

        // 변경 사항 저장
        if (GUI.changed)
        {
            EditorUtility.SetDirty(soundEvent);
        }
    }
}
