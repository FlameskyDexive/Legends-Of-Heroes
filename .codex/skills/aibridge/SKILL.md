---
name: aibridge
description: Unity Editor and Player Runtime CLI integration for AIBridge. Use when Codex needs to compile Unity, inspect Console logs, search/read assets, manipulate GameObjects, Transforms, components, SerializedProperty values, scenes, screenshots/GIFs, connect to built Player runtime targets, simulate Play Mode runtime input, editor focus/menu items/game view, or look up AIBridgeCLI command syntax. For batch/multi scripts use aibridge-batch-script. For complex prefab asset edits use aibridge-prefab-patch. For unsupported direct Unity YAML serialized asset edits use unity-yaml-editing.
---

# AI Bridge Unity Skill

## Invocation

Run commands from the Unity project root.

**CLI Path:** `./.aibridge/cli/AIBridgeCLI.exe`

**Example Alias:** `$CLI` is a shorthand for the platform-appropriate AIBridge CLI invocation. Examples use `$CLI` for consistency; replace it with the matching invocation form when running commands directly.

**Invocation Forms:**

- Windows executable: `./.aibridge/cli/AIBridgeCLI.exe <command> [action] [options]`
- macOS/Linux DLL: `dotnet ./.aibridge/cli/AIBridgeCLI.dll <command> [action] [options]`
- PowerShell call operator: `& "./.aibridge/cli/AIBridgeCLI.exe" <command> [action] [options]`

Most Unity-side commands require an `action` such as `asset search` or `inspector set_property`. CLI-only commands and helpers can differ: `focus` has no action, `dialog` uses `status/click/wait`, and `multi` uses `--cmd` or `--stdin`. Use `--help` or the generated command reference for the exact form.

**Global Options:**

- `--timeout <ms>` - Timeout (default: 5000)
- `--on-dialog <mode>` - When a Unity command is blocked by a modal dialog, optionally wait or click: `none`, `wait`, `cancel`, `save`, `discard`, `ok`, `yes`, `no`, `delete`, `replace`
- `--raw` / `--pretty` - JSON output (default: raw)
- `--json <json>` / `--stdin` - Complex parameters
- `--help` - Show help

**Cache Directory:** `.aibridge/` (Editor commands/results/screenshots and Runtime targets under `.aibridge/runtime/targets/`)

**Dialog Output Rule:** `dialog status` omits `blockedByDialog` and `dialogs` when no blocking Unity modal dialog is detected. Missing fields mean no dialog.

## Operating Rules

- Use `compile unity` for Unity validation. `compile dotnet` is an explicit extra solution-build check, not a fallback.
- For Unity assets, prefer `asset search/find --format paths`; use host file reads for file contents, and `asset read_text` only when host reads are unavailable.
- Resource edit order: use `inspector set_property/set_properties` for single or batched fields, `aibridge-prefab-patch` for complex Prefab structure edits it supports, Unity Editor scripts for high-level generated assets, and `unity-yaml-editing` only for unsupported serialized-file structure work.
- For scene objects, Prefabs, and serialized Unity assets, discover targets with `inspector get_components/get_properties/find_property`, then write with AIBridge/Unity APIs when possible; avoid raw YAML unless no supported API can express the operation.
- Direct YAML edits are acceptable only after loading `unity-yaml-editing`, including unsupported Scene/Prefab/ScriptableObjectTable/custom `.asset` operations. Preserve `m_Script`, `fileID/guid/type` references, YAML indentation, and paired `.meta` GUIDs; validate afterward with import, targeted inspection, `compile unity`, and Error logs.
- For prefab asset edits, use `assetPath + objectPath + componentName` or `componentIndex`; `componentInstanceId` is scene-only.
- For complex prefab asset edits, use the `aibridge-prefab-patch` skill and prefer `prefab patch --ops <file>` with dry-run first.
- In PowerShell, avoid inline complex `--json`; build JSON in a variable, escape embedded quotes for native EXE argument passing, and pass command parameters directly, especially `inspector set_properties --values $values`.
- `focus` is Windows CLI-only. `dialog` is CLI-only, uses Windows window APIs or macOS Accessibility permission, and omits dialog fields when no modal dialog is detected. `screenshot game` and `screenshot gif` require Play Mode; `screenshot scene_view` works in Edit mode when a Scene view is open.
- `input` requires Play Mode and an active EventSystem; use it with `gameview`, `screenshot`, and `get_logs` for UI interaction checks.
- `runtime` is CLI-only and talks to `AIBridgeRuntime` inside a Player or Play Mode target. Use `runtime list_targets` first, then target `latest` or a specific target id.
- Code Index is a separate optional Skill. Use `aibridge-code-index` only when that Skill is installed and project rules say Code Index is enabled; otherwise use `rg`, file reads, and regular AIBridge commands.

## Related Resources

- `aibridge-prefab-patch`: specialized Skill for complex prefab asset edits.
- `aibridge-batch-script`: specialized Skill for `batch` / `multi` script automation.
- `aibridge-code-index`: optional Skill for read-only semantic code lookup when Code Index is enabled.
- `unity-yaml-editing`: fallback Skill for direct UnityYAML edits when AIBridge/Unity APIs cannot express the operation.
- `references/command-reference.md`: generated CLI command syntax for common commands.
- `references/inspector-property-reference.md`: generated Inspector and SerializedProperty syntax.

## Essential Commands

### `focus` - Bring Unity to Foreground

CLI-only, Windows-only. Triggers Unity refresh/compile via Windows API.

```bash
$CLI focus
# Output: {"Success":true,"ProcessId":1234,"WindowTitle":"MyProject - Unity 6000.0.51f1","Error":null}
```

### `dialog` - Inspect Or Click Unity Modal Dialogs

CLI-only. Use when a save/confirm modal may be blocking the Editor. Unity commands report detected dialog details instead of blindly waiting; choose an explicit `dialog click` or use `--on-dialog` for unattended handling.

```bash
$CLI dialog status
$CLI dialog click --choice cancel
$CLI dialog click --button "Don't Save"
$CLI dialog wait --timeout 5000 --click cancel
$CLI scene load --scenePath Assets/Main.unity --on-dialog cancel
```

### `multi` - Execute Multiple Commands (Recommended)

Use `aibridge-batch-script` before writing long scripts or JSON-heavy `multi --stdin` commands.

```bash
$CLI multi --cmd 'editor log --message Step1&gameobject create --name Cube --primitiveType Cube'
$CLI multi --stdin
```

In batch/multi scripts, `dialog click ok | yes | Save` declares persistent dialog auto-click choices for subsequent steps; a later `dialog click` replaces the previous strategy. Keep the CLI invocation waiting, because `--no-wait` exits before it can continue clicking.

### `input` - Runtime Input Simulation

Play Mode only. Use the generated command reference for all actions and parameters.

```bash
$CLI input click --path "Canvas/StartButton"
$CLI input drag --path "Canvas/Item" --toPath "Canvas/Slot" --frames 12
```

### `runtime` - Built Player Runtime Bridge

Requires an `AIBridgeRuntime` component in the running Player or Play Mode scene. HTTP is the default Runtime transport; if the default HTTP port is occupied, Runtime auto-increments and writes the actual URL to the live heartbeat. LAN discovery also auto-increments from the default UDP port, and CLI defaults scan the local HTTP/discovery port ranges so multiple local Editors or built Players can be discovered. File transport remains available for Editor/local compatibility. Configure defaults in `AIBridge/Settings > Runtime`, and inspect live targets plus discovery cache in `AIBridge/Players`.

```bash
$CLI runtime list_targets
$CLI runtime status --target latest
$CLI runtime discover
$CLI runtime diagnose --target latest
$CLI runtime logs --target latest --logType Error --count 100
$CLI runtime perf --target latest --duration 5s --interval 100ms
$CLI runtime screenshot --target latest
$CLI runtime handlers --target latest
$CLI runtime call --target latest --action qa.open_panel --json "{\"panel\":\"Inventory\"}"
```

For multiple local Players, run `$CLI runtime list_targets` first and pass a specific target id such as `$CLI runtime status --target AIBridgeDev_12345`. `$CLI runtime diagnose --target <id>` diagnoses that target's resolved HTTP URL.

For remote phones, run `$CLI runtime discover` on the LAN first, then target the discovered id or URL. On Android USB, run `adb reverse tcp:27182 tcp:27182` first, then call `--transport http --url http://127.0.0.1:27182`; `adb` is not a runtime transport mode.

---

**Skill Version**: 1.0
**Package**: cn.lys.aibridge
