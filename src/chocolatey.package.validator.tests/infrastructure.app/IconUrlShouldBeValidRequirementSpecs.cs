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

    public abstract class IconUrlShouldBeValidRequirementSpecs : TinySpec
    {
        protected IconUrlValidRequirement validationCheck;
        protected Mock<IPackage> package = new Mock<IPackage>();

        public override void Context()
        {
            validationCheck = new IconUrlValidRequirement();
        }
    }

    public class when_inspecting_package_with_empty_icon_url : IconUrlShouldBeValidRequirementSpecs
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

    public class when_inspecting_package_with_invalid_icon_url : IconUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.IconUrl).Returns(new Uri("http://invalid.url"));
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

    public class when_inspecting_package_with_valid_icon_url : IconUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            package.Setup(p => p.IconUrl).Returns(new Uri("https://chocolatey.org"));
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

    public class when_inspecting_package_with_valid_icon_url_using_redir : IconUrlShouldBeValidRequirementSpecs
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();

            //This should redirect to https:// with a 301
            package.Setup(p => p.IconUrl).Returns(new Uri("http://chocolatey.org")); 
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
