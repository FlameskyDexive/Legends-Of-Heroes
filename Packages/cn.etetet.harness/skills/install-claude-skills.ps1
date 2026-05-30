<#
.SYNOPSIS
    把 harness 包内的 ET skills 注册到 Claude Code 能识别的 `<repo>/.claude/skills/`。

.DESCRIPTION
    Claude Code 只发现这些位置的 skill：`~/.claude/skills/`、`<repo>/.claude/skills/`、插件的 `skills/`。
    本仓库的 ET skill 实际存放在 `Packages/cn.etetet.harness/skills/<name>/SKILL.md`，不在上述任何位置，
    所以默认识别不到。本脚本为每个含 `SKILL.md` 的子目录在 `<repo>/.claude/skills/` 下创建同名符号链接，
    让它们成为可被 Skill 工具调用、可按 description 自动匹配的原生 skill。

    符号链接是“每台机器的本地状态”，不建议提交进 git（已在 .gitignore 忽略 .claude/skills/）。
    换机器 / 重新 clone 后重跑本脚本即可。

.NOTES
    需用 pwsh（PowerShell 7）执行。Windows 建符号链接需开发者模式或管理员权限；
    若创建符号链接失败，脚本会自动回退为目录联接（junction，本地目录无需额外权限）。

.EXAMPLE
    pwsh ./Packages/cn.etetet.harness/skills/install-claude-skills.ps1
#>
[CmdletBinding()]
param(
    # 显式指定仓库根；默认从脚本位置上溯 3 级推断（skills -> cn.etetet.harness -> Packages -> repo root）。
    [string]$RepoRoot
)

$ErrorActionPreference = 'Stop'

$skillsSrc = $PSScriptRoot
if (-not $RepoRoot) {
    $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
}
$dest = Join-Path $RepoRoot '.claude\skills'

Write-Host "Skills 源目录 : $skillsSrc"
Write-Host "仓库根       : $RepoRoot"
Write-Host "注册目标     : $dest"
Write-Host ''

New-Item -ItemType Directory -Path $dest -Force | Out-Null

# 发现所有“含 SKILL.md 的子目录”，即一个 skill。
$skillDirs = Get-ChildItem -Path $skillsSrc -Directory |
    Where-Object { Test-Path (Join-Path $_.FullName 'SKILL.md') }

if (-not $skillDirs) {
    Write-Warning "在 $skillsSrc 下未发现任何含 SKILL.md 的 skill 目录。"
    return
}

foreach ($s in $skillDirs) {
    $name     = $s.Name
    $linkPath = Join-Path $dest $name
    $target   = $s.FullName

    # 已存在：是链接就刷新；是真实目录（含内容）就跳过以免误删数据。
    if (Test-Path $linkPath) {
        $existing = Get-Item $linkPath -Force
        if ($existing.LinkType) {
            Remove-Item $linkPath -Force -Recurse
        }
        else {
            Write-Warning "跳过 $name：$linkPath 已是真实目录（非链接），请手动确认后再处理。"
            continue
        }
    }

    try {
        New-Item -ItemType SymbolicLink -Path $linkPath -Target $target -ErrorAction Stop | Out-Null
        $kind = 'SymbolicLink'
    }
    catch {
        # 回退为 junction（Windows 本地目录无需开发者模式/管理员权限）。
        New-Item -ItemType Junction -Path $linkPath -Target $target | Out-Null
        $kind = 'Junction'
    }

    $ok = Test-Path (Join-Path $linkPath 'SKILL.md')
    Write-Host ("  [{0}] {1} -> {2}  (SKILL.md: {3})" -f $kind, $name, $target, ($(if ($ok) { 'OK' } else { 'MISSING' })))
}

Write-Host ''
Write-Host '完成。重启 Claude Code 会话后，这些 skill 才会被加载（skill 在会话启动时发现）。'
