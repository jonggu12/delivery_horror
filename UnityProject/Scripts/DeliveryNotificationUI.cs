using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 배달 앱 스타일의 푸시 알림 UI를 관리하는 클래스
/// 실제 배달 앱처럼 화면 상단에서 슬라이드 다운되는 알림을 표시합니다.
/// </summary>
public class DeliveryNotificationUI : MonoBehaviour
{
    [Header("Notification Panel")]
    [SerializeField] private RectTransform notificationPanel;
    [SerializeField] private Image notificationBackground;
    [SerializeField] private Image appIcon;
    [SerializeField] private Text appName;
    [SerializeField] private Text notificationTitle;
    [SerializeField] private Text notificationMessage;
    [SerializeField] private Text timeStamp;
    [SerializeField] private Button dismissButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float slideOutDuration = 0.3f;
    [SerializeField] private AnimationCurve slideInCurve = AnimationCurve.EaseOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve slideOutCurve = AnimationCurve.EaseIn(0, 0, 1, 1);
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalNotificationColor = new Color(0.2f, 0.7f, 0.3f, 0.9f);
    [SerializeField] private Color abnormalNotificationColor = new Color(0.8f, 0.3f, 0.3f, 0.9f);
    [SerializeField] private Color urgentNotificationColor = new Color(0.9f, 0.6f, 0.1f, 0.9f);
    
    [Header("App Icons")]
    [SerializeField] private Sprite deliveryAppIcon;
    [SerializeField] private Sprite unknownAppIcon;
    
    // 알림 데이터 구조
    [System.Serializable]
    public class NotificationData
    {
        public string appName;
        public string title;
        public string message;
        public NotificationType type;
        public Sprite customIcon;
        public bool autoHide;
        
        public NotificationData(string appName, string title, string message, NotificationType type = NotificationType.Normal)
        {
            this.appName = appName;
            this.title = title;
            this.message = message;
            this.type = type;
            this.autoHide = true;
        }
    }
    
    public enum NotificationType
    {
        Normal,     // 일반 배달 알림
        Abnormal,   // 비정상 상황 알림
        Urgent,     // 긴급 알림
        System      // 시스템 메시지
    }
    
    // 상태 관리
    private bool isNotificationVisible = false;
    private Coroutine currentNotificationCoroutine;
    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;
    
    // 이벤트
    public System.Action OnNotificationShown;
    public System.Action OnNotificationHidden;
    public System.Action<NotificationData> OnNotificationClicked;
    
    void Start()
    {
        InitializeNotificationPanel();
        SetupEventHandlers();
    }
    
    /// <summary>
    /// 알림 패널을 초기화합니다
    /// </summary>
    private void InitializeNotificationPanel()
    {
        if (notificationPanel == null)
        {
            Debug.LogError("Notification Panel이 할당되지 않았습니다!");
            return;
        }
        
        // 초기 위치 설정 (화면 위쪽 숨김)
        RectTransform canvasRect = notificationPanel.root.GetComponent<RectTransform>();
        float panelHeight = notificationPanel.rect.height;
        
        hiddenPosition = new Vector2(0, canvasRect.rect.height * 0.5f + panelHeight * 0.5f);
        visiblePosition = new Vector2(0, canvasRect.rect.height * 0.5f - panelHeight * 0.5f - 50f); // 50px 마진
        
        // 초기 상태는 숨김
        notificationPanel.anchoredPosition = hiddenPosition;
        notificationPanel.gameObject.SetActive(false);
        
        // 기본 앱 이름 설정
        if (appName != null)
        {
            appName.text = "배달의공포";
        }
    }
    
    /// <summary>
    /// 이벤트 핸들러를 설정합니다
    /// </summary>
    private void SetupEventHandlers()
    {
        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(() => HideNotification(true));
        }
        
        // 패널 클릭으로도 알림 숨기기 (선택사항)
        if (notificationPanel != null)
        {
            Button panelButton = notificationPanel.gameObject.GetComponent<Button>();
            if (panelButton == null)
            {
                panelButton = notificationPanel.gameObject.AddComponent<Button>();
            }
            panelButton.onClick.AddListener(() => HideNotification(true));
        }
    }
    
    /// <summary>
    /// 알림을 표시합니다
    /// </summary>
    public void ShowNotification(NotificationData data)
    {
        if (isNotificationVisible)
        {
            // 이미 알림이 표시 중이면 기존 것을 숨기고 새 것을 표시
            if (currentNotificationCoroutine != null)
            {
                StopCoroutine(currentNotificationCoroutine);
            }
            HideNotification(false);
        }
        
        currentNotificationCoroutine = StartCoroutine(ShowNotificationCoroutine(data));
    }
    
    /// <summary>
    /// 알림 표시 코루틴
    /// </summary>
    private IEnumerator ShowNotificationCoroutine(NotificationData data)
    {
        // 알림 내용 설정
        SetupNotificationContent(data);
        
        // 패널 활성화
        notificationPanel.gameObject.SetActive(true);
        isNotificationVisible = true;
        
        // 슬라이드 인 애니메이션
        yield return StartCoroutine(AnimateSlideIn());
        
        OnNotificationShown?.Invoke();
        
        // 자동 숨김이 설정된 경우 대기
        if (data.autoHide)
        {
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(AnimateSlideOut());
        }
        
        currentNotificationCoroutine = null;
    }
    
    /// <summary>
    /// 알림 내용을 설정합니다
    /// </summary>
    private void SetupNotificationContent(NotificationData data)
    {
        // 앱 이름 설정
        if (appName != null && !string.IsNullOrEmpty(data.appName))
        {
            appName.text = data.appName;
        }
        
        // 제목 설정
        if (notificationTitle != null)
        {
            notificationTitle.text = data.title;
        }
        
        // 메시지 설정
        if (notificationMessage != null)
        {
            notificationMessage.text = data.message;
        }
        
        // 시간 스탬프 설정
        if (timeStamp != null)
        {
            timeStamp.text = System.DateTime.Now.ToString("HH:mm");
        }
        
        // 아이콘 설정
        if (appIcon != null)
        {
            if (data.customIcon != null)
            {
                appIcon.sprite = data.customIcon;
            }
            else
            {
                appIcon.sprite = data.type == NotificationType.Abnormal ? unknownAppIcon : deliveryAppIcon;
            }
        }
        
        // 배경색 설정
        if (notificationBackground != null)
        {
            Color backgroundColor = GetNotificationColor(data.type);
            notificationBackground.color = backgroundColor;
        }
    }
    
    /// <summary>
    /// 알림 타입에 따른 색상을 반환합니다
    /// </summary>
    private Color GetNotificationColor(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Normal:
                return normalNotificationColor;
            case NotificationType.Abnormal:
                return abnormalNotificationColor;
            case NotificationType.Urgent:
                return urgentNotificationColor;
            case NotificationType.System:
                return Color.gray;
            default:
                return normalNotificationColor;
        }
    }
    
    /// <summary>
    /// 슬라이드 인 애니메이션
    /// </summary>
    private IEnumerator AnimateSlideIn()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = hiddenPosition;
        
        while (elapsedTime < slideInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = slideInCurve.Evaluate(elapsedTime / slideInDuration);
            
            notificationPanel.anchoredPosition = Vector2.Lerp(startPosition, visiblePosition, progress);
            
            yield return null;
        }
        
        notificationPanel.anchoredPosition = visiblePosition;
    }
    
    /// <summary>
    /// 슬라이드 아웃 애니메이션
    /// </summary>
    private IEnumerator AnimateSlideOut()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = visiblePosition;
        
        while (elapsedTime < slideOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = slideOutCurve.Evaluate(elapsedTime / slideOutDuration);
            
            notificationPanel.anchoredPosition = Vector2.Lerp(startPosition, hiddenPosition, progress);
            
            yield return null;
        }
        
        notificationPanel.anchoredPosition = hiddenPosition;
        notificationPanel.gameObject.SetActive(false);
        isNotificationVisible = false;
        
        OnNotificationHidden?.Invoke();
    }
    
    /// <summary>
    /// 알림을 숨깁니다
    /// </summary>
    public void HideNotification(bool immediate = false)
    {
        if (!isNotificationVisible) return;
        
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
            currentNotificationCoroutine = null;
        }
        
        if (immediate)
        {
            notificationPanel.anchoredPosition = hiddenPosition;
            notificationPanel.gameObject.SetActive(false);
            isNotificationVisible = false;
            OnNotificationHidden?.Invoke();
        }
        else
        {
            StartCoroutine(AnimateSlideOut());
        }
    }
    
    /// <summary>
    /// 편의 메서드: 일반 배달 알림
    /// </summary>
    public void ShowDeliveryNotification(string title, string message)
    {
        var notification = new NotificationData("배달의공포", title, message, NotificationType.Normal);
        ShowNotification(notification);
    }
    
    /// <summary>
    /// 편의 메서드: 비정상 상황 알림
    /// </summary>
    public void ShowAbnormalNotification(string title, string message)
    {
        var notification = new NotificationData("???", title, message, NotificationType.Abnormal);
        notification.autoHide = false; // 수동으로 닫아야 함
        ShowNotification(notification);
    }
    
    /// <summary>
    /// 편의 메서드: 긴급 알림
    /// </summary>
    public void ShowUrgentNotification(string title, string message)
    {
        var notification = new NotificationData("긴급 상황", title, message, NotificationType.Urgent);
        notification.autoHide = false;
        ShowNotification(notification);
    }
    
    /// <summary>
    /// 현재 알림이 표시 중인지 확인
    /// </summary>
    public bool IsNotificationVisible => isNotificationVisible;
    
    void OnDestroy()
    {
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }
    }
}