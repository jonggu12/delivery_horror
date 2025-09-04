# Advanced UI Setup Guide - Subtask 2.2 Complete

## 새로 추가된 UI 컴포넌트들

### 1. DeliveryNotificationUI (배달 앱 알림)
```
NotificationPanel (RectTransform)
├── NotificationBackground (Image) [DeliveryNotificationUI 스크립트]
│   └── BackgroundColor: 동적 변경 (정상/비정상에 따라)
├── ContentContainer (Horizontal Layout Group)
│   ├── AppIcon (Image)
│   │   └── Size: 60x60
│   ├── TextContainer (Vertical Layout Group)
│   │   ├── AppName (Text)
│   │   │   └── "배달의공포" / "???" (비정상시)
│   │   ├── NotificationTitle (Text)
│   │   │   └── Font Size: 18, Bold
│   │   └── NotificationMessage (Text)
│   │       └── Font Size: 14
│   ├── TimeStamp (Text)
│   │   └── "HH:mm" 형식
│   └── DismissButton (Button)
│       └── "X" 아이콘
```

### 2. GameStatusUI (게임 상태 패널)
```
StatusPanel (RectTransform)
├── SurvivalPanel (Horizontal Layout Group)
│   ├── SurvivalIcon (Image)
│   ├── SurvivalBar (Slider)
│   │   ├── Fill Color: 생존력에 따라 초록→빨강
│   │   └── Range: 0-100
│   └── SurvivalText (Text)
├── SuspicionPanel (Horizontal Layout Group)
│   ├── SuspicionIcon (Image)
│   ├── SuspicionBar (Slider)
│   │   ├── Fill Color: 의심도에 따라 파랑→빨강
│   │   └── Range: 0-100
│   └── SuspicionText (Text)
├── ProgressPanel (Horizontal Layout Group)
│   ├── DayText (Text) - "Day X"
│   ├── ScenarioCountText (Text) - "X/5"
│   ├── ScoreText (Text) - "점수: XXX"
│   └── DayProgressBar (Slider)
└── ChoiceFeedbackPanel (Optional Display)
    ├── ChoiceResultIcon (Image)
    └── ChoiceResultText (Text)
```

### 3. PlayerChoicePanel (플레이어 선택 패널)
```
ChoicePanel (RectTransform)
├── SituationText (Text)
│   └── 현재 상황 설명
├── ChoiceButtonContainer (Vertical Layout Group)
│   └── [동적 생성되는 선택지 버튼들]
├── QuickActionPanel (Horizontal Layout Group)
│   ├── CallPoliceButton (Button) - "112"
│   ├── LockDoorButton (Button) - "🔒"
│   ├── StartRecordingButton (Button) - "📹"
│   └── PeekThroughDoorButton (Button) - "👁️"
└── TimerPanel (RectTransform)
    ├── TimerSlider (Slider)
    └── TimerText (Text) - "XX초"
```

### 4. UIManager 통합 패널들
```
UIManagerCanvas
├── MainMenuPanel
│   ├── StartGameButton
│   ├── SettingsButton
│   └── ExitButton
├── GameUIPanel
│   ├── [위의 모든 게임 UI 포함]
│   └── PauseButton
├── PauseMenuPanel
│   ├── ResumeButton
│   └── MainMenuButton
├── GameOverPanel
│   ├── GameOverText
│   ├── FinalScoreText
│   ├── RestartButton
│   └── QuitToMenuButton
├── SettingsPanel
│   ├── VolumeSlider
│   ├── NotificationsToggle
│   └── SettingsCloseButton
└── LoadingPanel
    ├── LoadingProgressBar
    └── LoadingText
```

## Unity 씬 설정 단계

### Step 1: 기본 Canvas 설정
```csharp
// Canvas 설정
Canvas Mode: Screen Space - Overlay
UI Scale Mode: Scale With Screen Size
Reference Resolution: 1080 x 1920
Screen Match Mode: Match Width Or Height (0.5)
```

### Step 2: DeliveryNotificationUI 설정
1. 빈 GameObject 생성 → "NotificationManager"
2. DeliveryNotificationUI 스크립트 첨부
3. UI 요소들 생성 및 연결:
```csharp
// Inspector 연결 필드
- Notification Panel: NotificationPanel RectTransform
- Notification Background: Background Image
- App Icon: AppIcon Image
- App Name: AppName Text
- Notification Title: Title Text
- Notification Message: Message Text
- Time Stamp: TimeStamp Text
- Dismiss Button: DismissButton Button

// 애니메이션 설정
- Slide In Duration: 0.5f
- Display Duration: 3.0f
- Slide Out Duration: 0.3f
```

### Step 3: GameStatusUI 설정
1. StatusUI GameObject 생성
2. GameStatusUI 스크립트 첨부
3. UI 요소들 연결:
```csharp
// 생존력 패널
- Survival Bar: Slider (0-100 범위)
- Survival Text: Text
- Survival Icon: Image

// 의심도 패널  
- Suspicion Bar: Slider (0-100 범위)
- Suspicion Text: Text
- Suspicion Icon: Image

// 진행도 패널
- Day Text: Text
- Scenario Count Text: Text
- Score Text: Text
- Day Progress Bar: Slider (0-1 범위)
```

### Step 4: PlayerChoicePanel 설정
1. ChoicePanel GameObject 생성
2. PlayerChoicePanel 스크립트 첨부
3. UI 구조 생성:
```csharp
// 메인 패널 설정
- Choice Panel: 전체 패널 RectTransform
- Situation Text: 상황 설명 Text
- Choice Button Container: 버튼들이 들어갈 Container
- Choice Button Prefab: 선택지 버튼 Prefab

// 퀵 액션 버튼들
- Call Police Button: 경찰신고 Button
- Lock Door Button: 문잠금 Button  
- Start Recording Button: 녹화시작 Button
- Peek Through Door Button: 문구멍확인 Button

// 타이머 UI
- Timer Panel: 타이머 패널
- Timer Slider: 시간 진행 Slider
- Timer Text: 남은 시간 Text
```

### Step 5: UIManager 설정
1. UIManager GameObject 생성 (DontDestroyOnLoad)
2. UIManager 스크립트 첨부
3. 모든 UI 컴포넌트 참조 연결:
```csharp
// UI 컴포넌트 참조
- Notification UI: DeliveryNotificationUI
- Status UI: GameStatusUI  
- Choice Panel: PlayerChoicePanel
- Card Swipe Controller: CardSwipeController

// 메뉴 패널들
- Main Menu Panel: GameObject
- Game UI Panel: GameObject
- Pause Menu Panel: GameObject
- Game Over Panel: GameObject
- Settings Panel: GameObject
- Loading Panel: GameObject
```

## 테스트 시나리오

### 1. 알림 시스템 테스트
```csharp
// 정상 배달 알림
UIManager.Instance.ShowDeliveryNotification("배달 도착", "치킨이 도착했습니다!");

// 비정상 배달 알림  
UIManager.Instance.ShowDeliveryNotification("???", "정체불명의 배달...", true);
```

### 2. 상태 UI 테스트
```csharp
// 게임 상태 업데이트
var status = new GameStatusUI.GameStatus();
status.survivalLevel = 75f;
status.suspicionLevel = 40f;
status.currentDay = 2;
status.currentScenario = 3;
status.totalScore = 250;

UIManager.Instance.UpdateGameStatus(status);
```

### 3. 선택지 패널 테스트
```csharp
// 상황별 선택지 표시
var situation = choicePanel.CreateDeliveryArrivalSituation(true); // 비정상 상황
UIManager.Instance.ShowPlayerChoices(situation);
```

## 스크립트 연결 체크리스트

### DeliveryNotificationUI
- [x] 패널 슬라이드 애니메이션
- [x] 알림 타입별 색상 변경
- [x] 자동 숨김/수동 숨김 지원
- [x] 시간 스탬프 자동 업데이트

### GameStatusUI  
- [x] 생존력/의심도 바 애니메이션
- [x] 상태별 색상 변화
- [x] 선택 피드백 표시
- [x] 위험 상황 이벤트 발생

### PlayerChoicePanel
- [x] 동적 선택지 버튼 생성
- [x] 선택 타이머 기능
- [x] 퀵 액션 버튼 지원
- [x] 위험도별 버튼 색상

### UIManager
- [x] 상태 기반 패널 전환
- [x] 로딩 화면 관리
- [x] 설정 저장/로드
- [x] 게임 플로우 제어

## 성능 최적화 팁

1. **Object Pooling**: 자주 생성/제거되는 선택지 버튼들은 풀링 사용
2. **Canvas 분리**: 자주 업데이트되는 UI는 별도 Canvas로 분리
3. **Text 최적화**: TextMeshPro 사용 권장
4. **Image 압축**: UI 이미지는 적절한 압축률 적용
5. **Animation 최적화**: DOTween 같은 최적화된 트위닝 라이브러리 고려