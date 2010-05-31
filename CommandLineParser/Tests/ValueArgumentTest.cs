using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ValueArgumentTest
    {
        public CommandLineParser.CommandLineParser CommandLineParser;
        ParsingTarget target; 

        class ParsingTarget
        {
            [ValueArgument(typeof(int), 'i', AllowMultiple = true)]
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
        public void MultipleValuesTest()
        {
            string[] args = new[] { "-i", "1", "-i", "2", "-i", "3" };
            CommandLineParser.ParseCommandLine(args);
        }
    }
}