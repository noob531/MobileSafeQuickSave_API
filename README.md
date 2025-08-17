# MobileSafeQuickSave_API (Android/SMAPI 4.2.1)

모바일용 Quick Save API 통합 보수적 중간저장 모드입니다. 모든 코드는 ChatGPT를 통해 작성되었습니다.

## 주요 특징
- **F5 키, /msqsave 콘솔 명령 지원**
- **/msqsave!** → 강제 저장(권장하지 않음)
- **FTM/SpaceCore 상태 감지** → 바쁠 시 저장 거부
- **Quick Save API 연동 시점만 저장 허용, 안전성 최대화**

## 설치 및 사용법

1. **빌드된 DLL과 manifest.json을 `Mods/MobileSafeQuickSave_API` 폴더에 넣으세요.**
2. 게임 내에서 F5 키 또는 콘솔 명령어 `/msqsave`, `/msqsave!` 사용 가능.
3. 세이브 전 항상 백업 권장.

## 주의사항
- **DLL 빌드 필요**: 소스 코드를 .NET Framework 4.5.2(net452)로 빌드해야 합니다.
- **세이브 백업 필수**: 비정상 저장 또는 충돌 시 세이브 손상 위험이 있으므로, 항상 세이브 파일을 백업하세요.
- **FTM/SpaceCore와의 호환**: 해당 모드들이 비정상적으로 동작 중일 때는 저장이 거부됩니다.
- **Quick Save API 연동**: API가 없거나 동작 불가 시 저장을 자동으로 거부합니다.

## 개발/기여
- 문의/기여는 GitHub Issue 또는 PR로 요청해 주세요.
- SMAPI 모바일(4.2.1) 환경을 기준으로 설계되었습니다.
---
**이 모드는 공식 SMAPI/스타듀밸리 모바일 버전의 동작을 100% 보장하지 않습니다. 모든 저장은 본인 책임 하에 진행해 주세요.**
