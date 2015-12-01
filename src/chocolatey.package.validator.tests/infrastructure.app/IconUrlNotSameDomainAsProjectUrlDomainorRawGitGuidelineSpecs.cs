namespace chocolatey.package.validator.tests.infrastructure.app
{
    using chocolatey.package.validator.infrastructure.app.rules;
    using Should;

    public abstract class IconUrlNotSameDomainAsProjectUrlDomainorRawGitGuidelineSpecsBase : TinySpec
    {
        protected IconUrlNotSameDomainAsProjectUrlDomainOrRawGitGuideline guideline;

        public override void Context()
        {
            this.guideline = new IconUrlNotSameDomainAsProjectUrlDomainOrRawGitGuideline();
        }
    }

    public class when_comparing_hosts : IconUrlNotSameDomainAsProjectUrlDomainorRawGitGuidelineSpecsBase
    {
        private string result;

        public override void Because()
        {
        }

        [Fact]
        public void should_return_true_from_same_hosts_with_single_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("test.com", "test.com").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_hosts_with_double_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("test.co.uk", "test.co.uk").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_rawgit_domain_with_single_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("cdn.rawgit.com", "test.com").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_rawgit_domain_with_double_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("cdn.rawgit.com", "test.co.uk").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_www_subdomain_on_project_and_no_subdomain_on_icon_and_single_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("test.com", "www.test.com").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_www_subdomain_on_project_and_no_subdomain_on_icon_and_double_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("test.co.uk", "www.test.co.uk").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_no_subdomain_on_project_and_www_subdomain_on_icon_and_single_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("www.test.com", "test.com").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_no_subdomain_on_project_and_www_subdomain_on_icon_and_double_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("www.test.co.uk", "test.co.uk").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_no_subdomain_on_project_and_arbitrary_subdomain_on_icon_and_single_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("somewildsubdomain.test.com", "test.com").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_www_subdomain_on_project_and_arbitrary_subdomain_on_icon_and_single_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("somewildsubdomain.test.com", "www.test.com").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_no_subdomain_on_project_and_arbitrary_subdomain_on_icon_and_double_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("somewildsubdomain.test.co.uk", "test.co.uk").ShouldBeTrue();
        }

        [Fact]
        public void should_return_true_from_same_domain_with_www_subdomain_on_project_and_arbitrary_subdomain_on_icon_and_double_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("somewildsubdomain.test.co.uk", "www.test.co.uk").ShouldBeTrue();
        }

        [Fact]
        public void should_return_false_from_different_domain_and_single_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("test1.com", "test.com").ShouldBeFalse();
        }

        [Fact]
        public void should_return_false_from_different_domain_and_double_level_top_level_domain()
        {
            guideline.is_icon_from_same_domain_as_project_or_rawgit("test1.co.uk", "test.co.uk").ShouldBeFalse();
        }
    }
}
