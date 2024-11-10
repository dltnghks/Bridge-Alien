using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Value { get; private set; }
    
    private Color[] cellColors = new Color[] 
    {
        Color.white,      // 0: 빈 셀
        Color.red,        // 1: 첫 번째 타입
        Color.blue,       // 2: 두 번째 타입
        Color.green,      // 3: 세 번째 타입
    };
    
    public void Initialize(int row, int col, int value)
    {
        Row = row;
        Column = col;
        Value = value;
        
        // 셀 시각화
        UpdateCellVisualization();
    }
    
    private void UpdateCellVisualization()
    {
        var renderer = GetComponent<Renderer>();
        
        // 기본 머티리얼 설정
        Material material = new Material(Shader.Find("Standard"));
        material.SetFloat("_Glossiness", 0.2f);
        
        // 값에 따라 색상 설정
        int colorIndex = Mathf.Clamp(Value, 0, cellColors.Length - 1);
        material.color = cellColors[colorIndex];
        
        // 약간의 투명도 추가
        material.SetFloat("_Mode", 3); // Transparent 모드
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        
        material.color = new Color(
            material.color.r, 
            material.color.g, 
            material.color.b, 
            0.7f
        );
        
        renderer.material = material;
    }
    
    void OnDrawGizmos()
    {
        // 셀 위치에 와이어프레임 큐브 그리기
        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        // 셀 값 표시
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.1f, 
            Value.ToString(),
            new GUIStyle() { 
                normal = new GUIStyleState() { textColor = Color.black },
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            }
        );
        #endif
    }
} 