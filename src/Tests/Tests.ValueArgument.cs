using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        class ValueArgumentParsingTarget
        {
            [ValueArgument(typeof(int), 'i', AllowMultiple = true)]
            public List<int> Numbers;

            [ValueArgument(typeof(int), 'v', DefaultValue = 2, ValueOptional = true)]
            public int Version;
            
            [ValueArgument(typeof(int), 'l', DefaultValue = 0)]
            public int Length;
        }

        ValueArgumentParsingTarget valueArgumentTarget;

        public CommandLineParser.CommandLineParser InitValueArgument()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();            
            valueArgumentTarget = new ValueArgumentParsingTarget();
            commandLineParser.ExtractArgumentAttributes(valueArgumentTarget);
            return commandLineParser;
        }

        [Fact]
        public void ArgumentBoundToCollection_shouldCreateInstanceOfTheCollection()
        {
            // ARRANGE 
            string[] args = { "-i", "1", "-i", "2", "-i", "3" };
            var commandLineParser = InitValueArgument();
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(new List<int> { 1, 2, 3 }, valueArgumentTarget.Numbers);
        }

        [Fact]
        public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenValueNotUsed_valueArgumentIsInTheMiddle()
        {
            // ARRANGE 
            string[] args = { "-v", "-i", "1" };
            var commandLineParser = InitValueArgument();
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
        }

        [Fact]
        public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenValueNotUsed_valueArgumentIsAtTheEnd()
        {
            // ARRANGE 
            string[] args = { "-i", "1", "-v" };
            var commandLineParser = InitValueArgument();
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
        }

        [Fact]
        public void ValueArgumentWithMandatoryValue_shouldFailParsing_whenValueNotFound_lastArgument()
        {
            // ARRANGE 
            string[] args = { "-l" };
            var commandLineParser = InitValueArgument();
            // ACT 
            var e = Assert.Throws<CommandLineArgumentException>(() => commandLineParser.ParseCommandLine(args));            
            Assert.Equal(e.Message, "Value argument l must be followed by a value."); 
        }

        [Fact]
        public void ValueArgumentWithMandatoryValue_shouldFailParsing_whenValueNotFound_argumentInTheMiddle()
        {
            // ARRANGE 
            string[] args = { "-l", "-v", "1" };
            var commandLineParser = InitValueArgument();
            // ACT 
            var e = Assert.Throws<CommandLineArgumentException>(() => commandLineParser.ParseCommandLine(args));
            Assert.Equal(e.Message, "Value argument l must be followed by a value, another argument(-v) found instead");
        }


        [Fact]
        public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_andValueNotUsed_whenUsingEqualsSyntax_andValueArgumentIsInTheMiddle()
        {
            // ARRANGE 
            string[] args = { "-v", "-i=\"1\"" };
            var commandLineParser = InitValueArgument();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
        }

        [Fact]
        public void ValueArgumentWithOptionalValue_shouldReturnDefaultValue_whenValueNotUsed_whenUsingEqualsSyntax_andValueArgumentIsAtTheEnd()
        {
            // ARRANGE 
            string[] args = { "-i=\"1\"", "-v" };
            var commandLineParser = InitValueArgument();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(2 /*default*/ , valueArgumentTarget.Version);
        }

        [Fact]
        public void ValueArgument_shouldHandleNegativeIntegers_whenUsingEqualsSyntax()
        {
            // ARRANGE 
            string[] args = { "-v=\"-1\""};
            var commandLineParser = InitValueArgument();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(-1, valueArgumentTarget.Version);
        }

        [Fact]
        public void ValueArgument_shouldHandleNegativeIntegers_whenNotUsingEqualsSyntax()
        {
            // ARRANGE 
            string[] args = { "-v", "-1" };
            var commandLineParser = InitValueArgument();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = false;
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(-1, valueArgumentTarget.Version);
        }
    }
}