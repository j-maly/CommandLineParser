using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class DeclarativeValueTests
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
        public void SwitchArgumentTrueSetTrue()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t", "true"});

            Assert.IsTrue(options.DefaultTrue);
        }

        [Test]
        public void SwitchArgumentTrueSetFalse()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t", "false" });

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
        public void SwitchArgumentFalseSetFalse()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f" , "false" });

            Assert.IsFalse(options.DefaultFalse);
        }

        [Test]
        public void SwitchArgumentFalseSetTrue()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f", "true" });

            Assert.IsTrue(options.DefaultFalse);
        }

        public class TestOptionsFalse
        {
            private bool defaultFalse;

            [ValueArgument(typeof(bool), 'f', DefaultValue = false)]
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

            [ValueArgument(typeof(bool), 't', DefaultValue = true)]
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