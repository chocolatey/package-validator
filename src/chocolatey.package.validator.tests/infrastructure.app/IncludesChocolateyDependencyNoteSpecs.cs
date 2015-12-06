namespace chocolatey.package.validator.tests.infrastructure.app
{
    using System.Collections.Generic;
    using System.Runtime.Versioning;
    using chocolatey.package.validator.infrastructure.app.rules;
    using NuGet;
    using Moq;
    using Should;

    public abstract class IncludesChocolateyDependencyNoteSpecsBase : TinySpec
    {
        protected IncludesChocolateyDependencyNote note;
        protected Mock<IPackage> packageWithoutChocolateyDependency = new Mock<IPackage>();
        protected Mock<IPackage> packageWithChocolateyDependency = new Mock<IPackage>();

        public override void Context()
        {
            note = new IncludesChocolateyDependencyNote();
            var fiddlerPackageDependency = new PackageDependency("fiddler");
            var chocolateyPackageDependency = new PackageDependency("chocolatey");
            var packageDependenciesWithoutChocolatey = new List<PackageDependency> { fiddlerPackageDependency };
            var packageDependenciesWithChocolatey = new List<PackageDependency> { fiddlerPackageDependency, chocolateyPackageDependency };
            var dependencySetWithoutChocolatey = new PackageDependencySet(new FrameworkName(".NETFramework, Version = 4.0"), packageDependenciesWithoutChocolatey);
            var dependencySetWithChocolatey = new PackageDependencySet(new FrameworkName(".NETFramework, Version = 4.0"), packageDependenciesWithChocolatey);
            var dependencySetsWithoutChocolatey = new List<PackageDependencySet> { dependencySetWithoutChocolatey };
            var dependencySetsWithChocolatey = new List<PackageDependencySet> { dependencySetWithChocolatey };
            packageWithoutChocolateyDependency.Setup(p => p.DependencySets).Returns(dependencySetsWithoutChocolatey);
            packageWithChocolateyDependency.Setup(p => p.DependencySets).Returns(dependencySetsWithChocolatey);
        }
    }

    public class when_inspecting_dependencies : IncludesChocolateyDependencyNoteSpecsBase
    {
        private string result;

        public override void Because()
        {
        }

        [Fact]
        public void should_return_false_with_chocolatey_as_a_dependency()
        {
            note.inspect_dependencies_for_chocolatey(packageWithChocolateyDependency.Object).ShouldBeFalse();
        }

        [Fact]
        public void should_return_true_with_chocolatey_not_as_a_dependency()
        {
            note.inspect_dependencies_for_chocolatey(packageWithoutChocolateyDependency.Object).ShouldBeTrue();
        }
    }
}
