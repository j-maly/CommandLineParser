using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using ParserTest;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        public CommandLineParser.CommandLineParser InitAdditionalArguments()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
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
                    string[] parts = stringValue.Substring(1, stringValue.Length - 2).Split(';', ',');
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

            /* 
			 * Example: requiring additional arguments of 
			 * certain type (1 file in this case)
			 * 
			 */
            FileArgument additionalFileArgument1 = new FileArgument('_');
            additionalFileArgument1.FileMustExist = false;
            additionalFileArgument1.Optional = false;
            FileArgument additionalFileArgument2 = new FileArgument('_');
            additionalFileArgument2.FileMustExist = false;
            additionalFileArgument2.Optional = false;

            commandLineParser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument1);
            commandLineParser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument2);

            return commandLineParser;
        }

        [Fact]
        public void AdditionalArguments_MissingAdditionalArgumentsException()
        {
            string[] args = new[] { "-d", "C:\\Input", "file1.txt" };

            var commandLineParser = InitAdditionalArguments();

            var ex = Assert.Throws<MissingAdditionalArgumentsException>(() => commandLineParser.ParseCommandLine(args));
            Assert.Equal("Not enough additional arguments. Needed 2 additional arguments.", ex.Message);
        }

        [Fact]
        public void AdditionalArguments_Test1()
        {
            string[] args = new[] { "-d", "C:\\Input", "file1.txt", "file2.txt" };

            var commandLineParser = InitAdditionalArguments();
            commandLineParser.ParseCommandLine(args);
        }
    }
}