using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using ParserTest;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        private CommandLineParser.CommandLineParser InitIgnoreCase()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            commandLineParser.IgnoreCase = true;
            commandLineParser.ShowUsageOnEmptyCommandline = true;

            SwitchArgument showArgument = new SwitchArgument('s', "show", "Set whether show or not", true);

            SwitchArgument hideArgument = new SwitchArgument('h', "hide", "Set whether hid or not", false);

            ValueArgument<string> level = new ValueArgument<string>('l', "level", "Set the level");

            ValueArgument<decimal> version = new ValueArgument<decimal>('v', "version", "Set desired version");

            ValueArgument<Point> point = new ValueArgument<Point>('p', "point", "specify the point");

            BoundedValueArgument<int> optimization = new BoundedValueArgument<int>('o', "optimization", 0, 3);

            EnumeratedValueArgument<string> color = new EnumeratedValueArgument<string>('c', "color", new[] { "red", "green", "blue" });

            FileArgument inputFile = new FileArgument('i', "input", "Input file");
            inputFile.FileMustExist = false;
            FileArgument outputFile = new FileArgument('x', "output", "Output file");
            outputFile.FileMustExist = false;

            DirectoryArgument inputDirectory = new DirectoryArgument('d', "directory", "Input directory");
            inputDirectory.DirectoryMustExist = false;

            point.ConvertValueHandler = delegate (string stringValue)
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

                throw new CommandLineArgumentException("Bad point format", "point");
            };

            commandLineParser.Arguments.Add(showArgument);
            commandLineParser.Arguments.Add(hideArgument);
            commandLineParser.Arguments.Add(level);
            commandLineParser.Arguments.Add(version);
            commandLineParser.Arguments.Add(point);
            commandLineParser.Arguments.Add(optimization);
            commandLineParser.Arguments.Add(color);
            commandLineParser.Arguments.Add(inputFile);
            commandLineParser.Arguments.Add(outputFile);
            commandLineParser.Arguments.Add(inputDirectory);

            return commandLineParser;
        }

        [Fact]
        public void IgnoreCase_1()
        {
            string[] args = new[] { "--VERSION", "1.3" };

            var commandLineParser = InitIgnoreCase();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void IgnoreCase_2()
        {
            string[] args = new[] { "--Color", "red", "--veRsion", "1.2" };

            var commandLineParser = InitIgnoreCase();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void IgnoreCase_3()
        {
            string[] args = new[] { "--Point", "[1;3]", "-o", "2" };

            var commandLineParser = InitIgnoreCase();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void IgnoreCase_4()
        {
            string[] args = (new[] { "-D", "C:\\Input", "-I", "in.txt", "-x", "out.txt" });

            var commandLineParser = InitIgnoreCase();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void IgnoreCase_5()
        {
            string[] args = new[] { "--ShOw", "--HiDe" };

            var commandLineParser = InitIgnoreCase();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void IgnoreCase_6()
        {
            string[] args = new[] { "-D", "C:\\Input" };

            var commandLineParser = InitIgnoreCase();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]        
        public void IgnoreCase_MissingValueTest()
        {
            string[] args = new[] { "-d" };

            var commandLineParser = InitIgnoreCase();

            var ex = Assert.Throws<CommandLineArgumentException>(() => commandLineParser.ParseCommandLine(args));
            Assert.Contains("must be followed by a value", ex.Message);
        }
    }
}