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

    public abstract class PackageSourceUrlShouldBeValidRequirementSpecs : TinySpec
    {
        protected PackageSourceUrlValidRequirement validationCheck;
        protected Mock<IPackage> package = new Mock<IPackage>();

        public override void Context()
        {
            validationCheck = new PackageSourceUrlValidRequirement();
        }
    }

    public class when_inspecting_package_with_empty_package_source_url : PackageSourceUrlShouldBeValidRequirementSpecs
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

    public class when_inspecting_package_with_invalid_package_source_url : PackageSourceUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.PackageSourceUrl).Returns(new Uri("http://invalid.url"));
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

    public class when_inspecting_package_with_valid_package_source_url : PackageSourceUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.PackageSourceUrl).Returns(new Uri("https://chocolatey.org"));
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

    public class when_inspecting_package_with_valid_package_source_url_using_redir : PackageSourceUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            //This should redirect to https:// with a 301
            package.Setup(p => p.PackageSourceUrl).Returns(new Uri("http://chocolatey.org"));
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
    public class when_inspecting_package_with_valid_package_source_url_that_requires_Useragent_header : PackageSourceUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            // This URL should require UserAgent header, otherwise a 403 response is returned
            package.Setup(p => p.PackageSourceUrl).Returns(new Uri("https://hamapps.com/php/license.php"));
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
    public class when_inspecting_package_with_valid_package_source_url_that_requires_tls_1_3 : PackageSourceUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            // This URL should require TLS 1.3 on the client, otherwise it won't be able to establish a connection
            // to the server
            package.Setup(p => p.PackageSourceUrl).Returns(new Uri("https://talk.atomisystems.com/"));
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
    /// This test case comes from a reported issue here: https://github.com/chocolatey/package-validator/issues/199
    /// </summary>
    public class when_inspecting_package_with_valid_package_source_url_with_non_http_or_https_scheme : PackageSourceUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            // Non http/https URLs shouldn't be validated, and if passed in, should simply return true
            package.Setup(p => p.PackageSourceUrl).Returns(new Uri("git://git.code.sf.net/p/mrviewer/code"));
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
