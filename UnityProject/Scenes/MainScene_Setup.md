# MainScene 설정 가이드

## 1. 씬 기본 설정

### Camera 설정
- **Main Camera**: 
  - Position: (0, 0, -10)
  - Projection: Orthographic
  - Size: 5
  - Background Color: #2C2C2C (어두운 회색)

### Lighting
- **Lighting Settings**:
  - Skybox Material: None
  - Environment Lighting: Color
  - Ambient Color: #404040 (중간 회색)

## 2. UI 계층 구조 생성

### Step 1: Canvas 생성
1. Hierarchy에서 우클릭 → UI → Canvas
2. Canvas 이름을 "MainCanvas"로 변경
3. Canvas 설정:
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1080 x 1920 (모바일 세로)
   - Screen Match Mode: Match Width Or Height (0.5)

### Step 2: EventSystem 확인
- Canvas 생성 시 자동으로 EventSystem이 생성됨
- EventSystem이 없으면 우클릭 → UI → Event System

### Step 3: 배경 이미지 추가
```
MainCanvas/
└── Background (Image)
    └── BackgroundImage 설정
        - Source Image: 어두운 아파트 복도 이미지
        - Image Type: Simple
        - Preserve Aspect: false
```

### Step 4: 카드 컨테이너 생성
```
MainCanvas/
└── CardContainer (Empty GameObject + RectTransform)
    - Anchors: Center
    - Position: (0, 0, 0)
    - Size: (800, 1200)
    
    └── DeliveryCard (Image + CardSwipeController)
        - Anchors: Center
        - Size: (700, 1000)
        - Image: 흰색 카드 배경
        
        ├── CardBackground (Image)
        │   - Source Image: 카드 프레임 이미지
        │   - Image Type: Sliced (9-slice sprite)
        │
        ├── DeliveryImage (Image)
        │   - Anchors: Top
        │   - Size: (600, 400)
        │   - Position: (0, -150, 0)
        │
        ├── ScenarioText (Text)
        │   - Font: Arial
        │   - Font Size: 32
        │   - Color: Black
        │   - Alignment: Center
        │   - Anchors: Bottom
        │   - Size: (650, 300)
        │   - Position: (0, 200, 0)
        │
        └── SwipeHints (Empty GameObject)
            ├── LeftHint (Image + Text)
            │   - Anchors: Middle Left
            │   - Position: (-250, 0, 0)
            │   - Image: 초록색 체크 아이콘
            │   - Text: "정상" (초록색)
            │
            └── RightHint (Image + Text)
                - Anchors: Middle Right
                - Position: (250, 0, 0)
                - Image: 빨간색 X 아이콘
                - Text: "비정상" (빨간색)
```

### Step 5: HUD 생성
```
MainCanvas/
└── UI_HUD (Empty GameObject + RectTransform)
    ├── TopPanel (Empty GameObject)
    │   - Anchors: Top Stretch
    │   - Size Delta: (0, 100)
    │   - Position: (0, -50, 0)
    │   
    │   ├── DayCounter (Text)
    │   │   - Anchors: Top Left
    │   │   - Position: (100, -50, 0)
    │   │   - Text: "Day 1"
    │   │   - Font Size: 36
    │   │   - Color: White
    │   │
    │   └── ScenarioCounter (Text)
    │       - Anchors: Top Right
    │       - Position: (-100, -50, 0)
    │       - Text: "1/5"
    │       - Font Size: 36
    │       - Color: White
    │
    └── BottomPanel (Empty GameObject)
        - Anchors: Bottom Stretch
        - Size Delta: (0, 150)
        - Position: (0, 75, 0)
        
        ├── Instructions (Text)
        │   - Anchors: Bottom Center
        │   - Position: (0, 100, 0)
        │   - Text: "← 정상          비정상 →"
        │   - Font Size: 28
        │   - Color: White
        │
        └── ProgressBar (Slider)
            - Anchors: Bottom Stretch
            - Position: (0, 50, 0)
            - Size Delta: (-100, 30)
            - Min Value: 0, Max Value: 1
            - Fill Color: #4CAF50 (초록색)
```

### Step 6: 디버그 패널 (개발용)
```
MainCanvas/
└── DebugPanel (Image)
    - Anchors: Top Left
    - Position: (200, -200, 0)
    - Size: (350, 300)
    - Color: #000000AA (반투명 검정)
    
    ├── DebugText (Text)
    │   - Anchors: Stretch
    │   - Margins: (10, 10, 10, 50)
    │   - Font Size: 18
    │   - Color: White
    │   - Alignment: Top Left
    │
    └── RestartButton (Button)
        - Anchors: Bottom Center
        - Position: (0, 25, 0)
        - Size: (150, 40)
        - Text: "Restart"
```

## 3. 스크립트 연결

### GameController 스크립트 연결
1. Hierarchy에서 빈 GameObject 생성 → "GameManager"로 이름 변경
2. GameController 스크립트 첨부
3. Inspector에서 UI 레퍼런스 연결:
   - Card Swipe Controller: DeliveryCard 오브젝트
   - Day Counter Text: DayCounter 텍스트
   - Scenario Counter Text: ScenarioCounter 텍스트
   - Instructions Text: Instructions 텍스트
   - Progress Bar: ProgressBar 슬라이더
   - Debug Panel: DebugPanel 오브젝트
   - Debug Text: DebugText 텍스트
   - Restart Button: RestartButton 버튼

### ScenarioManager 스크립트 연결
1. Hierarchy에서 빈 GameObject 생성 → "ScenarioManager"로 이름 변경
2. ScenarioManager 스크립트 첨부
3. Inspector에서 시나리오 데이터 설정 (기본값 자동 설정됨)

### CardSwipeController 스크립트 연결
1. DeliveryCard 오브젝트에 CardSwipeController 스크립트 첨부
2. Inspector에서 UI 레퍼런스 연결:
   - Card Image: DeliveryCard의 Image 컴포넌트
   - Scenario Text: ScenarioText 텍스트
   - Delivery Image: DeliveryImage 이미지
   - Swipe Threshold: 100
   - Card Return Speed: 10
   - Max Rotation: 30

## 4. 테스트 실행

### 빌드 설정
1. File → Build Settings
2. Platform: Android 또는 iOS (모바일 테스트용)
3. Player Settings → Resolution and Presentation:
   - Default Orientation: Portrait
   - Use 32-bit Display Buffer: true

### 테스트 순서
1. Play 버튼으로 게임 실행
2. 카드를 좌우로 드래그하여 스와이프 테스트
3. Debug 패널에서 게임 상태 확인
4. Restart 버튼으로 재시작 테스트

## 5. 추가 최적화

### Performance
- UI 요소들을 Canvas Group으로 묶어서 일괄 alpha 조정
- 텍스트는 TextMeshPro 사용 권장 (더 나은 품질)
- 이미지는 적절한 압축 설정으로 최적화

### Responsive Design
- SafeArea 스크립트 추가로 노치 영역 대응
- 다양한 해상도 테스트 (16:9, 18:9, 19.5:9)