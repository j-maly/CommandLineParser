using System;
using CommandLineParser.Compatibility;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Use BoundedValueArgument for an argument whose value must belong to an interval.
    /// </summary>
    /// <typeparam name="TValue">Type of the value, must support comparison</typeparam>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/BoundedValueArgument/*'/>
    public class BoundedValueArgument<TValue> : CertifiedValueArgument<TValue>
        where TValue : IComparable
    {
        #region min and max values

        private TValue _minValue;

        private TValue _maxValue;

        /// <summary>
        /// Minimal allowed value (inclusive)
        /// </summary>
        public TValue MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                UseMinValue = true;
            }
        }

        /// <summary>
        /// Maximal allowed value (inclusive) 
        /// </summary>
        public TValue MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                UseMaxValue = true;
            }
        }

        /// <summary>
        /// When set to true, value is checked for being greater than or equal to <see cref="MinValue"/>
        /// </summary>
        public bool UseMinValue { get; set; }

        /// <summary>
        /// When set to true, value is checked for being lesser than or equal to <see cref="MaxValue"/>
        /// </summary>
        public bool UseMaxValue { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>. 
        /// Without any value bounds.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        public BoundedValueArgument(char shortName) : base(shortName) { }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="longName">Long name of the argument</param>
        public BoundedValueArgument(string longName) : base(longName) { }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>
        /// and <see cref="Argument.LongName">long name</see>. Without any value bounds.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        public BoundedValueArgument(char shortName, string longName) : base(shortName, longName) { }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>
        /// and specified maximal value.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="maxValue">Maximal value of the argument</param>
        public BoundedValueArgument(char shortName, TValue maxValue) : base(shortName)
        {
            _maxValue = maxValue;
            UseMaxValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>
        /// and specified minimal value.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="minValue">Minimal value of the argument</param>
        public BoundedValueArgument(TValue minValue, char shortName) : base(shortName)
        {
            _minValue = minValue;
            UseMinValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>
        /// and specified minimal value and maximal value. 
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="minValue">Minimal value of the argument</param>
        /// <param name="maxValue">Maximal value of the argument</param>
        public BoundedValueArgument(char shortName, TValue minValue, TValue maxValue)
            : base(shortName)
        {
            _maxValue = maxValue;
            UseMaxValue = true;
            _minValue = minValue;
            UseMinValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see> and specified maximal value.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="maxValue">Maximal value of the argument</param>
        public BoundedValueArgument(char shortName, string longName, TValue maxValue)
            : base(shortName, longName)
        {
            _maxValue = maxValue;
            UseMaxValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see> and specified minimal value.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="minValue">minimal value of the argument</param>
        public BoundedValueArgument(TValue minValue, char shortName, string longName)
            : base(shortName, longName)
        {
            _minValue = minValue;
            UseMinValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see> and specified minimal and maximal value.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="maxValue">Maximal value of the argument</param>
        /// <param name="minValue">minimal value of the argument</param>
        public BoundedValueArgument(char shortName, string longName, TValue minValue, TValue maxValue)
            : base(shortName, longName)
        {
            _maxValue = maxValue;
            UseMaxValue = true;
            _minValue = minValue;
            UseMinValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see> and specified maximal value. 
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">Description of the argument</param>
        /// <param name="maxValue">Maximal value of the argument</param>
        public BoundedValueArgument(char shortName, string longName, string description, TValue maxValue)
            : base(shortName, longName, description)
        {
            _maxValue = maxValue;
            UseMaxValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see> and specified minimal value. 
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">Description of the argument</param>
        /// <param name="minValue">Minimal value of the argument</param>
        public BoundedValueArgument(TValue minValue, char shortName, string longName, string description)
            : base(shortName, longName, description)
        {
            _minValue = minValue;
            UseMinValue = true;
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see> and specified minimal and maximal value. 
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">Description of the argument</param>
        /// <param name="minValue">Minimal value of the argument</param>
        /// <param name="maxValue">Maximal value of the argument</param>
        public BoundedValueArgument(char shortName, string longName, string description, TValue minValue, TValue maxValue)
            : base(shortName, longName, description)
        {
            _maxValue = maxValue;
            UseMaxValue = true;
            _minValue = minValue;
            UseMinValue = true;
        }

        #endregion

        /// <summary>
        /// Checks whether the value belongs to the [<see cref="MinValue"/>, <see cref="MaxValue"/>] interval
        /// (when <see cref="UseMinValue"/> and <see cref="UseMaxValue"/> are set).
        /// </summary>
        /// <param name="value">value to certify</param>
        /// <exception cref="CommandLineArgumentOutOfRangeException">Thrown when <paramref name="value"/> lies outside the interval. </exception>
        protected override void Certify(TValue value)
        {
            if (UseMinValue && MinValue.CompareTo(value) == 1)
            {
                throw new CommandLineArgumentOutOfRangeException(
                    string.Format(Messages.EXC_ARG_BOUNDED_LESSER_THAN_MIN, value, _minValue), Name);
            }

            if (UseMaxValue && MaxValue.CompareTo(value) == -1)
            {
                throw new CommandLineArgumentOutOfRangeException(
                    string.Format(Messages.EXC_ARG_BOUNDED_GREATER_THAN_MAX, value, _maxValue), Name);
            }
        }
    }

    /// <summary>
    /// <para>
    /// Attribute for declaring a class' field a <see cref="BoundedValueArgument{TValue}"/> and 
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
    public class BoundedValueArgumentAttribute : ArgumentAttribute
    {
        private readonly Type _argumentType;

        /// <summary>
        /// Creates proper generic <see cref="BoundedValueArgument{TValue}"/> type for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">type of the argument value</param>
        /// <returns>generic type</returns>
        private static Type CreateProperValueArgumentType(Type type)
        {
            Type genericType = typeof(BoundedValueArgument<>);
            Type constructedType = genericType.MakeGenericType(type);
            return constructedType;
        }

        /// <summary>
        /// Creates new instance of BoundedValueArgument. BoundedValueArgument
        /// uses underlying <see cref="BoundedValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="BoundedValueArgument{TValue}"/>.</param>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public BoundedValueArgumentAttribute(Type type, char shortName)
            : base(CreateProperValueArgumentType(type), shortName)
        {
            _argumentType = CreateProperValueArgumentType(type);
        }

        /// <summary>
        /// Creates new instance of BoundedValueArgument. BoundedValueArgument
        /// uses underlying <see cref="BoundedValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="BoundedValueArgument{TValue}"/>.</param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public BoundedValueArgumentAttribute(Type type, string longName)
            : base(CreateProperValueArgumentType(type), longName)
        {
            _argumentType = CreateProperValueArgumentType(type);
        }

        /// <summary>
        /// Creates new instance of BoundedValueArgument. BoundedValueArgument
        /// uses underlying <see cref="BoundedValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="BoundedValueArgument{TValue}"/>.</param>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public BoundedValueArgumentAttribute(Type type, char shortName, string longName)
            : base(CreateProperValueArgumentType(type), shortName, longName)
        {
            _argumentType = CreateProperValueArgumentType(type);
        }

        /// <summary>
        /// Maximal allowed value (inclusive) 
        /// </summary>
        public object MaxValue
        {
            get
            {
                return _argumentType.GetPropertyValue<object>("MaxValue", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("MaxValue", Argument, value);
            }
        }

        /// <summary>
        /// Minimal allowed value (inclusive)
        /// </summary>
        public object MinValue
        {
            get
            {
                return _argumentType.GetPropertyValue<object>("MinValue", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("MinValue", Argument, value);
            }
        }

        /// <summary>
        /// When set to true, value is checked for being lesser than or equal to <see cref="MaxValue"/>
        /// </summary>
        public bool UseMaxValue
        {
            get
            {
                return _argumentType.GetPropertyValue<bool>("UseMaxValue", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("UseMaxValue", Argument, value);
            }
        }

        /// <summary>
        /// When set to true, value is checked for being greater than or equal to <see cref="MinValue"/>
        /// </summary>
        public bool UseMinValue
        {
            get
            {
                return _argumentType.GetPropertyValue<bool>("UseMinValue", Argument);
            }
            set
            {
                _argumentType.SetPropertyValue("UseMinValue", Argument, value);
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
    }
}