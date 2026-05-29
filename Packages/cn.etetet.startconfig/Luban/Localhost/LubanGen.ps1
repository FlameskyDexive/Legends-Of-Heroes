# 设置变量
$ErrorActionPreference = "Stop"

$PACKAGE = "Packages/cn.etetet.startconfig"
$CONFIG_NAME = "Localhost"


$WORKSPACE = "Packages/cn.etetet.yiuiluban"
$GEN_CLIENT = "Packages/cn.etetet.yiuiluban/.Tools/Luban/Luban.dll"
$CUSTOM = "Packages/cn.etetet.yiuiluban/.ToolsGen/Custom"

# powershell判断是不是Mac平台
$DotNet = "dotnet"
if ($IsMacOS) {
    $DotNet = "/usr/local/share/dotnet/dotnet"
}

function Invoke-Luban
{
    & $DotNet $GEN_CLIENT @args
    if ($LASTEXITCODE -ne 0)
    {
        throw "Luban 导出失败，ExitCode=$LASTEXITCODE Args=$($args -join ' ')"
    }
}

Invoke-Luban --customTemplateDir $CUSTOM -t all -c cs-code -d cs-code-data --conf $PACKAGE/Luban/$CONFIG_NAME/luban.conf -x outputCodeDir=$PACKAGE/CodeMode/Model/Server/StartConfig -x outputDataDir=$PACKAGE/CodeMode/Config/Server/$CONFIG_NAME -x configGroup=$CONFIG_NAME
Write-Host "==================== $CONFIG_NAME Server完成 ===================="

Invoke-Luban --customTemplateDir $CUSTOM -t all -c cs-code -d cs-code-data --conf $PACKAGE/Luban/$CONFIG_NAME/luban.conf -x outputCodeDir=$PACKAGE/CodeMode/Model/ClientServer/StartConfig -x outputDataDir=$PACKAGE/CodeMode/Config/ClientServer/$CONFIG_NAME -x configGroup=$CONFIG_NAME
Write-Host "==================== $CONFIG_NAME ClientServer完成 ===================="
