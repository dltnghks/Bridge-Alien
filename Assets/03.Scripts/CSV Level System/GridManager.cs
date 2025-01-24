using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public enum SpawnOrder
{
    Random,         // 랜덤 생성
    LeftToRight,    // 왼쪽에서 오른쪽으로
    RightToLeft,    // 오른쪽에서 왼쪽으로
    TopToBottom,    // 위에서 아래로
    BottomToTop,    // 아래에서 위로
    Instant         // 즉시 생성
}



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

    [Header("Line Delay Settings")]
    [SerializeField] private float lineDelay = 0.3f;       // 라인 단위로 셀 생성 시 딜레이 (초)

    [Header("Spawn Order Settings")]
    [SerializeField] private SpawnOrder spawnOrder = SpawnOrder.Random;    // 생성 순서 설정

    //~ 초기화 함수
    void Start()
    {
        InitializePrefabDictionary();                        // 프리팹 딕셔너리 초기화
        gridOrigin = transform.position;                     // 그리드 원점 설정
        LoadAllCSVFiles();                                   // 모든 CSV 파일 로드
        
        // 초기 CSV 파일 로드
        if (csvFiles.Count > 0)
        {
            gridData = csvFiles[0];
            LoadGridFromCSV();
        }
    }

    //~ 프리팹과 CSV 값을 매핑하는 딕셔너리 초기화
    private void InitializePrefabDictionary()               // 프리팹과 CSV 값을 매핑하는 딕셔너리 초기화
    {
        if (type1Prefab != null) prefabDictionary[1] = type1Prefab;
        if (type2Prefab != null) prefabDictionary[2] = type2Prefab;
        if (type3Prefab != null) prefabDictionary[3] = type3Prefab;
    }

    //~ CSV 파일에서 그리드 데이터 읽어오기
    private void LoadGridFromCSV()                          // CSV 파일에서 그리드 데이터 읽어오기
    {
        if (gridData == null)
        {
            Logger.LogError("CSV 파일이 할당되지 않았습니다!");
            return;
        }

        string[] lines = gridData.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);    // CSV 파일을 줄 단위로 분리

        if (lines.Length == 0)
        {
            Logger.LogError("CSV 파일이 비어있습니다!");
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
                Logger.LogError($"CSV 파일의 {i + 1}번째 행의 열 개수가 일치하지 않습니다!");
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
                    Logger.LogError($"CSV 파일의 [{i},{j}] 위치의 값을 숫자로 변환할 수 없습니다: {values[j]}");
                    gridArray[i, j] = 0;
                }
            }
        }
    }

    //~ 그리드 데이터를 기반으로 실제 그리드 생성
    private void CreateGrid()                               // 그리드 데이터를 기반으로 실제 그리드 생성
    {
        // CameraManager.Instance.ShakeCamera(0.5f, 0.8f); // 카메라 흔들기 (CameraManager.cs에서 호출)
        if (gridArray == null) return;

        int rows = gridArray.GetLength(0);
        int cols = gridArray.GetLength(1);

        currentGridContainer = new GameObject("GridContainer");
        currentGridContainer.transform.parent = transform;

        // 생성 순서에 따라 반복문 조정
        switch (spawnOrder)
        {
            case SpawnOrder.Random:
                CreateRandomOrder(rows, cols);
                break;
            case SpawnOrder.LeftToRight:
                CreateLeftToRight(rows, cols);
                break;
            case SpawnOrder.RightToLeft:
                CreateRightToLeft(rows, cols);
                break;
            case SpawnOrder.TopToBottom:
                CreateTopToBottom(rows, cols);
                break;
            case SpawnOrder.BottomToTop:
                CreateBottomToTop(rows, cols);
                break;
            case SpawnOrder.Instant:
                CreateInstantOrder(rows, cols);
                break;
        }
    }

    //~ 랜덤 순서로 그리드 생성
    private void CreateRandomOrder(int rows, int cols)
    {
        if (currentGridContainer == null)
        {
            Debug.LogError("그리드 컨테이너가 없습니다!");
            return;
        }

        if (gridArray == null)
        {
            Debug.LogError("그리드 배열이 초기화되지 않았습니다!");
            return;
        }

        List<CellSpawnData> spawnDataList = new List<CellSpawnData>();

        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                int cellValue = gridArray[row, col];
                // 0이 아닌 값을 가진 셀만 생성
                if (cellValue != 0 && prefabDictionary.ContainsKey(cellValue))
                {
                    Vector3 cellPosition = GetCellPosition(row, col, rows);
                    spawnDataList.Add(new CellSpawnData(cellPosition, row, col, currentGridContainer.transform));
                }
            }
        }

        // 모덤 순서로 섞기
        for (int i = spawnDataList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = spawnDataList[i];
            spawnDataList[i] = spawnDataList[j];
            spawnDataList[j] = temp;
        }

        // 모든 셀을 동시에 생성하되, 각각 랜덤한 페이드 효과 적용
        foreach (var spawnData in spawnDataList)
        {
            GameObject cell = null;

            if (showGridCells)
            {
                cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.transform.parent = spawnData.parent;
                cell.transform.position = spawnData.position;
                cell.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
                cell.name = $"Cell_{spawnData.row}_{spawnData.col}";

                GridCell cellComponent = cell.AddComponent<GridCell>();
                cellComponent.Initialize(spawnData.row, spawnData.col, gridArray[spawnData.row, spawnData.col]);
                cellComponent.SetAlpha(0f); // 초기 알파값 0으로 설정
            }

            int cellValue = gridArray[spawnData.row, spawnData.col];
            if (prefabDictionary.ContainsKey(cellValue))
            {
                GameObject prefab = prefabDictionary[cellValue];
                if (prefab != null)
                {
                    StartCoroutine(SpawnPrefabWithDelay(prefab, spawnData.position,
                        cell != null ? cell.transform : spawnData.parent));
                }
            }
        }
    }

    //~ 왼쪽에서 오른쪽으로 그리드 프리팹 생성
    private void CreateLeftToRight(int rows, int cols)
    {
        if (gridArray == null || currentGridContainer == null) return;  // 그리드 배열이나 컨테이너가 없으면 종료
        
        List<CellSpawnData> spawnDataList = new List<CellSpawnData>();

        // 각 열을 하나의 라인으로 처리 (왼쪽에서 오른쪽으로)
        for (int col = 0; col < cols; col++)
        {
            List<CellSpawnData> lineData = new List<CellSpawnData>();
            // 위에서 아래로 셀 추가
            for (int row = rows - 1; row >= 0; row--)
            {
                Vector3 cellPosition = GetCellPosition(row, col, rows);
                lineData.Add(new CellSpawnData(cellPosition, row, col, currentGridContainer.transform));
            }
            spawnDataList.AddRange(lineData);
        }

        StartCoroutine(SpawnLinesSequentially(spawnDataList, rows));
    }

    //~ 오른쪽에서 왼쪽으로 그리드 프리팹 생성
    private void CreateRightToLeft(int rows, int cols)
    {
        List<CellSpawnData> spawnDataList = new List<CellSpawnData>();

        // 각 열을 하나의 라인으로 처리 (오른쪽에서 왼쪽으로)
        for (int col = cols - 1; col >= 0; col--)
        {
            List<CellSpawnData> lineData = new List<CellSpawnData>();
            // 아래에서 위로 셀 추가
            for (int row = 0; row < rows; row++)
            {
                Vector3 cellPosition = GetCellPosition(row, col, rows);
                lineData.Add(new CellSpawnData(cellPosition, row, col, currentGridContainer.transform));
            }
            spawnDataList.AddRange(lineData);
        }

        StartCoroutine(SpawnLinesSequentially(spawnDataList, rows));
    }

    //~ 위에서 아래로 그리드 프리팹 생성
    private void CreateTopToBottom(int rows, int cols)
    {
        if (currentGridContainer == null)
        {
            Debug.LogError("그리드 컨테이너가 없습니다!");
            return;
        }

        List<CellSpawnData> spawnDataList = new List<CellSpawnData>();

        // 각 행을 하나의 라인으로 처리 (위에서 아래로)
        for (int row = 0; row < rows; row++)
        {
            List<CellSpawnData> lineData = new List<CellSpawnData>();
            // 왼쪽에서 오른쪽으로 셀 추가
            for (int col = 0; col < cols; col++)
            {
                if (gridArray[row, col] != 0) // 값이 0이 아닌 경우만 생성
                {
                    Vector3 cellPosition = GetCellPosition(row, col, rows);
                    lineData.Add(new CellSpawnData(cellPosition, row, col, currentGridContainer.transform));
                }
            }
            spawnDataList.AddRange(lineData);
        }

        StartCoroutine(SpawnLinesSequentially(spawnDataList, cols));
    }

    //~ 아래에서 위로 그리드 프리팹 생성
    private void CreateBottomToTop(int rows, int cols)
    {
        if (currentGridContainer == null)
        {
            Debug.LogError("그리드 컨테이너가 없습니다!");
            return;
        }

        if (gridArray == null)
        {
            Debug.LogError("그리드 배열이 초기화되지 않았습니다!");
            return;
        }

        List<CellSpawnData> spawnDataList = new List<CellSpawnData>();

        // 각 행을 하나의 라인으로 처리 (아래에서 위로)
        for (int row = rows - 1; row >= 0; row--)
        {
            List<CellSpawnData> lineData = new List<CellSpawnData>();
            // 왼쪽에서 오른쪽으로 셀 추가
            for (int col = 0; col < cols; col++)
            {
                if (gridArray[row, col] != 0) // 값이 0이 아닌 경우만 생성
                {
                    Vector3 cellPosition = GetCellPosition(row, col, rows);
                    lineData.Add(new CellSpawnData(cellPosition, row, col, currentGridContainer.transform));
                }
            }
            spawnDataList.AddRange(lineData);
        }

        StartCoroutine(SpawnLinesSequentially(spawnDataList, cols));
    }

    //~ 즉시 그리드 프리팹 생성
    private void CreateInstantOrder(int rows, int cols)
    {
        List<CellSpawnData> spawnDataList = new List<CellSpawnData>();

        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                int cellValue = gridArray[row, col];
                // 0이 아닌 값을 가진 셀만 생성
                if (cellValue != 0 && prefabDictionary.ContainsKey(cellValue))
                {
                    Vector3 cellPosition = GetCellPosition(row, col, rows);
                    spawnDataList.Add(new CellSpawnData(cellPosition, row, col, currentGridContainer.transform));
                }
            }
        }

        // 모든 셀을 동시에 생성하되, 페이드 효과는 유지
        foreach (var spawnData in spawnDataList)
        {
            GameObject cell = null;

            if (showGridCells)
            {
                cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.transform.parent = spawnData.parent;
                cell.transform.position = spawnData.position;
                cell.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
                cell.name = $"Cell_{spawnData.row}_{spawnData.col}";

                GridCell cellComponent = cell.AddComponent<GridCell>();
                cellComponent.Initialize(spawnData.row, spawnData.col, gridArray[spawnData.row, spawnData.col]);
                cellComponent.SetAlpha(0f); // 초기 알파값 0으로 설정
            }

            int cellValue = gridArray[spawnData.row, spawnData.col];
            if (prefabDictionary.ContainsKey(cellValue))
            {
                GameObject prefab = prefabDictionary[cellValue];
                if (prefab != null)
                {
                    StartCoroutine(SpawnPrefabWithDelay(prefab, spawnData.position,
                        cell != null ? cell.transform : spawnData.parent));
                }
            }
        }
    }

    //~ 셀 위치 계산
    private Vector3 GetCellPosition(int row, int col, int totalRows)
    {
        return gridOrigin + new Vector3(
            col * cellSize,
            gridHeight,
            (totalRows - 1 - row) * cellSize
        );
    }

    //~ 개별 그리드 셀 생성
    private void CreateCell(Vector3 position, int row, int col, Transform parent)
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

    //~ 프리팹 생성 딜레이 처리
    private System.Collections.IEnumerator SpawnPrefabWithDelay(GameObject prefab, Vector3 position, Transform parent)
    {
        float delay = spawnOrder == SpawnOrder.Random ?
            Random.Range(minSpawnDelay, maxSpawnDelay) :
            minSpawnDelay;

        // 셀 컴포넌트 가져오기
        GridCell cell = parent.GetComponent<GridCell>();
        if (cell != null)
        {
            // 딜레이 동안 알파값 서서히 증가
            float elapsedTime = 0f;
            while (elapsedTime < delay)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / delay);
                cell.SetAlpha(alpha);
                yield return null;
            }
            cell.SetAlpha(1f);

            // 프리팹 생성 및 EffectPrefab 컴포넌트 추가
            GameObject instance = Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);
            instance.transform.parent = parent;
            instance.AddComponent<EffectPrefab>();

            // 페이드아웃 시작
            StartCoroutine(FadeOutCell(cell));
        }
        else
        {
            yield return new WaitForSeconds(delay);
            // 프리팹 생성 및 EffectPrefab 컴포넌트 추가
            GameObject instance = Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);
            instance.transform.parent = parent;
            instance.AddComponent<EffectPrefab>();
        }
    }

    //~ 개별 셀을 페이드아웃하는 코루틴
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

    //~ 지정된 폴더에서 모든 CSV 파일 로드
    private void LoadAllCSVFiles()                         
    {
        if (csvFolder == null)
        {
            Logger.LogError("CSV 폴더가 설정되지 않았습니다!");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(csvFolder);
        if (!Directory.Exists(folderPath))
        {
            Logger.LogError($"폴더를 찾을 수 없습니다: {folderPath}");
            return;
        }

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
            Logger.LogError($"'{folderPath}' 폴더에서 CSV 파일을 찾을 수 없습니다!");
            return;
        }
    }

    //~ 셀을 순차적으로 생성
    private System.Collections.IEnumerator SpawnCellsSequentially(List<CellSpawnData> spawnDataList)
    {
        foreach (var spawnData in spawnDataList)
        {
            GameObject cell = null;

            if (showGridCells)
            {
                cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.transform.parent = spawnData.parent;
                cell.transform.position = spawnData.position;
                cell.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
                cell.name = $"Cell_{spawnData.row}_{spawnData.col}";

                GridCell cellComponent = cell.AddComponent<GridCell>();
                cellComponent.Initialize(spawnData.row, spawnData.col, gridArray[spawnData.row, spawnData.col]);
            }

            int cellValue = gridArray[spawnData.row, spawnData.col];
            if (prefabDictionary.ContainsKey(cellValue))
            {
                GameObject prefab = prefabDictionary[cellValue];
                if (prefab != null)
                {
                    yield return StartCoroutine(SpawnPrefabWithDelay(prefab, spawnData.position,
                        cell != null ? cell.transform : spawnData.parent));
                }
            }

            // 각 셀 생성 후 약간의 딜레이
            yield return new WaitForSeconds(0.1f);
        }
    }

    //~ 라인 단위로 셀 생성
    private System.Collections.IEnumerator SpawnLinesSequentially(List<CellSpawnData> spawnDataList, int colCount)
    {
        // 라인 단위로 처리
        for (int i = 0; i < spawnDataList.Count; i += colCount)
        {
            // 현재 라인의 모든 셀을 동시에 생성
            for (int j = 0; j < colCount && (i + j) < spawnDataList.Count; j++)
            {
                var spawnData = spawnDataList[i + j];
                GameObject cell = null;

                if (showGridCells)
                {
                    cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cell.transform.parent = spawnData.parent;
                    cell.transform.position = spawnData.position;
                    cell.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
                    cell.name = $"Cell_{spawnData.row}_{spawnData.col}";

                    GridCell cellComponent = cell.AddComponent<GridCell>();
                    cellComponent.Initialize(spawnData.row, spawnData.col, gridArray[spawnData.row, spawnData.col]);
                }

                int cellValue = gridArray[spawnData.row, spawnData.col];
                if (prefabDictionary.ContainsKey(cellValue))
                {
                    GameObject prefab = prefabDictionary[cellValue];
                    if (prefab != null)
                    {
                        StartCoroutine(SpawnPrefabWithDelay(prefab, spawnData.position,
                            cell != null ? cell.transform : spawnData.parent));
                    }
                }
            }

            // 한 라인이 완성되면 다음 라인까지 대기
            yield return new WaitForSeconds(lineDelay);
        }
    }

    //~ 셀 생성 데이터를 저장하는 구조체
    private struct CellSpawnData
    {
        public Vector3 position;
        public int row;
        public int col;
        public Transform parent;

        public CellSpawnData(Vector3 pos, int r, int c, Transform p)
        {
            position = pos;
            row = r;
            col = c;
            parent = p;
        }
    }

    //~ 그리드 초기화를 위한 새로운 메서드
    private void InitializeGridFromCSV()
    {
        LoadGridFromCSV();
        gridOrigin = transform.position;
        InitializePrefabDictionary();
    }

    // 이전 그리드 제거를 위한 메서드 추가
    private void ClearCurrentGrid()
    {
        if (currentGridContainer != null)
        {
            Destroy(currentGridContainer);
            currentGridContainer = null;
        }
    }

    // 각 패턴 실행 메서드 수정
    public void ExecuteRandomPattern()
    {
        ClearCurrentGrid();
        
        // CSV 파일 로드 체크
        if (csvFiles.Count == 0)
        {
            LoadAllCSVFiles();
            if (csvFiles.Count == 0)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다!");
                return;
            }
        }
        
        // 그리드 컨테이너 생성
        currentGridContainer = new GameObject("GridContainer");
        currentGridContainer.transform.parent = transform;
        
        // CSV 데이터 로드 및 초기화
        gridData = csvFiles[Random.Range(0, csvFiles.Count)];
        InitializeGridFromCSV();
        
        // 스폰 순서를 Random으로 설정
        spawnOrder = SpawnOrder.Random;
        
        // 그리드 배열이 제대로 초기화되었는지 확인
        if (gridArray == null)
        {
            Debug.LogError("그리드 배열이 초기화되지 않았습니다!");
            return;
        }
        
        CreateGrid(); // CreateRandomOrder가 호출됨
    }

    public void ExecuteLeftToRightPattern()
    {
        ClearCurrentGrid();
        
        // CSV 파일 로드 체크
        if (csvFiles.Count == 0)
        {
            LoadAllCSVFiles();
            if (csvFiles.Count == 0)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다!");
                return;
            }
        }
        
        // 그리드 컨테이너 생성
        currentGridContainer = new GameObject("GridContainer");
        currentGridContainer.transform.parent = transform;
        
        // CSV 데이터 로드 및 초기화
        gridData = csvFiles[Random.Range(0, csvFiles.Count)];
        InitializeGridFromCSV();
        
        // 스폰 순서를 LeftToRight로 설정
        spawnOrder = SpawnOrder.LeftToRight;
        
        // 그리드 배열이 제대로 초기화되었는지 확인
        if (gridArray == null)
        {
            Debug.LogError("그리드 배열이 초기화되지 않았습니다!");
            return;
        }
        
        CreateGrid(); // CreateLeftToRight가 호출됨
    }

    public void ExecuteRightToLeftPattern()
    {
        ClearCurrentGrid();
        
        // CSV 파일 로드 체크
        if (csvFiles.Count == 0)
        {
            LoadAllCSVFiles();
            if (csvFiles.Count == 0)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다!");
                return;
            }
        }
        
        // 그리드 컨테이너 생성
        currentGridContainer = new GameObject("GridContainer");
        currentGridContainer.transform.parent = transform;
        
        // CSV 데이터 로드 및 초기화
        gridData = csvFiles[Random.Range(0, csvFiles.Count)];
        InitializeGridFromCSV();
        
        // 스폰 순서를 RightToLeft로 설정
        spawnOrder = SpawnOrder.RightToLeft;
        
        // 그리드 배열이 제대로 초기화되었는지 확인
        if (gridArray == null)
        {
            Debug.LogError("그리드 배열이 초기화되지 않았습니다!");
            return;
        }
        
        CreateGrid(); // CreateRightToLeft가 호출됨
    }

    public void ExecuteTopToBottomPattern()
    {
        ClearCurrentGrid();
        if (csvFiles.Count == 0)
        {
            LoadAllCSVFiles();
        }
        
        gridData = csvFiles[Random.Range(0, csvFiles.Count)];
        InitializeGridFromCSV();
        
        // 스폰 순서를 TopToBottom으로 설정
        spawnOrder = SpawnOrder.TopToBottom;
        
        CreateGrid(); // CreateTopToBottom이 호출됨
    }

    public void ExecuteBottomToTopPattern()
    {
        ClearCurrentGrid();
        
        // CSV 파일 로드 체크
        if (csvFiles.Count == 0)
        {
            LoadAllCSVFiles();
            if (csvFiles.Count == 0)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다!");
                return;
            }
        }
        
        // 그리드 컨테이너 생성
        currentGridContainer = new GameObject("GridContainer");
        currentGridContainer.transform.parent = transform;
        
        // CSV 데이터 로드 및 초기화
        gridData = csvFiles[Random.Range(0, csvFiles.Count)];
        InitializeGridFromCSV();
        
        // 스폰 순서를 BottomToTop으로 설정
        spawnOrder = SpawnOrder.BottomToTop;
        
        // 그리드 배열이 제대로 초기화되었는지 확인
        if (gridArray == null)
        {
            Debug.LogError("그리드 배열이 초기화되지 않았습니다!");
            return;
        }
        
        CreateGrid(); // CreateBottomToTop이 호출됨
    }

    public void ExecuteInstantPattern()
    {
        ClearCurrentGrid();
        
        // CSV 파일 로드 체크
        if (csvFiles.Count == 0)
        {
            LoadAllCSVFiles();
            if (csvFiles.Count == 0)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다!");
                return;
            }
        }
        
        // 그리드 컨테이너 생성
        currentGridContainer = new GameObject("GridContainer");
        currentGridContainer.transform.parent = transform;
        
        // CSV 데이터 로드 및 초기화
        gridData = csvFiles[Random.Range(0, csvFiles.Count)];
        InitializeGridFromCSV();
        
        // 스폰 순서를 Instant로 설정
        spawnOrder = SpawnOrder.Instant;
        
        // 그리드 배열이 제대로 초기화되었는지 확인
        if (gridArray == null)
        {
            Debug.LogError("그리드 배열이 초기화되지 않았습니다!");
            return;
        }
        
        CreateGrid(); // CreateInstantOrder가 호출됨
    }
}