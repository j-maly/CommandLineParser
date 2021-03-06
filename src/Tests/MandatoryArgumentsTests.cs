using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace Tests
{
    public class MandatoryArgumentsTests
    {
        class TestTargetWithOptionalTrue
        {
            [ValueArgument(typeof(string), 's', Optional = true)]
            public string Severity { get; set; }
        }

        class TestTargetWithOptionalFalse
        {
            [ValueArgument(typeof(string), 's', Optional = false)]
            public string Severity { get; set; }
        }

        [Fact]
        public void ParseCommandLine_WithCheckMandatoryArgumentsTrue_OptionalArgumentTrue_and_SuppliedArgument_Works()
        {
            // Assign
            string[] args = { "-s", "test" };

            var commandLineParser = new CommandLineParser.CommandLineParser();
            var target = new TestTargetWithOptionalTrue();
            commandLineParser.ExtractArgumentAttributes(target);

            // Act
            commandLineParser.ParseCommandLine(args);

            // Assert
            target.Severity.Should().Be("test");
        }

        [Fact]
        public void ParseCommandLine_WithCheckMandatoryArgumentsTrue_OptionalArgumentFalse_and_SuppliedArgument_Works()
        {
            // Assign
            string[] args = { "-s", "test" };

            var commandLineParser = new CommandLineParser.CommandLineParser();
            var target = new TestTargetWithOptionalFalse();
            commandLineParser.ExtractArgumentAttributes(target);

            // Act
            commandLineParser.ParseCommandLine(args);

            // Assert
            target.Severity.Should().Be("test");
        }

        [Fact]
        public void ParseCommandLine_WithCheckMandatoryArgumentsTrue_OptionalArgumentTrue_and_MissingArgument_Works()
        {
            // Assign
            string[] args = { };

            var commandLineParser = new CommandLineParser.CommandLineParser();
            var target = new TestTargetWithOptionalTrue();
            commandLineParser.ExtractArgumentAttributes(target);

            // Act
            commandLineParser.ParseCommandLine(args);

            // Assert
            target.Severity.Should().BeNull();
        }

        [Fact]
        public void ParseCommandLine_WithCheckMandatoryArgumentsTrue_OptionalArgumentTrue_and_MissingArgument_ThrowsException()
        {
            // Assign
            string[] args = { };

            var commandLineParser = new CommandLineParser.CommandLineParser();
            var target = new TestTargetWithOptionalFalse();
            commandLineParser.ExtractArgumentAttributes(target);

            // Act
            Action act = () => commandLineParser.ParseCommandLine(args);

            // Assert
            act.Should().Throw<MandatoryArgumentNotSetException>();
        }

        [Fact]
        public void ParseCommandLine_WithCheckMandatoryArgumentsFalse_OptionalArgumentTrue_and_MissingArgument_Works()
        {
            // Assign
            string[] args = { };

            var commandLineParser = new CommandLineParser.CommandLineParser { CheckMandatoryArguments = false };

            var target = new TestTargetWithOptionalFalse();
            commandLineParser.ExtractArgumentAttributes(target);

            // Act
            commandLineParser.ParseCommandLine(args);

            // Assert
            target.Severity.Should().BeNull();
        }
    }
}