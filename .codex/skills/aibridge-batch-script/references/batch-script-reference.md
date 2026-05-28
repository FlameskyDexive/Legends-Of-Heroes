<!-- AIBRIDGE:GENERATED COMMAND REFERENCE - DO NOT EDIT MANUALLY -->
# AIBridge Command Reference

此文件由 AIBridge 自动生成。需要修改命令说明时，请修改对应 ICommand 的 SkillDoc/SkillDescription。
`$CLI` 表示当前平台的 AIBridge CLI 调用方式，Windows 项目通常是 `./.aibridge/cli/AIBridgeCLI.exe`。

### `batch` - 脚本自动化执行

**用途**：自动化 Unity 编辑器操作和 CLI 命令执行，支持编译暂停/恢复

**Actions**：
- `from_text` - 直接执行脚本文本（自动写入 `.aibridge/scripts` 临时目录）
- `from_file` - 执行已有脚本文件（.txt 格式）

**脚本语法**：
```
log "消息"              # 输出日志
delay 毫秒数            # 延迟执行
call [CLI命令] [参数]   # 调用 AIBridge CLI（可选 --timeout 毫秒数）
menu 菜单路径           # 执行编辑器菜单项
wait_compile [timeoutMs]                 # 等待 Unity 编译完成
wait_playmode playing|stopped [timeoutMs] # 等待进入/退出 PlayMode
assert_log_empty [Error|Warning|Log]      # 断言 Console 指定最低等级日志为空
assert_object "Canvas/Button"           # 断言场景对象存在
set_var name value                       # 设置脚本变量
print_var name                           # 打印脚本变量
dialog click ok | yes | Save             # 声明后续弹窗自动点击；再次声明会覆盖前一个策略
# 注释                 # 行注释
```

**使用示例**：
```bash
# 直接执行脚本文本
$CLI batch from_text --text "call editor log 'Hello'\ndelay 1000"

# 执行并保存脚本到 `.aibridge/scripts` 目录
$CLI batch from_text --text "..." --name "my_script" --keep-file

# 执行已有脚本文件
$CLI batch from_file --file "script.txt"
```

**脚本示例**：
```
# 自动化构建流程
log "开始构建"
dialog click ok | yes | Save
call compile unity
wait_compile 120000
call scene get_hierarchy --depth 2
assert_log_empty Error
menu File/Save Project
```

**典型场景**：编译流程、场景批处理、资源管理、重复任务自动化
