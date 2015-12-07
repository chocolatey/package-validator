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
    using System.Runtime.Versioning;
    using chocolatey.package.validator.infrastructure.app.rules;
    using chocolatey.package.validator.infrastructure.rules;
    using Moq;
    using NuGet;
    using Should;

    public abstract class DependencyWithNoVersionGuidelineSpecsBase : TinySpec
    {
        protected DependencyWithNoVersionGuideline guideline;
        protected Mock<IPackage> package = new Mock<IPackage>();
        protected PackageDependency packageDependencyWithVersion = new PackageDependency("fiddler", new VersionSpec(new SemanticVersion(1, 0, 0, 0)));
        protected PackageDependency packageDependencyWithNoVersion = new PackageDependency("chocolatey");

        public override void Context()
        {
            guideline = new DependencyWithNoVersionGuideline();
        }

        public class when_inspecting_single_dependency_that_has_no_version : DependencyWithNoVersionGuidelineSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();
                var packageDependencies = new List<PackageDependency>
            {
                packageDependencyWithNoVersion
            };
                var packageDependencySet = new PackageDependencySet(new FrameworkName(".NETFramework, Version = 4.0"), packageDependencies);
                package.Setup(p => p.DependencySets).Returns(
                    new List<PackageDependencySet>
                    {
                    packageDependencySet
                    });
            }

            public override void Because()
            {
                result = guideline.is_valid(package.Object);
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

        public class when_inspecting_dependencies_that_have_no_version : DependencyWithNoVersionGuidelineSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();
                var packageDependencies = new List<PackageDependency>
            {
                packageDependencyWithNoVersion,
                this.packageDependencyWithVersion
            };
                var packageDependencySet = new PackageDependencySet(new FrameworkName(".NETFramework, Version = 4.0"), packageDependencies);
                package.Setup(p => p.DependencySets).Returns(
                    new List<PackageDependencySet>
                    {
                    packageDependencySet
                    });
            }

            public override void Because()
            {
                result = guideline.is_valid(package.Object);
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

        public class when_inspecting_single_dependency_that_has_version : DependencyWithNoVersionGuidelineSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();
                var packageDependencies = new List<PackageDependency>
            {
                packageDependencyWithVersion
            };
                var packageDependencySet = new PackageDependencySet(new FrameworkName(".NETFramework, Version = 4.0"), packageDependencies);
                package.Setup(p => p.DependencySets).Returns(
                    new List<PackageDependencySet>
                    {
                    packageDependencySet
                    });
            }

            public override void Because()
            {
                result = guideline.is_valid(package.Object);
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
