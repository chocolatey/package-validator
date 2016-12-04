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

    public abstract class CopyrightAndAuthorFieldsShouldntContainEmailRequirementSpecsBase : TinySpec
    {
        protected CopyrightAndAuthorFieldsShouldntContainEmailRequirement requirement;
        protected Mock<IPackage> package = new Mock<IPackage>();
        protected Mock<IPackageFile> packageFile = new Mock<IPackageFile>();

        public override void Context()
        {
            requirement = new CopyrightAndAuthorFieldsShouldntContainEmailRequirement();
        }

        public class when_inspecting_package_with_email_in_copyright_field : CopyrightAndAuthorFieldsShouldntContainEmailRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Copyright).Returns("bob@bob.co.uk");
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

        public class when_inspecting_package_with_no_email_in_copyright_field : CopyrightAndAuthorFieldsShouldntContainEmailRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Copyright).Returns("Copyright 2016");
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

        public class when_inspecting_package_with_email_in_author_field : CopyrightAndAuthorFieldsShouldntContainEmailRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Authors).Returns(new List<string> { "Chocolatey", "bob@bob.co.uk"});
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

        public class when_inspecting_package_with_no_email_in_author_field : CopyrightAndAuthorFieldsShouldntContainEmailRequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Authors).Returns(new List<string> {"Chocolatey", "NuGet"} );
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