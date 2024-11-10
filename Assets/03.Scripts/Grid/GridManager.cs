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
    [SerializeField] private Object csvFolder;  // CSV 파일들이 있는 폴더
    [SerializeField] private float loadInterval = 5f;       // CSV 파일 로드 간격 (초)
    
    private List<TextAsset> csvFiles = new List<TextAsset>();    // 로드된 CSV 파일들을 저장하는 리스트
    private int currentFileIndex = 0;                            // 현재 처리 중인 CSV 파일의 인덱스
    private GameObject currentGridContainer;                      // 현재 활성화된 그리드를 담는 컨테이너

    private int[,] gridArray;                               // CSV 데이터를 저장하는 2차원 배열
    private Vector3 gridOrigin;                             // 그리드가 시작되는 월드 좌표
    private Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();   // CSV 값과 프리팹을 매핑하는 딕셔너리

    [Header("Spawn Delay Settings")]
    [SerializeField] private float minSpawnDelay = 0.1f;    // 프리팹 생성 최소 대기 시간
    [SerializeField] private float maxSpawnDelay = 0.5f;    // 프리팹 생성 최대 대기 시간

    [Header("Fade Out Settings")]
    [SerializeField] private float fadeOutDuration = 1f;    // 셀을 페이드아웃하는 데 걸리는 시간 (초)

    void Start()                                            // 초기화 함수
    {
        InitializePrefabDictionary();                       // 프리팹 딕셔너리 초기화
        gridOrigin = transform.position;                    // 그리드 시작점 설정
        LoadAllCSVFiles();                                  // 모든 CSV 파일 로드
        StartCoroutine(LoadGridSequentially());             // 순차적 그리드 로드 시작
    }

    private void InitializePrefabDictionary()               // 프리팹과 CSV 값을 매핑하는 딕셔너리 초기화
    {
        if (type1Prefab != null) prefabDictionary[1] = type1Prefab;
        if (type2Prefab != null) prefabDictionary[2] = type2Prefab;
        if (type3Prefab != null) prefabDictionary[3] = type3Prefab;
    }

    private void LoadGridFromCSV()                          // CSV 파일에서 그리드 데이터 읽어오기
    {
        if (gridData == null)                               
        {
            Debug.LogError("CSV 파일이 할당되지 않았습니다!");
            return;
        }

        string[] lines = gridData.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);    // CSV 파일을 줄 단위로 분리

        if (lines.Length == 0)
        {
            Debug.LogError("CSV 파일이 비어있습니다!");
            return;
        }

        int rows = lines.Length;                            // CSV 파일의 행 수
        int cols = lines[0].Trim().Split(',').Length;       // CSV 파일의 열 수

        gridArray = new int[rows, cols];                    // 그리드 배열 초기화

        for (int i = 0; i < rows; i++)                      // CSV 데이터를 2차원 배열로 변환
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

    private void CreateGrid()                               // 그리드 데이터를 기반으로 실제 그리드 생성
    {
        if (gridArray == null) return;                      

        int rows = gridArray.GetLength(0);                  
        int cols = gridArray.GetLength(1);                  

        currentGridContainer = new GameObject("GridContainer");    // 새로운 그리드 컨테이너 생성
        currentGridContainer.transform.parent = transform;         

        for (int row = rows - 1; row >= 0; row--)          // 그리드 셀 생성 (아래에서 위로)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 cellPosition = gridOrigin + new Vector3(
                    col * cellSize,
                    gridHeight,
                    (rows - 1 - row) * cellSize
                );

                CreateCell(cellPosition, row, col, currentGridContainer.transform);
            }
        }
    }

    private void CreateCell(Vector3 position, int row, int col, Transform parent)    // 개별 그리드 셀 생성
    {
        GameObject cell = null;                             

        if (showGridCells)                                  // 그리드 셀 시각화가 활성화된 경우
        {               
            cell = GameObject.CreatePrimitive(PrimitiveType.Cube);    // 큐브로 셀 생성
            cell.transform.parent = parent;                 
            cell.transform.position = position;             
            cell.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
            cell.name = $"Cell_{row}_{col}";               

            GridCell cellComponent = cell.AddComponent<GridCell>();    // 셀에 GridCell 컴포넌트 추가
            cellComponent.Initialize(row, col, gridArray[row, col]);   
        }               

        int cellValue = gridArray[row, col];               // CSV 값에 따른 프리팹 생성
        if (prefabDictionary.ContainsKey(cellValue))       
        {               
            GameObject prefab = prefabDictionary[cellValue];
            if (prefab != null)
            {
                StartCoroutine(SpawnPrefabWithDelay(prefab, position, cell != null ? cell.transform : parent));
            }
        }
    }

    private System.Collections.IEnumerator SpawnPrefabWithDelay(GameObject prefab, Vector3 position, Transform parent)    // 딜레이를 가진 프리팹 생성
    {
        float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        
        // 셀 컴포넌트 가져오기
        GridCell cell = parent.GetComponent<GridCell>();
        if (cell != null)
        {
            // 딜레이 동안 알파값 서서히 증가
            float elapsedTime = 0f;
            while (elapsedTime < randomDelay)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / randomDelay);
                cell.SetAlpha(alpha);
                yield return null;
            }
            cell.SetAlpha(1f);

            // 프리팹 생성
            GameObject instance = Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);
            instance.transform.parent = parent;

            // 페이드아웃 시작
            StartCoroutine(FadeOutCell(cell));
        }
        else
        {
            yield return new WaitForSeconds(randomDelay);
            // 프리팹만 생성
            GameObject instance = Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);
            instance.transform.parent = parent;
        }
    }

    // 개별 셀을 페이드아웃하는 코루틴
    private System.Collections.IEnumerator FadeOutCell(GridCell cell)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            cell.SetAlpha(alpha);
            yield return null;
        }
        
        // 최종적으로 완전히 투명하게 설정
        cell.SetAlpha(0f);
    }

    private void LoadAllCSVFiles()                         // 지정된 폴더에서 모든 CSV 파일 로드
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

        string[] csvPaths = Directory.GetFiles(folderPath, "*.csv");    // CSV 파일 검색
        csvFiles.Clear();

        foreach (string csvPath in csvPaths)               // 찾은 CSV 파일들을 리스트에 추가
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

    private System.Collections.IEnumerator LoadGridSequentially()    // CSV 파일들을 순차적으로 로드
    {
        while (true)
        {
            if (csvFiles.Count > 0)
            {
                if (currentGridContainer != null)           // 이전 그리드 제거
                {
                    Destroy(currentGridContainer);
                }

                gridData = csvFiles[currentFileIndex];      // 다음 CSV 파일 로드
                Debug.Log($"레벨 데이터 로드: {csvFiles[currentFileIndex].name} ({currentFileIndex + 1}/{csvFiles.Count})");
                
                LoadGridFromCSV();                         // 그리드 데이터 로드
                CreateGrid();                              // 새 그리드 생성

                currentFileIndex = (currentFileIndex + 1) % csvFiles.Count;    // 다음 파일 인덱스 계산
            }

            yield return new WaitForSeconds(loadInterval);    // 다음 로드까지 대기
        }
    }
}