using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using FluentAssertions;
using Xunit;

namespace Tests;

public partial class Tests
{
    private class RegexValueArgumentParsingTarget
    {
        [RegexValueArgument('c', "\\d[A-Z]\\d", Optional = false)]
        public string Code { get; set; } = null!;
    }

    private (CommandLineParser.CommandLineParser Parser, RegexValueArgumentParsingTarget ParsingTarget) InitForRegexValueArgument()
    {
        var commandLineParser = new CommandLineParser.CommandLineParser();
        var regexValueArgumentParsingTarget = new RegexValueArgumentParsingTarget();
        commandLineParser.ExtractArgumentAttributes(regexValueArgumentParsingTarget);

        return (commandLineParser, regexValueArgumentParsingTarget);
    }

    [Fact]
    public void RegexMatchTest()
    {
        // Arrange
        string[] args = { "-c", "1X2" };
        var (commandLineParser, parsingTarget) = InitForRegexValueArgument();
        
        // Act
        commandLineParser.ParseCommandLine(args);

        // Assert
        parsingTarget.Code.Should().Be("1X2");
    }

    [Fact]
    public void RegexMismatchTest()
    {
        // Arrange
        string[] args = { "-c", "XXX" };
        var (commandLineParser, _) = InitForRegexValueArgument();

        // Act and Assert
        Assert.Throws<CommandLineArgumentOutOfRangeException>(() => commandLineParser.ParseCommandLine(args));
    }
}