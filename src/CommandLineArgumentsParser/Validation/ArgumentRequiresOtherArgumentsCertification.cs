using System;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System.Linq;

namespace CommandLineParser.Validation
{
    /// <summary>
    /// Allows to define a group of arguments required for another argument. 
    /// </summary>
    public class ArgumentRequiresOtherArgumentsCertification : ArgumentCertification
    {
        private Argument[] _argumentsRequiredForMainArgument;
        private Argument _mainArgument;
        private readonly string _argumentsRequiredForMainArgumentString;
        private readonly string _mainArgumentString;

        /// <summary>
        /// Creates new instance of ArgumentRequiresOtherArgumentsCertification - this certification allows to define a 
        /// group of arguments required for another argument. 
        /// </summary>
        /// <param name="mainArgument"></param>
        /// <param name="argumentsRequiredForMainArgument"></param>
        public ArgumentRequiresOtherArgumentsCertification(Argument mainArgument, Argument[] argumentsRequiredForMainArgument)
        {
            _argumentsRequiredForMainArgument = argumentsRequiredForMainArgument;            
            _argumentsRequiredForMainArgumentString = ArgumentGroupCertification.GetGroupStringFromArguments(argumentsRequiredForMainArgument);
            _mainArgumentString = ArgumentGroupCertification.GetGroupStringFromArguments(new[] { mainArgument });
            if (_description == null)
            {
                _description = DefaultUsageDescription();
            }
        }

        /// <summary>
        /// Creates new instance of ArgumentRequiresOtherArgumentsCertification - this certification allows to define a 
        /// group of arguments required for another argument. 
        /// </summary>
        /// <param name="mainArgument">name of the argument that requires other arguments</param>
        /// <param name="argumentsRequiredForMainArgument">arguments required by <paramref name="mainArgument"/> - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        public ArgumentRequiresOtherArgumentsCertification(string mainArgument, string argumentsRequiredForMainArgument)
        {
            _argumentsRequiredForMainArgumentString = argumentsRequiredForMainArgument;
            _mainArgumentString = mainArgument;
            if (_description == null)
            {
                _description = DefaultUsageDescription();
            }
        }

        /// <summary>
        /// When main argument is present, tests, whether arguments required by main argument are also present. 
        /// If not <see cref="MandatoryArgumentNotSetException"/> is thrown.
        /// </summary>
        /// <param name="parser">parser object gives access to the defined arguments, their values and 
        /// parameters of the parser</param>
        /// <exception cref="MandatoryArgumentNotSetException">Thrown when main argument is present but some of the arguments
        /// required for main argument is not.</exception>        
        public override void Certify(CommandLineParser parser)
        {
            if (_argumentsRequiredForMainArgument == null || _mainArgument == null)
            {
                _argumentsRequiredForMainArgument = ArgumentGroupCertification.GetArgumentsFromGroupString(parser, _argumentsRequiredForMainArgumentString);
                _mainArgument = ArgumentGroupCertification.GetArgumentsFromGroupString(parser, _mainArgumentString).SingleOrDefault();
            }

            if (_argumentsRequiredForMainArgument.Length == 0)
                throw new InvalidArgumentGroupException(
                    "Argument group is empty. Argument group must have at least one member.");

            if (Description == null)
            {
                Description = DefaultUsageDescription();
            }

            if (_mainArgument.Parsed)
            {
                foreach (Argument requiredArgument in _argumentsRequiredForMainArgument)
                {
                    if (!requiredArgument.Parsed)
                    {
                        var withDefaultValue = requiredArgument as IArgumentWithDefaultValue;
                        if (withDefaultValue?.DefaultValue != null)
                        {
                            continue;
                        }
                        throw new MandatoryArgumentNotSetException(String.Format(Messages.EXC_GROUP_ARGUMENTS_REQUIRED_BY_ANOTHER_ARGUMENT, _mainArgumentString, _argumentsRequiredForMainArgumentString), requiredArgument.Name);
                    }
                }
            }            
        }

        private string DefaultUsageDescription()
        {
            return string.Format(Messages.EXC_GROUP_ARGUMENTS_REQUIRED_BY_ANOTHER_ARGUMENT, _mainArgument, _argumentsRequiredForMainArgumentString);
        }
    }
    
    /// <summary>
    /// Use DistinctGroupsCertificationAttribute to define <see cref="DistinctGroupsCertification"/>s declaratively. 
    /// </summary>
	/// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Certifications/CertificationAttribute/*'/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class ArgumentRequiresOtherArgumentsCertificationAttribute : ArgumentCertificationAttribute
    {
        /// <summary>
        /// Adds <see cref="ArgumentRequiresOtherArgumentsCertification"/> condition for the arguments. 
        /// </summary>
        /// <param name="mainArgument">name of the argument that requires other arguments</param>
        /// <param name="argumentsRequiredForMainArgument">arguments required by <paramref name="mainArgument"/> - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        public ArgumentRequiresOtherArgumentsCertificationAttribute(string mainArgument, string argumentsRequiredForMainArgument)
            : base(typeof(ArgumentRequiresOtherArgumentsCertification), mainArgument, argumentsRequiredForMainArgument)
        {
        }
    }
}