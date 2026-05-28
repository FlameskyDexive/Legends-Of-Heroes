<!-- AIBRIDGE:GENERATED COMMAND REFERENCE - DO NOT EDIT MANUALLY -->
# AIBridge Command Reference

此文件由 AIBridge 自动生成。需要修改命令说明时，请修改对应 ICommand 的 SkillDoc/SkillDescription。
`$CLI` 表示当前平台的 AIBridge CLI 调用方式，Windows 项目通常是 `./.aibridge/cli/AIBridgeCLI.exe`。

### `inspector` - Serialized Component/Asset Properties

```bash
$CLI inspector get_components --path "Player"
$CLI inspector get_properties --path "Player" --componentName "Transform"
$CLI inspector set_property --path "Player" --componentName "Rigidbody" --propertyName "mass" --value 10
$CLI inspector set_property --path "Player" --componentName "MeshRenderer" --propertyName "m_Materials.Array.data[0]" --value "Assets/Materials/MyMat.mat"
$CLI inspector get_components --assetPath "Assets/UI/LoginPanel.prefab" --objectPath "Root/Button"
$CLI inspector set_property --assetPath "Assets/UI/LoginPanel.prefab" --objectPath "Root/Button" --componentName "RectTransform" --propertyName "m_AnchoredPosition.x" --value 100
$CLI inspector set_property --assetPath "Assets/Data/Config.asset" --propertyName "maxCount" --value 20
$CLI inspector add_component --path "Player" --typeName "Rigidbody"
$CLI inspector remove_component --path "Player" --componentName "Rigidbody"
```

PowerShell JSON recommendation:
```powershell
$values = (@{ 'm_AnchoredPosition.x' = 100; 'm_AnchoredPosition.y' = -40; 'm_LocalPosition.z' = 0 } | ConvertTo-Json -Compress) -replace '"', '\"'
& $CLI inspector set_properties --assetPath 'Assets/UI/LoginPanel.prefab' --objectPath 'Root/Button' --componentName RectTransform --values $values
```

Use `assetPath + objectPath` for prefab asset editing. Prefer SerializedProperty paths over YAML text edits; YAML patching should only be a last-resort dry-run workflow.
For prefab assets, prefer `componentName` or `componentIndex`; `componentInstanceId` is scene-only because prefab assets are loaded in a temporary editing stage.
Avoid inline complex `--json` in PowerShell; build a JSON variable for `--values` and escape embedded quotes for native EXE argument passing, or pipe JSON through stdin when the command supports it.
