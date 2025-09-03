 # 배달호러 (Delivery Horror)

 한국형 심리 공포 어드벤처 게임 프로젝트입니다. 일상적인 배달 시나리오가 매일 반복되며, 점차 불길하고 기괴한 사건으로 변질되는 루프 구조를 가집니다.

 ## 저장소 구성
 - `prd.md`: 게임 기획서(메커닉, 오디오, 엔딩 구조)
 - `제작일정.md`: 8주 제작 일정(유니티 2D → 언리얼 3D)
 - `AGENTS.md`: 에이전트 운영 가이드(Codex/Claude/Taskmaster)
 - `CLAUDE.md`: Claude Code 가이드
 - `UnityProject/`: 유니티 프로토타입 루트
   - `Assets/`, `Scripts/`, `Scenes/`
 - `UnrealProject/`: 언리얼 구현 루트
   - `Assets/`, `Scripts/`, `Scenes/`

 ## 개발 단계
 1) 주차 1-2: 유니티 2D 카드 스와이프 프로토타입(핵심 루프)
 2) 주차 3-4: 오디오/플레이어 응답 시스템 확장
 3) 주차 5-6: 언리얼 3D 1인칭 구현
 4) 주차 7-8: 공포 요소 완성 및 폴리싱

 ## 로컬 개발 환경
 - Unity 2021 LTS 이상 권장
 - Unreal Engine 5.x 권장
 - Git, Git LFS(대용량 에셋 사용 시) 권장

 ## 유니티 프로젝트 설정 가이드
 1. Unity Hub에서 `UnityProject` 폴더를 열어 새 프로젝트로 인식시킵니다.
 2. 필요한 경우 2D 템플릿을 선택하고, 기본 씬을 `Scenes/` 아래에 생성합니다.
 3. 스크립트는 `Scripts/`, 에셋은 `Assets/`에 구성합니다.
 4. 카드 스와이프, 일일 루프, 선택 시스템을 최소 단위로 나누어 구현합니다.

 ## 언리얼 프로젝트 설정 가이드
 1. Unreal Editor에서 `UnrealProject` 폴더로 새 프로젝트를 생성(블루프린트 또는 C++ 선택)합니다.
 2. 콘텐츠 루트는 `Assets/`에, 스크립트 또는 블루프린트는 `Scripts/`에 정리합니다.
 3. 1인칭 템플릿을 기반으로 CCTV/문열림/침대로 후퇴 등 상호작용을 점진적으로 구현합니다.

 ## 작업 원칙
 - 기능은 작은 단위로 설계/구현/검증하며 순차 완료
 - 각 기능에 테스트/검증 시간을 반드시 포함
 - 변경 후 즉시 크리티컬 리뷰 및 리팩터 수행

 ## 참고
 - 상세 가이드는 `AGENTS.md`, `CLAUDE.md`를 확인하세요.
- 게임 디자인 세부사항은 `prd.md` 참조
- 개발 일정 및 마일스톤은 `제작일정.md` 참조

## Git 작업 가이드

### 📥 처음 가져오기 (Clone)
```bash
git clone https://github.com/jonggu12/delivery_horror.git
cd delivery_horror
```

### 🔄 작업 후 업로드
```bash
git add .
git commit -m "작업 내용 설명"
git push origin main
```

### 📤 다른 기기 변경사항 가져오기
```bash
git pull origin main
```

### 💡 협업 워크플로우
1. **시작**: `git pull` (최신 버전 받기)
2. **작업**: 파일 수정/추가
3. **저장**: `git add` → `git commit` → `git push`
4. **반복**: 다시 1번부터
