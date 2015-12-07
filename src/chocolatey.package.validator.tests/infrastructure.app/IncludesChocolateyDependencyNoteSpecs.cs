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
    using Moq;
    using NuGet;
    using Should;
    using validator.infrastructure.app.rules;
    using validator.infrastructure.rules;

    public abstract class IncludesChocolateyDependencyNoteSpecsBase : TinySpec
    {
        protected IncludesChocolateyDependencyNote rule;
        protected Mock<IPackage> package = new Mock<IPackage>();
        protected PackageDependency fiddlerPackageDependency = new PackageDependency("fiddler");
        protected PackageDependency chocolateyPackageDependency = new PackageDependency("chocolatey");

        public override void Context()
        {
            rule = new IncludesChocolateyDependencyNote();
        }
    }

    public class when_inspecting_dependencies_that_include_chocolatey : IncludesChocolateyDependencyNoteSpecsBase
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();
            var packageDependencies = new List<PackageDependency>
            {
                fiddlerPackageDependency,
                chocolateyPackageDependency
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
            result = rule.is_valid(package.Object);
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

    public class when_inspecting_dependencies_that_only_include_chocolatey : IncludesChocolateyDependencyNoteSpecsBase
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();
            var packageDependencies = new List<PackageDependency>
            {
                chocolateyPackageDependency
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
            result = rule.is_valid(package.Object);
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

    public class when_inspecting_empty_dependencies : IncludesChocolateyDependencyNoteSpecsBase
    {
        private PackageValidationOutput result;

        public override void Because()
        {
            result = rule.is_valid(package.Object);
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

    public class when_inspecting_dependencies_that_do_not_include_chocolatey : IncludesChocolateyDependencyNoteSpecsBase
    {
        private PackageValidationOutput result;

        public override void Context()
        {
            base.Context();
            var packageDependencies = new List<PackageDependency>
            {
                fiddlerPackageDependency
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
            result = rule.is_valid(package.Object);
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
