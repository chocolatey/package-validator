// Copyright © 2015 - Present RealDimensions Software, LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// 
// You may obtain a copy of the License at
// 
// 	http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.package.validator.tests.infrastructure.app
{
    using System.Collections.Generic;
    using Moq;
    using NuGet;
    using Should;
    using validator.infrastructure.app.rules;
    using validator.infrastructure.rules;

    public abstract class ScriptsDoNotDownloadFromFossHubRequirementSpecsBase : TinySpec
    {
        protected ScriptsDoNotDownloadFromFossHubRequirement validationCheck;
        protected Mock<IPackage> package = new Mock<IPackage>();
        protected Mock<IPackageFile> packageFile = new Mock<IPackageFile>();

        public override void Context()
        {
            validationCheck = new ScriptsDoNotDownloadFromFossHubRequirement();
        }

        public class when_inspecting_package_with_installation_script_that_attempts_to_download_from_fosshub_: ScriptsDoNotDownloadFromFossHubRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"
$ErrorActionPreference = 'Stop'
 
$packageName = 'audacity'
$url32 = Get-UrlFromFosshub 'https://www.fosshub.com/Audacity.html/audacity-win-2.1.2.exe'
$checksum32  = '22e0f0ada3e8d24690dd741ca9feb868dffc024d45d2cd3168f8c54c47eec3c9'
 
$packageArgs = @{
  packageName    = $packageName
  fileType       = 'exe'
  url            = $url32
  checksum       = $checksum32
  checksumType   = 'sha256'
  silentArgs     = '/VERYSILENT'
  validExitCodes = @(0)
}
 
Install-ChocolateyPackage @packageArgs
".to_stream());
                packageFile.Setup(f => f.Path).Returns("chocolateyInstall.ps1");

                package.Setup(p => p.GetFiles()).Returns(
                    new List<IPackageFile>()
                    {
                        packageFile.Object
                    });
            }

            public override void Because()
            {
                result = validationCheck.is_valid(package.Object);
            }

            [Fact]
            public void should_not_be_valid()
            {
                result.Validated.ShouldBeFalse();
            }

            [Fact]
            public void should_not_override_the_base_message()
            {
                result.ValidationFailureMessageOverride.ShouldBeNull();
            }
        }

        public class when_inspecting_package_with_installation_script_that_has_fosshub_in_comments : ScriptsDoNotDownloadFromFossHubRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"

#$url32 = Get-UrlFromFosshub 'https://www.fosshub.com/Audacity.html/audacity-win-2.1.2.exe'
 
Install-ChocolateyPackage @packageArgs
".to_stream());
                packageFile.Setup(f => f.Path).Returns("chocolateyInstall.ps1");

                package.Setup(p => p.GetFiles()).Returns(
                    new List<IPackageFile>()
                    {
                        packageFile.Object
                    });
            }

            public override void Because()
            {
                result = validationCheck.is_valid(package.Object);
            }

            [Fact]
            public void should_be_valid()
            {
                result.Validated.ShouldBeTrue();
            }

            [Fact]
            public void should_not_override_the_base_message()
            {
                result.ValidationFailureMessageOverride.ShouldBeNull();
            }
        }

        public class when_inspecting_package_with_installation_script_that_under_normal_circumstances : ScriptsDoNotDownloadFromFossHubRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"write-host 'hi!'".to_stream());
                packageFile.Setup(f => f.Path).Returns("chocolateyInstall.ps1");

                package.Setup(p => p.GetFiles()).Returns(
                    new List<IPackageFile>()
                    {
                        packageFile.Object
                    });
            }

            public override void Because()
            {
                result = validationCheck.is_valid(package.Object);
            }

            [Fact]
            public void should_be_valid()
            {
                result.Validated.ShouldBeTrue();
            }

            [Fact]
            public void should_not_override_the_base_message()
            {
                result.ValidationFailureMessageOverride.ShouldBeNull();
            }
        }
    }
}
