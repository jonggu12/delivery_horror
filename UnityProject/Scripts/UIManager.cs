using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 게임의 모든 UI 요소를 통합적으로 관리하는 메인 UI 매니저
/// 각종 UI 패널들의 상호작용과 전체적인 UI 플로우를 조율합니다.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private DeliveryNotificationUI notificationUI;
    [SerializeField] private GameStatusUI statusUI;
    [SerializeField] private PlayerChoicePanel choicePanel;
    [SerializeField] private CardSwipeController cardSwipeController;
    
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    [Header("Game UI")]
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("End Game")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitToMenuButton;
    
    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle notificationsToggle;
    [SerializeField] private Button settingsCloseButton;
    
    [Header("Loading")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingProgressBar;
    [SerializeField] private Text loadingText;
    
    // 게임 상태
    public enum UIState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Settings,
        Loading
    }
    
    [Header("Current State")]
    [SerializeField] private UIState currentState = UIState.MainMenu;
    
    // 싱글톤 패턴
    public static UIManager Instance { get; private set; }
    
    // 이벤트
    public System.Action OnGameStarted;
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    public System.Action OnGameRestarted;
    public System.Action OnReturnToMainMenu;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeUI();
        SetupEventHandlers();
        SetUIState(UIState.MainMenu);
    }
    
    /// <summary>
    /// UI를 초기화합니다
    /// </summary>
    private void InitializeUI()
    {
        // 각 UI 컴포넌트 초기화
        if (notificationUI != null)
        {
            notificationUI.OnNotificationShown += OnNotificationShown;
            notificationUI.OnNotificationHidden += OnNotificationHidden;
        }
        
        if (statusUI != null)
        {
            statusUI.OnSurvivalCritical += OnSurvivalCritical;
            statusUI.OnSuspicionHigh += OnSuspicionHigh;
        }
        
        if (choicePanel != null)
        {
            choicePanel.OnChoiceSelected += OnPlayerChoiceSelected;
            choicePanel.OnChoiceTimedOut += OnChoiceTimedOut;
            choicePanel.OnQuickActionUsed += OnQuickActionUsed;
        }
        
        if (cardSwipeController != null)
        {
            cardSwipeController.OnScenarioSelected += OnScenarioSwipeSelected;
        }
        
        // 볼륨 설정 로드
        LoadSettings();
    }
    
    /// <summary>
    /// UI 버튼 이벤트 핸들러를 설정합니다
    /// </summary>
    private void SetupEventHandlers()
    {
        // 메인 메뉴 버튼들
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => SetUIState(UIState.Settings));
        if (exitButton != null)
            exitButton.onClick.AddListener(QuitGame);
        
        // 게임 UI 버튼들
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        // 게임 오버 버튼들
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (quitToMenuButton != null)
            quitToMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        // 설정 버튼들
        if (settingsCloseButton != null)
            settingsCloseButton.onClick.AddListener(() => SetUIState(UIState.MainMenu));
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (notificationsToggle != null)
            notificationsToggle.onValueChanged.AddListener(OnNotificationsToggleChanged);
    }
    
    /// <summary>
    /// UI 상태를 변경합니다
    /// </summary>
    public void SetUIState(UIState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        UpdateUIVisibility();
    }
    
    /// <summary>
    /// 현재 상태에 따라 UI 패널들의 가시성을 업데이트합니다
    /// </summary>
    private void UpdateUIVisibility()
    {
        // 모든 패널 비활성화
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        
        // 현재 상태에 맞는 패널 활성화
        switch (currentState)
        {
            case UIState.MainMenu:
                if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
                Time.timeScale = 1f;
                break;
                
            case UIState.Playing:
                if (gameUIPanel != null) gameUIPanel.SetActive(true);
                Time.timeScale = 1f;
                break;
                
            case UIState.Paused:
                if (gameUIPanel != null) gameUIPanel.SetActive(true);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
                Time.timeScale = 0f;
                break;
                
            case UIState.GameOver:
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                Time.timeScale = 1f;
                break;
                
            case UIState.Settings:
                if (settingsPanel != null) settingsPanel.SetActive(true);
                break;
                
            case UIState.Loading:
                if (loadingPanel != null) loadingPanel.SetActive(true);
                break;
        }
    }
    
    /// <summary>
    /// 게임을 시작합니다
    /// </summary>
    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
    
    /// <summary>
    /// 게임 시작 코루틴
    /// </summary>
    private IEnumerator StartGameCoroutine()
    {
        SetUIState(UIState.Loading);
        
        // 로딩 시뮬레이션
        yield return StartCoroutine(ShowLoadingProgress("게임 초기화 중...", 0.3f));
        yield return StartCoroutine(ShowLoadingProgress("리소스 로딩 중...", 0.7f));
        yield return StartCoroutine(ShowLoadingProgress("게임 시작...", 1.0f));
        
        // 게임 상태 초기화
        if (statusUI != null)
        {
            statusUI.ResetGameStatus();
        }
        
        SetUIState(UIState.Playing);
        OnGameStarted?.Invoke();
    }
    
    /// <summary>
    /// 로딩 진행률을 표시합니다
    /// </summary>
    private IEnumerator ShowLoadingProgress(string message, float targetProgress)
    {
        if (loadingText != null)
        {
            loadingText.text = message;
        }
        
        float currentProgress = loadingProgressBar != null ? loadingProgressBar.value : 0f;
        float elapsedTime = 0f;
        float duration = 1f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Lerp(currentProgress, targetProgress, elapsedTime / duration);
            
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = progress;
            }
            
            yield return null;
        }
        
        if (loadingProgressBar != null)
        {
            loadingProgressBar.value = targetProgress;
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    /// <summary>
    /// 게임을 일시정지합니다
    /// </summary>
    public void PauseGame()
    {
        SetUIState(UIState.Paused);
        OnGamePaused?.Invoke();
    }
    
    /// <summary>
    /// 게임을 재개합니다
    /// </summary>
    public void ResumeGame()
    {
        SetUIState(UIState.Playing);
        OnGameResumed?.Invoke();
    }
    
    /// <summary>
    /// 메인 메뉴로 돌아갑니다
    /// </summary>
    public void ReturnToMainMenu()
    {
        SetUIState(UIState.MainMenu);
        OnReturnToMainMenu?.Invoke();
    }
    
    /// <summary>
    /// 게임을 재시작합니다
    /// </summary>
    public void RestartGame()
    {
        OnGameRestarted?.Invoke();
        StartGame();
    }
    
    /// <summary>
    /// 게임을 종료합니다
    /// </summary>
    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    
    /// <summary>
    /// 게임 오버를 표시합니다
    /// </summary>
    public void ShowGameOver(string message, int finalScore)
    {
        if (gameOverText != null)
        {
            gameOverText.text = message;
        }
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"최종 점수: {finalScore}";
        }
        
        SetUIState(UIState.GameOver);
    }
    
    /// <summary>
    /// 배달 알림을 표시합니다
    /// </summary>
    public void ShowDeliveryNotification(string title, string message, bool isAbnormal = false)
    {
        if (notificationUI != null)
        {
            if (isAbnormal)
            {
                notificationUI.ShowAbnormalNotification(title, message);
            }
            else
            {
                notificationUI.ShowDeliveryNotification(title, message);
            }
        }
    }
    
    /// <summary>
    /// 플레이어 선택지를 표시합니다
    /// </summary>
    public void ShowPlayerChoices(PlayerChoicePanel.SituationData situation)
    {
        if (choicePanel != null)
        {
            choicePanel.ShowChoices(situation);
        }
    }
    
    /// <summary>
    /// 게임 상태를 업데이트합니다
    /// </summary>
    public void UpdateGameStatus(GameStatusUI.GameStatus status)
    {
        if (statusUI != null)
        {
            statusUI.UpdateGameStatus(status);
        }
    }
    
    // 이벤트 핸들러들
    private void OnNotificationShown()
    {
        Debug.Log("알림이 표시되었습니다.");
    }
    
    private void OnNotificationHidden()
    {
        Debug.Log("알림이 숨겨졌습니다.");
    }
    
    private void OnSurvivalCritical()
    {
        ShowDeliveryNotification("위험!", "생존력이 위험 수준입니다!", true);
    }
    
    private void OnSuspicionHigh()
    {
        ShowDeliveryNotification("경고", "의심 수준이 높아졌습니다!", true);
    }
    
    private void OnPlayerChoiceSelected(PlayerChoicePanel.ChoiceOption choice)
    {
        Debug.Log($"플레이어가 '{choice.text}'을(를) 선택했습니다.");
    }
    
    private void OnChoiceTimedOut()
    {
        Debug.Log("선택 시간이 초과되었습니다!");
        ShowDeliveryNotification("시간 초과", "결정하지 못했습니다...", true);
    }
    
    private void OnQuickActionUsed(PlayerChoicePanel.ChoiceType actionType)
    {
        Debug.Log($"퀵 액션 사용: {actionType}");
    }
    
    private void OnScenarioSwipeSelected(bool selectedAbnormal)
    {
        Debug.Log($"카드 스와이프: {(selectedAbnormal ? "비정상" : "정상")} 선택");
    }
    
    // 설정 관련 메서드들
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        SaveSettings();
    }
    
    private void OnNotificationsToggleChanged(bool enabled)
    {
        // 알림 설정 변경
        SaveSettings();
    }
    
    private void LoadSettings()
    {
        if (volumeSlider != null)
        {
            float volume = PlayerPrefs.GetFloat("Volume", 1f);
            volumeSlider.value = volume;
            AudioListener.volume = volume;
        }
        
        if (notificationsToggle != null)
        {
            bool notifications = PlayerPrefs.GetInt("Notifications", 1) == 1;
            notificationsToggle.isOn = notifications;
        }
    }
    
    private void SaveSettings()
    {
        if (volumeSlider != null)
        {
            PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        }
        
        if (notificationsToggle != null)
        {
            PlayerPrefs.SetInt("Notifications", notificationsToggle.isOn ? 1 : 0);
        }
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 현재 UI 상태를 반환합니다
    /// </summary>
    public UIState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// 게임이 진행 중인지 확인합니다
    /// </summary>
    public bool IsGamePlaying()
    {
        return currentState == UIState.Playing;
    }
    
    void OnDestroy()
    {
        // 이벤트 연결 해제
        if (notificationUI != null)
        {
            notificationUI.OnNotificationShown -= OnNotificationShown;
            notificationUI.OnNotificationHidden -= OnNotificationHidden;
        }
        
        if (statusUI != null)
        {
            statusUI.OnSurvivalCritical -= OnSurvivalCritical;
            statusUI.OnSuspicionHigh -= OnSuspicionHigh;
        }
        
        if (choicePanel != null)
        {
            choicePanel.OnChoiceSelected -= OnPlayerChoiceSelected;
            choicePanel.OnChoiceTimedOut -= OnChoiceTimedOut;
            choicePanel.OnQuickActionUsed -= OnQuickActionUsed;
        }
        
        if (cardSwipeController != null)
        {
            cardSwipeController.OnScenarioSelected -= OnScenarioSwipeSelected;
        }
    }
}