using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class AdditionalArgumentsTests
    {
        public CommandLineParser.CommandLineParser CommandLineParser;

        [TestFixtureSetUp]
        public void Init()
        {
            CommandLineParser = new CommandLineParser.CommandLineParser();
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
                'c', "color", new[] {"red", "green", "blue"});

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
            CommandLineParser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument1);
            CommandLineParser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument2);
        }

        [Test]
        [ExpectedException(typeof(MissingAdditionalArgumentsException), ExpectedMessage = "Needed 2 additional arguments", MatchType = MessageMatch.Contains)]
        public void Ex1()
        {
            string[] args = new[] { "-d", "C:\\Input", "file1.txt" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        public void Ex2()
        {
            string[] args = new[] { "-d", "C:\\Input", "file1.txt", "file2.txt" };
            CommandLineParser.ParseCommandLine(args);
        }
        
    }
}