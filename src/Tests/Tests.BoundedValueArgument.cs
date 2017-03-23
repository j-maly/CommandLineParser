using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
#pragma warning disable CS0649
        class BoundedValueArgumentParsingTarget
        {
            [BoundedValueArgument(typeof(int), 'd', "duration", Description = "The duration of the acoustic data used in the adaptation.", 
                ValueOptional = true, MinValue = 0, DefaultValue = 0)]
            public int Duration { get; set; }
        }
#pragma warning restore CS0649

        BoundedValueArgumentParsingTarget boundedValueArgumentTarget;

        public CommandLineParser.CommandLineParser InitBoundedValueArgument()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            boundedValueArgumentTarget = new BoundedValueArgumentParsingTarget();
            commandLineParser.ExtractArgumentAttributes(boundedValueArgumentTarget);
            return commandLineParser;
        }

        [Fact]
        public void BoundedValueArgument_shouldRecognizeBoundedValueArgumentAttribute()
        {
            // ARRANGE 
            string[] args = { "-d", "50" };
            var commandLineParser = InitBoundedValueArgument();
            // ACT 
            commandLineParser.ParseCommandLine(args);
            // ASSERT
            Assert.Equal(50, boundedValueArgumentTarget.Duration);
        }
    }
}