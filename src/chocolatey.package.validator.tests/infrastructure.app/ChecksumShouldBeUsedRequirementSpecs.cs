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
    using chocolatey.package.validator.infrastructure.app.rules;
    using chocolatey.package.validator.infrastructure.rules;
    using Moq;
    using NuGet;
    using Should;

    public abstract class ChecksumShouldBeUsedRequirementSpecsBase : TinySpec
    {
        protected ChecksumShouldBeUsedRequirement requirement;
        protected Mock<IPackage> package = new Mock<IPackage>();
        protected Mock<IPackageFile> packageFile = new Mock<IPackageFile>();

        public override void Context()
        {
            requirement = new ChecksumShouldBeUsedRequirement();
        }

        public class when_inspecting_package_with_installation_script_with_checksum : ChecksumShouldBeUsedRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"$packageName = '0ad'
$installerType = 'exe'
$silentArgs = '/S'
$url = 'http://releases.wildfiregames.com/0ad-0.0.20-alpha-win32.exe'
$checksum = '579942a0391947a86e63d649cd69d9c2615bcd3b'
$checksumType = 'sha256'
$validExitCodes = @(0)

Install-ChocolateyPackage -PackageName '$packageName' `
                          -FileType '$installerType' `
                          -SilentArgs '$silentArgs' `
                          -Url '$url' `
                          -ValidExitCodes $validExitCodes `
                          -Checksum '$checksum' `
                          -ChecksumType '$checksumType'".to_stream());
                packageFile.Setup(f => f.Path).Returns("chocolateyInstall.ps1");

                package.Setup(p => p.GetFiles()).Returns(new List<IPackageFile>() { packageFile.Object });
            }

            public override void Because()
            {
                result = this.requirement.is_valid(package.Object);
            }

            [Fact]
            public void shoul_be_valid()
            {
                result.Validated.ShouldBeTrue();
            }

            [Fact]
            public void should_not_override_the_base_message()
            {
                result.ValidationFailureMessageOverride.ShouldBeNull();
            }
        }

        public class when_inspecting_package_with_installation_script_without_checksum : ChecksumShouldBeUsedRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"$packageName = '0ad'
$installerType = 'exe'
$silentArgs = '/S'
$url = 'http://releases.wildfiregames.com/0ad-0.0.20-alpha-win32.exe'
$validExitCodes = @(0)

Install-ChocolateyPackage -PackageName '$packageName' `
                          -FileType '$installerType' `
                          -SilentArgs '$silentArgs' `
                          -Url '$url' `
                          -ValidExitCodes $validExitCodes".to_stream());
                packageFile.Setup(f => f.Path).Returns("chocolateyInstall.ps1");

                package.Setup(p => p.GetFiles()).Returns(new List<IPackageFile>() { packageFile.Object });
            }

            public override void Because()
            {
                result = this.requirement.is_valid(package.Object);
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

        public class when_inspecting_package_with_installation_script_using_splatting_with_checksum : ChecksumShouldBeUsedRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"$packageName = '0ad'
$fileType = 'exe'
$silentArgs = '/S'
$url = 'http://releases.wildfiregames.com/0ad-0.0.20-alpha-win32.exe'
$validExitCodes = @(0)
$checksum = '579942a0391947a86e63d649cd69d9c2615bcd3b'
$checksumType = 'sha256'

$packageArgs = @{
  packageName   = $packageName
  fileType      = $fileType
  silentArgs    = $silentArgs
  url           = $url
  validExitCodes= $validExitCodes
  checksum      = $checksum
  checksumType  = $checksumType
}

Install-ChocolateyPackage @packageArgs".to_stream());
                packageFile.Setup(f => f.Path).Returns("chocolateyInstall.ps1");

                package.Setup(p => p.GetFiles()).Returns(new List<IPackageFile>() { packageFile.Object });
            }

            public override void Because()
            {
                result = this.requirement.is_valid(package.Object);
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

        public class when_inspecting_package_with_installation_script_using_splatting_without_checksum : ChecksumShouldBeUsedRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"$packageName = '0ad'
$fileType = 'exe'
$silentArgs = '/S'
$url = 'http://releases.wildfiregames.com/0ad-0.0.20-alpha-win32.exe'
$validExitCodes = @(0)

$packageArgs = @{
  packageName   = $packageName
  fileType      = $fileType
  silentArgs    = $silentArgs
  url           = $url
  validExitCodes= $validExitCodes
}

Install-ChocolateyPackage @packageArgs".to_stream());
                packageFile.Setup(f => f.Path).Returns("chocolateyInstall.ps1");

                package.Setup(p => p.GetFiles()).Returns(new List<IPackageFile>() { packageFile.Object });
            }

            public override void Because()
            {
                result = this.requirement.is_valid(package.Object);
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
    }
}