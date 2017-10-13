using System.Collections.Generic;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// CertifiedValueArgument is a base class for all arguments that have other limitations and restrictions (defined
    /// int <see cref="Certify"/> function) on their values apart from the value 
    /// being of a certain type (<typeparamref name="TValue"/>).
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <seealso cref="BoundedValueArgument{TValue}"/>
    /// <seealso cref="EnumeratedValueArgument{TValue}"/>
    /// <include file='..\Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/CertifiedValueArgument/*'/>
    public abstract class CertifiedValueArgument<TValue> : ValueArgument<TValue>
    {
        #region constructor

        /// <summary>
        /// Creates new certified value argument with a <see cref="Argument.ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        protected CertifiedValueArgument(char shortName) : base(shortName) { }

        /// <summary>
        /// Creates new certified value argument with a <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="longName">Long name of the argument</param>
        protected CertifiedValueArgument(string longName) : base(longName) { }

        /// <summary>
        /// Creates new certified value argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        protected CertifiedValueArgument(char shortName, string longName)
            : base(shortName, longName) { }

        /// <summary>
        /// Creates new certified value argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">description of the argument</param>
        protected CertifiedValueArgument(char shortName, string longName, string description)
            : base(shortName, longName, description) { }

        #endregion

        /// <summary>
        /// This method reads the argument and the following string representing the value of the argument. 
        /// This string is then converted to <typeparamref name="TValue"/> (using built-in <typeparamref name="TValue"/>.Parse
        /// method for built-in types or using <see cref="ValueArgument{TValue}.ConvertValueHandler"/> for user types).
        /// After successful conversion, validation <see cref="Certify"/> method is called
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="i">index to the args array, where this argument occured. The index to the next argument 
        /// after the argument is processed. </param>
        /// <seealso cref="ValueArgument{TValue}.ConvertValueHandler"/>
        public override void Parse(IList<string> args, ref int i)
        {
            base.Parse(args, ref i);
            if (AllowMultiple)
            {
                foreach (TValue val in Values)
                {
                    Certify(val);
                }
            }
            else
            {
                Certify(Value);
            }
        }

        /// <summary>
        /// Checks for argument specific 
        /// restriction and limitations. Exceptions are thrown when these are not met.
        /// </summary>
        /// <param name="value">value to certify</param>
        protected abstract void Certify(TValue value);
    }
}