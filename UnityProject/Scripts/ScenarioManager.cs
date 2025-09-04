using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 배달 시나리오 데이터를 관리하고 제공하는 매니저
/// 정상/비정상 시나리오들을 저장하고 게임 진행에 따라 제공합니다.
/// </summary>
public class ScenarioManager : MonoBehaviour
{
    [System.Serializable]
    public class DeliveryScenario
    {
        [Header("Scenario Info")]
        public int id;
        public string title;
        [TextArea(3, 5)]
        public string description;
        public bool isAbnormal;
        
        [Header("Visual")]
        public Sprite deliveryImage;
        
        [Header("Game Data")]
        public int day; // 몇 일차 시나리오인지
        public float suspicionLevel; // 의심도 (0-1)
    }
    
    [Header("Scenario Database")]
    [SerializeField] private List<DeliveryScenario> normalScenarios = new List<DeliveryScenario>();
    [SerializeField] private List<DeliveryScenario> abnormalScenarios = new List<DeliveryScenario>();
    
    [Header("Game Progress")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int scenariosPerDay = 5;
    [SerializeField] private float abnormalProbability = 0.3f; // 비정상 시나리오 확률
    
    // 현재 세션 데이터
    private List<DeliveryScenario> currentDayScenarios = new List<DeliveryScenario>();
    private int currentScenarioIndex = 0;
    
    // 싱글톤 패턴
    public static ScenarioManager Instance { get; private set; }
    
    // 이벤트
    public System.Action<DeliveryScenario> OnScenarioChanged;
    public System.Action<int> OnDayChanged;
    public System.Action OnAllScenariosCompleted;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultScenarios();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartNewDay();
    }
    
    /// <summary>
    /// 기본 시나리오 데이터를 초기화합니다
    /// </summary>
    private void InitializeDefaultScenarios()
    {
        // 정상 시나리오들
        normalScenarios.AddRange(new DeliveryScenario[]
        {
            new DeliveryScenario 
            { 
                id = 1, 
                title = "일반 치킨 배달", 
                description = "\"안녕하세요! 치킨 배달왔습니다. 2만원입니다.\"\\n배달원이 환하게 웃으며 따뜻한 치킨을 건네줍니다.", 
                isAbnormal = false, 
                day = 1, 
                suspicionLevel = 0f 
            },
            new DeliveryScenario 
            { 
                id = 2, 
                title = "피자 배달", 
                description = "\"피자 주문하신 분 맞죠? 페퍼로니 라지 사이즈요.\"\\n배달원이 따뜻한 피자 박스를 들고 있습니다.", 
                isAbnormal = false, 
                day = 1, 
                suspicionLevel = 0f 
            },
            new DeliveryScenario 
            { 
                id = 3, 
                title = "중국집 배달", 
                description = "\"짜장면 두 개 배달왔습니다!\"\\n익숙한 중국집 배달원이 주문을 확인하고 있습니다.", 
                isAbnormal = false, 
                day = 1, 
                suspicionLevel = 0f 
            }
        });
        
        // 비정상 시나리오들
        abnormalScenarios.AddRange(new DeliveryScenario[]
        {
            new DeliveryScenario 
            { 
                id = 101, 
                title = "이상한 시간 배달", 
                description = "\"배달왔습니다...\"\\n새벽 3시에 주문하지 않은 배달이 도착했습니다. 배달원의 얼굴이 잘 보이지 않습니다.", 
                isAbnormal = true, 
                day = 2, 
                suspicionLevel = 0.7f 
            },
            new DeliveryScenario 
            { 
                id = 102, 
                title = "정체불명 음식", 
                description = "\"특별 메뉴 배달이요...\"\\n주문한 적 없는 검은 봉지를 든 배달원이 서 있습니다. 봉지에서 이상한 냄새가 납니다.", 
                isAbnormal = true, 
                day = 3, 
                suspicionLevel = 0.9f 
            },
            new DeliveryScenario 
            { 
                id = 103, 
                title = "침묵하는 배달원", 
                description = "\"...\"\\n배달원이 아무 말 없이 당신을 뚫어지게 쳐다보고 있습니다. 손에는 빈 봉지만 들고 있습니다.", 
                isAbnormal = true, 
                day = 4, 
                suspicionLevel = 0.8f 
            }
        });
    }
    
    /// <summary>
    /// 새로운 하루를 시작합니다
    /// </summary>
    public void StartNewDay()
    {
        currentDayScenarios.Clear();
        currentScenarioIndex = 0;
        
        // 하루에 보여줄 시나리오들을 생성
        GenerateDayScenarios();
        
        OnDayChanged?.Invoke(currentDay);
        
        // 첫 번째 시나리오 시작
        if (currentDayScenarios.Count > 0)
        {
            OnScenarioChanged?.Invoke(currentDayScenarios[0]);
        }
    }
    
    /// <summary>
    /// 하루치 시나리오를 생성합니다
    /// </summary>
    private void GenerateDayScenarios()
    {
        for (int i = 0; i < scenariosPerDay; i++)
        {
            bool shouldBeAbnormal = Random.value < abnormalProbability;
            
            if (shouldBeAbnormal && abnormalScenarios.Count > 0)
            {
                // 현재 날짜에 적합한 비정상 시나리오 선택
                var validAbnormalScenarios = abnormalScenarios.FindAll(s => s.day <= currentDay);
                if (validAbnormalScenarios.Count > 0)
                {
                    var selectedScenario = validAbnormalScenarios[Random.Range(0, validAbnormalScenarios.Count)];
                    currentDayScenarios.Add(selectedScenario);
                }
                else
                {
                    // 적합한 비정상 시나리오가 없으면 정상 시나리오 사용
                    AddRandomNormalScenario();
                }
            }
            else
            {
                AddRandomNormalScenario();
            }
        }
    }
    
    /// <summary>
    /// 랜덤한 정상 시나리오를 추가합니다
    /// </summary>
    private void AddRandomNormalScenario()
    {
        if (normalScenarios.Count > 0)
        {
            var selectedScenario = normalScenarios[Random.Range(0, normalScenarios.Count)];
            currentDayScenarios.Add(selectedScenario);
        }
    }
    
    /// <summary>
    /// 현재 시나리오를 반환합니다
    /// </summary>
    public DeliveryScenario GetCurrentScenario()
    {
        if (currentScenarioIndex < currentDayScenarios.Count)
        {
            return currentDayScenarios[currentScenarioIndex];
        }
        return null;
    }
    
    /// <summary>
    /// 다음 시나리오로 진행합니다
    /// </summary>
    public void NextScenario()
    {
        currentScenarioIndex++;
        
        if (currentScenarioIndex < currentDayScenarios.Count)
        {
            // 다음 시나리오 시작
            OnScenarioChanged?.Invoke(currentDayScenarios[currentScenarioIndex]);
        }
        else
        {
            // 하루 종료
            CompleteDay();
        }
    }
    
    /// <summary>
    /// 하루를 완료하고 다음 날로 넘어갑니다
    /// </summary>
    private void CompleteDay()
    {
        currentDay++;
        
        // 게임 종료 조건 체크 (예: 7일차 완료)
        if (currentDay > 7)
        {
            OnAllScenariosCompleted?.Invoke();
        }
        else
        {
            // 다음 날 시작
            StartNewDay();
        }
    }
    
    /// <summary>
    /// 시나리오 선택 결과를 처리합니다
    /// </summary>
    public void ProcessScenarioChoice(bool selectedAbnormal)
    {
        var currentScenario = GetCurrentScenario();
        if (currentScenario == null) return;
        
        bool isCorrect = currentScenario.isAbnormal == selectedAbnormal;
        
        Debug.Log($"시나리오 선택: {(selectedAbnormal ? "비정상" : "정상")} | " +
                 $"정답: {(currentScenario.isAbnormal ? "비정상" : "정상")} | " +
                 $"결과: {(isCorrect ? "정답" : "오답")}");
        
        // 여기서 점수, 생존력, 의심도 등을 업데이트할 수 있습니다
        
        // 짧은 지연 후 다음 시나리오로 진행
        Invoke(nameof(NextScenario), 1f);
    }
    
    /// <summary>
    /// 게임 상태 정보를 반환합니다
    /// </summary>
    public (int currentDay, int totalScenarios, int completedScenarios) GetGameProgress()
    {
        return (currentDay, currentDayScenarios.Count, currentScenarioIndex);
    }
}