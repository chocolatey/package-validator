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

    public abstract class DependencyWithNoVersionGuidelineSpecsBase : TinySpec
    {
        protected DependencyWithNoVersionGuideline guideline;
        protected Mock<IPackage> package = new Mock<IPackage>();
        protected PackageDependency packageDependencyWithMinVersion;
        protected PackageDependency packageDependencyWithMaxVersion;
        protected PackageDependency packageDependencyWithNoVersion = new PackageDependency("chocolatey");

        public override void Context()
        {
            var minVersionSpec = new VersionSpec
            {
                MinVersion = new SemanticVersion("1.0.0"),
                IsMinInclusive = true
            };
            packageDependencyWithMinVersion = new PackageDependency("fiddler", minVersionSpec);

            var minAndMaxVersionSpec = new VersionSpec
            {
                MinVersion = new SemanticVersion("1.0.0"),
                MaxVersion = new SemanticVersion("2.0.0"),
                IsMinInclusive = true
            };
            packageDependencyWithMaxVersion = new PackageDependency("dude", minAndMaxVersionSpec);

            guideline = new DependencyWithNoVersionGuideline();
        }

        public class when_inspecting_a_package_that_has_no_dependencies : DependencyWithNoVersionGuidelineSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.DependencySets).Returns( new List<PackageDependencySet>
                {
                    new PackageDependencySet(null, new List<PackageDependency>())
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
                var packageDependencySet = new PackageDependencySet(null, packageDependencies);
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
                    packageDependencyWithMinVersion
                };
                var packageDependencySet = new PackageDependencySet(null, packageDependencies);
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

        public class when_inspecting_single_dependency_that_has_min_version : DependencyWithNoVersionGuidelineSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();
                var packageDependencies = new List<PackageDependency>
                {
                    packageDependencyWithMinVersion
                };
                var packageDependencySet = new PackageDependencySet(null, packageDependencies);
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
        
        public class when_inspecting_single_dependency_that_has_max_version : DependencyWithNoVersionGuidelineSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();
                var packageDependencies = new List<PackageDependency>
                {
                    packageDependencyWithMaxVersion
                };
                var packageDependencySet = new PackageDependencySet(null, packageDependencies);
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
