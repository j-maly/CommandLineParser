using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using CommandLineParser.Compatibility;
using CommandLineParser.Exceptions;

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

        private readonly List<TValue> _values = new List<TValue>();

        private CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        #endregion

        #region constructor

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        public ValueArgument(char shortName) : base(shortName) { }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="longName">Long name of the argument</param>
        public ValueArgument(string longName) : base(longName) { }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        public ValueArgument(char shortName, string longName) : base(shortName, longName) { }

        /// <summary>
        /// Creates new value argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">description of the argument</param>
        public ValueArgument(char shortName, string longName, string description)
            : base(shortName, longName, description) { }

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
                _value = value;
            }
        }

        /// <summary>
        /// Default value of the argument. Restored each time <see cref="Init"/> is called.
        /// </summary>
        public TValue DefaultValue { get; set; }

        object IArgumentWithDefaultValue.DefaultValue { get { return DefaultValue; } }

        /// <summary>
        /// When set to true, argument can appear on the command line with or without value, e.g. both is allowed: 
        /// <code>
        /// myexe.exe -Arg Value
        /// OR
        /// myexe.exe -Arg
        /// </code>
        /// </summary>
        public bool ValueOptional { get; set; }

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
        public ConvertValueDelegate<TValue> ConvertValueHandler { get; set; }

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
        public override void Parse(IList<string> args, ref int i)
        {
            base.Parse(args, ref i);

            i++; // move the cursor to the value
            if (args.Count - 1 < i)
            {
                if (ValueOptional)
                {
                    Parsed = true;
                    return;
                }
                throw new CommandLineArgumentException(string.Format(Messages.EXC_ARG_VALUE_MISSING2, Name), Name);
            }

            _stringValue = args[i];
            bool canParse = true;
            try { Convert(_stringValue); }
            catch { canParse = false; }

            if (!canParse && args[i].StartsWith("-"))
            {
                if (ValueOptional)
                {
                    Parsed = true;
                    return;
                }
                throw new CommandLineArgumentException(string.Format(Messages.EXC_ARG_VALUE_MISSING, Name, args[i]), Name);
            }

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
                    ICollection<TValue> targetCollection = InitializeTargetCollection(info);
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

        private ICollection<TValue> InitializeTargetCollection(MemberInfo info)
        {
            ICollection<TValue> targetCollection = null;
            if (info is FieldInfo)
                targetCollection = (ICollection<TValue>)(info as FieldInfo).GetValue(Bind.Value.Object);
            else if (info is PropertyInfo)
                targetCollection = (ICollection<TValue>)(info as PropertyInfo).GetValue(Bind.Value.Object, null);

            if (targetCollection == null)
            {
                if (info is FieldInfo)
                {
                    targetCollection = (ICollection<TValue>)Activator.CreateInstance((info as FieldInfo).FieldType);
                    (info as FieldInfo).SetValue(Bind.Value.Object, targetCollection);
                }
                else if (info is PropertyInfo)
                {
                    targetCollection = (ICollection<TValue>)Activator.CreateInstance((info as PropertyInfo).PropertyType);
                    (info as PropertyInfo).SetValue(Bind.Value.Object, targetCollection, null);
                }
            }
            targetCollection.Clear();
            return targetCollection;
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
            if (ConvertValueHandler != null)
            {
                tmpValue = ConvertValueHandler(stringValue);
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
            // If not provided, just return default value.
            if (stringValue == null)
            {
                return default(TValue);
            }

            Type valueType = typeof(TValue);

            // If the TValue is a nullable type, get the underlying type.
            if (valueType.GetTypeInfo().IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                valueType = Nullable.GetUnderlyingType(valueType);
            }

            try
            {
                if (valueType == typeof(String))
                {
                    return (TValue)(object)stringValue;
                }
                if (typeof(Enum).IsAssignableFrom(valueType))
                {
                    return (TValue)Enum.Parse(valueType, stringValue, true);
                }
                if (valueType == typeof(Guid))
                {
#if NET20 || NET35                           
                    return (TValue)(object)new Guid(stringValue);
#else
                    return (TValue)(object)Guid.Parse(stringValue);
#endif                    
                }

                MethodInfo parseMethod2 = valueType.GetMethod("Parse", new[] { typeof(string), typeof(CultureInfo) });
                if (parseMethod2 != null)
                {
                    if (parseMethod2.IsStatic && parseMethod2.ReturnType == valueType)
                    {
                        return (TValue)parseMethod2.Invoke(null, new object[] { stringValue, _cultureInfo });
                    }
                }

                MethodInfo parseMethod1 = valueType.GetMethod("Parse", new[] { typeof(string) });
                if (parseMethod1 != null)
                {
                    if (parseMethod1.IsStatic && parseMethod1.ReturnType == valueType)
                    {
                        return (TValue)parseMethod1.Invoke(null, new object[] { stringValue });
                    }
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
        public override void PrintValueInfo()
        {
            if (!AllowMultiple)
            {
                Console.WriteLine(Messages.EXC_ARG_VALUE_PRINT, Name, _stringValue, _value, typeof(TValue).Name);
            }
            else
            {
                string valuesString = string.Join(", ", _values.Select(v => v.ToString()).ToArray());
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
        /// uses underlying <see cref="ValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="ValueArgument{TValue}"/>. </param>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public ValueArgumentAttribute(Type type, char shortName)
            : base(CreateProperValueArgumentType(type), shortName) { }

        /// <summary>
        /// Creates new instance of ValueArgument. ValueArgument
        /// uses underlying <see cref="ValueArgument{TValue}"/>.
        /// </summary>
        /// <param name="type">Type of the generic parameter of <see cref="ValueArgument{TValue}"/>. </param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <remarks>
        /// Parameter <paramref name="type"/> has to be either built-in 
        /// type or has to define a static Parse(String, CultureInfo) 
        /// method for reading the value from string.
        /// </remarks>
        public ValueArgumentAttribute(Type type, string longName)
            : base(CreateProperValueArgumentType(type), longName) { }

        /// <summary>
        /// Creates new instance of ValueArgument. ValueArgument
        /// uses underlying <see cref="ValueArgument{TValue}"/>.
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
            : base(CreateProperValueArgumentType(type), shortName, longName) { }

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
                return underlyingValueArgument.GetPropertyValue<bool>("ValueOptional", Argument);
            }
            set
            {
                underlyingValueArgument.SetPropertyValue("ValueOptional", Argument, value);
            }
        }
    }
}