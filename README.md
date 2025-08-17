# MobileSafeQuickSave_API

**MobileSafeQuickSave_API**는 Stardew Valley에서 안전하게 중간저장(Quick Save)을 지원하는 모드입니다.  

- F5 키 입력 또는 콘솔 명령으로 중간 저장 실행  
- `spacechase0.SpaceCore` 설치 시 더 보수적인 저장 안전성 검사 적용  
- FTM, RSV 같은 대규모 확장 모드 사용 시에도 세이브 손상 방지  

## 설치 방법
1. [SMAPI](https://smapi.io) 설치  
2. 이 모드의 최신 릴리스 다운로드  
3. 압축을 풀고 `MobileSafeQuickSave_API` 폴더를 `Mods` 폴더에 넣기  

## 사용법
- **F5 키** → 중간 저장 시도  
- **콘솔 명령어**
  - `msqsave` → 안전 조건 검사 후 중간 저장  
  - `msqsave!` → 강제 저장 (비추천)  

## 설정 (config.json)
```json
{
  "ExtraDelayMs": 750,
  "VerboseLogging": true
}


ExtraDelayMs: 저장 전 대기 시간 (ms)

VerboseLogging: true일 경우 상세 로그 출력


주의사항

이벤트/축제/하루 전환 중에는 저장 차단

강제 저장(msqsave!)은 세이브 손상 위험 존재

SpaceCore/FTM이 없어도 동작하며, 설치 시 추가 안전 기능이 활성화됨


개발자 참고

Target framework: .NET Framework 4.5.2

Entry point: ModEntry.cs

UniqueID: noob531.MobileSafeQuickSave_API


라이선스

MIT License