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

    public abstract class DescriptionWordCountMaximum4000RequirementSpecsBase : TinySpec
    {
        protected DescriptionWordCountMaximum4000Requirement requirement;
        protected Mock<IPackage> package = new Mock<IPackage>();

        public override void Context()
        {
            requirement = new DescriptionWordCountMaximum4000Requirement();
        }

        public class when_inspecting_package_with_description_character_count_less_than_4000 : DescriptionWordCountMaximum4000RequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Description).Returns("This is a valid description");
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

        public class when_inspecting_package_with_description_character_count_greater_than_4000 : DescriptionWordCountMaximum4000RequirementSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                package.Setup(p => p.Description).Returns("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus eu odio eu massa eleifend iaculis. Nam blandit mi eget risus ullamcorper ornare et quis diam. Phasellus dapibus neque rhoncus nulla porta, et molestie orci auctor. Aliquam eu dui at metus tristique condimentum. Mauris ut est mollis, sagittis turpis sit amet, maximus ipsum. Sed metus diam, molestie vel justo vitae, hendrerit iaculis eros. Donec viverra, velit consectetur tincidunt posuere, dui lorem egestas ante, vel finibus quam lectus non eros. Integer accumsan, lacus et lobortis porta, orci sem tempor nisi, ut faucibus ligula augue nec orci. Praesent sed imperdiet elit, id ultricies lorem.Sed condimentum, eros ac consequat imperdiet, purus nunc mattis ex, sit amet elementum lacus eros vitae urna.Ut dapibus purus vel turpis luctus, sit amet maximus augue rutrum.Maecenas ut massa volutpat, sodales risus id, rhoncus enim.Proin interdum porta magna in eleifend.Aenean id feugiat ante.Phasellus vulputate semper purus.Cras dapibus laoreet ex, sit amet euismod nisi pretium ac.Cras a imperdiet nibh.Proin pulvinar, orci quis dapibus placerat, justo nulla fermentum ante, eleifend accumsan elit nisl ac neque.Sed cursus venenatis mauris, in malesuada lacus consectetur sit amet.Morbi finibus, odio egestas tincidunt malesuada, dolor nibh tempor leo, sed euismod justo leo dapibus velit.Maecenas nunc tortor, consectetur laoreet lacus cursus, venenatis sodales risus. Nam commodo odio id magna sagittis, a lobortis eros maximus.Donec hendrerit sem ac enim efficitur lobortis.Etiam auctor tincidunt maximus.Aenean facilisis dolor malesuada, fringilla nunc vitae, ultrices felis.Fusce ac libero sit amet mauris sodales molestie vitae a nisl.Proin feugiat odio sit amet nulla sollicitudin, eget eleifend ipsum luctus.Phasellus efficitur lectus non volutpat tincidunt.Curabitur lacinia eros sagittis tristique vulputate.Nunc volutpat lorem vitae nibh luctus, non efficitur neque interdum.Fusce neque nulla, hendrerit in lectus ac, luctus viverra enim.Maecenas blandit, ex at hendrerit convallis, lacus lacus commodo diam, sit amet imperdiet quam erat id ligula.Donec malesuada in elit vel ullamcorper.Aenean magna nunc, posuere ullamcorper aliquam eu, feugiat ut tellus.Pellentesque rutrum blandit dolor id fermentum. Quisque at sagittis orci.Ut quis mauris nec est placerat luctus nec non neque.Donec tristique sollicitudin massa nec fermentum.Suspendisse at hendrerit justo.Curabitur vel diam sagittis, vehicula augue sed, iaculis augue.In est diam, finibus et elit sit amet, placerat porttitor orci.Nunc viverra ex sed est dapibus, quis pellentesque neque tempus.Donec quam tellus, suscipit vel accumsan ultrices, consequat nec elit. Nulla facilisi.Nunc commodo est metus.Duis semper vitae metus a tincidunt.Nulla sed felis sed diam dapibus eleifend.Aenean sit amet finibus enim.Aenean non hendrerit mi.Donec fermentum semper lacus id suscipit.Duis nec finibus odio, vestibulum volutpat magna.Morbi in tempus quam. Mauris ac sodales metus, nec sollicitudin metus.Quisque accumsan sem lorem, id placerat ante egestas sed.Duis blandit fermentum ligula pulvinar dignissim.Curabitur porttitor posuere velit, sit amet lobortis mi.Nam eleifend auctor molestie.Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Quisque non augue libero.Mauris mattis aliquam cursus.Aenean vitae sapien id lectus sodales placerat.Sed porta lacinia diam nec volutpat.Quisque felis lacus, tempus at massa ac, dignissim scelerisque magna.In tincidunt interdum est et bibendum.Fusce tincidunt et eros a blandit. Integer vel nisl ut nisi elementum varius.Etiam porttitor, metus nec ultricies fermentum, libero nisl luctus enim, eu iaculis enim lorem sollicitudin augue.Cras iaculis a velit eget bibendum.Curabitur urna lacus, malesuada quis auctor at, gravida ut dolor.Aenean consectetur elit risus, blandit ornare orci mollis ut.Nulla faucibus scelerisque orci, sit amet viverra nisi convallis quis.Sed blandit ut enim quis dapibus.Curabitur et ligula quis turpis gravida pharetra nec a sem.Nunc a viverra massa.Etiam mollis eu purus quis sollicitudin.Duis a mattis tellus.Curabitur lorem turpis, congue at feugiat sagittis, auctor at lacus.Sed volutpat purus quis purus auctor suscipit.");
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
    }
}