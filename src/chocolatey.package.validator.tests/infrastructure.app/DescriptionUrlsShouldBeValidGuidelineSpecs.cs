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

    public abstract class DescriptionUrlsShouldBeValidGuidelineSpecs : TinySpec
    {
        protected DescriptionUrlValidGuideline validationCheck;
        protected Mock<IPackage> package = new Mock<IPackage>();

        public override void Context()
        {
            validationCheck = new DescriptionUrlValidGuideline();
        }
    }

    public class when_inspecting_package_with_empty_description : DescriptionUrlsShouldBeValidGuidelineSpecs
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

    public class when_inspecting_package_with_one_invalid_url_in_description : DescriptionUrlsShouldBeValidGuidelineSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.Description).Returns(@"
Chocolatey is a package manager for Windows (like apt-get but for Windows). It was designed to be a decentralized framework for quickly installing applications and tools that you need. It is built on the NuGet infrastructure currently using PowerShell as its focus for delivering packages from the distros to your door, err computer.

Chocolatey is brought to you by the work and inspiration of the community, the work and thankless nights of the [Chocolatey Team](https://invalid.url).");
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

    public class when_inspecting_package_with_valid_url_in_description : DescriptionUrlsShouldBeValidGuidelineSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.Description).Returns(@"
Chocolatey is a package manager for Windows (like apt-get but for Windows). It was designed to be a decentralized framework for quickly installing applications and tools that you need. It is built on the NuGet infrastructure currently using PowerShell as its focus for delivering packages from the distros to your door, err computer.

Chocolatey is brought to you by the work and inspiration of the community, the work and thankless nights of the [Chocolatey Team](https://github.com/orgs/chocolatey/people), with Rob heading up the direction.

You can host your own sources and add them to Chocolatey, you can extend Chocolatey's capabilities, and folks, it's only going to get better.
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

    public class when_inspecting_package_with_multiple_valid_url_in_description : DescriptionUrlsShouldBeValidGuidelineSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.Description).Returns(@"
Chocolatey is a package manager for Windows (like apt-get but for Windows). It was designed to be a decentralized framework for quickly installing applications and tools that you need. It is built on the NuGet infrastructure currently using PowerShell as its focus for delivering packages from the distros to your door, err computer.

Chocolatey is brought to you by the work and inspiration of the community, the work and thankless nights of the [Chocolatey Team](https://github.com/orgs/chocolatey/people), with Rob heading up the direction.

You can host your own sources and add them to Chocolatey, you can extend Chocolatey's capabilities, and folks, it's only going to get better.
### Information

 * [Chocolatey Website and Community Package Repository](https://chocolatey.org)
 * [Mailing List](http://groups.google.com/group/chocolatey) / [Release Announcements Only Mailing List](https://groups.google.com/group/chocolatey-announce) / [Build Status Mailing List](http://groups.google.com/group/chocolatey-build-status)
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

    public class when_inspecting_package_with_multiple_valid_url_and_one_invalid_in_description : DescriptionUrlsShouldBeValidGuidelineSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.Description).Returns(@"
Chocolatey is a package manager for Windows (like apt-get but for Windows). It was designed to be a decentralized framework for quickly installing applications and tools that you need. It is built on the NuGet infrastructure currently using PowerShell as its focus for delivering packages from the distros to your door, err computer.

Chocolatey is brought to you by the work and inspiration of the community, the work and thankless nights of the [Chocolatey Team](https://invalid.url), with Rob heading up the direction.

You can host your own sources and add them to Chocolatey, you can extend Chocolatey's capabilities, and folks, it's only going to get better.
### Information

 * [Chocolatey Website and Community Package Repository](https://chocolatey.org)
 * [Mailing List](http://groups.google.com/group/chocolatey) / [Release Announcements Only Mailing List](https://groups.google.com/group/chocolatey-announce) / [Build Status Mailing List](http://groups.google.com/group/chocolatey-build-status)
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

    public class when_inspecting_package_with_valid_url_in_description_using_redir : DescriptionUrlsShouldBeValidGuidelineSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            //This should redirect to https:// with a 301
            package.Setup(p => p.Description).Returns(@"
Chocolatey is a package manager for Windows (like apt-get but for Windows). It was designed to be a decentralized framework for quickly installing applications and tools that you need. It is built on the NuGet infrastructure currently using PowerShell as its focus for delivering packages from the distros to your door, err computer.

Chocolatey is brought to you by the work and inspiration of the community, the work and thankless nights of the [Chocolatey Team](http://github.com/orgs/chocolatey/people), with Rob heading up the direction.

You can host your own sources and add them to Chocolatey, you can extend Chocolatey's capabilities, and folks, it's only going to get better.
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
    public class when_inspecting_package_with_valid_url_that_requires_Useragent_header : DescriptionUrlsShouldBeValidGuidelineSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            // This URL should require UserAgent header, otherwise a 403 response is returned
            package.Setup(p => p.Description).Returns(@"
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
    /// This test case comes from a reported issue here: https://github.com/chocolatey/package-validator/issues/216
    /// </summary>
    public class when_inspecting_package_with_valid_url_that_requires_newer_tls_cipher : DescriptionUrlsShouldBeValidGuidelineSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            // This URL should require TLS 1.3 on the client, otherwise it won't be able to establish a connection
            // to the server
            package.Setup(p => p.Description).Returns(@"
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
