using System;
using System.Collections.Generic;
using CommandLineParser.Exceptions;
using ReflectionBridge.Extensions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Use EnumeratedValueArgument for an argument whose values must be from certain finite set 
    /// (see <see cref="AllowedValues"/>)
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/EnumeratedValueArgument/*'/>
    public class EnumeratedValueArgument<TValue> :
        CertifiedValueArgument<TValue>
    {
        private ICollection<TValue> _allowedValues;

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
        internal override void Certify(TValue value)
        {
            if (!_allowedValues.Contains(Value))
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
    /// <example>
    /// <code source="Examples\AttributeExample.cs" lang="cs" title="Example of declaring argument attributes" />
    /// </example>
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
		/// uses underlaying <see cref="EnumeratedValueArgument{TValue}"/>.
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
		/// Creates new instance of EnumeratedValueArgument. EnumeratedValueArgument
		/// uses underlaying <see cref="EnumeratedValueArgument{TValue}"/>.
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
    }
}