param(
    [string]$ConfigType = "Code"
)

$ErrorActionPreference = "Stop"

# 设置变量
$PACKAGE = "Packages/cn.etetet.config"
$CONFIG_NAME = "Config"


$WORKSPACE = "Packages/cn.etetet.yiuiluban"
$GEN_CLIENT = "Packages/cn.etetet.yiuiluban/.Tools/Luban/Luban.dll"
$SOURCE_GEN_CLIENT = "Packages/cn.etetet.yiuiluban/DontNet~/luban/src/Luban/bin/Debug/net10.0/Luban.dll"
$CUSTOM = "Packages/cn.etetet.yiuiluban/.ToolsGen/Custom"

# powershell判断是不是Mac平台
# 注意: PowerShell 7 中 $IsMacOS 在所有平台都已定义($true/$false), 不能用 $null -ne 判断(恒为真)
$DotNet = "dotnet"
if ($IsMacOS) {
    $DotNet = "/usr/local/share/dotnet/dotnet"
}

if (!(Test-Path $GEN_CLIENT))
{
    $GEN_CLIENT = $SOURCE_GEN_CLIENT
}

function Invoke-Luban
{
    & $DotNet $GEN_CLIENT @args
    if ($LASTEXITCODE -ne 0)
    {
        throw "Luban 导出失败，ExitCode=$LASTEXITCODE Args=$($args -join ' ')"
    }
}

function Invoke-ConfigExport
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$TargetName,

        [Parameter(Mandatory = $true)]
        [string]$OutputCodeDir,

        [Parameter(Mandatory = $true)]
        [string]$OutputDataDir
    )

    Invoke-Luban --customTemplateDir $CUSTOM -t $TargetName -c cs-code -d cs-code-data --conf $PACKAGE/Luban/$CONFIG_NAME/luban.conf -x outputCodeDir=$OutputCodeDir -x outputDataDir=$OutputDataDir -x configGroup=$CONFIG_NAME
    Write-Host "==================== $TargetName 完成 ===================="
}

function Invoke-NumericTypeEnumExport
{
    $OutputCodeDir = "Packages/cn.etetet.numeric/Scripts/Model/Share/"

    Invoke-Luban --customTemplateDir $CUSTOM -t numeric -c cs-code --conf $PACKAGE/Luban/$CONFIG_NAME/luban.conf -x outputCodeDir=$OutputCodeDir -x configGroup=$CONFIG_NAME -x outputSaver.cs-code.cleanUpOutputDir=false
    Remove-Item (Join-Path $OutputCodeDir "NumericTypeTables.cs") -ErrorAction SilentlyContinue
    Write-Host "==================== numeric 完成 ===================="
}

# 客户端
Invoke-ConfigExport -TargetName client -OutputCodeDir "$PACKAGE/CodeMode/Model/Client/$CONFIG_NAME/" -OutputDataDir "$PACKAGE/CodeMode/Config/Client/$CONFIG_NAME/"

# 服务器
Invoke-ConfigExport -TargetName server -OutputCodeDir "$PACKAGE/CodeMode/Model/Server/$CONFIG_NAME/" -OutputDataDir "$PACKAGE/CodeMode/Config/Server/$CONFIG_NAME/"

# 所有
Invoke-ConfigExport -TargetName all -OutputCodeDir "$PACKAGE/CodeMode/Model/ClientServer/$CONFIG_NAME/" -OutputDataDir "$PACKAGE/CodeMode/Config/ClientServer/$CONFIG_NAME/"

# NumericType 只导出 enum 到 numeric 运行时代码目录
Invoke-NumericTypeEnumExport
