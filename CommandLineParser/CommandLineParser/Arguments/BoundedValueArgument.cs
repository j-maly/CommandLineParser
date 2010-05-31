using System;
using System.Reflection;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Use BoundedValueArgument for an argument whose value must belong to an interval.
    /// </summary>
    /// <typeparam name="TValue">Type of the value, must support comparison</typeparam>
    /// <include file='Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/BoundedValueArgument/*'/>
    public class BoundedValueArgument<TValue> : CertifiedValueArgument<TValue> 
        where TValue : IComparable
        
    {
        #region min and max values

        private TValue minValue = default(TValue);

        private TValue maxValue = default(TValue);

        private bool useMinValue = false;

        private bool useMaxValue = false; 

        /// <summary>
        /// Minimal allowed value (inclusive)
        /// </summary>
        public TValue MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                UseMinValue = true;
            }
        }

        /// <summary>
        /// Maximal allowed value (inclusive) 
        /// </summary>
        public TValue MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                UseMaxValue = true;
            }
        }

        /// <summary>
        /// When set to true, value is checked for being greater than or equal to <see cref="MinValue"/>
        /// </summary>
        public bool UseMinValue
        {
            get { return useMinValue; }
            set { useMinValue = value; }
        }

        /// <summary>
        /// When set to true, value is checked for being lesser than or equal to <see cref="MaxValue"/>
        /// </summary>
        public bool UseMaxValue
        {
            get { return useMaxValue; }
            set { useMaxValue = value; }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>. 
        /// Without any value bounds.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        public BoundedValueArgument(char shortName)
            : base(shortName)
        {
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>
        /// and <see cref="Argument.LongName">long name</see>. Without any value bounds.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        public BoundedValueArgument(char shortName, string longName)
            : base(shortName, longName)
        {
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>
        /// and specified maximal value.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="maxValue">Maximal value of the argument</param>
        public BoundedValueArgument(char shortName, TValue maxValue)
            : base(shortName)
        {
            this.maxValue = maxValue;
            useMaxValue = true; 
        }


        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>
        /// and specified minimal value.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="minValue">Minimal value of the argument</param>
        public BoundedValueArgument(TValue minValue, char shortName)
            : base(shortName)
        {
            this.minValue = minValue;
            useMinValue = true; 
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
            this.maxValue = maxValue;
            useMaxValue = true; 
            this.minValue = minValue;
            useMinValue = true; 
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
            this.maxValue = maxValue;
            useMaxValue = true; 
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
            this.minValue = minValue;
            useMinValue = true; 
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
            this.maxValue = maxValue;
            useMaxValue = true; 
            this.minValue = minValue;
            useMinValue = true; 
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
            this.maxValue = maxValue;
            useMaxValue = true; 
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
            this.minValue = minValue;
            useMinValue = true; 
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
            this.maxValue = maxValue;
            useMaxValue = true; 
            this.minValue = minValue;
            useMinValue = true; 
        }

        #endregion

        /// <summary>
        /// Checks whether the value belongs to the [<see cref="MinValue"/>, <see cref="MaxValue"/>] interval
        /// (when <see cref="UseMinValue"/> and <see cref="UseMaxValue"/> are set).
        /// </summary>
        /// <param name="value">value to certify</param>
        /// <exception cref="CommandLineArgumentOutOfRangeException">Thrown when <paramref name="value"/> lies outside the interval. </exception>
        internal override void Certify(TValue value)
        {
            if (useMinValue && MinValue.CompareTo(value) == 1)
            {
                throw new CommandLineArgumentOutOfRangeException(
                    string.Format(Messages.EXC_ARG_BOUNDED_LESSER_THAN_MIN, value, minValue), Name);
            }

            if (useMaxValue && MaxValue.CompareTo(value) == -1)
            {
                throw new CommandLineArgumentOutOfRangeException(
                    string.Format(Messages.EXC_ARG_BOUNDED_GREATER_THAN_MAX, value, maxValue), Name);
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
    /// <example>
    /// <code source="Examples\AttributeExample.cs" lang="cs" title="Example of declaring argument attributes" />
    /// </example>
    public class BoundedValueArgumentAttribute : ArgumentAttribute
    {
        private readonly Type argumentType;

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
        /// uses underlaying <see cref="BoundedValueArgument{TValue}"/>.
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
            argumentType = CreateProperValueArgumentType(type);
        }

        /// <summary>
        /// Creates new instance of BoundedValueArgument. BoundedValueArgument
        /// uses underlaying <see cref="BoundedValueArgument{TValue}"/>.
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
            argumentType = CreateProperValueArgumentType(type);
        }     

        /// <summary>
        /// Maximal allowed value (inclusive) 
        /// </summary>
        public object MaxValue
        {
            get
            {
                return argumentType.InvokeMember("MaxValue", BindingFlags.GetProperty, null, Argument, null);
            }
            set
            {
                argumentType.InvokeMember("MaxValue", BindingFlags.SetProperty, null, Argument, new[] { value });
            }
        }
        /// <summary>
        /// Minimal allowed value (inclusive)
        /// </summary>
        public object MinValue
        {
            get
            {
                return argumentType.InvokeMember("MinValue", BindingFlags.GetProperty, null, Argument, null);
            }
            set
            {
                argumentType.InvokeMember("MinValue", BindingFlags.SetProperty, null, Argument, new[]{ value });
            }
        }

        /// <summary>
        /// When set to true, value is checked for being lesser than or equal to <see cref="MaxValue"/>
        /// </summary>
        public bool UseMaxValue
        {
            get
            {
                return (bool)argumentType.InvokeMember("UseMaxValue", BindingFlags.GetProperty, null, Argument, null);
            }
            set
            {
                argumentType.InvokeMember("UseMaxValue", BindingFlags.SetProperty, null, Argument, new object[] { value });
            }
        }

        /// <summary>
        /// When set to true, value is checked for being greater than or equal to <see cref="MinValue"/>
        /// </summary>
        public bool UseMinValue
        {
            get
            {
                return (bool) argumentType.InvokeMember("UseMinValue", BindingFlags.GetProperty, null, Argument, null);
            }
            set
            {
                argumentType.InvokeMember("UseMinValue", BindingFlags.SetProperty, null, Argument, new object[] { value });
            }
        }
    }
}