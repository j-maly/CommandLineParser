using System;
using CommandLineParser.Arguments;

namespace CommandLineParser.Exceptions
{
    /// <summary>
    /// General command line exception
    /// </summary>
    public class CommandLineException: Exception
    {
        /// <summary>
        /// Creates new instance of CommandLineException.
        /// </summary>
        /// <param name="message">Exception message</param>
        public CommandLineException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates new instance of CommandLineException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">The exception that is the cause of the 
        /// current exception, or a null reference (Nothing in Visual Basic) if no 
        /// inner exception is specified. </param>
        public CommandLineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown when command line arguments does not followgeneral rules for 
    /// command line arguments or rules specific for the particular argument type. 
    /// </summary>
    public class CommandLineFormatException: CommandLineException
    {
        /// <summary>
        /// Creates new instance of CommandLineFormatException.
        /// </summary>
        /// <param name="message">Exception message</param>
        public CommandLineFormatException(string message):
            base(message)
        {
        }
    }

    /// <summary>
    /// Thrown when an argument is used in a wrong way
    /// </summary>
    public class CommandLineArgumentException: CommandLineException
    {
        /// <summary>
        /// Creates new instance of CommandLineArgumentException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="argument">Name of the argument</param>
        public CommandLineArgumentException(string message, string argument) : base(message)
        {
            _argument = argument;
        }

        private string _argument;

        /// <summary>
        /// Argument used in a wrong way.
        /// </summary>
        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }
    }

    /// <summary>
    /// Thrown when an <see cref="Argument"/> with <see cref="Argument.Optional"/> field 
    /// set to false is not used on the parsed command line. 
    /// </summary>
    public class MandatoryArgumentNotSetException: CommandLineArgumentException
    {
        /// <summary>
        /// Creates new instance of MandatoryArgumentNotSetException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="argument">Name of the argument</param>
        public MandatoryArgumentNotSetException(string message, string argument) : 
            base(message, argument)
        {
        }
    }

    /// <summary>
    /// Thrown when <see cref="ValueArgument{TValue}"/> value is not set on the parsed command line.
    /// </summary>
    public class MissingArgumentValueException: CommandLineArgumentException
    {
        /// <summary>
        /// Creates new instance of MissingArgumentValueException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="argument">Name of the argument</param>
        public MissingArgumentValueException(string message, string argument) : 
            base(message, argument)
        {
        }
    }

    /// <summary>
    /// Thrown when uknown argument is found on the command line
    /// </summary>
    public class UnknownArgumentException: CommandLineArgumentException
    {
        /// <summary>
        /// Creates new instance of UnknownArgumentException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="argument">Name of the argument</param>
        public UnknownArgumentException(string message, string argument) : 
            base(message, argument)
        {
        }
    }

    /// <summary>
    /// Thrown when parser failed to converse string to arguments value
    /// </summary>
    public class InvalidConversionException: CommandLineArgumentException
    {
        /// <summary>
        /// Creates new instance of InvalidConversionException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="argument">Name of the argument</param>
        public InvalidConversionException(string message, string argument) : base(message, argument)
        {
        }
    }

    /// <summary>
    /// Thrown when arguments value does not follow limitations defined by the argument.
    /// </summary>
    public class CommandLineArgumentOutOfRangeException: CommandLineArgumentException
    {
        /// <summary>
        /// Creates new instance of CommandLineArgumentOutOfRangeException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="argument">Name of the argument</param>
        public CommandLineArgumentOutOfRangeException(string message, string argument)
            : base(message, argument)
        {
        }
    }

    /// <summary>
    /// Thrown when argument defined group is not valid.
    /// </summary>
    public class InvalidArgumentGroupException: CommandLineException
    {
        /// <summary>
        /// Creates new instance of InvalidArgumentGroupException.
        /// </summary>
        /// <param name="message">reason of the exception</param>
        public InvalidArgumentGroupException(string message)
            : base(message)
        {
        }
    }

	/// <summary>
	/// Thrown when there are not all requested additional arguments found on the 
	/// command line
	/// </summary>
	public class MissingAdditionalArgumentsException: CommandLineException
	{
		/// <summary>
		/// Creates new instance of CommandLineException.
		/// </summary>
		/// <param name="message">Exception message</param>
		public MissingAdditionalArgumentsException(string message) : base(message)
		{
		}
	}
}