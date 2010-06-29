using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class IgnoreCaseTests
    {
        public CommandLineParser.CommandLineParser CommandLineParser;

        [TestFixtureSetUp]
        public void Init()
        {
            CommandLineParser = new CommandLineParser.CommandLineParser();
            CommandLineParser.IgnoreCase = true;
            CommandLineParser.ShowUsageOnEmptyCommandline = true;

            SwitchArgument showArgument = new SwitchArgument(
                's', "show", "Set whether show or not", true);

            SwitchArgument hideArgument = new SwitchArgument(
                'h', "hide", "Set whether hid or not", false);

            ValueArgument<string> level = new ValueArgument<string>(
                'l', "level", "Set the level");

            ValueArgument<decimal> version = new ValueArgument<decimal>(
                'v', "version", "Set desired version");

            ValueArgument<Point> point = new ValueArgument<Point>(
                'p', "point", "specify the point");

            BoundedValueArgument<int> optimization = new BoundedValueArgument<int>(
                'o', "optimization", 0, 3);

            EnumeratedValueArgument<string> color = new EnumeratedValueArgument<string>(
                'c', "color", new string[] {"red", "green", "blue"});

            FileArgument inputFile = new FileArgument('i', "input", "Input file");
            inputFile.FileMustExist = false;
            FileArgument outputFile = new FileArgument('x', "output", "Output file");
            outputFile.FileMustExist = false;

            DirectoryArgument inputDirectory = new DirectoryArgument('d', "directory", "Input directory");
            inputDirectory.DirectoryMustExist = false;

            point.ConvertValueHandler = delegate(string stringValue)
                                            {
                                                if (stringValue.StartsWith("[") && stringValue.EndsWith("]"))
                                                {
                                                    string[] parts =
                                                        stringValue.Substring(1, stringValue.Length - 2).Split(';', ',');
                                                    Point p = new Point();
                                                    p.x = int.Parse(parts[0]);
                                                    p.y = int.Parse(parts[1]);
                                                    return p;
                                                }
                                                else
                                                    throw new CommandLineArgumentException("Bad point format", "point");
                                            };


            CommandLineParser.Arguments.Add(showArgument);
            CommandLineParser.Arguments.Add(hideArgument);
            CommandLineParser.Arguments.Add(level);
            CommandLineParser.Arguments.Add(version);
            CommandLineParser.Arguments.Add(point);
            CommandLineParser.Arguments.Add(optimization);
            CommandLineParser.Arguments.Add(color);
            CommandLineParser.Arguments.Add(inputFile);
            CommandLineParser.Arguments.Add(outputFile);
            CommandLineParser.Arguments.Add(inputDirectory);
        }

        [Test]
        public void Ex1()
        {
            string[] args = new[] { "--VERSION", "1.3" };
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex2()
        {
            string[] args = new[] {"--Color", "red", "--veRsion", "1.2"};
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex3()
        {
            string[] args = new[] {"--Point", "[1;3]", "-o", "2"};
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex4()
        {
            string[] args = (new[] { "-D", "C:\\Input", "-I", "in.txt", "-x", "out.txt" });
            CommandLineParser.ParseCommandLine(args);
        }
        [Test]
        public void Ex5()
        {
            string[] args = new[] { "--ShOw", "--HiDe" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        public void Ex6()
        {
            string[] args = new[] { "-D", "C:\\Input" };
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