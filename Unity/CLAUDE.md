<!-- AIBRIDGE:START {"assistant":"claude","templateId":"unity-integration","version":2,"target":"root-rule"} -->
## AIBridge Unity Integration

**Skill**: `aibridge` - Unity CLI automation tool

**CLI**: `./AIBridgeCache/CLI/AIBridgeCLI.exe` (outputs JSON by default)

**Core Workflows**:
- **Compile**: Use `compile unity` (default), `compile dotnet` (optional validation only)
- **Asset Search**: Use `asset search/find --format paths` before generic filesystem search
- **Console Logs**: `get_logs --logType Error`
- **Scene/GameObject**: Create, modify, inspect hierarchy
- **Visual Verification**: `screenshot game`, `screenshot gif --frameCount 50` (Play Mode)

**Quick Reference**:
```bash
./AIBridgeCache/CLI/AIBridgeCLI.exe compile unity
./AIBridgeCache/CLI/AIBridgeCLI.exe get_logs --logType Error
./AIBridgeCache/CLI/AIBridgeCLI.exe asset search --mode script --keyword "Player" --format paths
./AIBridgeCache/CLI/AIBridgeCLI.exe gameobject create --name "Cube" --primitiveType Cube
```

**Skill Documentation**: [AIBridge Skill](/.claude/skills/aibridge/SKILL.md)
<!-- AIBRIDGE:END -->
