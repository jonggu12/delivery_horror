using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임의 전체적인 흐름을 관리하는 메인 컨트롤러
/// 카드 스와이프와 시나리오 매니저를 연결하고 게임 상태를 관리합니다.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CardSwipeController cardSwipeController;
    [SerializeField] private Text dayCounterText;
    [SerializeField] private Text scenarioCounterText;
    [SerializeField] private Text instructionsText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Text debugText;
    [SerializeField] private Button restartButton;
    
    [Header("Game Settings")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private float cardTransitionDelay = 1.5f;
    
    // 게임 상태
    private bool isGameActive = false;
    private int correctChoices = 0;
    private int totalChoices = 0;
    
    void Start()
    {
        InitializeGame();
    }
    
    /// <summary>
    /// 게임을 초기화합니다
    /// </summary>
    private void InitializeGame()
    {
        // ScenarioManager 이벤트 연결
        if (ScenarioManager.Instance != null)
        {
            ScenarioManager.Instance.OnScenarioChanged += UpdateCardWithScenario;
            ScenarioManager.Instance.OnDayChanged += UpdateDayDisplay;
            ScenarioManager.Instance.OnAllScenariosCompleted += OnGameCompleted;
        }
        
        // CardSwipeController 이벤트 연결
        if (cardSwipeController != null)
        {
            cardSwipeController.OnScenarioSelected += OnScenarioSelected;
            cardSwipeController.OnCardReset += OnCardReset;
        }
        
        // UI 초기화
        SetupUI();
        
        // 디버그 패널 설정
        if (debugPanel != null)
        {
            debugPanel.SetActive(showDebugInfo);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        // 게임 시작
        StartGame();
    }
    
    /// <summary>
    /// UI 요소들을 초기화합니다
    /// </summary>
    private void SetupUI()
    {
        if (instructionsText != null)
        {
            instructionsText.text = "← 정상                    비정상 →";
        }
        
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 0f;
        }
        
        UpdateDebugDisplay();
    }
    
    /// <summary>
    /// 게임을 시작합니다
    /// </summary>
    public void StartGame()
    {
        isGameActive = true;
        correctChoices = 0;
        totalChoices = 0;
        
        Debug.Log("배달호러 게임 시작!");
        UpdateDebugDisplay();
    }
    
    /// <summary>
    /// 새로운 시나리오로 카드를 업데이트합니다
    /// </summary>
    private void UpdateCardWithScenario(ScenarioManager.DeliveryScenario scenario)
    {
        if (cardSwipeController == null) return;
        
        // ScenarioManager의 DeliveryScenario를 CardSwipeController.DeliveryScenario로 변환
        var cardScenario = new CardSwipeController.DeliveryScenario
        {
            scenarioText = scenario.description,
            isAbnormal = scenario.isAbnormal,
            deliveryImage = scenario.deliveryImage
        };
        
        cardSwipeController.SetScenario(cardScenario);
        
        UpdateProgressDisplay();
        UpdateDebugDisplay();
    }
    
    /// <summary>
    /// 일차 표시를 업데이트합니다
    /// </summary>
    private void UpdateDayDisplay(int day)
    {
        if (dayCounterText != null)
        {
            dayCounterText.text = $"Day {day}";
        }
        
        UpdateDebugDisplay();
    }
    
    /// <summary>
    /// 진행도 표시를 업데이트합니다
    /// </summary>
    private void UpdateProgressDisplay()
    {
        if (ScenarioManager.Instance == null) return;
        
        var (currentDay, totalScenarios, completedScenarios) = ScenarioManager.Instance.GetGameProgress();
        
        // 시나리오 카운터 업데이트
        if (scenarioCounterText != null)
        {
            scenarioCounterText.text = $"{completedScenarios + 1}/{totalScenarios}";
        }
        
        // 프로그레스 바 업데이트
        if (progressBar != null && totalScenarios > 0)
        {
            progressBar.value = (float)completedScenarios / totalScenarios;
        }
    }
    
    /// <summary>
    /// 시나리오 선택 이벤트 처리
    /// </summary>
    private void OnScenarioSelected(bool selectedAbnormal)
    {
        if (!isGameActive) return;
        
        var currentScenario = ScenarioManager.Instance?.GetCurrentScenario();
        if (currentScenario == null) return;
        
        totalChoices++;
        bool isCorrect = currentScenario.isAbnormal == selectedAbnormal;
        
        if (isCorrect)
        {
            correctChoices++;
            Debug.Log("정답! 올바른 선택입니다.");
        }
        else
        {
            Debug.Log("오답... 다시 생각해보세요.");
        }
        
        // 선택 결과를 ScenarioManager에 전달
        ScenarioManager.Instance?.ProcessScenarioChoice(selectedAbnormal);
        
        UpdateDebugDisplay();
    }
    
    /// <summary>
    /// 카드 리셋 이벤트 처리
    /// </summary>
    private void OnCardReset()
    {
        // 카드가 리셋될 때 필요한 추가 처리
        Debug.Log("카드가 리셋되었습니다.");
    }
    
    /// <summary>
    /// 게임 완료 이벤트 처리
    /// </summary>
    private void OnGameCompleted()
    {
        isGameActive = false;
        
        float accuracy = totalChoices > 0 ? (float)correctChoices / totalChoices * 100f : 0f;
        
        Debug.Log($"게임 완료! 정확도: {accuracy:F1}% ({correctChoices}/{totalChoices})");
        
        // 게임 종료 UI 표시 (향후 구현)
        ShowGameResults(accuracy);
    }
    
    /// <summary>
    /// 게임 결과를 표시합니다
    /// </summary>
    private void ShowGameResults(float accuracy)
    {
        string resultMessage = "";
        
        if (accuracy >= 90f)
        {
            resultMessage = "완벽한 판단력! 당신은 진정한 생존자입니다.";
        }
        else if (accuracy >= 70f)
        {
            resultMessage = "좋은 감각! 대부분의 위험을 감지했습니다.";
        }
        else if (accuracy >= 50f)
        {
            resultMessage = "평균적인 결과. 더 주의 깊게 관찰해보세요.";
        }
        else
        {
            resultMessage = "위험한 판단... 더 신중하게 선택하세요.";
        }
        
        Debug.Log($"결과: {resultMessage}");
        
        // UI에 결과 표시 (향후 구현)
        if (debugText != null)
        {
            debugText.text += $"\\n게임 완료! 정확도: {accuracy:F1}%\\n{resultMessage}";
        }
    }
    
    /// <summary>
    /// 디버그 정보를 업데이트합니다
    /// </summary>
    private void UpdateDebugDisplay()
    {
        if (!showDebugInfo || debugText == null) return;
        
        var progress = ScenarioManager.Instance?.GetGameProgress();
        var currentScenario = ScenarioManager.Instance?.GetCurrentScenario();
        
        string debugInfo = $"=== DEBUG INFO ===\\n";
        debugInfo += $"Game Active: {isGameActive}\\n";
        debugInfo += $"Choices: {correctChoices}/{totalChoices}\\n";
        
        if (progress.HasValue)
        {
            debugInfo += $"Day: {progress.Value.currentDay}\\n";
            debugInfo += $"Scenario: {progress.Value.completedScenarios + 1}/{progress.Value.totalScenarios}\\n";
        }
        
        if (currentScenario != null)
        {
            debugInfo += $"Current: {(currentScenario.isAbnormal ? "ABNORMAL" : "NORMAL")}\\n";
            debugInfo += $"Suspicion: {currentScenario.suspicionLevel:F2}\\n";
        }
        
        debugText.text = debugInfo;
    }
    
    /// <summary>
    /// 게임을 재시작합니다
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("게임을 재시작합니다...");
        
        // 게임 상태 리셋
        isGameActive = false;
        correctChoices = 0;
        totalChoices = 0;
        
        // 카드 리셋
        if (cardSwipeController != null)
        {
            cardSwipeController.ResetCard();
        }
        
        // ScenarioManager 재시작 (새로운 날 시작)
        if (ScenarioManager.Instance != null)
        {
            ScenarioManager.Instance.StartNewDay();
        }
        
        // 게임 재시작
        StartGame();
    }
    
    void OnDestroy()
    {
        // 이벤트 연결 해제
        if (ScenarioManager.Instance != null)
        {
            ScenarioManager.Instance.OnScenarioChanged -= UpdateCardWithScenario;
            ScenarioManager.Instance.OnDayChanged -= UpdateDayDisplay;
            ScenarioManager.Instance.OnAllScenariosCompleted -= OnGameCompleted;
        }
        
        if (cardSwipeController != null)
        {
            cardSwipeController.OnScenarioSelected -= OnScenarioSelected;
            cardSwipeController.OnCardReset -= OnCardReset;
        }
    }
}