$ErrorActionPreference = 'Stop'
$serviceName = 'package-validator'

Uninstall-ChocolateyWindowsService -Name $serviceName