using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ValueArgumentTests
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

        public enum MyEnum
        {
            One,
            Two
        }

        [Test]
        public void EnumTest()
        {
            ValueArgument<MyEnum> enumArg = new ValueArgument<MyEnum>('e');
            CommandLineParser.Arguments.Add(enumArg);
            string[] args = new[] { "-e", "One" };
            CommandLineParser.ParseCommandLine(args);
        }
    }
}