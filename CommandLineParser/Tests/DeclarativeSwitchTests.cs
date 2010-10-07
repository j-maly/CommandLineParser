using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class DeclarativeSwitchTests
    {
        [Test]
        public void SwitchArgumentTrueDefault()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.IsTrue(options.DefaultTrue);
        }

        [Test]
        public void SwitchArgumentTrueSet()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t"});

            Assert.IsFalse(options.DefaultTrue);
        }

        [Test]
        public void SwitchArgumentFalseDefault()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.IsFalse(options.DefaultFalse);
        }

        [Test]
        public void SwitchArgumentFalseSet()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f" });

            Assert.IsTrue(options.DefaultFalse);
        }

        public class TestOptionsFalse
        {
            private bool defaultFalse;

            [SwitchArgument('f', false)]
            public bool DefaultFalse
            {
                get { return defaultFalse; }

                set
                {
                    defaultFalse = value;
                }
            }
        }

        public class TestOptionsTrue
        {
            private bool defaultTrue;

            [SwitchArgument('t', true)]
            public bool DefaultTrue
            {
                get { return defaultTrue; }
                set
                {
                    defaultTrue = value;
                }
            }
        }  
    }
}