using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 게임 상태를 표시하는 UI 패널 관리
/// 생존력, 의심도, 점수, 진행도 등을 시각적으로 표현합니다.
/// </summary>
public class GameStatusUI : MonoBehaviour
{
    [Header("Status Panels")]
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject survivalPanel;
    [SerializeField] private GameObject suspicionPanel;
    [SerializeField] private GameObject progressPanel;
    
    [Header("Survival Status")]
    [SerializeField] private Slider survivalBar;
    [SerializeField] private Text survivalText;
    [SerializeField] private Image survivalIcon;
    [SerializeField] private Color safeColor = Color.green;
    [SerializeField] private Color dangerColor = Color.red;
    
    [Header("Suspicion Level")]
    [SerializeField] private Slider suspicionBar;
    [SerializeField] private Text suspicionText;
    [SerializeField] private Image suspicionIcon;
    [SerializeField] private Color lowSuspicionColor = Color.blue;
    [SerializeField] private Color highSuspicionColor = Color.red;
    
    [Header("Game Progress")]
    [SerializeField] private Text dayText;
    [SerializeField] private Text scenarioCountText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Slider dayProgressBar;
    
    [Header("Choice Feedback")]
    [SerializeField] private GameObject choiceFeedbackPanel;
    [SerializeField] private Text choiceResultText;
    [SerializeField] private Image choiceResultIcon;
    [SerializeField] private Sprite correctChoiceIcon;
    [SerializeField] private Sprite incorrectChoiceIcon;
    
    [Header("Animation Settings")]
    [SerializeField] private float statusUpdateSpeed = 2f;
    [SerializeField] private float feedbackDisplayDuration = 2f;
    [SerializeField] private AnimationCurve statusAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // 게임 상태 데이터
    [System.Serializable]
    public class GameStatus
    {
        [Range(0f, 100f)]
        public float survivalLevel = 100f;
        
        [Range(0f, 100f)]
        public float suspicionLevel = 0f;
        
        public int currentDay = 1;
        public int currentScenario = 1;
        public int totalScenarios = 5;
        public int totalScore = 0;
        public int correctChoices = 0;
        public int totalChoices = 0;
        
        public float GetAccuracy()
        {
            return totalChoices > 0 ? (float)correctChoices / totalChoices * 100f : 0f;
        }
        
        public string GetSurvivalStatus()
        {
            if (survivalLevel > 80f) return "안전";
            else if (survivalLevel > 60f) return "주의";
            else if (survivalLevel > 40f) return "위험";
            else if (survivalLevel > 20f) return "극도 위험";
            else return "생존 위기";
        }
        
        public string GetSuspicionStatus()
        {
            if (suspicionLevel < 20f) return "평온";
            else if (suspicionLevel < 40f) return "약간 의심";
            else if (suspicionLevel < 60f) return "의심 중";
            else if (suspicionLevel < 80f) return "고도 의심";
            else return "극도 위험";
        }
    }
    
    [Header("Current Game Status")]
    [SerializeField] private GameStatus currentStatus = new GameStatus();
    
    // 애니메이션 상태
    private Coroutine survivalAnimationCoroutine;
    private Coroutine suspicionAnimationCoroutine;
    private Coroutine feedbackCoroutine;
    
    // 이벤트
    public System.Action<GameStatus> OnStatusChanged;
    public System.Action OnSurvivalCritical; // 생존력 20% 이하
    public System.Action OnSuspicionHigh; // 의심도 80% 이상
    
    void Start()
    {
        InitializeUI();
        UpdateAllUI();
    }
    
    /// <summary>
    /// UI를 초기화합니다
    /// </summary>
    private void InitializeUI()
    {
        // 슬라이더 범위 설정
        if (survivalBar != null)
        {
            survivalBar.minValue = 0f;
            survivalBar.maxValue = 100f;
        }
        
        if (suspicionBar != null)
        {
            suspicionBar.minValue = 0f;
            suspicionBar.maxValue = 100f;
        }
        
        if (dayProgressBar != null)
        {
            dayProgressBar.minValue = 0f;
            dayProgressBar.maxValue = 1f;
        }
        
        // 선택 피드백 패널 숨김
        if (choiceFeedbackPanel != null)
        {
            choiceFeedbackPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 게임 상태를 업데이트합니다
    /// </summary>
    public void UpdateGameStatus(GameStatus newStatus)
    {
        var previousStatus = currentStatus;
        currentStatus = newStatus;
        
        UpdateAllUI();
        
        // 위험 상황 체크
        CheckCriticalStates(previousStatus);
        
        OnStatusChanged?.Invoke(currentStatus);
    }
    
    /// <summary>
    /// 모든 UI 요소를 업데이트합니다
    /// </summary>
    private void UpdateAllUI()
    {
        UpdateSurvivalUI();
        UpdateSuspicionUI();
        UpdateProgressUI();
    }
    
    /// <summary>
    /// 생존력 UI를 업데이트합니다
    /// </summary>
    private void UpdateSurvivalUI()
    {
        if (survivalAnimationCoroutine != null)
        {
            StopCoroutine(survivalAnimationCoroutine);
        }
        
        survivalAnimationCoroutine = StartCoroutine(AnimateSliderValue(
            survivalBar, currentStatus.survivalLevel, statusUpdateSpeed));
        
        if (survivalText != null)
        {
            survivalText.text = $"{currentStatus.GetSurvivalStatus()} ({currentStatus.survivalLevel:F0}%)";
        }
        
        // 생존력에 따른 색상 변경
        if (survivalIcon != null)
        {
            float normalizedValue = currentStatus.survivalLevel / 100f;
            survivalIcon.color = Color.Lerp(dangerColor, safeColor, normalizedValue);
        }
    }
    
    /// <summary>
    /// 의심도 UI를 업데이트합니다
    /// </summary>
    private void UpdateSuspicionUI()
    {
        if (suspicionAnimationCoroutine != null)
        {
            StopCoroutine(suspicionAnimationCoroutine);
        }
        
        suspicionAnimationCoroutine = StartCoroutine(AnimateSliderValue(
            suspicionBar, currentStatus.suspicionLevel, statusUpdateSpeed));
        
        if (suspicionText != null)
        {
            suspicionText.text = $"{currentStatus.GetSuspicionStatus()} ({currentStatus.suspicionLevel:F0}%)";
        }
        
        // 의심도에 따른 색상 변경
        if (suspicionIcon != null)
        {
            float normalizedValue = currentStatus.suspicionLevel / 100f;
            suspicionIcon.color = Color.Lerp(lowSuspicionColor, highSuspicionColor, normalizedValue);
        }
    }
    
    /// <summary>
    /// 진행도 UI를 업데이트합니다
    /// </summary>
    private void UpdateProgressUI()
    {
        if (dayText != null)
        {
            dayText.text = $"Day {currentStatus.currentDay}";
        }
        
        if (scenarioCountText != null)
        {
            scenarioCountText.text = $"{currentStatus.currentScenario}/{currentStatus.totalScenarios}";
        }
        
        if (scoreText != null)
        {
            scoreText.text = $"점수: {currentStatus.totalScore} | 정확도: {currentStatus.GetAccuracy():F1}%";
        }
        
        if (dayProgressBar != null)
        {
            float progress = currentStatus.totalScenarios > 0 ? 
                (float)(currentStatus.currentScenario - 1) / currentStatus.totalScenarios : 0f;
            dayProgressBar.value = progress;
        }
    }
    
    /// <summary>
    /// 슬라이더 값을 부드럽게 애니메이션합니다
    /// </summary>
    private IEnumerator AnimateSliderValue(Slider slider, float targetValue, float duration)
    {
        if (slider == null) yield break;
        
        float startValue = slider.value;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = statusAnimationCurve.Evaluate(elapsedTime / duration);
            
            slider.value = Mathf.Lerp(startValue, targetValue, progress);
            
            yield return null;
        }
        
        slider.value = targetValue;
    }
    
    /// <summary>
    /// 선택 결과 피드백을 표시합니다
    /// </summary>
    public void ShowChoiceFeedback(bool isCorrect, string message = "")
    {
        if (choiceFeedbackPanel == null) return;
        
        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }
        
        feedbackCoroutine = StartCoroutine(ShowChoiceFeedbackCoroutine(isCorrect, message));
    }
    
    /// <summary>
    /// 선택 피드백 표시 코루틴
    /// </summary>
    private IEnumerator ShowChoiceFeedbackCoroutine(bool isCorrect, string message)
    {
        // 피드백 내용 설정
        if (choiceResultText != null)
        {
            if (string.IsNullOrEmpty(message))
            {
                choiceResultText.text = isCorrect ? "정답!" : "오답...";
            }
            else
            {
                choiceResultText.text = message;
            }
            
            choiceResultText.color = isCorrect ? safeColor : dangerColor;
        }
        
        if (choiceResultIcon != null)
        {
            choiceResultIcon.sprite = isCorrect ? correctChoiceIcon : incorrectChoiceIcon;
            choiceResultIcon.color = isCorrect ? safeColor : dangerColor;
        }
        
        // 패널 표시
        choiceFeedbackPanel.SetActive(true);
        
        // 표시 시간 대기
        yield return new WaitForSeconds(feedbackDisplayDuration);
        
        // 패널 숨김
        choiceFeedbackPanel.SetActive(false);
        
        feedbackCoroutine = null;
    }
    
    /// <summary>
    /// 위험 상황을 체크하고 이벤트를 발생시킵니다
    /// </summary>
    private void CheckCriticalStates(GameStatus previousStatus)
    {
        // 생존력 위기 상황
        if (currentStatus.survivalLevel <= 20f && previousStatus.survivalLevel > 20f)
        {
            OnSurvivalCritical?.Invoke();
        }
        
        // 고도 의심 상황
        if (currentStatus.suspicionLevel >= 80f && previousStatus.suspicionLevel < 80f)
        {
            OnSuspicionHigh?.Invoke();
        }
    }
    
    /// <summary>
    /// 생존력을 조정합니다
    /// </summary>
    public void AdjustSurvival(float amount)
    {
        var newStatus = currentStatus;
        newStatus.survivalLevel = Mathf.Clamp(newStatus.survivalLevel + amount, 0f, 100f);
        UpdateGameStatus(newStatus);
    }
    
    /// <summary>
    /// 의심도를 조정합니다
    /// </summary>
    public void AdjustSuspicion(float amount)
    {
        var newStatus = currentStatus;
        newStatus.suspicionLevel = Mathf.Clamp(newStatus.suspicionLevel + amount, 0f, 100f);
        UpdateGameStatus(newStatus);
    }
    
    /// <summary>
    /// 점수를 추가합니다
    /// </summary>
    public void AddScore(int points)
    {
        var newStatus = currentStatus;
        newStatus.totalScore += points;
        UpdateGameStatus(newStatus);
    }
    
    /// <summary>
    /// 선택 결과를 기록합니다
    /// </summary>
    public void RecordChoice(bool isCorrect)
    {
        var newStatus = currentStatus;
        newStatus.totalChoices++;
        
        if (isCorrect)
        {
            newStatus.correctChoices++;
            newStatus.totalScore += 100; // 정답 보너스
        }
        else
        {
            newStatus.totalScore = Mathf.Max(0, newStatus.totalScore - 50); // 오답 페널티
        }
        
        UpdateGameStatus(newStatus);
        ShowChoiceFeedback(isCorrect);
    }
    
    /// <summary>
    /// 다음 시나리오로 진행합니다
    /// </summary>
    public void AdvanceScenario()
    {
        var newStatus = currentStatus;
        newStatus.currentScenario++;
        
        if (newStatus.currentScenario > newStatus.totalScenarios)
        {
            newStatus.currentDay++;
            newStatus.currentScenario = 1;
        }
        
        UpdateGameStatus(newStatus);
    }
    
    /// <summary>
    /// 현재 게임 상태를 반환합니다
    /// </summary>
    public GameStatus GetCurrentStatus()
    {
        return currentStatus;
    }
    
    /// <summary>
    /// 게임 상태를 리셋합니다
    /// </summary>
    public void ResetGameStatus()
    {
        currentStatus = new GameStatus();
        UpdateAllUI();
    }
    
    void OnDestroy()
    {
        if (survivalAnimationCoroutine != null)
            StopCoroutine(survivalAnimationCoroutine);
        if (suspicionAnimationCoroutine != null)
            StopCoroutine(suspicionAnimationCoroutine);
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);
    }
}