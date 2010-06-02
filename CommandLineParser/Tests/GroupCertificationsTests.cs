using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using CommandLineParser.Validation;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GroupCertificationsTests
    {
        public CommandLineParser.CommandLineParser CommandLineParser;

        public class Archiver
        {
            [ValueArgument(typeof(string), 'f', "file", Description = "Input from file")]
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

        [TestFixtureSetUp]
        public void Init()
        {
            CommandLineParser = new CommandLineParser.CommandLineParser();
            
            Archiver a = new Archiver();

            CommandLineParser.ExtractArgumentAttributes(a);
        }

        #region exactly one used

        [Test]
        public void ExactlyOneUsed1()
        {
            // exactly one of the arguments x, o, c must be used
            ArgumentGroupCertification eou = new ArgumentGroupCertification("x,o,c", 
                EArgumentGroupCondition.ExactlyOneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(eou);


            string[] args = new[] { "-x", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-o", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-c", "file" };
            CommandLineParser.ParseCommandLine(args);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "Only one of these arguments", MatchType = MessageMatch.Contains)]
        public void ExactlyOneUsed2()
        {
            // exactly one of the arguments x, o, c must be used
            ArgumentGroupCertification eou = new ArgumentGroupCertification("x,o,c",
                EArgumentGroupCondition.ExactlyOneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(eou);


            string[] args = new[] { "-c", "file", "-x", "file2" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "One of these arguments", MatchType = MessageMatch.Contains)]
        public void ExactlyOneUsed3()
        {
            // exactly one of the arguments x, o, c must be used
            ArgumentGroupCertification eou = new ArgumentGroupCertification("x,o,c",
                EArgumentGroupCondition.ExactlyOneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(eou);


            string[] args = new[] { "-j" };
            CommandLineParser.ParseCommandLine(args);
        }

        #endregion

        #region one or none used

        [Test]
        public void OneOrNone1()
        {
            ArgumentGroupCertification oon = new ArgumentGroupCertification("f,u",
                EArgumentGroupCondition.OneOreNoneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(oon);


            string[] args = new[] { "-f", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-u", "file" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        public void OneOrNone2()
        {
            // exactly one of the arguments x, o, c must be used
            ArgumentGroupCertification oon = new ArgumentGroupCertification("f,u",
                EArgumentGroupCondition.OneOreNoneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(oon);

            string[] args = new[] { "-x", "file" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage="These arguments can not be used together", MatchType = MessageMatch.StartsWith)]
        public void OneOrNone3()
        {
            // exactly one of the arguments x, o, c must be used
            ArgumentGroupCertification oon = new ArgumentGroupCertification("f,u",
                EArgumentGroupCondition.OneOreNoneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(oon);

            string[] args = new[] { "-f", "file", "-u", "file2" };
            CommandLineParser.ParseCommandLine(args);
        }


        #endregion 

        #region at least one used
        
        [Test]
        public void AtLeastOne1()
        {
            ArgumentGroupCertification oon = new ArgumentGroupCertification("f,u",
                EArgumentGroupCondition.AtLeastOneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(oon);


            string[] args = new[] { "-f", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-u", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-u", "file", "-f", "file" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "At least one of these", MatchType = MessageMatch.StartsWith)]
        public void AtLeastOne2()
        {
            ArgumentGroupCertification oon = new ArgumentGroupCertification("f,u",
                EArgumentGroupCondition.AtLeastOneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(oon);


            string[] args = new[] { "-x", "file" };
            CommandLineParser.ParseCommandLine(args);
        }

        #endregion 

        #region all or none used

        [Test]
        public void AllOrNone1()
        {
            ArgumentGroupCertification aon = new ArgumentGroupCertification("j,k",
                EArgumentGroupCondition.AllOrNoneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(aon);


            string[] args = new[] { "-f", "file" };
            CommandLineParser.ParseCommandLine(args);


            args = new[] { "-j","-k" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "All or none of these", MatchType = MessageMatch.StartsWith)]
        public void AllOrNone2()
        {
            ArgumentGroupCertification aon = new ArgumentGroupCertification("j,k",
                EArgumentGroupCondition.AllOrNoneUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(aon);


            string[] args = new[] { "-j", "file" };
            CommandLineParser.ParseCommandLine(args);
        }

        #endregion 

        #region all used 

        [Test]
        public void All1()
        {
            ArgumentGroupCertification au = new ArgumentGroupCertification("j,k",
                EArgumentGroupCondition.AllUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(au);


            string[] args = new[] { "-j", "-k" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "All of these", MatchType = MessageMatch.StartsWith)]
        public void All2()
        {
            ArgumentGroupCertification au = new ArgumentGroupCertification("j,k",
                EArgumentGroupCondition.AllUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(au);


            string[] args = new[] { "-j", "file" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "All of these", MatchType = MessageMatch.StartsWith)]
        public void All3()
        {
            ArgumentGroupCertification au = new ArgumentGroupCertification("j,k",
                EArgumentGroupCondition.AllUsed);
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(au);


            string[] args = new[] { "-x", "file" };
            CommandLineParser.ParseCommandLine(args);
        }

        #endregion 

        #region distinct groups

        [Test]
        public void Distinct1()
        {
            DistinctGroupsCertification d = new DistinctGroupsCertification("j,k", "l,m");
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(d);


            string[] args = new[] { "-j", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-k", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-l", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-m", "file" };
            CommandLineParser.ParseCommandLine(args);

            args = new[] { "-l", "-m" };

            CommandLineParser.ParseCommandLine(args);
            args = new[] { "-j", "-k" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "None of these", MatchType = MessageMatch.StartsWith)]
        public void Distinct2()
        {
            DistinctGroupsCertification d = new DistinctGroupsCertification("j,k", "l,m");
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(d);
            
            string[] args = new[] { "-j", "-l" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "None of these", MatchType = MessageMatch.StartsWith)]
        public void Distinct3()
        {
            DistinctGroupsCertification d = new DistinctGroupsCertification("j,k", "l,m");
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(d);

            string[] args = new[] { "-j", "-k", "-m" };
            CommandLineParser.ParseCommandLine(args);
        }

        [Test]
        [ExpectedException(typeof(ArgumentConflictException), ExpectedMessage = "None of these", MatchType = MessageMatch.StartsWith)]
        public void Distinct4()
        {
            DistinctGroupsCertification d = new DistinctGroupsCertification("j,k", "l,m");
            CommandLineParser.Certifications.Clear();
            CommandLineParser.Certifications.Add(d);

            string[] args = new[] { "-m", "-j" };
            CommandLineParser.ParseCommandLine(args);
        }

        #endregion
        //// only one of the arguments f, u must be used
        //[ArgumentGroupCertification("f,u", EArgumentGroupCondition.OneOreNoneUsed)]
        //// arguments j and k can not be used together with arguments l or m
        //[DistinctGroupsCertification("j,k", "l,m")]
    }
}