# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Hex-grid tactical strategy game (Battletech-inspired) built in Godot 4.7 with C# (.NET 8). Godot project name is "Battle Game", C# root namespace is `BattleGame`.

## Commands

- Build: `dotnet build` (also available as the VSCode task `build` in `tasks.json`)
- Run/Play: launched through the Godot editor, or via the VSCode "Play" debug config in `launch.json`, which runs `${env:GODOT4}` (an env var pointing at the Godot 4 executable) against this project after a build
- No test suite exists in this repo yet — don't assume a test runner or invent test commands

## Architecture

Scenes live under `scenes/`, split into `scenes/world` (tile/board pieces), `scenes/interface` (UI markers/overlays), and `scenes/scripts` (map-layer logic). Gameplay-object scripts (`Unit.cs`, `Selectable.cs`) sit at `scenes/` root alongside their `.tscn` files.

**TileMapLayer stacked overlay pattern**: the board is a base `TileMapLayer` with child `TileMapLayer` nodes stacked on top for different game states, rather than storing per-tile state in one layer.
- `MainTileMapLayer.cs` (on `HexMapLayer` in `main.tscn`) owns this — it's the entry point for hover/selection/movement logic and holds references to the overlay layers as `[Export]` `NodePath`s.
- `HoverLayer` is currently the only implemented overlay: `_Process` converts the mouse position to a map cell via `LocalToMap(ToLocal(...))`, and mirrors the hovered tile's `sourceId`/`atlasCoords` from the base layer onto the hover layer to draw a highlight.
- `SelectionLayer`, `MovementLayer`, and `FogLayer` are the intended next overlays in this same pattern, not yet built.
- Convention: overlay layers hardcode `sourceId: 0, atlasCoords: (0,0)` when painting a highlight cell rather than looking up the source tile's real atlas coords (see the commented-out `HighlightCell`/`RestoreCell` methods in `MainTileMapLayer.cs`, kept as a starting point for a future "battle scarring" terrain-damage feature).
- All overlay layers share the same `TileSet` resource (`assets/tileSets/terrain.tres`) as the base layer.

**Selection system**: `Selectable.cs` is a reusable `Area2D` component (not tied to units specifically) that tracks hover state via `MouseEntered`/`MouseExited` and emits `Selected`/`Deselected` signals on `_UnhandledInput` left-click. The two custom input actions `select_add` (Shift+click) and `select_sub` (Ctrl+click) are defined in `project.godot` and modify click behavior for multi-select. `Unit.cs` composes a `Selectable` child and a `SelectionUI` `ColorRect`, subscribing to the signals to toggle the selection outline — this composition (script + `Selectable` child + visual UI node) is the pattern to follow for other selectable board objects.

**Rendering**: `rendering_device/driver.windows="d3d12"` is set explicitly in `project.godot`. Jolt is configured as the 3D physics engine even though gameplay is 2D (`Area2D`/`TileMapLayer`) — this is a Godot default, not a signal that 3D physics is in use.

Godot `.uid` sidecar files next to `.cs` and `.tscn` resources are engine-managed (Godot 4.3+ resource identity) — don't hand-edit them.
