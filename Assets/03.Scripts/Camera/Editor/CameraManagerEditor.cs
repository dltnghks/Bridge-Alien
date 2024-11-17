using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor
{
    private SerializedProperty currentCameraType;
    private SerializedProperty target;
    private SerializedProperty topDownSettings;
    private SerializedProperty thirdPersonSettings;

    private void OnEnable()
    {
        currentCameraType = serializedObject.FindProperty("currentCameraType");
        target = serializedObject.FindProperty("target");
        topDownSettings = serializedObject.FindProperty("topDownSettings");
        thirdPersonSettings = serializedObject.FindProperty("thirdPersonSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(currentCameraType);
        EditorGUILayout.PropertyField(target);

        EditorGUILayout.Space();

        // 현재 선택된 카메라 타입에 따라 해당하는 설정만 표시
        CameraManager.CameraType selectedType = (CameraManager.CameraType)currentCameraType.enumValueIndex;
        switch (selectedType)
        {
            case CameraManager.CameraType.TopDown:
                EditorGUILayout.PropertyField(topDownSettings, true);
                break;
            case CameraManager.CameraType.ThirdPerson:
                EditorGUILayout.PropertyField(thirdPersonSettings, true);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
} 