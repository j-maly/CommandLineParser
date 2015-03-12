using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using CommandLineParser.Exceptions;
using ReflectionBridge.Extensions;

#if ASPNETCORE50
using System.Linq;
#endif

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Delegate for a method that converts string to a specified class
    /// </summary>
    /// <typeparam name="TValue">type of the converted class</typeparam>
    /// <param name="stringValue">string that represents the value of TValue</param>
    /// <returns>TValue loaded from string</returns>
    public delegate TValue ConvertValueDelegate<TValue>(string stringValue);

    /// <summary>
    /// Use value argument for an argument followed by value on the command line. (e. g. --version 1.2)
    /// </summary>
    /// <typeparam name="TValue">type of the value of the argument. 
    /// Can be either builtin type or any user type (for which specific
    /// conversion routine is provided - <see cref="ConvertValueHandler"/></typeparam>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/ValueArgument/*'/>
    public class ValueArgument<TValue> : Argument, IValueArgument, IArgumentWithDefaultValue
    {
        #region property backing fields

        private string _stringValue;

        private TValue _value;

        private TValue _defaultValue;

        private readonly List<TValue> _values = new List<TValue>();

        private ConvertValueDelegate<TValue> _convertValueHandler;

        private CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        #endregion

        #region constructor

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        public ValueArgument(char shortName)
            : base(shortName)
        {
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        public ValueArgument(char shortName, string longName)
            : base(shortName, longName)
        {
        }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">description of the argument</param>
        public ValueArgument(char shortName, string longName, string description)
            : base(shortName, longName, description)
        {
        }

        #endregion

        /// <summary>
        /// String read from command line as arguments <see cref="Value"/>. Available after <see cref="Parse"/> is called. 
        /// </summary>
        /// <exception cref="InvalidOperationException">String value was read before ParseCommandLine was called or when</exception>
        public string StringValue
        {
            get
            {
                if (Parsed)
                    return _stringValue;
                else
                    return null;
            }
        }

        /// <summary>
        /// Value of the ValueArgument, for arguments with single value.
        /// Can be used only if <see cref="Argument.AllowMultiple"/> is set to false.
        /// </summary>
        public TValue Value
        {
            get
            {
                if (AllowMultiple)
                    throw new InvalidOperationException("Cannot access Value field because AllowMultiple is set to true. Use Values instead.");
                return _value;
            }
            set
            {
                if (AllowMultiple)
                    throw new InvalidOperationException("Cannot access Value field because AllowMultiple is set to true. Use Values instead.");
                this._value = value;
            }
        }

        /// <summary>
        /// Default value of the argument. Restored each time <see cref="Init"/> is called.
        /// </summary>
        public TValue DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        object IArgumentWithDefaultValue.DefaultValue { get { return DefaultValue; } }

        /// <summary>
        /// Values of the ValueArgument - for arguments with multiple values allowed. 
        /// Can be used only if <see cref="Argument.AllowMultiple"/> is set to true.
        /// </summary>
        public List<TValue> Values
        {
            get
            {
                if (!AllowMultiple)
                    throw new InvalidOperationException("Cannot access Values field because AllowMultiple is set to false. Use Value instead.");
                return _values;
            }
        }

        /// <summary>
        /// Value of the ValueArgument, for arguments with single value.
        /// Can be used only if <see cref="Argument.AllowMultiple"/> is set to false.
        /// </summary>
        /// <value></value>
        object IValueArgument.Value
        {
            get { return Value; }
            set { Value = (TValue)value; }
        }

        IList<object> IValueArgument.Values
        {
            get
            {
                object[] objects = new object[Values.Count];
                for (int i = 0; i < Values.Count; i++)
                {
                    objects[i] = Values[i];
                }
                return objects;
            }
        }

        /// <summary>
        /// Adds an item to underlying <see cref="Values"/> collection.
        /// </summary>
        public void AddToValues(object value)
        {
            Values.Add((TValue)value);
        }

        /// <summary>
        /// Function that converts string to <typeparamref name="TValue"/> type.
        /// Necessary when non-builtin type is used as <typeparamref name="TValue"/>.
        /// </summary>
        public ConvertValueDelegate<TValue> ConvertValueHandler
        {
            get { return _convertValueHandler; }
            set { _convertValueHandler = value; }
        }

        /// <summary>
        /// This method reads the argument and the following string representing the value of the argument. 
        /// This string is then converted to <typeparamref name="TValue"/> (using built-in <typeparamref name="TValue"/>.Parse
        /// method for built-in types or using <see cref="ConvertValueHandler"/> for user types).
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="i">index to the args array, where this argument occured. 
        /// The index to the next argument after the argument is processed. </param>
        /// <seealso cref="ConvertValueHandler"/>
        /// <exception cref="CommandLineArgumentException">Incorrect format of the command line
        /// or multiple useage of an argument that is not <see cref="Argument.AllowMultiple"/> found.</exception>
        internal override void Parse(IList<string> args, ref int i)
        {
            base.Parse(args, ref i);

            i++; // move the cursor to the value EXC_ARG_VALUE_MISSING2
            if (args.Count - 1 < i)
            {
                throw new CommandLineArgumentException(string.Format(Messages.EXC_ARG_VALUE_MISSING2, Name), Name);
            }
            if (args[i].StartsWith("-"))
            {
                throw new CommandLineArgumentException(string.Format(Messages.EXC_ARG_VALUE_MISSING, Name, args[i]), Name);
            }
            _stringValue = args[i];

            if (!AllowMultiple)
                Value = Convert(_stringValue);
            else
                Values.Add(Convert(_stringValue));

            Parsed = true;
            i++; // move to the next argument 
        }

        /// <summary>
        /// If <see cref="Argument.Bind"/> is specified, the bound field of the bound object should is updated
        /// according to the value of the argument. Should be called after Parse is called. If 
        /// <see cref="Argument.AllowMultiple"/> is set to false, the binding fills a collection or an array with the 
        /// values. 
        /// </summary>
        public override void UpdateBoundObject()
        {
            if (Bind == null)
                return;

            MemberInfo info = Bind.Value.Object.GetType().GetMember(Bind.Value.Field)[0];
            Type boundType = null;
            if (info is PropertyInfo) boundType = (info as PropertyInfo).PropertyType;
            else if (info is FieldInfo) boundType = (info as FieldInfo).FieldType;


            //multiple value argument can be bound only to a proper collection
            if (AllowMultiple
                && (!(typeof(List<TValue>).IsAssignableFrom(boundType)))
                && !boundType.IsArray)
            {
                throw new InvalidOperationException("ValueArguments that allow multiple values can be bound only to an object implementing ICollection<TVale> and the collection must not be read only.");
            }

            try
            {
                if (!AllowMultiple || boundType.IsArray)
                {
                    // binding of a simple value or array
                    object newValue;
                    if (!boundType.IsArray)
                        newValue = _value;
                    else
                        newValue = Values.ToArray();
                    if (info is FieldInfo)
                        (info as FieldInfo).SetValue(Bind.Value.Object, newValue);
                    if (info is PropertyInfo)
                        (info as PropertyInfo).SetValue(Bind.Value.Object, newValue, null);
                }
                else
                {
                    ICollection<TValue> targetCollection = null;
                    if (info is FieldInfo)
                        targetCollection = (ICollection<TValue>)(info as FieldInfo).GetValue(Bind.Value.Object);
                    else if (info is PropertyInfo)
                        targetCollection = (ICollection<TValue>)(info as PropertyInfo).GetValue(Bind.Value.Object, null);

                    targetCollection.Clear();
                    foreach (TValue value in _values)
                    {
                        targetCollection.Add(value);
                    }
                }
            }
            catch (Exception e)
            {
                throw new CommandLineException(string.Format(Messages.EXC_BINDING, Name, Bind.Value.Field, Bind.Value.Object), e);
            }
        }

        /// <summary>
        /// Culture used for conversions of built-in types. InvariantCulture is used when 
        /// no other culture is specified. 
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }

        /// <summary>
        /// Converts <paramref name="stringValue"/> to <typeparamref name="TValue"/>.
        /// <see cref="ConvertValueHandler"/> is called if specified, otherwise
        /// <see cref="DefaultConvert"/> is called.
        /// </summary>
        /// <param name="stringValue">string representing the value</param>
        /// <returns>value as <typeparamref name="TValue"/></returns>
        /// <exception cref="InvalidConversionException">String cannot be converted to <typeparamref name="TValue"/>.</exception>
        public virtual TValue Convert(string stringValue)
        {
            TValue tmpValue;
            if (_convertValueHandler != null)
            {
                tmpValue = _convertValueHandler(stringValue);
            }
            else
            {
                tmpValue = DefaultConvert(stringValue);
            }
            return tmpValue;
        }

        /// <summary>
		/// Non-generic access to <see cref="ValueArgument{TValue}.Convert"/>
		/// </summary>
		/// <param name="stringValue">string representing the value</param>
		/// <returns>parsed value</returns>
		public object Convert_obj(string stringValue)
        {
            return Convert(stringValue);
        }

        /// <summary>
        /// Converts <paramref name="stringValue"/> to <typeparamref name="TValue"/>.
        /// Works only for built-in types. 
        /// </summary>
        /// <param name="stringValue">string representing the value</param>
        /// <returns>value as <typeparamref name="TValue"/></returns>
        /// <exception cref="InvalidConversionException">String cannot be converted to <typeparamref name="TValue"/>.</exception>
        protected virtual TValue DefaultConvert(string stringValue)
        {
            Type valueType = typeof(TValue);
            try
            {
                if (valueType == typeof(String)) return (TValue)(object)stringValue;
                if (valueType == typeof(int)) return (TValue)(object)int.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(decimal)) return (TValue)(object)decimal.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(long)) return (TValue)(object)long.Parse(stringValue, _cultureInfo);
                if (typeof(Enum).IsAssignableFrom(valueType)) return (TValue)Enum.Parse(valueType, stringValue);
                if (valueType == typeof(short)) return (TValue)(object)short.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(char)) return (TValue)(object)char.Parse(stringValue);
                if (valueType == typeof(bool)) return (TValue)(object)bool.Parse(stringValue);
                if (valueType == typeof(byte)) return (TValue)(object)byte.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(sbyte)) return (TValue)(object)sbyte.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(double)) return (TValue)(object)double.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(float)) return (TValue)(object)float.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(uint)) return (TValue)(object)uint.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(ulong)) return (TValue)(object)ulong.Parse(stringValue, _cultureInfo);
                if (valueType == typeof(ushort)) return (TValue)(object)ushort.Parse(stringValue, _cultureInfo);

                MethodInfo mi = typeof(TValue).GetMethod("Parse");
                if (mi != null)
                {
                    ParameterInfo[] pi = mi.GetParameters();
                    if (mi.IsStatic
                        && pi.Length == 2
                        && pi[0].ParameterType == typeof(string)
                        && pi[1].ParameterType == typeof(CultureInfo)
                        && mi.ReturnType == typeof(TValue))
                        return (TValue)mi.Invoke(null, new object[] { stringValue, _cultureInfo });
                }
            }
            catch (FormatException)
            {
                throw new InvalidConversionException(string.Format(Messages.EXC_ARG_VALUE_STANDARD_CONVERT_FAILED, stringValue, valueType), Name);
            }

            throw new InvalidConversionException(string.Format(Messages.EXC_ARG_VALUE_USER_CONVERT_MISSING, valueType, stringValue), Name);
        }

        /// <summary>
        /// Initializes the argument.
        /// </summary>
        public override void Init()
        {
            base.Init();
            _value = DefaultValue;
            _values.Clear();
            _stringValue = string.Empty;
        }

        /// <summary>
        /// Prints information about the argument value to the console.
        /// </summary>
        internal override void PrintValueInfo()
        {
            if (!AllowMultiple)
                Console.WriteLine(Messages.EXC_ARG_VALUE_PRINT, Name, _stringValue, _value, typeof(TValue).Name);
            else
            {
                string valuesString = String.Empty;
                foreach (TValue tvalue in _values)
                {
                    valuesString += tvalue.ToString();

                }
                Console.WriteLine(Messages.EXC_ARG_VALUE_PRINT_MULTIPLE, Name, Values.Count, valuesString, typeof(TValue).Name);
            }
        }
    }

    /// <summary>
    /// <para>
    /// Attribute for declaring a class' field a <see cref="ValueArgument{TValue}"/> and 
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
    public class ValueArgumentAttribute : ArgumentAttribute
    {
        private static Type underlyingValueArgument;

        /// <summary>
        /// Creates proper generic <see cref="ValueArgument{TValue}"/> type for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">type of the argument value</param>
        /// <returns>generic type</returns>
        private static Type CreateProperValueArgumentType(Type type)
        {
            Type genericType = typeof(ValueArgument<>);
            Type constructedType = genericType.MakeGenericType(type);
            underlyingValueArgument = constructedType;
            return underlyingValueArgument;
        }

        /// <summary>
        /// Creates new instance of ValueArgument. ValueArgument
        /// uses underlaying <see cref="ValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="ValueArgument{TValue}"/>. </param>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public ValueArgumentAttribute(Type type, char shortName)
            : base(CreateProperValueArgumentType(type), shortName)
        {
        }

        /// <summary>
        /// Creates new instance of ValueArgument. ValueArgument
        /// uses underlaying <see cref="ValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="ValueArgument{TValue}"/>.</param>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public ValueArgumentAttribute(Type type, char shortName, string longName)
            : base(CreateProperValueArgumentType(type), shortName, longName)
        {
        }

        /// <summary>
        /// Default value
        /// </summary>
        public object DefaultValue
        {
            get
            {
                return underlyingValueArgument.GetPropertyValue<object>("DefaultValue", Argument);
            }
            set
            {
                underlyingValueArgument.SetPropertyValue("DefaultValue", Argument, value);
            }       
        }
    }
}