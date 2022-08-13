using CommandLineParser.Arguments;
using Microsoft.Extensions.Logging;
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

            [ValueArgument(typeof(string), 'p')]
            public string Path { get; set; }
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
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.True(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeSwitch_SwitchArgumentTrueSet()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new TestOptionsTrue();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-t" });

            Assert.False(options.DefaultTrue);
        }

        [Fact]
        public void DeclarativeSwitch_SwitchArgumentFalseDefault()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { });

            Assert.False(options.DefaultFalse);
        }

        [Fact]
        public void DeclarativeSwitch_SwitchArgumentFalseSet()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-f" });

            Assert.True(options.DefaultFalse);
        }

        [Fact]
        public void PathAsAnArgumentShouldNotBreakThings()
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            ILogger<CommandLineParser.CommandLineParser> logger = factory.CreateLogger<CommandLineParser.CommandLineParser>();
            var parser = new CommandLineParser.CommandLineParser(logger);
            var options = new TestOptionsFalse();
            parser.ExtractArgumentAttributes(options);

            parser.ParseCommandLine(new string[] { "-p", "/root/folder" });

            Assert.Equal("/root/folder", options.Path);
        }
    }
}