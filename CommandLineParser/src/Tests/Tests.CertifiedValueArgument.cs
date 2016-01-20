using CommandLineParser.Arguments;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        class CertifiedValueArgumentParsingTarget
        {
            [BoundedValueArgument(typeof(int), 'i', MaxValue = 10, UseMaxValue = true, AllowMultiple = true)]
            public List<int> Numbers = new List<int>();
        }

        public CommandLineParser.CommandLineParser InitCertifiedValueArgument()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            var target = new CertifiedValueArgumentParsingTarget();
            commandLineParser.ExtractArgumentAttributes(target);

            return commandLineParser;
        }

        [Fact]
        public void MultpleCertifiedValueTest()
        {
            string[] args = new[] { "-i", "1", "-i", "2", "-i", "3" };

            var commandLineParser = InitCertifiedValueArgument();
            commandLineParser.ParseCommandLine(args);
        }
    }
}