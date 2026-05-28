---
name: aibridge-code-index
description: Optional read-only AIBridge Code Index semantic lookup for Unity projects. Use only when this Skill is installed and project rules or AIBridge settings say Code Index is enabled. Use for CLI-only symbol, definition, reference, implementation, derived-type, caller, and diagnostic queries. Do not use when Code Index is disabled; use rg, host file reads, and regular AIBridge commands instead.
---

# AIBridge Code Index Skill

Run commands from the Unity project root.

**CLI Path:** `./.aibridge/cli/AIBridgeCLI.exe`

**Availability Rule:** This Skill is installed only when `AIBridge/Settings > Code Index > Enable Code Index` is enabled. If project rules say Code Index is disabled, or `code_index status` reports `enabled=false` or `state=disabled`, stop using `code_index` and fall back to `rg` plus normal file reads.

## Operating Rules

- `code_index` is CLI-only, read-only, and does not rename, refactor, or write files.
- Use it for symbol lookup and source navigation before broad code edits when semantic lookup is useful.
- Trust only results marked `semantic=true`; fallback text candidates are explicitly marked `semantic=false`.
- If disabled or unavailable, do not retry repeatedly. Use `rg`, file reads, and regular AIBridge commands.

## Commands

```bash
$CLI code_index status
$CLI code_index doctor
$CLI code_index warmup
$CLI code_index symbol --query PlayerController
$CLI code_index definition --file Assets/Scripts/Foo.cs --line 42 --column 17
$CLI code_index references --file Assets/Scripts/Foo.cs --line 42 --column 17
$CLI code_index implementations --type Game.IFoo
$CLI code_index derived --type Game.BasePanel
$CLI code_index callers --file Assets/Scripts/Foo.cs --line 42 --column 17
$CLI code_index diagnostics --file Assets/Scripts/Foo.cs
```

Results include `enabled`, `semantic`, `source`, `state`, `stale`, `workspaceMode`, snapshot metadata, excluded snapshot counts, `projectRoot`, and `solution`. Unity can generate/prewarm the snapshot from `AIBridge/Settings > Code Index`, where PackageCache source indexing and ignored assembly/source-path patterns can also be configured.

---

**Skill Version**: 1.0
**Package**: cn.lys.aibridge
