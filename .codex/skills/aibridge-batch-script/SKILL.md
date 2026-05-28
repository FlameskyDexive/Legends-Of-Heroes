---
name: aibridge-batch-script
description: AIBridge batch and multi command automation. Use when Codex needs to run multiple Unity Editor CLI operations together, write batch scripts, use multi --cmd or multi --stdin, handle call/delay/log/menu lines, avoid PowerShell quoting issues, or automate long/JSON-heavy AIBridgeCLI workflows.
---

# AIBridge Batch Script

Use this Skill for `batch` and `multi` automation. Prefer `multi --stdin` for long scripts, JSON-heavy commands, or PowerShell quoting-sensitive commands.

`$CLI` means the platform-appropriate AIBridge CLI invocation, usually `./.aibridge/cli/AIBridgeCLI.exe` on Windows.

## Rules

- Run commands from the Unity project root.
- Plain `multi` lines become Batch `call` lines automatically.
- Native Batch lines are kept as-is: `call`, `delay`, `log`, `menu`, `wait_compile`, `wait_playmode`, `assert_log_empty`, `assert_object`, `set_var`, `print_var`, `dialog click`, and `#` comments.
- `dialog click ok | yes | Save` declares dialog auto-click choices; after that line executes, later steps auto-click the first matching logical choice or visible button text. A later `dialog click` declaration replaces the previous strategy. Keep the CLI invocation waiting; `--no-wait` cannot continue clicking after the process exits.
- Keep generated scripts under `.aibridge/scripts/` when a file is needed.
- Use `--keep-file` only when debugging the generated script.

## Quick Examples

```bash
$CLI multi --cmd "editor log --message Step1&get_logs --logType Error --count 1"
```

```powershell
$script = @'
editor log --message "开始批处理"
dialog click ok | yes | Save
wait_compile 120000
delay 1000
get_logs --logType Error --count 1
'@
$script | & $CLI multi --stdin
```

## References

- `references/batch-script-reference.md`: generated command reference for `batch`.
