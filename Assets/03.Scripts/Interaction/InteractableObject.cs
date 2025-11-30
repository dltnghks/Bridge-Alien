using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string myTag; // 예: "RailA", "RailB"

    public void OnInteract()
    {
        // 튜토리얼 중이라면 허용된 태그인지 확인
        if (TutorialManager.Instance != null) 
        {
            string allowed = TutorialManager.Instance.AllowedInteractableTag;
            
            // 허용된 태그가 비어있지 않은데, 내 태그랑 다르다면 무시
            if (!string.IsNullOrEmpty(allowed) && allowed != myTag)
            {
                Debug.Log("지금은 이 오브젝트와 상호작용할 수 없습니다.");
                return; 
            }
        }

        // 정상 동작 수행
        DoAction();
    }
    
    void DoAction() { /* ... */ }
}