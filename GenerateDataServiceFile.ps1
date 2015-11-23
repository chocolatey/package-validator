$thisDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$dataSvcUtil = 'C:\Program Files (x86)\Microsoft WCF Data Services\5.6\bin\tools\DataSvcUtil.exe'

$serviceFile = "$thisDir\src\chocolatey.package.validator\infrastructure.app\webservices\ChocolateySubmittedFeedService.cs"

. $dataSvcUtil /out:"$serviceFile" /version:2.0 /language:csharp /nologo /uri:https://chocolatey.org/api/v2/submitted/