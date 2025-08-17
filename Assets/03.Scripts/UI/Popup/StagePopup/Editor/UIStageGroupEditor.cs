using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIStageButtonGroup))]
public class UIStageGroupEditor : Editor
{
    private GameObject _stageButtonPrefab;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _stageButtonPrefab = (GameObject)EditorGUILayout.ObjectField(
            "생성할 스테이지",
            _stageButtonPrefab,
            typeof(GameObject),
            false
        );

        UIStageButtonGroup uiStageButtonGroup = (UIStageButtonGroup)target;

        if (GUILayout.Button("스테이지 추가"))
        {
            GameObject newStageButton = (GameObject)PrefabUtility.InstantiatePrefab(_stageButtonPrefab);
            newStageButton.transform.SetParent(Selection.activeTransform, false);

            newStageButton.transform.localPosition = Vector3.zero;
        }
    }
}
