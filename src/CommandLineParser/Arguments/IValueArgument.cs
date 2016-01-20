using System;
using System.Collections.Generic;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Interface for non-generic access to <see cref="ValueArgument{TValue}"/> fields.
    /// </summary>
    public interface IValueArgument
    {
        /// <summary>
        /// String read from command line as arguments <see cref="Value"/>. 
        /// Available after <see cref="ValueArgument{TValue}"/>.<see cref="ValueArgument{TValue}.Parse"/> is called. 
        /// </summary>
        /// <exception cref="InvalidOperationException">String value was read before ParseCommandLine was called or when</exception>
        string StringValue { get; }

        /// <summary>
        /// Specifies whether argument can appear multiple times on the command line. 
        /// Default is false; 
        /// </summary>
        bool AllowMultiple { get; set; }

        /// <summary>
        /// Mark argument optional.
        /// <see cref="CommandLineParser.CheckMandatoryArguments"/>
        /// <remarks>Default is true</remarks>
        /// </summary>
        bool Optional { get; set; }

        /// <summary>
        /// Value of the ValueArgument, for arguments with single value.
        /// Can be used only if <see cref="AllowMultiple"/> is set to false.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Values of the ValueArgument - for arguments with multiple values allowed. 
        /// Can be used only if <see cref="Argument.AllowMultiple"/> is set to true.
        /// </summary>
        IList<object> Values { get; }

        /// <summary>
        /// Adds an item to underlying <see cref="Values"/> collection.
        /// </summary>
	    void AddToValues(object value);

        /// <summary>
        /// Non-generic access to <see cref="ValueArgument{TValue}.Convert"/>
        /// </summary>
        /// <param name="stringValue">string representing the value</param>
        /// <returns>parsed value</returns>
        object Convert_obj(string stringValue);
    }
}