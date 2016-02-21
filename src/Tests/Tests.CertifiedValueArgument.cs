using CommandLineParser.Arguments;
using System.Collections.Generic;
using CommandLineParser.Exceptions;
using Xunit;
using System;

namespace Tests
{
    public partial class Tests
    {
        class CertifiedValueArgumentParsingTarget
        {
            [BoundedValueArgument(typeof(int), 'i', MaxValue = 10, UseMaxValue = true, AllowMultiple = true)]
            public List<int> Numbers = new List<int>();

            [EnumeratedValueArgument(typeof(string), 's', AllowedValues = "Error,Warning", IgnoreCase = true)]
            public string Severity { get; set; }
        }

        private CertifiedValueArgumentParsingTarget target;

        public CommandLineParser.CommandLineParser InitForCertifiedValueArgument()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            target = new CertifiedValueArgumentParsingTarget();
            commandLineParser.ExtractArgumentAttributes(target);

            return commandLineParser;
        }

        [Fact]
        public void MultpleCertifiedValueTest()
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

            Assert.Equal(target.Severity, "Error");
        }

        [Fact]
        public void EnumeratedValueArgument_shouldAcceptValueWithDifferencesInCapitalisation_whenIgnoreCaseIsOn()
        {
            string[] args = { "-s", "ERROR" };

            var commandLineParser = InitForCertifiedValueArgument();
            commandLineParser.ParseCommandLine(args);

            Assert.Equal(target.Severity, "Error");
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
            EnumeratedValueArgument<int> arg= new EnumeratedValueArgument<int>('t');
            var e = Assert.Throws<ArgumentException>(() => arg.IgnoreCase = true );

            Assert.Equal("Ignore case can be used only for string arguments, type of TValue is System.Int32", e.Message);
        }
        
        [Fact]
        public void ValueArgument_ShouldParseDate()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            ValueArgument<DateTime> dateTime = new ValueArgument<DateTime>('d', "date");
            commandLineParser.Arguments.Add(dateTime);

            commandLineParser.ParseCommandLine(new [] {"-d", "2008-11-01" });

            Assert.Equal(new DateTime(2008,11,01), dateTime.Value);
        }
        
    }
}