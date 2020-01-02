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
    using chocolatey.package.validator.infrastructure.app.rules;
    using chocolatey.package.validator.infrastructure.rules;
    using Moq;
    using NuGet;
    using Should;
    using System;

    public abstract class ReleaseNotesUrlsShouldBeValidRequirementSpecs : TinySpec
    {
        protected ReleaseNotesUrlValidRequirement validationCheck;
        protected Mock<IPackage> package = new Mock<IPackage>();

        public override void Context()
        {
            validationCheck = new ReleaseNotesUrlValidRequirement();
        }
    }

    public class when_inspecting_package_with_empty_releasenotes : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();
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

    public class when_inspecting_package_with_one_invalid_url_in_releasenotes : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.ReleaseNotes).Returns(@"
See all - https://invalid.url
");
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

    public class when_inspecting_package_with_valid_url_in_releasenotes : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.ReleaseNotes).Returns(@"
See all - https://github.com/chocolatey/choco/blob/stable/CHANGELOG.md

## 0.10.11
### BUG FIXES
");
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

    public class when_inspecting_package_with_multiple_valid_url_in_releasenotes : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.ReleaseNotes).Returns(@"
See all - https://github.com/chocolatey/choco/blob/stable/CHANGELOG.md

## 0.10.11
### BUG FIXES
 * Fix - AutoUninstaller - Captures registry snapshot escaping quotes - unable to find path for uninstall - see [#1540](https://github.com/chocolatey/choco/issues/1540)
 * Fix - Installation/Setup - Use of Write-Host in Install-ChocolateyPath.ps1 prevents non-interactive installation of Chocolatey itself - see [#1560](https://github.com/chocolatey/choco/issues/1560)
");
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

    public class when_inspecting_package_with_multiple_valid_url_and_one_invalid_in_releasenotes : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.ReleaseNotes).Returns(@"
See all - https://github.com/chocolatey/choco/blob/stable/CHANGELOG.md

## 0.10.11
### BUG FIXES
 * Fix - AutoUninstaller - Captures registry snapshot escaping quotes - unable to find path for uninstall - see [#1540](https://github.com/chocolatey/choco/issues/1540)
 * Fix - Installation/Setup - Use of Write-Host in Install-ChocolateyPath.ps1 prevents non-interactive installation of Chocolatey itself - see [#1560](https://invalid.url)
");
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

    public class when_inspecting_package_with_valid_url_in_releasenotes_using_redir : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            //This should redirect to https:// with a 301
            package.Setup(p => p.ReleaseNotes).Returns(@"
See all - http://github.com/chocolatey/choco/blob/stable/CHANGELOG.md

## 0.10.11
### BUG FIXES
 * Fix - AutoUninstaller - Captures registry snapshot escaping quotes - unable to find path for uninstall - see [#1540](https://github.com/chocolatey/choco/issues/1540)
 * Fix - Installation/Setup - Use of Write-Host in Install-ChocolateyPath.ps1 prevents non-interactive installation of Chocolatey itself - see [#1560](https://github.com/chocolatey/choco/issues/1560)
");
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

    /// <summary>
    /// This test case comes from a reported issue here: https://github.com/chocolatey/package-validator/issues/200#issuecomment-570052281
    /// </summary>
    public class when_inspecting_package_with_valid_url_in_releasenotes_that_requires_Useragent_header : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            // This URL should require UserAgent header, otherwise a 403 response is returned
            package.Setup(p => p.ReleaseNotes).Returns(@"
This is a test description with a [url](https://hamapps.com/php/license.php) that requires a User Agent Header in order to work.
");
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

    /// <summary>
    /// This test case comes from a reported issue here: https://github.com/chocolatey/package-validator/issues/200#issuecomment-570052562
    /// </summary>
    public class when_inspecting_package_with_valid_url_in_releasenotes_that_requires_tls_1_3 : ReleaseNotesUrlsShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            // This URL should require TLS 1.3 on the client, otherwise it won't be able to establish a connection
            // to the server
            package.Setup(p => p.ReleaseNotes).Returns(@"
This is a test description with a [url](https://talk.atomisystems.com/) that requires TLS 1.3 in the client.
");
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
