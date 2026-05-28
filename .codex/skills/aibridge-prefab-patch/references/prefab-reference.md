<!-- AIBRIDGE:GENERATED COMMAND REFERENCE - DO NOT EDIT MANUALLY -->
# AIBridge Command Reference

此文件由 AIBridge 自动生成。需要修改命令说明时，请修改对应 ICommand 的 SkillDoc/SkillDescription。
`$CLI` 表示当前平台的 AIBridge CLI 调用方式，Windows 项目通常是 `./.aibridge/cli/AIBridgeCLI.exe`。

### `prefab` - Prefab Operations

```bash
$CLI prefab instantiate --prefabPath "Assets/Prefabs/Player.prefab" [--posX 5 --posY 0 --posZ 0]
$CLI prefab save --gameObjectPath "Player" --savePath "Assets/Prefabs/Player.prefab"
$CLI prefab unpack --gameObjectPath "Player(Clone)" [--completely true]
$CLI prefab get_info --prefabPath "Assets/Prefabs/Player.prefab"
$CLI prefab get_hierarchy --prefabPath "Assets/Prefabs/Player.prefab" [--depth 4] [--includeInactive false]
$CLI prefab apply --gameObjectPath "Player(Clone)"
$CLI prefab patch --prefabPath "Assets/Prefabs/Player.prefab" --ops "patch_ops.json" [--dryRun true]
```
