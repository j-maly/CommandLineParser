using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using ParserTest;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        [Fact]
        public void ParserResult_ShouldBeFalse_WhenOnlyHelpWasShown()
        {
            var commandLineParser = InitDeclarativeArguments();
            commandLineParser.ShowUsageOnEmptyCommandline = true;

            string[] args = new string[0];
            commandLineParser.ParseCommandLine(args);
            
            Assert.Equal(false, commandLineParser.ParsingSucceeded);
        }

        [Fact]
        public void ParserResult_ShouldBeTrue_WhenParsingSucceeds()
        {
            var commandLineParser = InitDeclarativeArguments();
            commandLineParser.ShowUsageOnEmptyCommandline = true;

            string[] args = new[] { "-v", "1" };
            commandLineParser.ParseCommandLine(args);

            Assert.Equal(true, commandLineParser.ParsingSucceeded);
        }
    }
}