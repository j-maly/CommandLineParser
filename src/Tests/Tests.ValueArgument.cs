using System.Collections.Generic;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using Xunit;

namespace Tests;

public partial class Tests
{
#pragma warning disable CS0649
    enum OptionsFlag
    {
        O1, O2, O3
    }

    class ValueArgumentParsingTarget
    {
        [ValueArgument(typeof(int), 'i', AllowMultiple = true)]
        public List<int> Numbers;

        [ValueArgument(typeof(int), 'v', DefaultValue = 2, ForcedDefaultValue = 1, ValueOptional = true)]
        public int Version;

        [ValueArgument(typeof(int?), 'n', Optional = true)]
        public int? NullableInt;

        [ValueArgument(typeof(bool?), 'b', Optional = true)]
        public bool? NullableBool;

        [ValueArgument(typeof(int), 'l', DefaultValue = 0)]
        public int Length;

        [ValueArgument(typeof(int), 'd', DefaultValue = 9, ValueOptional = true)]
        public int Duration;

        [ValueArgument(typeof(OptionsFlag), 'f', "flag", DefaultValue = OptionsFlag.O3)]
        public OptionsFlag Flag;
    }
#pragma warning restore CS0649

    private (CommandLineParser.CommandLineParser Parser, ValueArgumentParsingTarget ParsingTarget) InitValueArgument()
    {
        var commandLineParser = new CommandLineParser.CommandLineParser();
        var valueArgumentTarget = new ValueArgumentParsingTarget();
        commandLineParser.ExtractArgumentAttributes(valueArgumentTarget);
        return (commandLineParser, valueArgumentTarget);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnDefaultEnumValue_whenParameterNotPassed_andForcedDefaultValueNotSet()
    {
        // Arrange
        string[] args = { " " };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();

        // Act
        commandLineParser.ParseCommandLine(args);

        // Assert
        Assert.Equal(OptionsFlag.O3, valueArgumentTarget.Flag);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenParameterNotPassed_andForcedDefaultValueNotSet()
    {
        // Arrange
        string[] args = { " " };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(9, valueArgumentTarget.Duration);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenParameterPassedWithoutValue_andForcedDefaultValueNotSet()
    {
        // Arrange
        string[] args = { "-d" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(9, valueArgumentTarget.Duration);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnValue_whenParameterPassedWithValue_andForcedDefaultValueNotSet()
    {
        // Arrange
        string[] args = { "-d", "7" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(7, valueArgumentTarget.Duration);
    }

    [Fact]
    public void ArgumentBoundToCollection_shouldCreateInstanceOfTheCollection()
    {
        // Arrange
        string[] args = { "-i", "1", "-i", "2", "-i", "3" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(new List<int> { 1, 2, 3 }, valueArgumentTarget.Numbers);
    }

    [Fact]
    public void ValueArgumentWithOptionalNullableValue_shouldReturnDefaultValue_whenValueNotSpecified()
    {
        // Arrange
        string[] args = { };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();

        // Act
        commandLineParser.ParseCommandLine(args);

        // Assert
        Assert.Null(valueArgumentTarget.NullableInt);
        Assert.Null(valueArgumentTarget.NullableBool);
    }

    [Fact]
    public void ValueArgumentWithOptionalNullableValue_shouldReturnCorrectValue_whenValueIsSpecified()
    {
        // Arrange
        string[] args = { "-n", "42", "-b", "true" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(42, valueArgumentTarget.NullableInt);
        Assert.Equal(true, valueArgumentTarget.NullableBool);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenValueNotUsed_valueArgumentIsInTheMiddle()
    {
        // Arrange
        string[] args = { "-v", "-i", "1" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenValueNotUsed_valueArgumentIsAtTheEnd()
    {
        // Arrange
        string[] args = { "-i", "1", "-v" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
    }

    [Fact]
    public void ValueArgumentWithMandatoryValue_shouldFailParsing_whenValueNotFound_lastArgument()
    {
        // Arrange
        string[] args = { "-l" };
        var (commandLineParser, _) = InitValueArgument();
        // Act
        var e = Assert.Throws<CommandLineArgumentException>(() => commandLineParser.ParseCommandLine(args));
        Assert.Equal("Value argument l must be followed by a value.", e.Message);
    }

    [Fact]
    public void ValueArgumentWithMandatoryValue_shouldFailParsing_whenValueNotFound_argumentInTheMiddle()
    {
        // Arrange
        string[] args = { "-l", "-v", "1" };
        var (commandLineParser, _) = InitValueArgument();
        // Act
        var e = Assert.Throws<CommandLineArgumentException>(() => commandLineParser.ParseCommandLine(args));
        Assert.Equal("Value argument l must be followed by a value, another argument(-v) found instead", e.Message);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_andValueNotUsed_whenUsingEqualsSyntax_andValueArgumentIsInTheMiddle()
    {
        // Arrange
        string[] args = { "-v", "-i=\"1\"" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnStrongDefaultValue_andValueNotUsed_whenUsingEqualsSyntax_andValueArgumentDoesNotExist()
    {
        // Arrange
        string[] args = { "-i=\"1\"" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(1 /*strong default*/ , valueArgumentTarget.Version);
    }

    [Fact]
    public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenValueNotUsed_whenUsingEqualsSyntax_andValueArgumentIsAtTheEnd()
    {
        // Arrange
        string[] args = { "-i=\"1\"", "-v" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
    }

    [Fact]
    public void ValueArgument_shouldHandleNegativeIntegers_whenUsingEqualsSyntax()
    {
        // Arrange
        string[] args = { "-v=\"-1\"" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(-1, valueArgumentTarget.Version);
    }

    [Fact]
    public void ValueArgument_shouldHandleNegativeIntegers_whenNotUsingEqualsSyntax()
    {
        // Arrange
        string[] args = { "-v", "-1" };
        var (commandLineParser, valueArgumentTarget) = InitValueArgument();
        commandLineParser.AcceptEqualSignSyntaxForValueArguments = false;
        // Act
        commandLineParser.ParseCommandLine(args);
        // Assert
        Assert.Equal(-1, valueArgumentTarget.Version);
    }
}