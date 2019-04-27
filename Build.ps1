function Exec {		
    [CmdletBinding()]		
    param(		
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,		
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)		
    )		
    & $cmd		
    if ($lastexitcode -ne 0) {		
        throw ("Exec: " + $errorMessage)		
    }		
}

if(Test-Path .\artifacts) { 
  Remove-Item .\artifacts -Force -Recurse 
}

exec { & dotnet restore .\JenkDotNetCoreCommon\ }

exec { & dotnet build .\JenkDotNetCoreCommon\ -c Release }

$root = (split-path -parent $MyInvocation.MyCommand.Definition)
$version = [System.Reflection.Assembly]::LoadFile("$root\JenkDotNetCoreCommon\bin\Release\netcoreapp2.1\JenkDotNetCoreCommon.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)
$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$revision = [convert]::ToInt32($revision, 10)

echo "versionStr $versionStr" 
echo "api_key $api_key"
echo "api_key ${api_key}"
echo "api_key $env:api_key"

exec { & dotnet pack .\JenkDotNetCoreCommon\JenkDotNetCoreCommon.csproj -c Release -o ..\artifacts --version-suffix=$revision }

exec { & dotnet nuget push .\artifacts\JenkDotNetCoreCommon.$versionStr.nupkg -k "%api_key%" -s https://api.nuget.org/v3/index.json } 