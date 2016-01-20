using CommandLineParser.Arguments;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
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

        [Fact]
        public void DeclarativeSwitch_SwitchArgumentTrueDefault()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.True(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeSwitch_SwitchArgumentTrueSet()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t" });

            Assert.False(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeSwitch_SwitchArgumentFalseDefault()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.False(options.DefaultFalse);
        }

        [Fact]
        public void DeclarativeSwitch_SwitchArgumentFalseSet()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f" });

            Assert.True(options.DefaultFalse);
        }
    }
}