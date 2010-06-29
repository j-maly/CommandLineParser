using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using CommandLineParser.Validation;

namespace CommandLineParser
{
    /// <summary>
    /// CommandLineParser allows user to define command line arguments and then parse
    /// the arguments from the command line.
    /// </summary>
    /// <include file='Doc/CommandLineParser.xml' path='CommandLineParser/Parser/*' />
    public class CommandLineParser
    {
        #region property backing fields

        private List<Argument> arguments = new List<Argument>();

        private List<ArgumentCertification> certifications = new List<ArgumentCertification>();

        private Dictionary<char, Argument> shortNameLookup;

        private Dictionary<string, Argument> longNameLookup;

        readonly Dictionary<string, Argument> ignoreCaseLookupDirectory = new Dictionary<string, Argument>();
        private string[] argsNotParsed;

        private bool showUsageOnEmptyCommandline = false;

        private bool checkMandatoryArguments = true;

        private bool checkArgumentCertifications = true; 

        private bool allowShortSwitchGrouping = true;

		private readonly AdditionalArgumentsSettings additionalArgumentsSettings = new AdditionalArgumentsSettings();

        private readonly List<string> showUsageCommands = new List<string> { "--help", "/?" };
        
        private string showUsageHeader;
        
        private string showUsageFooter;
        
        private bool acceptSlash = true;
        
        private bool acceptHyphen = true;

        private bool ignoreCase;

        private bool acceptEqualSignSyntaxForValueArguments;

        #endregion

        /// <summary>
        /// Defined command line arguments
        /// </summary>
        public List<Argument> Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        /// <summary>
        /// Set of <see cref="ArgumentCertification">certifications</see> - certifications can be used to define 
        /// which argument combinations are allowed and such type of validations. 
        /// </summary>
        /// <seealso cref="CheckArgumentCertifications"/>
        /// <seealso cref="ArgumentCertification"/>
        /// <seealso cref="ArgumentGroupCertification"/>
        /// <seealso cref="DistinctGroupsCertification"/>
        public List<ArgumentCertification> Certifications
        {
            get { return certifications; }
            set { certifications = value; }
        }

		/// <summary>
		/// Allows more specific definition of additional arguments 
		/// (arguments after those with - and -- prefix).
		/// </summary>
    	public AdditionalArgumentsSettings AdditionalArgumentsSettings
    	{
    		get { return additionalArgumentsSettings; }
    	}

        /// <summary>
        /// Text printed in the beginning of 'show usage'
        /// </summary>
        public string ShowUsageHeader
        {
            get { return showUsageHeader; }
            set { showUsageHeader = value; }
        }

        /// <summary>
        /// Text printed in the end of 'show usage'
        /// </summary>
        public string ShowUsageFooter
        {
            get { return showUsageFooter; }
            set { showUsageFooter = value; }
        }

        /// <summary>
        /// Arguments that directly invoke <see cref="ShowUsage()"/>. By default this is --help and /?.
        /// </summary>
        public IList <string> ShowUsageCommands
        {
            get
            {
                return showUsageCommands;
            }
        }

    	/// <summary>
        /// When set to true, usage help is printed on the console when command line is without arguments.
        /// Default is false. 
        /// </summary>
        public bool ShowUsageOnEmptyCommandline
        {
            get { return showUsageOnEmptyCommandline; }
            set { showUsageOnEmptyCommandline = value; }
        }

        
        /// <summary>
        /// When set to true, <see cref="MandatoryArgumentNotSetException"/> is thrown when some of the non-optional argument
        /// is not found on the command line. Default is true.
        /// See: <see cref="Argument.Optional"/>
        /// </summary>
        public bool CheckMandatoryArguments
        {
            get { return checkMandatoryArguments; }
            set { checkMandatoryArguments = value; }
        }

        /// <summary>
        /// When set to true, arguments are certified (using set of <see cref="Certifications"/>) after parsing. 
        /// Default is true.
        /// </summary>
        public bool CheckArgumentCertifications
        {
            get { return checkArgumentCertifications; }
            set { checkArgumentCertifications = value; }
        }

        /// <summary>
        /// When set to true (default) <see cref="SwitchArgument">switch arguments</see> can be grouped on the command line. 
        /// (e.g. -a -b -c can be written as -abc). When set to false and such a group is found, <see cref="CommandLineFormatException"/> is thrown.
        /// </summary>
        public bool AllowShortSwitchGrouping
        {
            get { return allowShortSwitchGrouping; }
            set { allowShortSwitchGrouping = value; }
        }

        /// <summary>
        /// Allows arguments in /a and /arg format
        /// </summary>
        public bool AcceptSlash
        {
            get { return acceptSlash; }
            set { acceptSlash = value; }
        }

        /// <summary>
        /// Allows arguments in -a and --arg format
        /// </summary>
        public bool AcceptHyphen
        {
            get { return acceptHyphen; }
            set { acceptHyphen = value; }
        }

        /// <summary>
        /// Argument names case insensitive (--OUTPUT or --output are treated equally)
        /// </summary>
        public bool IgnoreCase
        {
            get { return ignoreCase; }
            set { ignoreCase = value; }
        }

        /// <summary>
        /// When set to true, values of <see cref="ValueArgument{TValue}"/> are separeted by space, 
        /// otherwise, they are separeted by equal sign and enclosed in quotation marks
        /// </summary>
        /// <example>
        /// --output="somefile.txt"
        /// </example>
        public bool AcceptEqualSignSyntaxForValueArguments
        {
            get { return acceptEqualSignSyntaxForValueArguments; }
            set { acceptEqualSignSyntaxForValueArguments = value; }
        }

        /// <summary>
        /// Fills lookup dictionaries with arguments names and aliases 
        /// </summary>
        private void InitializeArgumentLookupDictionaries()
        {
            shortNameLookup = new Dictionary<char, Argument>();
            longNameLookup = new Dictionary<string, Argument>();
            foreach (Argument argument in arguments)
            {
                if (argument.ShortName != ' ')
                {
                    shortNameLookup.Add(argument.ShortName, argument);
                }
                foreach (char aliasChar in argument.ShortAliases)
                {
                    shortNameLookup.Add(aliasChar, argument);
                }
                if (!String.IsNullOrEmpty(argument.LongName))
                {
                    longNameLookup.Add(argument.LongName, argument);
                }
                foreach (string aliasString in argument.LongAliases)
                {
                    longNameLookup.Add(aliasString, argument);
                }     
            }

            ignoreCaseLookupDirectory.Clear();
            foreach (KeyValuePair<char, Argument> keyValuePair in shortNameLookup)
            {
                ignoreCaseLookupDirectory.Add(keyValuePair.Key.ToString().ToUpper(), keyValuePair.Value);
            }
            foreach (KeyValuePair<string, Argument> keyValuePair in longNameLookup)
            {
                ignoreCaseLookupDirectory.Add(keyValuePair.Key.ToUpper(), keyValuePair.Value);
            }
        }

        /// <summary>
        /// Resolves arguments from the command line and calls <see cref="Argument.Parse"/> on each argument. 
		/// Additional arguments are stored in AdditionalArgumentsSettings.AdditionalArguments 
		/// if AdditionalArgumentsSettings.AcceptAdditionalArguments is set to true. 
        /// </summary>
        /// <exception cref="CommandLineFormatException">Command line arguments are not in correct format</exception>
        /// <param name="args">Command line arguments</param>
        public void ParseCommandLine(string[] args)
        {
            arguments.ForEach(delegate(Argument a) { a.Init(); });
            List<string> argsList = new List<string>(args);
            InitializeArgumentLookupDictionaries();
            ExpandShortSwitches(argsList);
            ExpandValueArgumentsWithEqualSigns(argsList);
            AdditionalArgumentsSettings.AdditionalArguments = new string[0];

            this.argsNotParsed = args;

            if ((args.Length == 0 && ShowUsageOnEmptyCommandline) ||
                (args.Length == 1 && showUsageCommands.Contains(args[0])))
            {
                ShowUsage();
                return;
            }

            if (args.Length > 0)
            {
                int argIndex;

                for (argIndex = 0; argIndex < argsList.Count;)
                {
                    string curArg = argsList[argIndex];
                    Argument argument = ParseArgument(curArg);
                    if (argument == null)
                        break;

                    argument.Parse(argsList, ref argIndex);
                    argument.UpdateBoundObject();
                }

                ParseAdditionalArguments(argsList, argIndex);   
            }

            PerformMandatoryArgumentsCheck();
            PerformCertificationCheck();
        }

        /// <summary>
        /// Searches <paramref name="parsingTarget"/> for fields with 
        /// <see cref="ArgumentAttribute">ArgumentAttributes</see> or some of its descendats. Adds new argument
        /// for each such a field and defines binding of the argument to the field. 
        /// Also adds <see cref="ArgumentCertification"/> object to <see cref="Certifications"/> collection 
        /// for each <see cref="ArgumentCertificationAttribute"/> of <paramref name="parsingTarget"/>.
        /// </summary>
        /// <seealso cref="Argument.Bind"/>
        /// <param name="parsingTarget">object where you with some ArgumentAttributes</param>
        public void ExtractArgumentAttributes(object parsingTarget)
        {
            Type targetType = parsingTarget.GetType();

            MemberInfo[] fields = targetType.GetFields();

            MemberInfo[] properties = targetType.GetProperties();

            List<MemberInfo> fieldAndProps = new List<MemberInfo>(fields);
            fieldAndProps.AddRange(properties);

            foreach (MemberInfo info in fieldAndProps)
            {
                object[] attrs = info.GetCustomAttributes(typeof(ArgumentAttribute), true);

                if (attrs.Length == 1 && attrs[0] is ArgumentAttribute)
                {
                    this.Arguments.Add(((ArgumentAttribute) attrs[0]).Argument);
                    ((ArgumentAttribute) attrs[0]).Argument.Bind = 
                        new FieldArgumentBind(parsingTarget, info.Name);
                }
            }

            object[] typeAttrs = targetType.GetCustomAttributes(typeof(ArgumentCertificationAttribute), true);
            foreach (object certificationAttr in typeAttrs)
            {
                this.Certifications.Add(((ArgumentCertificationAttribute) certificationAttr).Certification);
            }
        }

        /// <summary>
        /// Parses one argument on the command line, lookups argument in <see cref="Arguments"/> using 
        /// lookup dictionaries.
        /// </summary>
        /// <param name="curArg">argument string (including '-' or '--' prefixes)</param>
        /// <returns>Look-uped Argument class</returns>
        /// <exception cref="CommandLineFormatException">Command line is in the wrong format</exception>
        /// <exception cref="UnknownArgumentException">Unknown argument found.</exception>
        private Argument ParseArgument(string curArg)
        {
            if (curArg[0] == '-')
            {
                if (AcceptHyphen)
                {
                    string argName;
                    if (curArg.Length > 1)
                    {
                        if (curArg[1] == '-')
                        {
                            //long name
                            argName = curArg.Substring(2);
                            if (argName.Length == 1)
                            {
                                throw new CommandLineFormatException(String.Format(
                                    Messages.EXC_FORMAT_SHORTNAME_PREFIX, argName));
                            }

                        }
                        else
                        {
                            //short name
                            argName = curArg.Substring(1);
                            if (argName.Length != 1)
                            {
                                throw new CommandLineFormatException(
                                    String.Format(Messages.EXC_FORMAT_LONGNAME_PREFIX, argName));
                            }
                        }

                        Argument argument = LookupArgument(argName);
                        if (argument != null) return argument;
                        else
                            throw new UnknownArgumentException(string.Format(Messages.EXC_ARG_UNKNOWN, argName), argName);
                    }
                    else
                    {
                        throw new CommandLineFormatException(Messages.EXC_FORMAT_SINGLEHYPHEN);
                    }
                }
                else
                    return null;
            }
            else if (curArg[0] == '/')
            {
                if (AcceptSlash)
                {
                    if (curArg.Length > 1)
                    {
                        if (curArg[1] == '/')
                        {
                            throw new CommandLineFormatException(Messages.EXC_FORMAT_SINGLESLASH);
                        }
                        string argName = curArg.Substring(1);
                        Argument argument = LookupArgument(argName);
                        if (argument != null) return argument;
                        else throw new UnknownArgumentException(string.Format(Messages.EXC_ARG_UNKNOWN, argName), argName);
                    }
                    else
                    {
                        throw new CommandLineFormatException(Messages.EXC_FORMAT_DOUBLESLASH);
                    }
                }
                else
                    return null;
            }
            else
            /*
             * curArg does not start with '-' character and therefore it is considered additional argument.
             * Argument parsing ends here.
             */
            return null;
        }

        /// <summary>
        /// Checks whether or non-optional arguments were defined on the command line. 
        /// </summary>
        /// <exception cref="MandatoryArgumentNotSetException"><see cref="Argument.Optional">Non-optional</see> argument not defined.</exception>
        /// <seealso cref="CheckMandatoryArguments"/>, <seealso cref="Argument.Optional"/>
        private void PerformMandatoryArgumentsCheck()
        {
            arguments.ForEach(delegate (Argument arg)
                                  {
                                      if (!arg.Optional && !arg.Parsed)
                                          throw new MandatoryArgumentNotSetException(string.Format(Messages.EXC_MISSING_MANDATORY_ARGUMENT, arg.Name), arg.Name);
                                  });                         
        }

		/// <summary>
		/// Performs certifications
		/// </summary>
        private void PerformCertificationCheck()
        {
            certifications.ForEach(delegate (ArgumentCertification certification)
                                       {
                                           certification.Certify(this);
                                       });
        }

        /// <summary>
        /// Parses the rest of the command line for additional arguments
        /// </summary>
        /// <param name="argsList">list of thearguments</param>
        /// <param name="i">index of the first additional argument in <paramref name="argsList"/></param>
        /// <exception cref="CommandLineFormatException">Additional arguments found, but they are 
        /// not accepted</exception>
        private void ParseAdditionalArguments(List<string> argsList, int i)
        {
            if (AdditionalArgumentsSettings.AcceptAdditionalArguments)
            {
				AdditionalArgumentsSettings.AdditionalArguments = new string[argsList.Count - i];
                if (i < argsList.Count)
                {
					Array.Copy(argsList.ToArray(), i, AdditionalArgumentsSettings.AdditionalArguments, 0, argsList.Count - i);
                }
            	AdditionalArgumentsSettings.ProcessArguments();
            }
            else
            {
                throw new CommandLineFormatException(
                    Messages.EXC_ADDITIONAL_ARGUMENTS_FOUND);
            }
        }

        /// <summary>
        /// If <see cref="AllowShortSwitchGrouping"/> is set to true,  each group of switch arguments (e. g. -abcd) 
        /// is expanded into full format (-a -b -c -d) in the list.
        /// </summary>
        /// <exception cref="CommandLineFormatException">Argument of type differnt from SwitchArgument found in one of the groups. </exception>
        /// <param name="argsList">List of arguments</param>
        /// <exception cref="CommandLineFormatException">Arguments that are not <see cref="SwitchArgument">switches</see> found 
        /// in a group.</exception>
        /// <seealso cref="AllowShortSwitchGrouping"/>
        private void ExpandShortSwitches(IList<string> argsList)
        {
            if (AllowShortSwitchGrouping)
            {
                for (int i = 0; i < argsList.Count; i++)
                {
                    string arg = argsList[i];
                    if (arg.Length > 2)
                    {
                        char sep = arg[0];
                        if ((arg[0] == '-' && arg[1] != '-' && AcceptHyphen) 
                            || (arg[0] == '/' && arg[1] != '/' && AcceptSlash))
                        {
                            argsList.RemoveAt(i);
                            //arg ~ -xyz
                            foreach (char c in arg.Substring(1))
                            {
                                if (shortNameLookup.ContainsKey(c) && !(shortNameLookup[c] is SwitchArgument))
                                {
                                    throw new CommandLineFormatException(
                                        string.Format(Messages.EXC_BAD_ARG_IN_GROUP, c));
                                }

                                argsList.Insert(i, sep.ToString() + c);
                                i++;
                            }
                        }
                    }
                }
            }
        }

        private void ExpandValueArgumentsWithEqualSigns(IList<string> argsList)
        {
            if (AcceptEqualSignSyntaxForValueArguments)
            {
                for (int i = 0; i < argsList.Count; i++)
                {
                    string arg = argsList[i];

                    Regex r = new Regex("(.*)=\"(.*)\"");
                    if (AcceptEqualSignSyntaxForValueArguments && r.IsMatch(arg))
                    {
                        Match m = r.Match(arg);
                        string argNameWithSep = m.Groups[1].Value;
                        string argName = argNameWithSep;
                        while (argName.StartsWith("-") && AcceptHyphen)
                            argName = argName.Substring(1);
                        while (argName.StartsWith("/") && AcceptSlash)
                            argName = argName.Substring(1);
                        string argValue = m.Groups[2].Value;
                        Argument argument = LookupArgument(argName);
                        if (argument is IValueArgument)
                        {
                            argsList.RemoveAt(i);
                            argsList.Insert(i, argNameWithSep);
                            i++;
                            argsList.Insert(i, argValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns argument of given name
        /// </summary>
        /// <param name="argName">Name of the argument (<see cref="Argument.ShortName"/>, <see cref="Argument.LongName"/>, or alias)</param>
        /// <returns>Found argument or null when argument is not present</returns>
        public Argument LookupArgument(string argName)
        {
            if (argName.Length == 1)
            {
                if (shortNameLookup.ContainsKey(argName[0]))
                {
                    return shortNameLookup[argName[0]];
                }
            }
            else
            {
                if (longNameLookup.ContainsKey(argName))
                {
                    return longNameLookup[argName];
                }
            }
            if (IgnoreCase && ignoreCaseLookupDirectory.ContainsKey(argName.ToUpper()))
            {
                return ignoreCaseLookupDirectory[argName.ToUpper()];
            }
            // argument not found anywhere
            return null;
        }

        /// <summary>
        /// Prints arguments information and usage information to the console. 
        /// </summary>
        public void ShowUsage()
        {
            Console.WriteLine(ShowUsageHeader);

            Console.WriteLine(Messages.MSG_USAGE);

            foreach (Argument argument in arguments)
            {
                Console.Write("\t");
                bool comma = false;
                if (argument.ShortName != ' ')
                {
                    Console.Write("-" + argument.ShortName);
                    comma = true;
                }
                foreach (char c in argument.ShortAliases)
                {
                    if (comma)
                        Console.WriteLine(", ");
                    Console.Write("-" + c);
                    comma = true;
                }
                if (!String.IsNullOrEmpty(argument.LongName))
                {
                    if (comma)
                        Console.Write(", ");
                    Console.Write("--" + argument.LongName);
                    comma = true;
                }
                foreach (string str in argument.LongAliases)
                {
                    if (comma)
                        Console.Write(", ");
                    Console.Write("--" + str);
                    comma = true; 
                }

                if (argument.Optional)
                    Console.Write(Messages.MSG_OPTIONAL);
                Console.WriteLine("... {0} ", argument.Description);

                if (!String.IsNullOrEmpty(argument.Example))
                {
                    Console.WriteLine(Messages.MSG_EXAMPLE_FORMAT, argument.Example);
                }

                if (!String.IsNullOrEmpty(argument.FullDescription))
                {
                    Console.WriteLine();
                    Console.WriteLine(argument.FullDescription);
                }
                Console.WriteLine();
            }
            
            if (Certifications.Count > 0)
            {
                Console.WriteLine(Messages.CERT_REMARKS);
                foreach (ArgumentCertification certification in Certifications)
                {
                    Console.WriteLine("\t" + certification.GetDescription);    
                }
                Console.WriteLine();
            }

            Console.WriteLine(ShowUsageFooter);
        }

        /// <summary>
        /// Prints values of parsed arguments. Can be used for debugging. 
        /// </summary>
        public void ShowParsedArguments()
        {
            Console.WriteLine(Messages.MSG_PARSING_RESULTS);
            Console.WriteLine("\t" + Messages.MSG_COMMAND_LINE);    
            foreach (string arg in argsNotParsed)
            {
                Console.Write(arg);
                Console.Write(" ");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t" + Messages.MSG_PARSED_ARGUMENTS);
            foreach (Argument argument in arguments)
            {
                if (argument.Parsed)
                    argument.PrintValueInfo();
            }
            Console.WriteLine();
			if (AdditionalArgumentsSettings.AcceptAdditionalArguments)
            {
                Console.WriteLine("\t" + Messages.MSG_ADDITIONAL_ARGUMENTS);

				foreach (string simpleArgument in AdditionalArgumentsSettings.AdditionalArguments)
                {
                    Console.Write(simpleArgument + " ");
                }

                Console.WriteLine();   
                Console.WriteLine();
            }
        }

		/// <summary>
		/// <para>
		/// Fills FullDescription of all the difined arguments from a resource file. 
		/// For each argument selects a string from a resource that has the same resource key
		/// as is the currrent value of the argument's FullDescription. 
		/// </para>
		/// <para>
		/// This way the value of FullDescription's can be set to keys and these keys are replaced by 
		/// the resource values when the method is called. 
		/// </para>
		/// </summary>
		/// <param name="resource">The resource.</param>
    	public void FillDescFromResource(IResource resource)
    	{
    		foreach (Argument argument in Arguments)
    		{
				if (!String.IsNullOrEmpty(argument.FullDescription))
				{
					string ld = resource.ResourceManager.GetString(argument.FullDescription);
					argument.FullDescription = ld;
				}
    		}

			foreach (Argument argument in AdditionalArgumentsSettings.TypedAdditionalArguments)
			{
				if (!String.IsNullOrEmpty(argument.FullDescription))
				{
					string ld = resource.ResourceManager.GetString(argument.FullDescription);
					argument.FullDescription = ld;
				}
			}	
    	}
    }
}