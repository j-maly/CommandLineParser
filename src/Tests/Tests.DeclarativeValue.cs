using CommandLineParser.Arguments;
using Microsoft.Extensions.Logging;
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
                get { return defaultFalse; }

                set
                {
                    defaultFalse = value;
                }
            }
        }

        public class DeclarativeValueTestOptionsTrue
        {
            private bool defaultTrue;

            [ValueArgument(typeof(bool), 't', ForcedDefaultValue = true)]
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
        public void DeclarativeValue_SwitchArgumentTrueDefault()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new DeclarativeValueTestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.True(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentTrueSetTrue()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new DeclarativeValueTestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t", "true" });

            Assert.True(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentTrueSetFalse()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new DeclarativeValueTestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t", "false" });

            Assert.False(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentFalseDefault()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new DeclarativeValueTestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.False(options.DefaultFalse);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentFalseSetFalse()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new DeclarativeValueTestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f", "false" });

            Assert.False(options.DefaultFalse);
        }

        [Fact]
        public void DeclarativeValue_SwitchArgumentFalseSetTrue()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new DeclarativeValueTestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f", "true" });

            Assert.True(options.DefaultFalse);
        }
    }
}