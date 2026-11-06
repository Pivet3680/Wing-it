# ADR-0002: Universal 2D 템플릿으로 프로젝트 초기화

- 날짜: 2026-11-06
- 상태: 확정
- 관련 문서: [GDD](../GDD.md), [로드맵](../ROADMAP.md), [README](../../README.md)

## 배경

기획은 풀 2D로 전환했지만 실제 Unity 프로젝트에는 3D URP 렌더러, 3D 모델·씬·충돌 설정과 이전 프로토타입이 남아 있었다. 사용자는 이 구성을 제거하고 새 2D 프로젝트를 만든 것과 같은 상태로 변경하도록 요청했다.

## 검토한 대안

1. 기존 씬과 설정을 개별적으로 2D로 바꾸기: 이전 자산을 계속 사용할 수 있으나 새 2D 프로젝트의 기본 상태와는 다르다.
2. 같은 Unity 버전의 공식 `Universal 2D` 템플릿으로 활성 Unity 프로젝트 폴더를 교체하기: 요청한 초기 상태와 일치한다.

## 선택한 방안과 이유

Unity `6000.3.21f1`의 `com.unity.template.universal-2d`에서 새 프로젝트를 생성해 `Assets`, `Packages`, `ProjectSettings`, `UserSettings`를 활성 프로젝트에 적용했다. 프로젝트 표시 이름은 `Wing it!`으로 맞췄다. 이전 `Assets`, `Packages`, `ProjectSettings`는 GitHub에서 제외한 로컬 `Art/LegacyUnityProject-2026-11-06.zip`에 보관하고 생성 캐시는 다시 만들었다. `Docs`, `Art`, Git 이력은 유지했다.

## 영향 범위

- 기존 3D 씬·모델·스크립트와 시험용 스프라이트가 활성 `Assets`에서 제거됐다.
- 새 기본 씬은 Orthographic 카메라와 `Global Light 2D`를 사용하며, URP는 `Renderer2D`를 참조한다.
- 펫·블록·보스전 및 플레이어 이동을 포함한 게임플레이는 새 프로젝트에서 다시 구현해야 한다. 필요한 2D 스프라이트는 보관본에서 선별해 가져올 수 있다.

## 검증 상태

- 성공: 공식 템플릿의 2D 렌더러·기본 씬·패키지 확인, Editor 임포트와 씬 로딩 완료, C# 컴파일 오류 없음.
- 성공: 기본 씬 Play Mode 화면 표시, Console 오류·경고 0건.
- 미검증: 새 게임플레이 기능. 현재 활성 프로젝트에 구현되지 않았다.
