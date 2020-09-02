$ErrorActionPreference = 'Stop'

$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$serviceExe = 'package-validator.exe'
$destinationPath = Join-Path (Get-ToolsLocation) -ChildPath 'chocolatey-package-validator'
$configPath = Join-Path -Path $destinationPath -ChildPath 'package-validator.exe.config'

$serviceParams = @{
  Name                  = 'package-validator'
  DisplayName           = 'Chocolatey Package Validator'
  Description           = 'Validates the contents of a package against the package review process parts that can be validated by a machine.'
  StartupType           = 'Automatic'
  ServiceExecutablePath = Join-Path -Path $destinationPath -ChildPath $serviceExe
  DoNotReinstallService = $true
}

$pp = Get-PackageParameters

# This is an upgrade if the service already exists
$isUpgrade = [bool](Get-Service -Name $serviceParams.Name -ErrorAction SilentlyContinue)

# We only need to reinstall the service if the ReinstallService package parameter is provided or
# this is an install (not an upgrade)
if (($isUpgrade -and $pp.ReinstallService) -or (-not $isUpgrade)) {
  if ($isUpgrade) {
    Write-Warning 'Service will be reinstalled after upgrade. This may cause issues and require a reboot and package upgrade run again.'
  }

  $serviceParams.DoNotReinstallService = $false
}

if ($pp.DoNotStartService) {
  Write-Warning 'Service will not be started after installation. This will need to be done manually.'
  $serviceParams.DoNotStartService = $true
}

if (-not $isUpgrade) {
  if ($pp.Username) {
    $serviceParams.Username = $pp.Username
  }
  else {
    Write-Warning "Service will be installed under the LocalSystem account"
  }

  if ($pp.EnterPassword) {
    $serviceParams.Password = Read-Host "Enter password for $($packageArgs.Username):" -AsSecureString
  }

  if ($pp.Password) {
    $serviceParams.Password = $pp.Password
  }
  elseif ($pp.Username -and -not $pp.EnterPassword) {
    Write-Warning "Service will be installed under the '$($pp.Username)' account with a random password."
  }
}

if (-not (Test-Path -Path $configPath)) {
  Write-Warning "Cannot find config file '$configPath'. Service will not be started after installation. This will need to be done manually if you do not reboot."
  $serviceParams.DoNotStartService = $true
}

$sourcePath = (Join-Path -Path $toolsDir -ChildPath "files")
Write-Verbose "Copying files from '$sourcePath' to '$destinationPath'"
Copy-Item -Path "$sourcePath\*" -Destination $destinationPath -Recurse -Force

Write-Verbose "Removing package files from '$sourcePath'"
Remove-Item -Path $sourcePath -Recurse -Force

# If the service is already installed this should simply restart it unless the
# $serviceParams.DoNotReinstallService is false in which case it will reinstall the service.
Install-ChocolateyWindowsService @serviceParams
