using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StageEditor : EditorWindow
{
    private StageDataSO stageDataSO;
    private Vector2 leftScrollPosition;
    private Vector2 rightScrollPosition;

    private Define.StageType? selectedStageType;

    private GameObject loadedPrefab;
    private SerializedObject miniGameUnloadSerializedObject;

    [MenuItem("Tools/Stage Editor")]
    public static void ShowWindow()
    {
        GetWindow<StageEditor>("Stage Editor");
    }

    private void OnEnable()
    {
        LoadStageDataSO();

        string selectedStageName = EditorPrefs.GetString("StageEditor_SelectedStage", "");
        if (!string.IsNullOrEmpty(selectedStageName))
        {
            if (System.Enum.TryParse<Define.StageType>(selectedStageName, out var stageType))
            {
                selectedStageType = stageType;
            }
        }

        int loadedPrefabID = EditorPrefs.GetInt("StageEditor_LoadedPrefabID", -1);
        if (loadedPrefabID != -1)
        {
            loadedPrefab = EditorUtility.InstanceIDToObject(loadedPrefabID) as GameObject;
            if (loadedPrefab != null)
            {
                MiniGameUnload miniGameUnload = loadedPrefab.GetComponent<MiniGameUnload>();
                if (miniGameUnload != null)
                {
                    miniGameUnloadSerializedObject = new SerializedObject(miniGameUnload);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (selectedStageType.HasValue)
        {
            EditorPrefs.SetString("StageEditor_SelectedStage", selectedStageType.Value.ToString());
        }
        else
        {
            EditorPrefs.DeleteKey("StageEditor_SelectedStage");
        }

        if (loadedPrefab != null)
        {
            EditorPrefs.SetInt("StageEditor_LoadedPrefabID", loadedPrefab.GetInstanceID());
        }
        else
        {
            EditorPrefs.DeleteKey("StageEditor_LoadedPrefabID");
        }
    }

    private void LoadStageDataSO()
    {
        stageDataSO = AssetDatabase.LoadAssetAtPath<StageDataSO>("Assets/Resources/Data/StageData.asset");
        if (stageDataSO == null)
        {
            Debug.LogError("StageData.asset not found. Please create one at Assets/Resources/Data/StageData.asset");
            return;
        }
        
        if (stageDataSO.stageData == null)
        {
            stageDataSO.stageData = new AYellowpaper.SerializedCollections.SerializedDictionary<Define.StageType, StageData>();
        }
    }

    private void OnGUI()
    {
        if (stageDataSO == null)
        {
            EditorGUILayout.LabelField("StageDataSO not loaded.");
            if (GUILayout.Button("Reload"))
            {
                LoadStageDataSO();
            }
            return;
        }

        EditorGUILayout.LabelField("Stage Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Reload Stage Data"))
        {
            LoadStageDataSO();
        }
        
        EditorGUILayout.BeginHorizontal();

        // Left Panel
        leftScrollPosition = EditorGUILayout.BeginScrollView(leftScrollPosition, GUILayout.Width(150));
        EditorGUILayout.BeginVertical("box");
        foreach (Define.StageType stageType in System.Enum.GetValues(typeof(Define.StageType)))
        {
            if (GUILayout.Button(stageType.ToString()))
            {
                selectedStageType = stageType;
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        // Right Panel
        rightScrollPosition = EditorGUILayout.BeginScrollView(rightScrollPosition);
        EditorGUILayout.BeginVertical("box");

        if (selectedStageType.HasValue)
        {
            DrawStageDetails(selectedStageType.Value);
        }
        else
        {
            EditorGUILayout.LabelField("Select a stage from the left panel.");
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(stageDataSO);
        }
    }

    private void DrawStageDetails(Define.StageType stageType)
    {
        if (!stageDataSO.stageData.ContainsKey(stageType))
        {
            stageDataSO.stageData.Add(stageType, new StageData());
        }

        StageData data = stageDataSO.stageData[stageType];

        EditorGUILayout.LabelField(stageType.ToString(), EditorStyles.boldLabel);
        data.StageName = EditorGUILayout.TextField("Stage Name", data.StageName);
        data.IsLocked = EditorGUILayout.Toggle("Is Locked", data.IsLocked);
        data.RequiredStars = EditorGUILayout.IntField("Required Stars", data.RequiredStars);
        data.ClearReward = EditorGUILayout.IntField("Clear Reward", data.ClearReward);

        EditorGUILayout.LabelField("Clear Score List");
        if (data.ClearScoreList == null)
        {
            data.ClearScoreList = new int[3];
        }
        for (int j = 0; j < data.ClearScoreList.Length; j++)
        {
            data.ClearScoreList[j] = EditorGUILayout.IntField($"Star {j + 1} Score", data.ClearScoreList[j]);
        }
        
        if (GUILayout.Button("Load Stage Prefab"))
        {
            LoadStagePrefab(data.StageName);
        }

        if (loadedPrefab != null && loadedPrefab.name == data.StageName)
        {
            EditorGUILayout.LabelField("MiniGameUnload Settings", EditorStyles.boldLabel);
            if (miniGameUnloadSerializedObject != null)
            {
                miniGameUnloadSerializedObject.Update();
                EditorGUILayout.PropertyField(miniGameUnloadSerializedObject.FindProperty("_gameSetting"), true);
                miniGameUnloadSerializedObject.ApplyModifiedProperties();

                if (GUILayout.Button("Save Game Settings"))
                {
                    SaveGameSettings();
                }
            }
        }
    }

    private void LoadStagePrefab(string stageName)
    {
        if (loadedPrefab != null)
        {
            if (loadedPrefab.name == stageName)
            {
                EditorUtility.DisplayDialog("알림", "이미 로드된 상태입니다.", "확인");
                return;
            }
            else
            {
                SaveGameSettings();
                DestroyImmediate(loadedPrefab);
                loadedPrefab = null;
                miniGameUnloadSerializedObject = null;
            }
        }

        if (string.IsNullOrEmpty(stageName))
        { 
            Debug.LogError("Stage name is empty.");
            return;
        }

        GameObject prefab = Resources.Load<GameObject>($"Prefab/Stages/{stageName}");
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at Prefab/Stages/{stageName}");
            return;
        }

        if (Application.isPlaying)
        {
            loadedPrefab = GameObject.Instantiate(prefab);
        }
        else
        {
            loadedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
        loadedPrefab.name = stageName;

        GameObject root = GameObject.Find("@MiniGameRoot");
        if (root != null)
        {
            loadedPrefab.transform.SetParent(root.transform);
        }
        else
        {
            Debug.LogWarning("@MiniGameRoot not found in the scene.");
        }

        MiniGameUnload miniGameUnload = loadedPrefab.GetComponent<MiniGameUnload>();
        if (miniGameUnload != null)
        {
            miniGameUnloadSerializedObject = new SerializedObject(miniGameUnload);
        }
        else
        {
            miniGameUnloadSerializedObject = null;
        }
    }

    private void SaveGameSettings()
    {
        if (loadedPrefab == null || Application.isPlaying)
        {
            Debug.LogWarning("Can only save game settings in Edit Mode and when a prefab is loaded.");
            return;
        }

        if (PrefabUtility.IsPartOfPrefabInstance(loadedPrefab))
        {
            PrefabUtility.ApplyPrefabInstance(loadedPrefab, InteractionMode.UserAction);
            Debug.Log($"Saved game settings to prefab: {PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(loadedPrefab)}");
        }
        else
        {
            Debug.LogWarning("Loaded object is not a prefab instance. Cannot save settings.");
        }
    }
}
