#define autoargs 
//uncomment autoargs defined, to see a declarative way of using the library 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandLineParser;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace ParserTest
{
	class Program
	{
		// with autoargs directive set, the arguments are defined declaratively using attributes
#if autoargs
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

			[ValueArgument(typeof(Point), 'p', "point", Description = "specify the point", Example = "[0,1]")]
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
#endif

		static void Main(string[] args)
		{
			CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();
            parser.ShowUsageOnEmptyCommandline = true;

#if autoargs
#else
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


        	parser.Arguments.Add(showArgument);
        	parser.Arguments.Add(hideArgument);
        	parser.Arguments.Add(level);
        	parser.Arguments.Add(version);
        	parser.Arguments.Add(point);
        	parser.Arguments.Add(optimization);
        	parser.Arguments.Add(color);
        	parser.Arguments.Add(inputFile);
        	parser.Arguments.Add(outputFile);
			parser.Arguments.Add(inputDirectory);
        	parser.ShowUsage();
#endif


#if autoargs
            ParsingTarget p = new ParsingTarget();
			// read the argument attributes 
		    parser.ShowUsageHeader = "This is an interesting command";
		    parser.ShowUsageFooter = "And that is all";
			parser.ExtractArgumentAttributes(p);
#endif
			List<string[]> examples = new List<string[]>();
            examples.Add(new string[0]); //No arguments passed
			examples.Add(new[] { "/help" }); //show usage 
			examples.Add(new[] { "/version", "1.3" }); //parses OK 
			examples.Add(new[] { "/color", "red", "/version", "1.2" }); //parses OK 
			examples.Add(new[] { "/point", "[1;3]", "/o", "2" }); //parses OK 
			examples.Add(new[] { "/d", "C:\\Input", "/i", "in.txt", "/x", "out.txt" }); //parses OK 
			examples.Add(new[] { "/show", "/hide" }); //parses OK 
			examples.Add(new[] { "/d" }); // error, missing value
			examples.Add(new[] { "/d", "C:\\Input" });

			foreach (string[] arguments in examples)
			{
				try
				{
                    if (arguments.Length == 0)
                        Console.WriteLine("INPUT: No arguments supplied.");
                    else
					    Console.WriteLine("INPUT: {0}", arguments);

					parser.ParseCommandLine(arguments);

					parser.ShowParsedArguments();
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
			 * Example: requiring additional arguments of 
			 * certain type (1 file in this case)
			 * 
			 */
			FileArgument additionalFileArgument1 = new FileArgument('_');
			additionalFileArgument1.FileMustExist = false;
			FileArgument additionalFileArgument2 = new FileArgument('_');
			additionalFileArgument2.FileMustExist = false;
			parser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument1);
			parser.AdditionalArgumentsSettings.TypedAdditionalArguments.Add(additionalFileArgument2);
			try
			{
				// this fails, because there is only one file
				parser.ParseCommandLine(new[] { "-d", "C:\\Input", "file1.txt" });
				parser.ShowParsedArguments();
			}
			catch (CommandLineException e)
			{
				Console.WriteLine("RESULT: EXC - " + e.Message);
				Console.WriteLine();
			}
			// two files - OK 
			parser.ParseCommandLine(new[] { "-d", "C:\\Input", "file1.txt", "file2.txt" });
			parser.ShowParsedArguments();
			Console.WriteLine("RESULT: OK");
			Console.WriteLine();

		}
	}
}