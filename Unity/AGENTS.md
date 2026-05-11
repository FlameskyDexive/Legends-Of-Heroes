<!-- AIBRIDGE:START {"assistant":"codex","templateId":"unity-integration","version":2,"target":"root-rule"} -->
## AIBridge Rules

**Skill**: `aibridge` - Unity CLI automation

**CLI**: `./AIBridgeCache/CLI/AIBridgeCLI.exe` (JSON output)

**Priority**:
- **Compile**: `compile unity` (default), `compile dotnet` (optional)
- **Asset Search**: `asset search/find --format paths` before filesystem search
- **Console**: `get_logs --logType Error`

**Quick Reference**:
```bash
./AIBridgeCache/CLI/AIBridgeCLI.exe compile unity
./AIBridgeCache/CLI/AIBridgeCLI.exe get_logs --logType Error
./AIBridgeCache/CLI/AIBridgeCLI.exe asset search --mode script --keyword "Player" --format paths
./AIBridgeCache/CLI/AIBridgeCLI.exe gameobject create --name "Cube" --primitiveType Cube
```

Reference: `/Packages/cn.lys.aibridge/Skill~/SKILL.md`
<!-- AIBRIDGE:END -->
