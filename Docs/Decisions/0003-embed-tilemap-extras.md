# ADR-0003 — Tilemap Extras 6.0.3을 프로젝트에 포함

- 날짜: 2026-11-20
- 상태: 확정
- 관련 시스템: Unity 6.3 LTS, 2D Tilemap Extras, Auto Tile Inspector

## 배경

스크립트 재로딩 중 `AutoTileEditor`에서 `VisualElementCreation is not allowed to be called during serialization` 예외가 발생했다. Unity의 핫 리로드는 직렬화 가능한 비공개 필드도 복원한다. `m_EditorElement`의 타입에 `[Serializable]`이 붙어 있어 복원 중 UI 생성이 호출됐다.

## 검토한 대안

- 원래 패키지의 캐시를 수정하면 변경이 작지만 패키지를 다시 내려받을 때 수정이 사라질 수 있다.
- 기존 버전을 프로젝트에 포함하면 모든 작업 환경에서 동일한 수정이 적용된다. 공식 버전으로 돌아갈 때는 프로젝트에 포함한 사본을 정리해야 한다.

## 선택한 방안과 이유

`Packages/com.unity.2d.tilemap.extras`에 기존 6.0.3을 포함하고 `m_EditorElement`에 `[System.NonSerialized]`를 추가했다. 기존 `CreateInspectorGUI()`가 필요할 때 UI를 다시 만들도록 했다. 이 수정은 타일 마스크와 런타임 동작에 영향을 주지 않는다.

## 영향 범위

- 프로젝트에 패키지 소스와 메타 파일이 포함된다. `packages-lock.json`은 이 패키지를 `embedded`로 기록한다.
- 공식 패키지 업데이트를 적용할 때 이 예외의 수정 여부를 확인하고, 수정이 포함되어 있다면 레지스트리 버전으로 돌아간다.

## 검증 상태

- 성공 — Unity 6000.3.28f1에서 패키지를 컴파일했다. `AutoTileDemo`에서 `New Auto Tile`이 선택된 상태로 Play Mode에 들어갔다가 나왔다. 도메인 재로딩 뒤 Inspector에 마스크가 표시됐고 동일 예외가 다시 발생하지 않았다.
