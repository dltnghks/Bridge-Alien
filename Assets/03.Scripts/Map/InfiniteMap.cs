using UnityEngine;                      

public class InfiniteMap : MonoBehaviour
{
    [Header("맵 설정")]                                               
    [SerializeField] private float moveSpeed = 5f;                          // 맵 이동 속도
    [SerializeField] private Vector3 moveDirection = Vector3.right;         // 이동 방향
    [SerializeField] private float blockSize = 50f;                         // 각 블록의 크기

    private Transform[] blocks;                                             // 맵 블록들을 저장할 배열
    private bool isInitialized = false;                                     // 초기화 완료 여부
    private float threshold;                                                // 블록 재배치 임계값 (블록 크기의 1.5배가 넘어가면 재배치됩니다 넉넉해야 깜빡이는 현상이 없습니다)

    //~ Start() 메서드에서는 InitializeBlocks() 메서드를 호출하여 맵 블록들을 초기화합니다.
    private void Start()                                                    
    {               
        InitializeBlocks();
    }               

    //~ InitializeBlocks() 메서드는 맵 블록들을 초기화합니다.
    private void InitializeBlocks()
    {
        blocks = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            blocks[i] = transform.GetChild(i);
            if (blocks[i] == null)
            {
                Logger.LogError($"블록 {i}를 찾을 수 없습니다!");
                this.enabled = false;
                return;
            }
        }

        if (blockSize <= 0)
            blockSize = blocks[0].localScale.x;

        threshold = blockSize * 1.5f;

        for (int i = 0; i < blocks.Length; i++)
            blocks[i].localPosition = moveDirection * ((i - 1) * blockSize);

        isInitialized = true;
    }

    //~ Update() 메서드에서는 맵 블록들을 이동시키고 재배치합니다.
    private void Update()
    {               
        if (!isInitialized) return;                                                     

        for (int i = 0; i < blocks.Length; i++)                                         
        {
            blocks[i].localPosition += moveDirection * (moveSpeed * Time.deltaTime);    
            
            float currentPos = Vector3.Dot(blocks[i].localPosition, moveDirection);     
            
            if (currentPos >= threshold)                                                
                blocks[i].localPosition -= moveDirection * (blockSize * 3);
            else if (currentPos <= -threshold)                                          
                blocks[i].localPosition += moveDirection * (blockSize * 3);
        }
    }
}