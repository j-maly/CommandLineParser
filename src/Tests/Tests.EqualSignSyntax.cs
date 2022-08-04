using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using ParserTest;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        private CommandLineParser.CommandLineParser InitEqualSignSyntax()
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
            commandLineParser.ShowUsageOnEmptyCommandline = true;

            SwitchArgument showArgument = new SwitchArgument('s', "show", "Set whether show or not", true);

            SwitchArgument hideArgument = new SwitchArgument('h', "hide", "Set whether hid or not", false);

            ValueArgument<string> level = new ValueArgument<string>('l', "level", "Set the level");

            ValueArgument<decimal> version = new ValueArgument<decimal>('v', "version", "Set desired version");

            ValueArgument<Point> point = new ValueArgument<Point>('p', "point", "specify the point");

            BoundedValueArgument<int> optimization = new BoundedValueArgument<int>('o', "optimization", 0, 3);

            EnumeratedValueArgument<string> color = new EnumeratedValueArgument<string>('c', "color", new string[] { "red", "green", "blue" });
            
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
                    Point p = new Point
                    {
                        X = int.Parse(parts[0]),
                        Y = int.Parse(parts[1])
                    };
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
        public void EqualSignSyntaxEx1()
        {
            string[] args = new[] { "--version=\"1.3\"" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void EqualSignSyntaxEx1_withoutDoubleQuotes()
        {
            string[] args = new[] { "--version=1.3" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void EqualSignSyntaxEx2()
        {
            string[] args = new[] { "--color=\"red\"", "--version=\"1.3\"" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void EqualSignSyntaxEx2_withoutDoubleQuotes()
        {
            string[] args = new[] { "--color=red", "--version=1.3" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void EqualSignSyntaxEx3()
        {
            string[] args = new[] { "--point=\"[1;3]\"", "-o=\"2\"" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void EqualSignSyntaxEx4()
        {
            string[] args = (new[] { "-d=\"C:\\Input\"", "-i=\"in.txt\"", "-x=\"out.txt\"" });

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void EqualSignSyntaxEx5()
        {
            string[] args = new[] { "--show", "--hide" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]
        public void EqualSignSyntaxEx6()
        {
            string[] args = new[] { "-d=\"C:\\Input\"" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }

        [Fact]        
        public void EqualSignSyntax_ShouldThroughCommandLineArgumentException_WhenValueIsMissing()
        {
            string[] args = new[] { "-d" };

            var commandLineParser = InitEqualSignSyntax();

            var ex = Assert.Throws<CommandLineArgumentException>(() => commandLineParser.ParseCommandLine(args));
            Assert.Contains("must be followed by a value", ex.Message);
        }

        [Fact]
        public void EqualSignSyntax_ShouldAcceptCommaSeparatedValues()
        {
            string[] args = new[] { "-n=\"1,2,3\"", "x" };

            var commandLineParser = new CommandLineParser.CommandLineParser();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
            ValueArgument<int> lines = new ValueArgument<int>('n', "lines", "Specific lines") { AllowMultiple = true };
            commandLineParser.Arguments.Add(lines);

            // ACT 
            commandLineParser.ParseCommandLine(args);

            // ASSERT
            Assert.Equal(new List<int> { 1, 2, 3 }, lines.Values);
            Assert.Equal(1, commandLineParser.AdditionalArgumentsSettings.AdditionalArguments.Length);
            Assert.Equal("x", commandLineParser.AdditionalArgumentsSettings.AdditionalArguments[0]);
        }

        [Fact]
        public void EqualSignSyntax_ShouldAcceptCommaSeparatedValues_WithoutQuotes()
        {
            string[] args = new[] { "-n=1,2,3", "-n=4" };

            var commandLineParser = new CommandLineParser.CommandLineParser();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
            ValueArgument<int> lines = new ValueArgument<int>('n', "lines", "Specific lines") { AllowMultiple = true };
            commandLineParser.Arguments.Add(lines);

            // ACT 
            commandLineParser.ParseCommandLine(args);

            // ASSERT
            Assert.Equal(new List<int> { 1, 2, 3, 4 }, lines.Values);
        }

        [Fact]
        public void EqualSignSyntax_ShouldAcceptEmptyListOfValues_whenValueOptional()
        {
            string[] args = new[] { "-n=", "-n=" };

            var commandLineParser = new CommandLineParser.CommandLineParser();
            commandLineParser.AcceptEqualSignSyntaxForValueArguments = true;
            ValueArgument<int> lines = new ValueArgument<int>('n', "lines", "Specific lines") { AllowMultiple = true, ValueOptional=true };
            commandLineParser.Arguments.Add(lines);

            // ACT 
            commandLineParser.ParseCommandLine(args);

            // ASSERT
            Assert.Equal(new List<int>(), lines.Values);
        }
         
        [Fact]
        public void EqualSignSyntax_ShouldSplitFromFirstEqualSignFound()
        {
            string[] args = new[] { "--level=ABCDEFGHI=+FTRWEQASD" };

            var commandLineParser = InitEqualSignSyntax();
            commandLineParser.ParseCommandLine(args);
        }
    }
}