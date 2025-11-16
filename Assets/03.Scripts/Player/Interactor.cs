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

    private void HandleInteractionInput()
    {
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
            // 1. 화면 좌표를 월드 좌표로 변환 (2D Raycast용)
            Vector2 worldPosition = _eventCamera.ScreenToWorldPoint(screenPosition);
            
            // 2. Raycast 발사
            // Mathf.Infinity 대신 interactionRange를 사용하거나, 2D에서는 필요 없을 수 있음.
            // 여기서는 무한대로 설정하여 터치 지점의 모든 콜라이더를 검사.
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, interactionLayer);

            // 3. 오브젝트 터치/클릭 확인
            if (hit.collider != null)
            {
                GameObject touchedObject = hit.collider.gameObject;
                Debug.Log("입력 감지: " + touchedObject.name);

                // 터치/클릭된 오브젝트가 IInteractable 인터페이스를 가지고 있는지 확인
                if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    Debug.Log("Found and Interacting with: " + interactable.GetInteractionPrompt());
                    // IInteractable 오브젝트를 찾으면 즉시 상호작용 실행
                    interactable.Interact(this);
                }
                else
                {
                    Debug.Log("Touched/Clicked object is not interactable: " + touchedObject.name);
                }
            }
            else
            {
                Debug.Log("No interactable object touched/clicked.");
            }
        }
    }
}
