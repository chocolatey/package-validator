$ErrorActionPreference = 'Stop'
$serviceName = 'package-validator'

$pp = Get-PackageParameters
if (-not $pp.DoNotRestartService -or -not $pp.DoNotReinstallService) {
  Stop-ChocolateyWindowsService -Name $serviceName
}
