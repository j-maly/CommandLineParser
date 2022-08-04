# CommandLineParser

[![Build status](https://ci.appveyor.com/api/projects/status/gcg703cp9l4sf3fm/branch/master?svg=true)](https://ci.appveyor.com/project/j-maly/commandlineparser/branch/master)

[![NuGet Badge](https://buildstats.info/nuget/CommandLineArgumentsParser)](https://www.nuget.org/packages/CommandLineArgumentsParser)

CommandLine Parser Library lets you easily define strongly typed command line arguments, allows automatic parsing of command line arguments and mapping the values to properites of your objects.

See [Quick Start](https://github.com/j-maly/CommandLineParser/wiki) on how to use the library. 

Supports the following Frameworks:
* NET 2.0
* NET 3.5
* NET 4.0
* NET 4.5
* NET 4.5.2 and higher
* NETStandard 1.3 and higher
* NETStandard 2.0

## Motivational Example

Although console applications are more common in UNIX environment, maybe you will one day need to write one for windows too. It is common for command line applications to accept arguments in a time-tested format that can look like this: 
``` cmd
Finder.exe -s 3 --distinct directory1 directory2 directory3
```
You probably get it already - arguments can be short or long, short arguments consist of '-' prefix and a single character, long arguments consist of '--' prefix and a single word. 

It may also be handy to support aliases for arguments (long and short name for one argument or even more long or short names for an argument). 

For application with one or two arguments, you could probably manage with some switches and ifs, but when there are more and more arguments, you could use a CommandLine Parser library and thus make your code more clean and elegant.

This is the way you define arguments for your application:
```csharp
CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();
//switch argument is meant for true/false logic
SwitchArgument showArgument = new SwitchArgument('s', "show", "Set whether show or not", true);
ValueArgument<decimal> version = new ValueArgument<decimal>('v', "version", "Set desired version");
EnumeratedValueArgument<string> color = new EnumeratedValueArgument<string>('c', "color", new string[] { "red", "green", "blue" });

parser.Arguments.Add(showArgument);
parser.Arguments.Add(version);
parser.Arguments.Add(color);
```
And this is how the arguments are parsed:
```csharp
try 
{
    parser.ParseCommandLine(args); 
    parser.ShowParsedArguments();
 
    // now you can work with the arguments ... 

    // if (color.Parsed) ... test, whether the argument appeared on the command line
    // {
    //     color.Value ... contains value of the level argument
    // } 
    // if (showArgument.Value) ... test the switch argument value 
    //     ... 
}
catch (CommandLineException e)
{
    Console.WriteLine(e.Message);
}
```
You can find more examples of use in the [wiki](https://github.com/j-maly/CommandLineParser/wiki).

The other way to use the library is to declare arguments by using attributes and thus make your code even more elegant:
```csharp
// fields of this class will be bound
class ParsingTarget
{
    //class has several fields and properties bound to various argument types

    [SwitchArgument('s', "show", true, Description = "Set whether show or not")]
    public bool show;

    private bool hide;
    [SwitchArgument('h', "hide", false, Description = "Set whether hid or not")]
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

    [EnumeratedValueArgument(typeof(string),'c', "color", AllowedValues = "red;green;blue")]
    public string color;
}
```
As Andreas Kroll pointed out - it can be useful to define set of arguments that cannot be used together or set of arguments, from which at least one argument must be used. This is now possible through Certifications collection of the parser and ArgumentCertification objects. They can also be defined declaratively using attributes, here is an example:

```csharp
// exactly one of the arguments x, o, c must be used
[ArgumentGroupCertification("x,o,c", EArgumentGroupCondition.ExactlyOneUsed)]
// only one of the arguments f, u must be used
[ArgumentGroupCertification("f,u", EArgumentGroupCondition.OneOrNoneUsed)]
// arguments j and k can not be used together with arguments l or m
[DistinctGroupsCertification("j,k","l,m")]
public class Archiver
{
    [ValueArgument(typeof(string), 'f', "file", Description="Input from file")]
    public string InputFromFile;

    [ValueArgument(typeof(string), 'u', "url", Description = "Input from url")]
    public string InputFromUrl;

    [ValueArgument(typeof(string), 'c', "create", Description = "Create archive")]
    public string CreateArchive;

    [ValueArgument(typeof(string), 'x', "extract", Description = "Extract archive")]
    public string ExtractArchive;

    [ValueArgument(typeof(string), 'o', "open", Description = "Open archive")]
    public string OpenArchive;

    [SwitchArgument('j', "g1a1", true)]
    public bool group1Arg1;

    [SwitchArgument('k', "g1a2", true)]
    public bool group1Arg2;

    [SwitchArgument('l', "g2a1", true)]
    public bool group2Arg1;

    [SwitchArgument('m', "g2a2", true)]
    public bool group2Arg2;
}
```
