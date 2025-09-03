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