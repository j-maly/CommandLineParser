using CommandLineParser.Arguments;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        public class DeclarativeValueTestOptionsFalse
        {
            private bool defaultFalse;

            [ValueArgument(typeof(bool), 'f', DefaultValue = false)]
            public bool DefaultFalse
            {
                get => defaultFalse;

                set => defaultFalse = value;
            }
        }

        public class DeclarativeValueTestOptionsTrue
        {
            private bool defaultTrue;

            [ValueArgument(typeof(bool), 't', ForcedDefaultValue = true)]
            public bool DefaultTrue
            {
                get => defaultTrue;
                set => defaultTrue = value;
            }
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentTrueDefault()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new DeclarativeValueTestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.True(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentTrueSetTrue()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new DeclarativeValueTestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t", "true" });

            Assert.True(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentTrueSetFalse()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new DeclarativeValueTestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t", "false" });

            Assert.False(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentFalseDefault()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new DeclarativeValueTestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.False(options.DefaultFalse);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentFalseSetFalse()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new DeclarativeValueTestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f", "false" });

            Assert.False(options.DefaultFalse);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentFalseSetTrue()
        {
            var parser = new CommandLineParser.CommandLineParser();
            var options = new DeclarativeValueTestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f", "true" });

            Assert.True(options.DefaultFalse);
        }
    }
}