using System;
using System.Collections.Generic;
using CommandLineParser.Compatibility;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Switch argument can be used to represent options with <c>true</c>/<c>false</c> logic. It is initialized with default value and
    /// when the argument appears on the command line, the value is flipped. 
    /// </summary>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/SwitchArgument/*'/>
    public class SwitchArgument : Argument, IArgumentWithDefaultValue
    {
        #region property backing fieldds

        #endregion

        #region constructors

        /// <summary>
        /// Creates new switch argument with a <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="longName">Long name of the argument</param>
        public SwitchArgument(string longName, bool defaultValue) : base(longName)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        /// <summary>
        /// Creates new switch argument with a <see cref="Argument.ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="defaultValue">default value of the argument</param>
        public SwitchArgument(char shortName, bool defaultValue) : base(shortName)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        /// <summary>
        /// Creates new switch argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="defaultValue">default value of the argument</param>
        public SwitchArgument(char shortName, string longName, bool defaultValue) : base(shortName, longName)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        /// <summary>
        /// Creates new switch argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">description of the argument</param>
        /// <param name="defaultValue">default value of the argument</param>
        public SwitchArgument(char shortName, string longName, string description, bool defaultValue) : base(shortName, longName, description)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        #endregion

        /// <summary>
        /// Value of the switch argument
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Default value of the switch argument. Restored each time <see cref="Init"/> is called.
        /// </summary>
        public bool DefaultValue { get; set; }

        object IArgumentWithDefaultValue.DefaultValue { get { return DefaultValue; } }

        /// <summary>
        /// Parse argument. This method reads the argument from the input field and moves the 
        /// index to the next argument.
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="i">index to the args array, where this argument occurred. </param>
        public override void Parse(IList<string> args, ref int i)
        {
            base.Parse(args, ref i);
            Value = !Value;
            Parsed = true;
            i++;
        }

        /// <summary>
        /// If <see cref="Argument.Bind"/> is specified, the bound field of the bound object should is updated
        /// according to the value of the argument. Should be called after Parse is called.  
        /// </summary>
        public override void UpdateBoundObject()
        {
            if (Bind.HasValue)
            {
                try
                {
                    Bind.Value.Object.GetType().SetFieldValue(Bind.Value.Field, Bind.Value.Object, Value);
                }
                catch (Exception e)
                {
                    throw new CommandLineException(string.Format(Messages.EXC_BINDING, Name, Bind.Value.Field, Bind.Value.Object), e);
                }
            }
        }

        /// <summary>
        /// Prints information about the argument value to the console.
        /// </summary>
        public override void PrintValueInfo()
        {
            Console.WriteLine(Messages.EXC_ARG_SWITCH_PRINT, Name, Value ? "1" : "0");
        }

        /// <summary>
        /// Initializes the argument and restores the <see cref="DefaultValue"/>.
        /// </summary>
        public override void Init()
        {
            base.Init();
            Value = DefaultValue;
        }
    }

    /// <summary>
    /// <para>
    /// Attribute for declaring a class' field a <see cref="SwitchArgument"/> and 
    /// thus binding a field's value to a certain command line switch argument.
    /// </para>
    /// <para>
    /// Instead of creating an argument explicitly, you can assign a class' field an argument
    /// attribute and let the CommandLineParse take care of binding the attribute to the field.
    /// </para>
    /// </summary>
    /// <remarks>Applicable to fields and properties (public).</remarks>
    /// <remarks>Use <see cref="CommandLineParser.ExtractArgumentAttributes"/> for each object 
    /// you where you have declared argument attributes.</remarks>
    public class SwitchArgumentAttribute : ArgumentAttribute
    {
        /// <summary>
        /// Creates new instance of SwitchArgumentAttribute.
        /// </summary>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="defaultValue"><see cref="SwitchArgument.DefaultValue">default value</see> of the underlying argument</param>
        public SwitchArgumentAttribute(char shortName, bool defaultValue) 
            : base(typeof(SwitchArgument), shortName, defaultValue) { }

        /// <summary>
        /// Creates new instance of SwitchArgumentAttribute.
        /// </summary>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <param name="defaultValue"><see cref="SwitchArgument.DefaultValue">default value</see> of the underlying argument</param>
        public SwitchArgumentAttribute(string longName, bool defaultValue) 
            : base(typeof(SwitchArgument), longName, defaultValue) { }

        /// <summary>
        /// Creates new instance of SwitchArgumentAttribute.
        /// </summary>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <param name="defaultValue"><see cref="SwitchArgument.DefaultValue">default value</see> of the underlying argument</param>
        public SwitchArgumentAttribute(char shortName, string longName, bool defaultValue)
            : base(typeof(SwitchArgument), shortName, longName, defaultValue) { }
    }
}