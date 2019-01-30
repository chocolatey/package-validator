# Chocolatey Package Validator

Validates the contents of a package against the package review process parts that can be validated by a machine. See https://github.com/chocolatey/choco/wiki/Moderation for more details.

The validator is a service that checks the quality of a package based on requirements, guidelines and suggestions for creating packages for Chocolatey’s community feed. We like to think of the validator as unit testing. It is validating that everything is as it should be and meets the minimum requirements for a package on the community feed.

What does the validator check? See https://github.com/chocolatey/package-validator/wiki

![Chocolatey Logo](https://github.com/chocolatey/chocolatey/raw/master/docs/logo/chocolateyicon.gif "Chocolatey")

## Chat Room

Want quick feedback to your questions? [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/chocolatey/choco?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

### Requirements
* .NET Framework 4.0

### License / Credits
Apache 2.0 - see [LICENSE](https://github.com/chocolatey/package-validator/blob/master/LICENSE) and [NOTICE](https://github.com/chocolatey/package-validator/blob/master/NOTICE) files.

## Contributing

If you would like to contribute code or help squash a bug or two, that's awesome. Please familiarize yourself with [CONTRIBUTING](https://github.com/chocolatey/package-validator/blob/master/CONTRIBUTING.md).

## Committers

Committers, you should be very familiar with [COMMITTERS](https://github.com/chocolatey/package-validator/blob/master/COMMITTERS.md).

### Compiling / Building Source

There is a `build.bat`/`build.sh` file that creates a necessary generated file named `SolutionVersion.cs`. It must be run at least once before Visual Studio will build.

#### Windows

Prerequisites:

 * .NET Framework 4.5+
 * Visual Studio is helpful for working on source.
 * ReSharper is immensely helpful (and there is a `.sln.DotSettings` file to help with code conventions).

Build Process:

 * Run `build.bat`.

Running the build on Windows should produce an artifact that is tested and ready to be used.


## Setup

You need the following installed on a machine that you will use the validator with:

* .NET Framework 4.5.
* Install the service and let it run.
