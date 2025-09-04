using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 배달 시나리오 카드의 스와이프 입력을 처리하는 컨트롤러
/// 좌우 스와이프로 정상/비정상 시나리오를 선택할 수 있습니다.
/// </summary>
public class CardSwipeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Card Settings")]
    [SerializeField] private float swipeThreshold = 100f; // 스와이프 인식 최소 거리
    [SerializeField] private float cardReturnSpeed = 10f; // 카드가 원래 위치로 돌아가는 속도
    [SerializeField] private float maxRotation = 30f; // 카드의 최대 회전 각도
    
    [Header("Visual Feedback")]
    [SerializeField] private Image cardImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color abnormalColor = Color.red;
    [SerializeField] private float colorChangeSpeed = 5f;
    
    [Header("Card Content")]
    [SerializeField] private Text scenarioText;
    [SerializeField] private Image deliveryImage;
    
    // 스와이프 상태 관리
    private Vector2 startPosition;
    private Vector2 currentPosition;
    private bool isDragging = false;
    private RectTransform cardRectTransform;
    private Vector2 originalPosition;
    
    // 게임 데이터
    [System.Serializable]
    public class DeliveryScenario
    {
        public string scenarioText;
        public bool isAbnormal; // true면 비정상 시나리오
        public Sprite deliveryImage;
    }
    
    [Header("Scenario Data")]
    [SerializeField] private DeliveryScenario currentScenario;
    
    // 이벤트
    public System.Action<bool> OnScenarioSelected; // bool: true면 abnormal 선택
    public System.Action OnCardReset;
    
    void Start()
    {
        cardRectTransform = GetComponent<RectTransform>();
        originalPosition = cardRectTransform.anchoredPosition;
        
        // 현재 시나리오 표시
        DisplayCurrentScenario();
    }
    
    /// <summary>
    /// 현재 시나리오를 UI에 표시합니다
    /// </summary>
    private void DisplayCurrentScenario()
    {
        if (scenarioText != null && currentScenario != null)
        {
            scenarioText.text = currentScenario.scenarioText;
        }
        
        if (deliveryImage != null && currentScenario?.deliveryImage != null)
        {
            deliveryImage.sprite = currentScenario.deliveryImage;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        startPosition = eventData.position;
        currentPosition = startPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        currentPosition = eventData.position;
        Vector2 dragDelta = currentPosition - startPosition;
        
        // 카드 위치 업데이트
        cardRectTransform.anchoredPosition = originalPosition + dragDelta;
        
        // 카드 회전 효과
        float rotationZ = (dragDelta.x / swipeThreshold) * maxRotation;
        rotationZ = Mathf.Clamp(rotationZ, -maxRotation, maxRotation);
        cardRectTransform.rotation = Quaternion.Euler(0, 0, rotationZ);
        
        // 색상 변경 효과
        UpdateCardVisualFeedback(dragDelta.x);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        Vector2 dragDelta = currentPosition - startPosition;
        
        // 스와이프 판정
        if (Mathf.Abs(dragDelta.x) >= swipeThreshold)
        {
            ProcessSwipe(dragDelta.x > 0);
        }
        else
        {
            // 스와이프 거리가 부족하면 원래 위치로 복귀
            ResetCardPosition();
        }
    }
    
    /// <summary>
    /// 스와이프에 따른 시각적 피드백을 업데이트합니다
    /// </summary>
    private void UpdateCardVisualFeedback(float deltaX)
    {
        if (cardImage == null) return;
        
        float normalizedDelta = Mathf.Abs(deltaX) / swipeThreshold;
        normalizedDelta = Mathf.Clamp01(normalizedDelta);
        
        Color targetColor = deltaX > 0 ? abnormalColor : normalColor;
        Color currentColor = Color.Lerp(normalColor, targetColor, normalizedDelta);
        
        cardImage.color = Color.Lerp(cardImage.color, currentColor, colorChangeSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// 스와이프 처리 - 오른쪽: 비정상, 왼쪽: 정상
    /// </summary>
    private void ProcessSwipe(bool swipeRight)
    {
        bool selectedAbnormal = swipeRight;
        
        // 선택 결과 전달
        OnScenarioSelected?.Invoke(selectedAbnormal);
        
        // 카드 제거 애니메이션 (향후 확장)
        StartCoroutine(RemoveCard(swipeRight));
    }
    
    /// <summary>
    /// 카드를 화면에서 제거하는 애니메이션
    /// </summary>
    private System.Collections.IEnumerator RemoveCard(bool swipeRight)
    {
        float elapsedTime = 0f;
        float animationDuration = 0.5f;
        Vector2 targetPosition = originalPosition + (swipeRight ? Vector2.right : Vector2.left) * 1000f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            
            cardRectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, progress);
            cardRectTransform.rotation = Quaternion.Euler(0, 0, (swipeRight ? 1 : -1) * maxRotation * progress * 2);
            
            yield return null;
        }
        
        // 다음 카드 준비 또는 게임 상태 전환
        OnCardReset?.Invoke();
        ResetCard();
    }
    
    /// <summary>
    /// 카드를 원래 위치로 부드럽게 복귀시킵니다
    /// </summary>
    private void ResetCardPosition()
    {
        StartCoroutine(SmoothReturnToPosition());
    }
    
    private System.Collections.IEnumerator SmoothReturnToPosition()
    {
        Vector2 startPos = cardRectTransform.anchoredPosition;
        Quaternion startRot = cardRectTransform.rotation;
        float elapsedTime = 0f;
        float returnDuration = 0.3f;
        
        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / returnDuration;
            
            cardRectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, progress);
            cardRectTransform.rotation = Quaternion.Lerp(startRot, Quaternion.identity, progress);
            
            if (cardImage != null)
            {
                cardImage.color = Color.Lerp(cardImage.color, normalColor, progress);
            }
            
            yield return null;
        }
        
        cardRectTransform.anchoredPosition = originalPosition;
        cardRectTransform.rotation = Quaternion.identity;
        if (cardImage != null) cardImage.color = normalColor;
    }
    
    /// <summary>
    /// 카드를 초기 상태로 리셋합니다
    /// </summary>
    public void ResetCard()
    {
        cardRectTransform.anchoredPosition = originalPosition;
        cardRectTransform.rotation = Quaternion.identity;
        if (cardImage != null) cardImage.color = normalColor;
        isDragging = false;
    }
    
    /// <summary>
    /// 새로운 시나리오로 카드를 업데이트합니다
    /// </summary>
    public void SetScenario(DeliveryScenario newScenario)
    {
        currentScenario = newScenario;
        DisplayCurrentScenario();
    }
}