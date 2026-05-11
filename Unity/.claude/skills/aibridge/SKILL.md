---
name: aibridge
description: "Unity CLI 工具。执行编译、资源搜索、游戏对象操作、变换操作、组件检查、场景/预制体管理、截图捕获和 GIF 录制。支持多命令执行、运行时扩展和脚本自动化。"
commands: [compile, asset, gameobject, transform, inspector, selection, scene, prefab, screenshot, gameview, get_logs, focus, batch, multi, menu_item, editor, script]
capabilities: [asset-lookup, scene-editing, build-automation, visual-verification, component-inspection, hierarchy-manipulation, prefab-management, console-monitoring, editor-control, script-automation]
triggers: [unity, compile, gameobject, transform, component, scene, prefab, screenshot, gif, console, log, asset, hierarchy, inspector, selection, menu, editor, focus, batch, gameview, resolution, script, automation]
---

# AI Bridge Unity Skill

## AI Operating Rules

**Compile Priority:**
- Use `compile unity` (default) - requires Unity Editor running
- Use `compile dotnet` (optional) - separate solution-build validation only, NOT a fallback

**Asset Lookup Priority:**
1. `asset search/find --format paths` (Unity AssetDatabase index - fastest)
2. `asset get_path` (only when starting from GUID)
3. `asset load` (only for metadata confirmation)
4. Host AI native file-read tool (for file contents)
5. `asset read_text` (fallback when native reads unavailable)
6. Generic grep/filesystem search (last resort)

**Special Constraints:**
- `focus` - Windows-only, CLI-only, triggers Unity refresh/compile
- `screenshot` - Requires Play Mode
- `multi` - Preferred for batch operations

---

## Invocation

**CLI Path:** `./AIBridgeCache/CLI/AIBridgeCLI.exe` (run from Unity project root)

**Alias (used in examples below):** `$CLI`

**OS Syntax:**
- Windows: `./AIBridgeCache/CLI/AIBridgeCLI.exe <command> <action> [options]`
- macOS/Linux: `dotnet ./AIBridgeCache/CLI/AIBridgeCLI.dll <command> <action> [options]`
- PowerShell: `& "./AIBridgeCache/CLI/AIBridgeCLI.exe" <command> <action> [options]`

**Global Options:**
- `--timeout <ms>` - Timeout (default: 5000)
- `--raw` / `--pretty` - JSON output (default: raw)
- `--json <json>` / `--stdin` - Complex parameters
- `--help` - Show help

**Cache Directory:** `AIBridgeCache/` (commands, results, screenshots)

---

## Command Reference

### `focus` - Bring Unity to Foreground

CLI-only, Windows-only. Triggers Unity refresh/compile via Windows API.

```bash
$CLI focus
# Output: {"Success":true,"ProcessId":1234,"WindowTitle":"MyProject - Unity 6000.0.51f1","Error":null}
```

### `multi` - Execute Multiple Commands (Recommended)

```bash
$CLI multi --cmd 'editor log --message Step1&gameobject create --name Cube --primitiveType Cube'
$CLI multi --stdin  # Read from stdin (one per line)
```

## Command Reference

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

### `batch` - 脚本自动化执行

**用途**：自动化 Unity 编辑器操作和 CLI 命令执行，支持编译暂停/恢复

**Actions**：
- `from_text` - 直接执行脚本文本（自动写入 Cache 临时目录）
- `from_file` - 执行已有脚本文件（.txt 格式）

**脚本语法**：
```
log "消息"              # 输出日志
delay 毫秒数            # 延迟执行
call [CLI命令] [参数]   # 调用 AIBridge CLI（可选 --timeout 毫秒数）
menu 菜单路径           # 执行编辑器菜单项
# 注释                 # 行注释
```

**使用示例**：
```bash
# 直接执行脚本文本
$CLI batch from_text --text "call editor log 'Hello'\ndelay 1000"

# 执行并保存脚本到 Cache 目录
$CLI batch from_text --text "..." --name "my_script" --keep-file

# 执行已有脚本文件
$CLI batch from_file --file "script.txt"
```

**脚本示例**：
```
# 自动化构建流程
log "开始构建"
call compile unity
delay 2000
call scene get_hierarchy --depth 2
menu File/Save Project
```

**典型场景**：编译流程、场景批处理、资源管理、重复任务自动化

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
$CLI get_logs [--count 100] [--logType Error|Warning]
```

### `inspector` - Component Operations

```bash
$CLI inspector get_components --path "Player"
$CLI inspector get_properties --path "Player" --componentName "Transform"
$CLI inspector set_property --path "Player" --componentName "Rigidbody" --propertyName "mass" --value 10
$CLI inspector set_property --path "Player" --componentName "MeshRenderer" --propertyName "m_Materials.Array.data[0]" --value "Assets/Materials/MyMat.mat"
$CLI inspector add_component --path "Player" --typeName "Rigidbody"
$CLI inspector remove_component --path "Player" --componentName "Rigidbody"
```

### `menu_item` - Invoke Menu Item

```bash
$CLI menu_item --menuPath "GameObject/Create Empty"
$CLI menu_item --menuPath "Assets/Create/Folder"
```

### `prefab` - Prefab Operations

```bash
$CLI prefab instantiate --prefabPath "Assets/Prefabs/Player.prefab" [--posX 5 --posY 0 --posZ 0]
$CLI prefab save --gameObjectPath "Player" --savePath "Assets/Prefabs/Player.prefab"
$CLI prefab unpack --gameObjectPath "Player(Clone)" [--completely true]
$CLI prefab get_info --prefabPath "Assets/Prefabs/Player.prefab"
$CLI prefab get_hierarchy --prefabPath "Assets/Prefabs/Player.prefab" [--depth 4] [--includeInactive false]
$CLI prefab apply --gameObjectPath "Player(Clone)"
```

### `scene` - Scene Operations

```bash
$CLI scene load --scenePath "Assets/Scenes/Main.unity" [--mode additive]
$CLI scene save [--saveAs "Assets/Scenes/NewScene.unity"]
$CLI scene get_hierarchy [--depth 3] [--includeInactive false]
$CLI scene get_active
$CLI scene new [--setup empty]
```

### `screenshot` - Screenshot & GIF Recording (Play Mode)

**Requires Play mode.** Files saved to `AIBridgeCache/screenshots/`.

```bash
$CLI screenshot game  # Capture Game view screenshot (JPG)
$CLI screenshot gif --frameCount 50  # Record GIF
$CLI screenshot gif --frameCount 100 --fps 25 --scale 0.5 --colorCount 128
```

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

### `transform` - Transform Operations

```bash
$CLI transform get --path "Player"
$CLI transform set_position --path "Player" --x 0 --y 1 --z 0 [--local true]
$CLI transform set_rotation --path "Player" --x 0 --y 90 --z 0
$CLI transform set_scale --path "Player" --x 2 --y 2 --z 2 [--uniform 2]
$CLI transform set_parent --path "Child" --parentPath "Parent"
$CLI transform look_at --path "Player" --targetPath "Enemy" [--targetX 0 --targetY 0 --targetZ 10]
$CLI transform reset --path "Player"
$CLI transform set_sibling_index --path "Child" --index 0 [--first true]
```
---

**Skill Version**: 1.0
**Package**: cn.lys.aibridge
