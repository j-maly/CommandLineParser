using System;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Validation
{
    /// <summary>
    /// Allows to define which two groups of arguments that can not be used together.
    /// </summary>
    /// <include file='Doc\CommandLineParser.xml' path='CommandLineParser/Certifications/Certification/*'/>
    public class DistinctGroupsCertification : ArgumentCertification
    {
        private Argument[] argumentGroup1;

        private Argument[] argumentGroup2;

        private string argumentGroupString1;

        private string argumentGroupString2;

        /// <summary>
        /// Creates new instance of DistinctGroupsCertification - this certification specifies two groups of arguments, that 
        /// can not be used together. 
        /// </summary>
        /// <param name="argumentGroup1">first group of arguments</param>
        /// <param name="argumentGroup2">second group of arguments</param>
        public DistinctGroupsCertification(Argument[] argumentGroup1, Argument[] argumentGroup2)
        {
            this.argumentGroup1 = argumentGroup1;
            this.argumentGroup2 = argumentGroup2;

            argumentGroupString1 = ArgumentGroupCertification.GetGroupStringFromArguments(argumentGroup1);
            argumentGroupString2 = ArgumentGroupCertification.GetGroupStringFromArguments(argumentGroup2);
        }

        /// <summary>
        /// Creates new instance of DistinctGroupsCertification - this certification specifies two groups of arguments, that 
        /// can not be used together. 
        /// </summary>
        /// <param name="argumentGroup1">first group of arguments - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        /// <param name="argumentGroup2">second group of arguments - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        public DistinctGroupsCertification(string argumentGroup1, string argumentGroup2)
        {
            this.argumentGroupString1 = argumentGroup1;
            this.argumentGroupString2 = argumentGroup2;
        }


        /// <summary>
        /// Tests, whether some arguments from both groups are not used. If so, ArgumentConflictException is thrown.
        /// </summary>
        /// <param name="parser">parser object gives access to the defined arguments, their values and 
        /// parameters of the parser</param>
        /// <exception cref="ArgumentConflictException">Thrown when arguments from both groups are used</exception>
        /// <exception cref="InvalidArgumentGroupException">Thrown when one of the groups is empty</exception>
        public override void Certify(CommandLineParser parser)
        {
            if (argumentGroup1 == null || argumentGroup2 == null)
            {
                argumentGroup1 = ArgumentGroupCertification.GetArgumentsFromGroupString(parser, argumentGroupString1);
                argumentGroup2 = ArgumentGroupCertification.GetArgumentsFromGroupString(parser, argumentGroupString2);
            }
            
            if (argumentGroup1.Length == 0 || argumentGroup1.Length == 0)
                throw new InvalidArgumentGroupException(
                    "Argument group is empty. Argument group must have at least one member.");

            int used1 = 0;
            int used2 = 0;

            foreach (Argument argument in argumentGroup1)
            {
                if (argument.Parsed) used1++;
            }
            foreach (Argument argument in argumentGroup2)
            {
                if (argument.Parsed) used2++;
            }

            if (used1 > 0 && used2 > 0)
            {
                throw new ArgumentConflictException(String.Format(Messages.EXC_GROUP_DISTINCT, argumentGroupString1, argumentGroupString2));
            }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(Messages.EXC_GROUP_DISTINCT, argumentGroupString1, argumentGroupString2);
        }
    }


    /// <summary>
    /// Use DistinctGroupsCertificationAttribute to define <see cref="DistinctGroupsCertification"/>s declaratively. 
    /// </summary>
	/// <include file='Doc\CommandLineParser.xml' path='CommandLineParser/Certifications/CertificationAttribute/*'/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class DistinctGroupsCertificationAttribute : ArgumentCertificationAttribute
    {
        /// <summary>
        /// Adds ArgumentGroupCertification condition for the arguments. 
        /// </summary>
        /// <param name="argumentGroup1">arguments in the first group - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        /// <param name="argumentGroup2">arguments in the first group - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        public DistinctGroupsCertificationAttribute(string argumentGroup1, string argumentGroup2)
            : base(typeof(DistinctGroupsCertification), argumentGroup1, argumentGroup2)
        {
        }
    }
}