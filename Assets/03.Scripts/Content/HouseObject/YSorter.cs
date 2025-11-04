using UnityEngine;

public class YSorter : MonoBehaviour
{
    // 부모 오브젝트의 SpriteRenderer를 여기에 할당합니다.
    private SpriteRenderer parentSpriteRenderer;

    [SerializeField]
    private float sortOrderMultiplier = 100f; 
    
    [SerializeField]
    private float sortOrderOffset = 0f;

    void Start()
    {
        // 부모 오브젝트에 있는 SpriteRenderer를 찾아옵니다.
        parentSpriteRenderer = GetComponentInParent<SpriteRenderer>();

        if (parentSpriteRenderer == null)
        {
            Debug.LogError("부모 오브젝트에서 SpriteRenderer를 찾을 수 없습니다!");
        }
    }
    
    void Update()
    {
        if (parentSpriteRenderer != null)
        {
            // 정렬의 기준은 "나"(SortPoint)의 Y 좌표입니다.
            float newY = transform.position.y + sortOrderOffset;

            // 정렬 순서 적용은 "부모"(캐릭터)의 SpriteRenderer에 합니다.
            parentSpriteRenderer.sortingOrder = Mathf.RoundToInt(-newY * sortOrderMultiplier);
        }
    }
}