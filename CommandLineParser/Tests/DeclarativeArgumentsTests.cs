using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DeclarativeArgumentsTests
    {
        public CommandLineParser.CommandLineParser CommandLineParser;

        class ParsingTarget
        {
            [SwitchArgument('s', "show", true, Description = "Set whether show or not")]
            public bool show;

            private bool hide;
            [SwitchArgument('h', "hide", false, Description = "Set whether hide or not")]
            public bool Hide
            {
                get { return hide; }
                set { hide = value; }
            }

            [ValueArgument(typeof(decimal), 'v', "version", Description = "Set desired version")]
            public decimal version;

            [ValueArgument(typeof(string), 'l', "level", Description = "Set the level")]
            public string level;

            [ValueArgument(typeof(Point), 'p', "point", Description = "specify the point")]
            public Point point;

            [BoundedValueArgument(typeof(int), 'o', "optimization",
                MinValue = 0, MaxValue = 3, Description = "Level of optimization")]
            public int optimization;

            [EnumeratedValueArgument(typeof(string), 'c', "color", AllowedValues = "red;green;blue")]
            public string color;

            [FileArgument('i', "input", Description = "Input file", FileMustExist = false)]
            public FileInfo inputFile;

            [FileArgument('x', "output", Description = "Output file", FileMustExist = false)]
            public FileInfo outputFile;

            [DirectoryArgument('d', "directory", Description = "Input directory", DirectoryMustExist = false)]
            public DirectoryInfo InputDirectory;
        }

        [TestFixtureSetUp]
        public void Init()
        {
            CommandLineParser = new CommandLineParser.CommandLineParser();
            ParsingTarget p = new ParsingTarget();
            // read the argument attributes 
            CommandLineParser.ExtractArgumentAttributes(p);
        }

        [Test]
        public void Ex1()
        {
            string[] args = new[] { "--version", "1.3" };
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex2()
        {
            string[] args = new[] { "--color", "red", "--version", "1.2" };
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex3()
        {
            string[] args = new[] { "--point", "[1;3]", "-o", "2" };
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex4()
        {
            string[] args = (new[] { "-d", "C:\\Input", "-i", "in.txt", "-x", "out.txt" });
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex5()
        {
            string[] args = new[] { "--show", "--hide" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        public void Ex6()
        {
            string[] args = new[] { "-d", "C:\\Input" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(CommandLineArgumentException), MatchType = MessageMatch.Contains, ExpectedMessage = "must be followed by a value")]
        public void MissingValueTest()
        {
            string[] args = new[] { "-d" };
            CommandLineParser.ParseCommandLine(args);
        }
    }
}