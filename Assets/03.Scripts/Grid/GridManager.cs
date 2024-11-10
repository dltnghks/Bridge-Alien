using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;           // 각 격자 셀의 크기
    [SerializeField] private float gridHeight = 0.01f;      // 그리드의 높이
    [SerializeField] private TextAsset gridData;            // CSV 파일 참조
    [SerializeField] private bool showGridCells = true;    // 그리드 셀 표시 여부

    [Header("Prefab Settings")]
    [SerializeField] private GameObject type1Prefab;        // 값이 1일 때 생성할 프리팹
    [SerializeField] private GameObject type2Prefab;        // 값이 2일 때 생성할 프리팹
    [SerializeField] private GameObject type3Prefab;        // 값이 3일 때 생성할 프리팹

    [Header("CSV Loading Settings")]
    [SerializeField] private Object csvFolder;  // 폴더 레퍼런스로 변경
    [SerializeField] private float loadInterval = 5f;
    
    private List<TextAsset> csvFiles = new List<TextAsset>();                    // 로드된 CSV 파일 리스트
    private int currentFileIndex = 0;                                            // 현재 로드 중인 파일 인덱스
    private GameObject currentGridContainer;                                      // 현재 그리드 컨테이너

    private int[,] gridArray;                               // CSV에서 읽어온 그리드 데이터
    private Vector3 gridOrigin;                             // 그리드의 시작점
    private Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();   // 프리팹 딕셔너리 (키: 데이터 값, 값: 프리팹)

    [Header("Spawn Delay Settings")]
    [SerializeField] private float minSpawnDelay = 0.1f;    // 최소 스폰 딜레이
    [SerializeField] private float maxSpawnDelay = 0.5f;    // 최대 스폰 딜레이

    //~ start() 에서는 InitializePrefabDictionary() 메서드를 호출하여 프리팹 딕셔너리를 초기화합니다.
    void Start()
    {
        InitializePrefabDictionary();       // 프리팹 딕셔너리 초기화
        gridOrigin = transform.position;    // 그리드 시작점 설정
        LoadAllCSVFiles();
        StartCoroutine(LoadGridSequentially());
    }

    //~ InitializePrefabDictionary() 메서드는 type1Prefab과 type2Prefab이 할당되어 있을 때만 프리팹 딕셔너리를 초기화합니다.
    private void InitializePrefabDictionary()
    {
        if (type1Prefab != null) prefabDictionary[1] = type1Prefab;
        if (type2Prefab != null) prefabDictionary[2] = type2Prefab;
        if (type3Prefab != null) prefabDictionary[3] = type3Prefab;
    }

    //~ LoadGridFromCSV() 메서드는 CSV 파일을 읽어와서 그리드 데이터를 2차원 배열에 저장합니다.
    private void LoadGridFromCSV()
    {
        if (gridData == null)                               // 만약 CSV 파일이 할당되지 않았다면 에러 메시지를 출력하고 메서드를 종료합니다.
        {
            Debug.LogError("CSV 파일이 할당되지 않았습니다!");
            return;
        }

        // CSV 파일 읽기 및 줄바꿈 문자 처리
        string[] lines = gridData.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
        {
            Debug.LogError("CSV 파일이 비어있습니다!");
            return;
        }

        int rows = lines.Length;
        int cols = lines[0].Trim().Split(',').Length;

        gridArray = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Trim().Split(',');
            if (values.Length != cols)
            {
                Debug.LogError($"CSV 파일의 {i + 1}번째 행의 열 개수가 일치하지 않습니다!");
                continue;
            }

            for (int j = 0; j < cols; j++)
            {
                if (int.TryParse(values[j].Trim(), out int value))
                {
                    gridArray[i, j] = value;
                }
                else
                {
                    Debug.LogError($"CSV 파일의 [{i},{j}] 위치의 값을 숫자로 변환할 수 없습니다: {values[j]}");
                    gridArray[i, j] = 0;
                }
            }
        }
    }

    //~ CreateGrid() 메서드는 2차원 배열에 저장된 그리드 데이터를 기반으로 실제로 그리드를 생성합니다.
    private void CreateGrid()
    {
        if (gridArray == null) return;                              // 그리드 데이터가 없으면 메서드 종료       

        int rows = gridArray.GetLength(0);                          // 행 개수
        int cols = gridArray.GetLength(1);                          // 열 개수

        // 그리드 생성
        currentGridContainer = new GameObject("GridContainer"); // 그리드 컨테이너 생성
        currentGridContainer.transform.parent = transform;                 // 그리드 컨테이너를 GridManager의 자식으로 설정

        // row 순서를 반대로 순회 (rows-1부터 0까지)
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 cellPosition = gridOrigin + new Vector3(
                    col * cellSize,
                    gridHeight,
                    (rows - 1 - row) * cellSize
                );

                CreateCell(cellPosition, row, col, currentGridContainer.transform);    // 셀 생성
            }
        }
    }

    //~ CreateCell() 메서드는 셀을 생성하고 셀에 데이터 값을 저장합니다.
    private void CreateCell(Vector3 position, int row, int col, Transform parent)
    {
        GameObject cell = null;                                                                                 // 셀 오브젝트

        if (showGridCells)                                                                                      // 그리드 셀 표시 여부에 따라 셀 생성
        {               
            cell = GameObject.CreatePrimitive(PrimitiveType.Cube);                                              // 셀 생성 (gridCell.cs)
            cell.transform.parent = parent;                                                                     // 부모 설정
            cell.transform.position = position;                                                                 // 위치 설정
            cell.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);                    // 크기 설정
            cell.name = $"Cell_{row}_{col}";                                                                    // 셀 객체 이름 설정

            GridCell cellComponent = cell.AddComponent<GridCell>();                                             // GridCell 컴포넌트 추가
            cellComponent.Initialize(row, col, gridArray[row, col]);                                            // 셀 초기화
        }               

        // 프리팹 생성을 코루틴으로 변경
        int cellValue = gridArray[row, col];                                                                    // 셀 데이터 값
        if (prefabDictionary.ContainsKey(cellValue))                                                            // 프리팹 딕셔너리에 해당 값이 있을 경우
        {               
            GameObject prefab = prefabDictionary[cellValue];                                                    // 프리팹 가져오기
            if (prefab != null)
            {
                StartCoroutine(SpawnPrefabWithDelay(prefab, position, cell != null ? cell.transform : parent));
            }
        }
    }

    // 새로운 코루틴 메서드 추가
    private System.Collections.IEnumerator SpawnPrefabWithDelay(GameObject prefab, Vector3 position, Transform parent)
    {
        float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        yield return new WaitForSeconds(randomDelay);

        GameObject instance = Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);
        instance.transform.parent = parent;
    }

    private void LoadAllCSVFiles()
    {
        if (csvFolder == null)
        {
            Debug.LogError("CSV 폴더가 설정되지 않았습니다!");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(csvFolder);
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"폴더를 찾을 수 없습니다: {folderPath}");
            return;
        }

        // 폴더 내의 모든 CSV 파일 검색
        string[] csvPaths = Directory.GetFiles(folderPath, "*.csv");
        csvFiles.Clear();

        foreach (string csvPath in csvPaths)
        {
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
            if (textAsset != null)
            {
                csvFiles.Add(textAsset);
            }
        }

        if (csvFiles.Count == 0)
        {
            Debug.LogError($"'{folderPath}' 폴더에서 CSV 파일을 찾을 수 없습니다!");
            return;
        }

        Debug.Log($"총 {csvFiles.Count}개의 CSV 파일을 로드했습니다.");
    }

    private System.Collections.IEnumerator LoadGridSequentially()
    {
        while (true)
        {
            if (csvFiles.Count > 0)
            {
                // 현재 그리드 삭제
                if (currentGridContainer != null)
                {
                    Destroy(currentGridContainer);
                }

                // 다음 CSV 파일 로드
                gridData = csvFiles[currentFileIndex];
                Debug.Log($"레벨 데이터 로드: {csvFiles[currentFileIndex].name} ({currentFileIndex + 1}/{csvFiles.Count})");
                
                LoadGridFromCSV();
                CreateGrid();

                // 다음 파일 인덱스 계산
                currentFileIndex = (currentFileIndex + 1) % csvFiles.Count;
            }

            yield return new WaitForSeconds(loadInterval);
        }
    }
}