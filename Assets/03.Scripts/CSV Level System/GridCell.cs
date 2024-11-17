using UnityEngine;

public class GridCell : MonoBehaviour
{
    //~ GridCell 클래스는 셀의 행, 열, 값 정보를 저장하고 시각화하는 역할을 합니다.
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Value { get; private set; }
    
    //~ Color 배열을 사용하여 셀의 값에 따라 색상을 지정합니다.
    private Color[] cellColors = new Color[] 
    {
        Color.white,      // 0: 빈 셀
        Color.red,        // 1: 첫 번째 타입
        Color.blue,       // 2: 두 번째 타입
        Color.green,      // 3: 세 번째 타입
    };
    
    private Material cellMaterial;  // 셀의 머티리얼 참조 저장
    
    //~ Initialize() 메서드는 셀의 행, 열, 값 정보를 받아와서 셀을 초기화합니다.
    public void Initialize(int row, int col, int value)
    {
        Row = row;                  // 행 정보 저장
        Column = col;               // 열 정보 저장
        Value = value;              // 값 정보 저장
        
        // 콜라이더가 있다면 트리거로 설정
        Collider cellCollider = GetComponent<Collider>();
        if (cellCollider != null)
        {
            cellCollider.isTrigger = true;
        }
        
        UpdateCellVisualization(0f);  // 초기 알파값 0으로 설정
    }
    
    //~ 알파값을 설정하는 새로운 메서드
    public void SetAlpha(float alpha)
    {
        if (cellMaterial != null)
        {
            Color color = cellMaterial.color;
            color.a = alpha;
            cellMaterial.color = color;
        }
    }
    
    //~ UpdateCellVisualization() 메서드는 셀의 값에 따라 색상을 변경합니다.
    private void UpdateCellVisualization(float initialAlpha = 0.7f)
    {
        var renderer = GetComponent<Renderer>();                        // 렌더러 컴포넌트 가져오기
        
        // 기본 머티리얼 설정
        cellMaterial = new Material(Shader.Find("Standard"));      // 표준 머티리얼 생성
        cellMaterial.SetFloat("_Glossiness", 0.2f);                         // 광택 설정
        
        // 값에 따라 색상 설정
        int colorIndex = Mathf.Clamp(Value, 0, cellColors.Length - 1);  // 색상 인덱스 계산
        cellMaterial.color = cellColors[colorIndex];                        // 색상 설정
        
        // 투명도 설정
        cellMaterial.SetFloat("_Mode", 3);                                                          // Transparent 모드
        cellMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);            // SrcAlpha 블렌딩
        cellMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);    // OneMinusSrcAlpha 블렌딩
        cellMaterial.SetInt("_ZWrite", 0);                                                          // ZWrite 비활성화
        cellMaterial.DisableKeyword("_ALPHATEST_ON");                                               // 알파 테스트 비활성화
        cellMaterial.EnableKeyword("_ALPHABLEND_ON");                                               // 알파 블렌딩 활성화
        cellMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");                                        // 사전 알파 블렌딩 비활성화
        cellMaterial.renderQueue = 3000;                                                            // 렌더 큐 설정
        
        cellMaterial.color = new Color(
            cellMaterial.color.r, 
            cellMaterial.color.g, 
            cellMaterial.color.b, 
            initialAlpha
        );
        
        renderer.material = cellMaterial;                                                           // 머티리얼 설정
    }
    
    //~ OnDrawGizmos() 메서드는 셀 위치에 와이어프레임 큐브를 그리고 셀의 값도 표시합니다.
    void OnDrawGizmos()
    {
        // 셀 위치에 와이어프레임 큐브 그리기
        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);                                             // 색상 설정
        Gizmos.DrawWireCube(transform.position, transform.localScale);                          // 와이어프레임 큐브 그리기
        
        // 셀 값 표시
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(                                                              // 레이블 표시
            transform.position + Vector3.up * 0.1f,                                             // 위치 설정
            Value.ToString(),                                                                   // 값 표시     
            new GUIStyle() { 
                normal = new GUIStyleState() { textColor = Color.black },                       // 텍스트 색상 설정
                fontSize = 12,                                                                  // 폰트 크기 설정
                alignment = TextAnchor.MiddleCenter                                             // 텍스트 정렬 설정
            }
        );
        #endif
    }
} 