using System;
using System.Text.RegularExpressions;
using CommandLineParser.Compatibility;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Use RegexValueArgument for an argument whose value must match a regular expression. 
    /// </summary>
    public class RegexValueArgument : CertifiedValueArgument<string>
    {
        private Regex regex;
        private string sampleValue;

        /// <summary>
        /// Regular expression which the value must match 
        /// </summary>
        public Regex Regex
        {
            get { return regex; }
            set { regex = value; }
        }
        
        /// <summary>
        /// Sample value that would be displayed to the user as a suggestion when 
        /// the user enters a wrong value. 
        /// </summary>
        public string SampleValue
        {
            get { return sampleValue; }
            set { sampleValue = value; }
        }

        #region constructor

        /// <summary>
        /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="regex">regular expressin which the value must match</param>
        public RegexValueArgument(char shortName, Regex regex) : base(shortName)
        {
            this.regex = regex;
        }

        /// <summary>
        /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
        /// </summary>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="regex">regular expressin which the value must match</param>
        public RegexValueArgument(string longName, Regex regex) : base(longName)
        {
            this.regex = regex;
        }

        /// <summary>
        /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="regex">regular expressin which the value must match</param>
        public RegexValueArgument(char shortName, string longName, Regex regex) : base(shortName, longName)
        {
            this.regex = regex;
        }

        /// <summary>
        /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">description of the argument</param>
        /// <param name="regex">regular expressin which the value must match</param>
        public RegexValueArgument(char shortName, string longName, string description, Regex regex) : base(shortName, longName, description)
        {
            this.regex = regex;
        }
        #endregion

        protected override void Certify(string value)
        {
            // override the Certify method to validate value against regex
            if (regex != null)
            {
                if (!regex.IsMatch(value))
                {
                    if (SampleValue == null)
                    {
                        throw new CommandLineArgumentOutOfRangeException(
                            string.Format("Argument '{0}' does not match the regex pattern '{1}'.", value, regex), Name);
                    }
                    else
                    {
                        throw new CommandLineArgumentOutOfRangeException(
                            string.Format("Argument '{0}' does not match the regex pattern '{1}'. An example of a valid value would be '{2}'.", value, regex, SampleValue), Name);
                    }                    
                }
            }
        }
    }

    /// <summary>
    /// <para>
    /// Attribute for declaring a class' field a <see cref="RegexValueArgumentAttribute"/> and 
    /// thus binding a field's value to a certain command line switch argument.
    /// </para>
    /// <para>
    /// Instead of creating an argument explicitly, you can assign a class' field an argument
    /// attribute and let the CommandLineParse take care of binding the attribute to the field.
    /// </para>
    /// </summary>
    /// <remarks>Appliable to fields and properties (public).</remarks>
    /// <remarks>Use <see cref="CommandLineParser.ExtractArgumentAttributes"/> for each object 
    /// you where you have delcared argument attributes.</remarks>
    public sealed class RegexValueArgumentAttribute : ArgumentAttribute
    {
        private readonly Type _argumentType;

        /// <summary>
        /// Creates new instance of RegexValueArgumentAttribute. RegexValueArgumentAttribute
        /// uses underlying <see cref="RegexValueArgument"/>.
        /// </summary>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="pattern">Regex pattern</param>
        public RegexValueArgumentAttribute(char shortName, string pattern)
            : base(typeof(RegexValueArgument), shortName, new Regex(pattern))
        {
            _argumentType = typeof(RegexValueArgument);
        }

        /// <summary>
		/// Creates new instance of RegexValueArgumentAttribute. RegexValueArgumentAttribute
		/// uses underlying <see cref="RegexValueArgument"/>.
        /// </summary>
        /// <param name="longName"><see cref="Argument.LongName">short name</see> of the underlying argument</param>
        /// <param name="pattern">Regex pattern</param>
        public RegexValueArgumentAttribute(string longName, string pattern)
            : base(typeof(RegexValueArgument), longName, new Regex(pattern))
        {
            _argumentType = typeof(RegexValueArgument);
        }

        /// <summary>
        /// Creates new instance of RegexValueArgument. RegexValueArgumentAttribute
        /// uses underlying <see cref="RegexValueArgument"/>.
        /// </summary>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <param name="pattern">Regex pattern</param>
        public RegexValueArgumentAttribute(char shortName, string longName, string pattern)
            : base(typeof(RegexValueArgument), shortName, longName, new Regex(pattern))
        {
            _argumentType = typeof(RegexValueArgumentAttribute);
        }

        /// <summary>
        /// Default value
        /// </summary>
        public object DefaultValue
        {
            get
            {
                return _argumentType.GetPropertyValue<object>("DefaultValue", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("DefaultValue", Argument, value);
            }
        }

        /// <summary>
        /// Sample value that would be displayed to the user as a suggestion when 
        /// the user enters a wrong value. 
        /// </summary>
        public string SampleValue
        {
            get
            {
                return _argumentType.GetPropertyValue<string>("SampleValue", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("SampleValue", Argument, value);
            }
        }

        /// <summary>
        /// When set to true, argument can appear on the command line with or without value, e.g. both is allowed: 
        /// <code>
        /// myexe.exe -Arg Value
        /// OR
        /// myexe.exe -Arg
        /// </code>
        /// </summary>
        public bool ValueOptional
        {
            get
            {
                return _argumentType.GetPropertyValue<bool>("ValueOptional", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("ValueOptional", Argument, value);
            }
        }
    }
}
