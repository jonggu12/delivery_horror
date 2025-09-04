# Card Swipe UI 프리팹 구조

## Canvas 계층 구조

```
MainCanvas
├── Background (Image)
│   └── BackgroundImage.png (어두운 아파트 복도)
│
├── CardContainer (RectTransform)
│   └── DeliveryCard (RectTransform) [CardSwipeController 스크립트 첨부]
│       ├── CardBackground (Image)
│       │   └── CardFrame.png (카드 프레임)
│       ├── DeliveryImage (Image)
│       │   └── delivery_placeholder.png (배달 이미지)
│       ├── ScenarioText (Text/TextMeshPro)
│       │   └── "배달 시나리오 텍스트"
│       └── SwipeHints (RectTransform)
│           ├── LeftHint (Image + Text)
│           │   ├── NormalIcon.png
│           │   └── "정상" 텍스트
│           └── RightHint (Image + Text)
│               ├── AbnormalIcon.png
│               └── "비정상" 텍스트
│
├── UI_HUD (RectTransform)
│   ├── TopPanel (RectTransform)
│   │   ├── DayCounter (Text)
│   │   │   └── "Day X"
│   │   └── ScenarioCounter (Text)
│   │       └── "X/5"
│   └── BottomPanel (RectTransform)
│       ├── Instructions (Text)
│       │   └── "← 정상   비정상 →"
│       └── ProgressBar (Slider)
│           └── 하루 진행도 표시
│
└── DebugPanel (RectTransform) [개발용]
    ├── DebugText (Text)
    └── RestartButton (Button)
```

## 컴포넌트 설정

### DeliveryCard GameObject
- **RectTransform**: 스크린 중앙 배치
- **Image**: 카드 배경 이미지
- **CardSwipeController**: 스와이프 로직
- **GraphicRaycaster**: UI 터치 감지

### 필요한 이미지 에셋
1. **CardFrame.png**: 카드 테두리 프레임
2. **delivery_placeholder.png**: 기본 배달 이미지
3. **NormalIcon.png**: 정상 시나리오 아이콘 (초록색 체크)
4. **AbnormalIcon.png**: 비정상 시나리오 아이콘 (빨간색 X)
5. **BackgroundImage.png**: 아파트 복도 배경

### 애니메이션 요소
- **카드 스와이프**: 드래그 중 회전 및 위치 변화
- **색상 변화**: 스와이프 방향에 따른 카드 색상 변화
- **카드 제거**: 선택 완료 시 카드가 화면에서 사라지는 효과
- **새 카드 등장**: 다음 시나리오 카드가 등장하는 효과

### 입력 설정
- **Touch Input**: 모바일 터치 지원
- **Mouse Input**: PC 개발/테스트용 마우스 지원
- **Drag Threshold**: 100픽셀 이상 드래그 시 스와이프 인식

## 스크립트 연결

### CardSwipeController 연결 필드
- `cardImage`: DeliveryCard의 Image 컴포넌트
- `scenarioText`: ScenarioText의 Text 컴포넌트
- `deliveryImage`: DeliveryImage의 Image 컴포넌트
- `swipeThreshold`: 100f
- `cardReturnSpeed`: 10f
- `maxRotation`: 30f

### ScenarioManager 연결 필드
- CardSwipeController의 이벤트와 연결
- UI 업데이트 담당