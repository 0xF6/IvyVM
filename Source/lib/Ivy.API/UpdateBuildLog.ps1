param([string]$projectPath="c:\temp\", [string]$targetDLLFullPath="c:\temp\",[string]$extraAttribute="XXX", [string]$buildConfig="Debug")
try
{
    $BuildLogFileLocation = [System.IO.Path]::Combine($projectPath,"BuildLog.txt")
    $AssemblyInfoFileLocation = $projectPath+"Properties\AssemblyInfo.cs"
	$attributeRead = "readonly"
    [Console]::Writeline("Updating build log...")    
    [Console]::Writeline([string]::Format("Log File Location: {0}",$BuildLogFileLocation))

    $enc = [Console]::OutputEncoding
    $dllVersion = (get-item $targetDLLFullPath).VersionInfo.FileVersion
    $dllVersionVersion = new-Object System.Version $dllVersion
    [int]$nxtBuildNumber = $dllVersionVersion.Build+1
    $nextdllVersion = new-Object System.Version $dllVersionVersion.Major, $dllVersionVersion.Minor, $nxtBuildNumber, 5
    [Console]::Writeline([string]::Format("Dll File: {0}", $targetDLLFullPath))
    [Console]::Writeline([string]::Format("Current Output Dll File Version: {0}", $dllVersion))
    $date = Get-Date
    $logText = [string]::Format("{0,-20}{1}{2,-25}{3,-15}{4,-20}",$dllVersion,"     -    ",[System.Convert]::ToString($date), $buildConfig, $extraAttribute)
    out-file $BuildLogFileLocation -Force -inputobject  $logText -append -encoding ASCII
	$BuildLogFileLocation =(gci $BuildLogFileLocation -force);
	$BuildLogFileLocation.Attributes = $BuildLogFileLocation.Attributes -bor ([System.IO.FileAttributes]$attributeRead).value__;
    [Console]::Writeline([string]::Format("Build log entry for this build - {0}", $logText))
    [Console]::Writeline([string]::Format("Write version {0} in VersionInfo file.", $nextdllVersion))
    $versionInfoUPdated = 0
    (get-Content $AssemblyInfoFileLocation -Encoding utf8) | ForEach-Object {
        if($_ -match '\[assembly: AssemblyFileVersion.*\("(?<content>.*)"\)\]')
        {
            $VerObject = new-Object System.Version $matches['content']
            $NewVerObject = $nextdllVersion
            $_ = '[assembly: AssemblyFileVersion("'+$NewVerObject+'")]'
            [Console]::Writeline([string]::Format("Written version {0} in VersionInfo file", $nextdllVersion))
        }
        $_
    } | set-Content -Encoding utf8 $AssemblyInfoFileLocation
	
    (get-Content $AssemblyInfoFileLocation -Encoding utf8) | ForEach-Object {
        if($_ -match '\[assembly: AssemblyVersion.*\("(?<content>.*)"\)\]')
        {
            $VerObject = new-Object System.Version $matches['content']
            $NewVerObject = $nextdllVersion
            $_ = '[assembly: AssemblyVersion("'+$NewVerObject+'")]'
            [Console]::Writeline([string]::Format("Written version {0} in VersionInfo file", $nextdllVersion))
        }
        $_
    } | set-Content -Encoding utf8 $AssemblyInfoFileLocation
	
    (get-Content $AssemblyInfoFileLocation -Encoding utf8) | ForEach-Object {
        if($_ -match '\[assembly: AssemblyFileVersion.*\("(?<content>.*)"\)\]')
        {
            $updatedVersion = new-Object System.Version $matches['content']
            if($updatedVersion -match $nextdllVersion)
            {
                [Console]::Writeline([string]::Format("Confirmed version {0} is written in VersionInfo file!", $nextdllVersion))
                $versionInfoUPdated = 1
            }
        }
        $_
    }| set-Content -Encoding utf8 tmp.txt
    
    if($versionInfoUPdated = 0)
    {
        [Console]::Writeline([string]::Format("Assembly file version failed to update to proper next version"))
        exit 1
    }
 
    [Console]::OutputEncoding = $enc

    [Console]::Writeline("Build log updated successfully.")    
    exit 0
}
catch
{
    [Console]::Writeline($_)
    exit 1
}