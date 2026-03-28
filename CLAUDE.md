# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**DementiaPropParaller** is a Unity 6 empathy simulation project that realistically portrays dementia symptoms on an NPC character ("Father"). Designed for caregivers/family members to build empathy through interactive character behaviour.

- **Unity:** 6000.3.7f1 with URP (com.unity.render-pipelines.universal 17.3.0)
- **Target Platform:** Meta Quest 2 & Quest 3 (Android, ARM64, Vulkan, ASTC)
- **Character Pipeline:** Avaturn (GLB) → Blender (FBX) → Mixamo (animations) → Unity

## Common Unity Commands

All Unity-specific operations (scenes, GameObjects, scripts, assets) should be done through the MCP tools (e.g., `gameobject-find`, `script-update-or-create`, `console-get-logs`) rather than raw file editing, since Unity requires its own import pipeline.

To run tests: use the `tests-run` skill (EditMode for faster iteration).

To compile/check for errors after script changes: use `assets-refresh` skill, then `console-get-logs`.

## Architecture

### Symptom Controller Pattern

All symptom logic lives in `Assets/Scripts/Symptoms/`. Each system is an independent MonoBehaviour component:

| Script | Symptom | Key Parameters |
|---|---|---|
| `CognitiveDelaySystem.cs` | Delayed responses | `cognitiveLoad` (0–2) |
| `EyeContactController.cs` | Avoidant gaze | `gazeOffsetMagnitude`, `gazeShiftInterval`, `confusionLevel`, `directGazeChance` |
| `FlatAffectController.cs` | Emotional blunting via BlendShapes | `suppressionLevel` (0–1) |
| `FidgetLayer.cs` | Hand fidgeting via Animator layer blend | `anxietyLevel` (0–2) |
| `ShufflingGait.cs` | Slow walk via Animator speed multiplier | `animatorSpeedMul` |

### Orchestrator

`Assets/Scripts/SceneManagers/DementiaSymptomOrchestrator.cs` is the single entry point that drives all symptoms. It exposes two top-level sliders:

- **`diseaseProgression`** (0–1): Maps to each symptom's severity parameters
- **`anxietyLevel`** (0–2): Rises automatically from 0 → 1.5 over ~45 seconds via `RiseAnxiety()` coroutine

Disease progression mapping:
- Cognitive load: direct (0–1)
- Gaze offset: Lerp(0.15, 0.55)
- Confusion level: direct (0–1)
- Direct gaze chance: Lerp(0.30, 0.05) — less eye contact as disease worsens
- Flat affect suppression: Lerp(0.4, 0.9)

### Action Routing (Cognitive Delay)

Do **not** call `Animator` directly on the Father character for triggered actions. All actions must go through `CognitiveDelaySystem.RequestAction(Action, bool highPriority)` so they receive the appropriate cognitive delay.

### Testing Symptoms

Each symptom controller and the orchestrator have `[ContextMenu]` test methods. Right-click the component in the Inspector to call:
- `TestEarlyStage()`, `TestModerateStage()`, `TestSevereStage()`, `TestMaxAnxiety()`, `ResetAll()` on the Orchestrator
- Individual test methods on each symptom script

## Key Files

- `Assets/Scripts/SceneManagers/DementiaSymptomOrchestrator.cs` — master state manager
- `Assets/Scripts/Symptoms/` — all five symptom controllers
- `Assets/Editor/ProjectFolderSetup.cs` — folder scaffold utility (Tools > Setup > Create Project Folder Structure)
- `Packages/manifest.json` — package versions
- `Assets/Settings/` — URP render pipeline assets (separate PC and Mobile profiles)

## Scenes

- `Assets/Scenes/MainScene.unity` — primary scene
- `Assets/Scenes/Scene1_LivingRoom.unity` — room-specific scenario

## Render Pipeline

Two URP configurations exist:
- **PC:** `PC_RPAsset` / `PC_Renderer` — high quality
- **Mobile:** `Mobile_RPAsset` / `Mobile_Renderer` — Quest-optimised
