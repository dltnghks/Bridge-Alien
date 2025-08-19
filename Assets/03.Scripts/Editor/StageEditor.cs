using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

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

        if (GUILayout.Button("Reset", GUILayout.ExpandWidth(true)))
        {
            if (EditorUtility.DisplayDialog("Reset Confirmation", 
                "@MiniGameRoot 아래 있는 모든 스테이지 프리팹을 지우겠습니까?", 
                "Yes", "No"))
            {
                ResetMiniGameRoot();
            }
        }

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

        string currentSceneName = EditorSceneManager.GetActiveScene().name;
        if (currentSceneName == "StageDesign")
        {
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

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Create MiniGame Objects", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Cooling Point(냉각기)"))
                {
                    CreateObjectFromPrefab("Assets/04.Prefabs/MiniGame/Unload/CoolingUnit.prefab");
                }
                if (GUILayout.Button("Disposal Point(폐기구역)"))
                {
                    CreateObjectFromPrefab("Assets/04.Prefabs/MiniGame/Unload/DisposalUnit.prefab");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Delivery Point(배달레일)"))
                {
                    CreateObjectFromPrefab("Assets/04.Prefabs/MiniGame/Unload/DeliveryPoint.prefab");
                }
                if (GUILayout.Button("Return Rail(반송레일)"))
                {
                    Debug.LogWarning("반송레일 프리팹을 찾을 수 없습니다. 경로를 확인해주세요.");
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            if (GUILayout.Button("Go to StageDesign Scene"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene("Assets/02.Scenes/Dev/StageDesign.unity");
                }
            }
        }
    }

    private void CreateObjectFromPrefab(string prefabPath)
    {
        if (loadedPrefab == null)
        {
            EditorUtility.DisplayDialog("알림", "먼저 Stage Prefab을 로드해야 합니다.", "확인");
            return;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at {prefabPath}");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(loadedPrefab.transform);
        Selection.activeObject = instance;
        Debug.Log($"{instance.name} created.");
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

        if (loadedPrefab != null)
        {
            ActivateChildObject(loadedPrefab, "CoolingUnit");
            ActivateChildObject(loadedPrefab, "DisposalUnit");
            ActivateChildObject(loadedPrefab, "DeliveryPoint");
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

    private void ActivateChildObject(GameObject parent, string name)
    {
        Transform child = FindDeepChild(parent.transform, name);
        if (child != null)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                Debug.Log($"Found and activated existing object: {name}");
            }
        }
    }

    private Transform FindDeepChild(Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach(Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    private void ResetMiniGameRoot()
    {
        GameObject root = GameObject.Find("@MiniGameRoot");
        if (root != null)
        {
            List<GameObject> childrenToDestroy = new List<GameObject>();
            foreach (Transform child in root.transform)
            {
                childrenToDestroy.Add(child.gameObject);
            }

            foreach (GameObject child in childrenToDestroy)
            {
                DestroyImmediate(child);
            }

            Debug.Log("All objects under @MiniGameRoot have been removed.");
        }
        else
        {
            Debug.LogWarning("@MiniGameRoot not found in the scene.");
        }

        loadedPrefab = null;
        miniGameUnloadSerializedObject = null;
        EditorPrefs.DeleteKey("StageEditor_LoadedPrefabID");
    }
}
