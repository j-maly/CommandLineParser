using CommandLineParser.Arguments;
using System.Collections.Generic;
using CommandLineParser.Exceptions;
using Xunit;
using System;

namespace Tests
{
    public partial class Tests
    {
        class RegexValueArgumentParsingTarget
        {
            [RegexValueArgument('c', "\\d[A-Z]\\d")]
            public string Code { get; set; }
        }

        private RegexValueArgumentParsingTarget regexTarget;

        public CommandLineParser.CommandLineParser InitForRegexValueArgument()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            regexTarget = new RegexValueArgumentParsingTarget();
            commandLineParser.ExtractArgumentAttributes(regexTarget);

            return commandLineParser;
        }

        [Fact]
        public void RegexMatchTest()
        {
            string[] args = { "-c", "1X2" };

            var commandLineParser = InitForRegexValueArgument();
            commandLineParser.ParseCommandLine(args);

            Assert.Equal("1X2", regexTarget.Code);
        }

        [Fact]
        public void RegexMismatchTest()
        {
            string[] args = { "-c", "XXX" };

            var commandLineParser = InitForRegexValueArgument();

            Assert.Throws<CommandLineArgumentOutOfRangeException>(() => commandLineParser.ParseCommandLine(args));
        }
    }
}