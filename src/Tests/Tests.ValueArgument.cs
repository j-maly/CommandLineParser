using CommandLineParser.Arguments;
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
            string[] args = new[] { "-i", "1", "-i", "2", "-i", "3" };
            var commandLineParser = InitValueArgument();
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(new List<int> { 1, 2, 3 }, valueArgumentTarget.Numbers);
        }
    }
}