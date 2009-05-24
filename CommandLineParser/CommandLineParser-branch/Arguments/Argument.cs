using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Abstract command line argument that can have name, description, aliases and can be marked
    /// optional/mandatory. 
    /// </summary>
    /// <seealso cref="ValueArgument{TValue}"/>
    /// <seealso cref="SwitchArgument"/>
    /// <seealso cref="CertifiedValueArgument{TValue}"/>
    /// <seealso cref="BoundedValueArgument{TValue}"/>
    /// <seealso cref="EnumeratedValueArgument{TValue}"/>
    /// <include file='Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/Argument/*'/>
    public abstract class Argument
    {
        #region property backing fields

        private char shortName = '_';

        private string longName;

        private string description;

        private string fullDescription;

        private bool optional = true;

        private bool parsed = false;

        private bool allowMultiple = false; 

        /// <summary>
        /// List of short aliases.
        /// </summary>
        internal List<char> shortAliases;

        /// <summary>
        /// List of long aliases.
        /// </summary>
        internal List<string> longAliases;

        private FieldArgumentBind ? bind;

        #endregion

        #region constructors

        /// <summary>
        /// Creates new command line argument with a <see cref="ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        protected Argument(char shortName)
        {
            this.ShortName = shortName;
        }

        /// <summary>
        /// Creates new command line argument with a <see cref="ShortName">short name</see>and <see cref="LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        protected Argument(char shortName, string longName)
        {
            this.LongName = longName;
            this.ShortName = shortName;
        }

        /// <summary>
        /// Creates new command line argument with a <see cref="ShortName">short name</see>,
        /// <see cref="LongName">long name</see> and <see cref="Description">description</see>
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">Description of the argument</param>
        protected Argument(char shortName, string longName, string description)
        {
            this.Description = description;
            this.LongName = longName;
            this.ShortName = shortName;
        }

        #endregion

        /// <summary>
        /// Mark argument optional. Arguments with Optional = false can be checked in <see cref="CommandLineParser.ParseCommandLine(string[])"/>"/> method.
        /// <see cref="CommandLineParser.CheckMandatoryArguments"/>
        /// <remarks>Default is true</remarks>
        /// </summary>
        public bool Optional
        {
            get { return optional; }
            set { optional = value; }
        }

        /// <summary>
        /// Specifies whether argument can appear multiple times on the command line. 
        /// Default is false; 
        /// </summary>
        public bool AllowMultiple
        {
            get { return allowMultiple; }
            set { allowMultiple = value; }
        }

        /// <summary>
        /// Tests whether argument was already parsed on the command line. 
        /// See <see cref="CommandLineParser.ParseCommandLine(string[])"/> and <see cref="Init"/>. 
        /// </summary>
        public bool Parsed
        {
            get { return parsed; }
            protected set { parsed = value; }
        }

        /// <summary>
        /// Long, full description of the argument. 
        /// </summary>
        public string FullDescription
        {
            get { return fullDescription; }
            set { fullDescription = value; }
        }

        /// <summary>
        /// Description of the argument 
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private static char[] badChars = new char[] { '\r', '\n', ' ', '\t'};

        /// <summary>
        /// Long name of the argument. Can apear on the command line in --<i>longName</i> format.
        /// Must be one word. 
        /// </summary>
        /// <exception cref="CommandLineFormatException">Name is invalid</exception>
        public string LongName
        {
            get { return longName; }
            set
            {
                if (value.IndexOfAny(badChars) > -1)
                {
                    throw new FormatException(Messages.EXC_ARG_NOT_ONE_WORD);
                }
                longName = value;
            }
        }

        /// <summary>
        /// Long name of the argument. Can apear on the command line in -<i>shortName</i> format.
        /// </summary>
        /// <exception cref="CommandLineFormatException">Name is invalid</exception>
        public char ShortName
        {
            get { return shortName; }
            set
            {
                if (char.IsWhiteSpace(value))
                {
                    throw new FormatException(Messages.EXC_ARG_NOT_ONE_CHAR);
                }
                shortName = value;
            }
        }

        /// <summary>
        /// Name of the argument. 
        /// </summary>
        internal string Name
        {
            get
            {
                if (shortName != ' ' && !String.IsNullOrEmpty(longName))
                {
                    return String.Format("{0}({1})", shortName, longName);
                }
                if (!String.IsNullOrEmpty(longName))
                {
                    return longName;
                }
                if (shortName != ' ')
                {
                    return shortName.ToString();
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Defined short aliases of the parameter. Can appear on the command line in  -<i>shortAlias</i> format.
        /// <see cref="AddAlias(char)"/>
        /// </summary>
        /// <seealso cref="LongAliases"/>
        public IEnumerable<char> ShortAliases
        {
            get
            {
                if (shortAliases != null)
                {
                    foreach (char c in shortAliases)
                    {
                        yield return c;
                    }
                }
                yield break;
            }
        }

        /// <summary>
        /// Defined long aliases of the parameter. Can appear on the command line in  --<i>longAlias</i> format.
        /// <see cref="AddAlias(string)"/>
        /// </summary>
        /// <seealso cref="ShortAliases"/>
        public IEnumerable<string> LongAliases
        {
            get
            {
                if (longAliases != null)
                {
                    foreach (string s in longAliases)
                    {
                        yield return s;
                    }
                }
                yield break;
            }
        }

        /// <summary>
        /// Defines mapping of the value of the argument to a field of another object.
        /// Bound field is updated after the value of the argument is parsed by <see cref="CommandLineParser"/>.
        /// </summary>
        public FieldArgumentBind ? Bind
        {
            get { return bind; }
            set { bind = value; }
        }

        /// <summary>
        /// Creates a short name alias for the parameter. The parameter is processed identically when the alias appears on the command line.
        /// <param name="alias">Short alias of the argument</param>
        /// </summary>
        public void AddAlias(char alias)
        {
            if (shortAliases == null)
                shortAliases = new List<char>();
            shortAliases.Add(alias);
        }

        /// <summary>
        /// Creates a long name alias for the parameter. The parameter is processed identically when the alias appears on the command line.
        /// <param name="alias">Long alias of the argument</param>
        /// </summary>
        public void AddAlias(string alias)
        {
            if (longAliases == null)
                longAliases = new List<string>();
            longAliases.Add(alias);
        }

        /// <summary>
        /// Parse argument. This method should read the input arguments and set the argument fields.  
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="i">index to the args array, where this argument occured. 
        /// Parse method should move the index to the next argument after the argument is processed. </param>
        /// <remarks>It is up to the argument class how many words it will consume from the command line. 
        /// At the end, it should just point the <paramref name="i"/> index to the correct place where the
        /// argument class passes the control back to the <see cref="CommandLineParser"/></remarks>
        internal virtual void Parse(IList<string> args, ref int i)
        {
            //check for invalid multiple occurences
            if (Parsed && !AllowMultiple)
                throw new CommandLineArgumentException(String.Format(Messages.EXC_ARG_VALUE_MULTIPLE_OCCURS, Name), Name);
        }

        /// <summary>
        /// Prints information about the argument value to the console.
        /// </summary>
        internal abstract void PrintValueInfo();

        /// <summary>
        /// Initializes the argument. Sets <see cref="Parsed"/> to false. Override in inherited classes 
        /// if any further initialization is needed. 
        /// </summary>
        public virtual void Init()
        {
            parsed = false; 
        }

        /// <summary>
        /// If <see cref="Bind"/> is specified, the bound field of the bound object should be updated. 
        /// according to the value of the argument. Should be called after Parse is called.  
        /// </summary>
        public abstract void UpdateBoundObject();

        
    }

    /// <summary>
    /// <para>
    /// Base class for argument attributes. Each subclass of Argument has corresponding 
    /// subclass of ArgumentAttribute that can be used to define the Argument declaratively. 
    /// </para>
    /// <para>
    /// Instead of creating an argument explicitly, you can assign a class' field an argument
    /// attribute and let the CommandLineParse take care of binding the attribute to the field.
    /// </para>
    /// </summary>
    /// <remarks>Use <see cref="CommandLineParser.ExtractArgumentAttributes"/> for each object 
    /// you where you have delcared argument attributes.
    /// </remarks>
    /// <example>
    /// <code source="Examples\AttributeExample.cs" lang="cs" title="Example of declaring argument attributes" />
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ArgumentAttribute: Attribute
    {
        private Argument argument;

        /// <summary>
        /// The underlying Argument type
        /// </summary>
        public Argument Argument
        {
            get { return argument; }
        }

        /// <summary>
        /// Creates new instance of ArgumentAttribute.
        /// </summary>
        /// <param name="underlyingArgumentType">Type of the underlying argument.</param>
        /// <param name="constructorParams">Parameters of the constructor of underlying argument</param>
        protected ArgumentAttribute(Type underlyingArgumentType, params object[] constructorParams)
        {
            if (!underlyingArgumentType.IsSubclassOf(typeof(Argument)))
            {
                throw new InvalidOperationException("Parameter underlyingArgumentType must be a subclass of Argument.");
            }
            //create argument object of proper type using reflection
            argument = (Argument)Activator.CreateInstance(underlyingArgumentType, constructorParams);
        }

        #region delegated argument members

        /// <summary>
        /// Description of the argument 
        /// </summary>
        public string Description
        {
            get { return argument.Description; }
            set { argument.Description = value; }
        }

        /// <summary>
        /// Long, full description of the argument. 
        /// </summary>
        public string FullDescription
        {
            get { return argument.FullDescription; }
            set { argument.FullDescription = value; }
        }

        /// <summary>
        /// Long name of the argument. Can apear on the command line in --<i>longName</i> format.
        /// Must be one word. 
        /// </summary>
        /// <exception cref="CommandLineFormatException">Name is invalid</exception>
        public string LongName
        {
            get { return argument.LongName; }
            set { argument.LongName = value; }
        }

        /// <summary>
        /// Mark argument optional. Arguments with Optional = false can be checked in <see cref="CommandLineParser.ParseCommandLine(string[])"/> method.
        /// <see cref="CommandLineParser.CheckMandatoryArguments"/>
        /// <remarks>Default is true</remarks>
        /// </summary>
        public bool Optional
        {
            get { return argument.Optional; }
            set { argument.Optional = value; }
        }

        /// <summary>
        /// Allows argument to appear multiple times on the command line. Default is false.
        /// </summary>
        public bool AllowMultiple
        {
            get { return Argument.AllowMultiple; }
            set { Argument.AllowMultiple = value; }
        }

        /// <summary>
        /// Long name of the argument. Can apear on the command line in -<i>shortName</i> format.
        /// </summary>
        /// <exception cref="CommandLineFormatException">Name is invalid</exception>
        public char ShortName
        {
            get { return argument.ShortName; }
            set { argument.ShortName = value; }
        }

        #endregion

    }
}