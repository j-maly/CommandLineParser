using System;
using System.Collections.Generic;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Tests;

public partial class Tests
{
    internal class CertifiedValueArgumentParsingTarget
    {
        [BoundedValueArgument(typeof(int), 'i', MaxValue = 10, UseMaxValue = true, AllowMultiple = true)]
        public List<int> Numbers = new List<int>();

        [EnumeratedValueArgument(typeof(string), 's', AllowedValues = "Error,Warning", IgnoreCase = true)]
        public string Severity { get; set; }
    }

    private CertifiedValueArgumentParsingTarget _certifiedValueArgumentParsingTarget = null!;

    public CommandLineParser.CommandLineParser InitForCertifiedValueArgument()
    {
        var factory = LoggerFactory.Create(b => b.AddConsole());
        ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
        var commandLineParser = new CommandLineParser.CommandLineParser(logger);
        _certifiedValueArgumentParsingTarget = new CertifiedValueArgumentParsingTarget();
        commandLineParser.ExtractArgumentAttributes(_certifiedValueArgumentParsingTarget);

        return commandLineParser;
    }

    [Fact]
    public void MultipleCertifiedValueTest()
    {
        string[] args = { "-i", "1", "-i", "2", "-i", "3" };

        var commandLineParser = InitForCertifiedValueArgument();
        commandLineParser.ParseCommandLine(args);
    }

    [Fact]
    public void EnumeratedValueArgument_shouldAcceptCorrectValue()
    {
        string[] args = { "-s", "Error" };

        var commandLineParser = InitForCertifiedValueArgument();
        commandLineParser.ParseCommandLine(args);

        _certifiedValueArgumentParsingTarget.Severity.Should().Be("Error");
    }

    [Fact]
    public void EnumeratedValueArgument_shouldAcceptValueWithDifferencesInCapitalization_whenIgnoreCaseIsOn()
    {
        string[] args = { "-s", "ERROR" };

        var commandLineParser = InitForCertifiedValueArgument();
        commandLineParser.ParseCommandLine(args);

        _certifiedValueArgumentParsingTarget.Severity.Should().Be("Error");
    }

    [Fact]
    public void EnumeratedValueArgument_shouldNotAcceptUnknownValue()
    {
        string[] args = { "-s", "Info" };

        var commandLineParser = InitForCertifiedValueArgument();
        var e = Assert.Throws<CommandLineArgumentOutOfRangeException>(() => commandLineParser.ParseCommandLine(args));

        Assert.Equal("Value Info is not allowed for argument s", e.Message);
    }

    [Fact]
    public void EnumeratedValueArgument_shouldNotAllowIgnoreCaseForNonStrings()
    {
        EnumeratedValueArgument<int> arg = new EnumeratedValueArgument<int>('t');
        var e = Assert.Throws<ArgumentException>(() => arg.IgnoreCase = true);

        Assert.Equal("Ignore case can be used only for string arguments, type of TValue is System.Int32", e.Message);
    }

    [Fact]
    public void ValueArgument_ShouldParseDate()
    {
        var factory = LoggerFactory.Create(b => b.AddConsole());
        ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
        var commandLineParser = new CommandLineParser.CommandLineParser(logger);
        ValueArgument<DateTime> dateTime = new ValueArgument<DateTime>('d', "date");
        commandLineParser.Arguments.Add(dateTime);

        commandLineParser.ParseCommandLine(new[] { "-d", "2008-11-01" });

        Assert.Equal(new DateTime(2008, 11, 01), dateTime.Value);
    }
}