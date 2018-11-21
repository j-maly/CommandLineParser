using System;
using System.Collections.Generic;
using System.Linq;
using CommandLineParser.Compatibility;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Use EnumeratedValueArgument for an argument whose values must be from certain finite set 
    /// (see <see cref="AllowedValues"/>)
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/EnumeratedValueArgument/*'/>
    public class EnumeratedValueArgument<TValue> : CertifiedValueArgument<TValue>
    {
        private ICollection<TValue> _allowedValues;
        private bool _ignoreCase;

        /// <summary>
        /// Set of values that are allowed for the argument.
        /// </summary>
        public ICollection<TValue> AllowedValues
        {
            get { return _allowedValues; }
            set { _allowedValues = value; }
        }

        /// <summary>
        /// Initilazes <see cref="AllowedValues"/> by a string of values separated by commas or semicolons.
        /// </summary>
        /// <param name="valuesString">Allowed values (separated by comas or semicolons)</param>
        public void InitAllowedValues(string valuesString)
        {
            string[] splitted = valuesString.Split(';', ',');
            TValue[] typedValues = new TValue[splitted.Length];
            int i = 0;
            foreach (string value in splitted)
            {
                typedValues[i] = Convert(value);
                i++;
            }
            AllowedValues = typedValues;
        }

        /// <summary>
        /// String arguments will be accepted even with differences in capitalisation (e.g. INFO will be accepted for info).
        /// </summary>
        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set
            {
                if (!(typeof(TValue) == typeof(string)) && value)
                {
                    throw new ArgumentException(string.Format("Ignore case can be used only for string arguments, type of TValue is {0}", typeof(TValue)));
                }
                _ignoreCase = value;
            }
        }

        #region constructor

        /// <summary>
        /// Creates new command line argument with a <see cref="Argument.ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        public EnumeratedValueArgument(char shortName)
            : base(shortName)
        {
            _allowedValues = new TValue[0];
        }

        /// <summary>
        /// Creates new command line argument with a <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="longName">Long name of the argument</param>
        public EnumeratedValueArgument(string longName) : base(longName) { }

        /// <summary>
        /// Creates new command line argument with a <see cref="Argument.ShortName">short name</see>
        /// and <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        public EnumeratedValueArgument(char shortName, string longName)
            : base(shortName, longName)
        {
            _allowedValues = new TValue[0];
        }

        /// <summary>
        /// Creates new command line argument with a <see cref="Argument.ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="allowedValues">Allowed values</param>
        public EnumeratedValueArgument(char shortName, ICollection<TValue> allowedValues)
            : base(shortName)
        {
            _allowedValues = allowedValues;
        }

        /// <summary>
        /// Creates new command line argument with a <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="longName">Short name of the argument</param>
        /// <param name="allowedValues">Allowed values</param>
        public EnumeratedValueArgument(string longName, ICollection<TValue> allowedValues)
            : base(longName)
        {
            _allowedValues = allowedValues;
        }

        /// <summary>
        /// Creates new command line argument with a <see cref="Argument.ShortName">short name</see>
        /// and <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="allowedValues">Allowed values</param>
        public EnumeratedValueArgument(char shortName, string longName, ICollection<TValue> allowedValues)
            : base(shortName, longName)
        {
            _allowedValues = allowedValues;
        }

        /// <summary>
        /// Creates new command line argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">Description of the argument</param>
        /// <param name="allowedValues">Allowed values</param>
        public EnumeratedValueArgument(char shortName, string longName, string description, ICollection<TValue> allowedValues)
            : base(shortName, longName, description)
        {
            _allowedValues = allowedValues;
        }

        #endregion

        /// <summary>
        /// Checks whether the specified value belongs to 
        /// the set of <see cref="AllowedValues">allowed values</see>. 
        /// </summary>
        /// <param name="value">value to certify</param>
        /// <exception cref="CommandLineArgumentOutOfRangeException">thrown when <paramref name="value"/> does not belong to the set of allowed values.</exception>
        protected override void Certify(TValue value)
        {
            bool ok;
            if (IgnoreCase && typeof(TValue) == typeof(string) && value is string)
            {
                TValue found = _allowedValues.FirstOrDefault(av => StringComparer.CurrentCultureIgnoreCase.Compare(value.ToString(), av.ToString()) == 0);
                ok = found != null;
                if (ok)
                {
                    base.Value = found;
                }
            }
            else
            {
                ok = _allowedValues.Contains(value);
            }
            if (!ok)
                throw new CommandLineArgumentOutOfRangeException(String.Format(
                                                                     Messages.EXC_ARG_ENUM_OUT_OF_RANGE, Value,
                                                                     Name), Name);
        }
    }

    /// <summary>
    /// <para>
    /// Attribute for declaring a class' field a <see cref="EnumeratedValueArgument{TValue}"/> and 
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
    public sealed class EnumeratedValueArgumentAttribute : ArgumentAttribute
    {
        private readonly Type _argumentType;

        /// <summary>
        /// Creates proper generic <see cref="BoundedValueArgument{TValue}"/> type for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">type of the argument value</param>
        /// <returns>generic type</returns>
        private static Type CreateProperValueArgumentType(Type type)
        {
            Type genericType = typeof(EnumeratedValueArgument<>);
            Type constructedType = genericType.MakeGenericType(type);
            return constructedType;
        }

        /// <summary>
		/// Creates new instance of EnumeratedValueArgumentAttribute. EnumeratedValueArgument
		/// uses underlying <see cref="EnumeratedValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="EnumeratedValueArgument{TValue}"/>.</param>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public EnumeratedValueArgumentAttribute(Type type, char shortName)
            : base(CreateProperValueArgumentType(type), shortName)
        {
            _argumentType = CreateProperValueArgumentType(type);
        }

        /// <summary>
		/// Creates new instance of EnumeratedValueArgumentAttribute. EnumeratedValueArgument
		/// uses underlying <see cref="EnumeratedValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="EnumeratedValueArgument{TValue}"/>.</param>
        /// <param name="longName"><see cref="Argument.LongName">short name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public EnumeratedValueArgumentAttribute(Type type, string longName)
            : base(CreateProperValueArgumentType(type), longName)
        {
            _argumentType = CreateProperValueArgumentType(type);
        }

        /// <summary>
		/// Creates new instance of EnumeratedValueArgument. EnumeratedValueArgument
		/// uses underlying <see cref="EnumeratedValueArgument{TValue}"/>.
        /// </summary>
		/// <param name="type">Type of the generic parameter of <see cref="EnumeratedValueArgument{TValue}"/>.</param>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public EnumeratedValueArgumentAttribute(Type type, char shortName, string longName)
            : base(CreateProperValueArgumentType(type), shortName, longName)
        {
            _argumentType = CreateProperValueArgumentType(type);
        }

        /// <summary>
        /// Allowed values of the argument, separated by commas or semicolons.
        /// </summary>
        public string AllowedValues
        {
            get
            {
                return string.Empty;
            }
            set
            {
                _argumentType.InvokeMethod("InitAllowedValues", Argument, value);
            }
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

        /// <summary>
        /// String arguments will be accepted even with differences in capitalisation (e.g. INFO will be accepted for info).
        /// </summary>
        public bool IgnoreCase
        {
            get
            {
                return _argumentType.GetPropertyValue<bool>("IgnoreCase", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("IgnoreCase", Argument, value);
            }
        }
    }
}