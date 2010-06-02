using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class CertifiedValueArgumentTests
    {
        public CommandLineParser.CommandLineParser CommandLineParser;
        ParsingTarget target;

        class ParsingTarget
        {
            [BoundedValueArgument(typeof(int), 'i', MaxValue = 10, UseMaxValue = true, AllowMultiple = true)]
            public List<int> Numbers = new List<int>();
        }

        [TestFixtureSetUp]
        public void Init()
        {
            CommandLineParser = new CommandLineParser.CommandLineParser();
            target = new ParsingTarget();
            CommandLineParser.ExtractArgumentAttributes(target);
        }

        [Test]
        public void MultpleCertifiedValueTest()
        {
            string[] args = new[] { "-i", "1", "-i", "2", "-i", "3" };
            CommandLineParser.ParseCommandLine(args);
        }
    }
}