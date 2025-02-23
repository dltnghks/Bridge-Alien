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
    private void Start()                                                    // 시작 시 호출
    {               
        InitializeBlocks();                                                 // 블록 초기화 실행
    }               

    //~ InitializeBlocks() 메서드는 맵 블록들을 초기화합니다.
    private void InitializeBlocks()                                         // 블록 초기화 메서드
    {               
        blocks = new Transform[3];                                          // 3개의 블록을 위한 배열 생성
        for (int i = 0; i < 3; i++)                                         // 3개의 블록에 대해 반복
        {               
            blocks[i] = transform.GetChild(i);                              // i번째 자식 블록 가져오기
            if (blocks[i] == null)                                          // 블록이 없다면
            {   
                Logger.LogError($"블록 {i}를 찾을 수 없습니다!");             // 에러 메시지 출력
                enabled = false;                                            // 컴포넌트 비활성화
                return;                                                     // 메서드 종료
            }       
        }       

        if (blockSize <= 0)                                                 // 블록 크기가 유효하지 않으면
        {               
            blockSize = blocks[0].localScale.x;                             // 첫 번째 블록의 크기로 설정
        }               

        threshold = blockSize * 1.5f;                                       // 임계값을 블록 크기의 1.5배로 설정

        for (int i = 0; i < blocks.Length; i++)                             // 모든 블록에 대해
        {
            blocks[i].localPosition = moveDirection * ((i - 1) * blockSize); // 초기 위치 설정
        }
        
        isInitialized = true;                                               // 초기화 완료 표시
    }               

    //~ Update() 메서드에서는 맵 블록들을 이동시키고 재배치합니다.
    private void Update()
    {               
        if (!isInitialized) return;                                                     // 초기화 안됐으면 실행하지 않음

        for (int i = 0; i < blocks.Length; i++)                                         // 모든 블록에 대해
        {
            blocks[i].localPosition += moveDirection * (moveSpeed * Time.deltaTime);    // 블록 이동
            
            float currentPos = Vector3.Dot(blocks[i].localPosition, moveDirection);     // 현재 위치 계산
            
            if (currentPos >= threshold)                                                // 임계값을 넘어섰다면
            {
                blocks[i].localPosition -= moveDirection * (blockSize * 3);             // 뒤로 이동
            }
            else if (currentPos <= -threshold)                                          // 반대쪽 임계값을 넘어섰다면
            {
                blocks[i].localPosition += moveDirection * (blockSize * 3);             // 앞으로 이동
            }
        }
    }
}