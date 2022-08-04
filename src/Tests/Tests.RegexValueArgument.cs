using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using FluentAssertions;
using Xunit;

namespace Tests;

public partial class Tests
{
    private class RegexValueArgumentParsingTarget
    {
        [RegexValueArgument('s', "\\d[A-Z]\\d", Optional = false)]
        public string Sample { get; set; } = null!;

        [RegexValueArgument(longName: "countdown"
            , shortName: 'c'
            , pattern: @"(?i:(?:^(?<SecondsOnly>\d{1,3})$)|(?:^(?<Min>\d{1,2})[:.](?<Sec>\d{1,2})$)|(?:(?<Min>\d{1,3})\s*(?:m(?:(?:(?:in)?(?:ute)?(?:s)?)?)))|(?:(?<Sec>\d{1,3})\s*(?:s(?:(?:(?:ec)?(?:ond)?(?:s)?)?))))"
            , Aliases = new[] { "t", "timeout", }
            , AllowMultiple = false
            , Description = "Time for executing selected command (range between 0 … 15 minutes)"
            , Example = "/c:00:00:10 | /c:12:34"
            , Optional = true
            , ValueOptional = false
            , SampleValue = "XXXXXXXXXXXXXXXXXXXXX"
        )]
        public string? TimeOut { get; set; }
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
        string[] args = { "-s", "1X2", "-c", "123" };
        var (commandLineParser, parsingTarget) = InitForRegexValueArgument();
        
        // Act
        commandLineParser.ParseCommandLine(args);

        // Assert
        parsingTarget.Sample.Should().Be("1X2");
        parsingTarget.TimeOut.Should().Be("123");
    }

    [Fact]
    public void RegexMismatchTest()
    {
        // Arrange
        string[] args = { "-s", "XXX" };
        var (commandLineParser, _) = InitForRegexValueArgument();

        // Act and Assert
        Assert.Throws<CommandLineArgumentOutOfRangeException>(() => commandLineParser.ParseCommandLine(args));
    }
}