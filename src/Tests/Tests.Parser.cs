using System;
using CommandLineParser.Arguments;
using Xunit;

namespace Tests;

public partial class Tests
{
    [Fact]
    public void ParserResult_ShouldBeFalse_WhenOnlyHelpWasShown()
    {
        var commandLineParser = InitDeclarativeArguments();
        commandLineParser.ShowUsageOnEmptyCommandline = true;

        string[] args = Array.Empty<string>();
        commandLineParser.ParseCommandLine(args);

        Assert.False(commandLineParser.ParsingSucceeded);
    }

    [Fact]
    public void ParserResult_ShouldBeTrue_WhenParsingSucceeds()
    {
        var commandLineParser = InitDeclarativeArguments();
        commandLineParser.ShowUsageOnEmptyCommandline = true;

        string[] args = { "-v", "1" };
        commandLineParser.ParseCommandLine(args);

        Assert.True(commandLineParser.ParsingSucceeded);
    }

    [Fact]
    public void Parser_shouldAllowArgsWithTheSameToUpperConversion_whenIgnoreCaseIsNotUsed()
    {
        // ARRANGE 
        var parser = new CommandLineParser.CommandLineParser();
        parser.Arguments.Add(new SwitchArgument('a', "switch", false));
        parser.Arguments.Add(new SwitchArgument('b', "SWiTCH", false));
        parser.IgnoreCase = false;

        // ACT 
        parser.ParseCommandLine(Array.Empty<string>());
    }

    [Fact]
    public void Parser_shouldFailWithArgsWithTheSameToUpperConversion_whenIgnoreCaseIsUsed()
    {
        // ARRANGE 
        var parser = new CommandLineParser.CommandLineParser();
        parser.Arguments.Add(new SwitchArgument('a', "switch", false));
        parser.Arguments.Add(new SwitchArgument('b', "SWiTCH", false));
        parser.IgnoreCase = true;

        // ACT 
        Assert.Throws<ArgumentException>(() => parser.ParseCommandLine(Array.Empty<string>()));
    }

    [Fact]
    public void Parser_shouldAcceptSwitch_WithoutShortName()
    {
        // ARRANGE 
        var parser = new CommandLineParser.CommandLineParser();
        var switchArgument = new SwitchArgument("switch", false);
        parser.Arguments.Add(switchArgument);

        // ACT 
        string[] args = { "--switch" };
        parser.ParseCommandLine(args);

        Assert.True(parser.ParsingSucceeded);
        Assert.True(switchArgument.Value);
    }
}