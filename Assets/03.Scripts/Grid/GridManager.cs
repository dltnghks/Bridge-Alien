using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;        // 각 격자 셀의 크기
    [SerializeField] private float gridHeight = 0.01f;   // 그리드의 높이
    [SerializeField] private TextAsset gridData;         // CSV 파일 참조
    
    private int[,] gridArray;                           // CSV에서 읽어온 그리드 데이터
    private Vector3 gridOrigin;                         // 그리드의 시작점
    
    void Start()
    {
        gridOrigin = transform.position;
        LoadGridFromCSV();
        CreateGrid();
    }
    
    private void LoadGridFromCSV()
    {
        if (gridData == null)
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
                Debug.LogError($"CSV 파일의 {i+1}번째 행의 열 개수가 일치하지 않습니다!");
                continue;
            }
            
            for (int j = 0; j < cols; j++)
            {
                if (int.TryParse(values[j].Trim(), out int value))
                {
                    gridArray[i,j] = value;
                }
                else
                {
                    Debug.LogError($"CSV 파일의 [{i},{j}] 위치의 값을 숫자로 변환할 수 없습니다: {values[j]}");
                    gridArray[i,j] = 0;
                }
            }
        }
    }
    
    private void CreateGrid()
    {
        if (gridArray == null) return;
        
        int rows = gridArray.GetLength(0);
        int cols = gridArray.GetLength(1);
        
        // 그리드 생성
        GameObject gridContainer = new GameObject("GridContainer");
        gridContainer.transform.parent = transform;
        
        // row 순서를 반대로 순회 (rows-1부터 0까지)
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 cellPosition = gridOrigin + new Vector3(
                    col * cellSize,
                    gridHeight,
                    (rows - 1 - row) * cellSize  // Z 좌표 계산 수정
                );
                
                CreateCell(cellPosition, row, col, gridContainer.transform);
            }
        }
    }
    
    private void CreateCell(Vector3 position, int row, int col, Transform parent)
    {
        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.transform.parent = parent;
        cell.transform.position = position;
        cell.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
        
        // 셀 이름 지정
        cell.name = $"Cell_{row}_{col}";
        
        // 셀에 데이터 값 저장
        GridCell cellComponent = cell.AddComponent<GridCell>();
        cellComponent.Initialize(row, col, gridArray[row,col]);
    }
}