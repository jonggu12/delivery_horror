# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Korean horror game development project called "배달호러" (Delivery Horror). It's a psychological horror adventure game with a loop-based structure where players experience increasingly disturbing delivery scenarios.

## Project Structure

The repository currently contains project documentation:
- `prd.md`: Comprehensive game design document with detailed mechanics, audio design, and ending structures
- `제작일정.md`: 8-week development schedule from Unity 2D prototype to Unreal Engine 3D implementation

## Development Plan

The project follows a structured approach:
1. **Weeks 1-2**: Unity 2D card-swipe prototype with basic loop mechanics
2. **Weeks 3-4**: Enhanced Unity prototype with audio and player response systems  
3. **Weeks 5-6**: Unreal Engine 3D implementation with first-person perspective
4. **Weeks 7-8**: Final Unreal implementation with complete horror elements

## Game Architecture

### Core Loop System
- Daily delivery cycles with normal/abnormal event branching
- Progressive horror buildup across multiple days
- Player choice system affecting story outcomes

### Key Mechanics
- CCTV/door viewer inspection system
- Multiple player response options (police call, door chain, recording)
- Four ending types: Good, Bad, Neutral, Secret

### Audio Design Framework
- Layered sound system from familiar daily sounds to distorted horror audio
- Specific audio cues for different threat levels and player feedback

## Development Tools
- **Unity**: For initial 2D prototype and mechanics testing
- **Unreal Engine 5**: For final 3D first-person implementation
- **AI Integration**: Extensive use of AI tools for code generation, asset creation, and debugging

## Development Methodology

### Feature Implementation Process
- Break down all features into small, testable units before implementation
- Each feature must include verification and testing time in estimates
- Complete one feature fully before moving to the next
- Conduct code review and critical analysis after each completed feature

### Code Quality Standards
- After implementing any feature, perform critical code review focusing on:
  - Code structure and maintainability
  - Performance implications
  - Potential bugs or edge cases
  - Adherence to project patterns
- Refactor immediately when issues are identified
- Document design decisions and architectural choices

## Language Notes
All documentation is in Korean. The game targets Korean-speaking audiences with culturally relevant delivery/apartment living scenarios.

## Taskmaster 작업 진행 원칙

### 📋 작업 관리 시스템
모든 개발 작업은 **Taskmaster**를 통해 체계적으로 진행됩니다.

### 🎯 메인 태스크 진행 방식
1. **시작 전 브리핑**: 메인 태스크의 전체 목적과 포함된 서브태스크들을 명확히 설명
2. **서브태스크 순차 진행**: 정의된 순서에 따라 하나씩 완료
3. **단일 집중**: 절대 여러 서브태스크를 동시에 진행하지 않음
4. **완료 후 요약**: 메인 태스크 완료 시 모든 서브태스크 성과 정리

### 🔄 서브태스크 진행 원칙
- **시작 설명**: 각 서브태스크 시작 전 무엇을 할 것인지 명확히 설명
- **완료 확인**: 서브태스크 완료 후 status를 "done"으로 변경
- **초보자 친화적 설명**: 완료한 내용과 다음 진행 내용을 쉽게 설명
- **단계적 피드백**: 각 단계에서 달성한 것과 다음 목표 제시

### ⚠️ 금지사항
- 독단적인 서브태스크 건너뛰기 또는 동시 진행
- 완료 상태 업데이트 누락
- 사용자 설명 없이 진행하는 작업

### 📊 상태 관리
- **pending**: 대기 중
- **in_progress**: 진행 중  
- **done**: 완료
- **blocked**: 차단됨 (의존성 미충족)