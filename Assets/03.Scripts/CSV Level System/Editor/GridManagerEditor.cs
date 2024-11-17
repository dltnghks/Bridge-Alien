using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridManager gridManager = (GridManager)target;
        
        // CSV 폴더 필드에 드래그 앤 드롭 지원
        EditorGUI.BeginChangeCheck();
        Object newFolder = EditorGUILayout.ObjectField("CSV Folder", serializedObject.FindProperty("csvFolder").objectReferenceValue, typeof(Object), false);
        if (EditorGUI.EndChangeCheck())
        {
            string folderPath = AssetDatabase.GetAssetPath(newFolder);
            if (System.IO.Directory.Exists(folderPath))
            {
                serializedObject.FindProperty("csvFolder").objectReferenceValue = newFolder;
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogError("올바른 폴더를 선택해주세요!");
            }
        }

        // 나머지 기본 인스펙터 그리기
        DrawPropertiesExcluding(serializedObject, new string[] { "csvFolder" });
        serializedObject.ApplyModifiedProperties();
    }
} 