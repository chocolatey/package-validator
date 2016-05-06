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
    using System.Collections.Generic;
    using chocolatey.package.validator.infrastructure.app.rules;
    using chocolatey.package.validator.infrastructure.rules;
    using Moq;
    using NuGet;
    using Should;

    public abstract class InstallScriptsShouldntUseCreateShortcutNoteSpecsBase : TinySpec
    {
        protected InstallScriptsShouldntUseCreateShortcutNote note;
        protected Mock<IPackage> package = new Mock<IPackage>();
        protected Mock<IPackageFile> packageFile = new Mock<IPackageFile>();

        public override void Context()
        {
            note = new InstallScriptsShouldntUseCreateShortcutNote();
        }

        public class when_inspecting_package_with_installation_script_with_createshortcut : InstallScriptsShouldntUseCreateShortcutNoteSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns(@"$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut('$Home/Desktop/ColorPix.lnk')
$Shortcut.TargetPath = 'C:/Program Files (x86)/ColorPix/ColorPix.exe'
$Shortcut.Save()".to_stream());
                packageFile.Setup(f => f.Path).Returns("test.ps1");

                package.Setup(p => p.GetFiles()).Returns(new List<IPackageFile>() { packageFile.Object });
            }

            public override void Because()
            {
                result = this.note.is_valid(package.Object);
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

        public class when_inspecting_package_with_installation_script_without_createshortcut : InstallScriptsShouldntUseCreateShortcutNoteSpecsBase
        {
            private PackageValidationOutput result;

            public override void Context()
            {
                base.Context();

                packageFile.Setup(f => f.GetStream()).Returns("Write-Output Test".to_stream());
                packageFile.Setup(f => f.Path).Returns("test.ps1");

                package.Setup(p => p.GetFiles()).Returns(new List<IPackageFile>() { packageFile.Object });
            }

            public override void Because()
            {
                result = this.note.is_valid(package.Object);
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
