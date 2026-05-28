<!-- AIBRIDGE:GENERATED COMMAND REFERENCE - DO NOT EDIT MANUALLY -->
# AIBridge Command Reference

此文件由 AIBridge 自动生成。需要修改命令说明时，请修改对应 ICommand 的 SkillDoc/SkillDescription。
`$CLI` 表示当前平台的 AIBridge CLI 调用方式，Windows 项目通常是 `./.aibridge/cli/AIBridgeCLI.exe`。

### `asset` - AssetDatabase Operations

```bash
# Search (recommended - Unity AssetDatabase index)
$CLI asset search --mode script --keyword "Player" --format paths
$CLI asset search --mode prefab --keyword "UI" --format paths
$CLI asset search --filter "t:ScriptableObject" --format paths
# Modes: all, prefab, scene, script, texture, material, audio, animation, shader, font, model, so

# Find (precise control)
$CLI asset find --filter "t:Prefab" --format paths [--searchInFolders "Assets/Textures"] [--maxResults 50]

# Import / Refresh
$CLI asset import --assetPath "Assets/Textures/icon.png"
$CLI asset refresh

# Get Path / Load Metadata
$CLI asset get_path --guid "abc123..."
$CLI asset load --assetPath "Assets/Prefabs/Player.prefab"

# Fallback: read text (use host AI native file-read tool first)
$CLI asset read_text --assetPath "Assets/Scripts/Player.cs" --startLine 1 --maxLines 120
```

**Note:** `format=paths` returns Unity asset paths only (efficient). `format=full` returns asset objects with metadata.

### `code execute` - Controlled Temporary C# Execution

Experimental and enabled by default in project settings. Disable **AIBridge/Settings -> Basic -> Enable Code Execution** for untrusted projects or callers.

```bash
$CLI code execute --file ".aibridge/code/check.csx" --timeout 5000
$CLI code execute --code "Debug.Log(\"hello\"); return 123;"
$CLI code runtime_execute --file ".aibridge/code/player_probe.csx" --transport http --url http://127.0.0.1:27182 --timeout 10000
$CLI code status
$CLI code cancel
```

**Rules:**
- Unity-side project setting cannot be bypassed by CLI parameters.
- `--file` must point to `.aibridge/code/*.cs` or `.aibridge/code/*.csx`.
- `--code` is intended for short snippets only.
- Prefer file mode for complex one-off Editor C# tasks: generated assets, structured analysis, diagnostics, Runtime/Public API calls, or multi-step UnityEditor API orchestration.
- `code runtime_execute` compiles a runtime-safe DLL in Editor, sends it to AIBridgeRuntime, and invokes it in Player through `Assembly.Load` + reflection. It is enabled only when `com.code-philosophy.hybridclr` is installed.
- For generation scripts, keep output under a clear folder such as `Assets/AIBridgeGenerated/<TaskName>/` and return structured result data.
- For existing Prefab structure changes prefer `prefab patch --dryRun true`; for single properties prefer `inspector`; for simple scene object edits prefer `gameobject`/`transform`.
- Snippets are wrapped as `object Execute()` or `Task<object> ExecuteAsync()` when `await` is present.
- Result data includes `enabled`, `status`, `source`, `elapsedMs`, `returnValue`, `logs`, `compileErrors`, `diagnostics`, and `exception` when applicable.
- `code execute` is single-flight. Use `code status` after a timeout and `code cancel` only to release AIBridge waiting state; user code may still finish on Unity's side.
- Use this only for trusted projects/callers; it is not a replacement for `compile unity` or `test run`.

### `compile` - Compilation Operations

```bash
$CLI compile unity  # Default (requires Unity Editor)
$CLI compile dotnet [--solution MyGame.sln]  # Optional validation

# Output: {"success":true,"status":"success","duration":5.2,"errorCount":0,"warningCount":3,...}
```

**Unity compile response fields:**

| Field | Description |
|-------|-------------|
| `success` | Whether build succeeded |
| `status` | "success", "failed", "idle", or "timeout" |
| `duration` | Build duration in seconds |
| `errorCount` | Number of errors |
| `warningCount` | Number of warnings |
| `errors` | Array of error details (file, line, column, code, message) |
| `warnings` | Array of warning details (limited to 20) |

**Unity compile parameters:**

| Parameter | Description | Default |
|-----------|-------------|---------|
| `--timeout` | Total compilation timeout in ms | `120000` |
| `--poll-interval` | Status polling interval in ms | `500` |
| `--transport-timeout` | Single command round-trip timeout in ms | `min(30000, timeout)` |

**Dotnet compile parameters (explicit solution build check):**

| Parameter | Description | Default |
|-----------|-------------|---------|
| `--solution` | Solution file path. If omitted, auto-detect from project root; if ambiguous, pass explicitly | auto-detect |
| `--configuration` | Build configuration | `Debug` |
| `--verbosity` | MSBuild verbosity | `minimal` |
| `--timeout` | Timeout in ms | `300000` |
| `--no-filter` | Disable error filtering | `false` |
| `--exclude` | Custom exclude paths (comma separated) | - |

**NOTE**:

- `compile unity` requires Unity Editor to be running, automatically polls for completion
- If Unity is already compiling or temporarily busy, `compile unity` will attach to the current compilation and keep polling until a final result or the outer timeout is reached
- `compile unity` does not fall back to `dotnet build`; Unity compile and solution build are intentionally separate validations
- `--timeout` controls the full compile wait window, while `--transport-timeout` controls each CLI-Unity communication attempt
- `compile dotnet` runs independently without Unity, auto-detects a single root-level `.sln` or `.slnx` when `--solution` is omitted, and has intelligent error filtering
- Use `compile start` and `compile status` for low-level manual compilation control

### `editor` - Editor Control

```bash
$CLI editor log --message "Hello" [--logType Warning|Error]
$CLI editor undo [--count 3]
$CLI editor redo
$CLI editor compile  # Simple compile, use `compile` command for full features
$CLI editor refresh [--forceUpdate true]
$CLI editor play [--domainReload false]
$CLI editor stop|pause
$CLI editor get_state
```

### `gameobject` - GameObject Operations

```bash
$CLI gameobject create --name "Cube" --primitiveType Cube [--parentPath "Parent"]
$CLI gameobject destroy --path "Cube" [--instanceId 12345]
$CLI gameobject find --name "Player" [--tag "Enemy"] [--withComponent "BoxCollider"] [--maxResults 10]
$CLI gameobject set_active --path "Player" --active false [--toggle true]
$CLI gameobject rename --path "OldName" --newName "NewName"
$CLI gameobject duplicate --path "Original"
$CLI gameobject get_info --path "Player"
```

### `gameview` - Game View Resolution

```bash
$CLI gameview get_resolution
$CLI gameview set_resolution --width 1920 --height 1080
$CLI gameview list_resolutions
```

**Parameters for `set_resolution`:**

| Parameter | Range | Required | Description |
|-----------|-------|----------|-------------|
| `--width` | 1-8192 | Yes | Resolution width in pixels |
| `--height` | 1-8192 | Yes | Resolution height in pixels |

### `get_logs` - Get Console Logs

```bash
$CLI get_logs [--count 100] [--logType Error|Warning] [--regex "pattern"]
```

### `input` - Runtime Input Simulation (Play Mode)

**Requires Play mode and an active EventSystem.** Uses Unity screen coordinates (bottom-left origin).

```bash
$CLI input click --path "Canvas/StartButton"
$CLI input click_at --x 960 --y 540
$CLI input drag --path "Canvas/Item" --toPath "Canvas/Slot" --frames 12
$CLI input long_press --instanceId 12345 --duration-ms 800
```

**Actions:**
- `click`: click a GameObject by `--path` or `--instanceId`
- `click_at`: click screen coordinates with `--x` and `--y`
- `drag`: drag from `--path`/`--instanceId` to `--toPath`/`--toInstanceId` or `--toX --toY`
- `long_press`: hold a target for `--duration-ms` milliseconds

Recommended flow: `editor play` -> `scene get_hierarchy` -> `input click` -> `get_logs --logType Error` -> `screenshot game` -> `editor stop`.

### `menu_item` - Invoke Menu Item

```bash
$CLI menu_item --menuPath "GameObject/Create Empty"
$CLI menu_item --menuPath "Assets/Create/Folder"
```

### `scene` - Scene Operations

```bash
$CLI scene load --scenePath "Assets/Scenes/Main.unity" [--mode additive]
$CLI scene save [--saveAs "Assets/Scenes/NewScene.unity"]
$CLI scene get_hierarchy [--depth 3] [--includeInactive false]
$CLI scene get_active
$CLI scene new [--setup empty]
```

### `screenshot` - Screenshot & GIF Recording

Files saved to `.aibridge/screenshots/`.

```bash
$CLI screenshot game  # Capture Game view screenshot (JPG)
$CLI screenshot scene_view  # Capture Scene view screenshot (JPG, Edit/Play mode)
$CLI screenshot scene_view --width 1920 --height 1080
$CLI screenshot gif --frameCount 50  # Record GIF
$CLI screenshot gif --frameCount 100 --fps 25 --scale 0.5 --colorCount 128
```

`game` and `gif` require Play mode. `scene_view` captures the last active Scene view and works in Edit mode.

**Scene View Parameters:**

| Parameter | Range | Default | Description |
|-----------|-------|---------|-------------|
| `--width` | 1-8192 | Scene view camera width | Output image width |
| `--height` | 1-8192 | Scene view camera height | Output image height |

**GIF Parameters:**

| Parameter | Range | Default | Description |
|-----------|-------|---------|-------------|
| `--frameCount` | 1-200 | Required | Number of frames to capture |
| `--fps` | 10-30 | 25 | Frames per second |
| `--scale` | 0.25-1.0 | 0.5 | Resolution scale factor |
| `--colorCount` | 64-256 | 128 | GIF palette color count |
| `--startDelay` | 0-5 seconds | 0 | Delay before capture starts |

**Estimated File Sizes:**

| Frames | Duration | Resolution | Size |
|--------|----------|------------|------|
| 25 | 1s | 480x270 | 200KB - 800KB |
| 50 | 2s | 480x270 | 400KB - 1.5MB |
| 100 | 4s | 480x270 | 800KB - 3MB |
| 200 | 8s | 480x270 | 1.5MB - 6MB |

### `selection` - Selection Operations

```bash
$CLI selection get [--includeComponents true]
$CLI selection set --path "Player" [--assetPath "Assets/Prefabs/Player.prefab"]
$CLI selection clear
$CLI selection add --path "Enemy1"
$CLI selection remove --path "Enemy1"
```

### `test` - Native Unity Test Runner

```bash
$CLI test run --mode EditMode
$CLI test run --test-name "MyNamespace.MyFixture.MyTest"
$CLI test status
```

### `transform` - Transform Operations

```bash
$CLI transform get --path "Player"
$CLI transform set_position --path "Player" --x 0 --y 1 --z 0 [--local true]
$CLI transform set_rotation --path "Player" --x 0 --y 90 --z 0
$CLI transform set_scale --path "Player" --x 2 --y 2 --z 2 [--uniform 2]
$CLI transform set_parent --path "Child" --parentPath "Parent"
$CLI transform look_at --path "Player" --targetPath "Enemy"
$CLI transform look_at --path "Player" --targetInstanceId 12345
$CLI transform look_at --path "Player" --targetX 0 --targetY 0 --targetZ 10
# look_at 目标参数三选一：--targetPath / --targetInstanceId / --targetX --targetY --targetZ
$CLI transform reset --path "Player"
$CLI transform set_sibling_index --path "Child" --index 0 [--first true] [--last true]
```
