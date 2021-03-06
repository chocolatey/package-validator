﻿// Copyright © 2015 - Present RealDimensions Software, LLC
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

    public abstract class PackageIdDoesNotEndWithConfigRequirementSpecsBase : TinySpec
    {
        protected PackageIdDoesNotEndWithConfigRequirement requirement;
        protected Mock<IPackage> package = new Mock<IPackage>();

        public override void Context()
        {
            requirement = new PackageIdDoesNotEndWithConfigRequirement();
        }

        public class when_inspecting_package_ending_with_config : PackageIdDoesNotEndWithConfigRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Id).Returns("package.config");
            }

            public override void Because()
            {
                result = requirement.is_valid(package.Object);
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

        public class when_inspecting_package_not_ending_with_config : PackageIdDoesNotEndWithConfigRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Id).Returns("packageA");
            }

            public override void Because()
            {
                result = requirement.is_valid(package.Object);
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

        public class when_inspecting_package_ending_with_configuration : PackageIdDoesNotEndWithConfigRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Id).Returns("bob.configuration");
            }

            public override void Because()
            {
                result = requirement.is_valid(package.Object);
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