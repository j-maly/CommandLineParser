using System;
using System.Collections.Generic;
using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using CommandLineParser.Validation;

namespace ParserTest
{
#pragma warning disable CS0649
    class ParsingTarget
    {
        [SwitchArgument('s', "show", true, Description = "Set whether show or not")]
        public bool show;

        private bool hide;
        [SwitchArgument('h', "hide", false, Description = "Set whether hide or not")]
        public bool Hide
        {
            get => hide;
            set => hide = value;
        }

        [ValueArgument(typeof(decimal), 'v', "version", Description = "Set desired version")]
        public decimal version;

        [ValueArgument(typeof(string), 'l', "level", Description = "Set the level")]
        public string level;

        [ValueArgument(typeof(Point), 'p', "point", Description = "Specify the point", Example = "[0,1]")]
        public Point point;

        [ValueArgument(typeof(int?), 'n', "nullableint", Description = "Nullable Integer", Optional = true)]
        public int? NullableInt;

        [BoundedValueArgument(typeof(int), 'o', "optimization", MinValue = 0, MaxValue = 3, Description = "Level of optimization")]
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
#pragma warning restore CS0649

    public class Program
    {
        public static void Main(string[] args)
        {
            var parser = new CommandLineParser.CommandLineParser();
            parser.ShowUsageOnEmptyCommandline = true;

            var p = new ParsingTarget();
            // read the argument attributes 
            parser.ShowUsageHeader = "This is an interesting command";
            parser.ShowUsageFooter = "And that is all";
            parser.ExtractArgumentAttributes(p);
            parser.Certifications.Add(new DistinctGroupsCertification("d", "color") { Description = "This is this" });

            var examples = new List<string[]>();
            examples.Add(new string[0]); //No arguments passed
            examples.Add(new[] { "/help" }); //show usage 
            examples.Add(new[] { "/version", "1.3" }); //parses OK 
            examples.Add(new[] { "/nullableint", "42" }); //parses OK
            examples.Add(new[] { "/color", "red", "/version", "1.2" }); //parses OK 
            examples.Add(new[] { "/point", "[1;3]", "/o", "2" }); //parses OK 
            examples.Add(new[] { "/d", "C:\\Input", "/i", "in.txt", "/x", "out.txt" }); //parses OK 
            examples.Add(new[] { "/show", "/hide" }); //parses OK 
            examples.Add(new[] { "/d" }); // error, missing value
            examples.Add(new[] { "/d", "C:\\Input" });
            examples.Add(new[] { "file1", "file2" });

            foreach (string[] arguments in examples)
            {
                try
                {
                    if (arguments.Length == 0)
                        Console.WriteLine("INPUT: No arguments supplied.");
                    else
                        Console.WriteLine("INPUT: {0}", arguments);

                    parser.ParseCommandLine(arguments);

                    parser.ShowParsedArguments(true);
                    Console.WriteLine("RESULT: OK");
                    Console.WriteLine();
                }
                catch (CommandLineException e)
                {
                    Console.WriteLine("RESULT: EXC - " + e.Message);
                    Console.WriteLine();
                }
            }

            /* 
			 * Example: requiring additional arguments of certain type (1 file in this case)
			 */
            FileArgument additionalFileArgument1 = new FileArgument('_');
            additionalFileArgument1.FileMustExist = false;
            additionalFileArgument1.Optional = false;

            FileArgument additionalFileArgument2 = new FileArgument('_');
            additionalFileArgument2.FileMustExist = false;
            additionalFileArgument2.Optional = false;

            parser.AdditionalArgumentsSettings.TypedAdditionalArguments.Clear();
            parser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument1);
            parser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument2);
            try
            {
                // this fails, because there is only one file
                parser.ParseCommandLine(new[] { "/d", "C:\\Input", "file1.txt" });
                parser.ShowParsedArguments();
            }
            catch (CommandLineException e)
            {
                Console.WriteLine("RESULT: EXC - " + e.Message);
                Console.WriteLine();
            }

            // two files - OK 
            parser.ParseCommandLine(new[] { "/d", "C:\\Input", "file1.txt", "file2.txt" });
            parser.ShowParsedArguments();
            Console.WriteLine("RESULT: OK");
            Console.WriteLine();
        }
    }
}