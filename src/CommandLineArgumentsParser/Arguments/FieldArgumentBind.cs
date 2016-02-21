namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Defines bind to the certain field of the certain object. See <see cref="Argument.Bind"/>. 
    /// Bind between object ensures that value of the argument from the command line is copied
    /// to the bound field or property.
    /// </summary>
    public struct FieldArgumentBind
    {
        /// <summary>
        /// Bound object
        /// </summary>
        public object Object;

        /// <summary>
        /// Bound field
        /// </summary>
        public string Field;

        /// <summary>
        /// Creates new instance of <see cref="FieldArgumentBind"/>.
        /// </summary>
        /// <param name="boundObject">any object whose field should be bound</param>
        /// <param name="boundField">ield of <paramref name="boundObject"/> that will be bound to the argument</param>
        public FieldArgumentBind(object boundObject, string boundField)
        {
            Object = boundObject;
            Field = boundField;
        }
    }
}