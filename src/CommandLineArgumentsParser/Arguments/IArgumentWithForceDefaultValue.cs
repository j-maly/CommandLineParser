namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Implemented by arguments with default value, e.g.
    /// <see cref="SwitchArgument"/> and <see cref="ValueArgument{TValue}"/>.
    /// These arguments updates the value of bound object (<see cref="Argument.UpdateBoundObject"/>)
    /// with the value of <see cref="DefaultValue" /> 
    /// when they do not appear on the command line. 
    /// </summary>
    public interface IArgumentWithForceDefaultValue
    {
        ///<summary>
        /// Default value of the argument.
        ///</summary>
        object DefaultValue { get; }
    }
}