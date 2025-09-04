using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 플레이어가 상황에 따라 선택할 수 있는 다양한 대응 옵션을 제공하는 UI 패널
/// 문 열기, 경찰 신고, 녹화 시작 등의 선택지를 동적으로 표시합니다.
/// </summary>
public class PlayerChoicePanel : MonoBehaviour
{
    [Header("Choice Panel")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Text situationText;
    [SerializeField] private Transform choiceButtonContainer;
    [SerializeField] private Button choiceButtonPrefab;
    
    [Header("Quick Actions")]
    [SerializeField] private GameObject quickActionPanel;
    [SerializeField] private Button callPoliceButton;
    [SerializeField] private Button lockDoorButton;
    [SerializeField] private Button startRecordingButton;
    [SerializeField] private Button peekThroughDoorButton;
    
    [Header("Timer")]
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Text timerText;
    [SerializeField] private float defaultChoiceTime = 30f;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalChoiceColor = new Color(0.2f, 0.6f, 0.9f, 1f);
    [SerializeField] private Color dangerousChoiceColor = new Color(0.9f, 0.4f, 0.3f, 1f);
    [SerializeField] private Color safeChoiceColor = new Color(0.3f, 0.8f, 0.4f, 1f);
    [SerializeField] private Color timerWarningColor = Color.red;
    [SerializeField] private Color timerNormalColor = Color.white;
    
    [Header("Animation Settings")]
    [SerializeField] private float panelFadeInDuration = 0.5f;
    [SerializeField] private float buttonAppearDelay = 0.1f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseOut(0, 0, 1, 1);
    
    // 선택지 데이터 구조
    [System.Serializable]
    public class ChoiceOption
    {
        public string text;
        public string description;
        public ChoiceType type;
        public float riskLevel; // 0-1, 위험도
        public int consequenceValue; // 선택 결과에 미치는 영향
        public bool requiresConfirmation;
        public System.Action onSelected;
        
        public ChoiceOption(string text, ChoiceType type, float riskLevel = 0.5f)
        {
            this.text = text;
            this.type = type;
            this.riskLevel = riskLevel;
            this.requiresConfirmation = riskLevel > 0.7f;
        }
    }
    
    public enum ChoiceType
    {
        OpenDoor,       // 문 열기
        CallPolice,     // 경찰 신고
        LockDoor,       // 문 잠그기
        StartRecording, // 녹화 시작
        PeekThrough,    // 문구멍으로 엿보기
        IgnoreDelivery, // 배달 무시
        TalkToDelivery, // 배달원과 대화
        CheckCCTV,      // CCTV 확인
        CallNeighbor,   // 이웃에게 연락
        WaitAndSee      // 지켜보기
    }
    
    // 상황 데이터
    [System.Serializable]
    public class SituationData
    {
        public string situationDescription;
        public List<ChoiceOption> availableChoices;
        public float timeLimit;
        public bool showQuickActions;
        
        public SituationData(string description)
        {
            situationDescription = description;
            availableChoices = new List<ChoiceOption>();
            timeLimit = 30f;
            showQuickActions = true;
        }
    }
    
    // 상태 관리
    private SituationData currentSituation;
    private bool isChoiceActive = false;
    private Coroutine timerCoroutine;
    private List<Button> currentChoiceButtons = new List<Button>();
    private CanvasGroup panelCanvasGroup;
    
    // 이벤트
    public System.Action<ChoiceOption> OnChoiceSelected;
    public System.Action OnChoiceTimedOut;
    public System.Action<ChoiceType> OnQuickActionUsed;
    
    void Start()
    {
        InitializePanel();
        SetupQuickActions();
    }
    
    /// <summary>
    /// 패널을 초기화합니다
    /// </summary>
    private void InitializePanel()
    {
        // CanvasGroup 설정 (페이드 효과용)
        panelCanvasGroup = choicePanel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = choicePanel.AddComponent<CanvasGroup>();
        }
        
        // 초기 상태: 패널 숨김
        choicePanel.SetActive(false);
        quickActionPanel.SetActive(false);
        
        // 타이머 설정
        if (timerSlider != null)
        {
            timerSlider.minValue = 0f;
            timerSlider.maxValue = 1f;
        }
    }
    
    /// <summary>
    /// 퀵 액션 버튼들을 설정합니다
    /// </summary>
    private void SetupQuickActions()
    {
        if (callPoliceButton != null)
        {
            callPoliceButton.onClick.AddListener(() => ExecuteQuickAction(ChoiceType.CallPolice));
        }
        
        if (lockDoorButton != null)
        {
            lockDoorButton.onClick.AddListener(() => ExecuteQuickAction(ChoiceType.LockDoor));
        }
        
        if (startRecordingButton != null)
        {
            startRecordingButton.onClick.AddListener(() => ExecuteQuickAction(ChoiceType.StartRecording));
        }
        
        if (peekThroughDoorButton != null)
        {
            peekThroughDoorButton.onClick.AddListener(() => ExecuteQuickAction(ChoiceType.PeekThrough));
        }
    }
    
    /// <summary>
    /// 선택지를 표시합니다
    /// </summary>
    public void ShowChoices(SituationData situation)
    {
        if (isChoiceActive)
        {
            HideChoices();
        }
        
        currentSituation = situation;
        isChoiceActive = true;
        
        StartCoroutine(ShowChoicesCoroutine());
    }
    
    /// <summary>
    /// 선택지 표시 코루틴
    /// </summary>
    private IEnumerator ShowChoicesCoroutine()
    {
        // 패널 활성화
        choicePanel.SetActive(true);
        quickActionPanel.SetActive(currentSituation.showQuickActions);
        
        // 상황 설명 설정
        if (situationText != null)
        {
            situationText.text = currentSituation.situationDescription;
        }
        
        // 페이드 인 애니메이션
        yield return StartCoroutine(FadeInPanel());
        
        // 선택지 버튼 생성
        CreateChoiceButtons();
        
        // 타이머 시작
        if (currentSituation.timeLimit > 0)
        {
            StartChoiceTimer(currentSituation.timeLimit);
        }
    }
    
    /// <summary>
    /// 패널 페이드 인 애니메이션
    /// </summary>
    private IEnumerator FadeInPanel()
    {
        panelCanvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < panelFadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = fadeInCurve.Evaluate(elapsedTime / panelFadeInDuration);
            panelCanvasGroup.alpha = progress;
            
            yield return null;
        }
        
        panelCanvasGroup.alpha = 1f;
    }
    
    /// <summary>
    /// 선택지 버튼들을 생성합니다
    /// </summary>
    private void CreateChoiceButtons()
    {
        // 기존 버튼 제거
        ClearChoiceButtons();
        
        if (choiceButtonPrefab == null || choiceButtonContainer == null)
        {
            Debug.LogError("Choice button prefab 또는 container가 설정되지 않았습니다!");
            return;
        }
        
        // 새 버튼 생성
        for (int i = 0; i < currentSituation.availableChoices.Count; i++)
        {
            var choice = currentSituation.availableChoices[i];
            var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            
            SetupChoiceButton(button, choice, i);
            currentChoiceButtons.Add(button);
        }
    }
    
    /// <summary>
    /// 개별 선택지 버튼을 설정합니다
    /// </summary>
    private void SetupChoiceButton(Button button, ChoiceOption choice, int index)
    {
        // 버튼 텍스트 설정
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = choice.text;
        }
        
        // 버튼 색상 설정 (위험도에 따라)
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (choice.riskLevel > 0.7f)
            {
                buttonImage.color = dangerousChoiceColor;
            }
            else if (choice.riskLevel < 0.3f)
            {
                buttonImage.color = safeChoiceColor;
            }
            else
            {
                buttonImage.color = normalChoiceColor;
            }
        }
        
        // 버튼 클릭 이벤트 설정
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectChoice(choice));
        
        // 버튼 등장 애니메이션
        StartCoroutine(AnimateButtonAppearance(button, index));
    }
    
    /// <summary>
    /// 버튼 등장 애니메이션
    /// </summary>
    private IEnumerator AnimateButtonAppearance(Button button, int index)
    {
        CanvasGroup buttonGroup = button.GetComponent<CanvasGroup>();
        if (buttonGroup == null)
        {
            buttonGroup = button.gameObject.AddComponent<CanvasGroup>();
        }
        
        // 초기 상태: 투명
        buttonGroup.alpha = 0f;
        
        // 지연 시간
        yield return new WaitForSeconds(index * buttonAppearDelay);
        
        // 페이드 인
        float elapsedTime = 0f;
        float duration = 0.3f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            buttonGroup.alpha = elapsedTime / duration;
            yield return null;
        }
        
        buttonGroup.alpha = 1f;
    }
    
    /// <summary>
    /// 선택지를 선택합니다
    /// </summary>
    private void SelectChoice(ChoiceOption choice)
    {
        if (!isChoiceActive) return;
        
        if (choice.requiresConfirmation)
        {
            ShowConfirmationDialog(choice);
        }
        else
        {
            ExecuteChoice(choice);
        }
    }
    
    /// <summary>
    /// 확인 대화상자를 표시합니다
    /// </summary>
    private void ShowConfirmationDialog(ChoiceOption choice)
    {
        string confirmMessage = $"정말로 '{choice.text}'을(를) 선택하시겠습니까?";
        if (!string.IsNullOrEmpty(choice.description))
        {
            confirmMessage += $"\\n\\n{choice.description}";
        }
        
        // 간단한 확인 UI (실제 구현에서는 별도의 확인 패널 사용)
        Debug.Log($"확인 필요: {confirmMessage}");
        
        // 임시로 바로 실행 (실제로는 확인 후 실행)
        ExecuteChoice(choice);
    }
    
    /// <summary>
    /// 선택을 실행합니다
    /// </summary>
    private void ExecuteChoice(ChoiceOption choice)
    {
        OnChoiceSelected?.Invoke(choice);
        choice.onSelected?.Invoke();
        
        HideChoices();
    }
    
    /// <summary>
    /// 퀵 액션을 실행합니다
    /// </summary>
    private void ExecuteQuickAction(ChoiceType actionType)
    {
        if (!isChoiceActive) return;
        
        OnQuickActionUsed?.Invoke(actionType);
        
        // 퀵 액션에 따른 즉시 효과
        switch (actionType)
        {
            case ChoiceType.CallPolice:
                Debug.Log("경찰에 신고했습니다!");
                break;
            case ChoiceType.LockDoor:
                Debug.Log("문을 잠갔습니다!");
                break;
            case ChoiceType.StartRecording:
                Debug.Log("녹화를 시작했습니다!");
                break;
            case ChoiceType.PeekThrough:
                Debug.Log("문구멍으로 살펴봤습니다!");
                break;
        }
        
        HideChoices();
    }
    
    /// <summary>
    /// 선택 타이머를 시작합니다
    /// </summary>
    private void StartChoiceTimer(float timeLimit)
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        timerCoroutine = StartCoroutine(ChoiceTimerCoroutine(timeLimit));
    }
    
    /// <summary>
    /// 선택 타이머 코루틴
    /// </summary>
    private IEnumerator ChoiceTimerCoroutine(float timeLimit)
    {
        if (timerPanel != null)
        {
            timerPanel.SetActive(true);
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < timeLimit && isChoiceActive)
        {
            elapsedTime += Time.deltaTime;
            float progress = 1f - (elapsedTime / timeLimit);
            
            // 타이머 UI 업데이트
            if (timerSlider != null)
            {
                timerSlider.value = progress;
            }
            
            if (timerText != null)
            {
                int remainingSeconds = Mathf.CeilToInt(timeLimit - elapsedTime);
                timerText.text = $"{remainingSeconds}초";
                
                // 경고 색상 변경
                if (progress < 0.3f)
                {
                    timerText.color = timerWarningColor;
                }
                else
                {
                    timerText.color = timerNormalColor;
                }
            }
            
            yield return null;
        }
        
        // 시간 초과
        if (isChoiceActive)
        {
            OnChoiceTimedOut?.Invoke();
            HideChoices();
        }
        
        timerCoroutine = null;
    }
    
    /// <summary>
    /// 선택지를 숨깁니다
    /// </summary>
    public void HideChoices()
    {
        isChoiceActive = false;
        
        // 타이머 중단
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        
        // 패널 숨김
        choicePanel.SetActive(false);
        quickActionPanel.SetActive(false);
        
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
        
        ClearChoiceButtons();
    }
    
    /// <summary>
    /// 생성된 선택지 버튼들을 제거합니다
    /// </summary>
    private void ClearChoiceButtons()
    {
        foreach (var button in currentChoiceButtons)
        {
            if (button != null)
            {
                DestroyImmediate(button.gameObject);
            }
        }
        currentChoiceButtons.Clear();
    }
    
    /// <summary>
    /// 미리 정의된 상황별 선택지를 생성합니다
    /// </summary>
    public SituationData CreateDeliveryArrivalSituation(bool isAbnormal)
    {
        string description = isAbnormal ? 
            "이상한 배달원이 문 앞에 서 있습니다. 어떻게 대응하시겠습니까?" :
            "배달원이 도착했습니다. 어떻게 하시겠습니까?";
            
        var situation = new SituationData(description);
        
        if (isAbnormal)
        {
            situation.availableChoices.AddRange(new ChoiceOption[]
            {
                new ChoiceOption("조심스럽게 문 열기", ChoiceType.OpenDoor, 0.8f),
                new ChoiceOption("경찰에 신고하기", ChoiceType.CallPolice, 0.2f),
                new ChoiceOption("문구멍으로 확인하기", ChoiceType.PeekThrough, 0.4f),
                new ChoiceOption("무시하고 기다리기", ChoiceType.IgnoreDelivery, 0.6f)
            });
        }
        else
        {
            situation.availableChoices.AddRange(new ChoiceOption[]
            {
                new ChoiceOption("문 열고 받기", ChoiceType.OpenDoor, 0.1f),
                new ChoiceOption("문구멍으로 확인하기", ChoiceType.PeekThrough, 0.2f),
                new ChoiceOption("배달원과 대화하기", ChoiceType.TalkToDelivery, 0.1f)
            });
        }
        
        return situation;
    }
    
    /// <summary>
    /// 현재 선택이 활성화되어 있는지 확인
    /// </summary>
    public bool IsChoiceActive => isChoiceActive;
    
    void OnDestroy()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        ClearChoiceButtons();
    }
}