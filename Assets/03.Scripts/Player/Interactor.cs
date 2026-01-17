using UnityEngine;

/// <summary>
/// This component, attached to the player, handles detecting and initiating
/// interactions with IInteractable objects based on touch input.
/// </summary>
[RequireComponent(typeof(HousePlayer))]
public class Interactor : MonoBehaviour
{
    public LayerMask interactionLayer; // 상호작용할 오브젝트들의 레이어

    // A reference to the camera for screen-to-world point conversion.
    // Assign this in the Inspector if it's not the main camera.
    [SerializeField] private Camera _eventCamera;

    private bool _isInteracting = false;

    private void Awake()
    {
        // If not assigned, try to find the main camera.
        if (_eventCamera == null)
        {
            _eventCamera = Camera.main;
        }
    }

    void Update()
    {
        HandleInteractionInput();
    }
    
    /// <summary>
    /// Call this method to signal that the current interaction is complete.
    /// </summary>
    public void InteractionComplete()
    {
        _isInteracting = false;
        Debug.Log("Interaction complete. Ready for new interactions.");
    }

    private void HandleInteractionInput()
    {
        if (Managers.UI != null && Managers.UI.IsBlurActive)
        {
            return;
        }


        // 이미 상호작용 중이면 새로운 입력을 처리하지 않음
        if (_isInteracting)
        {
            return;
        }
        
        Vector2 screenPosition = Vector2.zero;
        bool inputDetected = false;

        // 터치 입력 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                screenPosition = touch.position;
                inputDetected = true;
            }
        }
        // 마우스 클릭 입력 감지 (PC 테스트용)
        else if (Input.GetMouseButtonDown(0))
        {
            screenPosition = Input.mousePosition;
            inputDetected = true;
        }

        if (inputDetected)
        {
            // 1. 화면 좌표를 월드 좌표로 변환 (2D)
            Vector2 worldPosition = _eventCamera.ScreenToWorldPoint(screenPosition);

            // 2. 해당 지점의 모든 콜라이더를 검출
            Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition, interactionLayer);

            IInteractable highestPriorityInteractable = null;
            int maxPriority = int.MinValue;

            // 3. 검출된 콜라이더 중에서 우선순위가 가장 높은 IInteractable 찾기
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    if (interactable.Priority > maxPriority)
                    {
                        maxPriority = interactable.Priority;
                        highestPriorityInteractable = interactable;
                    }
                }
            }

            // 4. 가장 우선순위가 높은 오브젝트와 상호작용
            if (highestPriorityInteractable != null)
            {
                _isInteracting = true;
                Debug.Log("Starting interaction with: " + highestPriorityInteractable.GetInteractionPrompt());
                highestPriorityInteractable.Interact(this);
            }
            else
            {
                Debug.Log("No interactable object touched/clicked.");
            }
        }
    }
}
