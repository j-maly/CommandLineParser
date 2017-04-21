using System;
using System.Text;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Validation
{
    /// <summary>
    /// Condition for the argument group.
    /// </summary>
    public enum EArgumentGroupCondition
    {
        /// <summary>
        /// At least one of the arguments in the group must be used
        /// </summary>
        AtLeastOneUsed,

        /// <summary>
        /// Exactly one of the arguments in the group must be used
        /// </summary>
        ExactlyOneUsed,

        /// <summary>
        /// Only one of the arguments in the group can be used
        /// </summary>
        OneOreNoneUsed,

        /// <summary>
        /// All or none of the arguments in the group can be used
        /// </summary>
        AllOrNoneUsed,

        /// <summary>
        /// All of the arguments in the group must be used
        /// </summary>
        AllUsed
    }

    /// <summary>
    /// Allows to define which arguments can be used together and which combinations
    /// are forbidden for the application.
    /// </summary>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Certifications/Certification/*'/>
    public class ArgumentGroupCertification : ArgumentCertification
    {
        /// <summary>
        /// Condition of for the argument group.
        /// </summary>
        public EArgumentGroupCondition Condition { get; set; }

        private Argument[] _argumentGroup;

        private readonly string _argumentGroupString;

        /// <summary>
        /// Creates new instance of <see cref="ArgumentGroupCertification"/>. 
        /// </summary>
        /// <param name="arguments">arguments in the group</param>
        /// <param name="condition">condition for the group</param>
        public ArgumentGroupCertification(Argument[] arguments, EArgumentGroupCondition condition)
        {
            _argumentGroup = arguments;
            Condition = condition;
            if (arguments != null)
                _argumentGroupString = GetGroupStringFromArguments(_argumentGroup);
            if (_description == null && _argumentGroupString != null)
            {
                _description = DefaultUsageDescription();
            }
        }


        /// <summary>
        /// Creates new instance of <see cref="ArgumentGroupCertification"/>. 
        /// </summary>
        /// <param name="arguments">arguments in the group - separated by one of these characters: ',' ';' '|'</param>
        /// <param name="condition">condition for the group</param>
        public ArgumentGroupCertification(string arguments, EArgumentGroupCondition condition)
            : this((Argument[])null, condition)
        {
            _argumentGroupString = arguments;
            if (_description == null)
            {
                _description = DefaultUsageDescription();
            }
        }

        /// <summary>
        /// Finds Argument objects specified in groupString
        /// </summary>
        /// <param name="parser">parser where the arguments are defined</param>
        /// <param name="groupString">string containing argument names</param>
        /// <returns></returns>
        internal static Argument[] GetArgumentsFromGroupString(CommandLineParser parser, string groupString)
        {
            string[] argumentsSplitted = groupString.Split(new char[] { ';', ',', '|' });
            Argument[] arguments = new Argument[argumentsSplitted.Length];
            for (int i = 0; i < argumentsSplitted.Length; i++)
            {
                string argName = argumentsSplitted[i];
                arguments[i] = parser.LookupArgument(argName);
                if (arguments[i] == null)
                    throw new UnknownArgumentException(
                        String.Format(Messages.EXC_ARG_UNKNOWN, argName), argName);
            }
            return arguments;
        }

        /// <summary>
        /// Creates a string of argument names
        /// </summary>
        /// <param name="argumentGroup">arguments</param>
        /// <returns>string of names of the arguments, separated by '|' character</returns>
        internal static string GetGroupStringFromArguments(Argument[] argumentGroup)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Argument argument in argumentGroup)
            {
                sb.AppendFormat("-{0}|", argument.ShortName);
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Test the arguments parsed by the parser for the <see cref="Condition"/>.
        /// </summary>
        /// <param name="parser">parser object gives access to the defined arguments, their values and 
        /// parameters of the parser</param>
        /// <exception cref="InvalidArgumentGroupException">thrown when group contains no arguments</exception>
        /// <exception cref="ArgumentConflictException">thrown when the parsed arguments does not meet the condition</exception>
        public override void Certify(CommandLineParser parser)
        {
            if (_argumentGroup == null)
            {
                _argumentGroup = GetArgumentsFromGroupString(parser, _argumentGroupString);
            }

            if (_argumentGroup.Length == 0)
                throw new InvalidArgumentGroupException(
                    "Argument group is empty. Argument group must have at least one member.");

            int usedArgsFromGroup = 0;
            foreach (Argument argument in _argumentGroup)
            {
                if (argument.Parsed)
                {
                    usedArgsFromGroup++;
                }
            }

            switch (Condition)
            {
                case EArgumentGroupCondition.AtLeastOneUsed:
                    if (usedArgsFromGroup == 0)
                        throw new ArgumentConflictException(
                            string.Format(Description, _argumentGroupString));
                    break;
                case EArgumentGroupCondition.ExactlyOneUsed:
                    if (usedArgsFromGroup == 0)
                        throw new ArgumentConflictException(
                            string.Format(Description, _argumentGroupString));
                    if (usedArgsFromGroup > 1)
                        throw new ArgumentConflictException(
                            string.Format(Description, _argumentGroupString));
                    break;
                case EArgumentGroupCondition.OneOreNoneUsed:
                    if (usedArgsFromGroup > 1)
                        throw new ArgumentConflictException(
                            string.Format(Description, _argumentGroupString));
                    break;
                case EArgumentGroupCondition.AllUsed:
                    if (usedArgsFromGroup != _argumentGroup.Length)
                        throw new ArgumentConflictException(
                            string.Format(Description, _argumentGroupString));
                    break;
                case EArgumentGroupCondition.AllOrNoneUsed:
                    if (usedArgsFromGroup != _argumentGroup.Length && usedArgsFromGroup != 0)
                        throw new ArgumentConflictException(
                            string.Format(Description, _argumentGroupString));
                    break;
            }
        }
        
        private string DefaultUsageDescription()
        {
            switch (Condition)
            {
                case EArgumentGroupCondition.AtLeastOneUsed:
                    return string.Format(Messages.GROUP_AT_LEAST_ONE_USED, _argumentGroupString);
                case EArgumentGroupCondition.ExactlyOneUsed:
                        return string.Format(Messages.GROUP_EXACTLY_ONE_USED, _argumentGroupString);
                    case EArgumentGroupCondition.OneOreNoneUsed:
                        return string.Format(Messages.GROUP_ONE_OR_NONE_USED, _argumentGroupString);
                case EArgumentGroupCondition.AllUsed:
                    return string.Format(Messages.GROUP_ALL_USED, _argumentGroupString);
                case EArgumentGroupCondition.AllOrNoneUsed:
                    return string.Format(Messages.GROUP_ALL_OR_NONE_USED, _argumentGroupString);
                default:
                    return string.Empty;
            }
        }
    }

    /// <summary>
    /// Thrown when there is some conflict among the used arguments. 
    /// </summary>
    public class ArgumentConflictException : CommandLineException
    {
        /// <summary>
        /// Creates new instance of <see cref="ArgumentConflictException" />
        /// </summary>
        /// <param name="message">cause of the exception</param>
        public ArgumentConflictException(string message)
            : base(message)
        {

        }
    }

    /// <summary>
    /// Use ArgumentGroupCertificationAttribute to define <see cref="ArgumentGroupCertification"/>s declaratively. 
    /// </summary>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Certifications/CertificationAttribute/*'/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class ArgumentGroupCertificationAttribute : ArgumentCertificationAttribute
    {
        /// <summary>
        /// Adds ArgumentGroupCertification condition for the arguments. 
        /// </summary>
        /// <param name="arguments">arguments in the group - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        /// <param name="condition">condition for the group - names of the 
        /// arguments separated by commas, semicolons or '|' character</param>
        public ArgumentGroupCertificationAttribute(string arguments, EArgumentGroupCondition condition)
            : base(typeof(ArgumentGroupCertification), arguments, condition)
        {
        }
    }
}